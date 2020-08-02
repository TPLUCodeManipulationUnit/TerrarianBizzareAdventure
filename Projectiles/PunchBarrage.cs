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

            CanClash = true;
        }

        public override void AI()
        {
            Velocity = RushDirection;

            if (TimeLeft > 188)
                projectile.netUpdate = true;

            Center = Main.projectile[ParentProjectile].Center + Velocity;

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
        }

        public override void PostAI()
        {/*
            if (CanDeflectProjectiles)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (!proj.active || proj == projectile || (proj.friendly && proj.owner == Owner.whoAmI))
                        continue;

                    if (proj.Hitbox.Intersects(projectile.Hitbox))
                    {
                        proj.velocity = -proj.velocity;
                        proj.friendly = true;
                        proj.hostile = false;
                    }
                }
            }*/
             /*
            if (CanClash)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (!proj.active || proj == projectile || proj.owner == Owner.whoAmI || !(proj.modProjectile is PunchBarrage))
                        continue;

                    if (proj.Hitbox.Intersects(projectile.Hitbox))
                    {
                        CanClash = false;

                        ClashingWith = proj.whoAmI;
                    }
                }
            }

            if(ClashingWith != -1)
            {
                projectile.timeLeft = 4;

                if(TBAPlayer.Get(Owner).ActiveStandProjectile is PunchBarragingStand punchyBoi)
                {
                    punchyBoi.RushTimer = 4;
                }

                if(Main.myPlayer == Owner.whoAmI && TBAInputs.ContextAction.JustPressed)
                {
                    projectile.netUpdate = true;
                    ClashPower++;
                }

                if (Main.projectile[ClashingWith].modProjectile is PunchBarrage enemyBarrage)
                {
                    RushDirection = (enemyBarrage.Parent.Center - Center).SafeNormalize(-Vector2.UnitY) * 16f;

                    if (ClashPower > 5)
                    {
                        enemyBarrage.Kill(0);

                        TBAPlayer.Get(enemyBarrage.Owner).Stamina -= 25;
                        TBAPlayer.Get(enemyBarrage.Owner).ActiveStandProjectile.CurrentState = "DESPAWN";
                        ClashingWith = -1;
                    }

                    if(enemyBarrage.ClashPower > 5)
                    {
                        projectile.Kill();

                        TBAPlayer.Get(Owner).Stamina -= 25;
                        TBAPlayer.Get(Owner).ActiveStandProjectile.CurrentState = "DESPAWN";
                        ClashingWith = -1;

                    }
                }
            }*/
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Owner.whoAmI] = 6;

            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Punch" + Main.rand.Next(1, 5)).WithVolume(.2f));
        }

        // In ideal world, i wouldn't need to sync things, but we live in a shitty world so rip
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(RushDirection.X);
            writer.Write(RushDirection.Y);

            writer.Write(ParentProjectile);

            writer.Write(CanClash);

            writer.Write(ClashingWith);

            writer.Write(ClashPower);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            RushDirection = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            ParentProjectile = reader.ReadInt32();

            CanClash = reader.ReadBoolean();

            ClashingWith = reader.ReadInt32();

            ClashPower = reader.ReadInt32();
        }

        // Have I ever told you guys that vanilla drawing sucks balls? Well it does :)
        // So i'm gonna remove it entirely
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        //The fun begins kekW
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("TerrarianBizzareAdventure/" + TexturePath);

            Texture2D alTtexture = ModContent.GetTexture("TerrarianBizzareAdventure/" + SecondaryPath);


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
                spriteBatch.Draw(alTtexture, Center + dist.RotatedBy(Velocity.ToRotation()) - Main.screenPosition, sourceRect, Color.White * 0.75f, projectile.velocity.ToRotation(), origin, 1.1f, SpriteEffects.FlipHorizontally, 1f);
            }
        }

        public bool IsNativelyImmuneToTimeStop() => TimeStopManagement.TimeStopped && TimeStopManagement.TimeStopper.player.whoAmI == Owner.whoAmI;

        public List<Vector3> FrontPunches { get; } = new List<Vector3>();

        public List<Vector3> BackPunches { get; } = new List<Vector3>();

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

        private bool _canClash;
        public bool CanClash 
        {
            get => _canClash;
            set
            {
                if (_canClash != value)
                    projectile.netUpdate = true;

                _canClash = value;
            }
        }

        public int ClashingWith { get; set; } = -1;

        public int ClashPower { get; set; }

        public virtual bool CanDeflectProjectiles => true;

        // We do not need the texture really, but terraria is a boomer and makes a fuss
        public override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
