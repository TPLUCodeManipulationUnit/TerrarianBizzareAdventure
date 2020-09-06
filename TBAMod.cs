using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.Stands.GoldenWind.KingCrimson;
using TerrarianBizzareAdventure.TimeSkip;
using TerrarianBizzareAdventure.TimeStop;
using TerrarianBizzareAdventure.UserInterfaces;
using WebmilioCommons.Networking;

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


            OnEdits.Instance.LoadEdits();

            //VoiceRecognitionSystem.Load();
            TBAInputs.Load(this);
            TimeStopManagement.Load();

            if (!Main.dedServ)
            {

                Textures.Load();

                Ref<Effect> screenRef = new Ref<Effect>(GetEffect("Effects/ShockwaveEffect")); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();

                SkyManager.Instance["TBA:TimeStopInvert"] = new PerfectlyNormalSky();
                Filters.Scene["TBA:TimeStopInvert"] = new Filter(new ScreenShaderData("FilterInvert"), EffectPriority.High);

                UIManager.Load();

                Filters.Scene["TBA:FreezeSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(.7f, .7f, .7f), EffectPriority.VeryHigh);
                SkyManager.Instance["TBA:FreezeSky"] = new FreezeSky();

                AddEquipTexture(null, EquipType.Head, "DiavoloHead", "TerrarianBizzareAdventure/Items/Armor/Vanity/Vinegar/DiavoloHead_Head");
                AddEquipTexture(null, EquipType.Body, "DiavoloBody", "TerrarianBizzareAdventure/Items/Armor/Vanity/Vinegar/DiavoloChest_Body", "TerrarianBizzareAdventure/Items/Armor/Vanity/Vinegar/DiavoloChest_Arms");
            }
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
            {
                return;
            }

            if(TimeSkipManager.IsTimeSkipped)
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/KingCrimsonMusic");
                priority = MusicPriority.BossHigh;
            }
        }
        public override void Unload()
        {
            Instance = null;

            TBAInputs.Unload();
            TimeStopManagement.Unload();

            OnEdits.Instance.UnloadEdits();

            Textures.Unload();

            StandLoader.Instance.Unload();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetworkPacketLoader.Instance.HandlePacket(reader, whoAmI);
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


        /*#region Packets

        public PlayerJoiningSynchronizationPacket PlayerJoiningSynchronizationPacket { get; private set; }
        public TimeStateChangedPacket TimeStateChangedPacket { get; private set; }

        public CompileAssemblyPacket CompileAssemblyPacket { get; private set; }
        public InstantlyRunnableRanPacket InstantlyRunnableRanPacket { get; private set; }

        #endregion*/



        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Insert(0, new RATMLayer(UIManager.RATMState));
            layers.Add(UIManager.TimeSkipLayer);
            layers.Insert(layers.FindIndex(x => x.Name == "Vanilla: Mouse Text"), UIManager.ResourcesLayer);
            layers.Insert(layers.FindIndex(x => x.Name == "Vanilla: Mouse Text"), UIManager.SCLayer);
            layers.Insert(layers.FindIndex(x => x.Name == "Vanilla: Mouse Text"), UIManager.StandComboLayer);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if(!Main.gameMenu)
            {
                DisableTileDraw = false;

                foreach(Projectile proj in Main.projectile)
                {
                    if (!proj.active)
                        continue;

                    if (proj.type == ModContent.ProjectileType<FakeTilesProjectile>())
                        DisableTileDraw = true;
                }
            }


            UIManager.Update(gameTime);
        }

        public static void PlayVoiceLine(string SoundPath)
        {
            if (!Instance.VoiceLinesEnabled)
                return;

            Main.PlaySound(Instance.GetLegacySoundSlot(SoundType.Custom, SoundPath));
        }

        public bool VoiceLinesEnabled { get; set; }

        public bool DisableTileDraw { get; set; }

        public static TBAMod Instance { get; private set; }
    }
}