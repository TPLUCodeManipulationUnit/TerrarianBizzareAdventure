using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio
{
    public abstract class InstantlyRunnable
    {
        public abstract bool Run(TBAPlayer tbaPlayer);

        public virtual void Stop() { }
    }
}