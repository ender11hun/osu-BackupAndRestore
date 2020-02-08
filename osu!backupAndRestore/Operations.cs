using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using ThreadState = System.Threading.ThreadState;
using EnderCode.Utils;

namespace EnderCode.osu_backupAndRestore
{
    sealed class KeyEventArgs
    {
        internal ConsoleKeyInfo ConsoleKey { get; }
        internal KeyEventArgs(ConsoleKeyInfo consoleKey)
        {
            this.ConsoleKey = consoleKey;
        }
    }

    static class Operations
    {
        #region eventDeclaration
        internal delegate void KeyEventHandler(KeyEventArgs e);
        internal static event KeyEventHandler KeyEvent;
        #endregion
        private static readonly string errorPrefix = $"{MainEntry.langDict[UIElements.ErrorPrefix]} ";
        internal static void BackupAndRestore(bool isBackup, in bool exist)
        {
            DirectoryInfo destDir;
            string fileName, destFile;
            Console.Clear();
            if (!exist)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.BackupDirNotFound]);
                ChangeBackupDir();
            }
            string dst = "", src = "";
            if (isBackup)
            {
                src = MainEntry.data.installPath;
                dst = MainEntry.data.lastRunContent[2];
            }
            else
            {
                src = MainEntry.data.lastRunContent[2];
                dst = MainEntry.data.installPath;
            }

            Console.WriteLine($"{MainEntry.langDict[UIElements.GettingFiles]}");
            if (!Directory.Exists(src))
            {
                Util.WriteColored(errorPrefix, ConsoleColor.Red);
                Console.WriteLine(MainEntry.langDict[UIElements.NoSourceFound]);
                Console.ReadKey();
            }
            else
            {
                destDir = new DirectoryInfo(src);
                FileInfo[] files = destDir.GetFiles("*.db", SearchOption.TopDirectoryOnly);
                foreach (FileInfo item in files)
                {
                    fileName = item.Name;
                    Console.Write($"{MainEntry.langDict[UIElements.FileInfoPart1]} ");
                    Util.WriteColored(fileName, ConsoleColor.Cyan);
                    Console.Write($" {MainEntry.langDict[UIElements.FileInfoPart2]} ");
                    Util.WriteColored(src, ConsoleColor.Cyan);
                    Console.Write($" {MainEntry.langDict[UIElements.FileInfoPart3]} ");
                    Util.WriteColored(Util.SizeSuffixer(item.Length), ConsoleColor.Green);
                    Console.WriteLine($" {MainEntry.langDict[UIElements.FileInfoPart4]}...");
                }
                Console.Write($"{MainEntry.langDict[UIElements.CopyToast].Replace(@"%d", dst)}");
                foreach (FileInfo item in files)
                {
                    destFile = Path.Combine(dst, item.Name);
                    File.Copy(Path.Combine(src, item.Name), destFile, true);
                }
                IO.SettingsSaver(isBackup, false, MainEntry.data);
                Console.WriteLine(MainEntry.langDict[UIElements.Done]);
                DirectoryInfo dest = new DirectoryInfo(dst);
                FileInfo[] destFiles = dest.GetFiles("*.db", SearchOption.TopDirectoryOnly);
                long sizeFinal = 0;
                foreach (FileInfo item in destFiles)
                {
                    sizeFinal += item.Length;
                }
                Console.Write($"{MainEntry.langDict[UIElements.FinalSizePart1]}: ");
                Util.WriteColored(Util.SizeSuffixer(sizeFinal) + " ", ConsoleColor.Green);
                Console.WriteLine($"{MainEntry.langDict[UIElements.FinalSizePart2]}{destFiles.Length} {MainEntry.langDict[UIElements.FinalSizePart3]}");
                if (File.Exists(MainEntry.data.installPath + @"\safeguard.lock"))
                    File.Delete(MainEntry.data.installPath + @"\safeguard.lock");
                DumpMapIDs();
                if (MainEntry.data.qln)
                {
                    Launch();
                }
                else if (Dialogs.GeneralAskDialog(UIElements.QuestionLaunch))
                {
                    Launch();
                }
            }

        }
        internal static void Launch()
        {
            bool error = false;
            #region eventInit
            int threadCounter = 0;
            Thread thread = new Thread(RaiseKeyEvent);
            void @delegate(KeyEventArgs e) { thread.Abort(); }
            KeyEvent += @delegate;
            #endregion

            Console.WriteLine($"{MainEntry.langDict[UIElements.LaunchToast]}");
            Process process = null;
            try
            {
                process = Process.Start(Environment.ExpandEnvironmentVariables($@"{MainEntry.data.installPath}\osu!.exe"));
                Safeguard();
                Util.HideCurrentWindow(MainEntry.WindowHidden.Switch(), MainEntry.WindowHandle);
                for (byte i =  0; i < 4; i++)
                {
                    Interop.SetForegroundWindow(process.MainWindowHandle);
                    Thread.Sleep(1000);
                }
                process.WaitForExit();
                Util.HideCurrentWindow(MainEntry.WindowHidden.Switch(), MainEntry.WindowHandle);
            }
            catch (FileNotFoundException e)
            {
                Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(MainEntry.langDict[UIElements.FileNotFoundEx] + (MainEntry.data.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                ErrorMsg(e);
                error = true;
            }
            catch (Win32Exception e)
            {
                Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(MainEntry.langDict[UIElements.Win32Ex]);
                ErrorMsg(e);
                error = true;
                
            }
            catch (Exception e)
            {
                ErrorMsg(e);
                error = true;
            }

            if (process.ExitCode == 0)
                File.Delete($@"{MainEntry.data.installPath}\safeguard.lock");

            if (!error)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.ProcessEnded]);
                thread.Start();
                for (threadCounter = 0; threadCounter < 12; threadCounter++)
                {
                    CatchGameProcess(true);
                    if (thread.ThreadState == ThreadState.Aborted || thread.ThreadState == ThreadState.Stopped)
                        break;
                    Thread.Sleep(500);
                }
                if (thread.ThreadState != ThreadState.Aborted || thread.ThreadState != ThreadState.Stopped)
                {
                    thread.Abort();
                    KeyEvent -= @delegate;
                }
            }


            Console.Clear();
        }
        internal static void Repair()
        {
            Console.WriteLine(MainEntry.langDict[UIElements.RepairToast]);
            try
            {
                Process process = Process.Start(MainEntry.data.installPath + @"\osu!.exe", "-config");
                Safeguard();
                process.WaitForExit();
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.Win32Ex] + (MainEntry.data.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                ErrorMsg(e);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.FileNotFoundEx] + (MainEntry.data.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                ErrorMsg(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.WhatTheFuckWasThat] + (MainEntry.data.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                ErrorMsg(e);
            }

            File.Delete($@"{MainEntry.data.installPath}\safeguard.lock");
            Console.Clear();
        }
        internal static void RaiseKeyEvent()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            KeyEvent?.Invoke(new KeyEventArgs(keyInfo));
        }
        internal static void ChangeBackupDir()
        {
            Console.WriteLine(MainEntry.langDict[UIElements.FolderBrowsing]);
            var dialog = FormImpl4Con.CreateFolderBrowser(MainEntry.langDict[UIElements.BrowseFolder]);
            Thread.Sleep(3000);
            if (dialog.ShowDialog() == (System.Windows.Forms.DialogResult.Abort | System.Windows.Forms.DialogResult.Cancel))
            {
                Console.WriteLine(MainEntry.langDict[UIElements.BrowseAbort]);
                object a = Console.ReadKey();
                return;
            }
            MainEntry.data.backupDir = dialog.SelectedPath;
            Console.WriteLine(MainEntry.langDict[UIElements.BrowseSuccess]);
            Thread.Sleep(3000);
        }
        internal static void CatchGameProcess(bool isAutorun)
        {
            bool noError = true;
            Console.WriteLine("");
            Process process = new Process();
            try
            {
                Process[] processes = Process.GetProcessesByName("osu!");
                if (processes.Length > 1)
                {
                    Util.WriteColored($"{MainEntry.langDict[UIElements.WarnPrefix]}: ", ConsoleColor.Yellow);
                    Console.WriteLine(MainEntry.langDict[UIElements.MultiProcessWeirdness]);
                    Thread.Sleep(1500);
                }
                else
                {
                    process = Process.GetProcessById(processes[0].Id);
                    Safeguard();
                }
            }
            catch (IndexOutOfRangeException e)
            {
                noError = false;
                if (!isAutorun)
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(MainEntry.langDict[UIElements.NoProcess]);
                if (MainEntry.data.debug)
                {
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Message);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Source);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.StackTrace);
                }
                if (!isAutorun)
                {
                    Console.Write(MainEntry.langDict[UIElements.AwaitKeyToast]);
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                noError = false;
                Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine($"{MainEntry.langDict[UIElements.WhatTheFuckWasThat]} {(!MainEntry.data.debug ? MainEntry.langDict[UIElements.ErrorDetails] : string.Empty)}");
                if (MainEntry.data.debug)
                {
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Message);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Source);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.StackTrace);
                }
            }

            if (noError)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.ProcessCaught]);
                process.WaitForExit();
                try
                {
                    if (process.ExitCode == 0)
                        File.Delete($@"{MainEntry.data.installPath}\safeguard.lock");
                }
                catch (InvalidOperationException)
                {
                    File.Delete($@"{MainEntry.data.installPath}\safeguard.lock");
                }
            }
        }
        internal static void DumpMapIDs()
        {
            DirectoryInfo mapsDirectroy = new DirectoryInfo(MainEntry.data.installPath + @"\Songs");
            List<string> mapIDs = new List<string>();
            List<string> newMaps = new List<string>();
            List<FileInfo> collusions = new List<FileInfo>();
            foreach (var dir in mapsDirectroy.GetDirectories())
            {
                mapIDs.Add(dir.Name.Split(' ')[0]);
            }
            foreach (var file in mapsDirectroy.GetFiles("*.osz", SearchOption.TopDirectoryOnly))
            {
                string temp = file.Name.Split(' ')[0];
                if (mapIDs.Contains(temp))
                    collusions.Add(file);
                newMaps.Add(temp);
            }
            File.WriteAllLines($@"{MainEntry.data.backupDir}\mapdump.log", mapIDs.ToArray(), Encoding.UTF8);
            Util.WriteColored($"{mapIDs.Count}", ConsoleColor.Blue);
            Console.WriteLine(" " + MainEntry.langDict[UIElements.PartialDownloadedMaps]);
            Util.WriteColored($"{newMaps.Count}", ConsoleColor.Green);
            Console.WriteLine(" " + MainEntry.langDict[UIElements.PartialNewMaps]);
            if (collusions.Count != 0)
            {
                Util.WriteColored($"{collusions} ", ConsoleColor.Blue);
                Console.Write(MainEntry.langDict[UIElements.MapIDCollusion]);
                if (Dialogs.GeneralAskDialog(UIElements.CollusionDialog))
                    foreach (var file in collusions)
                    {
                        File.Delete(file.FullName);
                    }

            }
        }
        private static void Safeguard()
        {
            FileStream fs = File.Create(MainEntry.data.installPath + @"\safeguard.lock");
            fs.Dispose();
        }
        internal static void ConfirmDelete()
        {
            if (Dialogs.GeneralAskDialog(UIElements.QuestionDelete))
            {
                File.Delete($"{MainEntry.data.installPath}/safeguard.lock");
            }
            else { Util.WriteColoredLine(MainEntry.langDict[UIElements.Aborted], ConsoleColor.Red); }
        }
        internal static void ErrorMsg<T>(T e) where T : Exception
        {
            if (MainEntry.data.debug)
            {
                Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine((e as Exception).Message);
                Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine((e as Exception).Source);
                Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine((e as Exception).StackTrace);
                if (typeof(T) == typeof(Win32Exception))
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine("ErrorCode: " + (e as Win32Exception).ErrorCode);
            }
            Console.Write($"{MainEntry.langDict[UIElements.AwaitKeyToast]}");
            Console.ReadKey();
        }
    }
}
