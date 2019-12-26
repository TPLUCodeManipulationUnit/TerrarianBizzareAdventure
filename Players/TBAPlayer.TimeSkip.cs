using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.TimeSkip;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (TimeSkipManager.IsTimeSkipped && TimeSkipManager.TimeSkipper.player.whoAmI == player.whoAmI)
                return false;

            return base.CanBeHitByNPC(npc, ref cooldownSlot);
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (TimeSkipManager.IsTimeSkipped && TimeSkipManager.TimeSkipper.player.whoAmI == player.whoAmI)
                return false;

            return base.CanBeHitByProjectile(proj);
        }
    }
}
