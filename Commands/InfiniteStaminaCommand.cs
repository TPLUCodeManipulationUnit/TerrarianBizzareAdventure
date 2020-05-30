using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Commands;

namespace TerrarianBizzareAdventure.Commands
{
    public class InfiniteStaminaCommand : TBADebugCommand
    {
        public InfiniteStaminaCommand() : base("istamina", CommandType.Chat)
        {
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (TBAMultiplayerConfig.EnableDebugCommands)
                TBAPlayer.Get().IsDebugging = !TBAPlayer.Get().IsDebugging;
            else
                DisabledDebugCommands();
                
        }
    }
}
