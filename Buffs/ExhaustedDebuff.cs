using Terraria.ModLoader;
using Terraria;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Buffs
{
    public class ExhaustedDebuff : ModBuff
    {
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Exhausted");
			Description.SetDefault("Unable to manifest your stand");
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			TBAPlayer.Get(player).Exhausted = true;
		}
	}
}
