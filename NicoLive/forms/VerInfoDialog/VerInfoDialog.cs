//-------------------------------------------------------------------------
// バージョンダイアログ
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
	/// <summary>
	/// VerInfoDialog の概要の説明です。
	/// </summary>
	public class VerInfoDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label4;
        private LinkLabel mComuLink;
        private LinkLabel mProjectLink;
        private Label label5;
        private Label label6;
        private LinkLabel mLinkKaze;
        private Label label7;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public VerInfoDialog()
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.mComuLink = new System.Windows.Forms.LinkLabel();
            this.mProjectLink = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.mLinkKaze = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(24, 17);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(56, 50);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(96, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(336, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(96, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(336, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(96, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(336, 23);
            this.label3.TabIndex = 3;
            this.label3.Text = "label3";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(345, 186);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "OK";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(96, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(336, 56);
            this.label4.TabIndex = 5;
            this.label4.Text = "label4";
            // 
            // mComuLink
            // 
            this.mComuLink.AutoSize = true;
            this.mComuLink.Location = new System.Drawing.Point(151, 127);
            this.mComuLink.Name = "mComuLink";
            this.mComuLink.Size = new System.Drawing.Size(221, 12);
            this.mComuLink.TabIndex = 6;
            this.mComuLink.TabStop = true;
            this.mComuLink.Text = "http://ch.nicovideo.jp/community/co48276";
            this.mComuLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ComuLabel_LinkClicked);
            // 
            // mProjectLink
            // 
            this.mProjectLink.AutoSize = true;
            this.mProjectLink.Location = new System.Drawing.Point(151, 148);
            this.mProjectLink.Name = "mProjectLink";
            this.mProjectLink.Size = new System.Drawing.Size(157, 12);
            this.mProjectLink.TabIndex = 7;
            this.mProjectLink.TabStop = true;
            this.mProjectLink.Text = "http://nicolive.sourceforge.jp/";
            this.mProjectLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ProjLabel_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(83, 127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "コミュニティー：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(90, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "プロジェクト：";
            // 
            // mLinkKaze
            // 
            this.mLinkKaze.AutoSize = true;
            this.mLinkKaze.Location = new System.Drawing.Point(151, 172);
            this.mLinkKaze.Name = "mLinkKaze";
            this.mLinkKaze.Size = new System.Drawing.Size(166, 12);
            this.mLinkKaze.TabIndex = 10;
            this.mLinkKaze.TabStop = true;
            this.mLinkKaze.Text = "http://www43.atwiki.jp/kazenif/";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(105, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "改造版：";
            // 
            // VerInfoDialog
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(442, 221);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.mLinkKaze);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.mProjectLink);
            this.Controls.Add(this.mComuLink);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VerInfoDialog";
            this.ShowInTaskbar = false;
            this.Text = "バージョン情報";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.VerInfoDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void VerInfoDialog_Load(object sender, System.EventArgs e)
		{
			// ***** Applicationクラスのプロパティにより取得 *****
			// バージョン名（AssemblyInformationalVersionAttribute属性）を取得
			string appVersion = Application.ProductVersion;
			// 製品名（AssemblyProductAttribute属性）を取得
			string appProductName = Application.ProductName;
			// 会社名（AssemblyCompanyAttribute属性）を取得
			string appCompanyName = Application.CompanyName;

			// ***** アセンブリから直接取得 *****
			Assembly mainAssembly = Assembly.GetEntryAssembly();

			// コピーライト情報を取得
			string appCopyright = "-";
			object[] CopyrightArray = 
				mainAssembly.GetCustomAttributes(
				typeof(AssemblyCopyrightAttribute), false);
			if ((CopyrightArray != null) && (CopyrightArray.Length > 0))
			{
				appCopyright = 
					((AssemblyCopyrightAttribute)CopyrightArray[0]).Copyright;
			}

			// 詳細情報を取得
			string appDescription = "-";
			object[] DescriptionArray = 
				mainAssembly.GetCustomAttributes(
				typeof(AssemblyDescriptionAttribute), false);
			if ((DescriptionArray != null) && (DescriptionArray.Length > 0))
			{
				appDescription = 
					((AssemblyDescriptionAttribute)DescriptionArray[0]).Description;
			}

			// ***** EXEファイルから直接取得（Win32API使用） *****
			
			// アプリケーション・アイコンを取得
			Icon appIcon;
			SHFILEINFO shinfo = new SHFILEINFO();
			IntPtr hSuccess = SHGetFileInfo( 
				mainAssembly.Location, 0, 
				ref shinfo, (uint)Marshal.SizeOf(shinfo), 
				SHGFI_ICON | SHGFI_LARGEICON);
			if (hSuccess != IntPtr.Zero)
			{
				appIcon = Icon.FromHandle(shinfo.hIcon);
			}
			else
			{
				appIcon = SystemIcons.Application;
			}

			// ラベルなどにバージョン情報をセット
			pictureBox1.Image = appIcon.ToBitmap(); 
			Text = appProductName + " のバージョン情報";
			label1.Text = appProductName;
			label2.Text = "Version " + appVersion + " 改32rc3";
			label3.Text = appCopyright;
			label4.Text = appDescription;

			// バージョン情報を取得（別バージョン）
			//AssemblyName mainAssemName = mainAssembly.GetName();
			//Version appVersion2 = mainAssemName.Version;
			//MessageBox.Show(appVersion2.ToString());

            // センタリング
            this.Top = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		#region アイコン取得用のWin32API

		// SHGetFileInfo関数
		[DllImport("shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

		// SHGetFileInfo関数で使用するフラグ
		private const uint SHGFI_ICON = 0x100; // アイコン・リソースの取得
		private const uint SHGFI_LARGEICON = 0x0; // 大きいアイコン
		private const uint SHGFI_SMALLICON = 0x1; // 小さいアイコン

		// SHGetFileInfo関数で使用する構造体
		private struct SHFILEINFO
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		#endregion

        private void ComuLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://ch.nicovideo.jp/community/co48276");
        }

        private void ProjLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://sourceforge.jp/projects/nicolive");
        }
	}
}
//-------------------------------------------------------------------------
// バージョンダイアログ
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------