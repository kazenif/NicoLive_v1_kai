//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NicoLive
{
    partial class Form1
    {
        //-------------------------------------------------------------------------
        // Twitter投稿用
        //-------------------------------------------------------------------------
        private void TwitterPoster(bool iStart)
        {
            string token = Properties.Settings.Default.tw_token;

            if (token.Length == 0 )
                return;

            Thread th = new Thread(delegate()
            {
                using (Twitter tw = new Twitter())
                {
                    bool start = iStart;

                    string msg = (start) ? Properties.Settings.Default.tw_start : Properties.Settings.Default.tw_end;

                    string uri = "http://nico.ms/" + LiveID;

                    msg = msg.Replace("@URL", uri);

                    // タイトル取得
                    if (msg.Contains("@TITLE"))
                    {
                        using (WebClient wc = new WebClient())
                        {
                            try
                            {
                                Stream stm = wc.OpenRead(uri);
                                Encoding enc = Encoding.GetEncoding("utf-8");
                                StreamReader sr = new StreamReader(stm, enc);

                                string html = sr.ReadToEnd();

                                int st = html.IndexOf("<title>");
                                int en = html.IndexOf("</title>");

                                if (st > 0 && en > 0)
                                {
                                    string title = html.Substring(st + 7, en - st - 17);
                                    if (title == null || title.Length == 0)
                                        return;
                                    msg = msg.Replace("@TITLE", title);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("TwWorker_DoWork:" + ex.Message);
                                return;
                            }
                        }
                    }

                    Debug.WriteLine(msg);

                    // Twitterは140文字制限
                    if (msg.Length > 140)
                    {
                        MessageBox.Show("メッセージが長すぎてTwitterにポスト出来ません。", "NicoLive");
                        mTwPost = true;
                        return;
                    }

                    // Twitterへポスト
                    if (msg.Length > 0)
                    {
                        if (tw.Post(msg, "#nicolive"))
                        {
                            mTwPost = start;
                        }
                    }
                }
            });
            th.Start();
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
