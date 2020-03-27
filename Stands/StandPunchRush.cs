using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Projectiles;

namespace TerrarianBizzareAdventure.Stands
{
    public struct StandPunchRush
    {
        public StandPunchRush(int punchID, int altPunchID)
        {
            PunchID = punchID;
            AltPunchID = altPunchID;
        }
        /// <summary>
        /// Does rush
        /// </summary>
        /// <param name="stand">projectile</param>
        /// <param name="rushDirection">rush direction</param>
        /// <param name="damage"></param>
        /// <param name="rushTimer"></param>
        /// <param name="offset">offset from stand's center</param>
        /// <param name="range1">Y randomness range for back fist</param>
        /// <param name="range2">Y random range for front fist</param>
        public void DoRush(Projectile stand, Vector2 rushDirection, int damage, int rushTimer, Vector2 offset = new Vector2(), Vector2 range1 = new Vector2(), Vector2 range2 = new Vector2())
        {
            if (range1 == Vector2.Zero)
                range1 = new Vector2(5, 10);

            if (range2 == Vector2.Zero)
                range2 = new Vector2(10, 20);

            Vector2 off1 = offset + new Vector2(0, Main.rand.NextFloat(range1.X, range1.Y) * (Main.rand.NextBool() ? 1 : -1));
            Vector2 off2 = offset + new Vector2(0, Main.rand.NextFloat(range2.X, range2.Y) * (Main.rand.NextBool() ? 1 : -1));

            if (rushTimer > 1)
            {
                float randRot = 0.5f;
                int projBack = Projectile.NewProjectile(stand.Center + offset, rushDirection.RotatedByRandom(randRot), AltPunchID, damage, 3.5f, stand.owner);

                RushPunch rushBack = Main.projectile[projBack].modProjectile as RushPunch;
                rushBack.ParentProjectile = stand.whoAmI;
                rushBack.Offset = off1;

                int projFront = Projectile.NewProjectile(stand.Center + offset, rushDirection.RotatedByRandom(randRot), PunchID, damage, 3.5f, stand.owner);

                RushPunch rushFront = Main.projectile[projFront].modProjectile as RushPunch;
                rushFront.ParentProjectile = stand.whoAmI;
                rushFront.Offset = off2;
            }
            else
            {
                int projFront = Projectile.NewProjectile(stand.Center + offset, rushDirection.RotatedByRandom(.6f), PunchID, damage, 3.5f, stand.owner);

                RushPunch rushFront = Main.projectile[projFront].modProjectile as RushPunch;
                rushFront.ParentProjectile = stand.whoAmI;
                rushFront.IsFinalPunch = true;
                rushFront.Offset = offset;
            }
        }

        public int PunchID { get; }

        public int AltPunchID { get; }
    }
}
