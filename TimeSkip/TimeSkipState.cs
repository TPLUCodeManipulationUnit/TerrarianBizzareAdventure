using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrarianBizzareAdventure.TimeSkip
{
    public class TimeSkipState
    {
        public TimeSkipState(Vector2 pos, float scale, float rot, Rectangle frame, int dir)
        {
            Position = pos;
            Scale = scale;
            Rotation = rot;
            Frame = frame;
            Direction = dir;
        }

        public Vector2 Position { get; }
        public float Scale { get; }
        public float Rotation { get; }
        public Rectangle Frame { get; }
        public int Direction { get; }
    }
}
