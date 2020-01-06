using Microsoft.Speech.Recognition;
using System;
using System.Globalization;
using TerrarianBizzareAdventure.Stands;
using Microsoft.Win32;
namespace TerrarianBizzareAdventure
{
    internal class VoiceRecognitionSystem
    {
        public static void Load()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var hlkm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                RegistryKey keySpeech = hlkm.OpenSubKey(@"SOFTWARE\Microsoft\Speech");
                RegistryKey keySpeechLocale = hlkm.OpenSubKey(@"SOFTWARE\Microsoft\Speech Server\v11.0\DefaultProfiles\Tokens\SR_MS_en-US_TELE_11.0");

                if (keySpeech == null || keySpeechLocale == null)
                {
                    FailReason = "\nRegistry Entries aren't present in current registry";
                    SuccesfulBoot = false;
                    return;
                }

                RecEngine = new SpeechRecognitionEngine(new CultureInfo("en-US"));
                LoadRecEngine();
                SuccesfulBoot = true;
            }
            else
            {
                SuccesfulBoot = false;
                FailReason = "\nBooting on non-windows platform";
            }
        }

        public static void LoadRecEngine()
        {
            Choices commands = new Choices();
            commands.Add(new string[] { "Erase Time", "Toki Yo Tomare", "Ora" });
            StandLoader.Instance.ForAllGeneric(x => commands.Add(x.StandName));

            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);

            Grammar grammar = new Grammar(gBuilder);

            RecEngine.LoadGrammarAsync(grammar);
            RecEngine.SetInputToDefaultAudioDevice();
        }

        public static SpeechRecognitionEngine RecEngine { get; private set; }

        public static bool SuccesfulBoot { get; private set; }

        public static bool IsRecognizing { get; set; }

        public static string FailReason { get; set; }
    }
}
