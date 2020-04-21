using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public void ResetStaminaEffects()
        {
            var staminaDebuff = 0;
            if (ActiveStandProjectileId != ACTIVE_STAND_PROJECTILE_INACTIVE_ID)
                staminaDebuff -= 8 * Constants.TICKS_PER_SECOND;

            StaminaRegenTicks++;

            if (StaminaRegenTicks >= StaminaRegenTickRate)
            {
                Stamina += 1;
                StaminaRegenTicks = 0;
            }

            if (Stamina >= MaxStamina)
                StaminaRegenTicks = 0;

            int bonus = (player.statLifeMax - 200) + (player.statLife - player.statLifeMax) + StaminaRegenBuff;

            if (bonus > 210)
                bonus = 210;

            StaminaRegenBuff = 0;

            ResultingRegen = bonus + staminaDebuff;

            if (IsDebugging)
                Stamina = MaxStamina;
        }

        public bool CheckStaminaCost(int cost)
        {
            if (Stamina >= cost)
            {
                Stamina -= cost;
                return true;
            }

            return false;
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

        public bool IsDebugging { get; set; }
    }
}
