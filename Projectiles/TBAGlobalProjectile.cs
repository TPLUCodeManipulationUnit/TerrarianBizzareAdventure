using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Projectiles
{
    public sealed class TBAGlobalProjectile : GlobalProjectile
    {
        /*public override bool PreAI(Projectile projectile)
        {
            IsStopped = TimeStopManagement.TimeStopped;

            if (IsStopped)
            {
                if (!TimeStopManagement.projectileStates.ContainsKey(projectile))
                    TimeStopManagement.RegisterStoppedProjectile(projectile);

                if (projectile.owner == Main.myPlayer)
                {
                    projectile.aiStyle = 0;

                    if (projectile.velocity.LengthSquared() < 0.01f)
                        projectile.velocity = Vector2.Zero;
                    else
                    {
                        projectile.velocity /= 2;
                        return true;
                    }
                }
                else
                    projectile.velocity = Vector2.Zero;

                projectile.damage = 0;

                if (projectile.velocity == Vector2.Zero)
                {
                    projectile.frame = TimeStopManagement.projectileStates[projectile].Frame;
                    projectile.frameCounter = TimeStopManagement.projectileStates[projectile].FrameCounter;

                    projectile.ai = TimeStopManagement.projectileStates[projectile].AI;
                }

                return false;
            }

            return true;
        }*/

        public override bool PreAI(Projectile projectile)
        {
            IsStopped = TimeStopManagement.TimeStopped && !(projectile.modProjectile is IProjectileHasImmunityToTimeStop iisitts && iisitts.IsImmuneToTimeStop(projectile)) && RanForTicks > 1;

            if (IsStopped)
            {
                if (!TimeStopManagement.projectileStates.ContainsKey(projectile))
                    TimeStopManagement.RegisterStoppedProjectile(projectile);

                ProjectileInstantState state = TimeStopManagement.projectileStates[projectile];

                projectile.damage = 0;

                projectile.frameCounter = state.FrameCounter;
                projectile.frame = state.Frame;

                projectile.timeLeft++;

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
    }
}