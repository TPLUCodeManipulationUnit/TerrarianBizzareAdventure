using Microsoft.Xna.Framework;

namespace TerrarianBizzareAdventure.Stands.GoldenWind.KingCrimson
{
    public class FakeTileData
    {
        public FakeTileData(int tileid, Vector2 position, Rectangle frame)
        {
            TileID = tileid;

            Position = position;

            TileFrame = frame;
        }

        public FakeTileData(int tileid, Vector2 position, Rectangle frame, Color color)
        {
            TileID = tileid;

            Position = position;

            TileFrame = frame;

            Color = color;
        }

        public float Rotation { get; set; }

        public int RotationDirection { get; set; } = 1;

        public float RotationSpeed { get; set; } = 0.012f;

        public float Opacity { get; set; } = 1f;

        public int TileID { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 VFXOffset { get; set; }

        public Vector2 Velocity { get; set; }

        public Color Color { get; set; }

        public Rectangle TileFrame { get; set; }
    }
}
