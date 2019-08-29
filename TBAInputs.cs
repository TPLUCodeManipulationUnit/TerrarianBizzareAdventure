using Terraria.ModLoader;

namespace TerrarianBizzareAdventure
{
    public static class TBAInputs
    {
        public static void Load(Mod mod)
        {
            SummonStand = mod.RegisterHotKey("Summon Stand", "Z");
            StandPose = mod.RegisterHotKey("Pose", "X");
            ContextAction = mod.RegisterHotKey("Context Action", "C");
        }

        public static void Unload()
        {
            SummonStand = null;
            StandPose = null;
            ContextAction = null;
        }


        public static ModHotKey SummonStand { get; private set; }
        public static ModHotKey StandPose { get; private set; }
        public static ModHotKey ContextAction { get; private set; }
    }
}
