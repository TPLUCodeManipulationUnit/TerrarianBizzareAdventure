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
        public const string ANIMATION_ERROR = "ERROR";
        private const int ERROR_LOOP_COUNT = 3;

        private readonly string _texturePath;
        private int _errorLoopCount = 0;


        public WebmilioStand() : base("special.developer.webmilio.RATM", "Rage Against The Machine")
        {
            _texturePath = this.GetType().GetTexturePath();
        }


        public override void AddAnimations()
        {
            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(mod.GetTexture(_texturePath + ANIMATION_SUMMON), 10, 5));
            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(mod.GetTexture(_texturePath + ANIMATION_DESPAWN), 12, 2));

            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(mod.GetTexture(_texturePath + ANIMATION_IDLE), 2, 60, autoLoop: true));
            Animations.Add(ANIMATION_ERROR, new SpriteAnimation(mod.GetTexture(_texturePath + ANIMATION_ERROR), 22, 4));
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

                if (!InitialSummonComplete)
                    Opacity = CurrentAnimation.FrameRect.Y / CurrentAnimation.FrameRect.Height * 0.25f;

                if (CurrentAnimation.Finished)
                {
                    InitialSummonComplete = true;
                    CurrentState = ANIMATION_IDLE;
                }
            }

            projectile.timeLeft = 200;

            if (Owner == Main.LocalPlayer)
            {
                if (TBAInputs.SummonStand.JustPressed && CurrentState == ANIMATION_IDLE)
                    CurrentState = ANIMATION_DESPAWN;

                if (TBAInputs.ContextAction.JustPressed && CurrentState == ANIMATION_IDLE)
                    if (!InstantEnvironment.ExecuteClass(Path.Combine(Main.SavePath, "Mods", "Cache", "RATMClass.cs")))
                        CurrentState = ANIMATION_ERROR;

                if (TBAInputs.StandPose.JustPressed && CurrentState == ANIMATION_IDLE)
                {
                    InstantEnvironment?.LastRan?.Stop();
                    InstantEnvironment = new InstantEnvironment();
                }

                if (TBAInputs.ExtraAction01.JustPressed && CurrentState == ANIMATION_IDLE)
                    InstantEnvironment?.LastRan?.Stop();

                if (TBAInputs.ExtraAction02.JustPressed && CurrentState == ANIMATION_IDLE)
                    InstantEnvironment?.LastRan?.Run(TBAPlayer.Get(Owner));
            }

            if (CurrentState == ANIMATION_ERROR && _errorLoopCount < ERROR_LOOP_COUNT)
            {
                if (CurrentAnimation.Finished)
                {
                    _errorLoopCount++;
                    CurrentAnimation.ResetAnimation();
                }

                if (_errorLoopCount == 1)
                {
                    _errorLoopCount = 0;
                    CurrentState = ANIMATION_SUMMON;
                    CurrentAnimation.ResetAnimation();
                }
            }

            if (CurrentState == ANIMATION_DESPAWN)
            {
                Opacity = (12 - CurrentAnimation.FrameRect.Y / (int)CurrentAnimation.FrameSize.Y) * 0.2f;

                if (CurrentAnimation.Finished)
                {
                    KillStand();
                }
            }
        }


        public override bool CanAcquire(TBAPlayer tbaPlayer) =>
            tbaPlayer.player != Main.LocalPlayer || SteamHelper.Webmilio.SteamId64 == SteamHelper.SteamId64;


        public InstantEnvironment InstantEnvironment { get; private set; }

        public bool InitialSummonComplete { get; private set; }
    }
}