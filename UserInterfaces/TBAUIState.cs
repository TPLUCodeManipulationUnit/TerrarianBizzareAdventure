using Terraria.UI;

namespace TerrarianBizzareAdventure.UserInterfaces
{
    public abstract class TBAUIState : UIState, IHasVisibility
    {
        public virtual bool Visible { get; set; }
    }
}
