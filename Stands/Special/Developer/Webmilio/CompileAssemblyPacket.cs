using System.Collections.Generic;
using System.IO;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Networking.Attributes;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio
{
    public sealed class CompileAssemblyPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        internal Dictionary<int, InstantEnvironment> playerInstantEnvironments = new Dictionary<int, InstantEnvironment>();


        public override bool PostReceive(BinaryReader reader, int fromWho)
        {
            GetEnvironment(Player.whoAmI).CompileSource(false, Sources);
            return true;
        }


        public InstantEnvironment GetEnvironment(int playerId)
        {
            if (!playerInstantEnvironments.ContainsKey(playerId))
                playerInstantEnvironments.Add(playerId, new InstantEnvironment());

            return playerInstantEnvironments[playerId];
        }


        public string SerializedSources { get; set; }

        [NotNetworkField]
        public string[] Sources => SerializedSources.Split('\0');
    }
}