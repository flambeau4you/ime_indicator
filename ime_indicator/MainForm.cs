using Microsoft.Uii.Csr;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ime_indicator
{
    public partial class MainForm : Form
    {
        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref MainForm.COPYDATASTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        private const int WM_IME_CONTROL = 643;
        private const int IMC_GETOPENSTATUS = 5;

        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;

            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        public MainForm()
        {
            InitializeComponent();
        }
        private string GetActiveWindowTitle()
        {
            StringBuilder stringBuilder = new StringBuilder(256);
            IntPtr foregroundWindow = MainForm.GetForegroundWindow();
            bool flag = MainForm.GetWindowText(foregroundWindow, stringBuilder, 256) > 0;
            string result;
            if (flag)
            {
                result = stringBuilder.ToString();
            }
            else
            {
                result = null;
            }
            return result;
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            base.TopLevel = true;
            base.TopMost = true;
            base.FormBorderStyle = FormBorderStyle.None;

            int sizePx;
            if (appSettings["SizePx"] == null)
            {
                sizePx = 2;
            }
            else
            {
                sizePx = Convert.ToInt32(appSettings["SizePx"]);
            }
            
            if (appSettings["Position"] == "left")
            {
                base.Left = 0;
                base.Top = 0;
                base.Width = sizePx;
                base.Height = this.GetScreenHeight();
            }
            else if (appSettings["Position"] == "right")
            {
                base.Left = this.GetScreenWidth() - sizePx;
                base.Top = 0;
                base.Width = sizePx;
                base.Height = this.GetScreenHeight();
            } else if (appSettings["Position"] == "bottom")
            {
                base.Left = 0;
                base.Top = this.GetScreenHeight() - sizePx;
                base.Width = this.GetScreenWidth();
                base.Height = sizePx;
            }
            else
            {
                base.Left = 0;
                base.Top = 0;
                base.Width = this.GetScreenWidth();
                base.Height = sizePx;
            }
            this.timLanguage.Enabled = true;
        }

        private int GetScreenWidth()
        {
            return Screen.FromControl(this).Bounds.Width;
        }

        private int GetScreenHeight()
        {
            return Screen.FromControl(this).Bounds.Height;
        }

        private void timLanguage_Tick(object sender, EventArgs e)
        {
            // Makes this window is topmost.
            base.TopLevel = true;
            base.TopMost = true;

            // Finds active window.
            IntPtr foregroundWindow = MainForm.GetForegroundWindow();
            IntPtr hWnd = MainForm.ImmGetDefaultIMEWnd(foregroundWindow);
            string activeWindowTitle = this.GetActiveWindowTitle();
            MainForm.COPYDATASTRUCT copydatastruct = default(MainForm.COPYDATASTRUCT);

            // Gets Caps lock state.
            bool capsLock = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;

            // Gets the IME state.
            IntPtr value = MainForm.SendMessage(hWnd, WM_IME_CONTROL, IMC_GETOPENSTATUS, ref copydatastruct);
            bool flag = value == IntPtr.Zero;
            String onColor = appSettings["OnColor"];
            String offColor = appSettings["OffColor"];
            if (flag)
            {
                // IME is off. It's English mode.
                if (onColor != null)
                {
                    this.BackColor = System.Drawing.ColorTranslator.FromHtml(offColor);
                }
                else
                {
                    this.BackColor = Color.Red;
                }
            }
            else
            {
                // IME is on. So, it's not English mode.
                if (offColor != null)
                {
                    this.BackColor = System.Drawing.ColorTranslator.FromHtml(onColor);
                }
                else
                {
                    this.BackColor = Color.Blue;
                }
            }

            if (capsLock)
            {
                this.Visible = !this.Visible;
            } 
            else
            {
                this.Visible = true;
            }

        }

        protected override CreateParams CreateParams
        {
            get
            {
                // Hide this when alt+tab.
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
    }
}
