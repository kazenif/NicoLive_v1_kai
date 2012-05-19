namespace NicoLive
{
    partial class ChangeProp
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
            this.mOK = new System.Windows.Forms.Button();
            this.mDesc = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mTitle = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.mAd = new System.Windows.Forms.CheckBox();
            this.mFace = new System.Windows.Forms.CheckBox();
            this.mTotumachi = new System.Windows.Forms.CheckBox();
            this.mTimeShift = new System.Windows.Forms.CheckBox();
            this.mComuOnly = new System.Windows.Forms.CheckBox();
            this.mCancel = new System.Windows.Forms.Button();
            this.mCommunity = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.mLivetags = new System.Windows.Forms.DataGridView();
            this.mTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mLock = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.mLiveTagLockAll = new System.Windows.Forms.CheckBox();
            this.mCruise = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mLivetags)).BeginInit();
            this.SuspendLayout();
            // 
            // mOK
            // 
            this.mOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOK.Location = new System.Drawing.Point(301, 461);
            this.mOK.Name = "mOK";
            this.mOK.Size = new System.Drawing.Size(75, 23);
            this.mOK.TabIndex = 6;
            this.mOK.Text = "OK";
            this.mOK.UseVisualStyleBackColor = true;
            this.mOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // mDesc
            // 
            this.mDesc.Location = new System.Drawing.Point(43, 94);
            this.mDesc.MaxLength = 1000;
            this.mDesc.Multiline = true;
            this.mDesc.Name = "mDesc";
            this.mDesc.Size = new System.Drawing.Size(318, 91);
            this.mDesc.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mTitle);
            this.groupBox1.Location = new System.Drawing.Point(26, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 55);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "タイトル";
            // 
            // mTitle
            // 
            this.mTitle.FormattingEnabled = true;
            this.mTitle.Location = new System.Drawing.Point(17, 18);
            this.mTitle.MaxLength = 100;
            this.mTitle.Name = "mTitle";
            this.mTitle.Size = new System.Drawing.Size(318, 20);
            this.mTitle.TabIndex = 0;
            this.mTitle.SelectionChangeCommitted += new System.EventHandler(this.mTitle_SelectionChangeCommitted);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(26, 71);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(350, 138);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "詳細";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(128, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "（※改行は自動で<br>タグに変換されます）";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.mCruise);
            this.groupBox3.Controls.Add(this.mAd);
            this.groupBox3.Controls.Add(this.mFace);
            this.groupBox3.Controls.Add(this.mTotumachi);
            this.groupBox3.Controls.Add(this.mTimeShift);
            this.groupBox3.Controls.Add(this.mComuOnly);
            this.groupBox3.Location = new System.Drawing.Point(26, 264);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(350, 65);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "その他";
            // 
            // mAd
            // 
            this.mAd.AutoSize = true;
            this.mAd.Location = new System.Drawing.Point(232, 39);
            this.mAd.Name = "mAd";
            this.mAd.Size = new System.Drawing.Size(72, 16);
            this.mAd.TabIndex = 4;
            this.mAd.Text = "広告設定";
            this.mAd.UseVisualStyleBackColor = true;
            // 
            // mFace
            // 
            this.mFace.AutoSize = true;
            this.mFace.Location = new System.Drawing.Point(17, 39);
            this.mFace.Name = "mFace";
            this.mFace.Size = new System.Drawing.Size(57, 16);
            this.mFace.TabIndex = 3;
            this.mFace.Text = "顔出し";
            this.mFace.UseVisualStyleBackColor = true;
            // 
            // mTotumachi
            // 
            this.mTotumachi.AutoSize = true;
            this.mTotumachi.Location = new System.Drawing.Point(80, 39);
            this.mTotumachi.Name = "mTotumachi";
            this.mTotumachi.Size = new System.Drawing.Size(57, 16);
            this.mTotumachi.TabIndex = 2;
            this.mTotumachi.Text = "凸待ち";
            this.mTotumachi.UseVisualStyleBackColor = true;
            // 
            // mTimeShift
            // 
            this.mTimeShift.AutoSize = true;
            this.mTimeShift.Location = new System.Drawing.Point(180, 17);
            this.mTimeShift.Name = "mTimeShift";
            this.mTimeShift.Size = new System.Drawing.Size(129, 16);
            this.mTimeShift.TabIndex = 1;
            this.mTimeShift.Text = "タイムシフトを利用する";
            this.mTimeShift.UseVisualStyleBackColor = true;
            // 
            // mComuOnly
            // 
            this.mComuOnly.AutoSize = true;
            this.mComuOnly.Location = new System.Drawing.Point(17, 17);
            this.mComuOnly.Name = "mComuOnly";
            this.mComuOnly.Size = new System.Drawing.Size(149, 16);
            this.mComuOnly.TabIndex = 0;
            this.mComuOnly.Text = "コミュニティ限定で放送する";
            this.mComuOnly.UseVisualStyleBackColor = true;
            // 
            // mCancel
            // 
            this.mCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancel.Location = new System.Drawing.Point(206, 461);
            this.mCancel.Name = "mCancel";
            this.mCancel.Size = new System.Drawing.Size(75, 23);
            this.mCancel.TabIndex = 5;
            this.mCancel.Text = "Cancel";
            this.mCancel.UseVisualStyleBackColor = true;
            // 
            // mCommunity
            // 
            this.mCommunity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCommunity.FormattingEnabled = true;
            this.mCommunity.Location = new System.Drawing.Point(17, 18);
            this.mCommunity.Name = "mCommunity";
            this.mCommunity.Size = new System.Drawing.Size(318, 20);
            this.mCommunity.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.mCommunity);
            this.groupBox4.Location = new System.Drawing.Point(26, 211);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(350, 51);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "コミュニティ";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.mLivetags);
            this.groupBox5.Controls.Add(this.mLiveTagLockAll);
            this.groupBox5.Location = new System.Drawing.Point(26, 329);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(350, 126);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "タグ";
            // 
            // mLivetags
            // 
            this.mLivetags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mLivetags.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.mTag,
            this.mLock});
            this.mLivetags.Location = new System.Drawing.Point(17, 18);
            this.mLivetags.MultiSelect = false;
            this.mLivetags.Name = "mLivetags";
            this.mLivetags.RowHeadersVisible = false;
            this.mLivetags.RowTemplate.Height = 21;
            this.mLivetags.Size = new System.Drawing.Size(318, 80);
            this.mLivetags.TabIndex = 1;
            this.mLivetags.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.mLivetags_UserAddedRow);
            this.mLivetags.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.mLivetags_UserDeletedRow);
            // 
            // mTag
            // 
            this.mTag.Frozen = true;
            this.mTag.HeaderText = "タグ";
            this.mTag.Name = "mTag";
            this.mTag.Width = 250;
            // 
            // mLock
            // 
            this.mLock.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.mLock.FalseValue = "false";
            this.mLock.HeaderText = "ロック";
            this.mLock.Name = "mLock";
            this.mLock.TrueValue = "true";
            // 
            // mLiveTagLockAll
            // 
            this.mLiveTagLockAll.AutoSize = true;
            this.mLiveTagLockAll.Location = new System.Drawing.Point(17, 104);
            this.mLiveTagLockAll.Name = "mLiveTagLockAll";
            this.mLiveTagLockAll.Size = new System.Drawing.Size(157, 16);
            this.mLiveTagLockAll.TabIndex = 0;
            this.mLiveTagLockAll.Text = "視聴者にタグ編集をさせない";
            this.mLiveTagLockAll.UseVisualStyleBackColor = true;
            // 
            // mCruise
            // 
            this.mCruise.AutoSize = true;
            this.mCruise.Location = new System.Drawing.Point(143, 39);
            this.mCruise.Name = "mCruise";
            this.mCruise.Size = new System.Drawing.Size(83, 16);
            this.mCruise.TabIndex = 5;
            this.mCruise.Text = "クルーズ待ち";
            this.mCruise.UseVisualStyleBackColor = true;
            // 
            // ChangeProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 496);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.mCancel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.mDesc);
            this.Controls.Add(this.mOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChangeProp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "放送情報変更";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ChangeProp_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mLivetags)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mOK;
        private System.Windows.Forms.TextBox mDesc;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox mComuOnly;
        private System.Windows.Forms.CheckBox mTimeShift;
        private System.Windows.Forms.CheckBox mTotumachi;
        private System.Windows.Forms.CheckBox mFace;
        private System.Windows.Forms.Button mCancel;
        private System.Windows.Forms.ComboBox mCommunity;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox mTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridView mLivetags;
        private System.Windows.Forms.CheckBox mLiveTagLockAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn mTag;
        private System.Windows.Forms.DataGridViewCheckBoxColumn mLock;
        private System.Windows.Forms.CheckBox mAd;
        private System.Windows.Forms.CheckBox mCruise;
    }
}