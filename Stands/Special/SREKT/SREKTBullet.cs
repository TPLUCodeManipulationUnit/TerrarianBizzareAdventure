using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TerrarianBizzareAdventure.Networking;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.UserInterfaces;
using TerrarianBizzareAdventure.UserInterfaces.Elements.Misc;
using WebmilioCommons.Projectiles;
using WebmilioCommons.Networking;
using WebmilioCommons.Extensions;
using System.IO;

namespace TerrarianBizzareAdventure.Stands.Special.SREKT
{
    public class SREKTBullet : StandardProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SCAR-20");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.BulletHighVelocity);
            projectile.ignoreWater = true;

            projectile.penetrate = -1;

            Width = Height = 8;

            NoScope = Main.rand.Next(5) == 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.tileCollide = false;

            Position = projectile.oldPosition;

            WallBang = true;

            Velocity = oldVelocity;

            return false;
        }

        public override void AI()
        {
            projectile.netUpdate = true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Main.rand.Next(3) == 0)
            {
                crit = false;
                Headshot = true;
                damage = 6000;
            }

            projectile.damage = 0;
            TimeLeft = 2;

            if (target.life - damage <= 0)
            {
                UIManager.ResourcesLayer?.State?.Entries.Add(new SREKTFeedEntry(Owner.name, target.FullName, NoScope, Headshot, WallBang));
            }
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.Next(3) == 0)
            {
                crit = false;
                Headshot = true;
                damage = 6000;
            }

            projectile.damage = 0;
            TimeLeft = 2;

            if (target.statLife - damage <= 0)
            {
                UIManager.ResourcesLayer?.State?.Entries.Add(new SREKTFeedEntry(Owner.name, target.name, NoScope, Headshot, WallBang));
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Headshot);
            writer.Write(NoScope);
            writer.Write(WallBang);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Headshot = reader.ReadBoolean();
            NoScope = reader.ReadBoolean();
            WallBang = reader.ReadBoolean();
        }
        public bool Headshot { get; set; }

        public bool WallBang { get; set; }

        public bool NoScope { get; set; }

        public sealed override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
