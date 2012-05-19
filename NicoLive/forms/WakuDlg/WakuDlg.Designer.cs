namespace NicoLive
{
    partial class WakuDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WakuDlg));
            this.mCancel = new System.Windows.Forms.Button();
            this.mLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mWorker = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mCancel
            // 
            this.mCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancel.Location = new System.Drawing.Point(206, 77);
            this.mCancel.Name = "mCancel";
            this.mCancel.Size = new System.Drawing.Size(75, 23);
            this.mCancel.TabIndex = 0;
            this.mCancel.Text = "Cancel";
            this.mCancel.UseVisualStyleBackColor = true;
            this.mCancel.Click += new System.EventHandler(this.mCancel_Click);
            // 
            // mLabel
            // 
            this.mLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mLabel.Location = new System.Drawing.Point(17, 22);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(245, 23);
            this.mLabel.TabIndex = 1;
            this.mLabel.Text = "枠取り中";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mLabel);
            this.groupBox1.Location = new System.Drawing.Point(13, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 60);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ステータス";
            // 
            // mWorker
            // 
            this.mWorker.WorkerSupportsCancellation = true;
            this.mWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.mWorker_DoWork);
            // 
            // WakuDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 109);
            this.ControlBox = false;
            this.Controls.Add(this.mCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WakuDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "連続枠取り";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.WakuDlg_Load);
            this.Shown += new System.EventHandler(this.WakuDlg_Shown);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mCancel;
        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.ComponentModel.BackgroundWorker mWorker;
    }
}