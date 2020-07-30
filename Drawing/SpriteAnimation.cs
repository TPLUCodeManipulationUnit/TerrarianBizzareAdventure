using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace TerrarianBizzareAdventure.Drawing
{
    public sealed class SpriteAnimation
    {
        private int _ticks;

        /// <summary>
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="frameCount">How many frames does the animation contain.</param>
        /// <param name="frameSpeed">The amount of ticks to spend on each frame.</param>
        /// <param name="autoLoop">true to make the animation loop once it reaches the end. Recommended for idle animations.</param>
        public SpriteAnimation(Texture2D texture, int frameCount, int frameSpeed, bool autoLoop = false, SpriteAnimation nextAnimation = null, bool reverseNextAnimation = false)
        {
            SpriteSheet = texture;
            FrameCount = frameCount;
            FrameSpeed = frameSpeed;

            AutoLoop = autoLoop;

            NextAnimation = nextAnimation;

            ReverseNextAnimation = reverseNextAnimation;
        }

        public SpriteAnimation(string texture, int frameCount, int frameSpeed, bool autoLoop = false, SpriteAnimation nextAnimation = null, bool reverseNextAnimation = false)
        {
            SpriteSheet = ModContent.GetTexture("TerrarianBizzareAdventure/" + texture);
            FrameCount = frameCount;
            FrameSpeed = frameSpeed;

            AutoLoop = autoLoop;

            NextAnimation = nextAnimation;

            ReverseNextAnimation = reverseNextAnimation;
        }

        public void Update()
        {
            if (Finished)
            {

            }
            else
            {
                if (ReversePlayback)
                {
                    if (++_ticks >= FrameSpeed)
                    {
                        if (--CurrentFrame <= 0)
                        {
                            Finished = !AutoLoop;

                            if (AutoLoop)
                                ResetAnimation(ReversePlayback);
                        }

                        _ticks = 0;
                    }
                }
                else
                {
                    if (++_ticks >= FrameSpeed)
                    {
                        if (++CurrentFrame >= FrameCount)
                        {
                            Finished = !AutoLoop;

                            if (AutoLoop)
                                ResetAnimation();
                        }

                        _ticks = 0;
                    }
                }
            }
        }

        public void ResetAnimation(bool reversePlayback = false)
        {
            _ticks = 0;

            Finished = false;
            CurrentFrame = reversePlayback ? FrameCount - 1 : 0;
            ReversePlayback = reversePlayback;
        }

        public void SetNextAnimation(SpriteAnimation nextAnimation, bool reverse = false)
        {
            NextAnimation = nextAnimation;
            ReverseNextAnimation = reverse;
        }

        public bool ReversePlayback { get; set; }

        public Rectangle FrameRect => new Rectangle(0, (int)(FrameSize.Y * CurrentFrame), (int)FrameSize.X, (int)FrameSize.Y);

        public Vector2 DrawOrigin => new Vector2((int)(FrameSize.X * 0.5f), (int)(FrameSize.Y * 0.5f)); 
        // Animation's texture
        public Texture2D SpriteSheet { get; }
        
        public int CurrentFrame { get; private set; }

        ///<summary>Determines the dimensions of a single frame.</summary>
        public Vector2 FrameSize => new Vector2((int)SpriteSheet.Width, (int)(SpriteSheet.Height / FrameCount));

        ///<summary>Determines how many ticks to spend on one frame.</summary>
        public int FrameSpeed { get; set; }
        // Determines the amount of frames animation has
        public int FrameCount { get; }


        public bool Finished { get; private set; }

        /// <summary>true if the animation loops after reaching the end; otherwise false.</summary>
        public bool AutoLoop { get; set; }

        // next animation that will be played after this one is finished
        public SpriteAnimation NextAnimation { get; private set; }

        // shall the next animation be reversed or not.
        public bool ReverseNextAnimation { get; private set; }
    }
}
