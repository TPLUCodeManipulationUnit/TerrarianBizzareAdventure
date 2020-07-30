using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Projectiles;

namespace TerrarianBizzareAdventure.Stands.GoldenWind.Aerosmith
{
    public class AerosmithBullet : StandardProjectile
    {
        public override void SetDefaults()
        {
            aiType = ProjectileID.Bullet;

            Width = 4;
            Height = 4;
            TimeLeft = 120;

            projectile.extraUpdates = 1;
            projectile.friendly = true;
        }

        // Can't be bothered to use dumb vanilla drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!TBAPlayer.Get(Main.LocalPlayer).StandUser)
                return;


            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 position = Center - Main.screenPosition;

            float rotation = Velocity.ToRotation() + MathHelper.Pi / 2;
            float opacity = TimeLeft > 116 ? 0f : 1f;

            spriteBatch.Draw(texture, position, null, Color.White * opacity, rotation, new Vector2(1), 1f, SpriteEffects.None, 1f);
        }

        public override string Texture => "TerrarianBizzareAdventure/Stands/GoldenWind/Aerosmith/Bullet";
    }
}
