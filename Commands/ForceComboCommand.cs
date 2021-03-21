using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.Stands.Special.SREKT;

namespace TerrarianBizzareAdventure.Commands
{
    public class ForceComboCommand : TBADebugCommand
    {
        public ForceComboCommand() : base("force", CommandType.Chat)
        {
        }


        protected override void ActionLocal(CommandCaller caller, Player player, string input, string[] args)
        {
            if (TBAMultiplayerConfig.EnableDebugCommands)
            {
                TBAPlayer tPlayer = TBAPlayer.Get(caller.Player);

                Stand stand = tPlayer.ActiveStandProjectile;

                string cInput = input.Replace("/force ", "");
                if (stand == null)
                {
                    Main.NewText("Cannot force inactive stand");
                    return;
                }

                if (stand.Combos.ContainsKey(cInput))
                {
                    tPlayer.Inputs.Clear();
                    tPlayer.ComboResetTimer = 5;

                    foreach(string pInput in stand.Combos[cInput].Inputs)
                    {
                        tPlayer.Inputs.Add(new ComboInput(pInput));
                    }
                }
                else
                    Main.NewText("Invalid combo name");
            }
            else
                DisabledDebugCommands();
        }
    }
}
