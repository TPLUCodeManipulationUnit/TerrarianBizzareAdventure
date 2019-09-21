using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.Utilities;
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

                try
                {
                    File.WriteAllBytes(path, GetModBytes(mod));
                }
                catch (IOException)
                {
                    ; // Do nothing, the file is in use and we'll use it again.
                }

                _compilerParameters.ReferencedAssemblies.Add(path);
            }

            string terrariaHooksDLL = Path.Combine(Main.SavePath, "references", "TerrariaHooks.dll");

            if (File.Exists(terrariaHooksDLL))
                _compilerParameters.ReferencedAssemblies.Add(terrariaHooksDLL);
        }


        public bool CompileSource(bool local, params string[] sources)
        {
            CompilerResults result = Provider.CompileAssemblyFromSource(_compilerParameters, sources);

            if (result.Errors.Count > 0)
                return false;

            try
            {
                if (InstantlyRunnables != null)
                    foreach (InstantlyRunnable previousInstantlyRunnable in InstantlyRunnables)
                        previousInstantlyRunnable.Stop();

                List<InstantlyRunnable> compiled = new List<InstantlyRunnable>();

                foreach (TypeInfo type in result.CompiledAssembly.DefinedTypes.Where(t => t.IsSubclassOf(typeof(InstantlyRunnable))))
                    compiled.Add(Activator.CreateInstance(type) as InstantlyRunnable);

                InstantlyRunnables = compiled;

                if (local && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    //string.Join("\0", sources)
                    new CompileAssemblyPacket().Send();
                }

                UIManager.RATMState.GenerateButtons(this, InstantlyRunnables);

                return true;
            }
            catch (Exception e)
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
            {
                InstantlyRunnableRanPacket packet = new InstantlyRunnableRanPacket();
                packet.StringifiedClass = instantlyRunnable.GetType().ToString();
                packet.Send();
            }

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