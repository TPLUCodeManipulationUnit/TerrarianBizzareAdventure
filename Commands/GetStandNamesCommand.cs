using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio;

namespace TerrarianBizzareAdventure.Commands
{
    public class GetStandNamesCommand : ModCommand
    {
        public override string Command => "listStands";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            foreach(Stand s in StandLoader.Instance.Generics)
            {
                if(s.CanAcquire(TBAPlayer.Get(caller.Player)))
                    Main.NewText(s.StandName);
            }
        }
    }
}
