using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerrarianBizzareAdventure.Drawing;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;

namespace TerrarianBizzareAdventure.UserInterfaces.Elements.StandCollection
{
    public class StandCard : UIElement
    {
        public StandCard(Stand stand)
        {
            Width.Set(140, 0);
            Height.Set(200, 0);

            Stand = stand;
            Stand.AddAnimations();

            Unlock = Stand.Animations["SUMMON"];
            Idle = Stand.Animations["IDLE"];
            CallPath = Stand.CallSoundPath;

            StandUnlocalizedName = Stand.UnlocalizedName;
            StandDisplayName = Stand.StandName;

            Idle.AutoLoop = true;

            Stand = null;
        }

        public override void Update(GameTime gameTime)  
        {
            base.Update(gameTime);


            if(Main.GameUpdateCount % 1 == 0)
                Idle.Update();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dims = base.GetDimensions();
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);

            if (this.ContainsPoint(MousePosition))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Texture2D texture = TBAPlayer.Get().Stand.StandName == StandDisplayName ? Textures.SCCurrent : Textures.StandCard;

            spriteBatch.Draw(texture, dims.Position(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            if (!Unlocked)
            {
                spriteBatch.Draw(Textures.SCUnknown, dims.Position() + new Vector2(70), null, Color.White, 0f, new Vector2(Textures.SCUnknown.Width / 2, Textures.SCUnknown.Height / 2), 1f, SpriteEffects.None, 1f);
                Utils.DrawBorderString(spriteBatch, "???", dims.Position() + new Vector2(56, 160), Color.White);
            }

            if (Unlocked)
            {
                Vector2 anchor = dims.Position() + new Vector2(16, 160);
                spriteBatch.Draw(Idle.SpriteSheet, dims.Position() + new Vector2(70), Idle.FrameRect, Color.White, 0f, Idle.DrawOrigin, 1f, SpriteEffects.None, 1f);
                Utils.DrawBorderString(spriteBatch, StandDisplayName, anchor, Color.White, 1);
            }
        }

        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);

            if(Unlocked)
             Main.PlaySound(TBAMod.Instance.GetLegacySoundSlot(SoundType.Custom, CallPath));
        }

        public bool Unlocked => TBAPlayer.Get(Main.LocalPlayer).UnlockedStands.Contains(StandUnlocalizedName);

        public string StandUnlocalizedName { get; }

        public string StandDisplayName { get; }

        public string CallPath { get; }

        public SpriteAnimation Unlock { get; }

        public SpriteAnimation Idle { get; }

        public Stand Stand { get; }
    }
}
