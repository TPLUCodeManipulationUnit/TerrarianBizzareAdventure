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