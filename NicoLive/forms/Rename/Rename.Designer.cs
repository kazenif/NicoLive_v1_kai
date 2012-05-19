namespace NicoLive
{
    partial class Rename
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
            this.mName = new System.Windows.Forms.TextBox();
            this.mOK = new System.Windows.Forms.Button();
            this.mCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名前：";
            // 
            // mName
            // 
            this.mName.Location = new System.Drawing.Point(70, 26);
            this.mName.Name = "mName";
            this.mName.Size = new System.Drawing.Size(192, 19);
            this.mName.TabIndex = 1;
            // 
            // mOK
            // 
            this.mOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOK.Location = new System.Drawing.Point(187, 67);
            this.mOK.Name = "mOK";
            this.mOK.Size = new System.Drawing.Size(75, 23);
            this.mOK.TabIndex = 2;
            this.mOK.Text = "OK";
            this.mOK.UseVisualStyleBackColor = true;
            this.mOK.Click += new System.EventHandler(this.mOK_Click);
            // 
            // mCancel
            // 
            this.mCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancel.Location = new System.Drawing.Point(106, 67);
            this.mCancel.Name = "mCancel";
            this.mCancel.Size = new System.Drawing.Size(75, 23);
            this.mCancel.TabIndex = 3;
            this.mCancel.Text = "キャンセル";
            this.mCancel.UseVisualStyleBackColor = true;
            this.mCancel.Click += new System.EventHandler(this.mCancel_Click);
            // 
            // Rename
            // 
            this.AcceptButton = this.mOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 101);
            this.Controls.Add(this.mCancel);
            this.Controls.Add(this.mOK);
            this.Controls.Add(this.mName);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Rename";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "コテハン登録";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Rename_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mName;
        private System.Windows.Forms.Button mOK;
        private System.Windows.Forms.Button mCancel;
    }
}