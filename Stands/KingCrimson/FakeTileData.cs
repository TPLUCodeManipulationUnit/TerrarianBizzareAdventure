using Microsoft.Xna.Framework;

namespace TerrarianBizzareAdventure.Stands.KingCrimson
{
    public class FakeTileData
    {
        public FakeTileData(int tileid, Vector2 position, Rectangle frame)
        {
            TileID = tileid;

            Position = position;

            TileFrame = frame;
        }

        public float Rotation { get; set; }

        public int RotationDirection { get; set; } = 1;

        public float Opacity { get; set; } = 1f;

        public int TileID { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 VFXOffset { get; set; }

        public Vector2 Velocity { get; set; }

        public Rectangle TileFrame { get; set; }
    }
}
