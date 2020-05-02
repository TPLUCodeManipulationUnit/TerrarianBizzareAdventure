using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using System.Linq;
using Terraria;
using TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio;
using TerrarianBizzareAdventure.Stands.SREKT;

namespace TerrarianBizzareAdventure.Commands
{
    public class GetStandCommand : TBADebugCommand
    {
        public GetStandCommand() : base("getStand", CommandType.Chat)
        {
        }


        protected override void ActionLocal(CommandCaller caller, Player player, string input, string[] args)
        {
            TBAPlayer tPlayer = TBAPlayer.Get(caller.Player);

            if (input.Contains("DayOrk"))
            {
                tPlayer.Stand = StandLoader.Instance.FindGeneric(x => x is SREKTStand);
                Main.NewText("What the~");
                return;
            }

            if (StandLoader.Instance.FindGeneric(x => input.Contains(x.StandName.ToString()) && x.CanAcquire(TBAPlayer.Get(caller.Player))) != null)
                tPlayer.Stand = StandLoader.Instance.FindGeneric(x => input.Contains(x.StandName.ToString()));
            else
                Main.NewText("Incorrect stand name, please use /listStands to see their names");
        }
    }
}
