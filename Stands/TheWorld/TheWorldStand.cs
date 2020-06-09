using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrarianBizzareAdventure.Drawing;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Helpers;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Projectiles;
using TerrarianBizzareAdventure.TimeStop;
using TerrarianBizzareAdventure.Projectiles.Misc;
using System.IO;
using TerrarianBizzareAdventure.Enums;
using Terraria.ID;

namespace TerrarianBizzareAdventure.Stands.TheWorld
{
    public class TheWorldStand : PunchBarragingStand
    {
        public TheWorldStand() : base("theWorld", "The World")
        {
            AuraColor = new Color(1.0f, 0.7f, 0.0f);
            CallSoundPath = "Sounds/TheWorld/Call";
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            RoadRollerXAxis = -1.0f;
        }

        public override void AddAnimations()
        {
            string path = "Stands/TheWorld/TheWorld";

            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(mod.GetTexture(path + "Spawn"), 7, 5));

            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(mod.GetTexture(path + "Idle"), 8, 8, true));
            Animations.Add("FLY_UP", new SpriteAnimation(mod.GetTexture(path + "Idle"), 8, 8, true));

            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(mod.GetTexture(path + "Spawn"), 7, 5, false, null, true));
            Animations[ANIMATION_DESPAWN].ReversePlayback = true;

            Animations.Add("PUNCH_RD", new SpriteAnimation(mod.GetTexture(path + "PunchDown"), 7, 3));
            Animations.Add("PUNCH_LD", new SpriteAnimation(mod.GetTexture(path + "PunchDownAlt"), 8, 3));

            Animations.Add("PUNCH_RU", new SpriteAnimation(mod.GetTexture(path + "PunchUp"), 8, 3));
            Animations.Add("PUNCH_LU", new SpriteAnimation(mod.GetTexture(path + "PunchUpAlt"), 8, 3));

            Animations.Add("PUNCH_R", new SpriteAnimation(mod.GetTexture(path + "PunchMiddle"), 7, 3));
            Animations.Add("PUNCH_L", new SpriteAnimation(mod.GetTexture(path + "PunchMiddleAlt"), 8, 3));

            Animations.Add("THROW_KNIVES", new SpriteAnimation(mod.GetTexture(path + "KnifeThrow"), 14, 4));

            Animations["PUNCH_R"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_L"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_RD"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_LD"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_RU"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_LU"].SetNextAnimation(Animations[ANIMATION_IDLE]);

            Animations["THROW_KNIVES"].SetNextAnimation(Animations[ANIMATION_IDLE]);


            Animations.Add("RUSH_UP", new SpriteAnimation(mod.GetTexture(path + "RushUp"), 4, 4));
            Animations.Add("RUSH_DOWN", new SpriteAnimation(mod.GetTexture(path + "RushDown"), 4, 4));
            Animations.Add("RUSH_MID", new SpriteAnimation(mod.GetTexture(path + "RushMiddle"), 4, 4));

            Animations.Add("SLAM", new SpriteAnimation(mod.GetTexture(path + "SlamDunk"), 1, 5, true));

            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);
        }

        public override void AddCombos(List<StandCombo> combos)
        {
            combos.Add(new StandCombo("Punch", MouseClick.LeftClick.ToString()));
            combos.Add(new StandCombo("Punch Barrage", MouseClick.LeftClick.ToString(), MouseClick.LeftClick.ToString(), MouseClick.LeftClick.ToString()));
            combos.Add(new StandCombo("ZA WARUDO", TBAInputs.ContextAction.GetAssignedKeys()[0].ToString()));
            combos.Add(new StandCombo("Earth Flattener", TBAInputs.ExtraAction01.GetAssignedKeys()[0].ToString()));
            combos.Add(new StandCombo("Sick Throw", TBAInputs.ExtraAction02.GetAssignedKeys()[0].ToString()));
        }

        public override void AI()
        {
            base.AI();

            Opacity = 1f;

            IsFlipped = Owner.direction == 1;

            projectile.timeLeft = 200;

            int xOffset = CurrentState.Contains("PUNCH") || CurrentState.Contains("RUSH") || CurrentState == "THROW_KNIVES" ?  34 : -16;

            Vector2 lerpPos = Owner.Center + new Vector2(xOffset * Owner.direction, -24 + Owner.gfxOffY);

            if (CurrentState.Contains("PUNCH") || CurrentState == "SLAM" || CurrentState == "THROW_KNIVES")
                Owner.heldProj = projectile.whoAmI;

            projectile.Center = Vector2.Lerp(projectile.Center, lerpPos, 0.26f);

            if (CurrentState == "THROW_KNIVES")
            {
                if (CurrentAnimation.CurrentFrame == 10)
                {
                    Vector2 direction = VectorHelpers.DirectToMouse(projectile.Center, 14f);

                    if (Owner.Center.X + direction.X > Owner.Center.X)
                        Owner.direction = 1;
                    else
                        Owner.direction = -1;
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(projectile.Center + direction * 2.2f + new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-34, 34)).RotatedBy(direction.ToRotation()), direction, ModContent.ProjectileType<Knife>(), 80, 3.5f, Owner.whoAmI, projectile.whoAmI);
                    }
                }
            }


            if (CurrentState == ANIMATION_DESPAWN && CurrentAnimation.Finished && TimeStopDelay <= 0)
                KillStand();

            if (InIdleState)
            {
                if (Owner.whoAmI == Main.myPlayer)
                {
                    if (TBAInputs.StandPose.JustPressed)
                        if (CurrentState == ANIMATION_IDLE)
                            IsTaunting = true;
                        else
                            IsTaunting = false;

                    if (TBAInputs.ExtraAction02.JustPressed && TBAPlayer.Get(Owner).CheckStaminaCost(10))
                        CurrentState = "THROW_KNIVES";

                    if (TBAInputs.SummonStand.JustPressed)
                        CurrentState = ANIMATION_DESPAWN;

                    int roadRollerCost = TimeStopManagement.TimeStopped ? 10 : 50;

                    if (TBAInputs.ExtraAction01.JustPressed && !BeganAscending && TBAPlayer.Get(Owner).CheckStaminaCost(roadRollerCost))
                    {
                        projectile.netUpdate = true;
                        if (!TimeStopManagement.TimeStopped)
                        {
                            if (!TimeStopManagement.TimeStopped)
                                TBAMod.PlayVoiceLine("Sounds/TheWorld/TimeStop");

                            IsTaunting = false;
                            TimeStopDelay = 25;
                        }

                        Owner.Center -= new Vector2(0, 16);
                        Owner.velocity.Y = -16;
                        CurrentState = "FLY_UP";
                        projectile.netUpdate = true;
                        TBAPlayer.Get(Owner).PointOfInterest = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
                        BeganAscending = true;
                    }

                    if (TBAInputs.ContextAction.JustPressed)
                        TimeStop();
                }

                if (PunchCounter < 2)
                {
                    if (TBAPlayer.Get(Owner).MouseOneTimeReset > 0)
                    {
                        projectile.netUpdate = true;
                        if (TBAPlayer.Get(Owner).MouseOneTime < 15 && !Owner.controlUseItem)
                        {
                            TBAPlayer.Get(Owner).CheckStaminaCost(2);
                            Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;

                            if (Main.MouseWorld.Y > Owner.Center.Y + 60)
                                CurrentState = "PUNCH_" + (Main.rand.NextBool() ? "R" : "L") + "D";
                            else if (Main.MouseWorld.Y < Owner.Center.Y - 60)
                                CurrentState = "PUNCH_" + (Main.rand.NextBool() ? "R" : "L") + "U";
                            else
                                CurrentState = "PUNCH_" + (Main.rand.NextBool() ? "R" : "L");

                            PunchCounter++;

                            PunchCounterReset = 32;

                            Projectile.NewProjectile(projectile.Center, VectorHelpers.DirectToMouse(projectile.Center, 22f), ModContent.ProjectileType<Punch>(), 80, 3.5f, Owner.whoAmI, projectile.whoAmI);

                        }
                    }
                }
                else if (Owner.controlUseItem)
                {
                    projectile.netUpdate = true;
                    TBAMod.PlayVoiceLine("Sounds/TheWorld/MudaRush");

                    TBAPlayer.Get(Owner).CheckStaminaCost(16);
                    if (Main.MouseWorld.Y > Owner.Center.Y + 60)
                        CurrentState = "RUSH_DOWN";

                    else if (Main.MouseWorld.Y < Owner.Center.Y - 60)
                        CurrentState = "RUSH_UP";

                    else
                        CurrentState = "RUSH_MID";

                    RushTimer = 180;

                    PunchRushDirection = VectorHelpers.DirectToMouse(projectile.Center, 22f);

                    TBAPlayer.Get(Owner).AttackDirectionResetTimer = RushTimer;
                    TBAPlayer.Get(Owner).AttackDirection = Main.MouseWorld.X < projectile.Center.X ? -1 : 1;

                    int barrage = Projectile.NewProjectile(projectile.Center, PunchRushDirection, ModContent.ProjectileType<WorldBarrage>(), 60, 0, Owner.whoAmI);

                    if (Main.projectile[barrage].modProjectile is WorldBarrage worldBarrage)
                    {
                        worldBarrage.RushDirection = PunchRushDirection;
                        worldBarrage.ParentProjectile = projectile.whoAmI;
                    }
                }
            }

            if (PunchCounterReset > 0)
                PunchCounterReset--;
            else
                PunchCounter = 0;

            if (TimeStopDelay > 1)
                TimeStopDelay--;
            else if (TimeStopDelay == 1)
            {
                if (!TimeStopManagement.TimeStopped)
                {
                    Projectile.NewProjectile(Owner.Center, Vector2.Zero, ModContent.ProjectileType<TimeStopVFX>(), 0, 0, Owner.whoAmI);

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/TheWorld/TheWorld_ZaWarudoSFX"));
                }

                TimeStopManagement.ToggleTimeStopIfStopper(TBAPlayer.Get(Owner), 9 * Constants.TICKS_PER_SECOND);
                TimeStopDelay--;
            }

            if (RushTimer > 1)
            {
                if (CurrentAnimation != null && CurrentAnimation.Finished)
                    CurrentAnimation.ResetAnimation();

                RushTimer--;
            }
            else
            {
                if (RushTimer > 0 && CurrentAnimation != null && CurrentAnimation.Finished)
                {
                    RushTimer--;
                    CurrentState = ANIMATION_IDLE;
                }
            }

            if(BeganAscending)
            {
                if (AscensionTimer >= (int)(2 * Constants.TICKS_PER_SECOND))
                {
                    if (RoadRollerXAxis == -1.0f)
                        RoadRollerXAxis = Main.MouseWorld.X;

                    TBAPlayer.Get(Owner).PointOfInterest = new Vector2(MathHelper.Lerp(TBAPlayer.Get(Owner).PointOfInterest.X, RoadRollerXAxis, 0.1f), TBAPlayer.Get(Owner).PointOfInterest.Y);
                }

                if(++AscensionTimer >= (int)(2.5 * Constants.TICKS_PER_SECOND))
                {
                    projectile.netUpdate = true;
                    TBAMod.PlayVoiceLine("Sounds/TheWorld/RoadRoller");
                    BeganAscending = false;
                    AscensionTimer = 0;
                    CurrentState = "SLAM";
                    HasResetRoadRollerDrop = false;
                    RoadRollerID = Projectile.NewProjectile(new Vector2(RoadRollerXAxis, Owner.Center.Y), Vector2.Zero, ModContent.ProjectileType<RoadRoller>(), 20, 0, projectile.owner);
                    RoadaRollaDa.spriteDirection = Owner.direction * -1;
                    RoadaRollaDa.Center = new Vector2(RoadRollerXAxis, Owner.Center.Y);
                    RoadRollerXAxis = -1.0f;
                }
                else
                {
                    Owner.velocity = Vector2.Zero;
                    Owner.Center -= new Vector2(0f, 10f);
                }
            }

            if (RoadaRollaDa.modProjectile is RoadRoller roller)
            {
                if (RushTimer > 0 && roller.HasTouchedGround || !roller.HasTouchedGround)
                {
                    Owner.direction = RoadaRollaDa.spriteDirection * -1;
                    Owner.Center = projectile.Center + new Vector2(20 * Owner.direction * -1, -20);
                    projectile.Center = RoadaRollaDa.Center + new Vector2(40 * Owner.direction * -1, -40);
                }

                if (roller.HasTouchedGround && !HasResetRoadRollerDrop)
                {
                    TBAMod.PlayVoiceLine("Sounds/TheWorld/MudaRush");
                    CurrentState = "RUSH_DOWN";

                    RushTimer = 180;

                    PunchRushDirection = (RoadaRollaDa.Center - projectile.Center).SafeNormalize(-Vector2.UnitX) * 12f;

                    TBAPlayer.Get(Owner).AttackDirectionResetTimer = RushTimer;
                    TBAPlayer.Get(Owner).AttackDirection = RoadaRollaDa.Center.X < projectile.Center.X ? -1 : 1;

                    TBAPlayer.Get(Owner).PointOfInterest = Vector2.Zero;

                    HasResetRoadRollerDrop = true;

                    int barrage = Projectile.NewProjectile(projectile.Center, PunchRushDirection, ModContent.ProjectileType<WorldBarrage>(), 60, 0, Owner.whoAmI);

                    if (Main.projectile[barrage].modProjectile is WorldBarrage worldBarrage)
                    {
                        worldBarrage.RushDirection = PunchRushDirection;
                        worldBarrage.ParentProjectile = projectile.whoAmI;
                    }
                }
            }
        }

        public override bool PreKill(int timeLeft)
        {
            TBAPlayer.Get(Owner).PointOfInterest = Vector2.Zero;

            return base.PreKill(timeLeft);
        }

        private const int TIME_STOP_COST = 20;

        public void TimeStop()
        {
            if(TimeStopManagement.TimeStopped)
            {
                TimeStopManagement.TryResumeTime(TBAPlayer.Get(Owner));

                TBAMod.PlayVoiceLine("Sounds/TheWorld/TimeResume");
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/TheWorld/TheWorld_ZaWarudoReleaseSFX"));
                return;
            }

            if (TBAPlayer.Get(Owner).CheckStaminaCost(TIME_STOP_COST))
            {

                if (!TimeStopManagement.TimeStopped)
                    TBAMod.PlayVoiceLine("Sounds/TheWorld/TimeStop");

                IsTaunting = false;
                TimeStopDelay = 25;
            }
        }

        public override bool CanDie => TimeStopDelay <= 0;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);

            writer.Write(RoadRollerID);
            writer.Write(BeganAscending);
            writer.Write(RushTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);

            RoadRollerID = reader.ReadInt32();
            BeganAscending = reader.ReadBoolean();
            RushTimer = reader.ReadInt32();
        }


        public int PunchCounter { get; private set; }
        public int PunchCounterReset { get; private set; }


        public int TimeStopDelay { get; private set; }

        public int RoadRollerID { get; private set; }

        public bool HasResetRoadRollerDrop { get; private set; }
        public Projectile RoadaRollaDa => Main.projectile[RoadRollerID];

        public bool BeganAscending { get; private set; }

        public int AscensionTimer { get; private set; }

        public float RoadRollerXAxis { get; private set; }
    }
}
