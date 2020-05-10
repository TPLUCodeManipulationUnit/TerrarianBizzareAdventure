using System.IO;
using Terraria;
using Terraria.ID;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Networking
{
    public sealed class PlayerJoiningSynchronizationPacket : ModPlayerNetworkPacket<TBAPlayer>
    {
        public PlayerJoiningSynchronizationPacket()
        {
        }

        public PlayerJoiningSynchronizationPacket(TBAPlayer standUser) : base(standUser)
        {
        }


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