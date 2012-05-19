namespace NicoLive
{
    partial class Imakoko
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Imakoko));
            this.mBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // mBrowser
            // 
            this.mBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mBrowser.Location = new System.Drawing.Point(0, 0);
            this.mBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.mBrowser.Name = "mBrowser";
            this.mBrowser.ScriptErrorsSuppressed = true;
            this.mBrowser.Size = new System.Drawing.Size(349, 235);
            this.mBrowser.TabIndex = 0;
            this.mBrowser.Url = new System.Uri("http://imakoko-gps.appspot.com/static/view_hokkaido.html", System.UriKind.Absolute);
            // 
            // Imakoko
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 235);
            this.Controls.Add(this.mBrowser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Imakoko";
            this.Text = "今ココなう！";
            this.Load += new System.EventHandler(this.Imakoko_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser mBrowser;
    }
}