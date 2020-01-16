using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using TerrarianBizzareAdventure.TimeSkip;

namespace TerrarianBizzareAdventure.UserInterfaces
{
    public class TimeSkipVFX : TBAUIState
    {
        private const int FRAME_HEIGHT = 72,
            FRAME_WIDTH = 120;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if(TimeSkipManager.TimeSkippedFor > 0 && !TimeSkipManager.FullCycle)
            spriteBatch.Draw(
                Textures.TimeSkipVFX,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                new Rectangle(0, TimeSkipManager.CurrentFrame * FRAME_HEIGHT, FRAME_WIDTH, FRAME_HEIGHT),
                Color.White,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                1f);
        }
    }
}
