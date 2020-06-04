using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Networking
{
    public sealed class AttackTimerPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        public int AnimationID
        {
            get => ModPlayer.AttackDirectionResetTimer;
            set => ModPlayer.AttackDirectionResetTimer = value;
        }
    }
}
