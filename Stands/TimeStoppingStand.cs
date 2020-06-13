using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Stands
{
    public abstract class TimeStoppingStand : PunchBarragingStand
    {
        protected TimeStoppingStand(string unlocalizedName, string name) : base(unlocalizedName, name)
        {
        }

        public void TimeStop()
        {
            if (TimeStopManagement.TimeStopped)
            {
                TimeStopManagement.TryResumeTime(TBAPlayer.Get(Owner));
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, TimeStopRestorePath));
                return;
            }

            if (TBAPlayer.Get(Owner).CheckStaminaCost(TimeStopCost))
            {
                if (!TimeStopManagement.TimeStopped)
                    TBAMod.PlayVoiceLine(TimeStopVoiceLinePath);

                CurrentState = ANIMATION_IDLE;
                IsTaunting = false;
                TimeStopDelay = 25;
            }
        }

        public virtual int TimeStopCost => 20;

        public virtual string TimeStopRestorePath => "Sounds/StarPlatinum/SP_TimeRestore";

        public virtual string TimeStopVoiceLinePath => "Sounds/StarPlatinum/SP_TimeStopCall";

        public int TimeStopDelay { get; set; }
    }
}
