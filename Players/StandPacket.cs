using System;
using System.IO;
using Terraria;
using Terraria.ID;
using TerrarianBizzareAdventure.Stands;
using WebmilioCommons.Networking.Attributes;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Players
{
    public sealed class StandPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        public override bool Receive(BinaryReader reader, int fromWho)
        {
            if (!IsResponse && Main.netMode == NetmodeID.MultiplayerClient)
            {
                IsResponse = true;
                Send(Main.myPlayer, Player.whoAmI);
            }

            return true;
        }
        public bool IsResponse { get; set; }

        public string StandName
        {
            get => !ModPlayer.StandUser ? "" : ModPlayer.Stand.UnlocalizedName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;

                ModPlayer.Stand = StandLoader.Instance.GetGeneric(value);
            }
        }
    }
}