using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.TimeSkip;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Projectiles
{
    public sealed class TBAGlobalProjectile : GlobalProjectile
    {
        public override void SetDefaults(Projectile projectile)
        {
            TimeSkipStates = new List<TimeSkipState>();
        }

        public override bool PreAI(Projectile projectile)
        {
            int tickLimit = TimeStopManagement.TimeStopped && projectile.owner == TimeStopManagement.TimeStopper.player.whoAmI ? 10 : 1;
            IsStopped = TimeStopManagement.TimeStopped && !(projectile.modProjectile is IProjectileHasImmunityToTimeStop iisitts && iisitts.IsNativelyImmuneToTimeStop()) && RanForTicks > tickLimit && (!(projectile.modProjectile is Stand) && projectile.owner == TimeStopManagement.TimeStopper.player.whoAmI);


            var IsTimeSkipped = TimeSkipManager.IsTimeSkipped && projectile.hostile;

            RanForTicks++;

            if (IsTimeSkipped)
            {
                if (TimeSkipManager.TimeSkippedFor % 4 == 0)
                {
                    TimeSkipStates.Add
                        (
                            new TimeSkipState(projectile.Center, projectile.velocity, projectile.scale, projectile.rotation, new Rectangle(0, projectile.frame, 0, 0), projectile.direction, projectile.ai)
                        );
                }
            }

            if (TimeSkipStates.Count > 12)
                TimeSkipStates.RemoveAt(0);

            if(IsTimeSkipped && TimeSkipManager.TimeSkippedFor <= 2 && TimeSkipStates.Count > 0)
            {
                projectile.Center = TimeSkipStates[0].Position;
                projectile.ai = TimeSkipStates[0].AI;
                projectile.scale = TimeSkipStates[0].Scale;
                projectile.direction = TimeSkipStates[0].Direction;
            }


            if (IsTimeSkipped && RanForTicks > 2 && RanForTicks < 60)
                return false;

            if (IsStopped)
            {
                if (!TimeStopManagement.projectileStates.ContainsKey(projectile))
                    TimeStopManagement.RegisterStoppedProjectile(projectile);

                TimeStopManagement.projectileStates[projectile].PreAI(projectile);

                projectile.frameCounter = 0;

                return false;
            }
            else
            {
                return true;
            }
        }

        public override void PostDraw(Projectile projectile, SpriteBatch spriteBatch, Color lightColor)
        {
            var IsTimeSkipped = TimeSkipManager.IsTimeSkipped && projectile.hostile;

            if (IsTimeSkipped)
                lightColor = Color.Red;

            if (IsTimeSkipped)
            {
                Texture2D texture = Main.projectileTexture[projectile.type];
                int frameCount = Main.projFrames[projectile.type];
                int frameHeight = texture.Height / frameCount;

                Vector2 drawOrig = new Vector2(texture.Width * 0.5f, (texture.Height / frameCount) * 0.5f);

                for (int i = TimeSkipStates.Count - 1; i > 0; i--)
                {
                    SpriteEffects spriteEffects = TimeSkipStates[i].Direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    spriteBatch.Draw(texture, TimeSkipStates[i].Position - Main.screenPosition, new Rectangle(0, TimeSkipStates[i].Frame.Y * frameHeight, texture.Width, frameHeight), (i == 1 ? lightColor : Color.Red * 0.5f), TimeSkipStates[i].Rotation, drawOrig, TimeSkipStates[i].Scale, spriteEffects, 1f);
                }
            }
        }

        public override bool ShouldUpdatePosition(Projectile projectile)
        {
            bool notMove = TimeSkipManager.IsTimeSkipped && RanForTicks > 2 && RanForTicks < 60;

            if (notMove)
                return false;

            return !IsStopped;
        }

        public bool IsStopped { get; private set; }

        public int RanForTicks { get; private set; }

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;


        public List<TimeSkipState> TimeSkipStates { get; private set; }
    }
}