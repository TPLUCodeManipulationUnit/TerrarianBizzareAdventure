﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.Aerosmith;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public override void ModifyScreenPosition()
        {
            if(Stand is AerosmithStand && ActiveStandProjectileId != ACTIVE_STAND_PROJECTILE_INACTIVE_ID)
            {
                AerosmithStand stando = Main.projectile[ActiveStandProjectileId].modProjectile as AerosmithStand;
                if(stando != null && stando.CurrentState != "SUMMON")
                    Main.screenPosition = Main.projectile[ActiveStandProjectileId].Center - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            }

            if(PointOfInterest != Vector2.Zero)
            {
                Main.screenPosition = PointOfInterest - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            }
        }

        public Vector2 PointOfInterest { get; set; }
    }
}