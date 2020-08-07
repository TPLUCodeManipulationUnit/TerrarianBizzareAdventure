using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.GoldenWind.Aerosmith;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public override void ModifyScreenPosition()
        {
            if(StandActive && ActiveStandProjectile is AerosmithStand stand)
            {	
                if(!stand.IsDespawning && !stand.IsSpawning && !stand.IsReturning)
                    Main.screenPosition = stand.Center - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            }

            if(PointOfInterest != Vector2.Zero)
            {
                Main.screenPosition = PointOfInterest - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            }
        }

        public Vector2 PointOfInterest { get; set; }
    }
}
