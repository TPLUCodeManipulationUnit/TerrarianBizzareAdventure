using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.Helpers;

namespace TerrarianBizzareAdventure.Items.Weapons.Rewards
{
    public class RewardStiletto : TBAItem
    {
        public RewardStiletto() : base("☆Stiletto | Marble Fade", "Welcome to the Knife Gang", new Vector2(40, 40), 0, ItemRarityID.Expert)
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.melee = true;
            item.damage = 45;
            item.useAnimation = item.useTime = 16;
            item.knockBack = 3.5f;
        }

        public override void PostUpdate()
        {
            item.TurnToAir();
        }

        public override void UpdateInventory(Player player)
        {
            item.favorited = true;
            TBAPlayer.Get(player).StaminaRegenBuff += 60;

            if (!TBAPlayer.Get(player).KnifeGangMember)
                item.TurnToAir();
        }

        public override bool CanUseItem(Player player) => TBAPlayer.Get(player).KnifeGangMember;
    }
}
