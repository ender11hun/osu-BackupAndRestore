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
        /// <param name="dialogName">Párbeszéd ablak neve</param>
        /// <returns>Visszaad egy <see cref="FolderBrowserDialog"/> objektumot, amit újra fel lehet használni</returns>
        public static FolderBrowserDialog CreateFolderBrowser(string dialogName)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                Description = dialogName,
                RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = true
            };
            return dialog;
        }
    }
}
