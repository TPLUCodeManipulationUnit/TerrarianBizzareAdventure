using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using TerrarianBizzareAdventure.TimeSkip;
using TerrarianBizzareAdventure.Players;
using ReLogic.Graphics;

namespace TerrarianBizzareAdventure.UserInterfaces
{
    public class ResourcesState : TBAUIState
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            TBAPlayer mPlayer = TBAPlayer.Get(Main.LocalPlayer);

            Texture2D border = Textures.StaminaBarBorder;
            Texture2D bar = Textures.StaminaBar;
            Vector2 position = new Vector2(20, Main.screenHeight - 80);
            Rectangle barSize = new Rectangle((int)position.X, (int)position.Y, border.Width, border.Height);

            bool hasNoActiveStand = mPlayer.ActiveStandProjectileId == -999;

            if (mPlayer.StandUser)
            {
                int drawPercent = (mPlayer.Stamina * bar.Width) / mPlayer.MaxStamina;

                float regen = (mPlayer.StaminaRegenTickRate) / 60;

                string result = string.Format("{0}.{1}", regen, mPlayer.StaminaRegenTickRate % 60);

                if (barSize.Contains(Main.MouseScreen.ToPoint()))
                    Main.hoverItemName = "You restore 1 stamina over " + result + " seconds";

                spriteBatch.Draw(
                    border,
                    position,
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    1f);

                spriteBatch.Draw(
                    bar,
                    position + new Vector2(14, 6),
                    new Rectangle(0, 0, drawPercent, bar.Height),
                    Color.White,
                    0,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    1f);

                spriteBatch.DrawString(
                    Main.fontMouseText,
                    mPlayer.Stamina + " / " + mPlayer.MaxStamina,
                    position + new Vector2(30, 4),
                    Color.White);

                spriteBatch.DrawString(
                    Main.fontMouseText,
                    result + "s",
                    new Vector2(20, Main.screenHeight - 80) + new Vector2(116, 24),
                    Color.White,
                    -MathHelper.Pi / 4 + .1f,
                    Vector2.Zero,
                    0.75f,
                    SpriteEffects.None,
                    1f
                    );

                if(mPlayer.IsDebugging)
                spriteBatch.DrawString(
                    Main.fontMouseText,
                    "DEBUG",
                    new Vector2(20, Main.screenHeight - 80) + new Vector2(16, -36),
                    Color.Red,
                    0,
                    Vector2.Zero,
                    1,
                    SpriteEffects.None,
                    1f
                    );
            }
        }
    }
}
