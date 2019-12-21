using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Items.Armor.Vanity.Vinegar;
using TerrarianBizzareAdventure.Stands.KingCrimson;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public override void FrameEffects()
        {
            if (Stand is KingCrimson
                && ActiveStandProjectileId != ACTIVE_STAND_PROJECTILE_INACTIVE_ID
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
