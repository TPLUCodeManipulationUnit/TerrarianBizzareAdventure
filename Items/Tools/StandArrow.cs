using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;

namespace TerrarianBizzareAdventure.Items.Tools
{
    public class StandArrow : TBAItem
    {
        public StandArrow() : base("Bizzare Arrow", "Arrow forged from Meteorite.\nYou feel dizzy from just holding it.\nSelf harm is not recommended", new Vector2(40, 40), 0, ItemRarityID.Expert)
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            item.noUseGraphic = true; // TODO Enable this again
            item.useStyle = ItemUseStyleID.Stabbing;
        }


        public override bool UseItem(Player player)
        {
            TBAPlayer tbaPlayer = TBAPlayer.Get(player);

            Stand stand = StandLoader.Instance.GetRandom(tbaPlayer);

            tbaPlayer.Stand = stand;
            Main.NewText($"You got yourself a Stand{(Main.rand.Next(0, 100) > 98 ? "oda" : "")}!");

            if (TBAInputs.StandPose.GetAssignedKeys().Count <= 0
                || TBAInputs.ContextAction.GetAssignedKeys().Count <= 0
                || TBAInputs.ExtraAction01.GetAssignedKeys().Count <= 0
                || TBAInputs.ExtraAction02.GetAssignedKeys().Count <= 0)
            {
                Main.NewText("Whoops! It looks like you forgot to setup your hotkeys! Go to Settings -> Controls and scroll down. Bind all hotkeys from this mod & try again");
            }

                if (!tbaPlayer.UnlockedStands.Contains(stand.UnlocalizedName))
                tbaPlayer.UnlockedStands.Add(stand.UnlocalizedName);

            return true;
        }

        //public override bool CanUseItem(Player player) => !TBAPlayer.Get(player).StandUser;
        public override bool CanUseItem(Player player) => true;


        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.MeteoriteBar, 15);
            recipe.AddIngredient(ItemID.WoodenArrow);
            recipe.AddIngredient(ItemID.FallenStar, 30);

            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);

            recipe.AddRecipe();
        }
    }
}
