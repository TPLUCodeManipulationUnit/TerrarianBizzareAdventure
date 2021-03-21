using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Drawing;
using TerrarianBizzareAdventure.Enums;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Projectiles;
using TerrarianBizzareAdventure.Projectiles.Misc;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Stands.StardustCrusaders.StarPlatinum
{
    public class StarPlatinumStand : TimeStoppingStand
    {
        private const string
            TEXPATH = "Stands/StardustCrusaders/StarPlatinum/",
            PUNCH = "SPPunch_",
            LEFTHAND = "_LeftHand",
            RIGHTHAND = "_RightHand";


        public StarPlatinumStand() : base("starPlatinum", "Star Platinum")
        {
            CallSoundPath = "Sounds/StarPlatinum/SP_Call";
            AuraColor = new Color(1f, 0f, 1f);//new Color(210, 101, 198);//new Color(203, 85, 195);
        }

        public override void AddCombos()
        {
            Combos.Add("Barrage",
                   new StandCombo("Crusader's Judgement",
                       TBAInputs.LeftClick,
                       TBAInputs.RightClick,
                       TBAInputs.CABind()
                       )
                   );
            Combos.Add("Time Stop",
                    new StandCombo("Star Platinum: The World",
                        TBAInputs.CABind(),
                        TBAInputs.Up,
                        TBAInputs.CABind(),
                        TBAInputs.Down,
                        TBAInputs.CABind()
                        )
                    ); 
            Combos.Add("Uppercut",
                     new StandCombo("Jawbreaker",
                         TBAInputs.Up,
                         TBAInputs.Up,
                         TBAInputs.LeftClick
                         )
                     );
        }

        public override void AddAnimations()
        {

            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(TEXPATH + "SPSummon", 10, 4));
            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(TEXPATH + "SPIdle", 14, 4, true));

            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);

            Animations.Add("PUNCH_L", new SpriteAnimation(TEXPATH + PUNCH + "Middle" + LEFTHAND, 3, 5, false, Animations[ANIMATION_IDLE]) );
            Animations.Add("PUNCH_R", new SpriteAnimation(TEXPATH + PUNCH + "Middle" + RIGHTHAND, 3, 5, false, Animations[ANIMATION_IDLE]) );

            Animations.Add("PUNCH_LD", new SpriteAnimation(TEXPATH + PUNCH + "Down" + LEFTHAND, 3, 5, false, Animations[ANIMATION_IDLE]) );
            Animations.Add("PUNCH_RD", new SpriteAnimation(TEXPATH + PUNCH + "Down" + RIGHTHAND, 3, 5, false, Animations[ANIMATION_IDLE]) );

            Animations.Add("PUNCH_LU", new SpriteAnimation(TEXPATH + PUNCH + "Up" + LEFTHAND, 3, 5, false, Animations[ANIMATION_IDLE]) );
            Animations.Add("PUNCH_RU", new SpriteAnimation(TEXPATH + PUNCH + "Up" + RIGHTHAND, 3, 5, false, Animations[ANIMATION_IDLE]) );

            Animations.Add("POSE_TRANSITION", new SpriteAnimation(TEXPATH + "SPPose_Transition", 15, 4));
            Animations.Add("POSE_TRANSITION_REVERSE", new SpriteAnimation(TEXPATH + "SPPose_Transition", 15, 4, false, Animations[ANIMATION_IDLE]));
            Animations.Add("POSE_IDLE", new SpriteAnimation(TEXPATH + "SPPose_Idle", 11, 6, true, Animations[ANIMATION_IDLE]));

            Animations["POSE_TRANSITION"].SetNextAnimation(Animations["POSE_IDLE"]);

            Animations.Add("RUSH_UP", new SpriteAnimation(TEXPATH + "SPRush_Up", 4, 4));
            Animations.Add("RUSH_DOWN", new SpriteAnimation(TEXPATH + "SPRush_Down", 4, 4));
            Animations.Add("RUSH_MIDDLE", new SpriteAnimation(TEXPATH + "SPRush_Middle", 4, 4));

            Animations.Add("BLOCK_IDLE", new SpriteAnimation(TEXPATH + "SPBlockIdle", 7, 6, true));

            Animations.Add("BLOCK_TRANSITION", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPBlockTransition"), 11, 3, false, Animations["BLOCK_IDLE"]));

            Animations.Add("BLOCK_TRANSITION_REVERSE", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPBlockTransition"), 11, 3, false, Animations[ANIMATION_IDLE]));

            Animations["BLOCK_IDLE"].SetNextAnimation(Animations["BLOCK_TRANSITION_REVERSE"], true);

            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(mod.GetTexture(TEXPATH + "SPDespawn"), 6, 4));

            Animations.Add(TIMESTOP_ANIMATION, new SpriteAnimation(mod.GetTexture(TEXPATH + "SPPose_Transition"), 15, 4));

            Animations.Add("DONUT_IDLE", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPDonutIdle"), 4, 6, true));
            Animations.Add("DONUT_PREP", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPDonutTransition"), 13, 5));
            Animations.Add("DONUT_PUNCH", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPDonutPunch"), 15, 3));
            Animations.Add("DONUT_PULL", new SpriteAnimation(mod.GetTexture(TEXPATH + "SPDonutRetract"), 8, 5));

            Animations["DONUT_PULL"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["DONUT_PREP"].SetNextAnimation(Animations["DONUT_PUNCH"]);
            Animations["DONUT_PUNCH"].SetNextAnimation(Animations["DONUT_PULL"]);

            Animations["RUSH_UP"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["RUSH_MIDDLE"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["RUSH_DOWN"].SetNextAnimation(Animations[ANIMATION_IDLE]);
        }

        public override void AI()
        {
            base.AI();
            projectile.friendly = true;

            if (Animations.Count <= 0)
                return;

            if (BaseDamage <= 0)
                GetBaseDamage(DamageClass.Melee, Owner);

            if (CurrentState == ANIMATION_SUMMON)
            {
                if (CurrentAnimation.CurrentFrame < 3)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/StarPlatinum/SP_Spawn"));

                Opacity = CurrentAnimation.FrameRect.Y / CurrentAnimation.FrameRect.Height * 0.25f;

                XPosOffset = -16;
                YPosOffset = -24;

                PositionOffset = Owner.Center + new Vector2(XPosOffset * Owner.direction, YPosOffset + Owner.gfxOffY);
            }

            if(IsIdling)
                XPosOffset = -16;

            #region Rush


            if (Animations.Count > 0)
            {
                Animations["RUSH_DOWN"].AutoLoop = RushTimer > 0;
                Animations["RUSH_UP"].AutoLoop = RushTimer > 0;
                Animations["RUSH_MIDDLE"].AutoLoop = RushTimer > 0;
            }

            #endregion

            TimeLeft = 200;

            // Runs on clients only
            if (Owner.whoAmI == Main.myPlayer)
            {
                if (TBAInputs.StandPose.JustPressed)
                    if (CurrentState == ANIMATION_IDLE)
                        IsTaunting = true;
                    else
                        IsTaunting = false;

                if (TBAInputs.SummonStand.JustPressed && CurrentState == ANIMATION_IDLE)
                    CurrentState = ANIMATION_DESPAWN;

                
            }

            if(IsIdling)
            {
                ImmuneTime = 20;

                PositionOffset = Owner.Center + new Vector2(XPosOffset * Owner.direction, YPosOffset + Owner.gfxOffY);

                TBAPlayer tPlayer = TBAPlayer.Get(Owner);

                if (Combos["Time Stop"].CheckCombo(tPlayer))
                {
                    TBAMod.PlayVoiceLine("Sounds/StarPlatinum/SP_TimeStopCall");
                    TimeStop();
                }

                if(Combos["Barrage"].CheckCombo(tPlayer))
                {
                    Vector2 startPos = Owner.Center - new Vector2(-32 * Owner.direction, 16);
                    TBAMod.PlayVoiceLine("Sounds/StarPlatinum/Ora");
                    StateQueue.Add("PUNCH_" + (CurrentState == "PUNCH_R" ? "L" : "R"));

                    PunchRushDirection = GetRange(startPos, Main.MouseWorld);

                    PositionOffset = startPos + PunchRushDirection;
                    BarrageTime = 180;
                }

                if(Combos["Uppercut"].CheckCombo(tPlayer))
                {
                    StateQueue.Clear();
                    CurrentState = "DONUT_PREP";
                }
            }

            if(CurrentState == "DONUT_PUNCH")
            {
                ImmuneTime = 40;
            }

            if(CurrentState == "DONUT_PUNCH" && CurrentAnimation.CurrentFrame == 4)
               TBAMod.PlayVoiceLine("Sounds/StarPlatinum/Donut");

            if (CurrentState == "DONUT_PUNCH" && CurrentAnimation.CurrentFrame > 4)
                Damage = 350;

            Center = Vector2.Lerp(projectile.Center, PositionOffset, 0.26f);


            if (IsTaunting)
            {
                PositionOffset = Owner.Center + new Vector2(48 * Owner.direction, YPosOffset + Owner.gfxOffY);

                if (!CurrentState.Contains("POSE"))
                {
                    CurrentState = "POSE_TRANSITION";
                    CurrentAnimation.ResetAnimation();
                }
            }

            if (IsBarraging)
            {
                PunchCounterReset = 0;
                if (BarrageTime >= 180)
                {
                    int barrage = Projectile.NewProjectile(projectile.Center, VectorHelpers.DirectToMouse(projectile.Center, 18f), ModContent.ProjectileType<StarBarrage>(), BarrageDamage, 0, Owner.whoAmI);

                    if (Main.projectile[barrage].modProjectile is StarBarrage Barrage)
                    {
                        Barrage.RushDirection = VectorHelpers.DirectToMouse(projectile.Center, 18f);
                        Barrage.ParentProjectile = projectile.whoAmI;
                    }
                }

                if (CurrentAnimation.CurrentFrame > 1)
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


            if (CurrentState == ANIMATION_DESPAWN)
            {
                Opacity = (5 - CurrentAnimation.FrameRect.Y / (int)CurrentAnimation.FrameSize.Y) * 0.2f;

                XPosOffset += 1;
                YPosOffset += 0.75f;

                PositionOffset = Owner.Center + new Vector2(XPosOffset * Owner.direction, YPosOffset + Owner.gfxOffY);

                if (CurrentAnimation.Finished)
                    KillStand();
            }

            IsFlipped = Owner.direction == 1;

            #region Time Stop

            if (TimeStopDelay > 1)
                TimeStopDelay--;
            else if (TimeStopDelay == 1)
            {
                projectile.netUpdate = true;
                if (!TimeStopManagement.TimeStopped)
                {
                    Projectile.NewProjectile(Owner.Center, Vector2.Zero, ModContent.ProjectileType<TimeStopVFX>(), 0, 0, Owner.whoAmI);

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/StarPlatinum/SP_TimeStopSignal"));
                }

                TimeStopManagement.ToggleTimeStopIfStopper(TBAPlayer.Get(Owner), 11 * Constants.TICKS_PER_SECOND);
                TimeStopDelay--;
            }

            #endregion

            if (CurrentState == "POSE_IDLE" && !IsTaunting)
            {
                CurrentState = ANIMATION_IDLE;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);

            writer.Write(IsTaunting);
            writer.Write(RushTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);

            IsTaunting = reader.ReadBoolean();
            RushTimer = reader.ReadInt32();
        }

        public override bool CanDie => !CurrentState.Contains("DONUT") && CurrentState != TIMESTOP_ANIMATION && !CurrentState.Contains("RUSH") && RushTimer <= 0;

        public bool InPose { get; private set; }

        public override int PunchDamage => 6 + (int)(BaseDamage * 1.7);
        public override int BarrageDamage => 12 + (int)(BaseDamage * 1.2);
    }
}
