using Terraria;

namespace TerrarianBizzareAdventure.TimeStop
{
    public interface INPCHasImmunityToTimeStop
    {
        bool IsImmuneToTimeStop(NPC npc);
    }
}