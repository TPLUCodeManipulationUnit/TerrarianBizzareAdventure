using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands.SREKT;

namespace TerrarianBizzareAdventure.NPCs
{
    public sealed partial class TBAGlobalNPC : GlobalNPC
    {
        public void PostSCARDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (Main.LocalPlayer.whoAmI == Main.myPlayer)
            {
                TBAPlayer standUser = TBAPlayer.Get(Main.LocalPlayer);


                if (standUser.Stand is SREKTStand && standUser.StandActive)
                {
                    int maxHeight = (npc.life * npc.height) / npc.lifeMax;


                    if (standUser.ActiveStandProjectile is SREKTStand stand && stand.Wallhack)
                    {

                        DrawHelpers.DrawBorderedRectangle(npc.position - Main.screenPosition - new Vector2(12, maxHeight - npc.height), 10, maxHeight, Color.Lime, Color.Green, spriteBatch);
                        DrawHelpers.DrawBorderedRectangle(npc.position - Main.screenPosition, npc.width, npc.height, Color.Black * 0f, Color.Green, spriteBatch);
                    }
                }
            }
        }
    }
}
