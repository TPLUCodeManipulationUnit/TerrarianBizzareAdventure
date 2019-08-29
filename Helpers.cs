using Microsoft.Xna.Framework;
using Terraria;

namespace TerrarianBizzareAdventure
{
    public static class Helpers
    {
        public static Vector2 DirectToMouse(Vector2 start, float speed = 1.0f) => (Main.MouseWorld - start).SafeNormalize(-Vector2.UnitY) * speed;

        public static Vector2 DirectToPosition(Vector2 start, Vector2 end, float speed = 1.0f) => (end - start).SafeNormalize(-Vector2.UnitY) * speed;
    }
}
