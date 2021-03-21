using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Enums;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Stands
{
    public abstract class TimeStoppingStand : PunchBarragingStand
    {
        public const string TIMESTOP_ANIMATION = "TIMESTOP_PREPARE";

        protected TimeStoppingStand(string unlocalizedName, string name) : base(unlocalizedName, name)
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
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

                CurrentState = TIMESTOP_ANIMATION;
            }
        }

        public override void AI()
        {
            base.AI();

            if(BaseDamage == 0)
            {
                GetBaseDamage(DamageClass.Melee, Owner);
                Main.NewText(BaseDamage);
            }
        }

        public override void PostAI()
        {
            base.PostAI();

            if (CurrentState == TIMESTOP_ANIMATION)
            {
                if(CurrentAnimation.CurrentFrame == 10)
                    TimeStopDelay = 4;

                if (CurrentAnimation.Finished)
                {
                    CurrentAnimation.ResetAnimation();
                    CurrentState = ANIMATION_IDLE;
                }
            }
        }

        public override bool StopsItemUse => !Main.SmartCursorEnabled;

        public virtual int TimeStopCost => 20;

        public virtual string TimeStopRestorePath => "Sounds/StarPlatinum/SP_TimeRestore";

        public virtual string TimeStopVoiceLinePath => "Sounds/StarPlatinum/SP_TimeStopCall";

        public int TimeStopDelay { get; set; }
    }
}
