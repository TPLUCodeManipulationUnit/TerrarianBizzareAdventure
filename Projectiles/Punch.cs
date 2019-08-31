using Terraria;
using Terraria.ModLoader;

namespace TerrarianBizzareAdventure.Projectiles
{
    public class Punch : ModProjectile
    {
        public sealed override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;

            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 4;

            projectile.tileCollide = false;
            projectile.aiStyle = -1;
        }

        public override void AI()
        {
            projectile.netUpdate = true;

            projectile.Center = ParentProjectile.Center + projectile.velocity;
        }

        private Projectile ParentProjectile => Main.projectile[(int)projectile.ai[0]];
    }
}
