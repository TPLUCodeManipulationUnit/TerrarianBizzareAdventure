using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using System.Linq;
using Terraria;
using TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio;

namespace TerrarianBizzareAdventure.Commands
{
    public class GetStandCommand : ModCommand
    {
        public override string Command => "getStand";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            TBAPlayer tPlayer = TBAPlayer.Get(caller.Player);

            if (StandLoader.Instance.FindGeneric(x => input.Contains(x.StandName.ToString()) && x.CanAcquire(TBAPlayer.Get(caller.Player))) != null)
                tPlayer.Stand = StandLoader.Instance.FindGeneric(x => input.Contains(x.StandName.ToString()));
            else
                Main.NewText("Incorrect stand name, please use /listStands to see their names");
        }
    }
}
