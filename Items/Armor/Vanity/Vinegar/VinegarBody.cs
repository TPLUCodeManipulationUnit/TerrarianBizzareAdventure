using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Items.Armor.Vanity.Vinegar
{
    [AutoloadEquip(EquipType.Body)]
    public class VinegarShirt : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Vinegar's Disguise");
            Tooltip.SetDefault("Holds a terrible secret within...");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10000;
            item.rare = -12;
            item.vanity = true;
        }
    }
    public class DiavoloBody : EquipTexture
    {
    }
}
