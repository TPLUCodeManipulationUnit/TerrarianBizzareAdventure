using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using TerrarianBizzareAdventure.TimeStop;
using WebmilioCommons.Projectiles;

namespace TerrarianBizzareAdventure.Stands.StardustCrusaders.TheWorld
{
    public class Knife : StandardProjectile, IProjectileHasImmunityToTimeStop
    {
        public override void SetDefaults()
        {
            Width = Height = 8;
            projectile.friendly = true;
        }

        public override void AI()
        {
            projectile.rotation = Velocity.ToRotation();

            if (ShouldStop)
                projectile.timeLeft++;

            if(TimeStopManagement.TimeStopped && !ShouldStop)
            {
                foreach(NPC npc in Main.npc)
                {
                    if (!npc.active || !npc.CanBeChasedBy(this))
                        continue;

                    if (VelocityAdjustedHitbox.Intersects(npc.Hitbox))
                        ShouldStop = true;
                }
            }
        }

        public override bool ShouldUpdatePosition() => !ShouldStop || !TimeStopManagement.TimeStopped;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects fx = Reversed ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation + MathHelper.Pi / 2, new Vector2(4, 16), 1f, fx, 1f);
        }

        public bool IsNativelyImmuneToTimeStop() => true;

        public Rectangle VelocityAdjustedHitbox =>
            new Rectangle(projectile.Hitbox.X + (int)Velocity.X * 2,
                projectile.Hitbox.Y + (int)Velocity.Y * 2, 
                projectile.Hitbox.Width, projectile.Hitbox.Height);

        public bool Reversed { get; } = Main.rand.NextBool();

        public bool ShouldStop { get; set; }
    }
}
