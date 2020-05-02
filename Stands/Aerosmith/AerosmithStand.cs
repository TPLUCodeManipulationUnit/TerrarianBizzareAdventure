﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Drawing;
using TerrarianBizzareAdventure.Enums;
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
            var summon = new SpriteAnimation(mod.GetTexture("Stands/Aerosmith/Idle"), 18, 2);
            var idle = new SpriteAnimation(mod.GetTexture("Stands/Aerosmith/Idle"), 18, 4, true);
            var despawn = new SpriteAnimation(mod.GetTexture("Stands/Aerosmith/Idle"), 18, 4);
            var turnAround = new SpriteAnimation(mod.GetTexture("Stands/Aerosmith/Turn"), 17, 4);

            Animations.Add(ANIMATION_SUMMON, summon);
            Animations.Add(ANIMATION_IDLE, idle);
            Animations.Add(ANIMATION_DESPAWN, despawn);
            Animations.Add("TURN", turnAround);

            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);
        }

        public override void AddCombos(List<StandCombo> combos)
        {
            combos.Add(new StandCombo("Bullet Hell", MouseClick.LeftClick.ToString()));
            combos.Add(new StandCombo("500lb 'Gift'", MouseClick.RightClick.ToString()));
        }

        public override void AI()
        {
            base.AI();

            if (_previousAngle != Angle)
                projectile.netUpdate = true;

            Owner.heldProj = projectile.whoAmI;

            if(Vector2.Distance(projectile.Center, Owner.Center) >= 16 * 300)
            {
                CurrentState = ANIMATION_DESPAWN;
            }

            if (CurrentAnimation != null)
            {
                projectile.width = (int)CurrentAnimation.FrameSize.X;

                projectile.height = (int)CurrentAnimation.FrameSize.Y;
            }

            if (!SetVel)
            {
                if (Owner.direction == -1)
                    Angle = (float)MathHelper.Pi;
                Opacity = 0f;
                projectile.Center = Owner.Center - new Vector2(120 * Owner.direction, 24);
                SetVel = true;
            }
            
            Speed = 8.0f;

            if (CurrentState == "TURN" && CurrentAnimation.CurrentFrame <= 8)
                Speed = 9 - MathHelper.Clamp(CurrentAnimation.CurrentFrame, 0, 8);
            else if (CurrentState == "TURN" && CurrentAnimation.CurrentFrame >= 8)
                Speed = 0 - MathHelper.Clamp(8 - (CurrentAnimation.CurrentFrame - CurrentAnimation.FrameCount), 0, 8);


            TBAPlayer tPlayer = TBAPlayer.Get(Owner);

            if (CurrentState == ANIMATION_SUMMON)
                if (Opacity < 1f)
                    Opacity += 0.04f;

            

            if (CurrentState == ANIMATION_DESPAWN || CurrentState == ANIMATION_SUMMON)
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


            if (CurrentState == "TURN" && CurrentAnimation.Finished)
            {
                CurrentAnimation.ResetAnimation();
                CurrentState = ANIMATION_IDLE;
                Angle += (float)MathHelper.Pi;
            }

            #region controls


            if (Owner.whoAmI == Main.myPlayer)
            {
                if (tPlayer.ASHover)
                {
                    Speed = .000000001f;
                }

                if (IsFlipped && tPlayer.ASTurnLeft && CurrentState == ANIMATION_IDLE && !tPlayer.ASAngleUp && !tPlayer.ASAngleDown)
                {
                    CurrentState = "TURN";
                }

                if (!IsFlipped && tPlayer.ASTurnRight && CurrentState == ANIMATION_IDLE && !tPlayer.ASAngleUp && !tPlayer.ASAngleDown)
                {
                    CurrentState = "TURN";
                }

                if (tPlayer.ASAngleUp && CurrentState == ANIMATION_IDLE)
                {
                    Angle = projectile.velocity.RotatedBy(0.06f).ToRotation();
                }

                if (tPlayer.ASAngleDown && CurrentState == ANIMATION_IDLE)
                {
                    Angle = projectile.velocity.RotatedBy(-0.06f).ToRotation();
                }

                if (tPlayer.ASAttack && CurrentState == ANIMATION_IDLE && BarrageTime <= 0)
                {
                    tPlayer.CheckStaminaCost(2);
                    BarrageTime = 32;
                }

                if (TBAInputs.SummonStand.JustPressed && CurrentState == ANIMATION_IDLE && Vector2.Distance(Owner.Center, projectile.Center) <= 16 * 10)
                    CurrentState = ANIMATION_DESPAWN;

                if (tPlayer.ASBomb && CurrentState == ANIMATION_IDLE && Owner.ownedProjectileCounts[ModContent.ProjectileType<AerosmithBomb>()] <= 0)
                {
                    tPlayer.CheckStaminaCost(15);

                    Vector2 position = projectile.Center + new Vector2(0, 6);
                    Vector2 velocity = new Vector2(9, 0).RotatedBy(Angle).RotatedByRandom(.12f);
                    int type = ModContent.ProjectileType<AerosmithBomb>();
                    int damage = 1;

                    Main.PlaySound(SoundID.Item1, projectile.position);

                    Projectile.NewProjectile(position, velocity, type, damage, 0, Owner.whoAmI);
                }

            }

            #endregion

            if (BarrageTime > 0)
            {
                BarrageTime--;

                if (BarrageTime % 4 == 0)
                {
                    Vector2 position = projectile.Center;
                    Vector2 velocity = new Vector2(16, 0).RotatedBy(Angle);
                    int type = ModContent.ProjectileType<AerosmithBullet>();
                    int damage = 60;

                    float offX = -24;
                    float offY = 12;

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

                projectile.rotation = projectile.velocity.ToRotation() + (IsFlipped ? 0 : (float)MathHelper.Pi);

                IsFlipped = projectile.velocity.X > 0;
            }

            projectile.velocity = new Vector2(Speed, 0).RotatedBy(Angle);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(Angle);
            writer.Write(Speed);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            Angle = reader.ReadSingle();
            Speed = reader.ReadSingle();
        }

        public int BarrageTime { get; set; }

        public bool SetVel { get; set; }

        private float _speed;
        public float Speed
        {
            get => _speed;
            set
            {
                projectile.netUpdate = true;
                _speed = value;
            }
        }

        private float _previousAngle;
        private float _currentAngle;
        public float Angle 
        {
            get => _currentAngle;
            set
            {
                _previousAngle = _currentAngle;
                projectile.netUpdate = true;
                _currentAngle = value;
            }
        }
    }
}