using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using WebmilioCommons.Projectiles;

namespace TerrarianBizzareAdventure.Stands.KingCrimson
{
    public class FakeTilesProjectile : StandardProjectile
    {
        public override void SetDefaults()
        {
            TimeLeft = 612;
            projectile.friendly = false;
            projectile.hostile = false;
        }

        public override void AI()
        {
            if (!GotTiles)
            {
                GotTiles = true;

                int startPosX = (int)Main.screenPosition.X / 16;
                int startPosY = (int)Main.screenPosition.Y / 16;

                int tilesToPickX = Main.screenWidth / 16;
                int tilesToPickY = Main.screenHeight / 16;

                for (int i = startPosX; i < startPosX + tilesToPickX; i++)
                {
                    for (int j = startPosY; j < startPosY + tilesToPickY; j++)
                    {
                        Tile tile = Main.tile[i, j];

                        if (tile.active())
                            FakeTiles.Add(new FakeTileData(tile.type, new Vector2(i * 16, j * 16), new Rectangle(tile.frameX, tile.frameY, 16, 16)));
                    }
                }
            }

            if (TimeLeft == 575)
                foreach (FakeTileData fakes in FakeTiles)
                {
                    fakes.Velocity = new Vector2(Main.rand.Next(-2, 2), -Main.rand.NextFloat(2));
                    fakes.RotationDirection = Main.rand.NextBool() ? -1 : 1;
                }

            if (TimeLeft < 575)
                foreach (FakeTileData fakes in FakeTiles)
                {
                    fakes.Position += fakes.Velocity;
                    fakes.Rotation += 0.012f * fakes.RotationDirection;
                    fakes.Opacity -= 0.0075f;
                }

            if (TimeLeft > 575 && TimeLeft < 600)
                foreach (FakeTileData fakes in FakeTiles)
                {
                    fakes.VFXOffset = new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2));
                }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            foreach(FakeTileData tile in FakeTiles)
            {
                spriteBatch.Draw(Main.tileTexture[tile.TileID], tile.Position + tile.VFXOffset + new Vector2(8) - Main.screenPosition, tile.TileFrame, Color.White * tile.Opacity, tile.Rotation, new Vector2(8), 1f, SpriteEffects.None, 1f);
            }
        }

        public bool GotTiles { get; private set; }

        public List<FakeTileData> FakeTiles { get; } = new List<FakeTileData>();

        public sealed override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
