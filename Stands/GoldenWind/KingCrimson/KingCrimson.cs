using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Drawing;
using TerrarianBizzareAdventure.Enums;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Projectiles;
using TerrarianBizzareAdventure.TimeSkip;
using WebmilioCommons.Tinq;

namespace TerrarianBizzareAdventure.Stands.GoldenWind.KingCrimson
{
    public class KingCrimson : PunchBarragingStand
    {
        public KingCrimson() : base("howDoesItWork", "King Crimson")
        {
            CallSoundPath = "Sounds/KingCrimson/KC_Call";
            AuraColor = new Color(189, 0, 85);
        }

        public override void AddCombos()
        {
            Combos.Add("Heart Ripper",
                   new StandCombo(
                       MouseClick.LeftClick.ToString(),
                       TBAInputs.EA1Bind(),
                       TBAInputs.CABind(),
                       MouseClick.RightClick.ToString()
                       )
                   );

            Combos.Add("Slice N' Dice",
                new StandCombo(
                    MouseClick.RightClick.ToString(),
                    MouseClick.RightClick.ToString(),
                    TBAInputs.Up,
                    MouseClick.LeftClick.ToString()
                    )
                );

            Combos.Add("Tomato Sauce Special",
                new StandCombo(
                    TBAInputs.Down,
                    TBAInputs.CABind(),
                    MouseClick.LeftClick.ToString()
                    )
                );

            Combos.Add("Barrage",
                new StandCombo(
                    MouseClick.LeftClick.ToString(),
                    MouseClick.RightClick.ToString(),
                    TBAInputs.CABind()
                    )
                );

            Combos.Add("Time Erase",
                new StandCombo(
                    TBAInputs.Up,
                    TBAInputs.Down,
                    TBAInputs.Down,
                    TBAInputs.CABind()
                    )
                );
        }


