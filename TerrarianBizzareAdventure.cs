using Terraria.ModLoader;

namespace TerrarianBizzareAdventure
{
	public class TerrarianBizzareAdventure : Mod
	{
		public TerrarianBizzareAdventure()
		{
		}

        public override void Load()
        {
            Instance = this;
            TBAInputs.Load(this);
        }

        public override void Unload()
        {
            Instance = null;
            TBAInputs.Unload();
        }


        public static TerrarianBizzareAdventure Instance { get; private set; }
    }
}