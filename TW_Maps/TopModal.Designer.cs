using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;

namespace TW_Maps
{
    partial class TopModal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        const int baseClientWidth = 350;
        const int baseClientHeight = 200;
        bool fixedLocation = false;
        bool disabledShortcut = false;
        private ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

        // create scale list
        static List<double> scales = new List<double>() { 0.5, 0.75, 1.0, 1.5, 2.0 };
        static List<Keys> primaryShortcuts = new List<Keys>() { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5 };
        static List<Keys> secondaryShortcuts = new List<Keys>() { Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5};

        /// <summary>
        ///  Clean up any resources being used.
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

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // TopModal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.BlackholeMap_shortcut;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size((int)(baseClientWidth), (int)(baseClientHeight));
            ControlBox = false;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            KeyPreview = true;
            Name = "TopModal";
            Text = "TW_Maps";
            ResumeLayout(false);

            MouseDown += TopModal_MouseDown;
            KeyDown += TopModal_KeyDown;

            //// Define Sizes
            //clientSizes.AddRange(scales.Select(k => new Size((int)(baseClientWidth * k), (int)(baseClientHeight * k))));

            // Context menu
            contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(cms_Opening);
            ContextMenuStrip = contextMenuStrip;

        }

        private void cms_Opening(object sender, CancelEventArgs e)
        {
            Control c = contextMenuStrip.SourceControl as Control;
            contextMenuStrip.Items.Clear();
            if (c != null)
            {
                ToolStripMenuItem toolStripMenuItemFixLocation = new ToolStripMenuItem("위치 고정");
                toolStripMenuItemFixLocation.Click += (s, e) => { fixedLocation = !fixedLocation; };
                toolStripMenuItemFixLocation.Checked = fixedLocation;
                contextMenuStrip.Items.Add(toolStripMenuItemFixLocation);

                ToolStripMenuItem toolStripMenuItemDisabledShortcut = new ToolStripMenuItem("단축키 비활성화");
                toolStripMenuItemDisabledShortcut.Click += (s, e) => { disabledShortcut = !disabledShortcut; };
                toolStripMenuItemDisabledShortcut.Checked = disabledShortcut;
                contextMenuStrip.Items.Add(toolStripMenuItemDisabledShortcut);

                contextMenuStrip.Items.Add(new ToolStripSeparator());
                for (int k = 0; k < scales.Count; k++)
                {
                    double scale = scales[k];
                    string txt = "크기 " + ((int)(scale * 100)).ToString() + "%";
                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(txt);
                    toolStripMenuItem.ShortcutKeyDisplayString = primaryShortcuts[k].ToString().Replace("D", "") + " 또는 " + secondaryShortcuts[k].ToString();
                    toolStripMenuItem.Click += (s, e) =>
                    {
                        ClientSize = new Size((int)(baseClientWidth * scale), (int)(baseClientHeight * scale));
                    };
                    contextMenuStrip.Items.Add(toolStripMenuItem);
                }

                contextMenuStrip.Items.Add(new ToolStripSeparator());
                ToolStripMenuItem toolStripMenuItemClose = new ToolStripMenuItem("종료");
                toolStripMenuItemClose.Click += (s, e) => { Close(); };
                toolStripMenuItemClose.ShortcutKeyDisplayString = "Esc";
                contextMenuStrip.Items.Add(toolStripMenuItemClose);
            }
            e.Cancel = false;
        }

        // Snap function
        private const int SnapDist = 75;
        private bool DoSnap(int pos, int edge)
        {
            int delta = pos - edge;
            return delta > 0 && delta <= SnapDist;
        }
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            Screen scn = Screen.FromPoint(this.Location);
            if (DoSnap(this.Left, scn.WorkingArea.Left)) this.Left = scn.WorkingArea.Left;
            if (DoSnap(this.Top, scn.WorkingArea.Top)) this.Top = scn.WorkingArea.Top;
            if (DoSnap(scn.WorkingArea.Right, this.Right)) this.Left = scn.WorkingArea.Right - this.Width;
            if (DoSnap(scn.WorkingArea.Bottom, this.Bottom)) this.Top = scn.WorkingArea.Bottom - this.Height;
        }

        // Draggable window
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void TopModal_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (!fixedLocation)
                    {
                        ReleaseCapture();
                        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                    }
                    break;
            }
        }

        // KeyDown event
        private void TopModal_KeyDown(object sender,  KeyEventArgs e)
        {
            double scale = 1.0;
            if (!disabledShortcut)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        Close();
                        break;
                    case Keys.D1:
                    case Keys.NumPad1:
                        scale = scales[0];
                        ClientSize = new Size((int)(baseClientWidth * scale), (int)(baseClientHeight * scale));
                        break;
                    case Keys.D2:
                    case Keys.NumPad2:
                        scale = scales[1];
                        ClientSize = new Size((int)(baseClientWidth * scale), (int)(baseClientHeight * scale));
                        break;
                    case Keys.D3:
                    case Keys.NumPad3:
                        scale = scales[2];
                        ClientSize = new Size((int)(baseClientWidth * scale), (int)(baseClientHeight * scale));
                        break;
                    case Keys.D4:
                    case Keys.NumPad4:
                        scale = scales[3];
                        ClientSize = new Size((int)(baseClientWidth * scale), (int)(baseClientHeight * scale));
                        break;
                    case Keys.D5:
                    case Keys.NumPad5:
                        scale = scales[4];
                        ClientSize = new Size((int)(baseClientWidth * scale), (int)(baseClientHeight * scale));
                        break;
                }
            }
        }
    }
}
