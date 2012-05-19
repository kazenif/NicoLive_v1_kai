//-------------------------------------------------------------------------
// 新規バージョン通知ダイアログ
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.IO;
using System.Windows.Forms;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    public partial class NewVersion : Form
    {
        //-------------------------------------------------------------------------
        // 初期化
        //-------------------------------------------------------------------------
        public NewVersion()
        {
            InitializeComponent();
        }

        //-------------------------------------------------------------------------
        // ロード時
        //-------------------------------------------------------------------------
        private void NewVersion_Load(object sender, EventArgs e)
        {
            // センタリング
            this.Top = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
        }

        //-------------------------------------------------------------------------
        // クローズ
        //-------------------------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        //-------------------------------------------------------------------------
        // ダウンロードページへジャンプ
        //-------------------------------------------------------------------------
        private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(mLink.Text);
        }

        //-------------------------------------------------------------------------
        // Updater起動
        //-------------------------------------------------------------------------
        private void mUpdate_Click(object sender, EventArgs e)
        {
            if (!File.Exists("Updater.exe"))
            {
                MessageBox.Show("Updater.exeが見つかりません", "豆ライブ");
                return;
            }
            System.Diagnostics.Process.Start("Updater.exe");
            Application.Exit();
        }
    }
}
//-------------------------------------------------------------------------
// 新規バージョン通知ダイアログ
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------