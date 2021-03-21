using Terraria;
using Terraria.ID;
using WebmilioCommons.Projectiles;

namespace TerrarianBizzareAdventure.Stands.GoldenWind.KingCrimson
{
    public class BloodSplat : StandardProjectile
    {
        public override void SetDefaults()
        {
            TimeLeft = 20;
            Width = Height = 16;
            projectile.friendly = true;
        }
        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                int bloodDust = Dust.NewDust(Center, 0, 0, DustID.Blood);
                Main.dust[bloodDust].scale = Main.rand.NextFloat(10.0f, 15.0f) / 10.0f;
                Main.dust[bloodDust].velocity *= -1.25f;
            }

            projectile.velocity.X *= 0.96f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.life++;
            target.AddBuff(BuffID.Confused, 300);
        }

        public override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
