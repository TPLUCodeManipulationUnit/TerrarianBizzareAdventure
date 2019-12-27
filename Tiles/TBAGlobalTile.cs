using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.TimeSkip;

namespace TerrarianBizzareAdventure.Tiles
{
    public sealed class TBAGlobalTile : GlobalTile
    {
        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            if(TimeSkipManager.IsTimeSkipped)
            {
                DrawHelpers.StartShader(spriteBatch);

                TimeSkipManager.TwilightShader.CurrentTechnique.Passes[31].Apply();
                return true;
            }

            return true;
        }

        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            //if (TimeSkipManager.IsTimeSkipped)
                //DrawHelpers.EndShader(spriteBatch);

            base.PostDraw(i, j, type, spriteBatch);
        }

    }

    public sealed class TBAGlobalWall : GlobalWall
    {
        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            if (TimeSkipManager.IsTimeSkipped)
            {
                DrawHelpers.StartShader(spriteBatch);

                TimeSkipManager.TwilightShader.CurrentTechnique.Passes[31].Apply();
                return true;
            }

            return true;
        }

        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            //if (TimeSkipManager.IsTimeSkipped)
                //DrawHelpers.EndShader(spriteBatch);

            base.PostDraw(i, j, type, spriteBatch);
        }
    }
}
