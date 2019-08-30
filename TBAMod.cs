using System.IO;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Network;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.TimeStop;

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
            SteamHelper.Initialize();

            TBAInputs.Load(this);
            TimeStopManagement.Load(this);
        }

        public override void Unload()
        {
            Instance = null;

            TBAInputs.Unload();
            TimeStopManagement.Unload();

            StandManager.Instance.Unload();
        }


        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetworkPacketManager.Instance.HandlePacket(reader, whoAmI);
        }


        public static TBAMod Instance { get; private set; }
    }
}