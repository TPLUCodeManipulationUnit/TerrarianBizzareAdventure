using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Projectiles.Stands;
using TerrarianBizzareAdventure.Projectiles.Stands.Melee;

namespace TerrarianBizzareAdventure.Players
{
    public class TBAPlayer : ModPlayer
    {
        public override void Initialize()
        {
            ActiveStandID = -99;
            MyStand = -99;
        }

        public override void PostUpdate()
        {
            if (AttackDirectionResetTimer > 0)
                AttackDirectionResetTimer--;
            else
                AttackDirection = 0;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TBAInputs.SummonStand.JustPressed && MyStand !=  -99 && ActiveStandID == -99)
            {
                ActiveStandID = Projectile.NewProjectile(player.Center, Vector2.Zero, MyStand, 0, 0, player.whoAmI);
            }
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

        public int ActiveStandID { get; set; }

        public int MyStand { get; set; }

        public bool StandUser => MyStand != -99;

        public static TBAPlayer Get(Player player) => player.GetModPlayer<TBAPlayer>();

        public int AttackDirection { get; set; }

        public int AttackDirectionResetTimer { get; set; }
    }
}
