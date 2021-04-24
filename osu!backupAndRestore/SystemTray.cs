using EnderCode.osuBackupAndRestore.Properties;
using EnderCode.Utils;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EnderCode.osuBackupAndRestore
{
    [DesignerCategory("")]
    class SystemTray : Form, IDisposable
    {
        internal static SystemTray instance = new SystemTray();
        private NotifyIcon trayIcon;
        private IContainer components;

        SystemTray()
        {
            CreateNotifyicon();
        }

        ~SystemTray()
        {
           Dispose(true);
        }

        private void CreateNotifyicon()
        {
            components = new Container();

            trayIcon = new NotifyIcon(components)
            {
                Icon = Resources.icon,
                Text = CoreAssembly.Name.Name,
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
