using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Items
{
    public sealed class TBAInstanciatedGlobalItem : GlobalItem
    {
        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (TimeStopManagement.TimeStopped && !TimeStopManagement.IsImmune(item))
                IsStopped = true;
            else
                IsStopped = false;

            if (IsStopped)
            {
                if (!TimeStopManagement.itemStates.ContainsKey(item))
                    TimeStopManagement.RegisterStoppedItem(item);

                TimeStopManagement.itemStates[item].PreAI(item);
            }
        }


        public override bool OnPickup(Item item, Player player)
        {
            if (IsStopped && TimeStopManagement.itemStates.ContainsKey(item))
                TimeStopManagement.itemStates.Remove(item);

            return base.OnPickup(item, player);
        }

        public override bool CanPickup(Item item, Player player)
        {
            if (IsStopped && TimeStopManagement.TimeStopped)
            {
                TBAPlayer tbaPlayer = TBAPlayer.Get(player);

                if (tbaPlayer.IsImmuneToTimeStop())
                    return true;
                else
                    return false;
            }

            return base.CanPickup(item, player);
        }


        public bool IsStopped { get; private set; }

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
    }
}