//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace NicoLive
{
    partial class Form1
    {

        //-------------------------------------------------------------------------
        // フォームロード
        //-------------------------------------------------------------------------
        private void Form1_Load(object sender, EventArgs e)
        {
            mNico = Nico.Instance;
            mBouyomi = new Bouyomi();

            mSpeakList = new List<string>();
            mGatherUserID = new List<string>();
            mCommentForm = new CommentForm(this);

            // 色
            mCommentList.BackgroundColor = Properties.Settings.Default.back_color;
            mCommentList.RowsDefaultCellStyle.ForeColor = Properties.Settings.Default.text_color;

            mUid = UserID.Instance;
            //mNG = NG.Instance;
            mUIStatus = UIStatus.Instance;
            mLiveInfo = LiveInfo.Instance;
            mRes = Response.Instance;

            // キャプチャ突破フラグを落としておく
            //Properties.Settings.Default.auto_captcha = false;

            // ＮＧワード読み込み
            //mNG.LoadNGWord();

            // Response読み込み
            mRes.LoadResponse();

            // IDロード
            UserID uid = UserID.Instance;
            uid.LoadUserID(true);
            uid.LoadUserID(false);

            // フォント設定
            Utils.SetCommentFont(ref mCommentList);

            // カラム幅設定
            this.mCommentList.Columns[0].Width = Properties.Settings.Default.column_width_0;
            this.mCommentList.Columns[1].Width = Properties.Settings.Default.column_width_1;
            this.mCommentList.Columns[2].Width = Properties.Settings.Default.column_width_2;
            this.mCommentList.Columns[3].Width = Properties.Settings.Default.column_width_3;

            // 最後に取得した放送ＩＤを設定
            this.mLiveID.Text = Properties.Settings.Default.last_lv;

            // バージョンチェック開始
            CheckVersion();

            // 来場者数更新用ワーカースタート
            this.mUpdateInfoWorker.RunWorkerAsync();

            // コメント処理ワーカースタート
            this.mCommentWorker.RunWorkerAsync();

            // 自動延長ワーカースタート
            this.mAutoExtendWorker.RunWorkerAsync();

            // ユーザー名収集ワーカー
            this.mGatherUserIDWorker.RunWorkerAsync();

            mWakutoriBtn.Text = mWakutoriBtn.Text + "(F1)";
            mCopyBtn.Text = mCopyBtn.Text + "(F2)";
            mImakokoBtn.Text = mImakokoBtn.Text + "(F3)";
            mCommentBtn.Text = mCommentBtn.Text + "(F4)";
            mAutoExtendBtn.Text = mAutoExtendBtn.Text + "(F5)";
            mContWaku.Text = mContWaku.Text + "(F6)";
            mVisitorBtn.Text = mVisitorBtn.Text + "(F7)";
            mBouyomiBtn.Text = mBouyomiBtn.Text + "(F8)";

            // ウィンドステート復帰
            this.Width = Properties.Settings.Default.mw_size.Width;
            this.Height = Properties.Settings.Default.mw_size.Height;
            this.Top = Properties.Settings.Default.mw_pos.Y;
            this.Left = Properties.Settings.Default.mw_pos.X;
            this.WindowState = Properties.Settings.Default.mw_state;

            // 棒読みちゃん起動
            Bouyomi.Launch();

            // 今ココ起動
            ImakokoNow.Launch();
        }

        //-------------------------------------------------------------------------
        // フォームクローズ
        //-------------------------------------------------------------------------
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FMLE.Stop();

            if (mLogger != null)
            {
                try
                {
                    mLogger.Close();
                    mLogger = null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            // ID保存
            mUid.SaveUserID(false);
            mUid.SaveUserID(true);

            // カラム幅保存
            Properties.Settings.Default.column_width_0 = this.mCommentList.Columns[0].Width;
            Properties.Settings.Default.column_width_1 = this.mCommentList.Columns[1].Width;
            Properties.Settings.Default.column_width_2 = this.mCommentList.Columns[2].Width;
            Properties.Settings.Default.column_width_3 = this.mCommentList.Columns[3].Width;

            // ウィンドステート保存
            if (this.WindowState == FormWindowState.Normal)
            {
                Size size = new Size( this.Width,this.Height);
                Properties.Settings.Default.mw_size = size;
                Point pos = new Point(this.Top, this.Left);
                Properties.Settings.Default.mw_pos = pos;
            }
            Properties.Settings.Default.mw_state = this.WindowState;

            // セーブ
            Properties.Settings.Default.Save();

            // 棒読みちゃん終了
            Bouyomi.Exit();

            // 今ココ終了
            ImakokoNow.Exit();

            // iTunes終了
            ITunes.Close();

            mNico = null;
        }

        //-------------------------------------------------------------------------
        // 初回表示時
        //-------------------------------------------------------------------------
        private void Form1_Shown(object sender, EventArgs e)
        {

            // ステータス表示設定
            UpdateStatusVisibility();

            // 自動接続
            if (mAutoConnect)
                Connect(false);
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
