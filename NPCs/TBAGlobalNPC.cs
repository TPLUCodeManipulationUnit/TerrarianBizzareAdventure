using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.Aerosmith;
using TerrarianBizzareAdventure.States;
using TerrarianBizzareAdventure.TimeSkip;
using TerrarianBizzareAdventure.TimeStop;
using System.Linq;

namespace TerrarianBizzareAdventure.NPCs
{
    public sealed class TBAGlobalNPC : GlobalNPC
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

            int TimeSkipDuration = TimeSkipManager.TimeSkippedFor;

            var IsTimeSkipped = TimeSkipManager.IsTimeSkipped;

            if (IsStopped)
            {
                if (!TimeStopManagement.npcStates.ContainsKey(npc))
                    TimeStopManagement.RegisterStoppedNPC(npc);

                TimeStopManagement.npcStates[npc].PreAI(npc);
                npc.frameCounter = 0;
                return false;
            }


            if (IsTimeSkipped)
            {
                if (TimeSkipManager.TimeSkippedFor % 6 == 0)
                {
                        TimeSkipStates.Add
                        (
                            new TimeSkipState(npc.Center, npc.velocity, npc.scale, npc.rotation, npc.frame, npc.direction, npc.ai)
                        );
                }
            }

            if (IsTimeSkipped && TimeSkipStates.Count > 12)
                TimeSkipStates.RemoveAt(0);

            if(TimeSkipStates.Count <= 0 && IsTimeSkipped)
                for(int i = 0; i < 13; i++)
                TimeSkipStates.Add
                (
                    new TimeSkipState(npc.Center, npc.velocity, npc.scale, npc.rotation, npc.frame, npc.direction, npc.ai)
                );

            if (IsTimeSkipped && TimeSkipDuration <= 2 && TimeSkipStates.Count > 0)
            {
                /*
                npc.ai = TimeSkipStates[0].AI;
                npc.Center = TimeSkipStates[0].Position;
                npc.scale = TimeSkipStates[0].Scale;
                npc.direction = TimeSkipStates[0].Direction;
                */
                TimeSkipStates.Clear();
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

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (TimeSkipManager.IsTimeSkipped)
            {
                Texture2D texture = Main.npcTexture[npc.type];
                int frameCount = Main.npcFrameCount[npc.type];
                int frameHeight = texture.Height / frameCount;

                Vector2 drawOrig = new Vector2(texture.Width * 0.5f, (texture.Height / frameCount) * 0.5f);

                for (int i = TimeSkipStates.Count - 1; i > 0; i--)
                {
                    SpriteEffects spriteEffects = TimeSkipStates[i].Direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    spriteBatch.Draw(texture, TimeSkipStates[i].Position - Main.screenPosition, TimeSkipStates[i].Frame, (i == 1 ? Color.White : Color.Red * 0.5f), TimeSkipStates[i].Rotation, drawOrig, TimeSkipStates[i].Scale, spriteEffects, 1f);
                }
            }
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

        public override bool InstancePerEntity => true;
    }
}