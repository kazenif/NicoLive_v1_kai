namespace NicoLive
{
    partial class CommentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommentForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mStatus = new System.Windows.Forms.StatusStrip();
            this.mRestTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.mTotalCnt = new System.Windows.Forms.ToolStripStatusLabel();
            this.mActiveCnt = new System.Windows.Forms.ToolStripStatusLabel();
            this.mUniqCnt = new System.Windows.Forms.ToolStripStatusLabel();
            this.mCpuInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.mBattery = new System.Windows.Forms.ToolStripStatusLabel();
            this.mUpLink = new System.Windows.Forms.ToolStripStatusLabel();
            this.mCommentList = new System.Windows.Forms.DataGridView();
            this.mNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mHandle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mCmtCxtMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mCopyComment = new System.Windows.Forms.ToolStripMenuItem();
            this.mCopyID = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mOpenURL = new System.Windows.Forms.ToolStripMenuItem();
            this.mShowUserPage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mRename = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mNgID = new System.Windows.Forms.ToolStripMenuItem();
            this.mUITimer = new System.Windows.Forms.Timer(this.components);
            this.mStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mCommentList)).BeginInit();
            this.mCmtCxtMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mStatus
            // 
            this.mStatus.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mRestTime,
            this.mTotalCnt,
            this.mActiveCnt,
            this.mUniqCnt,
            this.mCpuInfo,
            this.mBattery,
            this.mUpLink});
            resources.ApplyResources(this.mStatus, "mStatus");
            this.mStatus.Name = "mStatus";
            // 
            // mRestTime
            // 
            this.mRestTime.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.mRestTime.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            resources.ApplyResources(this.mRestTime, "mRestTime");
            this.mRestTime.Name = "mRestTime";
            // 
            // mTotalCnt
            // 
            this.mTotalCnt.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.mTotalCnt.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.mTotalCnt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.mTotalCnt, "mTotalCnt");
            this.mTotalCnt.Name = "mTotalCnt";
            // 
            // mActiveCnt
            // 
            this.mActiveCnt.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.mActiveCnt.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            resources.ApplyResources(this.mActiveCnt, "mActiveCnt");
            this.mActiveCnt.Name = "mActiveCnt";
            // 
            // mUniqCnt
            // 
            this.mUniqCnt.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.mUniqCnt.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            resources.ApplyResources(this.mUniqCnt, "mUniqCnt");
            this.mUniqCnt.Name = "mUniqCnt";
            // 
            // mCpuInfo
            // 
            this.mCpuInfo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.mCpuInfo.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            resources.ApplyResources(this.mCpuInfo, "mCpuInfo");
            this.mCpuInfo.Name = "mCpuInfo";
            // 
            // mBattery
            // 
            this.mBattery.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.mBattery.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            resources.ApplyResources(this.mBattery, "mBattery");
            this.mBattery.Name = "mBattery";
            // 
            // mUpLink
            // 
            this.mUpLink.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.mUpLink.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            resources.ApplyResources(this.mUpLink, "mUpLink");
            this.mUpLink.Name = "mUpLink";
            // 
            // mCommentList
            // 
            this.mCommentList.AllowUserToAddRows = false;
            this.mCommentList.AllowUserToDeleteRows = false;
            this.mCommentList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.mCommentList.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.mCommentList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.mCommentList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mCommentList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.mNo,
            this.mID,
            this.mHandle,
            this.mComment});
            this.mCommentList.ContextMenuStrip = this.mCmtCxtMenu;
            resources.ApplyResources(this.mCommentList, "mCommentList");
            this.mCommentList.MultiSelect = false;
            this.mCommentList.Name = "mCommentList";
            this.mCommentList.ReadOnly = true;
            this.mCommentList.RowHeadersVisible = false;
            this.mCommentList.RowTemplate.Height = 21;
            this.mCommentList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // mNo
            // 
            resources.ApplyResources(this.mNo, "mNo");
            this.mNo.Name = "mNo";
            this.mNo.ReadOnly = true;
            // 
            // mID
            // 
            resources.ApplyResources(this.mID, "mID");
            this.mID.Name = "mID";
            this.mID.ReadOnly = true;
            // 
            // mHandle
            // 
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.mHandle.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.mHandle, "mHandle");
            this.mHandle.Name = "mHandle";
            this.mHandle.ReadOnly = true;
            // 
            // mComment
            // 
            this.mComment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.mComment.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.mComment, "mComment");
            this.mComment.Name = "mComment";
            this.mComment.ReadOnly = true;
            // 
            // mCmtCxtMenu
            // 
            this.mCmtCxtMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mCopyComment,
            this.mCopyID,
            this.toolStripMenuItem3,
            this.mOpenURL,
            this.mShowUserPage,
            this.toolStripMenuItem2,
            this.mRename,
            this.toolStripSeparator1,
            this.mNgID});
            this.mCmtCxtMenu.Name = "mCmtCxtMenu";
            resources.ApplyResources(this.mCmtCxtMenu, "mCmtCxtMenu");
            this.mCmtCxtMenu.Opening += new System.ComponentModel.CancelEventHandler(this.mCmtCxtMenu_Opening);
            // 
            // mCopyComment
            // 
            this.mCopyComment.Name = "mCopyComment";
            resources.ApplyResources(this.mCopyComment, "mCopyComment");
            this.mCopyComment.Click += new System.EventHandler(this.CopyComment_Click);
            // 
            // mCopyID
            // 
            this.mCopyID.Name = "mCopyID";
            resources.ApplyResources(this.mCopyID, "mCopyID");
            this.mCopyID.Click += new System.EventHandler(this.CopyID_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // mOpenURL
            // 
            this.mOpenURL.Name = "mOpenURL";
            resources.ApplyResources(this.mOpenURL, "mOpenURL");
            this.mOpenURL.Click += new System.EventHandler(this.OpenURL_Click);
            // 
            // mShowUserPage
            // 
            this.mShowUserPage.Name = "mShowUserPage";
            resources.ApplyResources(this.mShowUserPage, "mShowUserPage");
            this.mShowUserPage.Click += new System.EventHandler(this.ShowUserPage_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // mRename
            // 
            this.mRename.Name = "mRename";
            resources.ApplyResources(this.mRename, "mRename");
            this.mRename.Click += new System.EventHandler(this.Rename_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // mNgID
            // 
            this.mNgID.Name = "mNgID";
            resources.ApplyResources(this.mNgID, "mNgID");
            this.mNgID.Click += new System.EventHandler(this.mNgID_Click);
            // 
            // mUITimer
            // 
            this.mUITimer.Enabled = true;
            this.mUITimer.Tick += new System.EventHandler(this.UITimer_Tick);
            // 
            // CommentForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mCommentList);
            this.Controls.Add(this.mStatus);
            this.Name = "CommentForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.Load += new System.EventHandler(this.CommentForm_Load);
            this.VisibleChanged += new System.EventHandler(this.CommentForm_VisibleChanged);
            this.mStatus.ResumeLayout(false);
            this.mStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mCommentList)).EndInit();
            this.mCmtCxtMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip mStatus;
        private System.Windows.Forms.ToolStripStatusLabel mTotalCnt;
        private System.Windows.Forms.ToolStripStatusLabel mActiveCnt;
        private System.Windows.Forms.ToolStripStatusLabel mUniqCnt;
        private System.Windows.Forms.ToolStripStatusLabel mCpuInfo;
        private System.Windows.Forms.ToolStripStatusLabel mBattery;
        private System.Windows.Forms.ToolStripStatusLabel mUpLink;
        private System.Windows.Forms.DataGridView mCommentList;
        private System.Windows.Forms.DataGridViewTextBoxColumn mNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn mID;
        private System.Windows.Forms.DataGridViewTextBoxColumn mHandle;
        private System.Windows.Forms.DataGridViewTextBoxColumn mComment;
        private System.Windows.Forms.ContextMenuStrip mCmtCxtMenu;
        private System.Windows.Forms.ToolStripMenuItem mCopyComment;
        private System.Windows.Forms.ToolStripMenuItem mCopyID;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mOpenURL;
        private System.Windows.Forms.ToolStripMenuItem mShowUserPage;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mRename;
        private System.Windows.Forms.Timer mUITimer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mNgID;
        private System.Windows.Forms.ToolStripStatusLabel mRestTime;
    }
}