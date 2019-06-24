using System;
using System.Text;
using System.IO;

namespace osu_backupAndRestore
{
    static class IO
    {
        public static void SettingsSaver(bool isBackup, bool isUpdate, ref MainData data)
        {
            using (StreamWriter outFile = new StreamWriter(data.lastRunInfo, false, Encoding.UTF8))
            {
                outFile.WriteLine(isUpdate ? data.lastRunContent[0] : (isBackup ? "backup" : "restore"));
                outFile.WriteLine(isUpdate ? data.lastRunContent[1] : DateTime.Now.ToString());
                outFile.WriteLine(data.backupDir);
                outFile.WriteLine(data.isEng);
            }
        }
        public static void LastRunReader(out bool exist, ref MainData data)
        {
            exist = false;
            if (File.Exists(data.lastRunInfo))
            {
                int index = 0;
                exist = true;
                using (StreamReader file = new StreamReader(data.lastRunInfo, Encoding.UTF8))
                {
                    while (!file.EndOfStream)
                    {
                        data.lastRunContent[index] = file.ReadLine();
                        index++;
                    }
                }
                data.backupDir = data.lastRunContent[2];
            }
            else
            {
                data.backupDir = string.Empty;
            }
        }
    }
}
