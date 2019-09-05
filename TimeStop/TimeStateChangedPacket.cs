using System.IO;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Networking.Attributes;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.TimeStop
{
    public sealed class TimeStateChangedPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        private bool _stopped;
        private int _duration;


        public override bool PostReceive(BinaryReader reader, int fromWho)
        {
            if (Stopped)
                TimeStopManagement.TryStopTime(ModPlayer, Duration, false);
            else
                TimeStopManagement.TryResumeTime(ModPlayer, false);

            return true;
        }


        [NetworkField]
        public bool Stopped { get; set; }

        [NetworkField]
        public int Duration
        {
            get => _duration;
            set => _duration = value;
        }
    }
}