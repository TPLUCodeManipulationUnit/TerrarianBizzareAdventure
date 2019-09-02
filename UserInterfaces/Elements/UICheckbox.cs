using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace TerrarianBizzareAdventure.UserInterfaces.Elements
{
    public class UICheckbox : UIElement
    {
        public UICheckbox()
        {
            this.Width.Set(32, 0);
            this.Height.Set(32, 0);
        }

        public override void Click(UIMouseEvent evt)
        {
            Checked = !Checked;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TBAMod.Instance.GetTexture("UserInterfaces/Elements/UICheckbox"), this.GetDimensions().Position(), new Rectangle(0, Checked ? 32 : 0, 32, 32), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        public bool Checked { get; private set; }
    }
}
