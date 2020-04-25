using Terraria;
using System.IO;
using WebmilioCommons.Networking.Packets;
using TerrarianBizzareAdventure.Stands;
using Terraria.ID;

namespace TerrarianBizzareAdventure.Players
{
    public class StandProjectileIDPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        public int ProjectileID
        {
            get => ModPlayer.ActiveStandProjectileId;
            set => ModPlayer.ActiveStandProjectileId = value;
        }
    }
}
