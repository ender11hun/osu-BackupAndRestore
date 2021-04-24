using System;
using System.Windows.Forms;

namespace EnderCode.Utils
{
    /// <summary>
    ///     Form tricks for Console applications
    /// </summary>
    public static class FormImpl4Con
    {
        /// <summary>
        ///     Class implementing <see langword="interface"/> <see cref="IWin32Window"/>. This class cannot be inherited.
        /// </summary>
        public sealed class Win32Window : IWin32Window
        {

            IntPtr _handle;
            /// <summary>
            /// Konzol ablak handle
            /// </summary>
            public IntPtr Handle { get => _handle; }

            /// <exception cref="NullReferenceException"></exception>
            public Win32Window(IntPtr handle)
            {
                if (handle.Equals(IntPtr.Zero))
                    throw new NullReferenceException("The handle's pointer does not point to a valid handle.");
                _handle = handle;
            }
        }

        /// <summary>
        /// Könyvtár tallózó készítése konzolhoz beállítva
        /// </summary>
        /// <param name="dialogTitle">Párbeszéd ablak címe</param>
        /// <returns>Visszaad egy <see cref="FolderBrowserDialog"/> objektumot, amit újra fel lehet használni</returns>
        public static FolderBrowserDialog CreateFolderBrowser(string dialogTitle)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                Description = dialogTitle,
                RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = true
            };
            return dialog;
        }
    }
}
