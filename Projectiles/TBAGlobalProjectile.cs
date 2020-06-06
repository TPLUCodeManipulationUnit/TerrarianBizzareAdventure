using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.TimeSkip;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Projectiles
{
    public sealed partial class TBAGlobalProjectile : GlobalProjectile
    {
        public override void SetDefaults(Projectile projectile)
        {
            TimeSkipStates = new List<TimeSkipState>();
        }

        public override bool PreAI(Projectile projectile)
        {
            int tickLimit = TimeStopManagement.TimeStopped && projectile.owner == TimeStopManagement.TimeStopper.player.whoAmI ? 10 : 1;
            IsStopped = !TBAMod.Instance.TimeStopImmuneProjectiles.Contains(projectile.type) && TimeStopManagement.TimeStopped && !(projectile.modProjectile is IProjectileHasImmunityToTimeStop iisitts && iisitts.IsNativelyImmuneToTimeStop()) && RanForTicks > tickLimit && (!(projectile.modProjectile is Stand) && projectile.owner == TimeStopManagement.TimeStopper.player.whoAmI);

            var IsTimeSkipped = TimeSkipManager.IsTimeSkipped && projectile.hostile;

            RanForTicks++;

            PreTimeSkipAI(projectile);

            if (IsTimeSkipped && RanForTicks > 2 && RanForTicks < 60)
                return false;

            if (IsStopped)
            {
                if (!TimeStopManagement.projectileStates.ContainsKey(projectile))
                    TimeStopManagement.RegisterStoppedProjectile(projectile);

                TimeStopManagement.projectileStates[projectile].PreAI(projectile);

                projectile.frameCounter = 0;

                return false;
            }


            return true;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if(TimeStopManagement.TimeStopped)
            {
                for (int i = 0; i < Main.combatText.Length; i++)
                {
                    if (!Main.combatText[i].active)
                    {
                        if(!TBAPlayer.Get().CombatTextQue.ContainsKey(i))
                            TBAPlayer.Get().CombatTextQue.Add(i, damage);
                    }
                }
            }
        }

        public override void PostDraw(Projectile projectile, SpriteBatch spriteBatch, Color lightColor)
        {
            PostTimeSkipDraw(projectile, spriteBatch, lightColor);
        }

        public override bool ShouldUpdatePosition(Projectile projectile)
        {
            bool notMove = TimeSkipManager.IsTimeSkipped && RanForTicks > 2 && RanForTicks < 60;

            if (notMove)
                return false;

            return !IsStopped;
        }

        public bool IsStopped { get; private set; }

        public int RanForTicks { get; private set; }

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;


        public List<TimeSkipState> TimeSkipStates { get; private set; }
    }
}