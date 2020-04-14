using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using TerrarianBizzareAdventure.Helpers;

namespace TerrarianBizzareAdventure.UserInterfaces.Elements
{
    public class UIButtonPress : UIElement
    {
        /// <summary>
        /// UI element that displays mouse input required for action
        /// </summary>
        /// <param name="type">1 - left, 2 - right, 3 - mouse wheel clicks<</param>
        /// <param name="heldFor">How many frames will the button appear to be held for in the UI</param>
        public UIButtonPress(string buttonName)
        {
            ButtonName = buttonName;
            Width.Set(50, 0);
            Height.Set(48, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawHelpers.DrawInputButtonKeyboard(ButtonName, spriteBatch, this.GetDimensions().Position());
        }

        public string ButtonName { get; }
    }
}
