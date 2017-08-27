namespace dock
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
            this.cntxtIconClose = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.iconFocus)).BeginInit();
            this.cntxtIcon.SuspendLayout();
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
            this.cntxtIconPin,
            this.cntxtIconUnpin,
            this.cntxtIconClose});
            this.cntxtIcon.Name = "cntxtIcon";
            this.cntxtIcon.Size = new System.Drawing.Size(107, 70);
            this.cntxtIcon.Opening += new System.ComponentModel.CancelEventHandler(this.cntxtIcon_Opening);
            // 
            // cntxtIconPin
            // 
            this.cntxtIconPin.Name = "cntxtIconPin";
            this.cntxtIconPin.Size = new System.Drawing.Size(106, 22);
            this.cntxtIconPin.Text = "Pin";
            this.cntxtIconPin.Click += new System.EventHandler(this.cntxtIcon_pin);
            // 
            // cntxtIconUnpin
            // 
            this.cntxtIconUnpin.Name = "cntxtIconUnpin";
            this.cntxtIconUnpin.Size = new System.Drawing.Size(106, 22);
            this.cntxtIconUnpin.Text = "Unpin";
            this.cntxtIconUnpin.Click += new System.EventHandler(this.cntxtIcon_unpin);
            // 
            // cntxtIconClose
            // 
            this.cntxtIconClose.Name = "cntxtIconClose";
            this.cntxtIconClose.Size = new System.Drawing.Size(106, 22);
            this.cntxtIconClose.Text = "Close";
            this.cntxtIconClose.Click += new System.EventHandler(this.cntxtIcon_close);
            // 
            // dockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 50);
            this.Controls.Add(this.iconFocus);
            this.Controls.Add(this.taskbarPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "dockForm";
            this.Text = "dock";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.dockForm_FormClosing);
            this.Load += new System.EventHandler(this.dockForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.iconFocus)).EndInit();
            this.cntxtIcon.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel taskbarPanel;
        private System.Windows.Forms.PictureBox iconFocus;
        private System.Windows.Forms.ContextMenuStrip cntxtIcon;
        private System.Windows.Forms.ToolStripMenuItem cntxtIconPin;
        private System.Windows.Forms.ToolStripMenuItem cntxtIconClose;
        private System.Windows.Forms.ToolStripMenuItem cntxtIconUnpin;
    }
}

