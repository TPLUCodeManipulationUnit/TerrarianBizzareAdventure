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

            var keys = kState.GetPressedKeys();

            if (keys.Length > TBAInputs.LastState.GetPressedKeys().Length
                && EligibleKeys.Contains(keys[keys.GetUpperBound(0)].ToString())
                )
            {
                ComboResetTimer = 60;
                Inputs.Add(new ComboInput(keys[keys.GetUpperBound(0)].ToString()));
            }

            if(MouseOneTimeReset > 0 && MouseOneTime < 2)
            {
                ComboResetTimer = 60;
                Inputs.Add(new ComboInput(MouseClick.LeftClick.ToString()));
            }

            string ech = "";

            foreach (ComboInput s in Inputs)
                ech += s.Key;
            
            Main.NewText(ech);

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

            EligibleKeys.Add(Main.cLeft);
            EligibleKeys.Add(Main.cRight);
            EligibleKeys.Add(Main.cUp);
            EligibleKeys.Add(Main.cDown);
            EligibleKeys.Add(Main.cJump);
            EligibleKeys.Add(TBAInputs.CABind());
            EligibleKeys.Add(TBAInputs.EA1Bind());
            EligibleKeys.Add(TBAInputs.EA2Bind());
        }

        public int ComboResetTimer { get; set; }

        public List<string> EligibleKeys { get; } = new List<string>();
        public List<ComboInput> Inputs { get; } = new List<ComboInput>();
    }
}
