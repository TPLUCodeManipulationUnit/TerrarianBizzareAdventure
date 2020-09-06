using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Projectiles;
using TerrarianBizzareAdventure.TimeStop;
using System.Collections.Generic;

namespace TerrarianBizzareAdventure.Items
{
    public sealed class TBAInstanciatedGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if(item.buffType == BuffID.WellFed)
            {
                tooltips.Add(new TooltipLine(TBAMod.Instance, "StaminaGainLine", "Increases Stamina Gain by 1"));
            }
        }

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

        public override bool CanUseItem(Item item, Player player)
        {
            if(item.channel && item.shoot != ProjectileID.None)
            {
                if(!TimeStopManagement.TimeStopImmuneProjectiles.Contains(item.shoot))
                    TimeStopManagement.TimeStopImmuneProjectiles.Add(item.shoot);
            }

            TBAPlayer tPlayer = TBAPlayer.Get(player);

            if (tPlayer.StandActive && tPlayer.ActiveStandProjectile.StopsItemUse)
                return false;

            return base.CanUseItem(item, player);
        }


        public bool IsStopped { get; private set; }

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
    }
}