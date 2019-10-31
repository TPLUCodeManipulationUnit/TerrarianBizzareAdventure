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
<<<<<<< HEAD
			{
				IsResponse = true;
                SendPacket(Main.myPlayer, Player.whoAmI);
			}
=======
                Send(Main.myPlayer, Player.whoAmI);
>>>>>>> a54a3e9f85362851618d0b1d283e2cd1937b5127

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