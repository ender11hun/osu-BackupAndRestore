using System;
using System.Text;
using System.IO;

namespace EnderCode.osu_backupAndRestore
{
    static partial class AppData
    {
        internal static void SettingsSaver(bool isBackup, bool isUpdate)
        {
            using (StreamWriter outFile = new StreamWriter(settingsFile, false, Encoding.UTF8))
            {
                outFile.WriteLine(isUpdate ? lastRunContent[0] : (isBackup ? "backup" : "restore"));
                outFile.WriteLine(isUpdate ? lastRunContent[1] : DateTime.Now.ToString());
                outFile.WriteLine(backupDir);
                outFile.WriteLine(isEng ? "eng" : "hun");
                outFile.WriteLine(installPath);
            }
        }
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
