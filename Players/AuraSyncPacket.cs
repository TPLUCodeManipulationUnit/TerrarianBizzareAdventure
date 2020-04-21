using System.IO;
using Terraria;
using Terraria.ID;
using TerrarianBizzareAdventure.Stands;
using WebmilioCommons.Networking.Attributes;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Players
{
    public sealed class AuraSyncPacket  : ModPlayerNetworkPacket<TBAPlayer>
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

        public int AnimationID
        {
            get => ModPlayer.AuraAnimationKey;
            set => ModPlayer.AuraAnimationKey = value;
        }
    }
}
