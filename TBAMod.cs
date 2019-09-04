using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Items.Developer;
using TerrarianBizzareAdventure.Network;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.TimeStop;
using TerrarianBizzareAdventure.UserInterfaces;

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
            On.Terraria.Main.Update += MainOnUpdate;

            SteamHelper.Initialize();

            TBAInputs.Load(this);
            TimeStopManagement.Load(this);

            if (!Main.dedServ)
                UIManager.Load();
        }

        public override void Unload()
        {
            On.Terraria.Main.Update -= MainOnUpdate;

            Instance = null;

            TBAInputs.Unload();
            TimeStopManagement.Unload();

            StandManager.Instance.Unload();
        }


        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetworkPacketManager.Instance.HandlePacket(reader, whoAmI);
        }

        public override bool HijackGetData(ref byte messageType, ref BinaryReader reader, int playerNumber)
        {
            // TODO Fix this so it actually works
            /*if (Main.dedServ)
                return base.HijackGetData(ref messageType, ref reader, playerNumber);

            if (messageType != MessageID.ModPacket && TimeStopManagement.TimeStopped && TimeStopManagement.TimeStopper.player != Main.LocalPlayer)
            {
                return true;
            }
            else
                return base.HijackGetData(ref messageType, ref reader, playerNumber);*/

            return base.HijackGetData(ref messageType, ref reader, playerNumber);
        }


        private void MainOnUpdate(On.Terraria.Main.orig_Update orig, Main self, GameTime gameTime)
        {
            if (KingCrimsonAbilityTrigger.lagFor > 0)
            {
                KingCrimsonAbilityTrigger.lagFor--;

                if (KingCrimsonAbilityTrigger.lagFor <= 0)
                    KingCrimsonAbilityTrigger.lagger = null;
            }

            if (KingCrimsonAbilityTrigger.lagFor > 0)
            {
                if (KingCrimsonAbilityTrigger.lagger == Main.LocalPlayer)
                    orig(self, gameTime);
            }
            else
                orig(self, gameTime);
        }



        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Insert(0, new RATMLayer(UIManager.RATMState));
        }

        public override void UpdateUI(GameTime gameTime)
        {
            UIManager.Update(gameTime);
        }

        public static TBAMod Instance { get; private set; }
    }
}