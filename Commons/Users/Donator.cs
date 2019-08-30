namespace TerrarianBizzareAdventure.Commons.Users
{
    public sealed class Donator : User
    {
        public Donator(long steamId64, string displayName, ulong discordId) : base(steamId64, displayName, discordId)
        {
        }
    }
}