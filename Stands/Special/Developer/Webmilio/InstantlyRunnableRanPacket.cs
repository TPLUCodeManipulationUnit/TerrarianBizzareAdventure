using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Network;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio
{
    public sealed class InstantlyRunnableRanPacket : NetworkPacket
    {
        public override bool Receive(BinaryReader reader, int fromWho)
        {
            int whichPlayer = (int) reader.ReadInt32();
            string classToString = reader.ReadString();

            if (Main.netMode == NetmodeID.Server)
                NetworkPacketManager.Instance.InstantlyRunnableRan.SendPacketToAllClients(fromWho, whichPlayer, classToString);

            InstantEnvironment instantEnvironment = NetworkPacketManager.Instance.CompileAssembly.playerInstantEnvironments[whichPlayer];

            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];

                if (!player.active || player.name == "")
                    continue;

                instantEnvironment.Run(instantEnvironment.InstantlyRunnables.Find(ir => ir.GetType().ToString() == classToString), TBAPlayer.Get(player), false);
            }

            return true;
        }

        protected override void SendPacket(ModPacket packet, int toWho, int fromWho, params object[] args)
        {
            packet.Write((int) args[0]);
            packet.Write((string) args[1]);

            packet.Send(toWho, fromWho);
        }
    }
}