using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using TerrarianBizzareAdventure.Helpers;

namespace TerrarianBizzareAdventure.UserInterfaces.Elements
{
    public class UIMouseClick : UIElement
    {
        /// <summary>
        /// UI element that displays mouse input required for action
        /// </summary>
        /// <param name="type">1 - left, 2 - right, 3 - mouse wheel clicks<</param>
        /// <param name="heldFor">How many frames will the button appear to be held for in the UI</param>
        public UIMouseClick(int type, int heldFor)
        {
            ButtonID = type;
            MaxFrames = heldFor;
            Width.Set(48, 0);
            Height.Set(72, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawHelpers.DrawInputButtonMouse(spriteBatch, this.GetDimensions().Position(), FrameCounter >= MaxFrames ? 0 : ButtonID);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (FrameCounter < MaxFrames)
                FrameCounter++;

            if(FrameCounter >= MaxFrames)
            {
                DownTimeCounter++;

                if(DownTimeCounter >= 20)
                {
                    DownTimeCounter = 0;
                    FrameCounter = 0;
                }
            }
        }

        public int ButtonID { get; }

        public int MaxFrames { get; }

        public int DownTimeCounter { get; private set; }

        public int FrameCounter { get; private set; }
    }
}
