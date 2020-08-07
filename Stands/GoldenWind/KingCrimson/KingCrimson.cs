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

            Combos.Add("Heart Ripper", new StandCombo(TBAInputs.EA1Bind(), TBAInputs.EA1Bind(), TBAInputs.CABind(), MouseClick.LeftClick.ToString()));
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

            Animations.Add("BLIND", new SpriteAnimation(basePath + "KCBlind", 11, 4));

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
            Animations["CUT_PREP"].SetNextAnimation(Animations["CUT_IDLE"]);
            Animations["CUT_IDLE"].SetNextAnimation(Animations["CUT_ATT"]);
            #endregion
        }


        public override void AI()
        {
            base.AI();

            if (Animations.Count <= 0)
                return;

            PositionOffset = Owner.Center + new Vector2(-16 * Owner.direction, -24 + Owner.gfxOffY);
            Center = Vector2.Lerp(Center, PositionOffset, 0.26f);

            OwnerCtrlUse = Owner.controlUseTile;


            projectile.width = (int)CurrentAnimation.FrameSize.X;
            projectile.height = (int)CurrentAnimation.FrameSize.Y;

            IsFlipped = Owner.direction == 1;

            projectile.timeLeft = 200;
            projectile.friendly = true;

            Opacity = 1f;

            if (InIdleState)
            {
                if (Main.myPlayer == Owner.whoAmI)
                {
                    if (TBAInputs.SummonStand.JustPressed)
                        CurrentState = ANIMATION_DESPAWN;
                }

                if (Combos["Heart Ripper"].CheckCombo(TBAPlayer.Get(Owner)))
                    CurrentState = "DONUT_PREP";
            }

            if(IsDespawning)
            {
                if (CurrentAnimation.Finished)
                    KillStand();
            }
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

        public bool CanPunch => InIdleState || (CurrentState.Contains("PUNCH") && CurrentAnimation.CurrentFrame >= 3); 

        public override bool StopsItemUse => !Main.SmartCursorEnabled;

        public override bool CanDie => RushTimer <= 0;

        public bool HasMissedDonut { get; set; }

        public bool OwnerCtrlUse { get; set; }
    }
}
