using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Network;

namespace TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio
{
    public sealed class CompileAssemblyPacket : NetworkPacket
    {
        internal Dictionary<int, InstantEnvironment> playerInstantEnvironments = new Dictionary<int, InstantEnvironment>();


        public override bool Receive(BinaryReader reader, int fromWho)
        {
            int whichPlayer = reader.ReadInt32();
            string serializedSources = reader.ReadString();
            string[] sources = serializedSources.Split('\0');

            if (Main.netMode == NetmodeID.Server)
                NetworkPacketManager.Instance.CompileAssembly.SendPacketToAllClients(fromWho, whichPlayer, serializedSources);

            GetEnvironment(whichPlayer).CompileSource(false, sources);
            return true;
        }

        protected override void SendPacket(ModPacket packet, int toWho, int fromWho, params object[] args)
        {
            packet.Write((int)args[0]);
            packet.Write((string)args[1]);

            packet.Send(toWho, fromWho);
        }


        public InstantEnvironment GetEnvironment(int playerId)
        {
            if (!playerInstantEnvironments.ContainsKey(playerId))
                playerInstantEnvironments.Add(playerId, new InstantEnvironment());

            return playerInstantEnvironments[playerId];
        }
    }
}