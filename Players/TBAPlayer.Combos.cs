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
        public void ProcessComboTriggers()
        {
            if (!StandActive)
                return;

            KeyboardState kState = TBAInputs.CurrentState;

            if(EligibleKeys.Count <= 0)
            GetEligibleKeys();

            if (Inputs.Count > 12)
                Inputs.RemoveAt(0);

            var keys = kState.GetPressedKeys();

            if (keys.Length > TBAInputs.LastState.GetPressedKeys().Length
                && EligibleKeys.Contains(keys[keys.GetUpperBound(0)].ToString())
                )
            {
                ComboResetTimer = 60;
                Inputs.Add(new ComboInput(keys[keys.GetUpperBound(0)].ToString()));
            }

            if(MouseOneTimeReset > 2 && !player.controlUseItem)
            {
                ComboResetTimer = 60;
                Inputs.Add(new ComboInput(MouseClick.LeftClick.ToString()));
            }

            if (MouseTwoTimeReset > 2 && !player.controlUseTile)
            {
                ComboResetTimer = 60;
                Inputs.Add(new ComboInput(MouseClick.RightClick.ToString()));
            }

            TBAInputs.LastState = Keyboard.GetState();
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
            /*var inputMode = PlayerInput.CurrentInputMode;

            if (inputMode == InputMode.XBoxGamepad || inputMode == InputMode.XBoxGamepadUI)
            {
                inputMode = InputMode.XBoxGamepad;
            }
            else
            {
                inputMode = InputMode.Keyboard;
            }

            if (!PlayerInput.CurrentProfile.InputModes.TryGetValue(inputMode, out var keyConf))
            {
                return;
            }*/

            EligibleKeys.Add(TBAInputs.Left);
            EligibleKeys.Add(TBAInputs.Right);
            EligibleKeys.Add(TBAInputs.Up);
            EligibleKeys.Add(TBAInputs.Down);
            EligibleKeys.Add(TBAInputs.Jump);
            EligibleKeys.Add(TBAInputs.CABind());
            EligibleKeys.Add(TBAInputs.EA1Bind());
            EligibleKeys.Add(TBAInputs.EA2Bind());
        }

        public int ComboResetTimer { get; set; }

        public List<string> EligibleKeys { get; } = new List<string>();
        public List<ComboInput> Inputs { get; } = new List<ComboInput>(10);
    }
}
