﻿using EnderCode.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EnderCode.osuBackupAndRestore
{
    /// <summary>
    /// Application entrypoint
    /// </summary>
    static class MainEntry
    {
        internal static Dictionary<UIElements, string> langDict = new Dictionary<UIElements, string>();
        internal static int cursorTop;
        internal static IntPtr WindowHandle => Interop.GetConsoleWindow();
        internal static bool WindowHidden = false;
        internal static Thread trayiconThread;

        [STAThread]
        static void Main()
        {
            Init();

            static void Version()
            {
                string versionStr = langDict[UIElements.VersionString];
                string libVerSting = langDict[UIElements.LibVersion];
                cursorTop = Console.CursorTop;
                Console.SetCursorPosition(Console.WindowWidth - versionStr.Length - CoreAssembly.Version.ToString().Length - 1, 0); ;
                Console.Write(langDict[UIElements.VersionString]);
                Util.WriteColored(CoreAssembly.Version.ToString(), true, ConsoleColor.Green);
                Console.SetCursorPosition(Console.WindowWidth - libVerSting.Length - Util.CoreAssembly.Version.ToString().Length - 1, 1);
                Console.Write(langDict[UIElements.LibVersion]);
                Util.WriteColored(Util.CoreAssembly.Version.ToString(), true, ConsoleColor.Green);
                Console.SetCursorPosition(0, cursorTop);
            }
            do
            {
#pragma warning disable IDE0018 // Inline variable declaration
                bool settingsFound;
#pragma warning restore IDE0018
#if !DEBUG
                try
                {
#endif
                AppData.LastRunReader(out settingsFound);
                if (AppData.lastRunContent[3] != "eng")
                {
                    AppData.isEng = false;
                    LangInit();
                }
                string execPath = System.IO.Path.Combine(AppData.installPath, "osu!.exe");
                if (!System.IO.File.Exists(execPath))
                {
                    AppData.installPath = Dialogs.InstallNotFound();
                    if (AppData.installPath == null || execPath.Contains(AppData.installPath) || !System.IO.File.Exists(System.IO.Path.Combine(AppData.installPath, "osu!.exe")))
                    {
                        Console.WriteLine(langDict[UIElements.AbortedOrNotFound]);
                        Thread.Sleep(3000);
                        goto Exit;
                    }
                    else
                    {
                        AppData.SettingsSaver(false, true);
                    }
                }
#if !DEBUG
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(langDict[UIElements.EarlyException].Replace("<file>", Util.Logger(e, "exception")));
                    Console.WriteLine(langDict[UIElements.AwaitKeyToast]);
                    Console.ReadKey();
                    goto Exit;
                }
#endif
                Version();

                Console.Title = langDict[UIElements.WindowTitle];
                Console.WriteLine(langDict[UIElements.HeadLine]);

                if (settingsFound)
                {
                    Console.Write($"{langDict[UIElements.LastOp]}: ");
                    Util.WriteColored(AppData.lastRunContent[0],false, ConsoleColor.Cyan);
                    Console.Write($" {langDict[UIElements.LastOpTime]}: ");
                    Util.WriteColored(AppData.lastRunContent[1],false, ConsoleColor.Cyan);
                }
                else
                {
                    Console.WriteLine(langDict[UIElements.MissingLastRunInfo]);
                }

                Console.Write($"{langDict[UIElements.CurrentBackupDir]}: ");
                Util.WriteColored(string.IsNullOrEmpty(AppData.backupDir) ? langDict[UIElements.NoBackupDir] : AppData.backupDir,true, ConsoleColor.Magenta);
                Util.WriteColorFormated(langDict[UIElements.Commands] + "\n", ConsoleColor.DarkCyan, null);

                bool safeguardFound = System.IO.File.Exists($@"{AppData.installPath}\safeguard.lock");
                if (safeguardFound)
                {
                    Console.WriteLine(langDict[UIElements.SafeguardDeleteCmd]);
                    Util.WriteColored(langDict[UIElements.SafeguardFound],true, ConsoleColor.Red);
                }

                Console.Write($@"{langDict[UIElements.Prompt]}> ");
                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.B:
                        if (input.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        {
                            AppData.qln = true;
                            AppData.stay = false;
                            Util.WriteColorFormated("%fS%fh%fi%ff%ft + B", ConsoleColor.Green, null);
                        }
                        else Console.WriteLine('B');
                        Operations.BackupAndRestore(true, in settingsFound);
                        break;
                    case ConsoleKey.R:
                        Console.WriteLine('R');
                        Operations.BackupAndRestore(false, in settingsFound);
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine('C');
                        Operations.ChangeBackupDir();
                        AppData.SettingsSaver(false, false);
                        break;
                    case ConsoleKey.L:
                        if (input.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        {
                            AppData.qln = true;
                            AppData.stay = false;
                            Util.WriteColorFormated("%fS%fh%fi%ff%ft + L", ConsoleColor.Green, null);
                        }
                        else Console.WriteLine('L');
                        Operations.Launch();
                        break;
                    case ConsoleKey.P:
                        Console.WriteLine('P');
                        Dialogs.AreYouSure();
                        break;
                    case ConsoleKey.F2:
                        AppData.debug = !AppData.debug;
                        break;
                    case ConsoleKey.Q:
                        Console.WriteLine('Q');
                        AppData.stay = false;
                        break;
                    case ConsoleKey.E:
                        Console.WriteLine('E');
                        Operations.CatchGameProcess(false);
                        break;
                    case ConsoleKey.Enter:
                        AppData.qln = true;
                        Operations.BackupAndRestore(true, in settingsFound);
                        AppData.qln = false;
                        break;
                    case ConsoleKey.F1:
                        AppData.isEng = !AppData.isEng;
                        LangInit();
                        AppData.SettingsSaver(AppData.lastRunContent[0].Equals("backup"), true);
                        break;
                    case ConsoleKey.D:
                        if (input.Modifiers.HasFlag(ConsoleModifiers.Alt) && safeguardFound)
                        {
                            Util.WriteColorFormated("%fA%fl%ft + D", ConsoleColor.Green, null);
                            Operations.ConfirmDelete();
                        }
                        break;
                    #region Unused keys
                        /*
                    case ConsoleKey.V:
                        //Version();
                        break;
                    case ConsoleKey.F:
                        break;
                    case ConsoleKey.Backspace:
                        break;
                    case ConsoleKey.Tab:
                        break;
                    case ConsoleKey.Clear:
                        break;
                    case ConsoleKey.Pause:
                        break;
                    case ConsoleKey.Escape:
                        break;
                    case ConsoleKey.Spacebar:
                        break;
                    case ConsoleKey.PageUp:
                        break;
                    case ConsoleKey.PageDown:
                        break;
                    case ConsoleKey.End:
                        break;
                    case ConsoleKey.Home:
                        break;
                    case ConsoleKey.LeftArrow:
                        break;
                    case ConsoleKey.UpArrow:
                        break;
                    case ConsoleKey.RightArrow:
                        break;
                    case ConsoleKey.DownArrow:
                        break;
                    case ConsoleKey.Select:
                        break;
                    case ConsoleKey.Print:
                        break;
                    case ConsoleKey.Execute:
                        break;
                    case ConsoleKey.PrintScreen:
                        break;
                    case ConsoleKey.Insert:
                        break;
                    case ConsoleKey.Delete:
                        break;
                    case ConsoleKey.Help:
                        break;
                    case ConsoleKey.D0:
                        break;
                    case ConsoleKey.D1:
                        break;
                    case ConsoleKey.D2:
                        break;
                    case ConsoleKey.D3:
                        break;
                    case ConsoleKey.D4:
                        break;
                    case ConsoleKey.D5:
                        break;
                    case ConsoleKey.D6:
                        break;
                    case ConsoleKey.D7:
                        break;
                    case ConsoleKey.D8:
                        break;
                    case ConsoleKey.D9:
                        break;
                    case ConsoleKey.A:
                        break;
                    case ConsoleKey.G:
                        break;
                    case ConsoleKey.H:
                        break;
                    case ConsoleKey.I:
                        break;
                    case ConsoleKey.J:
                        break;
                    case ConsoleKey.K:
                        break;
                    case ConsoleKey.M:
                        break;
                    case ConsoleKey.N:
                        break;
                    case ConsoleKey.O:
                        break;
                    case ConsoleKey.S:
                        break;
                    case ConsoleKey.T:
                        break;
                    case ConsoleKey.U:
                        break;
                    case ConsoleKey.W:
                        break;
                    case ConsoleKey.X:
                        break;
                    case ConsoleKey.Y:
                        break;
                    case ConsoleKey.Z:
                        break;
                    case ConsoleKey.LeftWindows:
                        break;
                    case ConsoleKey.RightWindows:
                        break;
                    case ConsoleKey.Applications:
                        break;
                    case ConsoleKey.Sleep:
                        break;
                    case ConsoleKey.NumPad0:
                        break;
                    case ConsoleKey.NumPad1:
                        break;
                    case ConsoleKey.NumPad2:
                        break;
                    case ConsoleKey.NumPad3:
                        break;
                    case ConsoleKey.NumPad4:
                        break;
                    case ConsoleKey.NumPad5:
                        break;
                    case ConsoleKey.NumPad6:
                        break;
                    case ConsoleKey.NumPad7:
                        break;
                    case ConsoleKey.NumPad8:
                        break;
                    case ConsoleKey.NumPad9:
                        break;
                    case ConsoleKey.Multiply:
                        break;
                    case ConsoleKey.Add:
                        break;
                    case ConsoleKey.Separator:
                        break;
                    case ConsoleKey.Subtract:
                        break;
                    case ConsoleKey.Decimal:
                        break;
                    case ConsoleKey.Divide:
                        break;
                    case ConsoleKey.F3:
                        break;
                    case ConsoleKey.F4:
                        break;
                    case ConsoleKey.F5:
                        break;
                    case ConsoleKey.F6:
                        break;
                    case ConsoleKey.F7:
                        break;
                    case ConsoleKey.F8:
                        break;
                    case ConsoleKey.F9:
                        break;
                    case ConsoleKey.F10:
                        break;
                    case ConsoleKey.F11:
                        break;
                    case ConsoleKey.F12:
                        break;
                    case ConsoleKey.F13:
                        break;
                    case ConsoleKey.F14:
                        break;
                    case ConsoleKey.F15:
                        break;
                    case ConsoleKey.F16:
                        break;
                    case ConsoleKey.F17:
                        break;
                    case ConsoleKey.F18:
                        break;
                    case ConsoleKey.F19:
                        break;
                    case ConsoleKey.F20:
                        break;
                    case ConsoleKey.F21:
                        break;
                    case ConsoleKey.F22:
                        break;
                    case ConsoleKey.F23:
                        break;
                    case ConsoleKey.F24:
                        break;
                    case ConsoleKey.BrowserBack:
                        break;
                    case ConsoleKey.BrowserForward:
                        break;
                    case ConsoleKey.BrowserRefresh:
                        break;
                    case ConsoleKey.BrowserStop:
                        break;
                    case ConsoleKey.BrowserSearch:
                        break;
                    case ConsoleKey.BrowserFavorites:
                        break;
                    case ConsoleKey.BrowserHome:
                        break;
                    case ConsoleKey.VolumeMute:
                        break;
                    case ConsoleKey.VolumeDown:
                        break;
                    case ConsoleKey.VolumeUp:
                        break;
                    case ConsoleKey.MediaNext:
                        break;
                    case ConsoleKey.MediaPrevious:
                        break;
                    case ConsoleKey.MediaStop:
                        break;
                    case ConsoleKey.MediaPlay:
                        break;
                    case ConsoleKey.LaunchMail:
                        break;
                    case ConsoleKey.LaunchMediaSelect:
                        break;
                    case ConsoleKey.LaunchApp1:
                        break;
                    case ConsoleKey.LaunchApp2:
                        break;
                    case ConsoleKey.Oem1:
                        break;
                    case ConsoleKey.OemPlus:
                        break;
                    case ConsoleKey.OemComma:
                        break;
                    case ConsoleKey.OemMinus:
                        break;
                    case ConsoleKey.OemPeriod:
                        break;
                    case ConsoleKey.Oem2:
                        break;
                    case ConsoleKey.Oem3:
                        break;
                    case ConsoleKey.Oem4:
                        break;
                    case ConsoleKey.Oem5:
                        break;
                    case ConsoleKey.Oem6:
                        break;
                    case ConsoleKey.Oem7:
                        break;
                    case ConsoleKey.Oem8:
                        break;
                    case ConsoleKey.Oem102:
                        break;
                    case ConsoleKey.Process:
                        break;
                    case ConsoleKey.Packet:
                        break;
                    case ConsoleKey.Attention:
                        break;
                    case ConsoleKey.CrSel:
                        break;
                    case ConsoleKey.ExSel:
                        break;
                    case ConsoleKey.EraseEndOfFile:
                        break;
                    case ConsoleKey.Play:
                        break;
                    case ConsoleKey.Zoom:
                        break;
                    case ConsoleKey.NoName:
                        break;
                    case ConsoleKey.Pa1:
                        break;
                    case ConsoleKey.OemClear:
                        break;*/
                    #endregion
                    default:
                        break;
                }
                if (AppData.stay) Console.Clear();
            } while (AppData.stay);
            if (!AppData.qln)
            {
                Console.WriteLine(langDict[UIElements.SeeYa]);
                Thread.Sleep(1000);
            }
        Exit:
            Application.Exit();
        }


        /// <summary>
        /// Programkezdeti inícializálás
        /// </summary>
        /// <remarks><b>Megjegyzés:</b><br/>Mivel nem Forms app vagyunk, igy egy külön szálat (Thread) kell inditani a <see cref="SystemTray"/> komponenshez</remarks>
        static void Init()
        {
            Dialogs.Win32ConHandle = new FormImpl4Con.Win32Window(WindowHandle);
            if (!Environment.OSVersion.Platform.Equals(PlatformID.Win32NT))
            {
                throw new PlatformNotSupportedException("Only Windows NT or later supported");
            }
            trayiconThread = new Thread(delegate () { Application.Run(SystemTray.instance); });
            trayiconThread.Start();
            Console.OutputEncoding = Encoding.UTF8;
            Console.TreatControlCAsInput = true;
            Console.SetWindowSize(95, 24);
            Console.SetBufferSize(95, 9999);
            LangInit();
        }

        /// <summary>
        /// Nyelvi változók inícializálása
        /// </summary>
        static void LangInit()
        {
            string[] enumNames = typeof(UIElements).GetEnumNames();
            //Clear Dictionary
            langDict.Clear();
            //Fill Dictionary
            string lang = AppData.isEng ? "Eng" : "Hun";
            foreach (var item in enumNames)
            {
                langDict.Add(
                    item.GetEnum<UIElements>(),
                    ((string)typeof(Language).GetProperty(
                        item + lang,
                        BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public
                    ).GetValue(null, null)).Beautify()
                );
            }
        }
    }
}