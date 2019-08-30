using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Network;
using TerrarianBizzareAdventure.Stands;

namespace TerrarianBizzareAdventure.Players
{
    public sealed class PlayerJoiningSynchronizationPacket : NetworkPacket
    {
        public override bool Receive(BinaryReader reader, int fromWho)
        {
            int whichPlayer = reader.ReadInt32();
            string standName = reader.ReadString();
            bool isResponse = reader.ReadBoolean();

            if (Main.dedServ)
                NetworkPacketManager.Instance.PlayerJoiningSynchronization.SendPacketToAllClients(fromWho, whichPlayer, standName, isResponse);

            TBAPlayer tbaPlayer = TBAPlayer.Get(Main.player[whichPlayer]);

            if (!string.IsNullOrWhiteSpace(standName))
                tbaPlayer.Stand = StandManager.Instance[standName];

            if (!isResponse && Main.netMode == NetmodeID.MultiplayerClient)
                SendPacket(whichPlayer, Main.myPlayer, Main.myPlayer, standName, true);

            return true;
        }


        protected override void SendPacket(ModPacket packet, int toWho, int fromWho, params object[] args)
        {
            packet.Write((int) args[0]);
            packet.Write((string) args[1]);
            packet.Write((bool) args[2]);

            packet.Send(toWho, fromWho);
        }
    }
}