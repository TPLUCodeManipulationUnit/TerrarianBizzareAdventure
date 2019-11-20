using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Projectiles;

namespace TerrarianBizzareAdventure.Stands.KingCrimson
{
    public class KingCrimson : Stand
    {
        public KingCrimson() : base("howDoesItWork", "King Crimson")
        {
            AuraColor = new Color(189, 0, 85);
        }

        public override void AddAnimations()
        {
            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCSpawn"), 7, 4));
            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCIdle"), 5, 10));

            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCSpawn"), 7, 4));

            Animations.Add("PUNCH_R", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPunchRight"), 4, 5));
            Animations.Add("PUNCH_L", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCPunchLeft"), 4, 5));

            Animations["PUNCH_R"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["PUNCH_L"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);

            Animations.Add("CUT_PREP", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCCut"), 20, 3));
            Animations.Add("CUT_ATT", new SpriteAnimation(mod.GetTexture("Stands/KingCrimson/KCYeet"), 13, 3));

            Animations["CUT_ATT"].SetNextAnimation(Animations[ANIMATION_IDLE]);
            Animations["CUT_PREP"].SetNextAnimation(Animations["CUT_ATT"]);
        }

        public override void AI()
        {
            base.AI();

            if (Animations.Count <= 0)
                return;

            IsFlipped = Owner.direction == 1;

            projectile.timeLeft = 200;
            projectile.friendly = true;

            if (Owner.whoAmI == Main.myPlayer)
            {
                /*if (TBAInputs.StandPose.JustPressed)
                    if (CurrentState == ANIMATION_IDLE)
                        IsTaunting = true;
                    else
                        IsTaunting = false;*/

                if (TBAInputs.SummonStand.JustPressed && CurrentState == ANIMATION_IDLE)
                    CurrentState = ANIMATION_DESPAWN;
            }

            Vector2 lerpPos = Vector2.Zero;

            int xOffset = IsTaunting || CurrentState.Contains("PUNCH") || CurrentState.Contains("CUT_ATT")? 34 : -16;

            lerpPos = Owner.Center + new Vector2(xOffset * Owner.direction, -24 + Owner.gfxOffY);

            projectile.Center = Vector2.Lerp(projectile.Center, lerpPos, 0.26f);

            if (CurrentState == ANIMATION_SUMMON)
            {
                Opacity = 1;
            }

            // we only punch if are in IDLE state AND MouseTime is lower than 10. If it was higher, we'd do a donut punch
            // instead.
            if (CurrentState == ANIMATION_IDLE)
            {
                if (TBAPlayer.Get(Owner).MouseTimeReset > 0)
                {
                    if (TBAPlayer.Get(Owner).MouseTime < 10 && !Owner.controlUseItem)
                    {
                        Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
                        CurrentState = "PUNCH_" + (Main.rand.NextBool() ? "R" : "L");
                        Projectile.NewProjectile(projectile.Center, VectorHelpers.DirectToMouse(projectile.Center, 22f), ModContent.ProjectileType<Punch>(), 120, 3.5f, Owner.whoAmI, projectile.whoAmI);
                    }
                    if (TBAPlayer.Get(Owner).MouseTime >= 10)
                    {
                        Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
                        CurrentState = "CUT_PREP";
                        Projectile.NewProjectile(projectile.Center, VectorHelpers.DirectToMouse(projectile.Center, 22f), ModContent.ProjectileType<Punch>(), 120, 3.5f, Owner.whoAmI, projectile.whoAmI);
                    }
                }
            }

            // If we do a YEET attack, damage is dealt by stand itself instead of a seperate projectile
            projectile.damage = CurrentState == "CUT_ATT" && CurrentAnimation.CurrentFrame > 3 && CurrentAnimation.CurrentFrame < 9 ? 400 : 0;
            Animations["CUT_PREP"].AutoLoop = Owner.controlUseItem;


            if (CurrentState == ANIMATION_IDLE && CurrentAnimation.Finished)
                CurrentAnimation.ResetAnimation();

            if (CurrentState == ANIMATION_DESPAWN)
            {
                Opacity = (5 - CurrentAnimation.FrameRect.Y / (int)CurrentAnimation.FrameSize.Y) * 0.2f;

                if (CurrentAnimation.Finished)
                    KillStand();
            }
        }
    }
}
