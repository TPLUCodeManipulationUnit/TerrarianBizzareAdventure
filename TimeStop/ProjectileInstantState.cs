using Terraria;

namespace TerrarianBizzareAdventure.TimeStop
{
    public class ProjectileInstantState : EntityState
    {
        public static ProjectileInstantState FromProjectile(int projectileId, Projectile projectile) =>
            new ProjectileInstantState()
            {
                EntityId = projectileId,
                Projectile = projectile,

                Position = projectile.position,
                Velocity = projectile.velocity,

                Damage = projectile.damage,

                AI = projectile.ai,
                AIStyle = projectile.aiStyle,

                Frame = projectile.frame,
                FrameCounter = projectile.frameCounter
            };


        public override void Restore()
        {
            Projectile.velocity = Velocity;
            Projectile.damage = Damage;

            Projectile.ai = AI;
            Projectile.aiStyle = AIStyle;

            Projectile.frame = Frame;
            Projectile.frameCounter = FrameCounter;
        }


        public Projectile Projectile { get; set; }

        public float[] AI { get; set; }
        public int AIStyle { get; set; }

        public int Frame { get; set; }
        public int FrameCounter { get; set; }
    }
}