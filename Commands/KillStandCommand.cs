using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Commands
{
    public class KillStandCommand : TBADebugCommand
    {
        public KillStandCommand() : base("ks", CommandType.Chat)
        {
        }


        protected override void ActionLocal(CommandCaller caller, Player player, string input, string[] args)
        {
            if (TBAMultiplayerConfig.EnableDebugCommands)
            {
                TBAPlayer tPlayer = TBAPlayer.Get(caller.Player);

                tPlayer.ActiveStandProjectile.KillStand();
            }
            else
                DisabledDebugCommands();
        }
    }
}
