using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Projectiles.Stands
{
    public abstract class StandBase : ModProjectile
    {
        public string currentState;

        private bool _hasSetAnimations;

        public bool 
            isFlipped, isTaunting;

        public StandBase(string name)
        {
            StandName = name;

            currentState = "SUMMON"; // first animation *must* be have a key of "SUMMMON"

            Animations = new Dictionary<string, SpriteAnimation>();
        }

        public sealed override void SetStaticDefaults()
        {
            DisplayName.SetDefault(StandName);
        }

        public sealed override void SetDefaults()
        {
            SetChildDefaults();

            projectile.aiStyle = -1;
            projectile.penetrate = -1; // stand istelf should almost never hit anything by its own, this serves as fool proof though
            projectile.tileCollide = false; // tile collisions should be done manually
        }

        public abstract void SetChildDefaults();

        public abstract void AddAnimations();

        public override bool PreAI()
        {
            if(!_hasSetAnimations)
            {
                AddAnimations();

                if (Animations.Count >= 1)
                {
                    projectile.width = (int)Animations[currentState].FrameSize.X;
                    projectile.height = (int)Animations[currentState].FrameSize.Y;
                }

                _hasSetAnimations = true;
            }

            if(Animations.Count >= 1)
            {
                Animations[currentState].Update();
            }

            return true;
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(currentState);
            writer.Write(isFlipped);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            currentState = reader.ReadString();

            isFlipped = reader.ReadBoolean();
        }

        // getting rid of vanilla drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            TBAPlayer tbaPlayer = TBAPlayer.Get(Main.player[Main.myPlayer]);

            // Stand shouldn't be drawn in 2 scenarios:
            // 1) We aren't a stand user, so we can't see stands
            // 2) Someone fucked shit up and forgot to fill Animations smh;
            if (!tbaPlayer.StandUser || Animations.Count <= 0)
                return;

            SpriteEffects spriteEffects = isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(Animations[currentState].SpriteSheet, projectile.Center - Main.screenPosition, Animations[currentState].FrameRect, Color.White * Opacity, projectile.rotation, Animations[currentState].DrawOrigin, 1f, spriteEffects, 1f);
        }

        public override void Kill(int timeLeft)
        {
            Animations.Clear();

            TBAPlayer.Get(Owner).ActiveStandID = -99;
        }

        public Dictionary<string, SpriteAnimation> Animations { get; }

        public string StandName { get; }

        // automaticly supplies all future stands with a transparent texture so it won't ever draw
        // even if it gets past PreDraw somehow
        public sealed override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";

        public Player Owner => Main.player[projectile.owner];

        public float Opacity { get; set; }
    }
}
