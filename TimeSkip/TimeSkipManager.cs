using Terraria;
using Terraria.ModLoader;

namespace TerrarianBizzareAdventure.TimeSkip
{
    public sealed class TimeSkipManager
    {
        internal static void UpdateTimeSkip()
        {
            if (TimeSkippedFor > 0)
                TimeSkippedFor--;
            
            if(TimeSkippedFor <= 0)
            {
                TimeSkipper = null;
            }

            if (TimeSkippedFor < 36 && TimeSkippedFor > 1)
                HasToPlayVFX = true;

            if (HasToPlayVFX)
            {
                if (!FullCycle)
                {
                    if (++FrameCounter >= 2)
                    {
                        if (++CurrentFrame > 22)
                        {
                            FullCycle = true;
                            CurrentFrame = 0;
                        }

                        FrameCounter = 0;
                    }
                }
                else
                    HasToPlayVFX = false;
            }
            else
            {
                FrameCounter = 0;
                FullCycle = false;
                CurrentFrame = 0;
            }

            if(TimeSkippedFor > 5 && TimeSkippedFor < 7)
                Main.PlaySound(TBAMod.Instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/StandAbilityEffects/TimeSkip"));

            if(TimeSkippedFor >= 610)
                Main.PlaySound(TBAMod.Instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/StandAbilityEffects/BigTimeSkip"));
        }

        internal static void SkipTime(ModPlayer Skipper, int duration)
        {
            TimeSkipper = Skipper;
            TimeSkippedFor = duration;

            new TimeSkipPacket() { Duration = duration }.Send();
        }

        public static ModPlayer TimeSkipper { get; set; }
        public static int TimeSkippedFor { get; set; }
        public static bool IsTimeSkipped => TimeSkippedFor > 0 && TimeSkippedFor <= 600;

        public static int FrameCounter { get; private set; }

        public static int CurrentFrame { get; private set; }

        public static bool FullCycle { get; private set; }

        public static int WaveQuality { get; private set; }

        public static bool HasToPlayVFX { get; set; }
    }
}
