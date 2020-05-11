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
using TerrarianBizzareAdventure.Helpers;
using Terraria.ID;


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

            if (!HasTouchedGround)
                projectile.timeLeft = 200;


            projectile.velocity = Collision.TileCollision(projectile.position, projectile.velocity, projectile.width, projectile.height);

            if (projectile.velocity.Y == 0)
            {
                if (!HasTouchedGround)
                {
                    projectile.netUpdate = true;
                    projectile.damage = 0;
                }
                HasTouchedGround = true;
            }

            if(!HasNoTarget)
            {
                if (!Target.active)
                    projectile.Kill();
            }
        }
		
		public override bool PreKill(int timeLeft)
		{
			/// For now, copy pasted VFX for road roller. Will make it prettier in the next build
			DrawHelpers.CircleDust(projectile.Center, Vector2.Zero, DustID.Fire, 90, 90, 2.5f, 60);
			
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int i = 0; i < 60; i++)
            {
                int dust = Dust.NewDust(projectile.Center, 0, 0, DustID.Fire, 0, 0);
                Main.dust[dust].velocity *= 16.5f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 3.5f;
            }
            // Large Smoke Gore spawn
            for (int g = 0; g < 2; g++)
            {
                int goreIndex = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), Vector2.Zero, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;

                goreIndex = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), Vector2.Zero, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 3.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 3.5f;

                goreIndex = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), Vector2.Zero, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;

                goreIndex = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), Vector2.Zero, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
            }

			return true;
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

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (HasNoTarget)
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
            if (Target != null)
            {
                writer.Write(TargetType);
                writer.Write(Target.whoAmI);
            }
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
