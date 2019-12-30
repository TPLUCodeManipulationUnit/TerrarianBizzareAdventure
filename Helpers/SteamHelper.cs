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

        #region Knife Gang members

        public static List<KnifeGang> KnifeGangMembers
            => new List<KnifeGang>
            ()
            {
                new KnifeGang(76561198242290004, "Belarhos", 182295149685112832),
                new KnifeGang(76561198213553913, "Phobostar", 297422963500908544),
                new KnifeGang(76561198187030773, "Trashy", 294216125485547522),
                new KnifeGang(76561198302058112, "coolHaunterMan", 435803344557047810),
                new KnifeGang(76561198357025580, "DiabeticSquirtle", 353727606190768138),
                new KnifeGang(76561198229295712, "Dracovish", 234168644551049226),
                new KnifeGang(76561199014845586, "ConsumeMoment", 334493152217923585),
                new KnifeGang(76561198114280627, "derek", 143813009662410752),
                new KnifeGang(76561198289387534, "Goob", 233669259983585290),
                new KnifeGang(76561198095073401, "Sadbart", 232269063831814144),
                new KnifeGang(76561198399064104, "MistahPatrick", 660917304346935326),
                new KnifeGang(76561198324585006, "paparaulll", 256789521226530817),
                new KnifeGang(76561198135938508, "Duwangman", 237219489652015104),
                new KnifeGang(76561198274577162, "Skipping", 450018452103757835),
                new KnifeGang(76561198058214960, "LuchioAuditore", 332988883870679040),
                new KnifeGang(76561198138557670, "bastianbearxd", 337276268162842634),
                new KnifeGang(76561198094362255, "VenomPowered", 495679447970611248),
                new KnifeGang(76561198375347234, "Derp", 543225251517956096),
                new KnifeGang(76561197989661904, "Jimmy", 305147770283491340),
                new KnifeGang(76561198018564956, "RobertJ100", 338468090251837440),
                new KnifeGang(76561198286561466, "BrokenWyvern", 339868033709375489),
                new KnifeGang(76561198400289812, "SpyOfDeath",360208062058987522),
                new KnifeGang(76561198124306783, "Mike2000160", 223252569634504706),
                new KnifeGang(76561198202745322, "MiloJopus", 293425751875911681),
                new KnifeGang(76561198109842572, "DayOrk",262922175365775360),
				new KnifeGang(76561198166683164, "TheSilverGhost", 173933732774805505)
            };



        #endregion
    }
}
