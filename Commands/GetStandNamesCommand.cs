using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio;

namespace TerrarianBizzareAdventure.Commands
{
    public class GetStandNamesCommand : TBADebugCommand
    {
        public GetStandNamesCommand() : base("listStands", CommandType.Chat)
        {
        }


        protected override void ActionLocal(CommandCaller caller, Player player, string input, string[] args)
        {
            foreach (Stand s in StandLoader.Instance.Generics)
            {
                if (s.CanAcquire(TBAPlayer.Get(caller.Player)))
                    Main.NewText(s.StandName);
            }
        }
    }
}
