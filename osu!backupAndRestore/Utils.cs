using System;
using System.Text;
using System.Timers;
using System.Globalization;
using System.Collections.Generic;

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
        SafeguardDeleteCmd
    }
}
