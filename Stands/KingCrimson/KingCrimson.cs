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

namespace TerrarianBizzareAdventure.Stands.KingCrimson
{
    public class KingCrimson : PunchBarragingStand
    {
        public KingCrimson() : base("howDoesItWork", "King Crimson")
        {
            CallSoundPath = "Sounds/KingCrimson/KC_Call";
            AuraColor = new Color(189, 0, 85);
        }

        public override void AddCombos(List<StandCombo> combos)
        {
            combos.Add(new StandCombo("Punch", MouseClick.LeftClick.ToString()));
            combos.Add(new StandCombo("Slicer", MouseClick.LeftHold.ToString()));
            combos.Add(new StandCombo("Heart Ripper", MouseClick.RightHold.ToString()));
            combos.Add(new StandCombo("Punch Barrage", MouseClick.LeftClick.ToString(), MouseClick.LeftClick.ToString(), MouseClick.LeftClick.ToString()));
            combos.Add(new StandCombo("Time Obliteration", TBAInputs.ContextAction.GetAssignedKeys()[0].ToString()));
        }

        public override void AddAnimations()
        {
            #region Mandatory
            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCSpawn"), 7, 4));
            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCIdle"), 5, 22));
            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCSpawn"), 7, 4));
            #endregion

            #region Punch
            Animations.Add("PUNCH_R", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPunchRight"), 4, 5));
            Animations.Add("PUNCH_L", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPunchLeft"), 4, 5));

            Animations.Add("PUNCH_RU", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPunchRightU"), 4, 5));
            Animations.Add("PUNCH_LU", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPunchLeftU"), 4, 5));

            Animations.Add("PUNCH_RD", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPunchRightD"), 4, 5));
            Animations.Add("PUNCH_LD", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPunchLeftD"), 4, 5));

            Animations["PUNCH_R"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_L"].SetNextAnimation(Animations[ANIMATION_IDLE]);

            Animations["PUNCH_RU"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_LU"].SetNextAnimation(Animations[ANIMATION_IDLE]);

            Animations["PUNCH_RD"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_LD"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            #endregion

            #region Donut
            Animations.Add("DONUT_PREP", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCDonutPrep"), 15, 4));
            Animations.Add("DONUT_IDLE", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCDonutIdle"), 7, 15, true));
            Animations.Add("DONUT_ATT", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCDonutCommit"), 6, 4));
            Animations.Add("DONUT_UNDO", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCDonutUndo"), 12, 4));
            Animations.Add("DONUT_MISS", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCDonutMiss"), 7, 4));

            Animations["DONUT_PREP"].SetNextAnimation(Animations["DONUT_IDLE"]);
            Animations["DONUT_ATT"].SetNextAnimation(Animations["DONUT_MISS"]);
            Animations["DONUT_UNDO"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["DONUT_MISS"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            #endregion

            Animations.Add("POSE_PREP", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPoseTransition"), 8, 5));
            Animations.Add("POSE_END", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPoseTransition"), 8, 5, false, null, true));
            Animations.Add("POSE_IDLE", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPoseIdle"), 10, 22, true));

            Animations["POSE_PREP"].SetNextAnimation(Animations["POSE_IDLE"]);
            Animations["POSE_END"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["POSE_END"].ReversePlayback = true;

            #region Rush
            Animations.Add("RUSH_MID", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCRush"), 4, 4));
            Animations.Add("RUSH_UP", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCRushUp"), 4, 4));
            Animations.Add("RUSH_DOWN", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCRushDown"), 4, 4));

            Animations["RUSH_UP"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["RUSH_MID"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["RUSH_DOWN"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            #endregion

            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);

            #region Cut
            Animations.Add("CUT_IDLE", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCCutIdle"), 5, 25, true));

            Animations.Add("CUT_PREP", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCCut"), 20, 3));
            Animations.Add("CUT_ATT", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCYeet"), 13, 3));

            Animations["CUT_ATT"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["CUT_PREP"].SetNextAnimation(Animations["CUT_IDLE"]);
            Animations["CUT_IDLE"].SetNextAnimation(Animations["CUT_ATT"]);
            #endregion
        }


        public override void AI()
        {
            base.AI();

            if (Animations.Count <= 0)
                return;

            OwnerCtrlUse = Owner.controlUseTile;


            projectile.width = (int)CurrentAnimation.FrameSize.X;
            projectile.height = (int)CurrentAnimation.FrameSize.Y;

            IsFlipped = Owner.direction == 1;

            projectile.timeLeft = 200;
            projectile.friendly = true;

            if (Owner.whoAmI == Main.myPlayer)
            {
                /*if (TBAInputs.StandPose.JustPressed)
                    if (CurrentState == ANIMATION_IDLE)
                        IsTaunting = true;  
                    else
                        IsTaunting = false;*/

                if (TBAInputs.SummonStand.JustPressed && CurrentState == ANIMATION_IDLE)
                    CurrentState = ANIMATION_DESPAWN;
            }
            int xOffset = IsTaunting || CurrentState.Contains("PUNCH") || CurrentState.Contains("ATT") || CurrentState == "DONUT_UNDO" ||  CurrentState == "DONUT_MISS" ||RushTimer > 0? 34 : -16;

            int yOffset = CurrentState.Contains("POSE") ? 36 : 0;

            xOffset = CurrentState.Contains("POSE") ? 24 : xOffset;


            Vector2 lerpPos = Owner.Center + new Vector2(xOffset * Owner.direction, -24 + Owner.gfxOffY + yOffset);

            projectile.Center = Vector2.Lerp(projectile.Center, lerpPos, 0.26f);

            if (CurrentState == ANIMATION_SUMMON)
            {
                Opacity = 1;
            }

            #region Punching
            if (CurrentState == ANIMATION_IDLE)
            {
                if(TBAInputs.StandPose.JustPressed && Owner.whoAmI == Main.myPlayer)
                {
                    CurrentState = "POSE_PREP";
                }

                if (PunchCounter < 2)
                {
                    projectile.netUpdate = true;
                    if (TBAPlayer.Get(Owner).MouseOneTimeReset > 0)
                    {
                        if (TBAPlayer.Get(Owner).MouseOneTime < 15 && !Owner.controlUseItem)
                        {
                            TBAPlayer.Get(Owner).Stamina -= 2;
                            Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;

                            if (Main.MouseWorld.Y > Owner.Center.Y + 60)
                                CurrentState = "PUNCH_" + (Main.rand.NextBool() ? "R" : "L") + "D";
                            else if (Main.MouseWorld.Y < Owner.Center.Y - 60)
                                CurrentState = "PUNCH_" + (Main.rand.NextBool() ? "R" : "L") + "U";
                            else
                                CurrentState = "PUNCH_" + (Main.rand.NextBool() ? "R" : "L");

                            PunchCounter++;

                            PunchCounterReset = 28;

                            Projectile.NewProjectile(projectile.Center, VectorHelpers.DirectToMouse(projectile.Center, 22f), ModContent.ProjectileType<Punch>(), 60, 3.5f, Owner.whoAmI, projectile.whoAmI);

                        }

                        if (TBAPlayer.Get(Owner).MouseOneTime >= 15)
                        {
                            TBAPlayer.Get(Owner).Stamina -= 10;
                            Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
                            CurrentState = "CUT_PREP";
                        }
                    }
                }
                else if(Owner.controlUseItem)
                {
                    projectile.netUpdate = true;

                    TBAPlayer.Get(Owner).Stamina -= 16;

                    if (Main.MouseWorld.Y > Owner.Center.Y + 60)
                        CurrentState = "RUSH_DOWN";

                    else if (Main.MouseWorld.Y < Owner.Center.Y - 60)
                        CurrentState = "RUSH_UP";

                    else
                        CurrentState = "RUSH_MID";

                    RushTimer = 180;

                    PunchRushDirection = VectorHelpers.DirectToMouse(projectile.Center, 18f);

                    TBAPlayer.Get(Owner).AttackDirectionResetTimer = RushTimer;
                    TBAPlayer.Get(Owner).AttackDirection = Main.MouseWorld.X < projectile.Center.X ? -1 : 1;
                    
                    int barrage = Projectile.NewProjectile(projectile.Center, PunchRushDirection, ModContent.ProjectileType<CrimsonBarrage>(), 60, 0, Owner.whoAmI);

                    if (Main.projectile[barrage].modProjectile is CrimsonBarrage crimsonBarrage)
                    {
                        crimsonBarrage.RushDirection = PunchRushDirection;
                        crimsonBarrage.ParentProjectile = projectile.whoAmI;
                    }
                }
            }
            #endregion

            #region Rush
            if (RushTimer > 0)
            {
                RushTimer--;
            }

            Animations["RUSH_DOWN"].AutoLoop = RushTimer > 0;
            Animations["RUSH_UP"].AutoLoop = RushTimer > 0;
            Animations["RUSH_MID"].AutoLoop = RushTimer > 0;
            #endregion

            if (CurrentState.Contains("PUNCH") || CurrentState.Contains("ATT") || CurrentState.Contains("UNDO") || CurrentState == "DONUT_MISS")
                Owner.heldProj = projectile.whoAmI;

            #region Yeet attacc
            // If we do a YEET attack, damage is dealt by stand itself instead of a seperate projectile
            bool yeeting = CurrentState == "CUT_ATT" && CurrentAnimation.CurrentFrame > 3 && CurrentAnimation.CurrentFrame < 9;
            bool donuting = CurrentState == "DONUT_ATT" && CurrentAnimation.CurrentFrame == 3;
            projectile.damage = yeeting ? 400 : 0;

            if (donuting && Owner.ownedProjectileCounts[ModContent.ProjectileType<DonutPunch>()] <= 1)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<DonutPunch>(), 60, 0, Owner.whoAmI, projectile.whoAmI, -1);

            if(CurrentState == "CUT_IDLE" && !Owner.controlUseItem)
            {
                CurrentAnimation.ResetAnimation();
                CurrentState = "CUT_ATT";
            }
            #endregion

            if(TBAPlayer.Get(Owner).MouseTwoTime > 15 && CurrentState == ANIMATION_IDLE)
            {
                TBAPlayer.Get(Owner).Stamina -= 10;
                CurrentState = "DONUT_PREP";
            }

            if(CurrentState == "DONUT_IDLE" && !OwnerCtrlUse)
            {
                CurrentAnimation.ResetAnimation();
                CurrentState = "DONUT_ATT";
            }
            
            Animations["DONUT_UNDO"].FrameSpeed = Animations["DONUT_UNDO"].CurrentFrame < 5 ? 12 : 5;



            if (CurrentState == ANIMATION_IDLE && CurrentAnimation.Finished)
                CurrentAnimation.ResetAnimation();

            if (CurrentState == ANIMATION_DESPAWN)
            {
                Opacity = (5 - CurrentAnimation.FrameRect.Y / (int)CurrentAnimation.FrameSize.Y) * 0.2f;

                if (CurrentAnimation.Finished)
                    KillStand();
            }

            if (CurrentState == ANIMATION_IDLE) 
                HasMissedDonut = true;

            if (CurrentState == "DONUT_ATT")
                Animations["DONUT_ATT"].SetNextAnimation(HasMissedDonut ? Animations["DONUT_MISS"] : Animations["DONUT_UNDO"]);

            if (PunchCounterReset > 0)
                PunchCounterReset--;
            else
                PunchCounter = 0;

            if (TBAInputs.ContextAction.JustPressed && Owner.whoAmI == Main.myPlayer && !CurrentState.Contains("RUSH"))
            {
                EraseTime();
            }


            if (TBAInputs.StandPose.JustPressed && Owner.whoAmI == Main.myPlayer && CurrentState == "POSE_IDLE")
            {
                CurrentState = "POSE_END";
            }

            if(CurrentState.Contains("POSE"))
            Owner.heldProj = projectile.whoAmI;
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
            if (TimeSkipManager.TimeSkippedFor <= 0 && TBAPlayer.Get(Owner).Stamina >= 25)
            {
                TBAPlayer.Get(Owner).Stamina -= 25;
                TimeSkipManager.SkipTime(TBAPlayer.Get(Owner), Constants.TICKS_PER_SECOND * 10 + 26);
            }
        }

        public bool HasMissedDonut { get; set; }

        public int PunchCounter { get; private set; }
        public int PunchCounterReset { get; private set; }

        public bool OwnerCtrlUse { get; set; }
    }
}
