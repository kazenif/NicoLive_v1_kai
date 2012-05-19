//-------------------------------------------------------------------------
// 棒読みちゃんアクセスクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    class Bouyomi : IDisposable
    {
        //-------------------------------------------------------------------------
        // コメント読み上げ
        //-------------------------------------------------------------------------
        public void Talk(string iStr)
        {
            const Int16 iVoice = 0;		//  声質(0-8)、 0で棒読みちゃん画面上の設定
            const Int16 iVolume = -1;		// 音量(0-100)、 -1で棒読みちゃん画面上の設定
            const Int16 iSpeed = -1;		// 速度(50-300)、 -1で棒読みちゃん画面上の設定
            const Int16 iTone = -1;		// 音程(50-200)、 -1で棒読みちゃん画面上の設定
            
            Thread th = new Thread(delegate()
            {
                TalkByTCP(FormatText(iStr), iSpeed, iTone, iVolume, iVoice);
            });
            th.Start();
        }
        //-------------------------------------------------------------------------
        // 解放
        //-------------------------------------------------------------------------
        public void Dispose()
        {
        }

        //-------------------------------------------------------------------------
        // TCP/IPでおしゃべり
        //-------------------------------------------------------------------------
        private void TalkByTCP(string iStr, Int16 iSpeed, Int16 iTone, Int16 iVolume, Int16 iVoice)
        {
            string sMessage = null;
            const byte bCode = 0;
            const Int16 iCommand = 0x0001;

            sMessage = iStr;
            byte[] bMessage = Encoding.UTF8.GetBytes(sMessage);
            Int32 iLength = bMessage.Length;

            //棒読みちゃんのTCPサーバへ接続
            const string sHost = "127.0.0.1"; //棒読みちゃんが動いているホスト
            int iPort = Properties.Settings.Default.bouyomi_port;       //棒読みちゃんのTCPサーバのポート番号(デフォルト値)

            TcpClient tc = null;
            try
            {
                tc = new TcpClient(sHost, iPort);
            }
            catch (Exception)
            {
                Debug.WriteLine("接続失敗");
            }

            if (tc != null)
            {
                //メッセージ送信
                using (NetworkStream ns = tc.GetStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ns))
                    {
                        bw.Write(iCommand); //コマンド（ 0:メッセージ読み上げ）
                        bw.Write(iSpeed);   //速度    （-1:棒読みちゃん画面上の設定）
                        bw.Write(iTone);    //音程    （-1:棒読みちゃん画面上の設定）
                        bw.Write(iVolume);  //音量    （-1:棒読みちゃん画面上の設定）
                        bw.Write(iVoice);   //声質    （ 0:棒読みちゃん画面上の設定、1:女性1、2:女性2、3:男性1、4:男性2、5:中性、6:ロボット、7:機械1、8:機械2）
                        bw.Write(bCode);    //文字列のbyte配列の文字コード(0:UTF-8, 1:Unicode, 2:Shift-JIS)
                        bw.Write(iLength);  //文字列のbyte配列の長さ
                        bw.Write(bMessage); //文字列のbyte配列
                    }
                }

                //切断
                tc.Close();
            }

			bMessage = null;
        }
        
        //-------------------------------------------------------------------------
        // 起動
        //-------------------------------------------------------------------------
        public static void Launch()
        {
            if (!Properties.Settings.Default.launch_bouyomi) return;
            Process[] ps = Process.GetProcessesByName("BouyomiChan");
            if(ps.Length > 0) return;

            Thread th = new Thread(delegate()
            {
                try
                {
                    Process.Start(Properties.Settings.Default.bouyomi_path, "");
                }
                catch (System.Exception)
                {
                    System.Windows.Forms.MessageBox.Show("棒読みちゃん！の起動に\n失敗しました", "NicoLive");
                }
            });
            th.Start();
        }
        
        //-------------------------------------------------------------------------
        // 終了
        //-------------------------------------------------------------------------
        public static void Exit()
        {
            if (!Properties.Settings.Default.launch_bouyomi) return;
            Process[] ps = Process.GetProcessesByName("BouyomiChan");

            foreach (System.Diagnostics.Process p in ps)
            {
                p.CloseMainWindow();
                p.Kill();
            }
        }

        //-------------------------------------------------------------------------
        // テキストの整形
        //-------------------------------------------------------------------------
        private string FormatText(string iStr)
        {
            // 数字を半角に変換
            iStr = Regex.Replace(iStr, "[０-９]", delegate(Match m)
            {
                char ch = (char)('0' + (m.Value[0] - '０'));
                return ch.ToString();
            });
            // 括弧を半角に変換
            iStr = iStr.Replace("（", "(");
            iStr = iStr.Replace("）", ")");

            // コマンドを半角化
            iStr = iStr.Replace("ｙ)", "y)");
            iStr = iStr.Replace("ｂ)", "b)");
            iStr = iStr.Replace("ｈ)", "h");
            iStr = iStr.Replace("ｄ)", "d)");
            iStr = iStr.Replace("ａ)", "a)");
            iStr = iStr.Replace("ｒ)", "r)");
            iStr = iStr.Replace("ｔ)", "t)");
            iStr = iStr.Replace("ｇ)", "g)");
            iStr = iStr.Replace("ｃ)", "c)");
            return iStr;
        }
    }
}
//-------------------------------------------------------------------------
// 棒読みちゃんアクセスクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
