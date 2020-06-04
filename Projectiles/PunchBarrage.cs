using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WebmilioCommons.Projectiles;
using Terraria.ModLoader;
using System.Collections.Generic;
using TerrarianBizzareAdventure.TimeStop;
using System.IO;

namespace TerrarianBizzareAdventure.Projectiles
{
    public abstract class PunchBarrage : StandardProjectile, IProjectileHasImmunityToTimeStop
    {
        private const string UNSPECIFIED_PATH =
            "none";

        /// <summary>
        /// Le Puncho De Barragee
        /// </summary>
        /// <param name="path">Path for main texture</param>
        /// <param name="secondaryPath">Path for back texture</param>
        public PunchBarrage(string path, string secondaryPath = UNSPECIFIED_PATH)
        {
            TexturePath = path;

            SecondaryPath = secondaryPath;

            if (secondaryPath == UNSPECIFIED_PATH)
                SecondaryPath = TexturePath;
        }

        public override void SetDefaults()
        {
            // i'll fuck around with those values until i found a good hitbox size
            Width = Height = 60;
            Penetrate = -1;
            TimeLeft = 190;
            projectile.friendly = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Velocity = RushDirection;

            if (TimeLeft > 188)
                projectile.netUpdate = true;

            Center = Main.projectile[ParentProjectile].Center + Velocity;

            if (FrontPunches.Count <= 4)
            {
                float x = -Main.rand.NextFloat(26f, 50f);
                float y = Main.rand.NextFloat(-24f, 24f);
                FrontPunches.Add(new Vector2(x, y));

            }

            if (BackPunches.Count <= 4)
            {
                float x = -Main.rand.NextFloat(26f, 38f);
                float y = Main.rand.NextFloat(-24f, 24f);
                BackPunches.Add(new Vector2(x, y));
            }

            for (int i = FrontPunches.Count - 1; i > -1; i--)
            {
                FrontPunches[i] += new Vector2(5.6f, 0);

                if (FrontPunches[i].X >= 16f)
                    FrontPunches.RemoveAt(i);


            }

            for (int i = BackPunches.Count - 1; i > -1; i--)
            {
                BackPunches[i] += new Vector2(5.6f, 0);

                if (BackPunches[i].X >= 16f)
                    BackPunches.RemoveAt(i);
            }
        }

        // In ideal world, i wouldn't need to sync things, but we live in a shitty world so rip
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(RushDirection.X);
            writer.Write(RushDirection.Y);

                writer.Write(ParentProjectile);

        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            RushDirection = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            ParentProjectile = reader.ReadInt32();
        }

        // Have I ever told you guys that vanilla drawing sucks balls? Well it does :)
        // So i'm gonna remove it entirely
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        //The fun begins kekW
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("TerrarianBizzareAdventure/" + TexturePath);

            Texture2D alTtexture = ModContent.GetTexture("TerrarianBizzareAdventure/" + SecondaryPath);

            Rectangle sourceRect = new Rectangle(0, (int)(texture.Height * 0.5f), (int)texture.Width, (int)texture.Height / 2);

            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 4);

            foreach (Vector2 distance in FrontPunches)
            {
                spriteBatch.Draw(texture, Center + distance.RotatedBy(Velocity.ToRotation()) - Main.screenPosition, sourceRect, Color.White * 0.75f, projectile.velocity.ToRotation(), origin, 1.1f, SpriteEffects.FlipHorizontally, 1f);
            }

            foreach (Vector2 distance in BackPunches)
            {
                spriteBatch.Draw(alTtexture, Center + distance.RotatedBy(Velocity.ToRotation()) - Main.screenPosition, sourceRect, Color.White * 0.75f, projectile.velocity.ToRotation(), origin, 1.1f, SpriteEffects.FlipHorizontally, 1f);
            }
        }

        public bool IsNativelyImmuneToTimeStop() => TimeStopManagement.TimeStopped && TimeStopManagement.TimeStopper.player.whoAmI == Owner.whoAmI;

        public List<Vector2> FrontPunches { get; } = new List<Vector2>();

        public List<Vector2> BackPunches { get; } = new List<Vector2>();

        // We'll also need to know where stand is barraging towards
        public Vector2 RushDirection { get; set; }

        // We'll need to keep track of Stand thats doing the barrage
        public int ParentProjectile { get; set; }

        public string TexturePath { get; }
        public string SecondaryPath { get; }

        // We do not need the texture really, but terraria is a boomer and makes a fuss
        public override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
