using Microsoft.Xna.Framework;
using Terraria;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Items.Tools
{
    public class StandArrow : TBAItem
    {
        public StandArrow() : base("Bizzare Arrow", "Arrow forged from Meteorite. \nYou feel dizzy from just holding it. \nSelf harm is not recommended", new Vector2(40, 40), Item.buyPrice(10, 0, 0, 0), -12)
        {
        }
        public override void SetDefaults()
        {
            base.SetDefaults();

            item.useStyle = 1;
        }
        public override bool CanUseItem(Player player)
        {
            TBAPlayer.Get(player).MyStand = StandManager.Instance.StandPool[0];

            return base.CanUseItem(player);
        }
    }
}
