using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.GoldenWind.KingCrimson;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public override void FrameEffects()
        {
            if (Stand is KingCrimson
                && StandActive
                && player.head == mod.GetEquipSlot("VinegarHead", EquipType.Head)
                && player.body == mod.GetEquipSlot("VinegarShirt", EquipType.Body)
                && player.legs == mod.GetEquipSlot("VinegarPants", EquipType.Legs))
            {
                player.head = mod.GetEquipSlot("DiavoloHead", EquipType.Head);
                player.body = mod.GetEquipSlot("DiavoloBody", EquipType.Body);
            }

        }

        public bool CanTransform { get; set; }
    }
}
