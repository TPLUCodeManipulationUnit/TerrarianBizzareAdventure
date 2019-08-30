using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Network;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.TimeStop
{
    public sealed class TimeStateChangedPacket : NetworkPacket
    {
        public override bool Receive(BinaryReader reader, int fromWho)
        {
            int whichPlayer = reader.ReadInt32();
            bool stopped = reader.ReadBoolean();
            int time = reader.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
                SendPacketToAllClients(fromWho, whichPlayer, stopped, time);

            TBAPlayer tbaPlayer = TBAPlayer.Get(Main.player[whichPlayer]);

            if (stopped)
                TimeStopManagement.TryStopTime(tbaPlayer, time, false);
            else
                TimeStopManagement.TryResumeTime(tbaPlayer, false);

            return true;
        }


        protected override void SendPacket(ModPacket packet, int toWho, int fromWho, params object[] args)
        {
            packet.Write((int) args[0]);
            packet.Write((bool) args[1]);
            packet.Write((int) args[2]);

            packet.Send(toWho, fromWho);
        }
    }
}