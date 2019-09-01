using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using TerrarianBizzareAdventure.Extensions;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio
{
    public class WebmilioStand : Stand
    {
        private readonly string _texturePath;


        public WebmilioStand() : base("special.developer.webmilio.RATM", "Rage Against The Machine")
        {
            _texturePath = this.GetType().GetTexturePath();
        }


        public override void AddAnimations()
        {
            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(mod.GetTexture(_texturePath + "Summon"), 10, 5));
            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(mod.GetTexture(_texturePath + "Despawn"), 12, 2));

            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(mod.GetTexture(_texturePath + "Idle"), 2, 60, autoLoop: true));
        }


        public override void AI()
        {
            base.AI();

            Opacity = 1f;
            projectile.Center = Owner.Center + new Vector2(34 * Owner.direction, -10 + Owner.gfxOffY);
            IsFlipped = Owner.direction == 1;

            if (CurrentState == ANIMATION_SUMMON)
            {
                if (InstantEnvironment == null && Owner == Main.LocalPlayer)
                    InstantEnvironment = new InstantEnvironment();

                Opacity = CurrentAnimation.FrameRect.Y / CurrentAnimation.FrameRect.Height * 0.25f;

                if (CurrentAnimation.Finished)
                    CurrentState = ANIMATION_IDLE;
            }

            projectile.timeLeft = 200;

            if (Owner == Main.LocalPlayer)
            {
                if (TBAInputs.SummonStand.JustPressed && CurrentState == ANIMATION_IDLE)
                    CurrentState = ANIMATION_DESPAWN;

                if (TBAInputs.ContextAction.JustPressed && CurrentState == ANIMATION_IDLE)
                    InstantEnvironment.ExecuteClass(Path.Combine(Main.SavePath, "Mods", "Cache", "RATMClass.cs"));
            }


            if (CurrentState.Contains(ANIMATION_IDLE) && CurrentAnimation.Finished)
                CurrentAnimation.ResetAnimation();

            if (CurrentState == ANIMATION_DESPAWN)
            {
                Opacity = (12 - CurrentAnimation.FrameRect.Y / (int)CurrentAnimation.FrameSize.Y) * 0.2f;

                if (CurrentAnimation.Finished)
                {
                    InstantEnvironment = null;
                    KillStand();
                }
            }
        }


        public override bool CanAcquire(TBAPlayer tbaPlayer) =>
            tbaPlayer.player != Main.LocalPlayer || SteamHelper.Webmilio.SteamId64 == SteamHelper.SteamId64;


        public InstantEnvironment InstantEnvironment { get; private set; }
    }
}