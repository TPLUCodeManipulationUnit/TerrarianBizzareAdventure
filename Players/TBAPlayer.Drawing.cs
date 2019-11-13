using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Enums;
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
                if (stand.CurrentState == Stand.ANIMATION_SUMMON)
                    AuraAnimationID = (int)AuraAnimationType.Spawn;

                if (AuraAnimation != null)
                    AuraAnimationID = (int)AuraAnimationType.Idle;

                if (stand.CurrentState == Stand.ANIMATION_DESPAWN)
                    AuraAnimationID = (int)AuraAnimationType.Despawn;

                if(OldAnimationID != AuraAnimationID)
                    new AuraSyncPacket().Send();


                OldAnimationID = AuraAnimationID;
            }
            else
            {
                AuraAnimationID = (int)AuraAnimationType.None;
                AuraAnimation = null;
            }

            if (!Main.dedServ)
            {
                if (AuraAnimationID == (int)AuraAnimationType.Spawn && AuraAnimation == null)
                    AuraAnimation = new SpriteAnimation(TBAMod.Instance.GetTexture("Textures/AuraSpawn"), 9, 4);

                if (AuraAnimationID == (int)AuraAnimationType.Idle && AuraAnimation != null && AuraAnimation.Finished && !AuraAnimation.AutoLoop)
                {
                    AuraAnimation = new SpriteAnimation(TBAMod.Instance.GetTexture("Textures/AuraAnimation"), 11, 4, true);
                }
                if (AuraAnimationID == (int)AuraAnimationType.Despawn && AuraAnimation != null && AuraAnimation.AutoLoop)
                {
                    AuraAnimation = new SpriteAnimation(TBAMod.Instance.GetTexture("Textures/AuraSpawn"), 9, 2);
                    AuraAnimation.ResetAnimation(true);
                }
            }
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

        public int OldAnimationID { get; private set; }

        public int AuraAnimationID { get; set; }

        public SpriteAnimation AuraAnimation { get; private set; }
    }
}
