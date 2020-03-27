using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrarianBizzareAdventure.Drawing;

namespace TerrarianBizzareAdventure.Stands.TheWorld
{
    public class TheWorldStand : Stand
    {
        public TheWorldStand() : base("theWorld", "The World")
        {
            AuraColor = new Color(1.0f, 0.7f, 0.0f);
        }

        public override void AddAnimations()
        {
            string path = "Stands/TheWorld/";

            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(mod.GetTexture(path + "Spawn"), 7, 5));

            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(mod.GetTexture(path + "Idle"), 8, 8, true));

            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(mod.GetTexture(path + "Spawn"), 7, 5, false, null, true));
            Animations[ANIMATION_DESPAWN].ReversePlayback = true;

            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);
        }

        public override void AI()
        {
            base.AI();

            Opacity = 1f;

            IsFlipped = Owner.direction == 1;

            projectile.timeLeft = 200;

            Vector2 lerpPos = Vector2.Zero;

            int xOffset = -16;

            lerpPos = Owner.Center + new Vector2(xOffset * Owner.direction, -24 + Owner.gfxOffY);

            projectile.Center = Vector2.Lerp(projectile.Center, lerpPos, 0.26f);


            if (Owner.whoAmI == Main.myPlayer)
            {
                if (TBAInputs.StandPose.JustPressed)
                    if (CurrentState == ANIMATION_IDLE)
                        IsTaunting = true;
                    else
                        IsTaunting = false;

                if (TBAInputs.SummonStand.JustPressed && CurrentState == ANIMATION_IDLE)
                    CurrentState = ANIMATION_DESPAWN;
            }

            if (CurrentState == ANIMATION_DESPAWN && CurrentAnimation.Finished)
                KillStand();
        }
    }
}
