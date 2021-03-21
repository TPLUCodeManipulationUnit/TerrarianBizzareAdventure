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

namespace TerrarianBizzareAdventure.Stands.StardustCrusaders.TheWorld
{
    public class TheWorldStand : TimeStoppingStand
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

        public override void AddCombos()
        {
            Combos.Add("Barrage",
                   new StandCombo("The World: Speed Barrage",
                       MouseClick.LeftClick.ToString(),
                       MouseClick.RightClick.ToString(),
                       TBAInputs.CABind()
                       )
                   );

            Combos.Add("Time Stop",
                    new StandCombo("The World: Time Stop",
                        TBAInputs.EA1Bind(),
                        TBAInputs.EA1Bind(),
                        TBAInputs.Up,
                        TBAInputs.CABind()
                        )
                    );

            Combos.Add("Road",
                    new StandCombo("The World: Road Roller",
                        TBAInputs.Up,
                        TBAInputs.CABind(),
                        TBAInputs.Up,
                        TBAInputs.EA1Bind(),
                        TBAInputs.Up,
                        TBAInputs.CABind(),
                        TBAInputs.Down,
                        TBAInputs.CABind()
                        )
                    );
        }

        public override void AddAnimations()
        {
            string path = "Stands/StardustCrusaders/TheWorld/TheWorld";

            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(mod.GetTexture(path + "Spawn"), 7, 5));

            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(mod.GetTexture(path + "Idle"), 8, 8, true));
            Animations.Add("FLY_UP", new SpriteAnimation(mod.GetTexture(path + "Idle"), 8, 8, true));

            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(mod.GetTexture(path + "Spawn"), 7, 3, false, null));
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

            Animations.Add(TIMESTOP_ANIMATION, new SpriteAnimation(mod.GetTexture(path + "KnifeThrow"), 14, 4));


            Animations.Add("PUNCHRUSH_UP", new SpriteAnimation(mod.GetTexture(path + "RushUp"), 4, 4));
            Animations.Add("PUNCHRUSH_DOWN", new SpriteAnimation(mod.GetTexture(path + "RushDown"), 4, 4));
            Animations.Add("PUNCHRUSH_MID", new SpriteAnimation(mod.GetTexture(path + "RushMiddle"), 4, 4));

            Animations.Add("SLAM", new SpriteAnimation(mod.GetTexture(path + "SlamDunk"), 1, 5, true));

            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);

            Animations["PUNCHRUSH_UP"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCHRUSH_MID"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCHRUSH_DOWN"].SetNextAnimation(Animations[ANIMATION_IDLE]);
        }

        public override void AI()
        {
            base.AI();

            Opacity = 1f;

            IsFlipped = Owner.direction == 1;

            projectile.timeLeft = 200;

            bool punch = CurrentState.Contains("PUNCH") || CurrentState == "SLAM" || CurrentState == "THROW_KNIVES" || CurrentState == TIMESTOP_ANIMATION;
            if (punch && !CurrentState.Contains("RUSH"))
                Owner.heldProj = projectile.whoAmI;

            if (IsPunching)
            {
                ImmuneTime = 32;
            }

            if (CurrentState == "THROW_KNIVES")
                Damage = 0;

            if (CurrentState == ANIMATION_DESPAWN)
            {
                Opacity -= 0.2f;
                if (!ReversedAnimation)
                {
                    CurrentAnimation.ResetAnimation(true);
                    ReversedAnimation = true;
                }
                PositionOffset = Owner.Center + new Vector2(XPosOffset * Owner.direction, YPosOffset + Owner.gfxOffY);
            }

            if (CurrentState == ANIMATION_SUMMON)
            {
                XPosOffset = -16;
                YPosOffset = -24;
                PositionOffset = Owner.Center + new Vector2(XPosOffset * Owner.direction, YPosOffset + Owner.gfxOffY);

            }

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
                        Projectile.NewProjectile(projectile.Center + direction * 2.2f + new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-34, 34)).RotatedBy(direction.ToRotation()), direction, ModContent.ProjectileType<Knife>(), KnifeDamage, 3.5f, Owner.whoAmI, projectile.whoAmI);
                    }
                }
            }


            if (CurrentState == ANIMATION_DESPAWN && CurrentAnimation.Finished && TimeStopDelay <= 0)
                KillStand();

            if (IsIdling)
            {
                XPosOffset = -16;
                YPosOffset = -24;
                PositionOffset = Owner.Center + new Vector2(XPosOffset * Owner.direction, YPosOffset + Owner.gfxOffY);

                TBAPlayer tPlayer = TBAPlayer.Get(Owner);

                if (Combos["Barrage"].CheckCombo(tPlayer))
                {
                    Vector2 startPos = Owner.Center - new Vector2(-32 * Owner.direction, 16);
                    StateQueue.Add("PUNCH_" + (CurrentState == "PUNCH_R" ? "L" : "R"));

                    PunchRushDirection = GetRange(startPos, Main.MouseWorld);

                    PositionOffset = startPos + PunchRushDirection;
                    BarrageTime = 180;
                }

                if (Combos["Time Stop"].CheckCombo(tPlayer))
                {
                    TBAMod.PlayVoiceLine("Sounds/TheWorld/TimeStop");
                    TimeStop();
                }

                if (Combos["Road"].CheckCombo(TBAPlayer.Get(Owner)))
                {
                    if (!BeganAscending)
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
                }

                if (Owner.whoAmI == Main.myPlayer)
                {
                    if (TBAInputs.StandPose.JustPressed)
                        if (CurrentState == ANIMATION_IDLE)
                            IsTaunting = true;
                        else
                            IsTaunting = false;

                    if (TBAInputs.ExtraAction02.JustPressed && TBAPlayer.Get(Owner).CheckStaminaCost(10))
                    {
                        StateQueue.Clear();
                        CurrentState = "THROW_KNIVES";
                    }
                    if (TBAInputs.SummonStand.JustPressed)
                        CurrentState = ANIMATION_DESPAWN;
                }


            }

            if (TimeStopDelay > 1)
                TimeStopDelay--;
            else if (TimeStopDelay == 1)
            {
                if (!TimeStopManagement.TimeStopped)
                {
                    Projectile.NewProjectile(Owner.Center, Vector2.Zero, ModContent.ProjectileType<TimeStopVFX>(), 0, 0, Owner.whoAmI);

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/TheWorld/TheWorld_ZaWarudoSFX"));
                }

                TimeStopManagement.ToggleTimeStopIfStopper(TBAPlayer.Get(Owner), 11 * Constants.TICKS_PER_SECOND);
                TimeStopDelay--;
            }

            if (BeganAscending)
            {
                if (AscensionTimer >= (int)(2 * Constants.TICKS_PER_SECOND))
                {
                    XPosOffset = 8;
                    YPosOffset = -40;
                    if (RoadRollerXAxis == -1.0f)
                        RoadRollerXAxis = Main.MouseWorld.X;

                    TBAPlayer.Get(Owner).PointOfInterest = new Vector2(MathHelper.Lerp(TBAPlayer.Get(Owner).PointOfInterest.X, RoadRollerXAxis, 0.1f), TBAPlayer.Get(Owner).PointOfInterest.Y);
                }

                if (++AscensionTimer >= (int)(2.5 * Constants.TICKS_PER_SECOND))
                {
                    XPosOffset = 16;
                    YPosOffset = -48;
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
                    RoadRoller r = RoadaRollaDa.modProjectile as RoadRoller;

                    r.ExplosionDamage = RoadRollerExplosionDamage;
                }
                else
                {
                    Owner.velocity = Vector2.Zero;
                    Owner.Center -= new Vector2(0f, 10f);
                }

                PositionOffset = Owner.Center + new Vector2(XPosOffset * Owner.direction, YPosOffset + Owner.gfxOffY);
            }

            if(Animations.Count > 0)
            {
                Animations["PUNCHRUSH_UP"].AutoLoop = BarrageTime > 0;
                Animations["PUNCHRUSH_DOWN"].AutoLoop = BarrageTime > 0;
                Animations["PUNCHRUSH_MID"].AutoLoop = BarrageTime > 0;
            }

            if (IsBarraging)
            {
                PunchCounterReset = 0;
                if (BarrageTime >= 180)
                {
                    if (Center.Y > Owner.Center.Y + 16)
                        CurrentState = "PUNCHRUSH_DOWN";
                    else if (Center.Y < Owner.Center.Y - 40)
                        CurrentState = "PUNCHRUSH_UP";
                    else
                        CurrentState = "PUNCHRUSH_MID";

                    int barrage = Projectile.NewProjectile(projectile.Center, VectorHelpers.DirectToMouse(projectile.Center, 18f), ModContent.ProjectileType<WorldBarrage>(), BarrageDamage, 0, Owner.whoAmI);

                    if (Main.projectile[barrage].modProjectile is WorldBarrage Barrage)
                    {
                        Barrage.RushDirection = VectorHelpers.DirectToMouse(projectile.Center, 26f);
                        Barrage.ParentProjectile = projectile.whoAmI;
                    }
                }

                BarrageTime--;
            }

            if (RoadaRollaDa.modProjectile is RoadRoller roller)
            {
                if (RushTimer > 0 && roller.HasTouchedGround || !roller.HasTouchedGround)
                {
                    Owner.direction = RoadaRollaDa.spriteDirection * -1;
                    Center = RoadaRollaDa.Center + new Vector2(40 * Owner.direction * -1, -40);
                    Owner.Center = Center + new Vector2(20 * Owner.direction * -1, -20);

                    PositionOffset = Owner.Center + new Vector2(XPosOffset * Owner.direction, YPosOffset + Owner.gfxOffY);
                }

                if (roller.HasTouchedGround && !HasResetRoadRollerDrop)
                {
                    TBAMod.PlayVoiceLine("Sounds/TheWorld/MudaRush");
                    CurrentState = "RUSH_DOWN";

                    RushTimer = 180;

                    PunchRushDirection = (RoadaRollaDa.Center - Owner.Center).SafeNormalize(-Vector2.UnitX) * 12f;

                    TBAPlayer.Get(Owner).AttackDirectionResetTimer = RushTimer;
                    TBAPlayer.Get(Owner).AttackDirection = RoadaRollaDa.Center.X < projectile.Center.X ? -1 : 1;

                    TBAPlayer.Get(Owner).PointOfInterest = Vector2.Zero;

                    HasResetRoadRollerDrop = true;

                    int barrage = Projectile.NewProjectile(projectile.Center, PunchRushDirection, ModContent.ProjectileType<WorldBarrage>(), (int)(BarrageDamage * 0.5), 0, Owner.whoAmI);

                    if (Main.projectile[barrage].modProjectile is WorldBarrage worldBarrage)
                    {
                        worldBarrage.RushDirection = PunchRushDirection;
                        worldBarrage.ParentProjectile = projectile.whoAmI;
                    }
                }
            }

            Center = Vector2.Lerp(projectile.Center, PositionOffset, 0.26f);
        }

        public override bool PreKill(int timeLeft)
        {
            TBAPlayer.Get(Owner).PointOfInterest = Vector2.Zero;

            return base.PreKill(timeLeft);
        }

        public override string TimeStopRestorePath => "Sounds/TheWorld/TheWorld_ZaWarudoReleaseSFX";

        public override string TimeStopVoiceLinePath => "Sounds/TheWorld/TimeStop";

        public override bool CanDie => CurrentState != TIMESTOP_ANIMATION && RushTimer <= 0;

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

        public override bool CanAcquire(TBAPlayer tbaPlayer)
        {
            return true;
        }

        public int RoadRollerID { get; private set; }

        public bool HasResetRoadRollerDrop { get; private set; }
        public Projectile RoadaRollaDa => Main.projectile[RoadRollerID];

        public bool BeganAscending { get; private set; }

        public int AscensionTimer { get; private set; }

        public float RoadRollerXAxis { get; private set; }

        public override int PunchDamage => 32 + (int)(BaseDamage * 1.2);

        public int RoadRollerExplosionDamage => 300 + (int)(BaseDamage * 10);

        public int KnifeDamage => 5 + (int)(BaseDamage * 0.2);

        public bool ReversedAnimation { get; private set; }

        public override int BarrageDamage => 48 + (int)(BaseDamage * 0.66);
    }
}
