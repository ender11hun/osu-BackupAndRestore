using EnderCode.Utils;
using System;

namespace EnderCode.osuBackupAndRestore
{
    static class Dialogs
    {
        /// <summary>
        /// Win32 ablak handle
        /// </summary>
        /// <remarks>Szükséges, hogy a konzol ablakot blokkolja a könyvtár böngésző</remarks>
        internal static System.Windows.Forms.IWin32Window Win32ConHandle;
        internal static bool GeneralAskDialog(in UIElements element)
        {
            ConsoleKey a;
            Console.Write("\n" + MainEntry.langDict[element] + ": ");
            do
            {
                a = Console.ReadKey().Key;
            } while (!(a.Equals(AppData.isEng ? ConsoleKey.Y : ConsoleKey.I) || a.Equals(ConsoleKey.N)));
            Console.WriteLine();
            return a.Equals(AppData.isEng ? ConsoleKey.Y : ConsoleKey.I) ? true : false;
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
        internal static string InstallNotFound()
        {
            if (!GeneralAskDialog(UIElements.InstallNotFound))
            {
                Console.WriteLine();
            }
            Console.WriteLine(MainEntry.langDict[UIElements.FolderBrowsing]);
            System.Threading.Thread.Sleep(2000);
            string path = string.Empty;
            using (var folderDialog = FormImpl4Con.CreateFolderBrowser(MainEntry.langDict[UIElements.BrowseFolder]))
            {
                var dialogResult = folderDialog.ShowDialog(Win32ConHandle);
                if (dialogResult == System.Windows.Forms.DialogResult.Abort || dialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    Console.WriteLine(MainEntry.langDict[UIElements.BrowseAbort]);
                    Console.WriteLine(MainEntry.langDict[UIElements.AwaitKeyToast]);
                    Console.ReadKey();
                    return null;
                }
                Console.Clear();
                path = folderDialog.SelectedPath;
            }
            return path;
        }
    }
}
