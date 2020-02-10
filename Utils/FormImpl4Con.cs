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
