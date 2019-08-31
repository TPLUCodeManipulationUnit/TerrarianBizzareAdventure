﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Extensions;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Network;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.TimeStop
{
    public sealed class TimeStopManagement
    {
        private static List<int> _npcs = new List<int>();
        private static List<int> _items = new List<int>();

        internal static Dictionary<NPC, NPCInstantState> npcStates;
        internal static Dictionary<Projectile, ProjectileInstantState> projectileStates;
        internal static Dictionary<Player, PlayerInstantState> playerStates;
        internal static Dictionary<Item, ItemInstantState> itemStates;

        #region Immunity Registration

        public static void AddNPCImmunity<T>() where T : ModNPC => _npcs.Add(typeof(T).GetModFromType().NPCType<T>());
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


        public static void AddItemImmunity<T>() where T : ModItem => AddItemImmunity(typeof(T).GetModFromType().ItemType<T>());
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


        public static void ToggleTimeStopIfStopper(ModPlayer modPlayer)
        {
            if (TimeStopper != null && TimeStopper != modPlayer)
                return;

            if (TimeStopped)
                TryResumeTime(modPlayer);
            else
                TryStopTime(modPlayer, int.MaxValue); // TODO Change this
        }


        public static bool TryStopTime(ModPlayer modPlayer, int duration, bool local = true)
        {
            if (TimeStopped)
                return false;

            if (local)
                TimeStateChanged(modPlayer, duration, true);

            StopTime(modPlayer, duration);

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


            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (!projectile.active || projectile.modProjectile is IProjectileHasImmunityToTimeStop phitts && phitts.IsImmuneToTimeStop(projectile))
                    continue;

                RegisterStoppedProjectile(projectile);
            }


            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];
                
                if (!player.active || player.name == "")
                    continue;

                TBAPlayer tbaPlayer = TBAPlayer.Get(player);

                if (tbaPlayer == modPlayer || tbaPlayer.IsImmuneToTimeStop())
                    continue;

                RegisterStoppedPlayer(player);
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

            projectileStates.Clear(); ;


            foreach (KeyValuePair<Player, PlayerInstantState> state in playerStates)
                state.Value.Restore();

            playerStates.Clear(); ;


            foreach (KeyValuePair<Item, ItemInstantState> state in itemStates)
                state.Value.Restore();

            itemStates.Clear();


            Main.blockInput = false;


            TimeStoppedFor = 0;
            TimeStopper = null;
        }


        private static void TimeStateChanged(ModPlayer modPlayer, int duration, bool stopped)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetworkPacketManager.Instance.TimeStateChanged.SendPacketToAllClients(modPlayer.player.whoAmI, modPlayer.player.whoAmI, stopped, duration);
        }


        public static void OnPlayerEnterWorld(Player player)
        {
            if (player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.MultiplayerClient || Main.dedServ)
                return;

            TimeStopper = null;
            TimeStoppedFor = 0;
        }


        public static void RegisterStoppedNPC(NPC npc) => npcStates.Add(npc, new NPCInstantState(npc));
        public static void RegisterStoppedProjectile(Projectile projectile) => projectileStates.Add(projectile, new ProjectileInstantState(projectile));
        public static void RegisterStoppedPlayer(Player player) => playerStates.Add(player, new PlayerInstantState(player));
        public static void RegisterStoppedItem(Item item) => itemStates.Add(item, new ItemInstantState(item));


        internal static void OnGameTick(ModWorld modWorld)
        {
            int previousTimeStoppedFor = TimeStoppedFor;

            if (TimeStoppedFor > 0)
                TimeStoppedFor--;

            if (TimeStoppedFor == 0 && TimeStoppedFor != previousTimeStoppedFor)
                TryResumeTime(TimeStopper);


            if (TimeStopped)
            {
                TBAPlayer tbaPlayer = TBAPlayer.Get(Main.LocalPlayer);
                Main.blockInput = !(tbaPlayer.StandUser && (tbaPlayer.Stand.IsImmuneToTimeStop(tbaPlayer) || TimeStopper == tbaPlayer));

                Main.dayRate = 0;
                Main.time = TimeStopManagement.MainTime;

                Main.rainTime = TimeStopManagement.MainRainTimer;
            }
        }


        internal static void Load(TBAMod tbaMod)
        {
            _npcs = new List<int>();
            _items = new List<int>();

            npcStates = new Dictionary<NPC, NPCInstantState>();
            projectileStates = new Dictionary<Projectile, ProjectileInstantState>();
            playerStates = new Dictionary<Player, PlayerInstantState>();
            itemStates = new Dictionary<Item, ItemInstantState>();

            AddItemImmunity(ItemID.GravityGlobe);
        }

        internal static void Unload()
        {
            _npcs = null;
            _items = null;

            npcStates = null;
            projectileStates = null;
            playerStates = null;
            itemStates = null;
        }


        public static int TimeStoppedFor { get; set; }
        public static bool TimeStopped => TimeStoppedFor > 0;
        public static ModPlayer TimeStopper { get; set; }

        public static double MainTime { get; internal set; }
        public static int MainRainTimer { get; internal set; }
    }
}