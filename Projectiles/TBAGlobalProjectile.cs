using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Projectiles
{
    public sealed class TBAGlobalProjectile : GlobalProjectile
    {
        public override bool PreAI(Projectile projectile)
        {
            int tickLimit = TimeStopManagement.TimeStopped && projectile.owner == TimeStopManagement.TimeStopper.player.whoAmI ? 10 : 1;
            IsStopped = TimeStopManagement.TimeStopped && !(projectile.modProjectile is IProjectileHasImmunityToTimeStop iisitts && iisitts.IsImmuneToTimeStop(projectile)) && RanForTicks > tickLimit;

            if (IsStopped)
            {
                if (!TimeStopManagement.projectileStates.ContainsKey(projectile))
                    TimeStopManagement.RegisterStoppedProjectile(projectile);

                TimeStopManagement.projectileStates[projectile].PreAI(projectile);

                return false;
            }
            else
            {
                RanForTicks++;
                return true;
            }
        }

        public override bool ShouldUpdatePosition(Projectile projectile) => !IsStopped;


        public bool IsStopped { get; private set; }

        public int RanForTicks { get; private set; }

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
    }
}