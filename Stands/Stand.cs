using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Projectiles;
using TerrarianBizzareAdventure.TimeStop;
using WebmilioCommons.Managers;
using System.Linq;

namespace TerrarianBizzareAdventure.Stands
{
    public abstract class Stand : ModProjectile, IHasUnlocalizedName, IProjectileHasImmunityToTimeStop
    {
        public const string
            ANIMATION_SUMMON = "SUMMON",
            ANIMATION_DESPAWN = "DESPAWN",
            ANIMATION_IDLE = "IDLE";


        protected Stand(string unlocalizedName, string name)
        {
            UnlocalizedName = "stand." + unlocalizedName;
            StandName = name;

            CurrentState = ANIMATION_SUMMON; // first animation *must* be have a key of "SUMMMON"

            Animations = new Dictionary<string, SpriteAnimation>();
        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(StandName);
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = -1;
            projectile.penetrate = -1; // stand istelf should almost never hit anything by its own, this serves as fool proof though
            projectile.tileCollide = false; // tile collisions should be done manually
        }




        public virtual bool IsNativelyImmuneToTimeStop(TBAPlayer tbaPlayer) => false;
        public virtual bool IsNativelyImmuneToTimeStop(Projectile projectile) => IsNativelyImmuneToTimeStop(TBAPlayer.Get(Main.player[projectile.owner]));


        public abstract void AddAnimations();


        public override bool PreAI()
        {
            projectile.netUpdate = true;
            projectile.netUpdate2 = true;

            if (Main.dedServ)
                return false;

            if (Owner.dead || !Owner.active)
                KillStand();

            if (!HasSetAnimations)
            {
                AddAnimations();

                if (Animations.Count >= 1)
                {
                    projectile.width = (int)Animations[CurrentState].FrameSize.X;
                    projectile.height = (int)Animations[CurrentState].FrameSize.Y;
                }

                HasSetAnimations = true;
            }

            if (Animations.Count >= 1 && !TimeStopManagement.projectileStates.ContainsKey(projectile))
            {
                Animations[CurrentState].Update();

                SpriteAnimation nextAnimation = CurrentAnimation.NextAnimation;

                bool reverseAnimation = CurrentAnimation.ReverseNextAnimation;

                if (CurrentAnimation.Finished && nextAnimation != null && Animations.ContainsValue(nextAnimation))
                {
                    CurrentAnimation.ResetAnimation(CurrentAnimation.ReversePlayback);
                    CurrentState = Animations.Where(x => x.Value == nextAnimation).Select(x => x.Key).First();
                    CurrentAnimation.ResetAnimation(reverseAnimation);
                }
            }

            return true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(CurrentState);
            writer.Write(IsFlipped);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CurrentState = reader.ReadString();

            IsFlipped = reader.ReadBoolean();
        }

        // Getting rid of vanilla drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            TBAPlayer tbaPlayer = TBAPlayer.Get(Main.LocalPlayer);

            // Stand shouldn't be drawn in 2 scenarios:
            // 1) We aren't a stand user, so we can't see stands
            // 2) Someone fucked shit up and forgot to fill Animations smh;
            if (!tbaPlayer.StandUser || Animations.Count <= 0)
                return;

            SpriteEffects spriteEffects = IsFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(Animations[CurrentState].SpriteSheet, projectile.Center - Main.screenPosition, Animations[CurrentState].FrameRect, Color.White * Opacity, projectile.rotation, Animations[CurrentState].DrawOrigin, 1f, spriteEffects, 1f);
        }


        public virtual void KillStand()
        {
            projectile.Kill();
            TBAPlayer.Get(Owner).ActiveStandProjectileId = TBAPlayer.ACTIVE_STAND_PROJECTILE_INACTIVE_ID;

            Animations.Clear();
        }


        public virtual bool CanAcquire(TBAPlayer tbaPlayer) => true;
        public virtual bool CanUse(TBAPlayer tbaPlayer) => CanAcquire(tbaPlayer);


        public string UnlocalizedName { get; }
        public string StandName { get; }



        public Dictionary<string, SpriteAnimation> Animations { get; }


        public bool IsFlipped { get; set; }
        public bool IsTaunting { get; set; }

        public bool HasSetAnimations { get; private set; }

        public string CurrentState { get; set; }
        public SpriteAnimation CurrentAnimation => Animations[CurrentState];

        public Color AuraColor { get; set; }


        // Automaticly supplies all future stands with a transparent texture so it won't ever draw
        // Even if it gets past PreDraw somehow
        public sealed override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";

        public Player Owner => Main.player[projectile.owner];

        public float Opacity { get; set; }

        public string CallSoundPath { get; set; }
    }
}
