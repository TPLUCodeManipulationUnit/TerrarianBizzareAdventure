using System.Linq;
using Terraria;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Loaders;

namespace TerrarianBizzareAdventure.Stands
{
    public sealed class StandLoader : SingletonLoader<StandLoader, Stand>
    {
        public Stand GetRandom(TBAPlayer tbaPlayer)
        {
            Stand stand = null;

            while (stand == null || !stand.CanAcquire(tbaPlayer) || tbaPlayer.Stand == stand)
                stand = GetRandom();

            return stand;
        }


        public Stand GetRandom() => Main.rand.Next(genericByType.Values.ToArray());
    }
}
