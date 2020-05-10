using Terraria;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Networking
{
    public class ActiveStandChangedPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        public int ProjectileID
        {
            get => ModPlayer.ActiveStandProjectile?.projectile.whoAmI ?? TBAPlayer.ACTIVE_STAND_PROJECTILE_INACTIVE_ID;
            set => ModPlayer.ActiveStandProjectile = value == TBAPlayer.ACTIVE_STAND_PROJECTILE_INACTIVE_ID ? null : Main.projectile[value].modProjectile as Stand;
        }
    }
}
