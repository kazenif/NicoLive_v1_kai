//-------------------------------------------------------------------------
// ニコニコ生放送、配信ツール
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 多重起動回避 ----------------------------------
            if (Process.GetProcessesByName(
                 Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show("すでに起動しています！", "豆ライブ");
                Application.Exit() ;
                return;
            }

            //------------------------------------------------
            // 設定ファイルのアップグレード
            if (!Properties.Settings.Default.call_upgrade)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.call_upgrade = true;
                Properties.Settings.Default.Save();
            }
            
            //------------------------------------------------
			// Twitterの認証をxAuthに移行
            if (Properties.Settings.Default.tw_passwd.Length != 0 &&
                Properties.Settings.Default.tw_user.Length != 0)
            {
                Twitter tw = new Twitter();
                tw.xAuth(Properties.Settings.Default.tw_user,Properties.Settings.Default.tw_passwd);
            }

            //------------------------------------------------
            // Updater更新
            if (File.Exists("Updater-new.exe"))
            {
                File.Delete("Updater.exe");
                File.Move("Updater-new.exe", "Updater.exe");
            }

            //------------------------------------------------
            // 画面色数チェック
 			int bpp = Screen.PrimaryScreen.BitsPerPixel;
			if( bpp != 32 ) {
				MessageBox.Show("画面の色数が32ビットで無いため、正常に動作しない可能性があります","豆ライブ");	
			}

			int len = Screen.AllScreens.Length;
			if ( len > 1 ) {
                if (Screen.AllScreens[len-1].BitsPerPixel != 32)
                {
                    MessageBox.Show("サブ画面の色数が32ビットで無いため、正常に動作しない可能性があります", "豆ライブ");
                }
			}
			
            //------------------------------------------------
            // コマンドラインから放送ＩＤ取得
            string[] args = Environment.GetCommandLineArgs();
            bool auto_connect = false;
            if (args.Count() >= 2)
            {
                string uri = "";
                if (args[1].StartsWith("lv"))
                    uri = args[1];
                else if (args[1].StartsWith("http://"))
                {
                    int idx = args[1].IndexOf("lv");
                    uri = args[1].Substring(idx);
                }
                if (uri.Length >= 0)
                {
                    Properties.Settings.Default.last_lv = uri;
                    auto_connect = true;
                }
            }
            //------------------------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            form.mAutoConnect = auto_connect;
            Application.Run(form);

        }
    }
}

//-------------------------------------------------------------------------
// ニコニコ生放送、配信ツール
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
