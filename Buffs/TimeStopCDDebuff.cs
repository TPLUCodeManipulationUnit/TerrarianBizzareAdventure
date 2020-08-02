using Terraria.ModLoader;
using Terraria;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Buffs
{
    public class TimeStopCDDebuff : ModBuff
    {
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Shattered Time");
			Description.SetDefault("Cannot stop time flow");
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			TBAPlayer.Get(player).ShatteredTime = true;
		}
	}
}
