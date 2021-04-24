using System;
using System.IO;
using System.Text;
#pragma warning disable CA1305
namespace EnderCode.osuBackupAndRestore
{

    /// <summary>
    /// Beállítások statikus osztálya
    /// </summary>
    static class AppData
    {
        internal static string installPath = Environment.ExpandEnvironmentVariables(@"%userprofile%\AppData\Local\osu!");
        internal static string settingsFile = $@"{Environment.CurrentDirectory}\settings.obr";
        internal static string[] lastRunContent = { "backup", DateTime.MinValue.ToString(System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat), null, "eng", "" };
        internal static string backupDir;
        internal static bool stay = true, qln = false, debug;
        internal const string debugMsg = "DEBUG MODE";
        internal static bool isEng = true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline", Justification = "<Pending>")]
        static AppData()
        {
#if DEBUG
            debug = true;
#else
            debug = false;
#endif
        }

        /// <summary>
        /// Beállításokat menti settings.obr fájlba
        /// </summary>
        /// <remarks><b>Megjegyés:</b><br/>A fájl nem struktúrált, nem formázott csak sorba vannak az opciók (később lehet JSON-ra váltok)</remarks>
        /// <param name="isUpdate">Történt-e új esemény</param>
        /// <param name="isBackup">Mentés történt-e utoljára</param>
        internal static void SettingsSaver(bool isUpdate, bool isBackup)
        {
            using StreamWriter outFile = new StreamWriter(settingsFile, false, Encoding.UTF8);
            outFile.WriteLine(isUpdate ? lastRunContent[0] : (isBackup ? "backup" : "restore"));
            outFile.WriteLine(isUpdate ? lastRunContent[1] : DateTime.Now.ToString());
            outFile.WriteLine(backupDir);
            outFile.WriteLine(isEng ? "eng" : "hun");
            outFile.WriteLine(installPath);
        }

        /// <summary>
        /// Beállítások beolvasása
        /// </summary>
        /// <param name="exist">Létezik-e a fájl</param>
        internal static void LastRunReader(out bool exist)
        {
            exist = false;
            if (File.Exists(settingsFile))
            {
                exist = true;
                using (StreamReader file = new StreamReader(settingsFile, Encoding.UTF8))
                {
                    for (int i = 0; !file.EndOfStream; i++)
                    {
                        lastRunContent[i] = file.ReadLine();
                    }
                }
                backupDir = lastRunContent[2];
                installPath = lastRunContent[4];
            }
            else
            {
                backupDir = string.Empty;
            }
        }
    }
}
