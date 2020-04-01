using Terraria.ModLoader;
using Terraria;
using Terraria.Graphics.Effects;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Projectiles.Misc
{
    public sealed class TimeStopVFX : ModProjectile, IProjectileHasImmunityToTimeStop
    {
        private const int
            RIPPLE_COUNT = 10,
            RIPPLE_SIZE = 1;
        
        private const float
            RIPPLE_SPEED = 2.25f,
            DISTORT_STRENGTH = 10500f;

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.light = 0.9f;
            projectile.penetrate = -1;
            projectile.timeLeft = 90;
            projectile.friendly = true;
            projectile.tileCollide = false;
            aiType = 24;
        }

        public override void AI()
        {
            if (Main.dedServ)
                return;

            projectile.position = Main.player[projectile.owner].position;

            if (!Filters.Scene["Shockwave"].IsActive())
            {
                Filters.Scene.Activate("Shockwave", projectile.Center).GetShader().UseColor(RIPPLE_COUNT, RIPPLE_SIZE, RIPPLE_SPEED - 0.25f).UseTargetPosition(projectile.Center);
            }

            float progress = (180f - projectile.timeLeft) / 60f;

            Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(DISTORT_STRENGTH * (1 - progress / 3f));
        }

        public override void Kill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            Filters.Scene["Shockwave"].Deactivate();
        }

        public bool IsNativelyImmuneToTimeStop() => true;

        public override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";


    }
}
