using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (ActiveStandProjectileId == ACTIVE_STAND_PROJECTILE_INACTIVE_ID)
                return;

            Stand stand = Main.projectile[ActiveStandProjectileId].modProjectile as Stand;

            if (stand != null)
                layers.Insert(0, standAuraLayer);
        }

        public void ResetDrawingEffects()
        {
            Stand stand = null;
            if(ActiveStandProjectileId != ACTIVE_STAND_PROJECTILE_INACTIVE_ID)
                stand = Main.projectile[ActiveStandProjectileId].modProjectile as Stand;

            AuraAnimation?.Update();

            if (stand != null)
            {
                if (AuraAnimation == null && stand.CurrentState == Stand.ANIMATION_SUMMON)
                    AuraAnimation = new SpriteAnimation(TBAMod.Instance.GetTexture("Textures/AuraSpawn"), 9, 4);

                if (AuraAnimation != null && AuraAnimation.Finished && !AuraAnimation.AutoLoop)
                {
                    AuraAnimation = new SpriteAnimation(TBAMod.Instance.GetTexture("Textures/AuraAnimation"), 11, 4, true);
                }
                if (stand.CurrentState == Stand.ANIMATION_DESPAWN && AuraAnimation != null && AuraAnimation.AutoLoop)
                {
                    AuraAnimation = new SpriteAnimation(TBAMod.Instance.GetTexture("Textures/AuraSpawn"), 9, 4);
                    AuraAnimation.ResetAnimation(true);
                }
            }
            else
                AuraAnimation = null;
        }

        public readonly PlayerLayer standAuraLayer = new PlayerLayer("TBAMod", "StandAura", PlayerLayer.Body, delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player drawPlayer = drawInfo.drawPlayer;

            TBAPlayer tPlayer = Get(drawPlayer);

            if (drawPlayer.dead || tPlayer.Stand == null || tPlayer.AuraAnimation == null) // If the player can't use the item, don't draw it.
                return;

            DrawData auraData = new DrawData
            (
                tPlayer.AuraAnimation.SpriteSheet,
                new Vector2((int)drawPlayer.MountedCenter.X, (int)drawPlayer.MountedCenter.Y + drawPlayer.gfxOffY - 4) - Main.screenPosition,
                tPlayer.AuraAnimation.FrameRect,
                tPlayer.Stand.AuraColor,
                0,
                tPlayer.AuraAnimation.DrawOrigin,
                1f,
                drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                1
                );

            Main.playerDrawData.Add(auraData);
        });

        public SpriteAnimation AuraAnimation { get; private set; }
    }
}
