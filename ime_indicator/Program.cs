using ime_indicator.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ime_indicator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Prevents duplicated execution.
            String thisprocessname = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (NotifyIcon notifyIcon = new NotifyIcon())
            {
                notifyIcon.Text = "IME Indicator";
                notifyIcon.Icon = Resources.main_icon;
                notifyIcon.MouseClick += Program.Noti_MouseClick;
                notifyIcon.Visible = true;
                Application.Run(new MainForm());
            }
        }

        private static void Noti_MouseClick(object sender, MouseEventArgs e)
        {
            bool flag = e.Button == MouseButtons.Left;
            if (flag)
            {
                Application.Exit();
            }
        }
    }
}
