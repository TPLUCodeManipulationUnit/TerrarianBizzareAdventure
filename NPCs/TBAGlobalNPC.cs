using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.GoldenWind.Aerosmith;
using TerrarianBizzareAdventure.States;
using TerrarianBizzareAdventure.TimeSkip;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.NPCs
{
    public sealed partial class TBAGlobalNPC : GlobalNPC
    {
        public override void SetDefaults(NPC npc)
        {
            TimeSkipStates = new List<TimeSkipState>();
        }

        public override bool PreAI(NPC npc)
        {
            if (TimeStopManagement.TimeStopped && TimeStopManagement.IsNPCImmune(npc))
                return true;

            IsStopped = TimeStopManagement.TimeStopped;


            if (IsStopped)
            {
                if (!TimeStopManagement.npcStates.ContainsKey(npc))
                    TimeStopManagement.RegisterStoppedNPC(npc);

                TimeStopManagement.npcStates[npc].PreAI(npc);
                npc.frameCounter = 0;
                return false;
            }

            PreTimeSkipAI(npc);

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

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            PostKingDraw(npc, spriteBatch, drawColor);
            PostSCARDraw(npc, spriteBatch, drawColor);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (TimeSkipManager.IsTimeSkipped)
                drawColor = Color.Red * 0.5f;
        }

        public override bool CheckActive(NPC npc)
        {
            List<Projectile> aerosmiths = Main.projectile.Where(x => x.modProjectile is AerosmithStand).ToList();

            bool canDespawn = true;

            foreach(Projectile p in aerosmiths)
            {
                if (Vector2.Distance(npc.Center, p.Center) <= 16 * 64)
                    canDespawn = false;
            }

            return canDespawn ? base.CheckActive(npc) : false;
        }

        public List<TimeSkipState> TimeSkipStates { get; private set; }

        public bool IsStopped { get; private set; }


        // Time that NPCs is stunned for after getting brain damaged
        public int CombatLockTimer { get; set; }

        public bool IsCombatLocked => CombatLockTimer > 0;

        public static TBAGlobalNPC GetFor(NPC npc) => npc.GetGlobalNPC<TBAGlobalNPC>();

        public override bool InstancePerEntity => true;

        public float RotationToRestore { get; set; } = -999.0f;
        public bool HasRotationToRestore => RotationToRestore != -999.0f;
    }
}