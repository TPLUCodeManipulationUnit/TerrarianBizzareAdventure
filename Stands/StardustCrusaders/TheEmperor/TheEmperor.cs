using Terraria;
using Microsoft.Xna.Framework;
using TerrarianBizzareAdventure.Drawing;
using System;
using TerrarianBizzareAdventure.Players;
using Terraria.ID;

namespace TerrarianBizzareAdventure.Stands.StardustCrusaders.TheEmperor
{
    public class TheEmperor : Stand
    {
        public TheEmperor() : base("gunStand", "The Emperor")
        {
            AuraColor = Color.Azure;
        }

        public override void AddAnimations()
        {
            string path = "Stands/StardustCrusaders/TheEmperor/Emperor";

            Opacity = 0f;

            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(path, 1, 0, true));
            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(path, 1, 0, true));
            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(path, 1, 0, true));

            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);
        }

        public override void AI()
        {
            base.AI();

            AuraColor = Color.Azure;
            IsFlipped = Velocity.SafeNormalize(-Vector2.UnitY).X < 0;

            Owner.direction = IsFlipped ? -1 : 1;

            Center = Owner.MountedCenter + new Vector2(0, Owner.gfxOffY)+ Velocity;

            Owner.heldProj = projectile.whoAmI;

            if (IsSpawning)
            {
                if (Opacity < 1f)
                {
                    Opacity += 0.05f;
                    Rotation -= 0.36f * (IsFlipped ? -1 : 1);
                    Distance += MaxDistance * 0.05f;
                }
                else
                    CurrentState = ANIMATION_IDLE;
            }
            var multiplier = (IsFlipped ? -1 : 1);

            Vector2 vel = (Main.MouseWorld - Owner.Center).SafeNormalize(-Vector2.UnitY) * Distance;

            var xOffset = (3 * Math.Abs(Recoil * 10)) * multiplier;
            var yOffset = (5 * Math.Abs(Recoil * 8));

            projectile.velocity.X = vel.X - (Math.Abs(vel.SafeNormalize(-Vector2.UnitY).Y) < 0.75 ? xOffset : 0);
            projectile.velocity.Y = vel.Y - (Math.Abs(vel.SafeNormalize(-Vector2.UnitY).Y) < 0.6 ? yOffset : 0);

            if (InIdleState)
            {
                if (TBAInputs.SummonStand.JustPressed && Main.LocalPlayer == Owner)
                    CurrentState = ANIMATION_DESPAWN;

                if (TBAPlayer.Get(Owner).MouseOneTimeReset > 1)
                {
                    if (TBAPlayer.Get(Owner).MouseOneTime < 15 && Recoil <= 0 && !Owner.controlUseItem)
                    {
                        Main.PlaySound(SoundID.Item70);
                        TBAPlayer.Get(Owner).CheckStaminaCost(2, true);
                        Projectile.NewProjectile(projectile.Center - vel, vel.RotatedByRandom(0.02f) * 1.25f, 14, 24, 0, Owner.whoAmI);
                        Recoil = 0.28f;
                    }
                    else if (TBAPlayer.Get(Owner).MouseOneTime > 15 && !Owner.controlUseItem)
                    {
                        TBAPlayer.Get(Owner).CheckStaminaCost(6, true);
                        BarrageTime = 40;
                    }
                }

                if(BarrageTime % 8 == 0 && BarrageTime >= 0)
                {
                    Main.PlaySound(SoundID.Item70);
                    Projectile.NewProjectile(projectile.Center - vel, vel.RotatedByRandom(0.28f) * 1.25f, 14, 24, 0, Owner.whoAmI);
                    Recoil = 0.24f;
                }

                Rotation = Velocity.ToRotation() + (IsFlipped ? (float)MathHelper.Pi : 0) - Recoil * (IsFlipped ? -1 : 1);
            }

            if (Recoil > 0f)
                Recoil -= 0.02f;

            if (BarrageTime > -1)
                BarrageTime--;

            if (IsDespawning)
            {
                if (Opacity > 0f)
                {
                    Opacity -= 0.05f;
                    Rotation += 0.36f * (IsFlipped ? -1 : 1);
                    Distance -= MaxDistance * 0.05f;
                }
                else
                    KillStand();
            }
        }

        public override bool StopsItemUse => true;

        //public override bool CanAcquire(TBAPlayer tbaPlayer) => false;

        public override bool CanDie => BarrageTime < 0;

        public float MaxDistance => 11f;

        public float Distance { get; set; }

        public float Recoil { get; set; }

        public int BarrageTime { get; set; }
    }
}
