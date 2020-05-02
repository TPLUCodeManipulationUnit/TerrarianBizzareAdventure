using Terraria.ModLoader;
using WebmilioCommons.Commands;

namespace TerrarianBizzareAdventure.Commands
{
    public class TBADebugCommand : DebugCommand
    {
        public TBADebugCommand(string command, CommandType type) : base(command, type)
        {
        }


        public override bool Autoload(ref string name) => true;
    }
}