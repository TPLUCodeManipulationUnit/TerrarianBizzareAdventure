using Terraria;

namespace TerrarianBizzareAdventure.Stands
{
    public class HitNPCData
    {
        public HitNPCData(NPC target, int lifeTime = 5)
        {
            HitNPC = target;
            LifeTime = lifeTime;
        }

        public int LifeTime {get; set;}

        public NPC HitNPC { get; }
    }
}
