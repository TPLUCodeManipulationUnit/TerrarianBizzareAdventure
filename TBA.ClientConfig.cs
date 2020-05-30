using System.ComponentModel;
using Terraria.ModLoader.Config;
using TerrarianBizzareAdventure.Stands;
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

        [Label("Enable Stand Aura")]
        [DefaultValue(false)]
        public bool DrawStandAura { get; set; }

        [Header("Other")]

        [Label("Enable Voice Lines")]
        [DefaultValue(false)]
        public bool EnableVA { get; set; }

        public override void OnChanged()
        {
            TBAGlobalTile.ShaderEnabled = TimeSkipShader;
            TBAGlobalWall.ShaderEnabled = TimeSkipShader;

            Stand.DrawStandAura = DrawStandAura;

            TBAMod.Instance.VoiceLinesEnabled = EnableVA;
        }

        public override void OnLoaded()
        {
            TBAGlobalTile.ShaderEnabled = TimeSkipShader;
            TBAGlobalWall.ShaderEnabled = TimeSkipShader;

            Stand.DrawStandAura = DrawStandAura;

            TBAMod.Instance.VoiceLinesEnabled = EnableVA;
        }
    }
}
