using Terraria;
using System.IO;
using WebmilioCommons.Networking.Packets;
using TerrarianBizzareAdventure.Stands;
using Terraria.ID;

namespace TerrarianBizzareAdventure.Players
{
    public class StandProjectileIDPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        public override bool Receive(BinaryReader reader, int fromWho)
        {
            if (!IsResponse && Main.netMode == NetmodeID.MultiplayerClient)
            {
                IsResponse = true;
                Send(Main.myPlayer, Player.whoAmI);
            }

            return true;
        }

        public bool IsResponse { get; set; }
        public int ProjectileID
        {
            get => ModPlayer.ActiveStandProjectileId;
            set => ModPlayer.ActiveStandProjectileId = value;
        }
    }
}
