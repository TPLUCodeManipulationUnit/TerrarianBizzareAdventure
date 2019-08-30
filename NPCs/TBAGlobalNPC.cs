using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.NPCs
{
    public sealed class TBAGlobalNPC : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            if (TimeStopManagement.IsNPCImmune(npc.type))
                return true;

            IsStopped = TimeStopManagement.TimeStopped;

            if (IsStopped)
            {
                if (!TimeStopManagement.npcStates.ContainsKey(npc))
                    TimeStopManagement.RegisterStoppedNPC(npc);

                if (PreTimeStopPosition != Vector2.Zero)
                    npc.position = PreTimeStopPosition;

                NPCInstantState state = TimeStopManagement.npcStates[npc];

                npc.velocity = Vector2.Zero;
                npc.frameCounter = state.FrameCounter;
                npc.damage = 0;

                npc.ai = state.AI;

                return false;
            }
            else
                PreTimeStopPosition = npc.position;

            return true;
        }


        public override void SpawnNPC(int npc, int tileX, int tileY)
        {

        }


        public bool IsStopped { get; private set; }
        public Vector2 PreTimeStopPosition { get; set; }

        public override bool InstancePerEntity => true;
    }
}