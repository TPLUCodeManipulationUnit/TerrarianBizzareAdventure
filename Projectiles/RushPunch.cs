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
            projectile.timeLeft = 12;

            projectile.tileCollide = false;
            projectile.aiStyle = -1;

            Opacity = 0.7f;

            Frame = -1f;
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
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle(0, (int)(texture.Height * Frame), (int)texture.Width, (int)texture.Height / 2), Color.White * Opacity, projectile.velocity.ToRotation(), new Vector2(texture.Width / 2, texture.Height / 4), 1f, spriteEffects, 1f);
        }


        public override void AI()
        {
            if (projectile.timeLeft >= 11)
            {
                projectile.netUpdate = true;
                projectile.netUpdate2 = true;
            }

            if(Frame == -1)
                Frame = Main.rand.NextBool() ? 0f : 0.5f;

            if(SpeedMultiplier < 3f)
            SpeedMultiplier += 0.5f;

            Opacity -= SpeedMultiplier >= 3.0f ? 0.1f : 0.01f;

            var xx = (Main.projectile[ParentProjectile].Center + projectile.velocity * SpeedMultiplier).X;
            var yy = (Main.projectile[ParentProjectile].Center + projectile.velocity * SpeedMultiplier).Y;

            projectile.Center = new Vector2(xx, yy) + Offset.RotatedBy(projectile.velocity.ToRotation());
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!IsFinalPunch)
                target.velocity *= 0f;
            else
                target.velocity = projectile.velocity * 2f;

            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Punch" + Main.rand.Next(1, 5)).WithVolume(.2f));
            target.immune[projectile.owner] = 4;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(SpeedMultiplier);
            writer.Write(ParentProjectile);
            writer.Write(IsFinalPunch);
            writer.Write(Offset.X);
            writer.Write(Offset.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SpeedMultiplier = reader.ReadSingle();
            ParentProjectile = reader.ReadInt32();
            IsFinalPunch = reader.ReadBoolean();
            Offset = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public float SpeedMultiplier { get; private set; }

        public int ParentProjectile { get; set; }

        public bool IsFinalPunch { get; set; }

        public Vector2 Offset { get; set; }

        public float Opacity { get; private set; }

        public float Frame { get; private set; }

        public override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
