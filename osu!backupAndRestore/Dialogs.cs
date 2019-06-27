using System;

namespace osu_backupAndRestore
{
    enum DialogMode
    {
        Launch,
        Repair,
        Delete
    }

    static class Dialogs
    {
        private static readonly System.Collections.Generic.Dictionary<DialogMode, UIElements> questionLookUp = new System.Collections.Generic.Dictionary<DialogMode, UIElements>()
        {
            { DialogMode.Launch, UIElements.QuestionLaunch },
            { DialogMode.Repair, UIElements.QuestionSure },
            { DialogMode.Delete, UIElements.QuestionDelete }
        };
        public static bool GeneralAskDialog(DialogMode mode)
        {
            ConsoleKey a;
            Console.Write(MainEntry.langDict[questionLookUp[mode]] + ": ");
            do
            {
                a = Console.ReadKey().Key;
            } while (!(a.Equals(MainEntry.data.isEng ? ConsoleKey.Y : ConsoleKey.I) || a.Equals(ConsoleKey.N)));
            Console.WriteLine();
            return a.Equals(MainEntry.data.isEng ? ConsoleKey.Y : ConsoleKey.I) ? true : false;
        }
        public static void AreYouSure()
        {
            bool permited = GeneralAskDialog(DialogMode.Repair);
            Console.WriteLine();
            if (permited)
            {
                Operations.Repair();
            }
            else
            {
                Utils.WriteColoredLine(MainEntry.langDict[UIElements.Aborted], ConsoleColor.Red);
            }
        }
    }
}
