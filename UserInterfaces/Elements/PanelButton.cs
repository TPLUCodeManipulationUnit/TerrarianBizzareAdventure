using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace TerrarianBizzareAdventure.UserInterfaces.Elements
{
    public class PanelButton : UIPanel
    {
        public PanelButton(string name = "SampleText", float width = 60, float height = 40)
        {
            this.Width.Set(width, 0);
            this.Height.Set(height, 0);
            this.SetPadding(0);

            Name = name;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            NameText = new UIText(Name);
            NameText.HAlign = 0.5f;
            NameText.VAlign = 0.5f;

            this.Append(NameText);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (this.ContainsPoint(MousePosition))
            {
                Main.LocalPlayer.mouseInterface = true;
                NameText.TextColor = Color.Yellow;
                this.BackgroundColor = Color.LightBlue;
            }
            else
            {
                NameText.TextColor = Color.White;
                this.BackgroundColor = Color.CornflowerBlue;
            }
        }

        public UIText NameText { get; private set; }

        public string Name { get; }
    }
}
