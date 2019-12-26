using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Tiles;

namespace TerrarianBizzareAdventure.TimeSkip
{
    public sealed class TimeSkipManager
    {
        internal static void UpdateTimeSkip()
        {
            if (!Init)
            {
                TwilightShader = GameShaders.Armor.GetShaderFromItemId(ItemID.TwilightDye).Shader;
                Init = true;
            }


            if (TimeSkippedFor > 0)
                TimeSkippedFor--;
            
            if(TimeSkippedFor <= 0)
            {
                TimeSkipper = null;
            }

            if(IsTimeSkipped)
            {
                #region Shader
                TwilightShader.Parameters["uColor"].SetValue(new Vector3(.65f, 0.1f, .65f));
                TwilightShader.Parameters["uSourceRect"].SetValue(new Vector4(0, 0, 16, 16));
                TwilightShader.Parameters["uSecondaryColor"].SetValue(new Vector3(1f, 1f, 1f));
                TwilightShader.Parameters["uTime"].SetValue(TwiTime);
                TwilightShader.Parameters["uOpacity"].SetValue(1);
                TwilightShader.Parameters["uSaturation"].SetValue(1);
                TwilightShader.Parameters["uRotation"].SetValue(0);
                TwilightShader.Parameters["uImageSize0"].SetValue(new Vector2(30, 330));
                TwilightShader.Parameters["uImageSize1"].SetValue(new Vector2(16, 16));

                if (Main.myPlayer == Main.LocalPlayer.whoAmI)
                    TwilightShader.Parameters["uWorldPosition"].SetValue(Main.LocalPlayer.position);
                #endregion
            }

            TwiTime = MathHelper.Lerp(TwiTime, 26f, 0.12f);

            if (TwiTime > 25f)
                TwiTime = 0;

            if (TimeSkippedFor < 36 && TimeSkippedFor > 1)
                FullCycle = false;

            if (TimeSkippedFor > 0)
            {
                if (!FullCycle)
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

        internal static void Load()
        {
            TwilightShader = GameShaders.Armor.GetShaderFromItemId(ItemID.TwilightDye).Shader;
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

        public static Effect TwilightShader { get; private set; }

        public static float TwiTime { get; private set; }

        public static bool Init { get; private set; }



        public static int FrameCounter { get; private set; }

        public static int CurrentFrame { get; private set; }

        public static bool FullCycle { get; private set; }
    }
}
