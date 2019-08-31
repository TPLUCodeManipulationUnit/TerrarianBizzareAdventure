using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Projectiles
{
    public abstract class RushPunch : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;

            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 6;

            projectile.tileCollide = false;
            projectile.aiStyle = -1;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            TBAPlayer tbaPlayer = TBAPlayer.Get(Main.player[Main.myPlayer]);

            if (!tbaPlayer.StandUser)
                return;

            SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = ModContent.GetTexture(Texture);
            float frame = Main.rand.NextBool() ? 0f : 0.5f;
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle(0, (int)(texture.Height * frame), (int)texture.Width, (int)texture.Height / 2), Color.White, projectile.velocity.ToRotation(), new Vector2(texture.Width / 2, texture.Height / 4), 0.75f, spriteEffects, 1f);
        }


        public override void AI()
        {
            float x = Main.rand.NextFloat(-10, 10);
            float y = Main.rand.NextFloat(-20, 20);

            projectile.netUpdate = true;

            projectile.ai[1] += 0.28f;
            projectile.Center = ParentProjectile.Center + projectile.velocity * projectile.ai[1] + new Vector2(x, y).RotatedBy(projectile.velocity.ToRotation());
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            target.velocity = projectile.velocity * 0.1f;
            target.immune[projectile.owner] = 4;
        }


        private Projectile ParentProjectile => Main.projectile[(int)projectile.ai[0]];


        public override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
