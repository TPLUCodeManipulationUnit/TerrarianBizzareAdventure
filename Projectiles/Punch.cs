using Terraria;
using Terraria.ModLoader;
using WebmilioCommons.Projectiles;

namespace TerrarianBizzareAdventure.Projectiles
{
    public class Punch : StandardProjectile
    {
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
            if (projectile.timeLeft >= 4)
                projectile.netUpdate = true;
            

            projectile.Center = ParentProjectile.Center + projectile.velocity;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);

            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Punch" + Main.rand.Next(1, 5)).WithVolume(.2f));
        }


        private Projectile ParentProjectile => Main.projectile[(int) AI0];

        public sealed override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
