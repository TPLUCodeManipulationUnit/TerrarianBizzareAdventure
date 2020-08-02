using Terraria.ModLoader;
using Terraria;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Buffs
{
    public class TiredDebuff : ModBuff
    {
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Tired");
			Description.SetDefault("Reduced stamina regeneration rate");
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			TBAPlayer.Get(player).Tired = true;
		}
	}
}
