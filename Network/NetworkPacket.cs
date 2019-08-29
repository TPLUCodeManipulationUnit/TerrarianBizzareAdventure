using System.IO;
using Terraria.ModLoader;

namespace TerrarianBizzareAdventure.Network
{
    public abstract class NetworkPacket
    {
        protected NetworkPacket()
        {
        }

        public abstract bool Receive(BinaryReader reader, int fromWho);

        public void SendPacket(int toWho, int fromWho, params object[] args) => SendPacket(MakePacket(), toWho, fromWho, args);

        protected abstract void SendPacket(ModPacket packet, int toWho, int fromWho, params object[] args);

        public virtual void SendPacketToAllClients(int fromWho, params object[] args) => SendPacket(-1, fromWho, args);
        public virtual void SendPacketToServer(int fromWho, params object[] args) => SendPacket(256, fromWho, args);


        protected ModPacket MakePacket()
        {
            ModPacket packet = TBAMod.Instance.GetPacket();
            packet.Write(PacketType);

            return packet;
        }

        public byte PacketType { get; internal set; }
    }
}