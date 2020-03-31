using System;
using System.Text;
using System.IO;

namespace EnderCode.osu_backupAndRestore
{
    static class IO
    {
        internal static void SettingsSaver(bool isBackup, bool isUpdate)
        {
            using (StreamWriter outFile = new StreamWriter(AppData.lastRunInfo, false, Encoding.UTF8))
            {
                outFile.WriteLine(isUpdate ? AppData.lastRunContent[0] : (isBackup ? "backup" : "restore"));
                outFile.WriteLine(isUpdate ? AppData.lastRunContent[1] : DateTime.Now.ToString());
                outFile.WriteLine(AppData.backupDir);
                outFile.WriteLine(AppData.isEng ? "eng" : "hun");
                outFile.WriteLine(AppData.installPath);
            }
        }
        internal static void LastRunReader(out bool exist)
        {
            exist = false;
            if (File.Exists(AppData.lastRunInfo))
            {
                exist = true;
                using (StreamReader file = new StreamReader(AppData.lastRunInfo, Encoding.UTF8))
                {
                    for (int i = 0; !file.EndOfStream; i++)
                    {
                        AppData.lastRunContent[i] = file.ReadLine();
                    }
                }
                AppData.backupDir = AppData.lastRunContent[2];
                AppData.installPath = AppData.lastRunContent[4];
            }
            else
            {
                AppData.backupDir = string.Empty;
            }
        }
    }
}
