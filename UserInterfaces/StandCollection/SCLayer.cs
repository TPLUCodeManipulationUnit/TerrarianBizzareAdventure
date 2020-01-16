using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;
using TerrarianBizzareAdventure.UserInterfaces.Special.RATM;

namespace TerrarianBizzareAdventure.UserInterfaces.StandCollection
{
    public class SCLayer : GameInterfaceLayer
    {
        public SCLayer(SCUIState state) : base("ratmLayer", InterfaceScaleType.UI)
        {
            State = state;
            State.Activate();
            UserInterface = new UserInterface();
            UserInterface.SetState(State);
        }

        public void Update(GameTime gameTime)
        {
            UserInterface.Update(gameTime);
        }

        protected override bool DrawSelf()
        {
            if (State == null) // shouldn't really be ever equal to null, but just in case...
                return false;

            if(State.Visible)
                State.Draw(Main.spriteBatch);

            return true;
        }

        public UserInterface UserInterface { get; }

        public SCUIState State { get; }
    }
}
