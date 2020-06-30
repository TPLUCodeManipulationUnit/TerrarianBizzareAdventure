using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Steamworks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using TerrarianBizzareAdventure.Helpers;

namespace TerrarianBizzareAdventure.UserInterfaces.Elements.Misc
{
    public class SREKTFeedEntry
    {
        public SREKTFeedEntry(string killer, string victim, bool noScope, bool headShot, bool wallbang)
        {
            Height = 40;

            Killer = killer;
            Victim = victim;
            NoScope = noScope;
            Headshot = headShot;
            WallBang = wallbang;

            OriginalWidth = 40 + Killer.Length * 10 + Victim.Length * 10;

            Position = new Vector2(Main.screenWidth - Width - 20, 10 + OffsetY);
        }

        public void Update()
        {

            if (TimeLeft > 0)
                TimeLeft--;
            else if (Opacity > 0)
                Opacity -= 0.02f;

            float offsetX = 84;

            if (NoScope)
                offsetX += 64;

            if (WallBang)
                offsetX += 64;

            if (Headshot)
                offsetX += 64;


            if (!Headshot && !WallBang && !NoScope)
                offsetX += 30;

            Width = OriginalWidth + offsetX;

            Position = new Vector2(Main.screenWidth - Width - 20, 10 + OffsetY);
        }

        public void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Killer == null || Victim == null)
                return;

            Vector2 position = Position;

            Texture2D tex = Textures.KillFeedEntry;

            Texture2D texScar = Textures.SCAREntryIcon;

            float offX = Killer.Length * 10;

            float offsetX = 94;

            Color borderColor = Main.LocalPlayer.name == Killer ? Color.Red * 0.9f : Color.Black * 0.9f;

            DrawHelpers.DrawBorderedRectangle(position, (int)Width, (int)Height, Color.Black * 0.8f * Opacity, borderColor * Opacity, spriteBatch);

            spriteBatch.Draw(texScar, position + new Vector2(20 + offX + 10, 5), null, Color.White * Opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            if (NoScope)
            {
                spriteBatch.Draw(tex, position + new Vector2(20 + offX + offsetX, 5), new Rectangle(0, 80, 40, 40), Color.White * Opacity, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1f);
                offsetX += 34;
            }

            if (WallBang)
            {
                spriteBatch.Draw(tex, position + new Vector2(20 + offX + offsetX, 6), new Rectangle(0, 40, 40, 40), Color.White * Opacity, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1f);
                offsetX += 32;
            }

            if (Headshot)
            {
                spriteBatch.Draw(tex, position + new Vector2(20 + offX + offsetX, 6), new Rectangle(0, 0, 40, 40), Color.White * Opacity, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1f);
                offsetX += 44;
            }

            spriteBatch.DrawString(Main.fontMouseText, Killer, position + new Vector2(10, 10), Color.White * Opacity);

            spriteBatch.DrawString(Main.fontMouseText, Victim, position + new Vector2(offX + 20 + offsetX, 10), Color.White * Opacity);

        }

        public float OffsetY { get; set; }

        public float Opacity { get; set; } = 1f;

        public int TimeLeft { get; set; } = 300;

        public float OriginalWidth { get; set; }

        public string Killer { get; set; }

        public string Victim { get; set; }

        public bool WallBang { get; set; }

        public bool Headshot { get; set; }

        public bool NoScope { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public Vector2 Position { get; set; }
    }
}
