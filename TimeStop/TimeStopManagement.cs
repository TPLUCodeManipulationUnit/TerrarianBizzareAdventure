using System;
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
        private static List<int> _npcs;

        internal static Dictionary<NPC, NPCInstantState> npcStates = new Dictionary<NPC, NPCInstantState>();
        internal static Dictionary<Projectile, ProjectileInstantState> projectileStates = new Dictionary<Projectile, ProjectileInstantState>();
        internal static Dictionary<Player, PlayerInstantState> playerStates = new Dictionary<Player, PlayerInstantState>();


        public static void AddNPCImmunity<T>() where T : ModNPC => _npcs.Add(typeof(T).GetModFromType().NPCType<T>());
        public static void AddNPCImmunity(int type) => _npcs.Add(type);


        public static bool IsNPCImmune<T>() where T : ModNPC => typeof(T).IsSubclassOf(typeof(IIsNPCImmuneToTimeStop));
        public static bool IsNPCImmune(int type) => _npcs.Contains(type);


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

                if (!npc.active)
                    continue;

                RegisterStoppedNPC(i, npc);
            }


            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (!projectile.active)
                    continue;

                RegisterStoppedProjectile(i, projectile);
            }


            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];
                

                if (!player.active || player.name == "")
                    continue;

                TBAPlayer tbaPlayer = TBAPlayer.Get(player);

                if (tbaPlayer == modPlayer || tbaPlayer.IsImmuneToTimeStop())
                    continue;

                RegisterStoppedPlayer(i, player);
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


        public static void RegisterStoppedNPC(int npcId) => RegisterStoppedNPC(npcId, Main.npc[npcId]);
        public static void RegisterStoppedNPC(NPC npc) => RegisterStoppedNPC(Array.IndexOf(Main.npc, npc), npc);

        public static void RegisterStoppedNPC(int npcId, NPC npc) => npcStates.Add(npc, NPCInstantState.FromNPC(npcId, npc));


        public static void RegisterStoppedProjectile(int projectileId) => RegisterStoppedProjectile(projectileId, Main.projectile[projectileId]);
        public static void RegisterStoppedProjectile(Projectile projectile) => RegisterStoppedProjectile(Array.IndexOf(Main.projectile, projectile), projectile);

        public static void RegisterStoppedProjectile(int projectileId, Projectile projectile) => projectileStates.Add(projectile, ProjectileInstantState.FromProjectile(projectileId, projectile));


        public static void RegisterStoppedPlayer(int playerId) => RegisterStoppedPlayer(playerId, Main.player[playerId]);
        public static void RegisterStoppedPlayer(Player player) => RegisterStoppedPlayer(Array.IndexOf(Main.player, player), player);

        public static void RegisterStoppedPlayer(int playerId, Player player) => playerStates.Add(player, PlayerInstantState.FromPlayer(playerId, player));


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
        }

        internal static void Unload()
        {
            _npcs = null;
        }


        public static int TimeStoppedFor { get; set; }
        public static bool TimeStopped => TimeStoppedFor > 0;
        public static ModPlayer TimeStopper { get; set; }

        public static double MainTime { get; internal set; }
        public static int MainRainTimer { get; internal set; }
    }
}