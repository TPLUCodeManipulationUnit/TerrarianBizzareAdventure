using Terraria;
using Terraria.UI;
using TerrarianBizzareAdventure.UserInterfaces.Special.RATM;

namespace TerrarianBizzareAdventure.UserInterfaces
{
    public class ResourcesLayer : GameInterfaceLayer
    {
        public ResourcesLayer(ResourcesState state) : base("TBA: Resources", InterfaceScaleType.UI)
        {
            State = state;
        }

        protected override bool DrawSelf()
        {
            if (State == null) // shouldn't really be ever equal to null, but just in case...
                return false;

            State.Draw(Main.spriteBatch);

            return true;
        }

        public ResourcesState State { get; }
    }
}
