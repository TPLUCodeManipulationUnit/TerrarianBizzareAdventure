using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using WebmilioCommons.Projectiles;

namespace TerrarianBizzareAdventure.Stands.GoldenWind.KingCrimson
{
    //This code is disgusting. Too bad!
    public class FakeTilesProjectile : StandardProjectile
    {
        public override void SetDefaults()
        {
            TimeLeft = 612;
            projectile.friendly = false;
            projectile.hide = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            if (!GotTiles)
            {
                GotTiles = true;

                int startPosX = (int)Main.screenPosition.X / 16;
                int startPosY = (int)Main.screenPosition.Y / 16;

                int tilesToPickX = Main.screenWidth / 16 + 1;
                int tilesToPickY = Main.screenHeight / 16 + 1;

                for (int i = startPosX; i < startPosX + tilesToPickX; i++)
                {
                    for (int j = startPosY; j < startPosY + tilesToPickY; j++)
                    {
                        Tile tile = Main.tile[i, j];

                        Color lightColor = Lighting.GetColor(i, j);

                        if (tile.wall != WallID.None)
                            FakeWalls.Add(new FakeTileData(tile.wall, new Vector2(i * 16, j * 16) - new Vector2(8), new Rectangle(tile.wallFrameX(), tile.wallFrameY(), 36, 36), lightColor));

                        if (tile.active())
                            FakeTiles.Add(new FakeTileData(tile.type, new Vector2(i * 16, j * 16), new Rectangle(tile.frameX, tile.frameY, 16, 16), lightColor));
                    }
                }
            }

            if (TimeLeft == 575)
            {
                foreach (FakeTileData fakes in FakeTiles)
                {
                    fakes.Velocity = new Vector2(Main.rand.Next(-4, 4), -Main.rand.NextFloat(2));
                    fakes.RotationDirection = Main.rand.NextBool() ? -1 : 1;
                    fakes.RotationSpeed = (float)(Main.rand.Next(12, 64)) * 0.001f;
                }

                foreach (FakeTileData fakes in FakeWalls)
                {
                    fakes.Velocity = new Vector2(Main.rand.Next(-4, 4), -Main.rand.NextFloat(2));
                    fakes.RotationDirection = Main.rand.NextBool() ? -1 : 1;
                    fakes.RotationSpeed = (float)(Main.rand.Next(12, 64)) * 0.001f;
                }
            }

            if (TimeLeft < 575)
            {
                foreach (FakeTileData fakes in FakeTiles)
                {
                    fakes.Position += fakes.Velocity;
                    fakes.Rotation += fakes.RotationSpeed * fakes.RotationDirection;
                    fakes.Opacity -= 0.0075f;
					fakes.Color = Color.Lerp(fakes.Color, Color.White, 0.04f);
                }

                foreach (FakeTileData fakes in FakeWalls)
                {
                    fakes.Position += fakes.Velocity;
                    fakes.Rotation += fakes.RotationSpeed * fakes.RotationDirection;
                    fakes.Opacity -= 0.0075f;
					fakes.Color = Color.Lerp(fakes.Color, Color.White, 0.04f);
                }
            }

            if (TimeLeft > 575 && TimeLeft < 600)
            {
                foreach (FakeTileData fakes in FakeTiles)
                {
                    fakes.VFXOffset = new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2));
                }

                foreach (FakeTileData fakes in FakeWalls)
                {
                    fakes.VFXOffset = new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2));
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (GotTiles)
            {
                foreach (FakeTileData tile in FakeWalls)
                {
                    var texture = Main.wallTexture[tile.TileID];

                    if (tile.Opacity < 0.02f)
                        continue;

                    if (texture != null)
                        spriteBatch.Draw(texture, tile.Position + tile.VFXOffset + new Vector2(18) - Main.screenPosition, tile.TileFrame, tile.Color * tile.Opacity, tile.Rotation, new Vector2(18), 1f, SpriteEffects.None, 1f);
                }

                foreach (FakeTileData tile in FakeTiles)
                {
                    var texture = Main.tileTexture[tile.TileID];

                    if (tile.Opacity < 0.02f)
                        continue;
                    if (texture != null)
                        spriteBatch.Draw(texture, tile.Position + tile.VFXOffset + new Vector2(8) - Main.screenPosition, tile.TileFrame, tile.Color * tile.Opacity, tile.Rotation, new Vector2(8), 1f, SpriteEffects.None, 1f);
                }
            }
        }

        public bool GotTiles { get; private set; }

        public List<FakeTileData> FakeTiles { get; } = new List<FakeTileData>();

        public List<FakeTileData> FakeWalls { get; } = new List<FakeTileData>();

        public sealed override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";
    }
}
