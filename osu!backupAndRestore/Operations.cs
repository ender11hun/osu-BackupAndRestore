﻿using System;
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
        internal static void BackupAndRestore(bool isBackup, ref bool exist)
        {
            DirectoryInfo destDir;
            string fileName, destFile;
            Console.Clear();
            if (!exist)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.BackupDirNotFound].Beautify());
                ChangeBackupDir();
            }
            string dst = "", src = "";
            if (isBackup)
            {
                src = MainEntry.data.dir;
                dst = MainEntry.data.lastRunContent[2];
            }
            else
            {
                src = MainEntry.data.lastRunContent[2];
                dst = MainEntry.data.dir;
            }

            Console.WriteLine($"{MainEntry.langDict[UIElements.GettingFiles]}");
            if (!Directory.Exists(src))
            {
                Util.WriteColored(errorPrefix, ConsoleColor.Red);
                Console.WriteLine(MainEntry.langDict[UIElements.NoSourceFound].Beautify());
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
                IO.SettingsSaver(isBackup, false, ref MainEntry.data);
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
                if (File.Exists(MainEntry.data.dir + @"\safeguard.lock"))
                    File.Delete(MainEntry.data.dir + @"\safeguard.lock");
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
                process = Process.Start(Environment.ExpandEnvironmentVariables($@"{MainEntry.data.dir}\osu!.exe"));
                Safeguard();
                int x = await Util.BringMainWindowToFront(process.MainWindowHandle);
                process.WaitForExit();
            }
            catch (FileNotFoundException e)
            {
                Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(MainEntry.langDict[UIElements.FileNotFoundEx] + (MainEntry.data.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                if (MainEntry.data.debug)
                {
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Message);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Source);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.StackTrace);
                }
                Console.Write($"{MainEntry.langDict[UIElements.AwaitKeyToast]}");
                Console.ReadKey();
                error = true;
            }
            catch (Win32Exception e)
            {
                Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(MainEntry.langDict[UIElements.Win32Ex]);
                if (MainEntry.data.debug)
                {
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Message);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Source);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.StackTrace);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine("ErrorCode: " + e.ErrorCode);
                }
                Console.Write($"{MainEntry.langDict[UIElements.AwaitKeyToast]}");
                Console.ReadKey();
                error = true;
            }

            if (process.ExitCode == 0)
                File.Delete($@"{MainEntry.data.dir}\safeguard.lock");

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
                Process process = Process.Start(MainEntry.data.dir + @"\osu!.exe", "-config");
                Safeguard();
                process.WaitForExit();
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.Win32Ex] + (MainEntry.data.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                if (MainEntry.data.debug)
                {
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Message);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Source);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.StackTrace);
                }
                Console.Write($"{MainEntry.langDict[UIElements.AwaitKeyToast]}");
                Console.ReadKey();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.FileNotFoundEx] + (MainEntry.data.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                if (MainEntry.data.debug)
                {
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Message);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Source);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.StackTrace);
                }
                Console.Write($"{MainEntry.langDict[UIElements.AwaitKeyToast]}");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(MainEntry.langDict[UIElements.WhatTheFuckWasThat] + (MainEntry.data.debug ? $"\n{MainEntry.langDict[UIElements.ErrorDetails]}:" : string.Empty));
                if (MainEntry.data.debug)
                {
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Message);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.Source);
                    Util.WriteColored(errorPrefix, ConsoleColor.Red); Console.WriteLine(e.StackTrace);
                }
                Console.Write($"{MainEntry.langDict[UIElements.AwaitKeyToast]}");
                Console.ReadKey();
            }

            File.Delete($@"{MainEntry.data.dir}\safeguard.lock");
            Console.Clear();
        }
        internal static void RaiseKeyEvent()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            KeyEvent?.Invoke(new KeyEventArgs(keyInfo));
        }
        internal static void ChangeBackupDir()
        {
            Console.WriteLine($"{MainEntry.langDict[UIElements.CurrentBackupDir]}: {(MainEntry.data.backupDir.Equals(string.Empty) ? MainEntry.langDict[UIElements.NoCurrentBackupDir] : MainEntry.data.backupDir)}");
            Console.WriteLine(MainEntry.langDict[UIElements.EnvVarInfo]);
            string newDir;
            ConsoleKey a;
            bool isCorrect;
            do
            {
                Console.Write($"{MainEntry.langDict[UIElements.NewDir]}: ");
                newDir = Console.ReadLine();
                Console.WriteLine($@"{MainEntry.langDict[UIElements.NewDir]} ({newDir})");
                bool exist = Directory.Exists(newDir);
                do
                {
                    if (exist)
                    {
                        Console.Write($"{MainEntry.langDict[UIElements.CorrectQuestionStr]}: ");
                    }
                    else
                    {
                        Console.Write($"{MainEntry.langDict[UIElements.CreateNew]}: ");
                    }
                    a = Console.ReadKey().Key;
                } while (!(a.Equals(MainEntry.data.isEng ? ConsoleKey.Y : ConsoleKey.I) || a.Equals(ConsoleKey.N)));
                if (!exist) Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(newDir));
                isCorrect = a.Equals(MainEntry.data.isEng ? ConsoleKey.Y : ConsoleKey.I);
            } while (!isCorrect);
            MainEntry.data.backupDir = newDir;
            IO.SettingsSaver(true, true, ref MainEntry.data);
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
                        File.Delete($@"{MainEntry.data.dir}\safeguard.lock");
                }
                catch (InvalidOperationException)
                {
                    File.Delete($@"{MainEntry.data.dir}\safeguard.lock");
                }
            }
        }
        internal static void DumpMapIDs()
        {
            DirectoryInfo mapsDirectroy = new DirectoryInfo(MainEntry.data.dir + @"\Songs");
            List<string> mapIDs = new List<string>();
            List<string> newMaps = new List<string>();
            foreach (var dir in mapsDirectroy.GetDirectories())
            {
                mapIDs.Add(dir.Name.Split(' ')[0]);
            }
            foreach (var file in mapsDirectroy.GetFiles("*.osz", SearchOption.TopDirectoryOnly))
            {
                newMaps.Add(file.Name.Split(' ')[0]);
            }
            File.WriteAllLines($@"{MainEntry.data.backupDir}\mapdump.log", mapIDs.ToArray(), Encoding.UTF8);
            Util.WriteColored($"{mapIDs.Count}", ConsoleColor.Blue);
            Console.WriteLine(" " + MainEntry.langDict[UIElements.PartialDownloadedMaps]);
            Util.WriteColored($"{newMaps.Count}", ConsoleColor.Green);
            Console.WriteLine(" " + MainEntry.langDict[UIElements.PartialNewMaps]);
        }
        private static void Safeguard()
        {
            using (FileStream fs = File.Create(MainEntry.data.dir + @"\safeguard.lock"))
            { return; }
        }
        internal static void ConfirmDelete()
        {
            if (Dialogs.GeneralAskDialog(UIElements.QuestionDelete))
            {
                File.Delete($"{MainEntry.data.dir}/safeguard.lock");
            }
            else { Console.WriteLine(MainEntry.langDict[UIElements.Aborted]); }
        }
    }
}
