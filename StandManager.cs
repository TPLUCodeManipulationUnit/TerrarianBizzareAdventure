using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Projectiles.Stands;

namespace TerrarianBizzareAdventure
{
    public class StandManager
    {
        private static StandManager _instance;

        public static StandManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StandManager();

                if (!_instance.Initialized)
                    _instance.Initialize();

                return _instance;
            }
        }


        private void Initialize()
        {
            StandPool = new List<int>();

            Assembly myAssembly = Assembly.GetAssembly(typeof(StandBase));
            
            foreach(Type type in myAssembly.GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(StandBase))))
            {
                StandBase stand = (StandBase)Activator.CreateInstance(type);

                Projectile proj = new Projectile();

                proj.SetDefaults(TerrarianBizzareAdventure.Instance.ProjectileType(stand.GetType().Name));
                proj.type = TerrarianBizzareAdventure.Instance.ProjectileType(stand.GetType().Name);
                StandPool.Add(proj.type);

                proj = null;
            }

            Initialized = true;
        }

        public List<int> StandPool { get; private set; }

        public bool Initialized { get; private set; }
    }
}
