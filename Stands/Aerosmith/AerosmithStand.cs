using Microsoft.Xna.Framework;
using Terraria;
using TerrarianBizzareAdventure.Drawing;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands.Aerosmith
{
    public sealed class AerosmithStand : Stand
    {
        public AerosmithStand() : base("aerosmith", "Aerosmith")
        {
            AuraColor = new Color(0f, 0.25f, 1f);
        }

        public override void AddAnimations()
        {
            // kekW dis bad boi will use same animation for everything for now
            var animation = new SpriteAnimation(mod.GetTexture("Stands/Aerosmith/Idle"), 18, 2);
            var animation2 = new SpriteAnimation(mod.GetTexture("Stands/Aerosmith/Idle"), 18, 4, true);
            var animation3 = new SpriteAnimation(mod.GetTexture("Stands/Aerosmith/Idle"), 18, 4);
            Animations.Add(ANIMATION_SUMMON, animation);
            Animations.Add(ANIMATION_IDLE, animation2);
            Animations.Add(ANIMATION_DESPAWN, animation3);

            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);
        }

        public override void AI()
        {
            base.AI();

            if (!SetVel)
            {
                if (Owner.direction == -1)
                    Angle = (float)MathHelper.Pi;
                Opacity = 0f;
                projectile.Center = Owner.Center - new Vector2(120 * Owner.direction, 24);
                SetVel = true;
            }

            projectile.velocity = new Vector2(8, 0).RotatedBy(Angle);

            TBAPlayer tPlayer = TBAPlayer.Get(Owner);

            if (tPlayer.ASHover)
            {
                projectile.velocity = new Vector2(.000000001f, 0).RotatedBy(Angle);
            }

            projectile.timeLeft = 200;

            if (CurrentState == ANIMATION_SUMMON)
                if (Opacity < 1f)
                    Opacity += 0.04f;

            if (Owner.whoAmI == Main.myPlayer)
            {
                if (TBAInputs.SummonStand.JustPressed && CurrentState == ANIMATION_IDLE && Vector2.Distance(Owner.Center, projectile.Center) <= 16 * 10)
                    CurrentState = ANIMATION_DESPAWN;
            }

            if(CurrentState == ANIMATION_DESPAWN || CurrentState == ANIMATION_SUMMON)
            {
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y) + projectile.velocity * 4.5f, 0, 0, 31, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[dustIndex].velocity = -(projectile.velocity * 0.5f).RotatedByRandom(.45f);
                }
            }

            if (CurrentState == ANIMATION_DESPAWN)
            {
                if (Opacity > 0f)
                {
                    Opacity -= 0.02f;
                }
                else
                {
                    KillStand();
                }
            }

            if (tPlayer.ASAngleUp)
            {
                Angle = projectile.velocity.RotatedBy(0.06f).ToRotation();
            }

            if (tPlayer.ASAngleDown)
            {
                Angle = projectile.velocity.RotatedBy(-0.06f).ToRotation();
            }

            if (tPlayer.ASAttack)
            {
                Projectile.NewProjectile(projectile.Center, new Vector2(16, 0).RotatedBy(Angle).RotatedByRandom(.12f),14, 60, 0, Owner.whoAmI);
            }

            projectile.rotation = projectile.velocity.ToRotation() + (IsFlipped ? 0 : (float)MathHelper.Pi);

            IsFlipped = projectile.velocity.X > 0;

        }

        public bool SetVel { get; set; }

        public float Angle { get; set; }
    }
}
