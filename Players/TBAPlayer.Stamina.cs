using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public void ResetStaminaEffects()
        {
            if (ActiveStandProjectileId == ACTIVE_STAND_PROJECTILE_INACTIVE_ID)
            {
                StaminaRegenTicks++;

                if (StaminaRegenTicks >= StaminaRegenTickRate)
                {
                    Stamina += 1;
                    StaminaRegenTicks = 0;
                }
            }

            if (Stamina >= MaxStamina)
                StaminaRegenTicks = 0;

            StaminaRegenBuff = 0;
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

        public int StaminaRegenTickRate => 240 - StaminaRegenBuff;

        public int StaminaRegenBuff { get; set; }
    }
}
