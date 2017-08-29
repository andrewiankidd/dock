using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO;

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
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }
        [DllImport("User32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("User32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // WinAPI UI control constants
        private const int HIDE = 0;
        private const int SHOW = 1;
        private const int MAX = 3;
        private const int RESTORE = 4;
        private const int SW_SHOW = 5;
        private const int MIN = 6;
        private const uint SEE_MASK_INVOKEIDLIST = 12;
        private const UInt32 WM_CLOSE = 0x0010;
        private const int WS_EX_APPWINDOW = 0x40000;
        private const int WS_EX_TOOLWINDOW = 0x0080;
        private const int GWL_EXSTYLE = -0x14;


        // Track open windows
        public static Dictionary<string, Dictionary<IntPtr, GroupBox>> openWindows = new Dictionary<string, Dictionary<IntPtr, GroupBox>>();
        public static Dictionary<string, GroupBox> pinnedWindows = new Dictionary<string, GroupBox>();
        public static List<string> filteredWindows = new List<string>();
        public static List<string> activePins = new List<string>();
        public static List<string> hoverIcons = new List<string>();

        // Timer for UIUpdates
        private static System.Windows.Forms.Timer tickHandler;
        private static double baseWidth, baseHeight;
        private static IntPtr lastHWnd;
        private static GroupBox lastIcon;

        // Application Data
        public static NameValueCollection AppSettings { get; set; }

        public dockForm()
        {
            AppSettings = ConfigurationManager.AppSettings;
            filteredWindows.Add("Windows Shell Experience Host");
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
            this.taskbarPanel.AllowDrop = true;

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
            bool iconsFromAllDisplays = Convert.ToBoolean(AppSettings["iconsFromAllDisplays"]);
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
                        // Check if process has valid window
                        if ((!String.IsNullOrEmpty(process.MainWindowTitle) && !filteredWindows.Contains(process.MainWindowTitle)) && IsWindow(process.MainWindowHandle) && (GetWindowLong(process.MainWindowHandle, GWL_EXSTYLE) & WS_EX_TOOLWINDOW) == 0)
                        {
                            // Check which screen/display it is on, show/hide depending on settings
                            if (iconsFromAllDisplays || (Screen.FromHandle(process.MainWindowHandle).DeviceName == Screen.PrimaryScreen.DeviceName && !iconsFromAllDisplays))
                            {
                               
                                if (!openWindows.ContainsKey(process.MainModule.FileName))
                                {
                                    lock (openWindows)
                                    {
                                        openWindows.Add(process.MainModule.FileName, new Dictionary<IntPtr, GroupBox>());
                                    }
                                }
                               

                                // Add this window if it is not already being tracked
                                if (!openWindows[process.MainModule.FileName].ContainsKey(process.MainWindowHandle))
                                {
                                    // prepare group
                                    GroupBox grp = new GroupBox();
                                    grp.Name = process.MainWindowHandle.ToString();
                                    grp.AccessibleName = process.MainModule.FileName.ToString();
                                    grp.Tag = process.MainWindowTitle;
                                    grp.Height = this.Height;
                                    grp.Width = this.Height;
                                    grp.MouseDown += (sender, e) =>
                                    {

                                    };
                                    grp.MouseEnter += (sender, e) =>
                                    {
                                        hoverIcons.Add(grp.Name);
                                    };
                                    grp.MouseLeave += (sender, e) =>
                                    {
                                        hoverIcons.Remove(grp.Name);
                                    };
                                    grp.Click += (sender, e) =>
                                    {
                                        MouseEventArgs me = (MouseEventArgs)e;
                                        lastHWnd = process.MainWindowHandle;
                                        lastIcon = grp;

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
                                        else if (me.Button == MouseButtons.Right)
                                        {
                                            cntxtIcon.Show(Cursor.Position);
                                        }
                                    };
                                    new ToolTip().SetToolTip(grp, process.MainWindowTitle);

                                    if (Convert.ToBoolean(AppSettings["iconLabels"]))
                                    {
                                        // Prepare label
                                        Label lbl = new Label();
                                        lbl.BackgroundImageLayout = ImageLayout.Stretch;
                                        lbl.TextAlign = ContentAlignment.MiddleLeft;
                                        lbl.Location = new Point(this.Height, 0);
                                        lbl.Text = process.MainWindowTitle;
                                        lbl.Name = "icoLabel";
                                        lbl.Height = this.Height;
                                        lbl.Enabled = false;
                                        grp.Width += 100;
                                        grp.Controls.Add(lbl);
                                    }

                                    // Build Icon as PictureBox Control
                                    PictureBox ico = new PictureBox();
                                    ico.Image = Bitmap.FromHicon(Icon.ExtractAssociatedIcon(process.MainModule.FileName).Handle);
                                    ico.BackgroundImageLayout = ImageLayout.Stretch;
                                    ico.SizeMode = PictureBoxSizeMode.CenterImage;
                                    ico.Height = this.Height;
                                    ico.Width = this.Height;                                   
                                    ico.Enabled = false;
                                    grp.Controls.Add(ico);

                                    lock (openWindows)
                                    {
                                        // Add to tracked windows
                                        openWindows[process.MainModule.FileName].Add(process.MainWindowHandle, grp);
                                    }
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
            int padding = Convert.ToInt32(AppSettings["iconSpacing"]);

            // Lock window list
            lock (openWindows)
            {
                // Check if each window is still valid
                foreach (KeyValuePair<string, Dictionary<IntPtr, GroupBox>> windowGroup in openWindows)
                {   
                    if (windowGroup.Value.Count > 0)
                    { 
                        foreach (KeyValuePair<IntPtr, GroupBox> entry in windowGroup.Value)
                        {

                            // If window is still valid and isnt already on the dock
                            if (IsWindow(entry.Key) && ! taskbarPanel.Controls.ContainsKey(entry.Key.ToString()))
                            {                              

                                // Adjust padding where possible
                                entry.Value.Margin = new Padding(padding, 0, padding, 0);
                             
                                // Add Icon to dock
                                taskbarPanel.Controls.Add(entry.Value);
                                
                                //                      
                                if (activePins.Contains(windowGroup.Key))
                                {
                                    activePins.Remove(windowGroup.Key);
                                    taskbarPanel.Controls.RemoveByKey(windowGroup.Key);                      
                                }                                
                            }
                            // Not a window (anymore) so remove it from the list and form
                            else if (!IsWindow(entry.Key))
                            {
                                taskbarPanel.Controls.Remove(entry.Value);
                                openWindows[windowGroup.Key].Remove(entry.Key);

                                // if notepad is pinned, but it is not active
                                if (pinnedWindows.ContainsKey(windowGroup.Key) && !activePins.Contains(windowGroup.Key))
                                {
                                    // Set it as active and add the pin to taskbar
                                    activePins.Add(windowGroup.Key);
                                    taskbarPanel.Controls.Add(pinnedWindows[windowGroup.Key]);
                                    GroupBox pin = pinnedWindows[lastIcon.AccessibleName];
                                    pin.Width = this.Height;
                                    if (pin.Controls.Count>1)
                                    {
                                        pin.Controls.RemoveByKey("icoLabel");
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }

            // Resize dock
            if (taskbarPanel.Controls.Count>1)
            {
                taskbarPanel.Width = taskbarPanel.Controls.Count * taskbarPanel.Controls[0].Width + ((padding * 2) * taskbarPanel.Controls.Count);
                foreach(GroupBox grp in taskbarPanel.Controls)
                {
                    // is window highlighted
                    Bitmap bgImg;
                    if (hoverIcons.Contains(grp.Name))
                    {
                        bgImg = new Bitmap(".\\Resources\\iconhighlight.png");
                    }
                    else if (grp.Name == GetForegroundWindow().ToString())
                    {
                        bgImg = new Bitmap(".\\Resources\\iconhighlight.png");
                    }
                    else if (activePins.Contains(grp.Name))
                    {
                        bgImg = null;
                    }
                    else
                    {
                        bgImg = new Bitmap(".\\Resources\\iconbg.png");
                    }

                    // Apply bg
                    foreach (Control child in grp.Controls)
                    {
                        child.BackgroundImage = bgImg;
                    }
                }
                // Check if active
                
            }

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
                center = (this.Width / 2) - taskbarPanel.Width/2;
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
            // Send Close comand to window via windows API
            SendMessage(lastHWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        private void cntxtIcon_properties(object sender, EventArgs e)
        {
            ShowFileProperties(lastIcon.AccessibleName);
        }

        private void cntxtIcon_pin(object sender, EventArgs e)
        {          
            // check if already pinned
            if (!pinnedWindows.ContainsKey(lastIcon.AccessibleName))
            {
                GroupBox entry = lastIcon;
                entry.Name = lastIcon.AccessibleName;
                entry.Click += (ssender, ee) =>
                {
                    MouseEventArgs me = (MouseEventArgs)ee;

                    if (me.Button == MouseButtons.Left)
                    {
                      Process.Start(entry.Name);
                              
                    }
                    else if (me.Button == MouseButtons.Right)
                    {
                        cntxtIcon.Show(Cursor.Position);
                    }
                };
                // Find the PictureBox control and store it as a pin
                pinnedWindows.Add(lastIcon.AccessibleName, lastIcon);
            }
        }

        private void cntxtIcon_unpin(object sender, EventArgs e)
        {
            // Check if icon is pinned
            if (pinnedWindows.ContainsKey(lastIcon.AccessibleName))
            {
                // Remove pin
                pinnedWindows.Remove(lastIcon.AccessibleName);
            }
        }

        private void cntxtIcon_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Find the icon that was right-clicked
            /*
            GroupBox tmp = lastIcon;
                  
            if (!IsWindow((IntPtr)Convert.ToInt32(tmp.Name)))
            {
                // Hide unrelated context menu items
                this.cntxtIconClose.Visible = false;
                this.cntxtIconPin.Visible = false;
                this.cntxtIconUnpin.Visible = true;
            }
            else
            {
                // Hide unrelated context menu items
                this.cntxtIconClose.Visible = true;
                if (!pinnedWindows.ContainsKey(lastIcon.AccessibleName))
                {
                    this.cntxtIconPin.Visible = true;
                }
                this.cntxtIconUnpin.Visible = false;
            }*/
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

        public static bool ShowFileProperties(string Filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            return ShellExecuteEx(ref info);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                Params.ExStyle |= 0x80;
                return Params;
            }
        }

  }
    
}
