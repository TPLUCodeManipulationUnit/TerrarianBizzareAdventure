using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.States;
using TerrarianBizzareAdventure.TimeSkip;
using TerrarianBizzareAdventure.TimeStop;

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
            if (TimeStopManagement.IsNPCImmune(npc))
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

            var IsTimeSkipped = TimeSkipManager.IsTimeSkipped;

            if (IsTimeSkipped)
            {
                if (TimeSkipManager.TimeSkippedFor % 6 == 0)
                {
                        TimeSkipStates.Add
                        (
                            new TimeSkipState(npc.Center, npc.scale, npc.rotation, npc.frame, npc.direction)
                        );
                }
            }

            if (TimeSkipStates.Count > 12 || (!IsTimeSkipped && TimeSkipStates.Count > 0))
                TimeSkipStates.RemoveAt(0);

            if(TimeSkipStates.Count <= 0 && IsTimeSkipped)
                for(int i = 0; i < 13; i++)
                TimeSkipStates.Add
                (
                    new TimeSkipState(npc.Center, npc.scale, npc.rotation, npc.frame, npc.direction)
                );

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
                    spriteBatch.Draw(texture, TimeSkipStates[i].Position - Main.screenPosition, TimeSkipStates[i].Frame, (i == 1 ? LastDrawColor : Color.Red * 0.5f), TimeSkipStates[i].Rotation, drawOrig, TimeSkipStates[i].Scale, spriteEffects, 1f);
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (TimeSkipManager.IsTimeSkipped)
                drawColor = Color.Red * 0.5f;
            else
                LastDrawColor = Color.White;
        }

        public List<TimeSkipState> TimeSkipStates { get; private set; }

        public bool IsStopped { get; private set; }

        public Color LastDrawColor { get; private set; }

        public override bool InstancePerEntity => true;
    }
}