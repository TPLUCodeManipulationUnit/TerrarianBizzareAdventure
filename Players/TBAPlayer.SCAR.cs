using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.SREKT;
using TerrarianBizzareAdventure.Stands;
using Terraria;

namespace TerrarianBizzareAdventure.Players
{
    public partial class TBAPlayer : ModPlayer
    {
        public void ModifySCARLayers()
        {
            if (Stand is SREKTStand && ActiveStandProjectileId != ACTIVE_STAND_PROJECTILE_INACTIVE_ID)
            {
                if(player.controlLeft || player.controlRight)
                    player.legFrame = new Rectangle(0, 56 * 9, player.bodyFrame.Width, player.bodyFrame.Height);
                else
                    player.legFrame = new Rectangle(0, 56 * 6, player.bodyFrame.Width, player.bodyFrame.Height);

                Stand tryGetSCAR = Main.projectile[ActiveStandProjectileId].modProjectile as SREKTStand;

                if(tryGetSCAR != null)
                {
                    Projectile SCAR = tryGetSCAR.projectile;

                    if(SCAR.velocity.SafeNormalize(-Vector2.UnitY).Y > 0.5)
                        player.bodyFrame = new Rectangle(0, 56 * 4, player.bodyFrame.Width, player.bodyFrame.Height);

                    else if (SCAR.velocity.SafeNormalize(-Vector2.UnitY).Y < -0.5)
                        player.bodyFrame = new Rectangle(0, 56 * 2, player.bodyFrame.Width, player.bodyFrame.Height);

                    else
                        player.bodyFrame = new Rectangle(0, 56 * 3, player.bodyFrame.Width, player.bodyFrame.Height);
                }
            }
        }
    }
}
