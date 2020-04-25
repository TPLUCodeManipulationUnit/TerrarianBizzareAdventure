using System.IO;
using Terraria;
using Terraria.ID;
using TerrarianBizzareAdventure.Stands;
using WebmilioCommons.Networking.Attributes;
using WebmilioCommons.Networking.Packets;

namespace TerrarianBizzareAdventure.Players
{
    public sealed class AuraSyncPacket  : ModPlayerNetworkPacket<TBAPlayer>
    {
        public int AnimationID
        {
            get => ModPlayer.AuraAnimationKey;
            set => ModPlayer.AuraAnimationKey = value;
        }
    }
}
