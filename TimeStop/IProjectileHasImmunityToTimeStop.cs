using Terraria;

namespace TerrarianBizzareAdventure.TimeStop
{
    public interface IProjectileHasImmunityToTimeStop
    {
        bool IsImmuneToTimeStop(Projectile projectile);
    }
}