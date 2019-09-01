using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CSharp;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.Utilities;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio
{
    public sealed class InstantEnvironment
    {
        private static PropertyInfo _getModTModFile;

        private readonly string[] modReferences;
        private readonly CompilerParameters _compilerParameters;


        public InstantEnvironment()
        {
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
                if (assembly.IsDynamic)
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


        public void ExecuteClass(string pathToClass)
        {
            CompilerResults result = Provider.CompileAssemblyFromFile(_compilerParameters, pathToClass);

            (Activator.CreateInstance(result.CompiledAssembly.DefinedTypes.First()) as InstantlyRunnable).Run(TBAPlayer.Get(Main.LocalPlayer));
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
    }
}