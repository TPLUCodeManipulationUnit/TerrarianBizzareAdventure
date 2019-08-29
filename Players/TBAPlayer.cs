using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public const int ACTIVE_STAND_PROJECTILE_INACTIVE_ID = -999;


        public static TBAPlayer Get(Player player) => player.GetModPlayer<TBAPlayer>();


        public override void PostUpdate()
        {
            if (AttackDirectionResetTimer > 0)
                AttackDirectionResetTimer--;
            else
                AttackDirection = 0;
        }


        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!StandUser) return;

            if (TBAInputs.SummonStand.JustPressed)
            {
                if (ActiveStandProjectileId == -999) // Minimal value for a DAT in SHENZEN.IO :haha:
                    ActiveStandProjectileId = Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType(Stand.GetType().Name), 0, 0, player.whoAmI);
            }

            if (ActiveStandProjectileId == ACTIVE_STAND_PROJECTILE_INACTIVE_ID)
                return;
        }

        public override void SetControls()
        {
            if (AttackDirection != 0)
            {
                player.velocity.X *= 0.2f;
                player.velocity.Y *= 0.01f;
                player.direction = AttackDirection;
                player.controlLeft = AttackDirection != 1;
                player.controlRight = AttackDirection != -1;
            }
        }


        public Stand Stand { get; set; }
        public int ActiveStandProjectileId { get; set; }


        public bool StandUser => Stand != null;


        public int AttackDirection { get; set; }

        public int AttackDirectionResetTimer { get; set; }
    }
}
