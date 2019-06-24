using System;

namespace osu_backupAndRestore
{
    static class Questions
    {
        //public static bool GoBack()
        //{
        //    ConsoleKey a;
        //    Console.Write("Want to go back? [(Y)es/(N)o]: ");
        //    do
        //    {
        //        a = Console.ReadKey().Key;
        //    } while (!(a.Equals(ConsoleKey.Y) || a.Equals(ConsoleKey.N)));
        //    Console.WriteLine();
        //    return a.Equals(ConsoleKey.Y) ? true : false;
        //}
        public static bool WantLaunch()
        {
            ConsoleKey a;
            Console.Write(MainEntry.langDict[UIElements.QuestionLaunch]+": ");
            do
            {
                a = Console.ReadKey().Key;
            } while (!(a.Equals(MainEntry.data.isEng ? ConsoleKey.Y : ConsoleKey.I) || a.Equals(ConsoleKey.N)));
            Console.WriteLine();
            return a.Equals(MainEntry.data.isEng ? ConsoleKey.Y : ConsoleKey.I) ? true : false;
        }
        public static void AreYouSure()
        {
            ConsoleKey a;
            do
            {
                Console.Write(MainEntry.langDict[UIElements.QuestionSure]+":");
                Console.SetCursorPosition(23, Console.CursorTop);
                a = Console.ReadKey().Key;
            } while (!(a.Equals(MainEntry.data.isEng ?ConsoleKey.Y:ConsoleKey.I) || a.Equals(ConsoleKey.N)));
            Console.WriteLine();
            if (a.Equals(MainEntry.data.isEng ?ConsoleKey.Y:ConsoleKey.I))
            {
                Operations.Repair();
            }
            else
            {
                Utils.WriteColoredLine("Aborted", ConsoleColor.Red);
            }
        }
    }
}
