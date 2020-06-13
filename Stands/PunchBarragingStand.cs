using Microsoft.Xna.Framework;

namespace TerrarianBizzareAdventure.Stands
{
    public abstract class PunchBarragingStand : Stand
    {
        protected PunchBarragingStand(string unlocalizedName, string name) : base(unlocalizedName, name)
        {
        }

        public override void AI()
        {
            base.AI();

            if (PunchCounterReset > 0)
                PunchCounterReset--;
            else
                PunchCounter = 0;

            if (RushTimer > 0)
                RushTimer--;
        }

        public Vector2 PunchRushDirection { get; set; }

        public int RushTimer { get; set; }

        public int PunchCounter { get; set; }
        public int PunchCounterReset { get; set; }
    }
}
