using System;
using System.Runtime.InteropServices;
using static EnderCode.Utils.Interop.ENUMS;

namespace EnderCode.Utils
{

    /// <summary>
    /// Static p/invoke class for unmanaged dll calls
    /// </summary>
    public static class Interop
    {
        /// <summary>
        /// Static class for interop enums
        /// </summary>
        public static class ENUMS
        {
            /// <summary>
            /// Win32 kernel32 - Window state enum commands
            /// </summary>
            public enum ShowWindowEnum : uint
            {
                /// <summary>
                ///     Hides the window and activates another window.
                /// </summary>
                SW_HIDE = 0,

                /// <summary>
                ///     Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
                /// </summary>
                SW_SHOWNORMAL = 1,

                /// <summary>
                ///     Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
                /// </summary>
                SW_NORMAL = 1,

                /// <summary>
                ///     Activates the window and displays it as a minimized window.
                /// </summary>
                SW_SHOWMINIMIZED = 2,

                /// <summary>
                ///     Activates the window and displays it as a maximized window.
                /// </summary>
                SW_SHOWMAXIMIZED = 3,

                /// <summary>
                ///     Maximizes the specified window.
                /// </summary>
                SW_MAXIMIZE = 3,

                /// <summary>
                ///     Displays a window in its most recent size and position. This value is similar to <c><see cref="SW_SHOWNORMAL"/></c>, except the window is not activated.
                /// </summary>
                SW_SHOWNOACTIVATE = 4,

                /// <summary>
                ///     Activates the window and displays it in its current size and position.
                /// </summary>
                SW_SHOW = 5,

                /// <summary>
                ///     Minimizes the specified window and activates the next top-level window in the z-order.
                /// </summary>
                SW_MINIMIZE = 6,

                /// <summary>
                ///     Displays the window as a minimized window. This value is similar to <c><see cref="SW_SHOWMINIMIZED"/></c>, except the window is not activated.
                /// </summary>
                SW_SHOWMINNOACTIVE = 7,

                /// <summary>
                ///     Displays the window in its current size and position. This value is similar to <c><see cref="SW_SHOW"/></c>, except the window is not activated.
                /// </summary>
                SW_SHOWNA = 8,

                /// <summary>
                ///     Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
                /// </summary>
                SW_RESTORE = 9,

                /// <summary>
                ///     Makes sure the window is restored prior to showing, then activating.
                /// </summary>
                SW_SHOWDEFAULT = 10,

                /// <summary>
                /// Tries to coerce a window to minimized or maximized.
                /// </summary>
                SW_FORCEMINIMIZE = 11,

                /// <inheritdoc cref="SW_FORCEMINIMIZE"/>
                SW_MAX = 11

            }

            /// <summary>
            /// Win32 API kernel32 - <c><see cref="EXECUTION_STATE"/></c> enum flags for the <c><see cref="Interop.SetThreadExecutionState(EXECUTION_STATE)"/></c> method
            /// </summary>
            [Flags]
            public enum EXECUTION_STATE : uint
            {
                /// <summary>
                /// Set the execution state to Away mode
                /// <para>Note: the mode must be explicitly allowed by the current power policy</para>
                /// </summary>
                ES_AWAYMODE_REQUIRED    = 0x00000040,
                /// <summary>
                /// Set the execution state to Continuous
                /// <para>Using this flag makes the other flags' effect to last indefinitely</para>
                /// </summary>
                ES_CONTINUOUS           = 0x80000000,
                /// <summary>
                /// Set the executions state to Display Required
                /// <para>If this flag is set, the display won't turn off</para>
                /// </summary>
                ES_DISPLAY_REQUIRED     = 0x00000002,
                /// <summary>
                /// Set the executions state to System Required
                /// <para>This will prevent the system from Idle-to-Sleep</para>
                /// </summary>
                ES_SYSTEM_REQUIRED      = 0x00000001,
                ///<summary>
                /// Legacy flag (DO NOT USE!)
                /// <para>Original value: 0x00000004</para>
                ///</summary>
                ES_USER_PRESENT = 0x00000000
            }

        }

        /// <summary>
        /// Win32 API user32 - Set window's state to one of the <c><see cref="ShowWindowEnum"/></c>'s values
        /// </summary>
        /// <param name="hWnd">The handle to the window</param>
        /// <param name="flag">Flag to be applied to the window</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flag);

        /// <summary>
        /// Win32 API user32 - Set a window to active foreground
        /// </summary>
        /// <param name="hwnd">Handle to the window</param>
        /// <returns>The HRESULT of the operation marshaled as <c><see cref="int"/></c></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        /// <summary>
        /// Win32 API kernel32 - Get the current thread/process' handle to its console window
        /// </summary>
        /// <returns>Window handle to the current console window</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// Win32 API kernel32 - Set the current thread's execution state to one or a combination of <c><see cref="EXECUTION_STATE"/></c> flag(s)
        /// </summary>
        /// <param name="esFlags">Flags to be applied</param>
        /// <returns>The previous exec. state of thread</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
    }
}
