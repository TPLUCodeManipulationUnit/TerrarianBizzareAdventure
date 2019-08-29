using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrarianBizzareAdventure.Managers;

namespace TerrarianBizzareAdventure.Stands
{
    public sealed class StandManager : SingletonManager<StandManager, Stand>
    {
        internal override void DefaultInitialize()
        {
            Assembly myAssembly = Assembly.GetAssembly(typeof(Stand));

            foreach (Type type in myAssembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Stand))))
                Add(Activator.CreateInstance(type) as Stand);

            base.DefaultInitialize();
        }
    }
}
