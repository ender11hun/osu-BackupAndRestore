using EnderCode.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using ThreadState = System.Threading.ThreadState;

#pragma warning disable CA1031,CA2000 // Do not catch general exception types and don't pester me for you being blind to not see I properly disposed my Proccess objects.

namespace EnderCode.osuBackupAndRestore
{
    sealed class KeyEventArgs
    {
        internal ConsoleKeyInfo ConsoleKey { get; }
        internal KeyEventArgs(ConsoleKeyInfo consoleKey)
        {
            ConsoleKey = consoleKey;
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
            Console.Clear();
            if (!exist)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.BackupDirNotFound]);
                ChangeBackupDir();
            }
            string destination, source;
            if (isBackup)
            {
                source = AppData.installPath;
                destination = AppData.lastRunContent[2];
            }
            else
            {
                source = AppData.lastRunContent[2];
                destination = AppData.installPath;
            }

            Console.WriteLine($"{MainEntry.langDict[UIElements.GettingFiles]}");
            if (!Directory.Exists(source))
            {
                Util.WriteColored(errorPrefix, ConsoleColor.Red);
                Console.WriteLine(MainEntry.langDict[UIElements.NoSourceFound]);
                Console.ReadKey();
            }
            else
            {
                destDir = new DirectoryInfo(source);
                FileInfo[] files = destDir.GetFiles("*.db", SearchOption.TopDirectoryOnly);
                foreach (FileInfo item in files)
                {
                    Console.Write($"{MainEntry.langDict[UIElements.FileInfoPart1]} ");
                    Util.WriteColored(item.Name, ConsoleColor.Cyan);
                    Console.Write($" {MainEntry.langDict[UIElements.FileInfoPart2]} ");
                    Util.WriteColored(source, ConsoleColor.Cyan);
                    Console.Write($" {MainEntry.langDict[UIElements.FileInfoPart3]} ");
                    Util.WriteColored(Util.SizeSuffixer(item.Length), ConsoleColor.Green);
                    Console.WriteLine($" {MainEntry.langDict[UIElements.FileInfoPart4]}...");
                }
                Console.Write($"{MainEntry.langDict[UIElements.CopyToast].Replace(@"%d", destination)}");
                foreach (FileInfo item in files)
                {
                    File.Copy(Path.Combine(source, item.Name), Path.Combine(destination, item.Name), true);
                }
                AppData.SettingsSaver(isBackup, false);
                Console.WriteLine(MainEntry.langDict[UIElements.Done]);
                DirectoryInfo dest = new DirectoryInfo(destination);
                FileInfo[] destFiles = dest.GetFiles("*.db", SearchOption.TopDirectoryOnly);
                long sizeFinal = 0;
                foreach (FileInfo item in destFiles)
                {
                    sizeFinal += item.Length;
                }
                Console.Write($"{MainEntry.langDict[UIElements.FinalSizePart1]}: ");
                Util.WriteColored(Util.SizeSuffixer(sizeFinal) + " ", ConsoleColor.Green);
                Console.WriteLine($"{MainEntry.langDict[UIElements.FinalSizePart2]}{destFiles.Length} {MainEntry.langDict[UIElements.FinalSizePart3]}");
                if (File.Exists(AppData.installPath + @"\safeguard.lock"))
                    File.Delete(AppData.installPath + @"\safeguard.lock");
                DumpMapIDs();
                if (AppData.qln)
                {
                    Launch();
                }
                else if (Dialogs.GeneralAskDialog(UIElements.QuestionLaunch))
                {
                    Launch();
                }
            }

        }
        internal static async void Launch()
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
                process = Process.Start(Environment.ExpandEnvironmentVariables($@"{AppData.installPath}\osu!.exe"));
                Safeguard();
                Util.HideCurrentWindow(MainEntry.WindowHidden = true, MainEntry.WindowHandle);
                for (byte i = 0; i < 4; i++)
                {
                    _ = Interop.SetForegroundWindow(process.MainWindowHandle);
                    Thread.Sleep(1000);
                }
                process.WaitForExit();
                Util.HideCurrentWindow(MainEntry.WindowHidden = false, MainEntry.WindowHandle);
                _ = await Util.BringWindowToFront(MainEntry.WindowHandle).ConfigureAwait(false);
            }
            catch (FileNotFoundException e)
            {
                Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(MainEntry.langDict[UIElements.FileNotFoundEx] + (AppData.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
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
                File.Delete($@"{AppData.installPath}\safeguard.lock");

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
                Process process = Process.Start(AppData.installPath + @"\osu!.exe", "-config");
                Safeguard();
                process.WaitForExit();
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.Win32Ex] + (AppData.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                ErrorMsg(e);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.FileNotFoundEx] + (AppData.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                ErrorMsg(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.WhatTheFuckWasThat] + (AppData.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                ErrorMsg(e);
            }

            File.Delete($@"{AppData.installPath}\safeguard.lock");
            Console.Clear();
        }
        internal static void RaiseKeyEvent()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            KeyEvent?.Invoke(new KeyEventArgs(keyInfo));
        }
        internal static void ChangeBackupDir()
        {
            Console.WriteLine(MainEntry.langDict[UIElements.BrowseBackup]);
            using var dialog = FormImpl4Con.CreateFolderBrowser(MainEntry.langDict[UIElements.BrowseFolder]);
            Thread.Sleep(3000);
            if (dialog.ShowDialog() == (System.Windows.Forms.DialogResult.Abort | System.Windows.Forms.DialogResult.Cancel))
            {
                Console.WriteLine(MainEntry.langDict[UIElements.BrowseBackupAbort]);
                _ = Console.ReadKey();
                return;
            }
            AppData.backupDir = dialog.SelectedPath;
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
                if (AppData.debug)
                {
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Message);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Source);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.StackTrace);
                }
                Util.WriteColoredLine("Log file saved as: " + Util.Logger(e, "ProcessCatcher"), ConsoleColor.Yellow);
                if (!isAutorun)
                {
                    Console.Write(MainEntry.langDict[UIElements.AwaitKeyToast]);
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                noError = false;
                Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine($"{MainEntry.langDict[UIElements.WhatTheFuckWasThat]} {(!AppData.debug ? MainEntry.langDict[UIElements.ErrorDetails] : string.Empty)}");
                if (AppData.debug)
                {
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Message);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Source);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.StackTrace);
                }
            }
            finally
            {
                if (noError)
                {
                    Console.WriteLine(MainEntry.langDict[UIElements.ProcessCaught]);
                    process.WaitForExit();
                    try
                    {
                        if (process.ExitCode == 0)
                            File.Delete($@"{AppData.installPath}\safeguard.lock");
                    }
                    catch (InvalidOperationException)
                    {
                        File.Delete($@"{AppData.installPath}\safeguard.lock");
                    }
                    catch (IOException ioEx)
                    {
                        Util.WriteColoredLine("Log file saved as: " + Util.Logger(ioEx, "SafeguardIO"), ConsoleColor.Yellow);
                    }
                    catch (UnauthorizedAccessException authEx)
                    {
                        Util.WriteColoredLine("Log file saved as: " + Util.Logger(authEx, "SafeguardAuth"), ConsoleColor.Yellow);
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }
            }
        }
        internal static void DumpMapIDs()
        {
            DirectoryInfo mapsDirectroy = new DirectoryInfo(AppData.installPath + @"\Songs");
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
            File.WriteAllLines($@"{AppData.backupDir}\mapdump.log", mapIDs.ToArray(), Encoding.UTF8);
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
            FileStream fs = File.Create(AppData.installPath + @"\safeguard.lock");
            fs.Dispose();
        }
        internal static void ConfirmDelete()
        {
            if (Dialogs.GeneralAskDialog(UIElements.QuestionDelete))
            {
                File.Delete($"{AppData.installPath}/safeguard.lock");
            }
            else { Util.WriteColoredLine(MainEntry.langDict[UIElements.Aborted], ConsoleColor.Red); }
        }
        internal static void ErrorMsg<T>(T e) where T : Exception
        {
            if (AppData.debug)
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
