using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrarianBizzareAdventure.Players;
using WebmilioCommons.Loaders;
using WebmilioCommons.Managers;

namespace TerrarianBizzareAdventure.Stands
{
    public sealed class StandLoader : SingletonLoader<StandLoader, Stand>
    {
        public Stand GetRandom(TBAPlayer tbaPlayer)
        {
            Stand stand = null;

            while (stand == null || !stand.CanAcquire(tbaPlayer))
                stand = GetRandom();

            return stand;
        }


        public Stand GetRandom() => Main.rand.Next(genericByType.Values.ToArray());
    }
}
