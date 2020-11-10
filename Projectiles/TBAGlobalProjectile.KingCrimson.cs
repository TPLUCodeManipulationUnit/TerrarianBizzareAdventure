using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.TimeSkip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TerrarianBizzareAdventure.Projectiles
{
    public sealed partial class TBAGlobalProjectile : GlobalProjectile
    {
        public void PreTimeSkipAI(Projectile projectile)
        {
            if (IsAffectedByKC(projectile))
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


            if (IsAffectedByKC(projectile) && TimeSkipManager.TimeSkippedFor <= 2 && TimeSkipStates.Count > 0)
            {
                projectile.Center = TimeSkipStates[0].Position;
                projectile.ai = TimeSkipStates[0].AI;
                projectile.scale = TimeSkipStates[0].Scale;
                projectile.direction = TimeSkipStates[0].Direction;
            }
        }

        public bool IsAffectedByKC(Projectile projectile) => TimeSkipManager.IsTimeSkipped && projectile.hostile && !(projectile.modProjectile is PunchBarrage);


#pragma warning disable IDE0060 // Remove unused parameter
        public void PostTimeSkipDraw(Projectile projectile, SpriteBatch spriteBatch, Color lightColor)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (IsAffectedByKC(projectile))
            {
                lightColor = Color.Red;

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
    }
}
