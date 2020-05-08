using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Networking
{
    public sealed class AuraSyncPacket  : ModPlayerNetworkPacket<TBAPlayer>
    {
        public int AnimationID
        {
            get => ModPlayer.AuraAnimationKey;
            set => ModPlayer.AuraAnimationKey = value;
        }
    }
}
