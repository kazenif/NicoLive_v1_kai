//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace NicoLive
{
    partial class Form1
    {


        //-------------------------------------------------------------------------
        // ログイン用ワーカー
        //-------------------------------------------------------------------------
        private void LoginWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string user_id = Properties.Settings.Default.user_id;
            string passwd = Properties.Settings.Default.password;

            string live_id = LiveID;
            int try_cnt = 0;
            int try_new_connect=0;

GET_LIVE_ID:
            // LVが入ってないときは自動で取ってくる
            if (live_id.Length <= 2)
            {
                live_id = GetCurrentLive();
                if (live_id.Length > 2)
                {
                    this.Invoke((Action)delegate()
                    {
                        LiveID = live_id;
                    });
                }
            }

            if (live_id.Length <= 2)
            {
                try_cnt++;
                if (try_cnt < 10)
                {
                    Thread.Sleep(500);
                    goto GET_LIVE_ID;
                }
            }

            if (live_id.Length <= 2)
                goto END;

            try_cnt = 0;

            mCurrentLiveID = live_id;

            // ログイン
            if (mSkipLogin) mNico.IsLogin = true;
            bool login = (mSkipLogin)?true:mNico.Login(user_id, passwd);
            if (login)
            {
GET_COMMENT:
                if( !mNico.IsLogin )
                    mNico.Login(user_id, passwd);

                //最終コメントレス番号リセット
                mLastChatNo = 0;
                
                // コメントサーバーに接続
                NicoErr err = mNico.ConnectToCommentServer(live_id, 50);


                switch (err)
                {
                    case NicoErr.ERR_COULD_NOT_CONNECT_COMMENT_SERVER:
                        if (try_cnt < 30)
                        {
                            try_cnt++;
                            Thread.Sleep(1000);
                            Debug.WriteLine("Retry: " + try_cnt);
                            goto GET_COMMENT;
                        }
                        MessageBox.Show("コメントサーバーにログインできませんでした。", "NicoLive");
                        break;
                    case NicoErr.ERR_COMMENT_SERVER_IS_FULL:
                        MessageBox.Show("コメントサーバーが満員でログインできません。", "NicoLive");
                        break;
                    case NicoErr.ERR_NOT_LIVE:
                        try_new_connect++;

                        if (try_new_connect < 30)
                        {
                            Thread.Sleep(500);
                            live_id="";
                            goto GET_LIVE_ID;
                        }
                        MessageBox.Show("指定した放送は終了しています。", "NicoLive");
                        break;
                    case NicoErr.ERR_COMMUNITY_ONLY:
                        MessageBox.Show("コミュニティー限定です。", "NicoLive");
                        break;
                }

                if (err == NicoErr.ERR_NO_ERR)
                {
                    this.Invoke((Action)delegate()
                    {
                        mNico.WakutoriMode = false;
                   
                        // プレイヤーロード
                        GetPlayer();

                        // 開演待ちスキップするか？
                        if (Properties.Settings.Default.skip5min)
                        {
                            // 情報を取得
                            mLiveInfo.GetInfo(live_id);

                            // 開演5分待ちスキップ
                            mNico.LiveStart(live_id, mLiveInfo.Token);
                        }
                       
                        // ロガー起動
                        if (Properties.Settings.Default.save_log)
                        {
                            try
                            {
                                Directory.CreateDirectory("log");
                                mLogger = new StreamWriter("log/" + LiveID + ".xml", true);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("LoginWorker_DoWork:" + ex.Message);
                            }
                        }
                        if (mViewer != null && !mViewer.IsDisposed)
                        {
                            mViewer.SetLiveID(LiveID);
                        }
                    });
                }
                else
                {
                    mNico.Close();
                }
            }
            else
            {
                MessageBox.Show("ログインできませんでした。", "NicoLive");
            }
END:
            this.Invoke((Action)delegate()
            {
                this.mConnectBtn.Enabled = true;
                this.mLastChatTime = DateTime.Now;
                this.mLastCompctTime = DateTime.Now;
                this.mCompactForcast = false;
                this.mStartFME = false;
                this.mDisconnect = false;
            });
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
