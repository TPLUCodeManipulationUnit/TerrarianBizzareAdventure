using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Stands.TheWorld
{
    public class RoadRoller : ModProjectile, IProjectileHasImmunityToTimeStop
    {
        private const int MAX_DAMAGE = 6969;

        public bool IsNativelyImmuneToTimeStop() => projectile.timeLeft > 4;

        public override void SetDefaults()
        {
            projectile.width = 134;
            projectile.height = 72;
            projectile.timeLeft = 360;
            projectile.friendly = true;
            projectile.melee = true;

            projectile.tileCollide = false;

            Target = null;
            TargetType = -1;

            YOffset = 20;
        }

        public override void AI()
        {
            if (!HasNoTarget)
            {
                projectile.velocity = Vector2.Zero;
                projectile.Center = Target.Center - new Vector2(0, Target.height - YOffset);

                if (projectile.timeLeft % 10 == 0 && YOffset < 40)
                    YOffset++;
            }

            if (projectile.timeLeft <= 1)
            {
                projectile.penetrate = -1;
                projectile.damage = 1600;
                projectile.friendly = true;
            }
            if (HasNoTarget && projectile.velocity.Y < 16f)
                projectile.velocity.Y += 0.18f;


            projectile.velocity = Collision.TileCollision(projectile.position, projectile.velocity, projectile.width, projectile.height);

            if (projectile.velocity.Y == 0)
            {
                if (!HasTouchedGround)
                    projectile.damage = 0;

                HasTouchedGround = true;
            }

            if(!HasNoTarget)
            {
                if (!Target.active)
                    projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (HasNoTarget)
            {
                projectile.penetrate += 1;
                projectile.friendly = false;
                projectile.damage = 0;
                Target = target;
                TargetType = 0;
                projectile.netUpdate = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if(HasNoTarget)
            {
                projectile.penetrate += 1;
                projectile.friendly = false;
                projectile.damage = 0;
                Target = target;
                TargetType = 1;
                projectile.netUpdate = true;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(TargetType);
            writer.Write(Target.whoAmI);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (reader.ReadInt32() == 0)
                Target = Main.npc[reader.ReadInt32()];
            else if (reader.ReadInt32() == 1)
                Target = Main.player[reader.ReadInt32()];
            else
                Target = null;
        }



        public Entity Target { get; private set; }
        public int TargetType { get; private set; } // -1 - Not a valid target, 0 - NPC, 1 - Player

        public bool HasNoTarget => Target == null && TargetType == -1; 

        public bool HasTouchedGround { get; private set; }

        public float YOffset { get; private set; }
    }
}
