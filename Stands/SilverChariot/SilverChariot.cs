using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Drawing;
using TerrarianBizzareAdventure.Projectiles;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Helpers;
using System.Collections.Generic;

namespace TerrarianBizzareAdventure.Stands.SilverChariot
{
    public class SilverChariot : PunchBarragingStand
    {
        public SilverChariot() : base("silverChariot", "Silver Chariot")
        {
            AuraColor = new Color(230, 230, 230);
        }

        public override void AddAnimations()
        {
            string path = "TerrarianBizzareAdventure/Stands/SilverChariot/";

            Animations.Add(ANIMATION_SUMMON, new SpriteAnimation(path + "SCSpawn", 13, 4));
            Animations.Add(ANIMATION_IDLE, new SpriteAnimation(path + "SCIdle", 6, 6, true));

            Animations.Add("SLASH_DOWN", new SpriteAnimation(path + "SCSlashDown", 4, 6, false));
            Animations.Add("SLASH_DOWNALT", new SpriteAnimation(path + "SCSlashDownAlt", 4, 6, false));

            Animations.Add("SLASH_MID", new SpriteAnimation(path + "SCSlashMid", 4, 6, false));
            Animations.Add("SLASH_MIDALT", new SpriteAnimation(path + "SCSlashMidAlt", 4, 6, false));

            Animations.Add("SLASH_UP", new SpriteAnimation(path + "SCSlashUp", 4, 6, false));
            Animations.Add("SLASH_UPALT", new SpriteAnimation(path + "SCSlashUp", 4, 6, false));


            Animations.Add("RUSH_UP", new SpriteAnimation(path + "SCRushUp", 4, 6, false));
            Animations.Add("RUSH_MID", new SpriteAnimation(path + "SCRushMid", 4, 6, false));
            Animations.Add("RUSH_DOWN", new SpriteAnimation(path + "SCRushDown", 4, 6, false));

            Animations.Add(ANIMATION_DESPAWN, new SpriteAnimation(path + "SCIdle", 6, 6));

            foreach(KeyValuePair<string, SpriteAnimation> kvp in Animations)
            {
                if (kvp.Key == ANIMATION_DESPAWN || kvp.Key == ANIMATION_IDLE)
                    continue;

                kvp.Value.SetNextAnimation(Animations[ANIMATION_IDLE]);
            }

        }

        public override void AI()
        {
            base.AI();

            IsFlipped = Owner.direction == 1;

            if (Animations.Count > 0)
            {
                Animations["RUSH_DOWN"].AutoLoop = RushTimer > 0;
                Animations["RUSH_UP"].AutoLoop = RushTimer > 0;
                Animations["RUSH_MID"].AutoLoop = RushTimer > 0;
            }

            if (CurrentState == ANIMATION_SUMMON)
            {
                Width = Height = 40;

                if (CurrentAnimation.CurrentFrame < 9)
                IsFlipped = Owner.direction == -1;

                Opacity = 1f;
                XPosOffset = 18;
                YPosOffset = -24;
            }


            PositionOffset = Owner.Center + new Vector2(XPosOffset * -Owner.direction, YPosOffset);

            projectile.Center = Vector2.Lerp(projectile.Center, PositionOffset, 0.26f);

            if (CurrentState == ANIMATION_DESPAWN)
            {
                Opacity -= 0.12f;
                YPosOffset += 4;
                XPosOffset -= 3;

                if (CurrentAnimation.Finished)
                    KillStand();
            }

            if(InIdleState)
                XPosOffset = 18;
            if (CurrentState.Contains("SLASH") || CurrentState.Contains("RUSH"))
                XPosOffset = -32;

            #region controls
            if (Owner.whoAmI == Main.myPlayer)
            {
                if (InIdleState)
                {
                    if (TBAInputs.SummonStand.JustPressed)
                    {
                        CurrentState = ANIMATION_DESPAWN;
                    }


                    if (PunchCounter < 2)
                    {
                        if (TBAPlayer.Get(Owner).MouseOneTimeReset > 0)
                        {
                            projectile.netUpdate = true;
                            if (TBAPlayer.Get(Owner).MouseOneTime < 15 && !Owner.controlUseItem)
                            {
                                TBAPlayer.Get(Owner).CheckStaminaCost(2, true);
                                Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;

                                string useAlt = (Main.rand.NextBool() ? "" : "ALT");

                                if (Main.MouseWorld.Y > Owner.Center.Y + 60)
                                    CurrentState = "SLASH_DOWN" + useAlt;
                                else if (Main.MouseWorld.Y < Owner.Center.Y - 60)
                                    CurrentState = "SLASH_UP" + useAlt;
                                else
                                    CurrentState = "SLASH_MID" + useAlt;

                                PunchCounter++;

                                PunchCounterReset = 28;

                                Projectile.NewProjectile(projectile.Center, VectorHelpers.DirectToMouse(projectile.Center, 32f), ModContent.ProjectileType<Punch>(), 80, 3.5f, Owner.whoAmI, projectile.whoAmI);

                                CurrentAnimation.ResetAnimation();
                            }
                        }
                    }
                    else if (Owner.controlUseItem)
                    {
                        projectile.netUpdate = true;

                        TBAPlayer.Get(Owner).CheckStaminaCost(16);

                        if (Main.MouseWorld.Y > Owner.Center.Y + 60)
                            CurrentState = "RUSH_DOWN";

                        else if (Main.MouseWorld.Y < Owner.Center.Y - 60)
                            CurrentState = "RUSH_UP";

                        else
                            CurrentState = "RUSH_MID";

                        RushTimer = 180;

                        PunchRushDirection = VectorHelpers.DirectToMouse(projectile.Center, 22f);

                        TBAPlayer.Get(Owner).AttackDirectionResetTimer = RushTimer;
                        TBAPlayer.Get(Owner).AttackDirection = Main.MouseWorld.X < projectile.Center.X ? -1 : 1;

                        int barrage = Projectile.NewProjectile(projectile.Center, PunchRushDirection, ModContent.ProjectileType<ChariotBarrage>(), 60, 0, Owner.whoAmI);

                        if (Main.projectile[barrage].modProjectile is ChariotBarrage silverBarrage)
                        {
                            silverBarrage.RushDirection = PunchRushDirection;
                            silverBarrage.ParentProjectile = projectile.whoAmI;
                        }
                    }

                }
            }
            #endregion
        }
    }
}
