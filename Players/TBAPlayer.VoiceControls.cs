using Microsoft.Speech.Recognition;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.KingCrimson;
using TerrarianBizzareAdventure.Stands.StarPlatinum;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public void ProcessVoiceControls()
        {
            if (!VoiceRecognitionSystem.SuccesfulBoot)
                return;

            if (!StandUser)
                return;

            SpeechRecognitionEngine recEngine = VoiceRecognitionSystem.RecEngine;

            if (TBAInputs.VoiceRec.JustPressed)
            {
                VoiceRecognitionSystem.IsRecognizing = !VoiceRecognitionSystem.IsRecognizing;
                Main.NewText(VoiceRecognitionSystem.IsRecognizing);

                if (VoiceRecognitionSystem.IsRecognizing)
                {
                    VoiceRecognitionSystem.LoadRecEngine();
                    recEngine.RecognizeAsync(RecognizeMode.Multiple);
                    recEngine.SpeechRecognized += RecEngine_Recognized;
                }
                else
                {
                    recEngine.RecognizeAsyncStop();
                    recEngine.SpeechRecognized -= RecEngine_Recognized;
                }
            }
        }

        private void RecEngine_Recognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == Stand.StandName)
            {
                if (ActiveStandProjectileId == -999) // Minimal value for a DAT in SHENZEN.IO :haha:
                {
                    ActiveStandProjectileId = Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType(Stand.GetType().Name), 0, 0, player.whoAmI);

                    if (Stand.CallSoundPath != "")
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, Stand.CallSoundPath));
                }
            }

            if (e.Result.Text == "Erase Time" && Stand is KingCrimson)
            {
                if (ActiveStandProjectileId == -999)
                    return;

                KingCrimson kc = Main.projectile[ActiveStandProjectileId].modProjectile as KingCrimson;
                kc.EraseTime();
            }

            if (e.Result.Text == "Toki Yo Tomare" && Stand is StarPlatinumStand)
            {
                if (ActiveStandProjectileId == -999)
                    return;

                StarPlatinumStand sp = Main.projectile[ActiveStandProjectileId].modProjectile as StarPlatinumStand;
                sp.TimeStop();
            }

            if (e.Result.Text == "Ora" && Stand is StarPlatinumStand)
            {
                if (ActiveStandProjectileId == -999)
                    return;

                StarPlatinumStand sp = Main.projectile[ActiveStandProjectileId].modProjectile as StarPlatinumStand;
                sp.PunchCounter = 3;
                sp.Punching(true);
            }
        }
    }
}
