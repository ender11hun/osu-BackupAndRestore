using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnderCode.Utils
{
    public static class FormImpl4Con
    {
        public sealed class Win32Window : IWin32Window
        {
            /// <summary>
            /// Konzol ablak handle
            /// </summary>
            public IntPtr Handle { get; private set; }

            /// <exception cref="NullReferenceException"></exception>
            public Win32Window(IntPtr handle)
            {
                if (handle.Equals(IntPtr.Zero))
                    throw new NullReferenceException("The pointer does not point to a valid handle.");
                Handle = handle;
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
