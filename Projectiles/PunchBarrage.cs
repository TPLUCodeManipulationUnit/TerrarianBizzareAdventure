using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WebmilioCommons.Projectiles;
using Terraria.ModLoader;
using System.Collections.Generic;
using TerrarianBizzareAdventure.TimeStop;
using System.IO;
using Terraria.ID;
using System.Linq;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using System;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.NPCs;
using WebmilioCommons.Extensions;

namespace TerrarianBizzareAdventure.Projectiles
{
    public abstract class PunchBarrage : StandardProjectile, IProjectileHasImmunityToTimeStop
    {
        private const string UNSPECIFIED_PATH = "none";

        /// <summary>
        /// Le Puncho De Barragee
        /// </summary>
        /// <param name="path">Path for main texture</param>
        /// <param name="secondaryPath">Path for back texture</param>
        protected PunchBarrage(string path, string secondaryPath = UNSPECIFIED_PATH)
        {
            var barrageType = GetType();
            TexturePath = $"{barrageType.GetRootPath()}/{path}";

            if (secondaryPath == UNSPECIFIED_PATH)
                SecondaryPath = TexturePath;
            else
                SecondaryPath = $"{barrageType.GetRootPath()}/{secondaryPath}";
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
            {
                TBAPlayer.Get(Owner).PointOfInterest = Owner.Center;
                projectile.netUpdate = true;
            }

            Vector2 pov = TBAPlayer.Get(Owner).PointOfInterest;

            if(TimeLeft > 12)
                TBAPlayer.Get(Owner).PointOfInterest = Vector2.Lerp(pov, Center + RandomScreenOffset, 0.25f);
            else
                TBAPlayer.Get(Owner).PointOfInterest = Vector2.Lerp(pov, Owner.Center + RandomScreenOffset, 0.25f);

            Center = Main.projectile[ParentProjectile].Center + Velocity;

            RandomScreenOffset = Vector2.Lerp(RandomScreenOffset, Vector2.Zero, 0.12f);

            if (!(Parent.modProjectile is Stand))
                projectile.Kill();

            if (FrontPunches.Count <= 4)
            {
                float x = -Main.rand.NextFloat(26f, 50f);
                float y = Main.rand.NextFloat(-24f, 24f);

                FrontPunches.Add(new Vector3(x, y, Main.rand.Next(2)));
            }

            if (BackPunches.Count <= 4)
            {
                float x = -Main.rand.NextFloat(26f, 38f);
                float y = Main.rand.NextFloat(-24f, 24f);

                BackPunches.Add(new Vector3(x, y, Main.rand.Next(2)));
            }

            for (int i = FrontPunches.Count - 1; i > -1; i--)
            {
                FrontPunches[i] += new Vector3(5.6f, 0, 0);

                if (FrontPunches[i].X >= 16f)
                    FrontPunches.RemoveAt(i);
            }

            for (int i = BackPunches.Count - 1; i > -1; i--)
            {
                BackPunches[i] += new Vector3(5.6f, 0, 0);

                if (BackPunches[i].X >= 16f)
                    BackPunches.RemoveAt(i);
            }

            for(int n = HitNPCs.Count - 1; n > 0; n--)
                if (!TBAGlobalNPC.GetFor(HitNPCs[n]).IsCombatLocked)
                    HitNPCs.RemoveAt(n);
        }

        public override void Kill(int timeLeft)
        {
            TBAPlayer.Get(Owner).PointOfInterest = Vector2.Zero;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Owner.whoAmI] = 6;

            RandomScreenOffset = new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-40, 30));

            for (int i = 0; i < 2; i++)
            {
                int rand = Main.rand.Next(FrontPunches.Count);
                var punch = FrontPunches[rand];

                Vector2 randomVector = new Vector2(punch.X, punch.Y);

                DrawHelpers.CircleDust(target.Center + randomVector * Owner.direction, Velocity, DustID.AncientLight, 2, 8, 0.85f);
            }

            TBAGlobalNPC.GetFor(target).CombatLockTimer = 8;

            TBAPlayer.Get(Owner).CombatLockTimer = 8;

            if (!HitNPCs.Contains(target))
            {
                TBAGlobalNPC.GetFor(target).RotationToRestore = target.rotation;
                HitNPCs.Add(target);
            }

            int multiplier = Main.rand.NextBool() ? -1 : 1;

            target.rotation = RushDirection.ToRotation() + MathHelper.PiOver2 + Main.rand.NextFloat(45.0f) * 0.01f * multiplier;

            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Punch" + Main.rand.Next(1, 5)).WithVolume(.4f));
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
            Texture2D
                texture = TBAMod.Instance.GetTexture(TexturePath),
                altTexture = TBAMod.Instance.GetTexture(SecondaryPath);

            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 4);

            foreach (Vector3 distance in FrontPunches)
            {
                Vector2 dist = new Vector2(distance.X, distance.Y);
                Rectangle sourceRect = new Rectangle(0, (int)(texture.Height * 0.5f) * (int)distance.Z, (int)texture.Width, (int)texture.Height / 2);
                spriteBatch.Draw(texture, Center + dist.RotatedBy(Velocity.ToRotation()) - Main.screenPosition, sourceRect, Color.White * 0.75f, projectile.velocity.ToRotation(), origin, 1.1f, SpriteEffects.FlipHorizontally, 1f);
            }

            foreach (Vector3 distance in BackPunches)
            {
                Vector2 dist = new Vector2(distance.X, distance.Y);
                Rectangle sourceRect = new Rectangle(0, (int)(texture.Height * 0.5f) * (int)distance.Z, (int)texture.Width, (int)texture.Height / 2);
                spriteBatch.Draw(altTexture, Center + dist.RotatedBy(Velocity.ToRotation()) - Main.screenPosition, sourceRect, Color.White * 0.75f, projectile.velocity.ToRotation(), origin, 1.1f, SpriteEffects.FlipHorizontally, 1f);
            }
        }

        public bool IsNativelyImmuneToTimeStop() => TimeStopManagement.TimeStopped && TimeStopManagement.TimeStopper.player.whoAmI == Owner.whoAmI;

        public List<Vector3> FrontPunches { get; } = new List<Vector3>();

        public List<Vector3> BackPunches { get; } = new List<Vector3>();

        public Vector2 RandomScreenOffset { get; set; }

        // We'll also need to know where stand is barraging towards
        public Vector2 RushDirection { get; set; }

        // We'll need to keep track of Stand thats doing the barrage
        // Why won't we use a Projectile instead of a integer?
        // I'm glad you asked. 
        // On the server it would be null meaning packet cannot be sent, therefore causing an error :)
        public int ParentProjectile { get; set; }

        public Projectile Parent => Main.projectile[ParentProjectile];

        public string TexturePath { get; }
        public string SecondaryPath { get; }

        public List<NPC> HitNPCs { get; } = new List<NPC>();

        // We do not need the texture really, but terraria is a boomer and makes a fuss
        public override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
