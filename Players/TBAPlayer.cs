using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerrarianBizzareAdventure.Projectiles.Stands;
using TerrarianBizzareAdventure.Stands;

namespace TerrarianBizzareAdventure.Players
{
    public class TBAPlayer : ModPlayer
    {
        public static TBAPlayer Get(Player player) => player.GetModPlayer<TBAPlayer>();


        public override void PostUpdate()
        {
            if (AttackDirectionResetTimer > 0)
                AttackDirectionResetTimer--;
            else
                AttackDirection = 0;
        }


        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();

            if (StandUser)
                tag.Add(nameof(Stand), Stand.UnlocalizedName);

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(Stand)))
                Stand = StandManager.Instance[tag.GetString(nameof(Stand))];
        }


        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TBAInputs.SummonStand.JustPressed && StandUser)
                Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType(Stand.GetType().Name), 0, 0, player.whoAmI);
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


        public bool StandUser => Stand != null;


        public int AttackDirection { get; set; }

        public int AttackDirectionResetTimer { get; set; }
    }
}
