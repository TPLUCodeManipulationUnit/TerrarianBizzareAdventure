using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TerrarianBizzareAdventure
{
    public sealed class SpriteAnimation
    {
        private int
            _frameCounter, _currentFrame;

        public SpriteAnimation(Texture2D texture, int frameCount, int frameSpeed)
        {
            SpriteSheet = texture;
            FrameCount = frameCount;
            FrameSpeed = frameSpeed;
        }

        public void Update()
        {
            if (!Finished)
            {
                if (ReversePlayback)
                {
                    if (++_frameCounter >= FrameSpeed)
                    {
                        if (--_currentFrame <= 1)
                            Finished = true;

                        _frameCounter = 0;
                    }
                }
                else
                {
                    if (++_frameCounter >= FrameSpeed)
                    {
                        if (++_currentFrame >= FrameCount - 1)
                            Finished = true;

                        _frameCounter = 0;
                    }
                }
            }
        }

        public void ResetAnimation(bool reversePlayback = false)
        {
            Finished = false;
            _frameCounter = 0;
            _currentFrame = reversePlayback ? FrameCount - 1 : 0;
            ReversePlayback = reversePlayback;
        }

        public bool ReversePlayback { get; set; }

        public Rectangle FrameRect => new Rectangle(0, (int)FrameSize.Y * _currentFrame, (int)FrameSize.X, (int)FrameSize.Y);

        public Vector2 DrawOrigin => new Vector2(FrameSize.X * 0.5f, FrameSize.Y * 0.5f); 
        // Animation's texture
        public Texture2D SpriteSheet { get; }

        // Determines Width/Height of a single frame
        public Vector2 FrameSize => new Vector2(SpriteSheet.Width, SpriteSheet.Height / FrameCount);

        // Determines how fast animation will play
        public int FrameSpeed { get; }

        // Determines the amount of frames animation has
        public int FrameCount { get; }


        public bool Finished { get; private set; }
    }
}
