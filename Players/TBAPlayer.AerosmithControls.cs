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

                ASTurnLeft = player.controlLeft;
                ASTurnRight = player.controlRight;

                ASBomb = player.controlUseTile;

                player.controlUp = false;
                player.controlDown = false;

                player.controlJump = false;

                player.controlUseItem = false;

                player.controlRight = false;
                player.controlLeft = false;

                player.controlUseTile = false;
            }
        }


        public bool ASHover { get; set; }

        public bool ASAngleUp { get; set; }
        public bool ASAngleDown { get; set; }

        public bool ASAttack { get; set; }
        public bool ASBomb { get; set; }

        public bool ASTurnLeft { get; set; }
        public bool ASTurnRight { get; set; }
    }
}
