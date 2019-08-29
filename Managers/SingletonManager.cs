using System;
using System.Collections.Generic;

namespace TerrarianBizzareAdventure.Managers
{
    public class SingletonManager<TManager, TManagerOf> : Manager<TManagerOf> where TManager : Manager<TManagerOf>, new() where TManagerOf : IHasUnlocalizedName
    {
        private static TManager _instance;


        public List<TManagerOf> Where(Predicate<TManagerOf> predicate)
        {
            List<TManagerOf> found = new List<TManagerOf>();

            for (int i = 0; i < Count; i++)
                if (predicate(this[i]))
                    found.Add(this[i]);

            return found;
        }


        internal void Unload()
        {
            Clear();
            _instance = null;
        }


        public static TManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TManager();

                if (!_instance.Initialized)
                    _instance.DefaultInitialize();

                return _instance;
            }
        }
    }
}