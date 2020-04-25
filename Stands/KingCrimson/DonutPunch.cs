using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands.KingCrimson;

namespace TerrarianBizzareAdventure.Projectiles
{
    public class DonutPunch : ModProjectile
    {
        public sealed override string Texture => "TerrarianBizzareAdventure/Textures/EmptyPixel";

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 48;

            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 8;

            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.ai[1] = -1;
        }

        public override void AI()
        {
            projectile.netUpdate = true;
            projectile.netUpdate2 = true;

            var dir = Main.player[projectile.owner].direction;

            Vector2 offset = new Vector2(36 * dir, projectile.ai[1] > -1 ? -8 : 0);

            projectile.Center = ParentProjectile.Center + offset;

            if (DonutTarget != null)
            {
                for (int i = 0; i < 3; i++)
                    Dust.NewDust(projectile.Center - new Vector2(dir == 1 ? 8 : 0, 0), 0, 0, DustID.Blood, 8 * Main.player[projectile.owner].direction, -2, 0, default(Color), 1.5f);

                if (DonutType == TargetType.NPC)
                {
                    NPC tryGetNpc = DonutTarget as NPC;

                    bool shouldNotPull = tryGetNpc.type == NPCID.WallofFlesh || tryGetNpc.type == NPCID.WallofFleshEye;

                    if (!shouldNotPull)
                        DonutTarget.Center = projectile.Center;

                    if (shouldNotPull)
                        projectile.Center = DonutTarget.Center;
                }

                if (DonutType == TargetType.Player)
                {
                    DonutTarget.velocity = Vector2.Zero;
                    DonutTarget.Center = projectile.Center;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            if(DonutType != TargetType.None)
                writer.Write((byte)DonutType);

            if(DonutTarget != null)
                writer.Write(DonutTarget.whoAmI);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DonutType = (TargetType)reader.ReadByte();

            if (DonutType == TargetType.NPC)
                DonutTarget = Main.npc[reader.ReadInt32()];

            if (DonutType == TargetType.Player)
                DonutTarget = Main.player[reader.ReadInt32()];
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);

            KingCrimson stando = ParentProjectile.modProjectile as KingCrimson;

            if (stando != null)
            {
                projectile.timeLeft = stando.HasMissedDonut ? 75 : projectile.timeLeft;

                stando.HasMissedDonut = false;
            }

            projectile.damage = 600;

            if (target.life - damage > 0)
            {
                DonutTarget = target;
                DonutType = TargetType.NPC;
                projectile.ai[1] = (int)target.whoAmI;
            }
            else
            {
                projectile.Kill();
            }

            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Punch" + Main.rand.Next(1, 5)).WithVolume(.2f));
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            base.ModifyHitPvp(target, ref damage, ref crit);

            KingCrimson stando = ParentProjectile.modProjectile as KingCrimson;

            if (stando != null)
            {
                projectile.timeLeft = stando.HasMissedDonut ? 75 : projectile.timeLeft;

                stando.HasMissedDonut = false;
            }

            projectile.damage = 600;

            if (target.statLife - damage > 0)
            {
                DonutTarget = target;
                DonutType = TargetType.Player;
                projectile.ai[1] = (int)target.whoAmI;
            }
            else
            {
                projectile.Kill();
            }

            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Punch" + Main.rand.Next(1, 5)).WithVolume(.2f));
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.ai[1] == -5)
                return false;

            if (projectile.ai[1] == -1)
                return true;

            if (target.whoAmI == (int)projectile.ai[1] && projectile.timeLeft <= 2)
                return true;

            return false;
        }

        public override bool CanHitPvp(Player target)
        {
            if (projectile.ai[1] == -5)
                return false;

            if (projectile.ai[1] == -1)
                return true;

            if (target.whoAmI == (int)projectile.ai[1] && projectile.timeLeft <= 2)
                return true;

            return false;
        }

        private Projectile ParentProjectile => Main.projectile[(int)projectile.ai[0]];

        public Entity DonutTarget { get; private set; }

        public TargetType DonutType { get; private set; } 
    }

    public enum TargetType : byte
    {
        None,
        Player,
        NPC
    }
}
