using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Buffs;
using TerrarianBizzareAdventure.Extensions;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.States;

namespace TerrarianBizzareAdventure.TimeStop
{
    public sealed class TimeStopManagement
    {
        private static List<int> _npcs = new List<int>();
        private static List<int> _items = new List<int>();

        internal static Dictionary<NPC, NPCInstantState> npcStates;
        internal static Dictionary<Projectile, ProjectileInstantState> projectileStates;
        internal static Dictionary<Item, ItemInstantState> itemStates;


        #region Immunity Registration

        public static void AddNPCImmunity<T>() where T : ModNPC => _npcs.Add(ModContent.NPCType<T>());
        public static void AddNPCImmunity(int type) => _npcs.Add(type);

        public static bool IsNPCImmune(NPC npc)
        {
            ModNPC modNPC = npc.modNPC;

            if (modNPC == null || modNPC.mod.Name == "CalamityMod")
                return false;

            if (_npcs.Contains(npc.type))
                return true;

            if (modNPC is INPCHasImmunityToTimeStop nhitts && nhitts.IsImmuneToTimeStop(npc))
                return true;

            return false;
        }


        public static void AddItemImmunity<T>() where T : ModItem => AddItemImmunity(ModContent.ItemType<T>());
        public static void AddItemImmunity(int itemType) => _items.Add(itemType);

        public static bool IsImmune(Item item)
        {
            if (_items.Contains(item.type))
                return true;

            if (item.modItem is IItemHasImmunityToTimeStop ihitts && ihitts.IsImmuneToTimeStop(item))
                return true;

            return false;
        }

        #endregion


        public static void ToggleTimeStopIfStopper(ModPlayer modPlayer, int duration)
        {
            if (TimeStopper != null && TimeStopper != modPlayer)
                return;

            if (TimeStopped)
                TryResumeTime(modPlayer);
            else
                TryStopTime(modPlayer, duration); // TODO Change this
        }


        public static bool TryStopTime(ModPlayer modPlayer, int duration, bool local = true)
        {
            if (TimeStopped)
                return false;

            StopTime(modPlayer, duration);

            if (local)
                TimeStateChanged(modPlayer, duration, true);

            return true;
        }

        internal static void StopTime(ModPlayer modPlayer, int duration)
        {
            MainTime = Main.time;
            MainRainTimer = Main.rainTime;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || IsNPCImmune(npc))
                    continue;

                RegisterStoppedNPC(npc);
            }


            for (int i = 0; i < Main.projectile.Length - 1; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (TimeStopImmuneProjectiles.Contains(projectile.type))
                    continue;

                if (!projectile.active || projectile.modProjectile is IProjectileHasImmunityToTimeStop phitts && phitts.IsNativelyImmuneToTimeStop())
                    continue;

                ModProjectile modProjectile = projectile.modProjectile as Stand;

                if (modProjectile != null && modProjectile.projectile.owner == modPlayer.player.whoAmI)
                    continue;

                RegisterStoppedProjectile(projectile);
            }


            for (int i = 0; i < Main.item.Length; i++)
            {
                Item item = Main.item[i];

                if (!item.active || IsImmune(item))
                    continue;

                RegisterStoppedItem(item);
            }


