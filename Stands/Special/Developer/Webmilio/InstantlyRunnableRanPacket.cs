using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Networking.Attributes;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio
{
    public sealed class InstantlyRunnableRanPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        public override bool PostReceive(BinaryReader reader, int fromWho)
        {
            InstantEnvironment instantEnvironment = TBAMod.Instance.CompileAssemblyPacket.playerInstantEnvironments[Player.whoAmI];

            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];

                if (!player.active || player.name == "")
                    continue;

                instantEnvironment.Run(instantEnvironment.InstantlyRunnables.Find(ir => ir.GetType().ToString() == StringifiedClass), TBAPlayer.Get(player), false);
            }

            return true;
        }

        [NetworkField]
        public string StringifiedClass { get; set; }
    }
}