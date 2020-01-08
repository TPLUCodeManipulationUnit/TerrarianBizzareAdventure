using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Commands
{
    public class DebugCommand : ModCommand
    {
        public override string Command => "debugTBA";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            TBAPlayer tPlayer = TBAPlayer.Get(caller.Player);

            tPlayer.IsDebugging = !tPlayer.IsDebugging;
        }
    }
}
