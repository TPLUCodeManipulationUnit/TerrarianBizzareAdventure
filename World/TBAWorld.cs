using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.World
{
    public sealed class TBAWorld : ModWorld
    {
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(TimeStopManagement.TimeStopped);

            if (TimeStopManagement.TimeStopped)
            {
                writer.Write(Main.time);
                writer.Write(Main.rainTime);

                writer.Write(TimeStopManagement.TimeStopper.player.whoAmI);
                writer.Write(TimeStopManagement.TimeStoppedFor);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            bool timeStopped = reader.ReadBoolean();

            if (timeStopped)
            {
                TimeStopManagement.MainTime = reader.ReadDouble();
                TimeStopManagement.MainRainTimer = reader.ReadInt32();

                TimeStopManagement.StopTime(TBAPlayer.Get(Main.player[reader.ReadInt32()]), reader.ReadInt32());
            }
            else
            {
                TimeStopManagement.ResumeTime();
            }
        }
    }
}