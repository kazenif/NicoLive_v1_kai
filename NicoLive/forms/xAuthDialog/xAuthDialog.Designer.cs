namespace NicoLive
{
    partial class xAuthDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mID = new System.Windows.Forms.TextBox();
            this.mPassword = new System.Windows.Forms.TextBox();
            this.mOKBtn = new System.Windows.Forms.Button();
            this.mGroupBox = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "ＩＤ：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "パスワード：";
            // 
            // mID
            // 
            this.mID.Location = new System.Drawing.Point(88, 28);
            this.mID.Name = "mID";
            this.mID.Size = new System.Drawing.Size(100, 19);
            this.mID.TabIndex = 2;
            // 
            // mPassword
            // 
            this.mPassword.Location = new System.Drawing.Point(88, 57);
            this.mPassword.Name = "mPassword";
            this.mPassword.Size = new System.Drawing.Size(100, 19);
            this.mPassword.TabIndex = 4;
            this.mPassword.UseSystemPasswordChar = true;
            this.mPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mPassword_KeyDown);
            // 
            // mOKBtn
            // 
            this.mOKBtn.Location = new System.Drawing.Point(205, 53);
            this.mOKBtn.Name = "mOKBtn";
            this.mOKBtn.Size = new System.Drawing.Size(75, 23);
            this.mOKBtn.TabIndex = 5;
            this.mOKBtn.Text = "OK";
            this.mOKBtn.UseVisualStyleBackColor = true;
            this.mOKBtn.Click += new System.EventHandler(this.OK_Click);
            // 
            // mGroupBox
            // 
            this.mGroupBox.Location = new System.Drawing.Point(2, 3);
            this.mGroupBox.Name = "mGroupBox";
            this.mGroupBox.Size = new System.Drawing.Size(291, 86);
            this.mGroupBox.TabIndex = 0;
            this.mGroupBox.TabStop = false;
            this.mGroupBox.Text = "TwitterのIDとパスワードを入力してください";
            // 
            // xAuthDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 92);
            this.Controls.Add(this.mOKBtn);
            this.Controls.Add(this.mPassword);
            this.Controls.Add(this.mID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "xAuthDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Twitter認証";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mID;
        private System.Windows.Forms.TextBox mPassword;
        private System.Windows.Forms.Button mOKBtn;
        private System.Windows.Forms.GroupBox mGroupBox;
    }
}