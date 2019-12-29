using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Commons.Users;

// ReSharper disable IdentifierTypo
namespace TerrarianBizzareAdventure.Helpers
{
    public static class SteamHelper
    {
        private static bool _initialized = false;


        private static readonly List<Developer> _activeDevelopers = new List<Developer>()
        {
            Webmilio
        };

        private static readonly List<Donator> _activeDonators = new List<Donator>()
        {
        };


        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                string unparsedSteamID64 = typeof(ModLoader).GetProperty("SteamID64", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null).ToString();

                if (!string.IsNullOrWhiteSpace(unparsedSteamID64))
                {
                    HasSteamId64 = true;
                    SteamId64 = long.Parse(unparsedSteamID64);
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("Unable to fetch SteamID, assuming no steam is present.");
            }

            if (!HasSteamId64) return;

            foreach (Donator donator in _activeDonators)
                if (donator.SteamId64 == SteamId64)
                {
                    CurrentUser = donator;

                    donator.IsCurrentUser = true;

                    CurrentDonator = donator;
                    IsDonator = true;

                    break;
                }

            foreach (Developer developer in _activeDevelopers)
                if (developer.SteamId64 == SteamId64)
                {
                    CurrentUser = developer;

                    developer.IsCurrentUser = true;

                    CurrentDeveloper = developer;
                    IsDeveloper = true;

                    break;
                }
        }

        public static bool CanAccess() => CurrentUser != null;
        public static bool CanAccess(User user) => CanAccess(user, false);

        public static bool CanAccess(User user, bool ignoreDevelopers) => !ignoreDevelopers && IsDeveloper || CanAccess(user);

        public static bool CanAccess(bool ignoreDevelopers, bool donators) => ignoreDevelopers && IsDeveloper || donators && IsDonator;

        public static bool CanAccess(params User[] users) => CanAccess(false, users);

        public static bool CanAccess(bool ignoreDevelopers, params User[] users)
        {
            if (!ignoreDevelopers && IsDeveloper) return true;

            for (int i = 0; i < users.Length; i++)
                if (users[i] == CurrentUser)
                    return true;

            return false;
        }


        public static bool HasSteamId64 { get; private set; }
        public static long SteamId64 { get; private set; }

        public static User CurrentUser { get; private set; }

        public static Developer CurrentDeveloper { get; private set; }
        public static bool IsDeveloper { get; private set; }

        public static Donator CurrentDonator { get; private set; }
        public static bool IsDonator { get; private set; }

        #region Developers

        public static Developer Webmilio => new Developer(76561198046878487, "webmilio", 247893661990387713);

        #endregion


        #region Donators

        public static Donator TheSilverGhost => new Donator(76561198166683164, "TheSilverGhost", 173933732774805505);

        #endregion
    }
}
