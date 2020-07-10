// This class lib is not only for the osu! backup tool

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
#pragma warning disable CS1998

namespace EnderCode.Utils
{
    public static class Util
    {
        private static string LogFile
        {
            get
            {
                return $"<name>-{DateTime.Today.Year}{DateTime.Today.Month}{DateTime.Today.Day}{DateTime.Today.Hour}{DateTime.Today.Minute}{DateTime.Today.Second}.log";
            }
        }
        public static string GetVersion
        {
            get
            {
                return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            }
        }
        public static string SizeSuffixer(long bytes)
        {
            if (((bytes / 1024f) / 1024f) >= 1)
            {
                return $"{double.Parse(string.Format("{0:0.0##}", Math.Round((bytes / 1024f) / 1024f), 3), CultureInfo.InvariantCulture)}MB";
            }
            return $"{double.Parse(string.Format("{0:0.0##}", Math.Round(bytes / 1024f, 3)), CultureInfo.InvariantCulture)}KB";
        }
        public static void WriteColored(string text, ConsoleColor font)
        {
            Console.ForegroundColor = font;
            Console.Write(text);
            Console.ResetColor();
        }
        public static void WriteColored(string text, ConsoleColor? font, ConsoleColor back)
        {
            Console.BackgroundColor = back;
            WriteColored(text, font.HasValue ? (ConsoleColor)font : Console.ForegroundColor);
        }
        public static void WriteColoredLine(string text, ConsoleColor color)
        {
            WriteColored(text+"\n", color);
        }
        public static void WriteColoredLine(string text, ConsoleColor? font, ConsoleColor back)
        {
            Console.BackgroundColor = back;
            WriteColoredLine(text, font.HasValue ? (ConsoleColor)font : Console.ForegroundColor);
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
                        Console.ForegroundColor = !fontColor.HasValue ? Console.ForegroundColor : (ConsoleColor)fontColor;
                    }
                    if (text[i + 1] == 'b')
                    {
                        Console.BackgroundColor = !backColor.HasValue ? Console.BackgroundColor : (ConsoleColor)backColor;
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
                    if (Interop.ShowWindow(hwnd, ShowWindowEnum.Restore))
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
            Interop.ShowWindow(hwnd, hide ? ShowWindowEnum.Hide : ShowWindowEnum.Restore);
            if (!hide) Interop.SetForegroundWindow(hwnd);
        }
        public static T GetEnum<T>(this string name) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), name);
        }
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
    }
}
