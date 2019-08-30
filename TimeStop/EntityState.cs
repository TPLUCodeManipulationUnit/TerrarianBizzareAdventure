using Microsoft.Xna.Framework;

namespace TerrarianBizzareAdventure.TimeStop
{
    public abstract class EntityState
    {
        public abstract void Restore();


        public int Damage { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        public int EntityId { get; set; }
    }
}