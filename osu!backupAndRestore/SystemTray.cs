using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using EnderCode.Utils;

#pragma warning disable IDE1006

namespace EnderCode.osu_backupAndRestore
{
    class SystemTray : Form
    {
        internal static SystemTray instance = new SystemTray();
        private NotifyIcon trayIcon;
        private IContainer components;

        SystemTray()
        {
            CreateNotifyicon();
        }

        private void CreateNotifyicon()
        {
            components = new Container();

            trayIcon = new NotifyIcon(components)
            {
                Icon = Properties.Resources.icon,
                Text = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).ProductName,
                Visible = true
            };

            trayIcon.Click += new EventHandler(trayIcon_Click);
        }
        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            base.OnLoad(e);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }
        private void trayIcon_Click(object Sender, EventArgs e)
        {
            Util.HideCurrentWindow(MainEntry.WindowHidden.Switch(), MainEntry.WindowHandle);
        }
    }
}
