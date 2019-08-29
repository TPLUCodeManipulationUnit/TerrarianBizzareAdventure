using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrarianBizzareAdventure.Items
{
    public abstract class TBAItem : ModItem
    {
        protected TBAItem(string name, string toolTip, Vector2 size, int value = 0, int rare = ItemRarityID.White)
        {
            ItemName = name;
            ItemTooltip = toolTip;
            Size = size;

            Value = value;
            Rarity = rare;
        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(ItemName);
            Tooltip.SetDefault(ItemTooltip);
        }

        public override void SetDefaults()
        {
            item.width = (int)Size.X;
            item.height = (int)Size.Y;

            item.value = Value;
            item.rare = Rarity;
        }


        private int Rarity { get; }

        private string ItemName { get; }

        private string ItemTooltip { get; }

        private Vector2 Size { get; }

        private int Value { get; }
    }
}
