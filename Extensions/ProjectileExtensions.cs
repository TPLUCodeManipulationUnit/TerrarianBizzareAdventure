﻿using Microsoft.Xna.Framework;
using Terraria;

namespace TerrarianBizzareAdventure.Extensions
{
    public static class ProjectileExtensions
    {
        public static Vector2 GetProjectilePosition(this Projectile projectile) => projectile.position - Main.screenPosition;
    }
}
