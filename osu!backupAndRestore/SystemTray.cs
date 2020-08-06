using EnderCode.osuBackupAndRestore.Properties;
using EnderCode.Utils;
using System;
using System.ComponentModel;
using System.Windows.Forms;

#pragma warning disable IDE1006, CA2213 //<-- This sh*t pesters me to call dispose on components field... Take a look at my Dispose()

namespace EnderCode.osuBackupAndRestore
{
    class SystemTray : Form, IDisposable
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
                Icon = Resources.icon,
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
                components.Dispose();
            }

            base.Dispose(isDisposing);
        }
        private void trayIcon_Click(object Sender, EventArgs e)
        {
            Util.HideCurrentWindow(MainEntry.WindowHidden = !MainEntry.WindowHidden, MainEntry.WindowHandle);
        }
    }
}
