using System.IO;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Networking.Attributes;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.TimeStop
{
    public sealed class TimeStateChangedPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        protected override bool PostReceive(BinaryReader reader, int fromWho)
        {
            if (Stopped)
                TimeStopManagement.TryStopTime(ModPlayer, Duration, false);
            else
                TimeStopManagement.TryResumeTime(ModPlayer, false);

            return true;
        }


        public bool Stopped { get; set; }

        public int Duration { get; set; }
    }
}