        public override void AddAnimations()
        {
            string basePath = "Stands/GoldenWind/KingCrimson/";
            #region Mandatory

            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(basePath +"KCSpawn", 7, 4));
            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(basePath + "KCIdle", 5, 22, true));
            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(basePath + "KCSpawn", 7, 4));
            #endregion

            #region Punch
            Animations.Add("PUNCH_R", new SpriteAnimation(basePath + "KCPunchRight", 4, 5));
            Animations.Add("PUNCH_L", new SpriteAnimation(basePath + "KCPunchLeft", 4, 5));

            Animations.Add("PUNCH_RU", new SpriteAnimation(basePath + "KCPunchRightU", 4, 5));
            Animations.Add("PUNCH_LU", new SpriteAnimation(basePath + "KCPunchLeftU", 4, 5));

            Animations.Add("PUNCH_RD", new SpriteAnimation(basePath + "KCPunchRightD", 4, 5));
            Animations.Add("PUNCH_LD", new SpriteAnimation(basePath + "KCPunchLeftD", 4, 5));

            Animations["PUNCH_R"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_L"].SetNextAnimation(Animations[ANIMATION_IDLE]);

            Animations["PUNCH_RU"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_LU"].SetNextAnimation(Animations[ANIMATION_IDLE]);

            Animations["PUNCH_RD"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_LD"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            #endregion

            #region Donut
            Animations.Add("DONUT_PREP", new SpriteAnimation(basePath + "KCDonutPrep", 15, 4));
            Animations.Add("DONUT_IDLE", new SpriteAnimation(basePath + "KCDonutIdle", 7, 15, true));
            Animations.Add("DONUT_ATT", new SpriteAnimation(basePath + "KCDonutCommit", 6, 4));
            Animations.Add("DONUT_UNDO", new SpriteAnimation(basePath + "KCDonutUndo", 12, 4));
            Animations.Add("DONUT_MISS", new SpriteAnimation(basePath + "KCDonutMiss", 7, 4));

            Animations.Add("BLIND", new SpriteAnimation(basePath + "KCBlind", 11, 3));
            Animations["BLIND"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            DrawInFrontStates.Add("BLIND");

            Animations["DONUT_PREP"].SetNextAnimation(Animations["DONUT_ATT"]);
            Animations["DONUT_ATT"].SetNextAnimation(Animations["DONUT_MISS"]);
            Animations["DONUT_UNDO"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["DONUT_MISS"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            #endregion

            Animations.Add("POSE_PREP", new SpriteAnimation(basePath + "KCPoseTransition", 8, 5));
            Animations.Add("POSE_END", new SpriteAnimation(basePath + "KCPoseTransition", 8, 5, false, null, true));
            Animations.Add("POSE_IDLE", new SpriteAnimation(basePath + "KCPoseIdle", 10, 22, true));

            Animations["POSE_PREP"].SetNextAnimation(Animations["POSE_IDLE"]);
            Animations["POSE_END"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["POSE_END"].ReversePlayback = true;

            #region Rush
            Animations.Add("RUSH_MID", new SpriteAnimation(basePath + "KCRush", 4, 4));
            Animations.Add("RUSH_UP", new SpriteAnimation(basePath + "KCRushUp", 4, 4));
            Animations.Add("RUSH_DOWN", new SpriteAnimation(basePath + "KCRushDown", 4, 4));

            Animations["RUSH_UP"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["RUSH_MID"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["RUSH_DOWN"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            #endregion

            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);

            #region Cut
            Animations.Add("CUT_IDLE", new SpriteAnimation(basePath + "KCCutIdle", 5, 25, true));

            Animations.Add("CUT_PREP", new SpriteAnimation(basePath + "KCCut", 20, 3));
            Animations.Add("CUT_ATT", new SpriteAnimation(basePath + "KCYeet", 13, 3));

            Animations["CUT_ATT"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["CUT_PREP"].SetNextAnimation(Animations["CUT_ATT"]);
            Animations["CUT_IDLE"].SetNextAnimation(Animations["CUT_ATT"]);
            #endregion

            DrawInFrontStates.Add("DONUT_PREP");
            DrawInFrontStates.Add("DONUT_ATT");
            DrawInFrontStates.Add("DONUT_UNDO");
            DrawInFrontStates.Add(ANIMATION_DESPAWN);
        }


        public override void AI()
        {
            base.AI();

            if (Animations.Count <= 0)
                return;

            TBAPlayer tPlayer = TBAPlayer.Get(Owner);

            OwnerCtrlUse = Owner.controlUseTile;


            projectile.width = (int)CurrentAnimation.FrameSize.X;
            projectile.height = (int)CurrentAnimation.FrameSize.Y;

            IsFlipped = Owner.direction == 1;

            projectile.timeLeft = 200;
            projectile.friendly = true;

            if (IsSpawning)
            {
                PositionOffset = Owner.Center + new Vector2(-16 * Owner.direction, -24 + Owner.gfxOffY);
                Opacity = CurrentAnimation.FrameRect.Y / CurrentAnimation.FrameRect.Height * 0.25f;
            }

            if (InIdleState)
            {
                PositionOffset = Owner.Center + new Vector2(-16 * Owner.direction, -24 + Owner.gfxOffY);

                if (Main.myPlayer == Owner.whoAmI)
                {
                    if (TBAInputs.SummonStand.JustPressed)
                        CurrentState = ANIMATION_DESPAWN;
                }

                Damage = 0;

                if (Combos["Tomato Sauce Special"].CheckCombo(TBAPlayer.Get(Owner)))
                {
                    CurrentState = "BLIND";
                }

                if (Combos["Heart Ripper"].CheckCombo(TBAPlayer.Get(Owner)))
                {
                    CurrentState = "DONUT_PREP";
                }

                if (Combos["Slice N' Dice"].CheckCombo(TBAPlayer.Get(Owner)))
                {
                    CurrentState = "CUT_PREP";
                }

                if (Combos["Barrage"].CheckCombo(TBAPlayer.Get(Owner)))
                {
                    Vector2 startPos = Owner.Center - new Vector2(-32 * Owner.direction, 16);
                    CurrentState = "PUNCH_" + (CurrentState == "PUNCH_R" ? "L" : "R");

                    PunchRushDirection = GetRange(startPos, Main.MouseWorld);

                    PositionOffset = startPos + PunchRushDirection;
                    BarrageTime = 180;
                }

                if(Combos["Time Erase"].CheckCombo(TBAPlayer.Get(Owner)))
                {
                    TBAMod.PlayVoiceLine("Sounds/KingCrimson/KC_Call");
                    EraseTime();
                }
            }

            if (PunchCounterReset > 0)
            {
                Owner.heldProj = projectile.whoAmI;

                PositionOffset = Owner.Center + new Vector2(8 * Owner.direction, -24 + Owner.gfxOffY);
            }

            if (IsPunching)
            {
                if (!IsBarraging)
                {
                    CurrentAnimation.FrameSpeed = 5;
                    if (CurrentAnimation.CurrentFrame == 1)
                        Damage = 60;

                    Owner.heldProj = projectile.whoAmI;
                }
                Owner.direction = Center.X < Owner.Center.X ? -1 : 1;

                IsFlipped = Owner.direction == 1;

                PositionOffset = Owner.Center - new Vector2(-8 * Owner.direction, 16) + PunchRushDirection;
            }

            if (CanPunch)
            {
                if ((tPlayer.MouseOneTimeReset > 0 || tPlayer.MouseTwoTimeReset > 0) && !Owner.controlUseItem && !Owner.controlUseTile)
                {
                    ImmuneTime = 20;

                    Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;

                    Vector2 startPos = Owner.Center - new Vector2(-32 * Owner.direction, 16);

                    PunchRushDirection = GetRange(startPos, Main.MouseWorld);

                    PositionOffset = startPos + PunchRushDirection;

                    if (Main.MouseWorld.Y > Owner.Center.Y + 90)
                        CurrentState = "PUNCH_" + (tPlayer.MouseTwoTimeReset > 0 ? "L" : "R") + "D";
                    else if (Main.MouseWorld.Y < Owner.Center.Y - 90)
                        CurrentState = "PUNCH_" + (tPlayer.MouseTwoTimeReset > 0 ? "L" : "R") + "U";
                    else
                        CurrentState = "PUNCH_" + (tPlayer.MouseTwoTimeReset > 0 ? "L" : "R");

                    PunchCounterReset = 90;
                }
            }

            if (IsBarraging)
            {
                PunchCounterReset = 0;
                if (BarrageTime >= 180)
                {
                    int barrage = Projectile.NewProjectile(projectile.Center, VectorHelpers.DirectToMouse(projectile.Center, 18f), ModContent.ProjectileType<CrimsonBarrage>(), 60, 0, Owner.whoAmI);

                    if (Main.projectile[barrage].modProjectile is CrimsonBarrage Barrage)
                    {
                        Barrage.RushDirection = VectorHelpers.DirectToMouse(projectile.Center, 18f);
                        Barrage.ParentProjectile = projectile.whoAmI;
                    }
                }

                if (CurrentAnimation.CurrentFrame > 2)
                {
                    CurrentAnimation.ResetAnimation();

                    if (Center.Y > Owner.Center.Y + 16)
                        CurrentState = "PUNCH_" + (CurrentState == "PUNCH_RD" ? "LD" : "RD");
                    else if (Center.Y < Owner.Center.Y - 40)
                        CurrentState = "PUNCH_" + (CurrentState == "PUNCH_RU" ? "LU" : "RU");
                    else
                        CurrentState = "PUNCH_" + (CurrentState == "PUNCH_R" ? "L" : "R");

                    CurrentAnimation.FrameSpeed = 4;
                    
                    CurrentAnimation.CurrentFrame = 1;
                }

                BarrageTime--;
            }

            if (IsDespawning)
            {
                Opacity = (5 - CurrentAnimation.FrameRect.Y / (int)CurrentAnimation.FrameSize.Y) * 0.2f;

                if (CurrentAnimation.Finished)
                    KillStand();

                Center = Vector2.Lerp(Center, PositionOffset, 0.4f);

                PositionOffset = Owner.Center - new Vector2(0, 12);
            }

            if(DrawInFront)
                Owner.heldProj = projectile.whoAmI;

            Center = Vector2.Lerp(Center, PositionOffset, 0.26f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);

            writer.Write(OwnerCtrlUse);
            writer.Write(RushTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);

            OwnerCtrlUse = reader.ReadBoolean();
            RushTimer = reader.ReadInt32();
        }

        public void EraseTime()
        {
            if (TimeSkipManager.TimeSkippedFor <= 0 && TBAPlayer.Get(Owner).CheckStaminaCost(25))
            {
                TBAPlayer.Get(Owner).TirePlayer(15);
                Projectile.NewProjectile(Center, Vector2.Zero, ModContent.ProjectileType<FakeTilesProjectile>(), 0, 0, Owner.whoAmI);
                TimeSkipManager.SkipTime(TBAPlayer.Get(Owner), Constants.TICKS_PER_SECOND * 10 + 26);
            }
            else if (TimeSkipManager.TimeSkippedFor > 0)
            {
                Main.projectile.FirstActive(x => x.modProjectile is FakeTilesProjectile).timeLeft = 30;
                TimeSkipManager.SkipTime(TBAPlayer.Get(Owner), 36);
            }
        }

        public bool IsPunching => CurrentState.Contains("PUNCH");

        public bool CanPunch => InIdleState; 

        public override bool StopsItemUse => !Main.SmartCursorEnabled;

        public override bool CanDie => RushTimer <= 0;

        public bool HasMissedDonut { get; set; }

        public bool OwnerCtrlUse { get; set; }
    }
}
