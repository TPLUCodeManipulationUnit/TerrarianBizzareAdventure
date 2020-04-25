using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.TimeSkip;

namespace TerrarianBizzareAdventure.NPCs
{
    public sealed partial class TBAGlobalNPC : GlobalNPC
    {
        public void PreTimeSkipAI(NPC npc)
        {
            var IsTimeSkipped = TimeSkipManager.IsTimeSkipped;

            int TimeSkipDuration = TimeSkipManager.TimeSkippedFor;

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

            if (TimeSkipStates.Count <= 0 && IsTimeSkipped)
                for (int i = 0; i < 13; i++)
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
        }

        public void PostKingDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
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
    }
}
