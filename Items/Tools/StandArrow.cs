using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
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

            item.noUseGraphic = true; ;
            item.useStyle = 3;
        }

        public override bool CanUseItem(Player player)
        {
            TBAPlayer.Get(player).MyStand = Main.rand.Next(StandManager.Instance.StandPool);

            Main.NewText("You got yourself a Stand!");

            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddIngredient(ItemID.WoodenArrow);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);

            recipe.AddRecipe();
        }
    }
}
