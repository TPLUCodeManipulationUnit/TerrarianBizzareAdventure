using Microsoft.Xna.Framework;
using Terraria.UI;
using TerrarianBizzareAdventure.UserInterfaces.Special.RATM;
using TerrarianBizzareAdventure.UserInterfaces.StandCollection;

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

            ResourcesLayer = new ResourcesLayer(new ResourcesState());

            SCLayer = new SCLayer(new SCUIState());

            StandComboLayer = new UIStandCombosLayer(new UIStandCombos());
        }

        public static void Update(GameTime gameTime)
        {
            if (RATMState.Visible)
                RATMUserInterface?.Update(gameTime);

            TimeSkipLayer.State.Update(gameTime);

            if(SCLayer.State.Visible)
                SCLayer.UserInterface.Update(gameTime);

            if (StandComboLayer.State.Visible)
                StandComboLayer.UserInterface.Update(gameTime);
        }

        public static RATMState RATMState { get; private set; }

        internal static UserInterface RATMUserInterface { get; private set; }

        public static TimeSkipLayer TimeSkipLayer { get; private set; }

        public static ResourcesLayer ResourcesLayer { get; private set; }

        public static SCLayer SCLayer { get; private set; }

        public static UIStandCombosLayer StandComboLayer { get; private set; }
    }
}
