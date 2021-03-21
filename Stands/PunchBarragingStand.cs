using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TerrarianBizzareAdventure.Enums;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.NPCs;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands
{
    public abstract class PunchBarragingStand : Stand
    {
        protected PunchBarragingStand(string unlocalizedName, string name) : base(unlocalizedName, name)
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            projectile.penetrate = 1;
        }

        public override void AI()
        {
            base.AI();

            TBAPlayer tPlayer = TBAPlayer.Get(Owner);

            if (PunchCounterReset > 0)
                PunchCounterReset--;
            else
                PunchCounter = 0;

            if (RushTimer > 0)
                RushTimer--;

            if (!Main.dedServ)
            {
                if (CurrentAnimation != null)
                {
                    Height = (int)CurrentAnimation.FrameSize.Y;
                    Width = (int)CurrentAnimation.FrameSize.X;
                }
            }

            HitNPCs.ForEach(x => x.LifeTime -= 1);

            HitNPCs.RemoveAll(x => x.LifeTime <= 0);

            if (IsIdling)
            {
                Damage = 0;
            }

            bool doPunch = (tPlayer.MouseOneTimeReset > 0 || tPlayer.MouseTwoTimeReset > 0) && !Owner.controlUseItem && !Owner.controlUseTile;

            if (CanPunch)
            {
                CommitPunch(tPlayer, doPunch);
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
                    if (CurrentAnimation.CurrentFrame <= 1)
                    {
                        Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;

                        Vector2 startPos = Owner.Center - new Vector2(-32 * Owner.direction, 16);

                        PunchRushDirection = GetRange(startPos, Main.MouseWorld);

                        PositionOffset = startPos + PunchRushDirection;
                    }

                    CurrentAnimation.FrameSpeed = 5;

                    Owner.heldProj = projectile.whoAmI;
                }

                if (doPunch && CurrentAnimation.CurrentFrame >= CurrentAnimation.FrameCount * 0.6 && StateQueue.Count > 0)
                    CurrentState = StateQueue[0];

                Owner.direction = Center.X < Owner.Center.X ? -1 : 1;

                IsFlipped = Owner.direction == 1;

                PositionOffset = Owner.Center - new Vector2(-8 * Owner.direction, 16) + PunchRushDirection;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            HitNPCs.Add(new HitNPCData(target, ImmuneTime));
            projectile.penetrate += 1;
            if (!TBAGlobalNPC.GetFor(target).IsCoolingOff)
                TBAGlobalNPC.GetFor(target).CL_LockTimer = 10;

            TBAPlayer.Get(Owner).CombatLockTimer = 10;
            TBAPlayer.Get(Owner).StoredVelocity = (target.Center - Owner.Center).SafeNormalize(-Vector2.UnitY) * 6f;
        }

        public Vector2 GetRange(Vector2 startPos, Vector2 endPos)
        {
            float rangeInMeters = AttackRange * 32;

            bool exceedsRange = Vector2.Distance(startPos, endPos) > rangeInMeters;

            Vector2 result = exceedsRange ? VectorHelpers.DirectToMouse(startPos, rangeInMeters) : endPos - startPos;

            return result;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return HitNPCs.Count(x => x.HitNPC == target) <= 0;
        }

        private void CommitPunch(TBAPlayer tPlayer, bool doPunch)
        {
            if (doPunch
                                || TBAInputs.ContextAction.JustPressed
                                || TBAInputs.ExtraAction01.JustPressed
                                || TBAInputs.ExtraAction02.JustPressed)
            {
                Damage = PunchDamage;
                string baseString = "PUNCH_" + (tPlayer.MouseTwoTimeReset > 0 ? "L" : "R");
                if (Main.MouseWorld.Y > Owner.Center.Y + 90)
                    StateQueue.Insert(0, baseString + "D");
                else if (Main.MouseWorld.Y < Owner.Center.Y - 90)
                    StateQueue.Insert(0, baseString + "U");
                else
                    StateQueue.Insert(0, baseString);

                PunchCounterReset = 90;
            }
        }

        public Vector2 PunchRushDirection { get; set; }

        public int RushTimer { get; set; }

        public int PunchCounter { get; set; }
        public int PunchCounterReset { get; set; }

        public int ImmuneTime { get; set; }
        public List<HitNPCData> HitNPCs { get; } = new List<HitNPCData>();

        public bool IsBarraging => BarrageTime > 0;
        public int BarrageTime { get; set; }

        public List<string> DrawInFrontStates { get; } = new List<string>();

        public bool DrawInFront => DrawInFrontStates.Contains(CurrentState);

        public virtual float AttackRange => 2f;

        public bool CanPunch => IsIdling || (IsPunching && CurrentAnimation.CurrentFrame > (int)(CurrentAnimation.FrameCount * 0.8f));
        public bool IsPunching => CurrentState.Contains("PUNCH");

        public virtual int PunchDamage => 5;
        public virtual int BarrageDamage => 5;

    }
}
