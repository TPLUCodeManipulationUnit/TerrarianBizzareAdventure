using Microsoft.Xna.Framework;
using Terraria;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Projectiles.Stands.PunchRushProjectile;

namespace TerrarianBizzareAdventure.Projectiles.Stands.Melee
{
    public class StarPlatinum : StandBase
    {
        private const string
            TEXPATH = "Textures/Stands/StarPlatinum/",
            PUNCH = "SPPunch_",
            LEFTHAND = "_LeftHand",
            RIGHTHAND = "_RightHand";

        private bool
            _isPunching, _inPose, _leftMouseButtonLastState;

        private int
            _punchCounter, _punchCounterReset, _rushTimer;

        private Vector2 _punchRushDirection;

        public StarPlatinum() : base("Star Platinum")
        {
        }

        public override void AddAnimations()
        {
            Animations.Add("SUMMON", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPSummon"), 10, 4));
            Animations.Add("IDLE", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPIdle"), 14, 4));

            Animations.Add("MIDDLEPUNCH_LEFTHAND", new SpriteAnimation(mod.GetTexture(TEXPATH + PUNCH + "Middle" + LEFTHAND), 3, 5));
            Animations.Add("MIDDLEPUNCH_RIGHTHAND", new SpriteAnimation(mod.GetTexture(TEXPATH + PUNCH + "Middle" + RIGHTHAND), 3, 5));

            Animations.Add("DOWNPUNCH_LEFTHAND", new SpriteAnimation(mod.GetTexture(TEXPATH + PUNCH + "Down" + LEFTHAND), 3, 5));
            Animations.Add("DOWNPUNCH_RIGHTHAND", new SpriteAnimation(mod.GetTexture(TEXPATH + PUNCH + "Down" + RIGHTHAND), 3, 5));

            Animations.Add("UPPUNCH_LEFTHAND", new SpriteAnimation(mod.GetTexture(TEXPATH + PUNCH + "Up" + LEFTHAND), 3, 5));
            Animations.Add("UPPUNCH_RIGHTHAND", new SpriteAnimation(mod.GetTexture(TEXPATH + PUNCH + "Up" + RIGHTHAND), 3, 5));

            Animations.Add("POSE_TRANSITION", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPPose_Transition"), 15, 4));
            Animations.Add("POSE_IDLE", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPPose_Idle"), 11, 4));

            Animations.Add("RUSH_UP", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPRush_Up"), 4, 4));
            Animations.Add("RUSH_DOWN", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPRush_Down"), 4, 4));
            Animations.Add("RUSH_MIDDLE", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPRush_Middle"), 4, 4));

            Animations.Add("DESPAWN", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPDespawn"), 6, 4));
        }

        public override void AI()
        {
            base.AI();

            if (_punchCounterReset > 0)
                _punchCounterReset--;
            else
                _punchCounter = 0;

            if (_rushTimer > 1)
            {
                if (CurrentAnimation.Finished)
                    CurrentAnimation.ResetAnimation();

                if (_rushTimer % 2 == 0)
                {
                    Projectile.NewProjectile(projectile.Center, _punchRushDirection, mod.ProjectileType<StarPlatinumRushBack>(), 120, 3.5f, Owner.whoAmI, projectile.whoAmI);
                    Projectile.NewProjectile(projectile.Center, _punchRushDirection, mod.ProjectileType<StarPlatinumRush>(), 120, 3.5f, Owner.whoAmI, projectile.whoAmI);
                }
                _rushTimer--;
            }
            else
            {
                if (_rushTimer > 0 && CurrentAnimation.Finished)
                {
                    _rushTimer--;
                    currentState = "IDLE";
                }
            }

            projectile.timeLeft = 2;

            // Runs on clients only
            if (Owner.whoAmI == Main.myPlayer)
            {
                if (TBAInputs.StandPose.JustPressed)
                    if (currentState == "IDLE")
                        isTaunting = true;
                    else
                        isTaunting = false;

                if (TBAInputs.SummonStand.JustPressed && currentState == "IDLE")
                        currentState = "DESPAWN";

            }

            projectile.Center = Owner.Center + new Vector2(34 * Owner.direction, -20 + Owner.gfxOffY);

            if (!_inPose && currentState == "POSE_TRANSITION" && Animations[currentState].Finished)
            {
                _inPose = true;
                Animations[currentState].ResetAnimation(true);
                currentState = "POSE_IDLE";
            }

            if (isTaunting)
            {
                if(!_inPose)
                    currentState = "POSE_TRANSITION";
            }

            if(_inPose && !isTaunting)
            {
                currentState = "POSE_TRANSITION";
                if (currentState == "POSE_TRANSITION" && Animations[currentState].Finished)
                {
                    Animations[currentState].ResetAnimation();
                    _inPose = false;
                    currentState = "IDLE";
                }
            }

            if(currentState == "SUMMON")
            {
                Opacity = Animations[currentState].FrameRect.Y / Animations[currentState].FrameRect.Height * 0.25f;
            }

            if (currentState == "SUMMON" && Animations[currentState].Finished)
                currentState = "IDLE";

            if (currentState.Contains("IDLE") && Animations[currentState].Finished)
                Animations[currentState].ResetAnimation();

            if (currentState == "IDLE" && Owner.controlUseItem && !_leftMouseButtonLastState && !_isPunching && !isTaunting && _rushTimer <= 0)
            {
                if (_punchCounter < 3)
                {
                    if (Main.MouseWorld.Y > Owner.Center.Y + 60)
                        currentState = Main.rand.NextBool() ? "DOWNPUNCH_LEFTHAND" : "DOWNPUNCH_RIGHTHAND";

                    else if (Main.MouseWorld.Y < Owner.Center.Y - 60)
                        currentState = Main.rand.NextBool() ? "UPPUNCH_LEFTHAND" : "UPPUNCH_RIGHTHAND";

                    else
                        currentState = Main.rand.NextBool() ? "MIDDLEPUNCH_LEFTHAND" : "MIDDLEPUNCH_RIGHTHAND";

                    SpawnPunch();

                    SetOwnerDirection();

                    _punchCounter++;

                    _punchCounterReset = 24;

                    _isPunching = true;
                }

                else
                {
                    if (Main.MouseWorld.Y > Owner.Center.Y + 60)
                        currentState = "RUSH_DOWN";

                    else if (Main.MouseWorld.Y < Owner.Center.Y - 60)
                        currentState = "RUSH_UP";

                    else
                        currentState = "RUSH_MIDDLE";

                    _punchCounter = 0;

                    _punchCounterReset = 0;

                    _punchRushDirection = Helpers.DirectToMouse(projectile.Center, 14f);

                    _rushTimer = 120;

                    SetOwnerDirection(120);
                }
            }
            
            if (_isPunching && Animations[currentState].Finished)
            {
                Animations[currentState].ResetAnimation();
                currentState = "IDLE";
                _isPunching = false;
            }

            if (currentState == "DESPAWN")
            {
                Opacity = (5 - CurrentAnimation.FrameRect.Y / (int)CurrentAnimation.FrameSize.Y) * 0.2f;
                if(Animations[currentState].Finished)
                    projectile.Kill();
            }

            isFlipped = Owner.direction == 1;

            _leftMouseButtonLastState = Owner.controlUseItem;
        }

        public override void SetChildDefaults()
        {
        }

        private void SpawnPunch()
        {
            Projectile.NewProjectile(projectile.Center, Helpers.DirectToMouse(projectile.Center, 22f), mod.ProjectileType<Punch>(), 120, 3.5f, Owner.whoAmI, projectile.whoAmI);
        }

        private void SetOwnerDirection(int time = 5)
        {
            TBAPlayer.Get(Owner).AttackDirectionResetTimer = time;
            TBAPlayer.Get(Owner).AttackDirection = Main.MouseWorld.X < projectile.Center.X ? -1 : 1;
        }

        public SpriteAnimation CurrentAnimation => Animations[currentState];

    }
}
