﻿using Terraria.ID;
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

            tbaPlayer.ActiveStandProjectileId = TBAPlayer.ACTIVE_STAND_PROJECTILE_INACTIVE_ID;
            tbaPlayer.Stand = StandLoader.Instance.GetRandom(tbaPlayer);
            Main.NewText($"You got yourself a Stand{(Main.rand.Next(0, 100) > 98 ? "oda" : "")}!");

            return true;
        }

        //public override bool CanUseItem(Player player) => !TBAPlayer.Get(player).StandUser;
        public override bool CanUseItem(Player player) => true;


        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.MeteoriteBar, 100);
            recipe.AddIngredient(ItemID.WoodenArrow);

            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);

            recipe.AddRecipe();
        }
    }
}
