using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using TerrarianBizzareAdventure.Drawing;
using TerrarianBizzareAdventure.Enums;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands.SREKT
{
    public class SREKTStand : Stand
    {
        public SREKTStand() : base("srekt20", "SCAR-20")
        {
            AuraColor = Main.DiscoColor;
        }

        public override void AddAnimations()
        {
            SCARState = SCARStates.Passive;

            XDirection = -16.5f;

            SpriteAnimation scarTexture = new SpriteAnimation(mod.GetTexture("Stands/SREKT/SREKT"), 1, 20);
            SpriteAnimation scarTexture3 = new SpriteAnimation(mod.GetTexture("Stands/SREKT/SREKT"), 1, 0);
            SpriteAnimation scarTexture2 = new SpriteAnimation(mod.GetTexture("Stands/SREKT/SREKT"), 1, 0, true);

            Animations.Add(ANIMATION_SUMMON, scarTexture);
            Animations.Add(ANIMATION_DESPAWN, scarTexture3);
            Animations.Add(ANIMATION_IDLE, scarTexture2);

            Animations[ANIMATION_SUMMON].SetNextAnimation(Animations[ANIMATION_IDLE]);
        }

        public override bool PreAI()
        {
            IsFlipped = projectile.velocity.X < 0;
            AuraColor = Main.DiscoColor;
            return base.PreAI();
        }

        public override void AI()
        {
            base.AI();

            AuraColor = Main.DiscoColor;

            Opacity = 1f;

            projectile.netUpdate = true;

            projectile.width = 16;
            projectile.height = 16;

            projectile.Center = Owner.MountedCenter + new Vector2(0, Owner.gfxOffY)+ projectile.velocity.SafeNormalize(-Vector2.UnitY) * 4f;

            Owner.heldProj = projectile.whoAmI;

            if (CurrentState == ANIMATION_DESPAWN && CurrentAnimation.Finished)
                KillStand();

            if (Main.rand.Next(15) <= 1)
            {
                XDirection *= -1.0f;
            }
            if (SCARState == SCARStates.Triggerbot)
            {
                if (Main.npc.Any(x => x.Distance(Owner.MountedCenter) <= 50 * 16 && 
                x.CanBeChasedBy(this)
                /*&& Collision.CanHitLine(Owner.MountedCenter - new Vector2(0, 8), 0, 0, x.position, x.Hitbox.Width, x.Hitbox.Height) */
                && TriggerBotCD <= 0))
                {
                    Main.PlaySound(SoundID.Item70);
                    TriggerBotCD = 16;

                    FlickDirection = (Main.npc.First(x => x.Distance(Owner.MountedCenter) <= 50 * 16
                    /*&& Collision.CanHitLine(Owner.MountedCenter - new Vector2(0, 8), 0, 0, x.position, x.Hitbox.Width, x.Hitbox.Height)*/ 
                    && x.CanBeChasedBy(this)).Center - Owner.MountedCenter).SafeNormalize(-Vector2.UnitY) * 16f;

                    FlickTicks = 8;
                    projectile.velocity = FlickDirection;
                    Projectile.NewProjectile(projectile.Center, projectile.velocity * 1.5f, ModContent.ProjectileType<SREKTBullet>(), 103, 0, Owner.whoAmI);
                }
                else
                    projectile.velocity = new Vector2(XDirection, 16.0f);
            }

            if (SCARState == SCARStates.Passive)
            {
                projectile.velocity = new Vector2(XDirection, 16.0f);
            }

            if (Main.LocalPlayer == Owner)
            {
                if (TBAInputs.SummonStand.JustPressed && CurrentState == ANIMATION_IDLE)
                    CurrentState = ANIMATION_DESPAWN;

                if (TBAInputs.ContextAction.JustPressed)
                {
                    TriggerActive = !TriggerActive;
                    if (TriggerActive)
                        Main.NewText("TRIGGERBOT v34: ON");
                    else
                        Main.NewText("TRIGGERBOT v34: OFF");

                }

                if (TBAInputs.ExtraAction01.JustPressed)
                {
                    Wallhack = !Wallhack;
                    if (Wallhack)
                        Main.NewText("WALLHACK v1.16: ON");
                    else
                        Main.NewText("WALLHACK v1.16: OFF");

                }
            }

            if (!TriggerActive && SCARState == SCARStates.Triggerbot)
                SCARState = SCARStates.Passive;

            if (TriggerActive && SCARState == SCARStates.Passive)
                SCARState = SCARStates.Triggerbot;

            if (SCARState == SCARStates.Aggressive)
                projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(-Vector2.UnitY) * 16f;

            if(TBAPlayer.Get(Owner).MouseOneTime > 0 && TBAPlayer.Get(Owner).MouseOneTime < 2)
            {
                Main.PlaySound(SoundID.Item70);
                   FlickDirection = (Main.MouseWorld - Owner.Center).SafeNormalize(-Vector2.UnitY) * 16f;
                FlickTicks = 8;
                projectile.velocity = FlickDirection;
                Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<SREKTBullet>(), 103, 0, Owner.whoAmI);
                SCARState = SCARStates.Aggressive;
            }

            if(SCARState == SCARStates.Aggressive && FlickTicks <= 0)
                SCARState = SCARStates.Passive;

            

            if (FlickDirection != Vector2.Zero)
                projectile.velocity = FlickDirection;

            projectile.rotation = projectile.velocity.ToRotation() + (IsFlipped ? (float)MathHelper.Pi : 0 );

            Owner.direction = projectile.velocity.X > 0 ? 1 : -1;

            if (FlickTicks > 0)
                FlickTicks--;
            else
                FlickDirection = Vector2.Zero;

            if (TriggerBotCD > 0)
                TriggerBotCD--;
        }

        public bool TriggerActive;

        public Vector2 FlickDirection { get; set; }

        public int FlickTicks { get; set; }

        public int TriggerBotCD { get; set; }

        public bool Wallhack { get; set; }

        public override bool CanAcquire(TBAPlayer tbaPlayer) => false;

        public SCARStates SCARState { get; set; }

        public float XDirection { get; set; }
    }
}
