using System.IO;
using Terraria;
using Terraria.ID;
using TerrarianBizzareAdventure.Stands;
using WebmilioCommons.Networking.Attributes;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Players
{
    public sealed class PlayerJoiningSynchronizationPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        public override bool Receive(BinaryReader reader, int fromWho)
        {
            if (!IsResponse && Main.netMode == NetmodeID.MultiplayerClient)
			{
				IsResponse = true;
                SendPacket(Main.myPlayer, Player.whoAmI);
			}

            return true;
        }


        public string StandName
        {
            get
            {
                if (!ModPlayer.StandUser)
                    return "";

                return ModPlayer.Stand.UnlocalizedName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;

                ModPlayer.Stand = StandManager.Instance[value];
            }
        }

        public bool IsResponse { get; set; }
    }
}