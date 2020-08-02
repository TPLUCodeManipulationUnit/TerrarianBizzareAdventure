using System.Security.Policy;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Buffs;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public void ResetBuffEffects()
        {
            if (Tired && player.HasBuff(ModContent.BuffType<TiredDebuff>()))
                StaminaRegenBuff -= player.buffTime[player.FindBuffIndex(ModContent.BuffType<TiredDebuff>())];

            if (Exhausted)
            {
                StaminaRegenTicks = 0;
            }

            Exhausted = false;
            Tired = false;
            ShatteredTime = false;
        }

        public bool Exhausted { get; set; }

        public bool Tired { get; set; }

        public bool ShatteredTime { get; set; }
    }
}
