using System.IO;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.TimeSkip
{
    public class TimeSkipPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        protected override bool PostReceive(BinaryReader reader, int fromWho)
        {
            TimeSkipManager.TimeSkipper = Skipper;
            TimeSkipManager.TimeSkippedFor = Duration;

            return true;
        }

        public int Duration { get; set; }

        public ModPlayer Skipper { get; set; }
    }
}
