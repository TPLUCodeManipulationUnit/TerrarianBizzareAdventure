using Microsoft.Xna.Framework;
using Terraria;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Projectiles;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Stands.StarPlatinum
{
    public class StarPlatinumStand : Stand
    {
        private const string
            TEXPATH = "Stands/StarPlatinum/",
            PUNCH = "SPPunch_",
            LEFTHAND = "_LeftHand",
            RIGHTHAND = "_RightHand";


        private bool _leftMouseButtonLastState;


        private Vector2 _punchRushDirection;


        public StarPlatinumStand() : base("starPlatinum", "Star Platinum")
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
            Animations.Add("POSE_IDLE", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPPose_Idle"), 11, 6));

            Animations.Add("RUSH_UP", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPRush_Up"), 4, 4));
            Animations.Add("RUSH_DOWN", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPRush_Down"), 4, 4));
            Animations.Add("RUSH_MIDDLE", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPRush_Middle"), 4, 4));

            Animations.Add("DESPAWN", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPDespawn"), 6, 4));
        }


        public override void AI()
        {
            base.AI();

            if (Animations.Count <= 0)
                return;

            if (PunchCounterReset > 0)
                PunchCounterReset--;
            else
                PunchCounter = 0;

            if (RushTimer > 1)
            {
                if (CurrentAnimation.Finished)
                    CurrentAnimation.ResetAnimation();

                if (RushTimer % 2 == 0)
                {
                    Projectile.NewProjectile(projectile.Center, _punchRushDirection, mod.ProjectileType<StarPlatinumRushBack>(), 120, 3.5f, Owner.whoAmI, projectile.whoAmI);
                    Projectile.NewProjectile(projectile.Center, _punchRushDirection, mod.ProjectileType<StarPlatinumRush>(), 120, 3.5f, Owner.whoAmI, projectile.whoAmI);
                }
                RushTimer--;
            }
            else
            {
                if (RushTimer > 0 && CurrentAnimation.Finished)
                {
                    RushTimer--;
                    CurrentState = "IDLE";
                }
            }

            projectile.timeLeft = 200;

            // Runs on clients only
            if (Owner.whoAmI == Main.myPlayer)
            {
                if (TBAInputs.StandPose.JustPressed)
                    if (CurrentState == "IDLE")
                        IsTaunting = true;
                    else
                        IsTaunting = false;

                if (TBAInputs.SummonStand.JustPressed && CurrentState == "IDLE")
                    CurrentState = "DESPAWN";

                if (TBAInputs.ContextAction.JustPressed && CurrentState == "POSE_IDLE")
                    TimeStopManagement.ToggleTimeStopIfStopper(TBAPlayer.Get(Owner));
            }

            projectile.Center = Owner.Center + new Vector2(34 * Owner.direction, -20 + Owner.gfxOffY);

            if (!InPose && CurrentState == "POSE_TRANSITION" && Animations[CurrentState].Finished)
            {
                InPose = true;
                CurrentAnimation.ResetAnimation(true);
                CurrentState = "POSE_IDLE";
            }

            if (IsTaunting)
            {
                if (!InPose)
                    CurrentState = "POSE_TRANSITION";
            }

            if (InPose && !IsTaunting)
            {
                CurrentState = "POSE_TRANSITION";
                if (CurrentState == "POSE_TRANSITION" && Animations[CurrentState].Finished)
                {
                    CurrentAnimation.ResetAnimation();
                    InPose = false;
                    CurrentState = "IDLE";
                }
            }

            if (CurrentState == "SUMMON")
            {
                Opacity = CurrentAnimation.FrameRect.Y / CurrentAnimation.FrameRect.Height * 0.25f;
            }

            if (CurrentState == "SUMMON" && CurrentAnimation.Finished)
                CurrentState = "IDLE";

            if (CurrentState.Contains("IDLE") && CurrentAnimation.Finished)
                CurrentAnimation.ResetAnimation();

            if (CurrentState == "IDLE" && Owner.controlUseItem && !_leftMouseButtonLastState && !IsPunching && !IsTaunting && RushTimer <= 0)
            {
                if (PunchCounter < 3)
                {
                    if (Main.MouseWorld.Y > Owner.Center.Y + 60)
                        CurrentState = Main.rand.NextBool() ? "DOWNPUNCH_LEFTHAND" : "DOWNPUNCH_RIGHTHAND";

                    else if (Main.MouseWorld.Y < Owner.Center.Y - 60)
                        CurrentState = Main.rand.NextBool() ? "UPPUNCH_LEFTHAND" : "UPPUNCH_RIGHTHAND";

                    else
                        CurrentState = Main.rand.NextBool() ? "MIDDLEPUNCH_LEFTHAND" : "MIDDLEPUNCH_RIGHTHAND";

                    SpawnPunch();

                    SetOwnerDirection();

                    PunchCounter++;

                    PunchCounterReset = 24;

                    IsPunching = true;
                }

                else
                {
                    if (Main.MouseWorld.Y > Owner.Center.Y + 60)
                        CurrentState = "RUSH_DOWN";

                    else if (Main.MouseWorld.Y < Owner.Center.Y - 60)
                        CurrentState = "RUSH_UP";

                    else
                        CurrentState = "RUSH_MIDDLE";

                    PunchCounter = 0;

                    PunchCounterReset = 0;

                    _punchRushDirection = VectorHelpers.DirectToMouse(projectile.Center, 14f);

                    RushTimer = 120;

                    SetOwnerDirection(120);
                }
            }

            if (IsPunching && CurrentAnimation.Finished)
            {
                CurrentAnimation.ResetAnimation();
                CurrentState = "IDLE";
                IsPunching = false;
            }

            if (CurrentState == "DESPAWN")
            {
                Opacity = (5 - CurrentAnimation.FrameRect.Y / (int)CurrentAnimation.FrameSize.Y) * 0.2f;

                if (CurrentAnimation.Finished)
                {
                    projectile.Kill();
                    TBAPlayer.Get(Owner).ActiveStandProjectileId = TBAPlayer.ACTIVE_STAND_PROJECTILE_INACTIVE_ID;
                }
            }

            IsFlipped = Owner.direction == 1;

            _leftMouseButtonLastState = Owner.controlUseItem;
        }


        private void SpawnPunch()
        {
            Projectile.NewProjectile(projectile.Center, VectorHelpers.DirectToMouse(projectile.Center, 22f), mod.ProjectileType<Punch>(), 120, 3.5f, Owner.whoAmI, projectile.whoAmI);
        }

        private void SetOwnerDirection(int time = 5)
        {
            TBAPlayer.Get(Owner).AttackDirectionResetTimer = time;
            TBAPlayer.Get(Owner).AttackDirection = Main.MouseWorld.X < projectile.Center.X ? -1 : 1;
        }


        public SpriteAnimation CurrentAnimation => Animations[CurrentState];

        public bool IsPunching { get; private set; }
        public bool InPose { get; private set; }

        public int PunchCounter { get; private set; }
        public int PunchCounterReset { get; private set; }
        public int RushTimer { get; private set; }
    }
}
