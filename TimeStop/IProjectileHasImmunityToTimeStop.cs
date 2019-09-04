using Terraria;

namespace TerrarianBizzareAdventure.TimeStop
{
    public interface IProjectileHasImmunityToTimeStop
    {
        bool IsNativelyImmuneToTimeStop(Projectile projectile);
    }
}