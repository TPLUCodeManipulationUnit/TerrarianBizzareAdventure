using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Items.Standard;

namespace TerrarianBizzareAdventure.Items
{
    public abstract class TBAItem : StandardItem
    {
        protected TBAItem(string name, string tooltip, Vector2 size, int value = 0, int rare = ItemRarityID.White) : base(name, tooltip, (int) size.X, (int) size.Y, value: value, rarity: rare)
        {
        }
    }
}
