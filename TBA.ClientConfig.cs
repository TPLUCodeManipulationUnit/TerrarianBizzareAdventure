using System.ComponentModel;
using Terraria.ModLoader.Config;
using TerrarianBizzareAdventure.Tiles;

namespace TerrarianBizzareAdventure
{
    public sealed class TBAConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [Header("Visuals")]

        [Label("Time Skip VFX for tiles")]
        [DefaultValue(true)]
        public bool TimeSkipShader { get; set; }

        public override void OnChanged()
        {
            TBAGlobalTile.ShaderEnabled = TimeSkipShader;
            TBAGlobalWall.ShaderEnabled = TimeSkipShader;
        }

        public override void OnLoaded()
        {
            TBAGlobalTile.ShaderEnabled = TimeSkipShader;
            TBAGlobalWall.ShaderEnabled = TimeSkipShader;
        }
    }
}
