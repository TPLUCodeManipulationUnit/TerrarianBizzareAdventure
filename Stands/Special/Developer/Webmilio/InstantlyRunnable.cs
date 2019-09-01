using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio
{
    public abstract class InstantlyRunnable
    {
        public abstract void Run(TBAPlayer tbaPlayer);

        public virtual void Stop() { }
    }
}