using System;
using System.Text;
using System.Timers;
using System.Globalization;
using System.Collections.Generic;

namespace osu_backupAndRestore
{
    static class Utils
    {
        public static string EnvExpand(string path)
        {
            return Environment.ExpandEnvironmentVariables(path);
        }
        public static string SizeSuffixer(long bytes)
        {
            if (((bytes / 1024f) / 1024f) >= 1)
            {
                return $"{double.Parse(string.Format("{0:0.0##}", Math.Round((bytes / 1024f) / 1024f), 3), CultureInfo.InvariantCulture)}MB";
            }
            return $"{double.Parse(string.Format("{0:0.0##}", Math.Round(bytes / 1024f, 3)), CultureInfo.InvariantCulture)}KB";
        }
        public static void WriteColored(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
        public static void WriteColoredLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
            Console.WriteLine();
        }
        public static string Beautify(this string unformated)
        {
            return unformated.Replace("\\n", Environment.NewLine);
        }
    }
    
}
