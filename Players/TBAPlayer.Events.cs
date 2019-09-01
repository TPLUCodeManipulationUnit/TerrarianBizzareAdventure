namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer
    {
        public delegate void ModPlayerEvent(TBAPlayer tbaPlayer);

        public static event ModPlayerEvent OnPostUpdate;
    }
}
