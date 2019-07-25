using System;
using System.Text;
using System.Timers;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
#pragma warning disable CS1998

namespace osu_backupAndRestore
{
    static class Utils
    {
        public static string SizeSuffixer(long bytes)
        {
            if (((bytes / 1024f) / 1024f) >= 1)
            {
                return $"{double.Parse(string.Format("{0:0.0##}", Math.Round((bytes / 1024f) / 1024f), 3), CultureInfo.InvariantCulture)}MB";
            }
            return $"{double.Parse(string.Format("{0:0.0##}", Math.Round(bytes / 1024f, 3)), CultureInfo.InvariantCulture)}KB";
        }
        public static void WriteColored(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
        public static void WriteColoredLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
            Console.WriteLine();
        }
        public static string Beautify(this string unformated)
        {
            return unformated.Replace("\\n", Environment.NewLine).Replace("\\t", "\t");
        }
        public static void WriteColorFormated(string text, ConsoleColor? foreColor, ConsoleColor? backColor)
        {
            bool noFormating = true;
            int cycle = 0;
            int offset = 0;
            while (text.Contains("%f") || text.Contains("%b"))
            {
                noFormating = false;
                int index = text.IndexOf('%', offset++);
                if (text[index + 1] == 'f')
                {
                    Console.ForegroundColor = !foreColor.HasValue ? Console.ForegroundColor : (ConsoleColor)foreColor;
                    text = text.Remove(index, 2);
                    if (text[index] != '%')
                    {
                        Console.Write(text[cycle++]);
                        Console.ResetColor();
                    }
                    continue;
                }
                if (text[index + 1] == 'b')
                {
                    Console.BackgroundColor = !backColor.HasValue ? Console.BackgroundColor : (ConsoleColor)backColor;
                    text = text.Remove(index, 2);
                    if (text[index] != '%')
                    {
                        Console.Write(text[cycle++]);
                        Console.ResetColor();
                    }
                    continue;
                }
            }
            
            if (noFormating) Console.Write(text);
            else Console.Write(text.Remove(0, offset));
        }
        public static async Task<int> BringMainWindowToFront(IntPtr hwnd)
        {
            // check if the window is hidden / minimized
            if (hwnd == IntPtr.Zero)
            {
                // the window is hidden so try to restore it before setting focus.
                for (int i = 0; i < 4; i++)
                {
                    if (ShowWindow(hwnd, ShowWindowEnum.Restore))
                        return 0;
                    Thread.Sleep(1000);
                }
            }

            // set user the focus to the window
            for (int i = 0; i < 4; i++)
            {
                if (SetForegroundWindow(hwnd) == 0)
                    return 0;
                Thread.Sleep(1000);
            }
            //throw new InvalidOperationException("Window is UAC protected or its parent process has closed");
            return int.MaxValue;
        }
        #region Interop
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);
        #endregion
    }

    enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };
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
        SafeguardDeleteCmd
    }
}
