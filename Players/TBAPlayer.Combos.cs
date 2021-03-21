using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Enums;
using TerrarianBizzareAdventure.Stands;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public const int COMBO_TIME = 120;

        public const int DELAY = 4;

        public void ProcessComboTriggers()
        {
            if (!StandActive)
                return;

            if (Inputs.Count > 12)
                Inputs.RemoveAt(0);

            if (player.controlUp && !TBAInputs.LastUpState)
            {
                OnInput(TBAInputs.Up);
            }

            if (player.controlDown && !TBAInputs.LastDownState)
            {
                OnInput(TBAInputs.Down);
            }

            if (TBAInputs.ContextAction.JustPressed)
            {
                OnInput(TBAInputs.CABind());
            }

            if (TBAInputs.ExtraAction01.JustPressed)
            {
                OnInput(TBAInputs.EA1Bind());
            }

            if (TBAInputs.ExtraAction02.JustPressed)
            {
                OnInput(TBAInputs.EA2Bind());
            }

            if (MouseOneTimeReset > 2 && !player.controlUseItem)
            {
                OnInput(MouseClick.LeftClick.ToString());
            }

            if (MouseTwoTimeReset > 2 && !player.controlUseTile)
            {
                OnInput(MouseClick.RightClick.ToString());
            }

            if(ComboDelayTime > 0)
                ComboDelayTime--;

            TBAInputs.LastDownState = player.controlDown;
            TBAInputs.LastUpState = player.controlUp;
        }

        private void OnInput(string key)
        {
            ComboDelayTime = DELAY;
            ComboResetTimer = COMBO_TIME;
            Inputs.Add(new ComboInput(key));
        }

        public void ResetInputs()
        {
            if (ComboResetTimer > 0)
                ComboResetTimer--;
            else
            {
                Inputs.Clear();
            }

        }

        public void GetEligibleKeys()
        {
            Stand?.Combos.Clear();
            Stand?.AddCombos();
        }

        public int ComboResetTimer { get; set; }

        public int ComboDelayTime { get; set; }
        public bool IsComboCheckDelayed => ComboDelayTime > 0;
        public List<ComboInput> Inputs { get; } = new List<ComboInput>(10);
    }
}
