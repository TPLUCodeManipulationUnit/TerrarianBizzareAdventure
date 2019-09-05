using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TerrarianBizzareAdventure.Items.Developer
{
    public sealed class KingCrimsonAbilityTrigger : TBAItem
    {
        internal static int lagFor;
        internal static Player lagger;


        public KingCrimsonAbilityTrigger() : base("King Crimson No Noryoku", "xd", new Vector2(32, 32))
        {
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useTime = 15;
            item.useAnimation = 15;
        }


        public override bool UseItem(Player player)
        {
            lagger = player;
            lagFor = 10 * Constants.TICKS_PER_SECOND;


            return true;
        }


    }
}