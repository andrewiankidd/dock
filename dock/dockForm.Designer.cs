﻿namespace dock
{
    partial class dockForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.taskbarPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.iconFocus = new System.Windows.Forms.PictureBox();
            this.cntxtIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cntxtIconPin = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtIconUnpin = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtIconProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtIconClose = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtDock = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cntxtDockTaskManager = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtDockExit = new System.Windows.Forms.ToolStripMenuItem();
            this.trayPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cntxtIconMinimize = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtIconMaximize = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtIconRestore = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.iconFocus)).BeginInit();
            this.cntxtIcon.SuspendLayout();
            this.cntxtDock.SuspendLayout();
            this.SuspendLayout();
            // 
            // taskbarPanel
            // 
            this.taskbarPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.taskbarPanel.Location = new System.Drawing.Point(0, 0);
            this.taskbarPanel.Name = "taskbarPanel";
            this.taskbarPanel.Size = new System.Drawing.Size(655, 50);
            this.taskbarPanel.TabIndex = 0;
            this.taskbarPanel.WrapContents = false;
            this.taskbarPanel.Click += new System.EventHandler(this.dockForm_Click);
            // 
            // iconFocus
            // 
            this.iconFocus.Location = new System.Drawing.Point(0, 0);
            this.iconFocus.Name = "iconFocus";
            this.iconFocus.Size = new System.Drawing.Size(50, 50);
            this.iconFocus.TabIndex = 0;
            this.iconFocus.TabStop = false;
            this.iconFocus.Visible = false;
            // 
            // cntxtIcon
            // 
            this.cntxtIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cntxtIconRestore,
            this.cntxtIconMaximize,
            this.cntxtIconMinimize,
            this.toolStripSeparator1,
            this.cntxtIconPin,
            this.cntxtIconUnpin,
            this.cntxtIconProperties,
            this.cntxtIconClose});
            this.cntxtIcon.Name = "cntxtIcon";
            this.cntxtIcon.Size = new System.Drawing.Size(181, 186);
            this.cntxtIcon.Opening += new System.ComponentModel.CancelEventHandler(this.cntxtIcon_Opening);
            // 
            // cntxtIconPin
            // 
            this.cntxtIconPin.Name = "cntxtIconPin";
            this.cntxtIconPin.Size = new System.Drawing.Size(180, 22);
            this.cntxtIconPin.Text = "Pin";
            this.cntxtIconPin.Click += new System.EventHandler(this.cntxtIcon_pin);
            // 
            // cntxtIconUnpin
            // 
            this.cntxtIconUnpin.Name = "cntxtIconUnpin";
            this.cntxtIconUnpin.Size = new System.Drawing.Size(180, 22);
            this.cntxtIconUnpin.Text = "Unpin";
            this.cntxtIconUnpin.Click += new System.EventHandler(this.cntxtIcon_unpin);
            // 
            // cntxtIconProperties
            // 
            this.cntxtIconProperties.Name = "cntxtIconProperties";
            this.cntxtIconProperties.Size = new System.Drawing.Size(180, 22);
            this.cntxtIconProperties.Text = "Properties";
            this.cntxtIconProperties.Click += new System.EventHandler(this.cntxtIcon_properties);
            // 
            // cntxtIconClose
            // 
            this.cntxtIconClose.Name = "cntxtIconClose";
            this.cntxtIconClose.Size = new System.Drawing.Size(180, 22);
            this.cntxtIconClose.Text = "Close";
            this.cntxtIconClose.Click += new System.EventHandler(this.cntxtIcon_close);
            // 
            // cntxtDock
            // 
            this.cntxtDock.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cntxtDockTaskManager,
            this.cntxtDockExit});
            this.cntxtDock.Name = "cntxtDock";
            this.cntxtDock.Size = new System.Drawing.Size(148, 48);
            // 
            // cntxtDockTaskManager
            // 
            this.cntxtDockTaskManager.Name = "cntxtDockTaskManager";
            this.cntxtDockTaskManager.Size = new System.Drawing.Size(147, 22);
            this.cntxtDockTaskManager.Text = "Task Manager";
            this.cntxtDockTaskManager.Click += new System.EventHandler(this.cntxtDock_taskManager);
            // 
            // cntxtDockExit
            // 
            this.cntxtDockExit.Name = "cntxtDockExit";
            this.cntxtDockExit.Size = new System.Drawing.Size(147, 22);
            this.cntxtDockExit.Text = "Exit Dock";
            this.cntxtDockExit.Click += new System.EventHandler(this.cntxtDock_exit);
            // 
            // trayPanel
            // 
            this.trayPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trayPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.trayPanel.Location = new System.Drawing.Point(0, 0);
            this.trayPanel.Name = "trayPanel";
            this.trayPanel.Size = new System.Drawing.Size(655, 50);
            this.trayPanel.TabIndex = 2;
            this.trayPanel.WrapContents = false;
            this.trayPanel.Click += new System.EventHandler(this.dockForm_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // cntxtIconMinimize
            // 
            this.cntxtIconMinimize.Name = "cntxtIconMinimize";
            this.cntxtIconMinimize.Size = new System.Drawing.Size(180, 22);
            this.cntxtIconMinimize.Text = "Minimize";
            this.cntxtIconMinimize.Click += new System.EventHandler(this.cntxtIconMinimize_Click);
            // 
            // cntxtIconMaximize
            // 
            this.cntxtIconMaximize.Name = "cntxtIconMaximize";
            this.cntxtIconMaximize.Size = new System.Drawing.Size(180, 22);
            this.cntxtIconMaximize.Text = "Maximize";
            this.cntxtIconMaximize.Click += new System.EventHandler(this.cntxtIconMaximize_Click);
            // 
            // cntxtIconRestore
            // 
            this.cntxtIconRestore.Name = "cntxtIconRestore";
            this.cntxtIconRestore.Size = new System.Drawing.Size(180, 22);
            this.cntxtIconRestore.Text = "Restore";
            this.cntxtIconRestore.Click += new System.EventHandler(this.cntxtIconRestore_Click);
            // 
            // dockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 50);
            this.Controls.Add(this.taskbarPanel);
            this.Controls.Add(this.trayPanel);
            this.Controls.Add(this.iconFocus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "dockForm";
            this.Text = "dock";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.dockForm_FormClosing);
            this.Load += new System.EventHandler(this.dockForm_Load);
            this.Click += new System.EventHandler(this.dockForm_Click);
            ((System.ComponentModel.ISupportInitialize)(this.iconFocus)).EndInit();
            this.cntxtIcon.ResumeLayout(false);
            this.cntxtDock.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel taskbarPanel;
        private System.Windows.Forms.PictureBox iconFocus;
        private System.Windows.Forms.ContextMenuStrip cntxtIcon;
        private System.Windows.Forms.ToolStripMenuItem cntxtIconPin;
        private System.Windows.Forms.ToolStripMenuItem cntxtIconClose;
        private System.Windows.Forms.ToolStripMenuItem cntxtIconUnpin;
				private System.Windows.Forms.ContextMenuStrip cntxtDock;
				private System.Windows.Forms.ToolStripMenuItem cntxtDockTaskManager;
				private System.Windows.Forms.ToolStripMenuItem cntxtDockExit;
        private System.Windows.Forms.ToolStripMenuItem cntxtIconProperties;
		private System.Windows.Forms.FlowLayoutPanel trayPanel;
        private System.Windows.Forms.ToolStripMenuItem cntxtIconRestore;
        private System.Windows.Forms.ToolStripMenuItem cntxtIconMaximize;
        private System.Windows.Forms.ToolStripMenuItem cntxtIconMinimize;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

