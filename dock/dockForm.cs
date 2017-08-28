using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace dock
{
    public partial class dockForm : Form
    {
        // WinAPI Callbacks
        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        private static extern int ToggleWindow(int hwnd, int command);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        // WinAPI UI control constants
        private const int HIDE = 0;
        private const int SHOW = 1;
        private const int MAX = 3;
        private const int RESTORE = 4;
        private const int MIN = 6;
        private const UInt32 WM_CLOSE = 0x0010;

        // Track open windows
        public static Dictionary<IntPtr, PictureBox> openWindows = new Dictionary<IntPtr, PictureBox>();
        public static Dictionary<IntPtr, PictureBox> pinnedWindows = new Dictionary<IntPtr, PictureBox>();
        public static List<string> hoverIcons = new List<string>();

        // Timer for UIUpdates
        private static System.Windows.Forms.Timer tickHandler;
        private static double baseWidth, baseHeight;
        private static IntPtr lastHWnd;
        private static PictureBox lastIcon;

        // Application Data
        public static NameValueCollection AppSettings { get; set; }

        public dockForm()
        {
            AppSettings = ConfigurationManager.AppSettings;
            InitializeComponent();
        }

        private void dockForm_Load(object sender, EventArgs e)
        {
            // Set up dockWidth
            if (AppSettings["dockWidth"].Contains("%"))
            {
                double sWidth = Screen.PrimaryScreen.Bounds.Width;
                double cPerc = Convert.ToInt32(AppSettings["dockWidth"].Replace("%", string.Empty));
                baseWidth = (sWidth * (cPerc / 100));
                this.Width = (int)baseWidth;
            }
            else
            {
                this.Width = Convert.ToInt32(AppSettings["dockWidth"]);
            }

            // Set up dockHeight
            if (AppSettings["dockHeight"].Contains("%"))
            {
                double sHeight = Screen.PrimaryScreen.Bounds.Height;
                double cPerc = Convert.ToInt32(AppSettings["dockHeight"].Replace("%", string.Empty));
                baseHeight = (sHeight * (cPerc / 100));
                this.Height = (int)baseHeight;
            }
            else
            {
                this.Height = Convert.ToInt32(AppSettings["dockHeight"]);
            }

            // Set up dockColor
            if (AppSettings["dockColor"].Contains("#"))
            {
                this.BackColor = ColorTranslator.FromHtml(AppSettings["dockColor"]);
            }
            else
            {
                this.BackColor = Color.FromName(AppSettings["dockColor"]);
            }

            // Set up dockOpacity
            if (AppSettings["dockOpacity"].Contains("%")){
                this.Opacity = Double.Parse(AppSettings["dockOpacity"].Replace("%", string.Empty)) / 100;
            }

            // Set up centering
            if (Convert.ToBoolean(AppSettings["dockCentered"]))
            {
                int sWidth = Screen.PrimaryScreen.Bounds.Width;
                int dWidth = this.taskbarPanel.Width;
                int diff = (sWidth - dWidth) / 2;
                this.Location = new Point(diff, Screen.PrimaryScreen.Bounds.Height - this.Height);
            }
            else
            {
                this.Location = new Point(0, Screen.PrimaryScreen.Bounds.Height - this.Height);
            }

            this.TopMost = true;
            this.ShowInTaskbar = false;

            // Hide windows taskbar
            toggleTaskbar(HIDE);

            // Start polling windows
            getOpenWindows();

            //Prepare iconFocus
            iconFocus.Image = new Bitmap(".\\Resources\\iconbg.png");

            // Start UI monitor
            tickHandler = new System.Windows.Forms.Timer();
            tickHandler.Tick += new EventHandler(UpdateDockUI);
            tickHandler.Interval = 10;
            tickHandler.Start();
        }

      private void dockForm_Click(object sender, EventArgs e)
      {
            MouseEventArgs me = (MouseEventArgs)e;

            if (me.Button == MouseButtons.Right)
            {
              cntxtDock.Show(Cursor.Position);
            }
      }

      public void getOpenWindows()
      {
            // Get running windows
            new Thread(() =>
            {
                while (this.Visible)
                {
                    // Retreive window information
                    Thread.CurrentThread.IsBackground = true;
                    Process[] processlist = Process.GetProcesses();
                    foreach (Process process in processlist)
                    {
                        if (!String.IsNullOrEmpty(process.MainWindowTitle))
                        {
                            if (!openWindows.ContainsKey(process.MainWindowHandle))
                            {
                                // Build Control
                                PictureBox tmp = new PictureBox();
                                tmp.Image = Bitmap.FromHicon(Icon.ExtractAssociatedIcon(process.MainModule.FileName).Handle);
                                new ToolTip().SetToolTip(tmp, process.MainWindowTitle);
                                tmp.Name = process.MainWindowHandle.ToString();
                                tmp.AccessibleName = process.MainModule.FileName.ToString();
                                tmp.SizeMode = PictureBoxSizeMode.CenterImage;
                                tmp.Height = this.Height;
                                tmp.Width = this.Height;
                                tmp.BackgroundImageLayout = ImageLayout.Stretch;
                                tmp.MouseEnter+= (sender, e) =>
                                {
                                  hoverIcons.Add(tmp.Name);
                                };
                                tmp.MouseLeave += (sender, e) =>
                                {
                                  hoverIcons.Remove(tmp.Name);
                                };
                                tmp.Click += (sender, e) =>
                                {
                                    MouseEventArgs me = (MouseEventArgs)e;
                                    lastHWnd = process.MainWindowHandle;
                                    lastIcon = tmp;

                                    if (me.Button == MouseButtons.Left)
                                    {                                        
                                        if (IsIconic(lastHWnd))
                                        {
                                            SetForegroundWindow(lastHWnd);
                                            ToggleWindow((int)lastHWnd, RESTORE);
                                        }
                                        else
                                        {
                                            ToggleWindow((int)lastHWnd, MIN);
                                        }
                                    }
                                    else if(me.Button == MouseButtons.Right)
                                    {
                                        cntxtIcon.Show(Cursor.Position);
                                    }
                                };

                                lock (openWindows)
                                {
                                    // Add to tracked windows
                                    openWindows.Add(process.MainWindowHandle, tmp);
                                }
                            }
                        }
                    }
                }
            }).Start();
        }

        public void UpdateDockUI(object sender, EventArgs e)
        {
            // Suspend the dock layout
            taskbarPanel.SuspendLayout();

            // Get Spacing Config
            int padding = Convert.ToInt32(AppSettings["IconSpacing"]);

            // Lock window list
            lock (openWindows)
            {
                // Check if each window is still valid
                foreach (KeyValuePair<IntPtr, PictureBox> entry in openWindows)
                {
                    // If window is still valid and isnt already on the dock
                    if (IsWindow(entry.Key) && !this.Controls.ContainsKey(entry.Key.ToString()))
                    {            
                        // Adjust padding where possible
                        entry.Value.Margin = new Padding(padding, 0, padding, 0);

                        // Check if active
                        if (entry.Value.Name == GetForegroundWindow().ToString())
                        {
                            entry.Value.BackgroundImage = new Bitmap(".\\Resources\\iconhighlight.png");
                        }
                        else
                        {
                            entry.Value.BackgroundImage = new Bitmap(".\\Resources\\iconbg.png");
                        }

                        // Add Icon to dock
                        taskbarPanel.Controls.Add(entry.Value);
                    }
                    else
                    {
                        if (!pinnedWindows.ContainsValue(entry.Value) || entry.Value.BackgroundImage != null )
                        {
                            // Remove Icon from dock
                            taskbarPanel.Controls.Remove(entry.Value);
                        }                       
                    }
                    
                    // is window highlighted
                    if (hoverIcons.Contains(entry.Value.Name)){
                        entry.Value.BackgroundImage = new Bitmap(".\\Resources\\iconhighlight.png");
                    }
                }
            }

            // Lock window list
            lock (pinnedWindows)
            {
                // Check if each window is still valid
                foreach (KeyValuePair<IntPtr, PictureBox> entry in pinnedWindows)
                {
                    // If window is still valid and isnt already on the dock
                    if (!this.taskbarPanel.Controls.ContainsKey(entry.Key.ToString()))
                    {
                        // Adjust padding where possible
                        entry.Value.Margin = new Padding(padding, 0, padding, 0);
                        entry.Value.BackgroundImage = null;
                        entry.Value.Click += (ssender, ee) =>
                        {
                            MouseEventArgs me = (MouseEventArgs)ee;

                            if (me.Button == MouseButtons.Left)
                            {
                                Process.Start(entry.Value.AccessibleName);
                            }
                            else if (me.Button == MouseButtons.Right)
                            {
                                cntxtIcon.Show(Cursor.Position);
                            }
                        };

                        // Add Icon to dock
                        taskbarPanel.Controls.Add(entry.Value);
                    }
                }
            }

            // Resize dock
            taskbarPanel.Width = (taskbarPanel.Controls.Count * this.Height) + ((padding*2) * taskbarPanel.Controls.Count);

            // If config is set to enable centering, calculate center
            if (Convert.ToBoolean(Convert.ToBoolean(AppSettings["dockCentered"])))
            {
                // def center 
                int center = 0;

                // Center dock to screen
                int sWidth = Screen.PrimaryScreen.Bounds.Width;
                int dWidth = this.Width;
                int diff = (sWidth - dWidth) / 2;
                this.Location = new Point(diff, Screen.PrimaryScreen.Bounds.Height - this.Height);

                // Center icons to dock
                center = (this.Width / 2) - ((taskbarPanel.Controls.Count * this.Height) / 2) - (padding * taskbarPanel.Controls.Count);
                taskbarPanel.Location = new Point(center, 0);               
            }

            // Resume the dock layout            
            taskbarPanel.ResumeLayout();

            // Allow for growth
            if (taskbarPanel.Width > (int)baseWidth)
            { this.Width = taskbarPanel.Width; }
        }

        public void dockForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Re-open windows taskbar
            toggleTaskbar(SHOW);
        }

        private void cntxtIcon_close(object sender, EventArgs e)
        {
            SendMessage(lastHWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        private void cntxtIcon_pin(object sender, EventArgs e)
        {          
            if (!pinnedWindows.ContainsKey(lastHWnd))
            {
                PictureBox tmp = (PictureBox)this.Controls.Find(lastHWnd.ToString(), true)[0];
                pinnedWindows.Add(lastHWnd, tmp);
            }
        }

        private void cntxtIcon_unpin(object sender, EventArgs e)
        {
            if (pinnedWindows.ContainsKey(lastHWnd))
            {
                pinnedWindows.Remove(lastHWnd);
            }
        }

        private void cntxtIcon_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PictureBox tmp = (PictureBox)this.Controls.Find(lastHWnd.ToString(), true)[0];
            if (tmp.BackgroundImage==null)
            {
                this.cntxtIconClose.Visible = false;
                this.cntxtIconPin.Visible = false;
                this.cntxtIconUnpin.Visible = true;
            }
            else
            {
                this.cntxtIconClose.Visible = true;
                if (!pinnedWindows.ContainsKey(lastHWnd))
                {
                    this.cntxtIconPin.Visible = true;
                }
                this.cntxtIconUnpin.Visible = false;

            }
        }

    private void cntxtDock_exit(object sender, EventArgs e)
    {
      this.Close();
    }

    private void cntxtDock_taskManager(object sender, EventArgs e)
    {
      Process.Start("taskmgr");
    }

    private void toggleTaskbar(int action)
        {
            // Find Taskbar window handle
            int hwnd = FindWindow("Shell_TrayWnd", "");
            // Hide Window
            ToggleWindow(hwnd, action);
        }

    }
}
