using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Stands.GoldenWind.Aerosmith;
using TerrarianBizzareAdventure.ScreenModifiers;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public override void ModifyScreenPosition()
        {
            if (ScreenModifiers.Count > 0)
            {
                ScreenModifiers[0].UpdateScreenPosition(ref Main.screenPosition);
                ScreenModifiers[0].UpdateModifier(player);

                if (ScreenModifiers[0].LifeTimeEnded)
                    ScreenModifiers.RemoveAt(0);
            }

            /// TO-DO: Re-implement this
            if(StandActive && ActiveStandProjectile is AerosmithStand stand)
            {	
                if(!stand.IsDespawning && !stand.IsSpawning && !stand.IsReturning && !stand.IsPatroling)
                    Main.screenPosition = stand.Center + VectorHelpers.DirectToMouse(stand.Center, 16) - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            }

            if(PointOfInterest != Vector2.Zero)
            {
                Main.screenPosition = PointOfInterest - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            }
        }

        public List<ScreenModifier> ScreenModifiers { get; } = new List<ScreenModifier>();

        public Vector2 PointOfInterest { get; set; }
    }
}
