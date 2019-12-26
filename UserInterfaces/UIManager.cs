using Microsoft.Xna.Framework;
using Terraria.UI;
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

            TimeSkipLayer = new TimeSkipLayer(new TimeSkipVFX());
        }

        public static void Update(GameTime gameTime)
        {
            if (RATMState.Visible)
                RATMUserInterface?.Update(gameTime);

            TimeSkipLayer.State.Update(gameTime);
        }

        public static RATMState RATMState { get; private set; }

        internal static UserInterface RATMUserInterface { get; private set; }

        public static TimeSkipLayer TimeSkipLayer { get; private set; }
    }
}
