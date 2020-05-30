using System.ComponentModel;
using Terraria.ModLoader.Config;
using TerrarianBizzareAdventure.Items.Tools;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.Tiles;

namespace TerrarianBizzareAdventure
{
    public sealed class TBAMultiplayerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Gameplay")]

        [Label("Enable debug commands")]
        [DefaultValue(false)]
        public bool EnableDebug { get; set; }

        [Label("Allow re-usage of Bizarre Arrow")]
        [DefaultValue(false)]
        public bool AllowStandRoll { get; set; }

        public override void OnChanged()
        {
            StandArrow.RerollAllowed = AllowStandRoll;
            EnableDebugCommands = EnableDebug;
        }

        public override void OnLoaded()
        {
            StandArrow.RerollAllowed = AllowStandRoll;
            EnableDebugCommands = EnableDebug;
        }


        public static bool EnableDebugCommands { get; set; }
    }
}
