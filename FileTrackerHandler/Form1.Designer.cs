namespace FileTrackerHandler
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.bttnBrowse = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsss = new System.Windows.Forms.ToolStripStatusLabel();
            this._mtimerClientReadFromPipe = new System.Windows.Forms.Timer(this.components);
            this.btnRest = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Folder";
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(56, 9);
            this.txtURL.Name = "txtURL";
            this.txtURL.ReadOnly = true;
            this.txtURL.Size = new System.Drawing.Size(532, 20);
            this.txtURL.TabIndex = 1;
            this.txtURL.TextChanged += new System.EventHandler(this.txtURL_TextChanged);
            // 
            // bttnBrowse
            // 
            this.bttnBrowse.Location = new System.Drawing.Point(594, 7);
            this.bttnBrowse.Name = "bttnBrowse";
            this.bttnBrowse.Size = new System.Drawing.Size(75, 23);
            this.bttnBrowse.TabIndex = 2;
            this.bttnBrowse.Text = "Browse";
            this.bttnBrowse.UseVisualStyleBackColor = true;
            this.bttnBrowse.Click += new System.EventHandler(this.bttnBrowse_Click);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(56, 35);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(151, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start Tracking";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsss});
            this.statusStrip1.Location = new System.Drawing.Point(0, 72);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(680, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsss
            // 
            this.tsss.Name = "tsss";
            this.tsss.Size = new System.Drawing.Size(43, 17);
            this.tsss.Text = "Offline";
            // 
            // _mtimerClientReadFromPipe
            // 
            this._mtimerClientReadFromPipe.Enabled = true;
            this._mtimerClientReadFromPipe.Interval = 500;
            this._mtimerClientReadFromPipe.Tick += new System.EventHandler(this._mtimerClientReadFromPipe_Tick);
            // 
            // btnRest
            // 
            this.btnRest.Location = new System.Drawing.Point(594, 35);
            this.btnRest.Name = "btnRest";
            this.btnRest.Size = new System.Drawing.Size(74, 23);
            this.btnRest.TabIndex = 5;
            this.btnRest.Text = "Reset";
            this.btnRest.UseVisualStyleBackColor = true;
            this.btnRest.Click += new System.EventHandler(this.btnRest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 94);
            this.Controls.Add(this.btnRest);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.bttnBrowse);
            this.Controls.Add(this.txtURL);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Folder Track Handler Example by @qursaan";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Button bttnBrowse;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsss;
        private System.Windows.Forms.Timer _mtimerClientReadFromPipe;
        private System.Windows.Forms.Button btnRest;
    }
}

