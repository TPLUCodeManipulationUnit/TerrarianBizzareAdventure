using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Commands;

namespace TerrarianBizzareAdventure.Commands
{
    public class TBADebugCommand : DebugCommand
    {
        public TBADebugCommand(string command, CommandType type) : base(command, type)
        {
        }


        public override bool Autoload(ref string name) => true;

        public void DisabledDebugCommands() => Main.NewText("Game session has disabled debug commands");
    }
}