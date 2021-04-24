using System.Reflection;
using System;

namespace EnderCode.osuBackupAndRestore
{
    /// <summary>
    /// <see cref="Assembly"/> "hack" az EXE fájl verizójának visszakéréséhez
    /// </summary>
    public static class CoreAssembly
    {/// <summary>
     /// A tartalmzó <see cref="Assembly"/> hivatkozása
     /// </summary>
        public static readonly Assembly Reference = typeof(CoreAssembly).Assembly;
        /// <summary>
        /// Tartalmazó <see cref="Assembly"/> verziója
        /// </summary>
        public static readonly Version Version = Reference.GetName().Version;
        /// <summary>
        /// Tartalmazó <see cref="Assembly"/> neve
        /// </summary>
        public static readonly AssemblyName Name = typeof(SystemTray).Assembly.GetName();
    }
}