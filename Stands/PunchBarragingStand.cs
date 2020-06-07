using Microsoft.Xna.Framework;

namespace TerrarianBizzareAdventure.Stands
{
    public abstract class PunchBarragingStand : Stand
    {
        protected PunchBarragingStand(string unlocalizedName, string name) : base(unlocalizedName, name)
        {
        }

        public Vector2 PunchRushDirection { get; set; }

        public int RushTimer { get; set; }
    }
}
