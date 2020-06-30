using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands.SREKT;
using TerrarianBizzareAdventure.UserInterfaces;
using TerrarianBizzareAdventure.UserInterfaces.Elements.Misc;

namespace TerrarianBizzareAdventure.NPCs
{
    public sealed partial class TBAGlobalNPC : GlobalNPC
    {
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if(projectile.modProjectile is SREKTBullet bullet)
            {
                if(npc.life - damage <= 0)
                {
                    UIManager.ResourcesLayer?.State?.Entries.Add(new SREKTFeedEntry(bullet.Owner.name, npc.FullName, bullet.NoScope, bullet.Headshot, bullet.WallBang));
                }
            }
        }

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
