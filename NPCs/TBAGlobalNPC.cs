using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.States;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.NPCs
{
    public sealed class TBAGlobalNPC : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            if (TimeStopManagement.IsNPCImmune(npc))
                return true;

            IsStopped = TimeStopManagement.TimeStopped;

            if (IsStopped)
            {
                if (!TimeStopManagement.npcStates.ContainsKey(npc))
                    TimeStopManagement.RegisterStoppedNPC(npc);

                TimeStopManagement.npcStates[npc].PreAI(npc);
                return false;
            }

            return true;
        }


        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit) =>
            ModifyHitByPlayer(npc, player, player.direction, ref damage, ref knockback, ref crit);

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) =>
            ModifyHitByPlayer(npc, projectile.owner == 255 ? null : Main.player[projectile.owner], hitDirection, ref damage, ref knockback, ref crit);

        public void ModifyHitByPlayer(NPC npc, Player player, int hitDirection, ref int damage, ref float knockback, ref bool crit)
        {
            if (IsStopped && TimeStopManagement.TimeStopped)
            {
                NPCInstantState state = TimeStopManagement.npcStates[npc];

                state.AccumulatedDamage += damage;
                state.AccumulatedKnockback += knockback;
                state.AccumulatedHitDirection = hitDirection;

                damage = 0;
                knockback = 0;
            }
        }
        

        public bool IsStopped { get; private set; }

        public override bool InstancePerEntity => true;
    }
}