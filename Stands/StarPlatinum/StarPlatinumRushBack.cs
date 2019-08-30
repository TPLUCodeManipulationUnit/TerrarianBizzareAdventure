using Terraria;
using TerrarianBizzareAdventure.Projectiles;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Stands.StarPlatinum
{
    public class StarPlatinumRushBack : RushPunch, IProjectileHasImmunityToTimeStop
    {
        public bool IsImmuneToTimeStop(Projectile projectile) => projectile.owner == TimeStopManagement.TimeStopper.player.whoAmI;


        public override string Texture => "TerrarianBizzareAdventure/Stands/StarPlatinum/StarFistBack";
    }
}
