using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TerrarianBizzareAdventure.Helpers;

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

            if (PunchCounterReset > 0)
                PunchCounterReset--;
            else
                PunchCounter = 0;

            if (RushTimer > 0)
                RushTimer--;

            HitNPCs.ForEach(x => x.LifeTime -= 1);

            HitNPCs.RemoveAll(x => x.LifeTime <= 0);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            HitNPCs.Add(new HitNPCData(target, ImmuneTime));
            projectile.penetrate += 1;
        }

        public Vector2 GetRange(Vector2 startPos, Vector2 endPos)
        {
            bool exceedsRange = Vector2.Distance(startPos, endPos) > AttackRange * 24;

            Vector2 result = exceedsRange ? VectorHelpers.DirectToMouse(startPos, AttackRange * 24) : endPos - startPos;

            return result;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return HitNPCs.Count(x => x.HitNPC == target) <= 0;
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
    }
}
