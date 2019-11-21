using System;
using EnderCode.Utils;

namespace EnderCode.osu_backupAndRestore
{
    static class Dialogs
    {
        internal static bool GeneralAskDialog(in UIElements element)
        {
            ConsoleKey a;
            Console.Write("\n" + MainEntry.langDict[element] + ": ");
            do
            {
                a = Console.ReadKey().Key;
            } while (!(a.Equals(MainEntry.data.isEng ? ConsoleKey.Y : ConsoleKey.I) || a.Equals(ConsoleKey.N)));
            Console.WriteLine();
            return a.Equals(MainEntry.data.isEng ? ConsoleKey.Y : ConsoleKey.I) ? true : false;
        }
        internal static void AreYouSure()
        {
            bool permited = GeneralAskDialog(UIElements.QuestionSure);
            Console.WriteLine();
            if (permited)
            {
                Operations.Repair();
            }
            else
            {
                Util.WriteColoredLine(MainEntry.langDict[UIElements.Aborted], ConsoleColor.Red);
            }
        }
    }
}
