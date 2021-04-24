// This class lib is not only for the osu! backup tool

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static EnderCode.Utils.Interop.ENUMS;
#pragma warning disable CS1998, CS1591

namespace EnderCode.Utils
{
    public static class Util
    {
        /// <summary>
        /// A Log fájl nevét építi fel
        /// </summary>
        private static string LogFile
        {
            get
            {
                return $"<name>-{DateTime.Today.Year}{DateTime.Today.Month}{DateTime.Today.Day}{DateTime.Today.Hour}{DateTime.Today.Minute}{DateTime.Today.Second}.log";
            }
        }

        /// <summary>
        /// Önmagyarázó, a megfelelő suffixel lát el egy méretet
        /// </summary>
        /// <param name="bytes">Méret</param>
        /// <returns>Suffixelt méret <see cref="string"/>ként</returns>
        public static string SizeSuffixer(long bytes)
        {
            if ((bytes / 1048576f) >= 1)
            {
                return $"{double.Parse(string.Format("{0:0.0##}", Math.Round(bytes / 1048576f, 3), CultureInfo.InvariantCulture))}MB";
            }
            return $"{double.Parse(string.Format("{0:0.0##}", Math.Round(bytes / 1024f, 3)), CultureInfo.InvariantCulture)}KB";
        }

        public static void WriteColored(string text, bool newLine, ConsoleColor? font, ConsoleColor? back = null)
        {
            if (back.HasValue)
                Console.BackgroundColor = back.Value;
            if (font.HasValue)
                Console.ForegroundColor = font.Value;
            Console.Write(text);
            Console.ResetColor();
            if (newLine)
                Console.WriteLine();
        }

        public static void WriteColorFormated(string text, ConsoleColor? fontColor, ConsoleColor? backColor)
        {
            int variableTextLength = text.Length;
            for (int i = 0; i < variableTextLength; i++)
            {
                if (text[i] == '%')
                {
                    if (text[i + 1] == 'f')
                    {
                        Console.ForegroundColor = fontColor ?? Console.ForegroundColor ;
                    }
                    if (text[i + 1] == 'b')
                    {
                        Console.BackgroundColor = backColor ?? Console.BackgroundColor ;
                    }
                    text = text.Remove(i, 2);
                    variableTextLength -= 2;
                }
                Console.Write(text[i]);
                Console.ResetColor();
            }
        }

        public static async Task<int> BringWindowToFront(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (Interop.ShowWindow(hwnd, ShowWindowEnum.SW_RESTORE))
                        return 0;
                    Thread.Sleep(600);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                if (Interop.SetForegroundWindow(hwnd) == 0)
                    return 0;
                Thread.Sleep(600);
            }
            return int.MaxValue;
        }

        public static string Beautify(this string unformated)
        {
            return unformated.Replace(@"\n", Environment.NewLine).Replace(@"\t", "\t");
        }

        public static void HideCurrentWindow(bool hide, IntPtr hwnd)
        {
            Interop.ShowWindow(hwnd, hide ? ShowWindowEnum.SW_HIDE : ShowWindowEnum.SW_RESTORE);
            if (!hide) Interop.SetForegroundWindow(hwnd);
        }

        /// <summary>
        /// Generikus "hack" <see cref="string"/>ből <see langword="enum"/> parse
        /// </summary>
        /// <typeparam name="T">Cél típus (<see langword="where"/> T <see langword="extends"/> <see cref="Enum"/>)</typeparam>
        /// <param name="name">a bővített <see cref="string"/> objektum</param>
        /// <returns>T típusú <see cref="Enum"/>ot</returns>
        public static T GetEnum<T>(this string name) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        /// <summary>
        /// Generikus logger
        /// </summary>
        /// <typeparam name="T">Forrás kivétel típusa</typeparam>
        /// <param name="e">Kivétel objektum</param>
        /// <param name="logType">Log típusa</param>
        /// <returns>Az elkészített log fájl nevét tartalmazó <see cref="string"/>et</returns>
        public static string Logger<T>(T e, string logType) where T : Exception
        {
            string[] content = {
                                e.Message,
                                e.Source,
                                e.StackTrace,
                                $"[HRESULT]: {e.HResult} (0x{e.HResult:X})"
                                };
            if (typeof(T).Equals(typeof(System.Runtime.InteropServices.ExternalException)))
                content.Append((e as System.Runtime.InteropServices.ExternalException).ErrorCode.ToString());
            string logname = LogFile.Replace("<name>", logType);
            System.IO.File.WriteAllLines(logname, content);
            return logname;
        }

        /// <summary>
        /// <see cref="Assembly"/> "hack" a DLL fájl verizójának visszakéréséhez
        /// </summary>
        public static class CoreAssembly
        {
            /// <summary>
            /// A jelenlegi <see cref="Assembly"/> hivatkozása
            /// </summary>
            public static readonly Assembly Reference = typeof(CoreAssembly).Assembly;
            /// <summary>
            /// <see cref="Assembly"/> verziója
            /// </summary>
            public static readonly Version Version = Reference.GetName().Version;
        }
    }
}
