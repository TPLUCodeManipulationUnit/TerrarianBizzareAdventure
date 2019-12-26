using System.IO;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.TimeSkip
{
    public class TimeSkipPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        protected override bool PostReceive(BinaryReader reader, int fromWho)
        {
            TimeSkipManager.TimeSkipper = ModPlayer;
            TimeSkipManager.TimeSkippedFor = Duration;

            return true;
        }

        public int Duration { get; set; }
    }
}
