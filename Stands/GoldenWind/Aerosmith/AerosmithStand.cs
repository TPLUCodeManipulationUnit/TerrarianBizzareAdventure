using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Drawing;
using TerrarianBizzareAdventure.Enums;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands.GoldenWind.Aerosmith
{
    public sealed class AerosmithStand : Stand
    {
        private float _previousAngle, _currentAngle, _speed;


        public AerosmithStand() : base("aerosmith", "Aerosmith")
        {
            AuraColor = Color.Cyan;
        }

        public override void AddAnimations()
        {
            // kekW dis bad boi will use same animation for everything for now
            var summon = new SpriteAnimation(mod.GetTexture("Stands/GoldenWind/Aerosmith/Idle"), 18, 2);
            var idle = new SpriteAnimation(mod.GetTexture("Stands/GoldenWind/Aerosmith/Idle"), 18, 4, true);
            var despawn = new SpriteAnimation(mod.GetTexture("Stands/GoldenWind/Aerosmith/Idle"), 18, 4);
            var turnAround = new SpriteAnimation(mod.GetTexture("Stands/GoldenWind/Aerosmith/Turn"), 17, 4);
            var returnAuto = new SpriteAnimation(mod.GetTexture("Stands/GoldenWind/Aerosmith/Idle"), 18, 4, true);

            Animations.Add(ANIMATION_SUMMON, summon);
            Animations.Add(ANIMATION_IDLE, idle);
            Animations.Add(ANIMATION_DESPAWN, despawn);
            Animations.Add("TURN", turnAround);
            Animations.Add("RETURN", returnAuto);


            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);

            Speed = 8.0f;

        }

        public override void AI()
        {
            base.AI();

            if (BaseDamage <= 0)
            {
                GetBaseDamage(DamageClass.Ranged, Owner);
                Main.NewText(BaseDamage);
            }
            if (_previousAngle != Angle)
                projectile.netUpdate = true;


            Owner.heldProj = projectile.whoAmI;

            if (Vector2.Distance(Center, Owner.Center) >= 16 * 200)
            {
                IsPatroling = false;
                BarrageTime = 0;
                CurrentState = "RETURN";
            }

            if (CurrentAnimation != null)
            {
                Width = (int)CurrentAnimation.FrameSize.X;
                Height = (int)CurrentAnimation.FrameSize.Y;
            }


            if (!SetVel)
            {
                if (Owner.direction == -1)
                    Angle = (float)MathHelper.Pi;


                Opacity = 0f;
                Center = Owner.Center - new Vector2(120 * Owner.direction, 24);
                SetVel = true;
            }

            TBAPlayer tPlayer = TBAPlayer.Get(Owner);

            if (IsSpawning)
                if (Opacity < 1f)
                    Opacity += 0.04f;



            if (IsDespawning || IsSpawning)
            {
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y) + projectile.velocity * 4.5f, 0, 0, 31, 0f, 0f, 100, default, 2f);
                    Main.dust[dustIndex].velocity = -(projectile.velocity * 0.5f).RotatedByRandom(.45f);
                }
            }

            if (IsDespawning)
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

            #region controls

            if (IsReturning)
            {
                Speed = 8.0f;
                tPlayer.ASHover = false;
                if (Vector2.Distance(Owner.Center, projectile.Center) <= 16 * 10)
                    CurrentState = ANIMATION_DESPAWN;

                Angle = (Owner.Center - Center).SafeNormalize(-Vector2.UnitY).ToRotation();

                IsEngineOn = true;

                Velocity = new Vector2(Speed, 0).RotatedBy(Angle);
            }

            if (Owner.whoAmI == Main.myPlayer)
            {
                if (IsIdling)
                {

                    if (TBAInputs.ContextAction.JustPressed)
                        IsEngineOn = !IsEngineOn;

                    float angle = (Main.MouseWorld - Center).SafeNormalize(-Vector2.UnitY).ToRotation();

                    Angle = Utils.AngleLerp(Angle, angle, 0.07f);

                    float omegaAngle = Utils.AngleLerp(Angle, angle, 0.5f);

                    if (tPlayer.ASAttack && BarrageTime <= 0)
                    {
                        BarrageTime = 16;
                    }

                    if (TBAInputs.ExtraAction01.JustPressed)
                    {
                        IsPatroling = !IsPatroling;

                        if (IsPatroling)
                            PatrolDistance = 60;
                    }

                    if (TBAInputs.SummonStand.JustPressed)
                    {
                        IsPatroling = false;
                        if (Vector2.Distance(Owner.Center, projectile.Center) <= 16 * 10)
                            CurrentState = ANIMATION_DESPAWN;
                        else
                            CurrentState = "RETURN";
                    }

                    if (tPlayer.ASBomb && Owner.ownedProjectileCounts[ModContent.ProjectileType<AerosmithBomb>()] <= 0)
                    {
                        Vector2 position = projectile.Center + new Vector2(0, 6);
                        Vector2 velocity = new Vector2(9, 0).RotatedBy(Angle).RotatedByRandom(.12f);
                        int type = ModContent.ProjectileType<AerosmithBomb>();
                        int damage = 1;

                        Main.PlaySound(SoundID.Item1, projectile.position);

                        int proj = Projectile.NewProjectile(position, velocity, type, damage, 0, Owner.whoAmI);

                        AerosmithBomb bomb = Main.projectile[proj].modProjectile as AerosmithBomb;
                        bomb.ExplosionDamage = BombDamage;
                    }
                }

            }

            #endregion


            if (BarrageTime > 0)
            {
                BarrageTime--;

                if (BarrageTime % 4 == 0)
                {
                    Vector2 position = Center;
                    Vector2 velocity = new Vector2(16, 0).RotatedBy(Angle);
                    int type = ModContent.ProjectileType<AerosmithBullet>();
                    int damage = BulletDamage;

                    float offX = -24;
                    float offY = 4;

                    int multiplier = (IsFlipped ? 1 : -1);

                    Vector2 offset1 = new Vector2(offX, offY * multiplier).RotatedBy(Angle);
                    Vector2 offset2 = new Vector2(offX, (offY + 2) * multiplier).RotatedBy(Angle);

                    Main.PlaySound(SoundID.Item31, projectile.position);

                    Projectile.NewProjectile(position + offset1, velocity.RotatedByRandom(.035f), type, damage, 0, Owner.whoAmI);

                    Projectile.NewProjectile(position + offset2, velocity.RotatedByRandom(.035f), type, damage, 0, Owner.whoAmI);
                }
            }

            if (CurrentState != "TURN")
            {

                projectile.rotation = Angle + (IsFlipped ? 0 : (float)MathHelper.Pi);

                IsFlipped = projectile.velocity.X > 0;
            }

            if (CurrentState == "TURN")
            {
                if (Speed > 0)
                    Speed = MathHelper.Clamp(Speed - 0.05f, 0, 3f);

                if (CurrentAnimation.Finished)
                {
                    Angle = new Vector2(PatrolDirection, 0).ToRotation() + (IsFlipped ? 0.12f : -0.12f);
                    CurrentAnimation.ResetAnimation();
                    Speed = 3f;
                    CurrentState = ANIMATION_IDLE;
                    Angle = Velocity.ToRotation();
                    PatrolDirection = -PatrolDirection;
                    PatrolDistance = 60;
                }
            }



            if (IsPatroling)
            {
                BarrageTime = 0;
                Angle = new Vector2(PatrolDirection, 0.15f).ToRotation();
                if (PatrolDistance > 0)
                {
                    Speed = 3f;
                    PatrolDistance -= 1;
                }
                else
                {
                    CurrentState = "TURN";
                }

                Velocity = new Vector2(PatrolDirection, 0) * Speed;
            }
            else
            {


                if (IsEngineOn)
                {
                    FlightVector = new Vector2(1, 0).RotatedBy(Angle);

                    if (Speed < 8.0f)
                        Speed += 0.2f;

                    if (YSpeed > 0)
                        YSpeed -= 0.12f;
                }
                else
                {
                    if (Speed > 0)
                        Speed -= 0.025f;

                    if (YSpeed < 12f)
                        YSpeed += 0.06f;

                    if (CurrentAnimation != null)
                        CurrentAnimation.CurrentFrame = 0;
                }

                Velocity = FlightVector * Speed + new Vector2(0, YSpeed);

                if (!IsReturning && !IsDespawning)
                    Velocity = Collision.TileCollision(Center - new Vector2(10), Velocity, 20, 20, false, false, 1);

                if (Velocity.Y == 0 && !IsEngineOn)
                {
                    if (Speed > 0)
                        Speed = MathHelper.Clamp(Speed - 0.15f, 0, 8f);
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(Angle);
            writer.Write(Speed);
            writer.Write(IsPatroling);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            Angle = reader.ReadSingle();
            Speed = reader.ReadSingle();
            IsPatroling = reader.ReadBoolean();
        }

        public int BarrageTime { get; set; }

        public bool SetVel { get; set; }

        public bool IsReturning => CurrentState == "RETURN";


        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                projectile.netUpdate = true;
            }
        }

        public float YSpeed { get; set; }

        public Vector2 FlightVector { get; set; }

        public bool IsEngineOn { get; set; } = true;

        public int BulletDamage => 8 + (int)(BaseDamage * 1.35);

        public int BombDamage => 225 + (int)(BaseDamage * 8.5);

        public bool IsPatroling { get; set; }

        public int PatrolDistance { get; set; }

        public int PatrolDirection { get; set; } = 1;

        public float Angle 
        {
            get => _currentAngle;
            set
            {
                _previousAngle = _currentAngle;
                _currentAngle = value;

                projectile.netUpdate = true;
            }
        }
    }
}
