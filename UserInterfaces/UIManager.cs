using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.UI;
using TerrarianBizzareAdventure.Managers;
using TerrarianBizzareAdventure.UserInterfaces.Special.RATM;

namespace TerrarianBizzareAdventure.UserInterfaces
{
    public static class UIManager
    {
        public static void Load()
        {
            RATMState = new RATMState();
            RATMState.Activate();

            RATMUserInterface = new UserInterface();
            RATMUserInterface.SetState(RATMState);
        }

        public static void Update(GameTime gameTime)
        {
            if (RATMState.Visible)
                RATMUserInterface?.Update(gameTime);
        }

        public static RATMState RATMState { get; private set; }

        internal static UserInterface RATMUserInterface { get; private set; }
    }
}
