using Terraria;

namespace TerrarianBizzareAdventure.TimeStop
{
    public interface IItemHasImmunityToTimeStop
    {
        bool IsImmuneToTimeStop(Item item);
    }
}