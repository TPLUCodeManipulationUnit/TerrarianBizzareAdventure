using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Drawing;
using TerrarianBizzareAdventure.Enums;
using TerrarianBizzareAdventure.Networking;
using TerrarianBizzareAdventure.Stands;
using WebmilioCommons.Extensions;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            ModifySCARLayers();

            layers.Insert(0, standAuraLayer);
        }

        public void ResetDrawingEffects()
        {
            if (AuraAnimations.Count <= 0)
                FillAnimations();

            if (Main.LocalPlayer.whoAmI == player.whoAmI)
            {
                int key = 3;

                int stamina = (Stamina * 100) / MaxStamina;

                if (stamina <= 100)
                    key = 3;

                if (stamina <= 75)
                    key = 2;

                if (stamina <= 50)
                    key = 1;

                if (stamina <= 25)
                    key = 0;

                AuraAnimationKey = key;
            }

            if (AuraAnimation == null || AuraAnimation != AuraAnimations[AuraAnimationKey])
            {
                AuraAnimation = AuraAnimations[AuraAnimationKey];
            }

            if (StandActive)
            {
                if (Opacity < 0.7f)
                    Opacity += 0.035f;
            }
            else
            {
                if (Opacity > 0)
                    Opacity -= 0.035f;
            }

            AuraAnimation?.Update();
        }

        public void FillAnimations()
        {
            ServerFriendlyAnimation low = new ServerFriendlyAnimation("Textures/PlayerVFX/Aura_LowStamina", 80, 880, 11, 5, true);
            ServerFriendlyAnimation medium = new ServerFriendlyAnimation("Textures/PlayerVFX/Aura_Medium", 80, 880, 11, 5, true);
            ServerFriendlyAnimation high = new ServerFriendlyAnimation("Textures/PlayerVFX/Aura_HighStamina", 80, 880, 11, 5, true);
            ServerFriendlyAnimation veryhigh = new ServerFriendlyAnimation("Textures/PlayerVFX/Aura_VeryHigh", 80, 880, 11, 5, true);

            AuraAnimations.Add(0, low);
            AuraAnimations.Add(1, medium);
            AuraAnimations.Add(2, high);
            AuraAnimations.Add(3, veryhigh);
        }

        public int ChooseAnimation()
        {
            int key = 0;

            int stamina = (Stamina * 100) / MaxStamina;

            if (stamina <= 100)
                key = 3;

            if (stamina <= 75)
                key = 2;

            if (stamina <= 50)
                key = 1;

            if (stamina <= 25)
                key = 0;

            return key;
        }


        public readonly PlayerLayer standAuraLayer = new PlayerLayer("TBAMod", "StandAura", PlayerLayer.Body, delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player drawPlayer = drawInfo.drawPlayer;

            TBAPlayer standUser = Get(drawPlayer);

            if (drawPlayer.dead || standUser.AuraAnimation == null) // If the player can't use the item, don't draw it.
                return;

            Color drawColor = standUser.Stand == null ? Color.White : standUser.Stand.AuraColor;


            if (standUser.StandActive)
                drawColor = standUser.ActiveStandProjectile.AuraColor;


            DrawData auraData = new DrawData
            (
                TBAMod.Instance.GetTexture(standUser.AuraAnimation.SpritePath),
                new Vector2((int)drawPlayer.MountedCenter.X, (int)drawPlayer.MountedCenter.Y + drawPlayer.gfxOffY - 4) - Main.screenPosition,
                standUser.AuraAnimation.FrameRect,
                drawColor * standUser.Opacity,
                0,
                standUser.AuraAnimation.DrawOrigin,
                1f,
                drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                1
                );

            DrawData auraData2ElectricBoogaloo = new DrawData
            (
                TBAMod.Instance.GetTexture(standUser.AuraAnimation.SpritePath),
                new Vector2((int)drawPlayer.MountedCenter.X, (int)drawPlayer.MountedCenter.Y + ((drawPlayer.gfxOffY - 6) * 1.2f)) - Main.screenPosition,
                standUser.AuraAnimation.FrameRect,
                drawColor * standUser.Opacity * 0.5f,
                0,
                standUser.AuraAnimation.DrawOrigin,
                1.1f,
                drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                1
                );

            Main.playerDrawData.Add(auraData2ElectricBoogaloo);
            Main.playerDrawData.Add(auraData);
        });


        public ServerFriendlyAnimation AuraAnimation { get; set; }

        private int _auraAnimationKey;

        public int AuraAnimationKey
        {
            get => _auraAnimationKey;
            set
            {
                if (_auraAnimationKey == value) 
                    return;

                _auraAnimationKey = value;

                this.SendIfLocal<AuraSyncPacket>();
            }
        }

        public float Opacity { get; private set; }

        public static Dictionary<int, ServerFriendlyAnimation> AuraAnimations
            = new Dictionary<int, ServerFriendlyAnimation>();
    }
}