            TimeStopper = modPlayer;
            TimeStoppedFor = duration;
        }


        public static bool TryResumeTime(ModPlayer modPlayer, bool local = true)
        {
            if (!TimeStopped && TimeStopper != modPlayer)
                return false;

            if (local)
                TimeStateChanged(modPlayer, 0, false);

            ResumeTime();

            return true;
        }

        internal static void ResumeTime()
        {
            foreach (KeyValuePair<NPC, NPCInstantState> state in npcStates)
                state.Value.Restore();

            npcStates.Clear();


            foreach (KeyValuePair<Projectile, ProjectileInstantState> state in projectileStates)
                state.Value.Restore();

            projectileStates.Clear();


            foreach (KeyValuePair<Item, ItemInstantState> state in itemStates)
                state.Value.Restore();

            itemStates.Clear();


            Main.blockInput = false;


            TimeStoppedFor = 0;
            TimeStopper.player.AddBuff(ModContent.BuffType<TimeStopCDDebuff>(), 6 * Constants.TICKS_PER_SECOND);
            TimeStopper = null;
        }


        private static void TimeStateChanged(ModPlayer modPlayer, int duration, bool stopped)
        {
            new TimeStateChangedPacket
            {
                Duration = !stopped ? 0 : duration, Stopped = stopped
            }.Send();
        }


        public static void OnPlayerEnterWorld(Player player)
        {
            if (player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.MultiplayerClient || Main.dedServ)
                return;

            TimeStopper = null;
            TimeStoppedFor = 0;
        }


        public static void RegisterStoppedNPC(NPC npc)
        { 
            if (!npcStates.ContainsKey(npc))
                npcStates.Add(npc, new NPCInstantState(npc));
        }
        public static void RegisterStoppedProjectile(Projectile projectile)
        {
            if (!projectileStates.ContainsKey(projectile))
                projectileStates.Add(projectile, new ProjectileInstantState(projectile));
        }
        public static void RegisterStoppedItem(Item item)
        {
            if(!itemStates.ContainsKey(item))
                itemStates.Add(item, new ItemInstantState(item));
        }

        internal static void MainOnOnTick()
        {
            int previousTimeStoppedFor = TimeStoppedFor;

            if (TimeStoppedFor > 0)
                TimeStoppedFor--;

            if (TimeStoppedFor == 0 && TimeStoppedFor != previousTimeStoppedFor)
                TryResumeTime(TimeStopper);


            if (TimeStopped)
            {
                TBAPlayer tbaPlayer = TBAPlayer.Get(Main.LocalPlayer);
                Main.blockInput = !(tbaPlayer.StandUser && (tbaPlayer.Stand.IsNativelyImmuneToTimeStop() || TimeStopper == tbaPlayer));

                Main.dayRate = 0;
                Main.time = TimeStopManagement.MainTime;

                Main.rainTime = TimeStopManagement.MainRainTimer;
            }

            if(!TimeStopped)
            {
                if (itemStates.Count > 0)
                    itemStates.Clear();

                if (npcStates.Count > 0)
                    npcStates.Clear();

                if (projectileStates.Count > 0)
                    projectileStates.Clear();
            }
        }


        internal static void Load()
        {
            //Main.OnTick += MainOnOnTick;

            _npcs = new List<int>();
            _items = new List<int>();

            npcStates = new Dictionary<NPC, NPCInstantState>();
            projectileStates = new Dictionary<Projectile, ProjectileInstantState>();
            itemStates = new Dictionary<Item, ItemInstantState>();

            AddItemImmunity(ItemID.GravityGlobe);
        }

        internal static void Unload()
        {
            //Main.OnTick -= MainOnOnTick;

            _npcs = null;
            _items = null;

            npcStates = null;
            projectileStates = null;
            itemStates = null;
        }


        /*private static void MainOnDoUpdate(On.Terraria.Main.orig_DoUpdate orig, Main self, GameTime gameTime)
        {
            bool executeOriginal = true;

            if (Main.gameMenu)
                executeOriginal = true;
            else if (!Main.gameMenu && Main.LocalPlayer != null && Main.LocalPlayer.active && TBAPlayer.Get(Main.LocalPlayer).IsStopped() && TimeStopped && !Main.dedServ)
            {
                executeOriginal = false;
            }


            if (executeOriginal)
                orig(self, gameTime);
        }*/


        public static int TimeStoppedFor { get; set; }
        public static bool TimeStopped => TimeStoppedFor > 0;
        public static ModPlayer TimeStopper { get; set; }

        public static double MainTime { get; internal set; }
        public static int MainRainTimer { get; internal set; }

        public static List<int> TimeStopImmuneProjectiles = new List<int>();
    }
}