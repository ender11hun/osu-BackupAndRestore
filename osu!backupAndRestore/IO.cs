using System;
using System.Text;
using System.IO;

namespace EnderCode.osu_backupAndRestore
{
    static class IO
    {
        internal static void SettingsSaver(bool isBackup, bool isUpdate, MainData data)
        {
            using (StreamWriter outFile = new StreamWriter(data.lastRunInfo, false, Encoding.UTF8))
            {
                outFile.WriteLine(isUpdate ? data.lastRunContent[0] : (isBackup ? "backup" : "restore"));
                outFile.WriteLine(isUpdate ? data.lastRunContent[1] : DateTime.Now.ToString());
                outFile.WriteLine(data.backupDir);
                outFile.WriteLine(data.isEng ? "eng" : "hun");
                outFile.WriteLine(data.installPath);
            }
        }
        internal static void LastRunReader(out bool exist, MainData data)
        {
            exist = false;
            if (File.Exists(data.lastRunInfo))
            {
                exist = true;
                using (StreamReader file = new StreamReader(data.lastRunInfo, Encoding.UTF8))
                {
                    for (int i = 0; !file.EndOfStream; i++)
                    {
                        data.lastRunContent[i] = file.ReadLine();
                    }
                }
                data.backupDir = data.lastRunContent[2];
                data.installPath = data.lastRunContent[4];
            }
            else
            {
                data.backupDir = string.Empty;
            }
        }
    }
}
