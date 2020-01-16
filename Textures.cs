using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace TerrarianBizzareAdventure
{
    public static class Textures
    {
        public static void Load(Mod mod)
        {
            TimeSkipVFX =
                ModContent.GetTexture("TerrarianBizzareAdventure/Textures/TimeSkipVFX");

            StaminaBar = ModContent.GetTexture("TerrarianBizzareAdventure/Textures/Bar");
            StaminaBarBorder = ModContent.GetTexture("TerrarianBizzareAdventure/Textures/BarBorder");

            StandCard = ModContent.GetTexture("TerrarianBizzareAdventure/UserInterfaces/Elements/StandCollection/StandCard");
            SCLock = ModContent.GetTexture("TerrarianBizzareAdventure/UserInterfaces/Elements/StandCollection/Locked");
            SCUnknown = ModContent.GetTexture("TerrarianBizzareAdventure/UserInterfaces/Elements/StandCollection/Unknown");
        }
        public static void Unload()
        {
            StaminaBar = null;
            StaminaBarBorder = null;
            TimeSkipVFX = null;

            StandCard = null;
            SCLock = null;
            SCUnknown = null;
        }

        public static Texture2D StaminaBar { get; private set; }
        public static Texture2D StaminaBarBorder { get; private set; }
        public static Texture2D TimeSkipVFX { get; private set; }

        public static Texture2D StandCard { get; private set; }
        public static Texture2D SCLock { get; private set; }
        public static Texture2D SCUnknown { get; private set; }
    }
}
