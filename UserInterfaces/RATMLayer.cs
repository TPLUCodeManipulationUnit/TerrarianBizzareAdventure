using Terraria;
using Terraria.UI;
using TerrarianBizzareAdventure.UserInterfaces.Special.RATM;

namespace TerrarianBizzareAdventure.UserInterfaces
{
    public class RATMLayer : GameInterfaceLayer
    {
        public RATMLayer(RATMState state) : base("ratmLayer", InterfaceScaleType.UI)
        {
            State = state;
        }

        protected override bool DrawSelf()
        {
            if (State == null) // shouldn't really be ever equal to null, but just in case...
                return false;

            if(State.Visible)
                State.Draw(Main.spriteBatch);

            return true;
        }

        public RATMState State { get; }
    }
}
