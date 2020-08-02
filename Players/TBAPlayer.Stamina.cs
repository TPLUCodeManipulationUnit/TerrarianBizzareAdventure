using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Buffs;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public void ResetStaminaEffects()
        {
            var staminaDebuff = 0;

            if (StandActive)
                staminaDebuff -= 8 * Constants.TICKS_PER_SECOND;

            StaminaRegenTicks++;

            if (StaminaRegenTicks >= StaminaRegenTickRate)
            {
                Stamina += StaminaGain;
                StaminaRegenTicks = 0;
            }

            if (Stamina >= MaxStamina)
                StaminaRegenTicks = 0;

            int bonus = (player.statLifeMax - 200) + (player.statLife - player.statLifeMax);

            if (bonus > 210)
                bonus = 210;

            ResultingRegen = bonus + staminaDebuff + StaminaRegenBuff;

            StaminaRegenBuff = 0;

            if (IsDebugging)
                Stamina = MaxStamina;
        }

        public bool CheckStaminaCost(int cost, bool forceSpend = false)
        {
            if (IsDebugging)
                return true;

            if (forceSpend)
            {
                int cStamina = Stamina;
                int duration = cStamina - cost;

                if (duration < 0)
                {
                    player.AddBuff(ModContent.BuffType<TiredDebuff>(), (1 + Math.Abs(duration)) * Constants.TICKS_PER_SECOND);
                    player.AddBuff(ModContent.BuffType<ExhaustedDebuff>(), (1 + Math.Abs(duration)) * Constants.TICKS_PER_SECOND);
                }

                Stamina -= cost;
                return true;
            }

            if (Stamina >= cost)
            {
                Stamina -= cost;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds "Tired" debuff to player
        /// </summary>
        /// <param name="Duration">Duration in seconds</param>
        public void TirePlayer(int Duration)
        {
            player.AddBuff(ModContent.BuffType<TiredDebuff>(), Duration * Constants.TICKS_PER_SECOND);
        }

        private int _stamina;
        public int Stamina
        {
            get => _stamina;
            set => _stamina = (int)MathHelper.Clamp(value, 0, MaxStamina);
        }

        public int MaxStamina
        {
            get;
            set;
        }

        public int StaminaRegenTicks { get; private set; }

        public int StaminaRegenTickRate => 4 * Constants.TICKS_PER_SECOND - ResultingRegen;

        public int StaminaRegenBuff { get; set; }

        public int ResultingRegen { get; set; }

        public int StaminaGain => 1 + StaminaGainBonus;

        public int StaminaGainBonus { get; set; }

        public bool IsDebugging { get; set; }
    }
}
