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
            TimeSkipManager.TimeSkippedFor = Duration;
            TimeSkipManager.TimeSkipper = ModPlayer;

            return true;
        }

        public int Duration { get; set; }
    }
}
