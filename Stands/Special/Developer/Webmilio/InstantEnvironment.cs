﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CSharp;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.Utilities;
using TerrarianBizzareAdventure.Network;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.UserInterfaces;

namespace TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio
{
    public sealed class InstantEnvironment
    {
        private static PropertyInfo _getModTModFile;
        private readonly CompilerParameters _compilerParameters;

        public InstantEnvironment()
        {
            //Provider = new CSharpCodeProvider(new ConcurrentDictionary<string, string>(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("-langversion", "7.3") }));
            Provider = new CSharpCodeProvider();

            _getModTModFile = typeof(Mod).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).First();

            _compilerParameters = new CompilerParameters()
            {
                GenerateInMemory = true,
                IncludeDebugInformation = false,
                GenerateExecutable = false
            };


            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic || string.IsNullOrWhiteSpace(assembly.Location))
                    continue;

                _compilerParameters.ReferencedAssemblies.Add(assembly.Location);
            }

            foreach (Mod mod in ModLoader.Mods)
            {
                if (mod.Name == "ModCompile" || mod.Code == null)
                    continue;

                string path = Path.Combine(Main.SavePath, "Mods", "Cache", "TBADLLS");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);


                path = Path.Combine(path, mod.Name + ".dll");
                File.WriteAllBytes(path, GetModBytes(mod));

                _compilerParameters.ReferencedAssemblies.Add(path);
            }
        }


        public bool CompileSource(bool local, params string[] sources)
        {
            CompilerResults result = Provider.CompileAssemblyFromSource(_compilerParameters, sources);

            if (result.Errors.Count > 0)
                return false;

            try
            {
                List<InstantlyRunnable> compiled = new List<InstantlyRunnable>();

                foreach (TypeInfo type in result.CompiledAssembly.DefinedTypes.Where(t => t.IsSubclassOf(typeof(InstantlyRunnable))))
                    compiled.Add(Activator.CreateInstance(type) as InstantlyRunnable);

                InstantlyRunnables = compiled;

                if (local && Main.netMode == NetmodeID.MultiplayerClient)
                    NetworkPacketManager.Instance.CompileAssembly.SendPacketToAllClients(Main.myPlayer, Main.myPlayer, string.Join("\0", sources));

                UIManager.RATMState.GenerateButtons(this, InstantlyRunnables);

                return true;
            }
            catch
            {
                return false;
                //tbaPlayer.player.KillMe(PlayerDeathReason.ByCustomReason(tbaPlayer.player.name + "'s Stand encountered an error."), 0, 0);
            }
        }

        public bool CompileAssembly(string root, bool local = true)
        {
            List<string> sources = new List<string>();

            foreach (string path in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
                sources.Add(File.ReadAllText(path));

            return CompileSource(local, sources.ToArray());
        }

        public void Select(InstantlyRunnable instantlyRunnable) => Selected = instantlyRunnable;

        public bool Run(TBAPlayer tbaPlayer, bool local = true) => Run(Selected, tbaPlayer, local);
        public bool Run(InstantlyRunnable instantlyRunnable, TBAPlayer tbaPlayer, bool local = true)
        {
            if (instantlyRunnable == null)
                return false;

            LastRan = instantlyRunnable;

            try
            {
                if (!LastRan.Run(tbaPlayer))
                    return false;
            }
            catch
            {
                return false;
            }

            if (local && Main.netMode == NetmodeID.MultiplayerClient)
                NetworkPacketManager.Instance.InstantlyRunnableRan.SendPacketToAllClients(Main.myPlayer, Main.myPlayer, instantlyRunnable.GetType().ToString());

            return true;
        }


        // Code below taken from tModLoader.

        private byte[] GetModBytes(Mod mod) => GetModAssembly(_getModTModFile.GetValue(mod) as TmodFile);


        private static string GetModAssemblyFileName(TmodFile modFile, bool? xna = null)
        {
            string variant = modFile.HasFile($"{modFile.name}.All.dll") ? "All" : (xna ?? PlatformUtilities.IsXNA) ? "XNA" : "FNA";
            string fileName = $"{modFile.name}.{variant}.dll";

            if (!modFile.HasFile(fileName)) // legacy compatibility
                fileName = modFile.HasFile("All.dll") ? "All.dll" : (xna ?? FrameworkVersion.Framework == Framework.NetFramework) ? "Windows.dll" : "Mono.dll";

            return fileName;
        }

        internal static byte[] GetModAssembly(TmodFile modFile, bool? xna = null) => modFile.GetBytes(GetModAssemblyFileName(modFile, xna));


        public CodeDomProvider Provider { get; }

        public InstantlyRunnable Selected { get; private set; }
        public InstantlyRunnable LastRan { get; private set; }

        public List<InstantlyRunnable> InstantlyRunnables { get; private set; }
    }
}