using Terraria;

namespace TerrarianBizzareAdventure.TimeStop
{
    public interface IIsNPCImmuneToTimeStop
    {
        bool IsImmuneToTimeStop(NPC npc);
    }
}