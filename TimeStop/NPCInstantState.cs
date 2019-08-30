using Terraria;

namespace TerrarianBizzareAdventure.TimeStop
{
    public class NPCInstantState : EntityState
    {
        public static NPCInstantState FromNPC(int npcId, NPC npc) =>
            new NPCInstantState()
            {
                EntityId = npcId,
                NPC = npc,

                Position = npc.position,
                Velocity = npc.velocity,

                Damage = npc.damage,

                AI = npc.ai,

                FrameCounter = npc.frameCounter,
            };


        public override void Restore()
        {
            NPC.velocity = Velocity;
            NPC.damage = Damage;

            NPC.ai = AI;

            NPC.frameCounter = FrameCounter;
        }


        public NPC NPC { get; set; }

        public double FrameCounter { get; set; }

        public float[] AI { get; set; }
    }
}