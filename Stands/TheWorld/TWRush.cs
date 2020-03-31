using Terraria;
using TerrarianBizzareAdventure.Projectiles;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Stands.TheWorld
{
    public class TWRush : RushPunch, IProjectileHasImmunityToTimeStop
    {
        public bool IsNativelyImmuneToTimeStop() => projectile.owner == TimeStopManagement.TimeStopper.player.whoAmI;


        public override string Texture => "TerrarianBizzareAdventure/Stands/TheWorld/TWRush";
    }
}
