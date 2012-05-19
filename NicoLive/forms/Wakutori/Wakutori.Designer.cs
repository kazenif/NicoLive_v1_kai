namespace NicoLive
{
    partial class Wakutori
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wakutori));
            this.mToolStrip = new System.Windows.Forms.ToolStrip();
            this.mEnableBtn = new System.Windows.Forms.ToolStripButton();
            this.mBackBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mFwdBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mReloadBtn = new System.Windows.Forms.ToolStripButton();
            this.mBrowser = new System.Windows.Forms.WebBrowser();
            this.mUITimer = new System.Windows.Forms.Timer(this.components);
            this.mStatusStrip = new System.Windows.Forms.StatusStrip();
            this.mEnableLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.mToolStrip.SuspendLayout();
            this.mStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mToolStrip
            // 
            this.mToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mEnableBtn,
            this.mBackBtn,
            this.toolStripSeparator1,
            this.mFwdBtn,
            this.toolStripSeparator2,
            this.mReloadBtn});
            this.mToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mToolStrip.Name = "mToolStrip";
            this.mToolStrip.Size = new System.Drawing.Size(922, 25);
            this.mToolStrip.TabIndex = 0;
            this.mToolStrip.Text = "toolStrip1";
            // 
            // mEnableBtn
            // 
            this.mEnableBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mEnableBtn.CheckOnClick = true;
            this.mEnableBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mEnableBtn.Enabled = false;
            this.mEnableBtn.Image = ((System.Drawing.Image)(resources.GetObject("mEnableBtn.Image")));
            this.mEnableBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mEnableBtn.Name = "mEnableBtn";
            this.mEnableBtn.Size = new System.Drawing.Size(120, 22);
            this.mEnableBtn.Text = "自動枠取り開始(F4)";
            this.mEnableBtn.ToolTipText = "自動枠取り";
            this.mEnableBtn.Click += new System.EventHandler(this.EnableBtn_Click);
            // 
            // mBackBtn
            // 
            this.mBackBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mBackBtn.Image = ((System.Drawing.Image)(resources.GetObject("mBackBtn.Image")));
            this.mBackBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mBackBtn.Name = "mBackBtn";
            this.mBackBtn.Size = new System.Drawing.Size(60, 22);
            this.mBackBtn.Text = "戻る(F1)";
            this.mBackBtn.Click += new System.EventHandler(this.BackBtn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // mFwdBtn
            // 
            this.mFwdBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mFwdBtn.Image = ((System.Drawing.Image)(resources.GetObject("mFwdBtn.Image")));
            this.mFwdBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mFwdBtn.Name = "mFwdBtn";
            this.mFwdBtn.Size = new System.Drawing.Size(60, 22);
            this.mFwdBtn.Text = "進む(F2)";
            this.mFwdBtn.Click += new System.EventHandler(this.FwdBtn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // mReloadBtn
            // 
            this.mReloadBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mReloadBtn.Image = ((System.Drawing.Image)(resources.GetObject("mReloadBtn.Image")));
            this.mReloadBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mReloadBtn.Name = "mReloadBtn";
            this.mReloadBtn.Size = new System.Drawing.Size(72, 22);
            this.mReloadBtn.Text = "再読込(F3)";
            this.mReloadBtn.Click += new System.EventHandler(this.ReloadBtn_Click);
            // 
            // mBrowser
            // 
            this.mBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mBrowser.Location = new System.Drawing.Point(0, 25);
            this.mBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.mBrowser.Name = "mBrowser";
            this.mBrowser.ScriptErrorsSuppressed = true;
            this.mBrowser.Size = new System.Drawing.Size(922, 449);
            this.mBrowser.TabIndex = 1;
            this.mBrowser.Url = new System.Uri("", System.UriKind.Relative);
            this.mBrowser.ProgressChanged += new System.Windows.Forms.WebBrowserProgressChangedEventHandler(this.Browser_ProgressChanged);
            this.mBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.Browser_DocumentCompleted);
            // 
            // mUITimer
            // 
            this.mUITimer.Enabled = true;
            this.mUITimer.Tick += new System.EventHandler(this.mUITimer_Tick);
            // 
            // mStatusStrip
            // 
            this.mStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mEnableLabel,
            this.mProgress});
            this.mStatusStrip.Location = new System.Drawing.Point(0, 447);
            this.mStatusStrip.Name = "mStatusStrip";
            this.mStatusStrip.Size = new System.Drawing.Size(922, 27);
            this.mStatusStrip.TabIndex = 2;
            this.mStatusStrip.Text = "statusStrip1";
            // 
            // mEnableLabel
            // 
            this.mEnableLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.mEnableLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.mEnableLabel.Name = "mEnableLabel";
            this.mEnableLabel.Size = new System.Drawing.Size(48, 22);
            this.mEnableLabel.Text = "待機中";
            // 
            // mProgress
            // 
            this.mProgress.Name = "mProgress";
            this.mProgress.Size = new System.Drawing.Size(100, 21);
            // 
            // Wakutori
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 474);
            this.Controls.Add(this.mStatusStrip);
            this.Controls.Add(this.mBrowser);
            this.Controls.Add(this.mToolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Wakutori";
            this.Text = "枠取り";
            this.Load += new System.EventHandler(this.Wakutori_Load);
            this.mToolStrip.ResumeLayout(false);
            this.mToolStrip.PerformLayout();
            this.mStatusStrip.ResumeLayout(false);
            this.mStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mToolStrip;
        private System.Windows.Forms.ToolStripButton mEnableBtn;
        private System.Windows.Forms.WebBrowser mBrowser;
        private System.Windows.Forms.Timer mUITimer;
        private System.Windows.Forms.StatusStrip mStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel mEnableLabel;
        private System.Windows.Forms.ToolStripButton mBackBtn;
        private System.Windows.Forms.ToolStripButton mFwdBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton mReloadBtn;
        private System.Windows.Forms.ToolStripProgressBar mProgress;
    }
}