using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EnderCode.Utils;


namespace EnderCode.osu_backupAndRestore
{
    static class MainEntry
    {
        internal static Dictionary<UIElements, string> langDict = new Dictionary<UIElements, string>();
        internal static MainData data = null;

        static void Main(string[] args)
        {
            Init();
            do
            {
                IO.LastRunReader(out bool exist, ref data);
                try
                {
                    if (data.lastRunContent[3] != "eng")
                    {
                        data.isEng = false;
                        LangInit();
                    }
                }
                catch (Exception)
                {
                }
                int cursorTop;

                if (data.debug)
                {
                    cursorTop = Console.CursorTop;
                    Console.SetCursorPosition(Console.WindowWidth - data.debugMsg.Length - 1, 0);
                    Util.WriteColored(data.debugMsg, ConsoleColor.DarkYellow);
                    Console.SetCursorPosition(0, cursorTop);
                }
                Console.Title = langDict[UIElements.WindowTitle];
                Console.WriteLine(langDict[UIElements.HeadLine]);
                if (exist)
                {
                    Console.Write($"{langDict[UIElements.LastOp]}: ");
                    Util.WriteColored(data.lastRunContent[0], ConsoleColor.Cyan);
                    Console.Write($" {langDict[UIElements.LastOpTime]}: ");
                    Util.WriteColoredLine(data.lastRunContent[1], ConsoleColor.Cyan);
                }
                else
                {
                    Console.WriteLine(langDict[UIElements.MissingLastRunInfo].Beautify());
                }
                Console.Write($"{langDict[UIElements.CurrentBackupDir]}: ");
                Util.WriteColoredLine(data.backupDir.Equals(string.Empty) ? langDict[UIElements.NoBackupDir] : data.backupDir, ConsoleColor.Magenta);
                Util.WriteColorFormated(langDict[UIElements.Commands].Beautify() + "\n", ConsoleColor.DarkCyan, null);
                bool safeguardFound = System.IO.File.Exists($@"{data.dir}\safeguard.lock");
                if (safeguardFound)
                {
                    Console.WriteLine(langDict[UIElements.SafeguardDeleteCmd]);
                    Util.WriteColoredLine(langDict[UIElements.SafeguardFound], ConsoleColor.Red);
                }
                Console.Write($@"{langDict[UIElements.Prompt]}> ");
                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.B:
                        if (input.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        {
                            data.qln = true;
                            data.stay = false;
                            Util.WriteColorFormated("%fS%fh%fi%ff%ft + B", ConsoleColor.Green, null);
                        }
                        else Console.WriteLine('B');
                        Operations.BackupAndRestore(true, ref exist);
                        break;
                    case ConsoleKey.R:
                        Console.WriteLine('R');
                        Operations.BackupAndRestore(false, ref exist);
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine('C');
                        Operations.ChangeBackupDir();
                        break;
                    case ConsoleKey.L:
                        if (input.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        {
                            data.qln = true;
                            data.stay = false;
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
                        data.debug = !data.debug;
                        break;
                    case ConsoleKey.Q:
                        Console.WriteLine('Q');
                        data.stay = false;
                        break;
                    case ConsoleKey.E:
                        Console.WriteLine('E');
                        Operations.CatchGameProcess(false);
                        break;
                    case ConsoleKey.Enter:
                        data.qln = true;
                        Operations.BackupAndRestore(true, ref exist);
                        data.qln = false;
                        break;
                    case ConsoleKey.F1:
                        data.isEng = !data.isEng;
                        LangInit();
                        IO.SettingsSaver(data.lastRunContent[0].Equals("backup"), true, ref data);
                        break;
                    case ConsoleKey.D:
                        if (input.Modifiers.HasFlag(ConsoleModifiers.Alt) && safeguardFound)
                        {
                            Util.WriteColorFormated("%fA%fl%ft + D", ConsoleColor.Green, null);
                            Operations.ConfirmDelete();
                        }
                        break;
                    case ConsoleKey.V:
                        Version();
                        break;
                    #region Unused      
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
                    case ConsoleKey.F:
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
                        break;
                    #endregion
                    default:
                        break;
                }
                if (data.stay) Console.Clear();
            } while (data.stay);
            if (!data.qln)
            {
                Console.WriteLine(langDict[UIElements.SeeYa]);
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Programkezdeti inícializálás
        /// </summary>
        static void Init()
        {

            if (!Environment.OSVersion.Platform.Equals(PlatformID.Win32NT))
            {
                throw new PlatformNotSupportedException("Only Windows NT or later supported");
            }
            data = new MainData();
            Console.OutputEncoding = Encoding.UTF8;
            Console.TreatControlCAsInput = true;
            Console.SetWindowSize(95, 24);
            Console.SetBufferSize(95, 9999);
            LangInit();
        }
        static void LangInit()
        {
            //Clear Dictionary
            langDict.Clear();
            //Fill Dictionary
            if (data.isEng)
            {
                //langDict.Add(UIElements, Language);
                langDict.Add(UIElements.WindowTitle, Language.WindowTitleEng);
                langDict.Add(UIElements.HeadLine, Language.HeadLineEng);
                langDict.Add(UIElements.CurrentBackupDir, Language.CurrentBackupDirEng);
                langDict.Add(UIElements.NoBackupDir, Language.NoBackupDirEng);
                langDict.Add(UIElements.NoSourceFound, Language.NoSourceFoundEng);
                langDict.Add(UIElements.Commands, Language.CommandsEng);
                langDict.Add(UIElements.LastOp, Language.LastOpEng);
                langDict.Add(UIElements.LastOpTime, Language.LastOpTimeEng);
                langDict.Add(UIElements.MissingLastRunInfo, Language.MissingLastRunInfoEng);
                langDict.Add(UIElements.SafeguardFound, Language.SafeguardFoundEng);
                langDict.Add(UIElements.Prompt, Language.PromptEng);
                langDict.Add(UIElements.SeeYa, Language.SeeYaEng);
                langDict.Add(UIElements.ErrorPrefix, Language.ErrorPrefixEng);
                langDict.Add(UIElements.GettingFiles, Language.GettingFilesEng);
                langDict.Add(UIElements.LaunchToast, Language.LaunchToastEng);
                langDict.Add(UIElements.AwaitKeyToast, Language.AwaitKeyToastEng);
                langDict.Add(UIElements.RepairToast, Language.RepairToastEng);
                langDict.Add(UIElements.CopyToast, Language.CopyToastEng);
                langDict.Add(UIElements.FileNotFoundEx, Language.FileNotFoundExEng);
                langDict.Add(UIElements.Win32Ex, Language.Win32ExEng);
                langDict.Add(UIElements.ProcessEnded, Language.ProcessEndedEng);
                langDict.Add(UIElements.FileInfoPart1, Language.FileInfoPart1Eng);
                langDict.Add(UIElements.FileInfoPart2, Language.FileInfoPart2Eng);
                langDict.Add(UIElements.FileInfoPart3, Language.FileInfoPart3Eng);
                langDict.Add(UIElements.FileInfoPart4, Language.FileInfoPart4Eng);
                langDict.Add(UIElements.FinalSizePart1, Language.FinalSizePart1Eng);
                langDict.Add(UIElements.FinalSizePart2, Language.FinalSizePart2Eng);
                langDict.Add(UIElements.FinalSizePart3, Language.FinalSizePart3Eng);
                langDict.Add(UIElements.ErrorDetails, Language.ErrorDetailsEng);
                langDict.Add(UIElements.NoCurrentBackupDir, Language.NoCurrentBackupDirEng);
                langDict.Add(UIElements.EnvVarInfo, Language.EnvVarInfoEng);
                langDict.Add(UIElements.NewDir, Language.NewDirEng);
                langDict.Add(UIElements.CorrectQuestionStr, Language.CorrectQuestionStrEng);
                langDict.Add(UIElements.CreateNew, Language.CreateNewEng);
                langDict.Add(UIElements.WarnPrefix, Language.WarnPrefixEng);
                langDict.Add(UIElements.QueryProcess, Language.QueryProcessEng);
                langDict.Add(UIElements.MultiProcessWeirdness, Language.MultiProcessWeirdnessEng);
                langDict.Add(UIElements.NoProcess, Language.NoProcessEng);
                langDict.Add(UIElements.WhatTheFuckWasThat, Language.WhatTheFuckWasThatEng);
                langDict.Add(UIElements.ProcessCaught, Language.ProcessCaughtEng);
                langDict.Add(UIElements.PartialDownloadedMaps, Language.PartialDownloadedMapsEng);
                langDict.Add(UIElements.PartialNewMaps, Language.PartialNewMapsEng);
                langDict.Add(UIElements.QuestionLaunch, Language.QuestionLaunchEng);
                langDict.Add(UIElements.QuestionSure, Language.QuestionSureEng);
                langDict.Add(UIElements.Done, Language.DoneEng);
                langDict.Add(UIElements.Aborted, Language.AbortedEng);
                langDict.Add(UIElements.BackupDirNotFound, Language.BackupDirNotFoundEng);
                langDict.Add(UIElements.QuestionDelete, Language.QuestionDeleteEng);
                langDict.Add(UIElements.SafeguardDeleteCmd, Language.SafeguardCommandEng);
                langDict.Add(UIElements.VersionToast, Language.VersionToastEng);
            }
            else
            {
                langDict.Add(UIElements.WindowTitle, Language.WindowTitleHun);
                langDict.Add(UIElements.HeadLine, Language.HeadLineHun);
                langDict.Add(UIElements.CurrentBackupDir, Language.CurrentBackupDirHun);
                langDict.Add(UIElements.NoBackupDir, Language.NoBackupDirHun);
                langDict.Add(UIElements.NoSourceFound, Language.NoSourceFoundHun);
                langDict.Add(UIElements.Commands, Language.CommandsHun);
                langDict.Add(UIElements.LastOp, Language.LastOpHun);
                langDict.Add(UIElements.LastOpTime, Language.LastOpTimeHun);
                langDict.Add(UIElements.MissingLastRunInfo, Language.MissingLastRunInfoHun);
                langDict.Add(UIElements.SafeguardFound, Language.SafeguardFoundHun);
                langDict.Add(UIElements.Prompt, Language.PromptHun);
                langDict.Add(UIElements.SeeYa, Language.SeeYaHun);
                langDict.Add(UIElements.ErrorPrefix, Language.ErrorPrefixHun);
                langDict.Add(UIElements.GettingFiles, Language.GettingFilesHun);
                langDict.Add(UIElements.LaunchToast, Language.LaunchToastHun);
                langDict.Add(UIElements.AwaitKeyToast, Language.AwaitKeyToastHun);
                langDict.Add(UIElements.RepairToast, Language.RepairToastHun);
                langDict.Add(UIElements.CopyToast, Language.CopyToastHun);
                langDict.Add(UIElements.FileNotFoundEx, Language.FileNotFoundExHun);
                langDict.Add(UIElements.Win32Ex, Language.Win32ExHun);
                langDict.Add(UIElements.ProcessEnded, Language.ProcessEndedHun);
                langDict.Add(UIElements.FileInfoPart1, Language.FileInfoPart1Hun);
                langDict.Add(UIElements.FileInfoPart2, Language.FileInfoPart2Hun);
                langDict.Add(UIElements.FileInfoPart3, Language.FileInfoPart3Hun);
                langDict.Add(UIElements.FileInfoPart4, Language.FileInfoPart4Hun);
                langDict.Add(UIElements.FinalSizePart1, Language.FinalSizePart1Hun);
                langDict.Add(UIElements.FinalSizePart2, Language.FinalSizePart2Hun);
                langDict.Add(UIElements.FinalSizePart3, Language.FinalSizePart3Hun);
                langDict.Add(UIElements.ErrorDetails, Language.ErrorDetailsHun);
                langDict.Add(UIElements.NoCurrentBackupDir, Language.NoCurrentBackupDirHun);
                langDict.Add(UIElements.EnvVarInfo, Language.EnvVarInfoHun);
                langDict.Add(UIElements.NewDir, Language.NewDirHun);
                langDict.Add(UIElements.CorrectQuestionStr, Language.CorrectQuestionStrHun);
                langDict.Add(UIElements.CreateNew, Language.CreateNewHun);
                langDict.Add(UIElements.WarnPrefix, Language.WarnPrefixHun);
                langDict.Add(UIElements.QueryProcess, Language.QueryProcessHun);
                langDict.Add(UIElements.MultiProcessWeirdness, Language.MultiProcessWeirdnessHun);
                langDict.Add(UIElements.NoProcess, Language.NoProcessHun);
                langDict.Add(UIElements.WhatTheFuckWasThat, Language.WhatTheFuckWasThatHun);
                langDict.Add(UIElements.ProcessCaught, Language.ProcessCaughtHun);
                langDict.Add(UIElements.PartialDownloadedMaps, Language.PartialDownloadedMapsHun);
                langDict.Add(UIElements.PartialNewMaps, Language.PartialNewMapsHun);
                langDict.Add(UIElements.QuestionLaunch, Language.QuestionLaunchHun);
                langDict.Add(UIElements.QuestionSure, Language.QuestionSureHun);
                langDict.Add(UIElements.Done, Language.DoneHun);
                langDict.Add(UIElements.Aborted, Language.AbortedHun);
                langDict.Add(UIElements.BackupDirNotFound, Language.BackupDirNotFoundHun);
                langDict.Add(UIElements.QuestionDelete, Language.QuestionDeleteHun);
                langDict.Add(UIElements.SafeguardDeleteCmd, Language.SafeguardCommandHun);
                langDict.Add(UIElements.VersionToast, Language.VersionToastHun);
            }
        }
        static void Version()
        {
            string versionString = MainEntry.langDict[UIElements.VersionToast].Replace("%d", System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion);
            int strLength = versionString.Length;
            //strLength += strLength == 32 ? 4 : 0;
            int left = (Console.WindowWidth % 2 == 1 ? Console.WindowWidth + 1 : Console.WindowWidth) / 2 - strLength / 2,
                top = (Console.WindowHeight % 2 == 1 ? Console.WindowHeight + 1 : Console.WindowHeight) / 2 - 3;
            Console.SetCursorPosition(left, top);

            for (int i = 0; i < 5; i++)
            {
                Console.SetCursorPosition(left, top + i);
                for (int j = 0; j < strLength + (data.isEng ? 4 : 2); j++)
                {
                    if (i == 0 || i == 4)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write("* ");
                        if (i == 1 || i == 3)
                            for (int k = 0; k < strLength; k++)
                            {
                                Console.Write(' ');
                            }
                        if (i == 2)
                        {
                            Console.SetCursorPosition(left + 2, top + i);
                            Console.Write(versionString);
                        }
                        Console.SetCursorPosition(left * 2 - (data.isEng ? 4 : 0), top + i);
                        Console.Write(" *");
                        break;
                    }
                }
            }
            Console.ReadKey(true);
        }
        public static string Beautify(this string unformated)
        {
            return unformated.Replace(@"\n", Environment.NewLine).Replace(@"\t", "\t");
        }

    }

    sealed class MainData
    {
        internal string dir = Environment.ExpandEnvironmentVariables(@"%userprofile%\AppData\Local\osu!");
        internal string lastRunInfo;
        internal string[] lastRunContent = { "backup", DateTime.MinValue.ToString(), null, "eng", "" };
        internal string backupDir;
        internal bool stay = true, qln = false, debug = false;
        internal readonly string debugMsg = "DEBUG MODE";
        internal bool isEng = true;

        public MainData()
        {
            lastRunInfo = $@"{dir}\settings.obr";
        }
    }

    enum UIElements
    {
        WindowTitle,
        HeadLine,
        CurrentBackupDir,
        BackupDirNotFound,
        NoBackupDir,
        NoSourceFound,
        Commands,
        LastOp,
        LastOpTime,
        MissingLastRunInfo,
        SafeguardFound,
        Prompt,
        SeeYa,
        ErrorPrefix,
        GettingFiles,
        LaunchToast,
        AwaitKeyToast,
        RepairToast,
        CopyToast,
        FileNotFoundEx,
        Win32Ex,
        ProcessEnded,
        FileInfoPart1,
        FileInfoPart2,
        FileInfoPart3,
        FileInfoPart4,
        FinalSizePart1,
        FinalSizePart2,
        FinalSizePart3,
        ErrorDetails,
        NoCurrentBackupDir,
        EnvVarInfo,
        NewDir,
        CorrectQuestionStr,
        CreateNew,
        WarnPrefix,
        QueryProcess,
        MultiProcessWeirdness,
        NoProcess,
        WhatTheFuckWasThat,
        ProcessCaught,
        PartialDownloadedMaps,
        PartialNewMaps,
        QuestionLaunch,
        QuestionSure,
        Done,
        Aborted,
        QuestionDelete,
        SafeguardDeleteCmd,
        VersionToast
    }

}
