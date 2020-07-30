using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.StardustCrusaders.TheEmperor;

namespace TerrarianBizzareAdventure.Players
{

    public partial class TBAPlayer : ModPlayer
    {
        public void ModifyEmperorLayers()
        {
            if (StandActive && ActiveStandProjectile is TheEmperor stand)
            {
                Projectile emperor = stand.projectile;

                if (emperor.velocity.SafeNormalize(-Vector2.UnitY).Y > 0.5)
                    player.bodyFrame = new Rectangle(0, 56 * 4, player.bodyFrame.Width, player.bodyFrame.Height);

                else if (emperor.velocity.SafeNormalize(-Vector2.UnitY).Y < -0.5)
                    player.bodyFrame = new Rectangle(0, 56 * 2, player.bodyFrame.Width, player.bodyFrame.Height);

                else
                    player.bodyFrame = new Rectangle(0, 56 * 3, player.bodyFrame.Width, player.bodyFrame.Height);
            }
        }
    }
}
