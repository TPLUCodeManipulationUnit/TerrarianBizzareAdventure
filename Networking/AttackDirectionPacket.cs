using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Networking
{
    public sealed class AttackDirectionPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        public int AnimationID
        {
            get => ModPlayer.AttackDirection;
            set => ModPlayer.AttackDirection = value;
        }
    }
}
