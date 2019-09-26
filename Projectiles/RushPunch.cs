using System.IO;
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
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle(0, (int)(texture.Height * frame), (int)texture.Width, (int)texture.Height / 2), Color.White, projectile.velocity.ToRotation(), new Vector2(texture.Width / 2, texture.Height / 4), 1f, spriteEffects, 1f);
        }


        public override void AI()
        {
            projectile.netUpdate = true;
            projectile.netUpdate2 = true;

            SpeedMultiplier += 0.28f;

            Vector2 bullshitVector = Main.projectile[ParentProjectile].Center + projectile.velocity * SpeedMultiplier;

            float x = Main.rand.NextFloat(-10, 10);
            float y = Main.rand.NextFloat(-20, 20);

            projectile.ai[0] = (Main.projectile[ParentProjectile].Center + projectile.velocity * SpeedMultiplier).X;
            projectile.ai[1] = (Main.projectile[ParentProjectile].Center + projectile.velocity * SpeedMultiplier).Y;
            projectile.Center = new Vector2(projectile.ai[0], projectile.ai[1]) + new Vector2(x, y).RotatedBy(projectile.velocity.ToRotation());
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if(target.CanBeChasedBy(this))
                target.velocity = projectile.velocity * 0.1f;

            target.immune[projectile.owner] = 4;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(SpeedMultiplier);
            writer.Write(ParentProjectile);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SpeedMultiplier = reader.ReadSingle();
            ParentProjectile = reader.ReadInt32();
        }

        public float SpeedMultiplier { get; private set; }

        public int ParentProjectile { get; set; }


        public override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
