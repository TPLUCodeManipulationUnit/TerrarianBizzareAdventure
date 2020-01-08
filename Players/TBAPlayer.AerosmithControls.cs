using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.Aerosmith;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public void SetAerosmithControls()
        {
            if (Stand is AerosmithStand && ActiveStandProjectileId != ACTIVE_STAND_PROJECTILE_INACTIVE_ID)
            {
                ASHover = player.controlJump;
                ASAngleUp = player.controlUp;
                ASAngleDown = player.controlDown;
                ASAttack = player.controlUseItem;

                player.controlUp = false;
                player.controlDown = false;
                player.controlJump = false;
                player.controlUseItem = false;
            }
        }

        public bool ASHover { get; set; }
        public bool ASAngleUp { get; set; }
        public bool ASAngleDown { get; set; }
        public bool ASAttack { get; set; }
    }
}
