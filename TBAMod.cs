using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands;

namespace TerrarianBizzareAdventure
{
	public class TBAMod : Mod
	{
		public TBAMod()
		{
            Instance = this;
        }

        public override void Load()
        {
            TBAInputs.Load(this);
        }

        public override void Unload()
        {
            Instance = null;
            TBAInputs.Unload();

            StandManager.Instance.Clear();
        }


        public static TBAMod Instance { get; private set; }
    }
}