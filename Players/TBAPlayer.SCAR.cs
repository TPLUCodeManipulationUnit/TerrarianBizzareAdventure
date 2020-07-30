using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.Special.SREKT;
using TerrarianBizzareAdventure.UserInterfaces;
using TerrarianBizzareAdventure.UserInterfaces.Elements.Misc;

namespace TerrarianBizzareAdventure.Players
{
    public partial class TBAPlayer : ModPlayer
    {
        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if(proj.modProjectile is SREKTBullet bullet)
            {
                if (player.statLife - damage <= 0)
                {
                    UIManager.ResourcesLayer?.State?.Entries.Add(new SREKTFeedEntry(bullet.Owner.name, player.name, bullet.NoScope, bullet.Headshot, bullet.WallBang));
                }
            }
        }

        public void ModifySCARLayers()
        {
            if (Stand is SREKTStand && StandActive)
            {
                if (player.controlLeft || player.controlRight)
                    player.legFrame = new Rectangle(0, 56 * 9, player.bodyFrame.Width, player.bodyFrame.Height);
                else
                    player.legFrame = new Rectangle(0, 56 * 6, player.bodyFrame.Width, player.bodyFrame.Height);


                if (StandActive && ActiveStandProjectile is SREKTStand stand)
                {
                    Projectile scar = stand.projectile;

                    if (scar.velocity.SafeNormalize(-Vector2.UnitY).Y > 0.5)
                        player.bodyFrame = new Rectangle(0, 56 * 4, player.bodyFrame.Width, player.bodyFrame.Height);

                    else if (scar.velocity.SafeNormalize(-Vector2.UnitY).Y < -0.5)
                        player.bodyFrame = new Rectangle(0, 56 * 2, player.bodyFrame.Width, player.bodyFrame.Height);

                    else
                        player.bodyFrame = new Rectangle(0, 56 * 3, player.bodyFrame.Width, player.bodyFrame.Height);
                }
            }
        }
    }
}
