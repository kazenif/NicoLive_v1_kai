namespace NicoLive
{
    partial class NewVersion
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mLink = new System.Windows.Forms.LinkLabel();
            this.mUpdate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(225, 74);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "閉じる";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(237, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "新しいバージョンの豆ライブがリリースされています。";
            // 
            // mLink
            // 
            this.mLink.AutoSize = true;
            this.mLink.Location = new System.Drawing.Point(60, 42);
            this.mLink.Name = "mLink";
            this.mLink.Size = new System.Drawing.Size(151, 12);
            this.mLink.TabIndex = 2;
            this.mLink.TabStop = true;
            this.mLink.Text = "http://nicolive.sourceforge.jp";
            this.mLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Link_LinkClicked);
            // 
            // mUpdate
            // 
            this.mUpdate.Location = new System.Drawing.Point(121, 74);
            this.mUpdate.Name = "mUpdate";
            this.mUpdate.Size = new System.Drawing.Size(98, 23);
            this.mUpdate.TabIndex = 3;
            this.mUpdate.Text = "自動更新する";
            this.mUpdate.UseVisualStyleBackColor = true;
            this.mUpdate.Click += new System.EventHandler(this.mUpdate_Click);
            // 
            // NewVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 109);
            this.ControlBox = false;
            this.Controls.Add(this.mUpdate);
            this.Controls.Add(this.mLink);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewVersion";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "豆ライブ";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.NewVersion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel mLink;
        private System.Windows.Forms.Button mUpdate;
    }
}
