using Terraria.ModLoader;

namespace TerrarianBizzareAdventure
{
    public static class TBAInputs
    {
        public static ModHotKey SummonStand;

        public static ModHotKey StandPose;

        public static void Load(Mod mod)
        {
            SummonStand = mod.RegisterHotKey("Summon Stand", "Z");

            StandPose = mod.RegisterHotKey("Pose", "X");
        }

        public static void Unload()
        {
            SummonStand = null;
            StandPose = null;
        }
    }
}
