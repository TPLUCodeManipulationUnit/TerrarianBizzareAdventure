using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands.StardustCrusaders.StarPlatinum;
using TerrarianBizzareAdventure.Stands.StardustCrusaders.TheWorld;
using TerrarianBizzareAdventure.TimeSkip;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.World
{
    public sealed class TBAWorld : ModWorld
    {
        public override void PostDrawTiles()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                TimeSkipManager.UpdateTimeSkip();
                TimeStopManagement.MainOnOnTick();
            }

            base.PostDrawTiles();
        }

        public override void PreUpdate()
        {
            if (Main.netMode == NetmodeID.SinglePlayer || Main.dedServ)
            {
                TimeSkipManager.UpdateTimeSkip();
                TimeStopManagement.MainOnOnTick();
            }


            TBAPlayer plr = TimeStopManagement.TimeStopper as TBAPlayer;

            if (TimeStopManagement.TimeStoppedFor <= 78 && TimeStopManagement.TimeStoppedFor > 76)
            {
                if (plr.StandUser)
                {

                    if (plr.Stand is TheWorldStand)
                    {
                        TBAMod.PlayVoiceLine("Sounds/TheWorld/TimeResume");
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/TheWorld/TheWorld_ZaWarudoReleaseSFX"));
                    }

                    if (plr.Stand is StarPlatinumStand)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/StarPlatinum/SP_TimeRestore"));
                    }
                }
            }

            base.PreUpdate();
        }


        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(TimeStopManagement.TimeStopped);

            if (TimeStopManagement.TimeStopped)
            {
                writer.Write(Main.time);
                writer.Write(Main.rainTime);

                writer.Write(TimeStopManagement.TimeStopper.player.whoAmI);
                writer.Write(TimeStopManagement.TimeStoppedFor);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            bool timeStopped = reader.ReadBoolean();

            if (timeStopped)
            {
                TimeStopManagement.MainTime = reader.ReadDouble();
                TimeStopManagement.MainRainTimer = reader.ReadInt32();

                TimeStopManagement.StopTime(TBAPlayer.Get(Main.player[reader.ReadInt32()]), reader.ReadInt32());
            }
            else
            {
                TimeStopManagement.ResumeTime();
            }
        }
    }
}