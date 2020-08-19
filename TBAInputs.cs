using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace TerrarianBizzareAdventure
{
    public static class TBAInputs
    {
        public static void Load(Mod mod)
        {
            SummonStand = mod.RegisterHotKey("Summon Stand", "Z");
            StandPose = mod.RegisterHotKey("Pose", "X");
            ContextAction = mod.RegisterHotKey("Context Action", "C");

            ExtraAction01 = mod.RegisterHotKey("Extra Action 01", null);
            ExtraAction02 = mod.RegisterHotKey("Extra Action 02", null);

            /*if(VoiceRecognitionSystem.SuccesfulBoot)
                VoiceRec = mod.RegisterHotKey("Voice controls", "V");
                */
            OpenCollection = mod.RegisterHotKey("Stand album", "[");
        }

        public static void Unload()
        {
            SummonStand = null;
            StandPose = null;
            ContextAction = null;

            ExtraAction01 = null;
            ExtraAction02 = null;

            OpenCollection = null;
        }


        public static KeyboardState CurrentState => Keyboard.GetState();

        public static KeyboardState LastState { get; set; }

        public static ModHotKey SummonStand { get; private set; }
        public static ModHotKey StandPose { get; private set; }

        public static ModHotKey ContextAction { get; private set; }
        public static string CABind(int index = 0)
        {
            if (ContextAction == null)
                return "";

            return ContextAction.GetAssignedKeys()[index].ToString();
        }

        public static ModHotKey ExtraAction01 { get; private set; }
        public static string EA1Bind(int index = 0)
        {
            if (ExtraAction01 == null)
                return "";

            return ExtraAction01.GetAssignedKeys()[index].ToString();
        }

        public static ModHotKey ExtraAction02 { get; private set; }
        public static string EA2Bind(int index = 0)
        {
            if (ExtraAction02 == null)
                return "";

            return ExtraAction02.GetAssignedKeys()[index].ToString();
        }
        public static ModHotKey VoiceRec { get; private set; }

        public static ModHotKey OpenCollection { get; private set; }

        public static string Up => Main.cUp;

        public static string Down => Main.cDown;

        public static string Left => Main.cLeft;

        public static string Right => Main.cRight;

        public static string Jump => Main.cJump;
    }
}
