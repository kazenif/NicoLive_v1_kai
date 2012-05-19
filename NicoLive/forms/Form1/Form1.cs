//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;


//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    delegate void SendCommentDelegate(string iComment, bool iAdmin);
    // フォームクラス
    public partial class Form1 : Form
    {
        #region 変数
        // ニコニコアクセスクラス
        private Nico mNico = null;

        // 棒読みちゃんアクセスクラス
        private Bouyomi mBouyomi = null;

        // コテハン用クラス
        private UserID mUid = null;

        // 放送情報管理クラス
        private LiveInfo mLiveInfo = null;

        // 読み上げコメントリスト
        private List<string> mSpeakList = null;
        static readonly object mSpeakLock = new object();

        // ＮＧワード
        //private NG mNG = null;

        // ユーザー名取得スレッド
        private List<string> mGatherUserID = null;
        private ReaderWriterLock mGatherLock = new ReaderWriterLock();

        // 前のログイン状態
        private bool mPrevLogin = false;

        // 来場者がこれ以上になったとき読み上げる
        private int mTargetVisitorCnt = 10;

        // 来場者数
        private int mVisitorCnt = 0;

        // 自動接続
        public bool mAutoConnect = false;

        // 自動再接続
        public bool mAutoReconnectOnGoing = false;

        // console.swfのuri
        private readonly string mConsoleUri = "http://live.nicovideo.jp/console.swf?v=";

        // Twitterで放送開始をポストしたかどうか
        private bool mTwPost = false;

        // 自分の配信かどうか
        private bool mOwnLive = false;

        // 切断済みかどうか
        private bool mDisconnect = false;

        // 最後にコメントを読み上げた時間
        //private DateTime mSpeakTime;

        // 残り3分通知したかどうか
        private bool mTalkLimit = false;

        // ロガー
        private StreamWriter mLogger = null;

        // 配信開始ボタンがおされた
        private bool mPushStart = false;

        // 延長ボタンがおされた
        private bool mPushExtend = false;

		// 延長されたかどうか
		private bool mIsExtend = false;

        // NetworkSpeed計測用
        private UIStatus mUIStatus;

        // メッセージ設定
        private MessageSettings mMsg;

        // 外部コメントウィンド
        private CommentForm mCommentForm;

        // 返信クラス
        private Response mRes;

        // keepaliveコメントを送信するタイミング（5分おき）
        private readonly int KEEP_ALIVE_TIME = 5;

        // 最後にコメントを受信した時間
        private DateTime mLastChatTime;

        // /compact を送信するタイミング(11分おき)
        private int COMPACT_TIME = 11;

        // compact 予告送信フラグ
        private bool mCompactForcast = false;

        // 最後にコンパクトを送信した時刻
        private DateTime mLastCompctTime;

		// FME開始フラグ
		private bool mStartFME = false;

        private bool mSkipLogin;

        private UInt32 mNextGC;

        private Viewer mViewer = null;

        private bool mSkipBouyomi = true;

        private string mCurrentLiveID = "";

        // 無料延長用
        private SaleList mFreeExtendItem;

        // 最終コメントレス番号
        private int mLastChatNo;

        #endregion

        #region 列挙列
        // コメントカラム
        private enum CommentColumn : int
        {
            COLUMN_NUMBER = 0,			// 番号
            COLUMN_ID,					// ＩＤ
            COLUMN_HANDLE,				// コテハン
            COLUMN_COMMENT,				// コメント
        }
        #endregion

        //-------------------------------------------------------------------------
        // 放送ＩＤ
        //-------------------------------------------------------------------------
        public string LiveID
        {
            set { this.mLiveID.Text = value; }
            get { return this.mCurrentLiveID; }
        }

        //-------------------------------------------------------------------------
        // コンポーネント初期化
        //-------------------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();

            Properties.Settings.Default.auto_wakutori = true;
            
            // メッセージロード
            mMsg = MessageSettings.Instance;
            mMsg.Load();
        }

        //-------------------------------------------------------------------------
        // ステータス表示設定
        //-------------------------------------------------------------------------
        public void UpdateStatusVisibility()
        {
            mTotalCnt.Visible = Properties.Settings.Default.TotalCnt_view;
            mActiveCnt.Visible = Properties.Settings.Default.ActiveCnt_view;
            mUniqCnt.Visible = Properties.Settings.Default.UniqCnt_view;
            mCpuInfo.Visible = Properties.Settings.Default.CpuInfo_view;
            mBattery.Visible = Properties.Settings.Default.Battery_view;
            mUpLink.Visible = Properties.Settings.Default.UpLink_view;

            // サブウインドウもついでに更新
            if (mCommentForm != null && mCommentForm.Visible)
            {
                mCommentForm.Invoke((Action)delegate()
                {
                    mCommentForm.UpdateStatusVisibility();
                });
            }
        }

        //-------------------------------------------------------------------------
        // 接続
        //-------------------------------------------------------------------------
        public void Connect(bool iSkipLogin)
        {
            if (mLoginWorker.IsBusy)
                return;
            mSkipBouyomi = true;
            mSkipLogin = iSkipLogin;
            mPushStart = false;
            mPushExtend = false;
			mIsExtend = false;
            mStartFME = false;
            mPrevLogin = false;
            mTwPost = false;
            mOwnLive = false;
            mDisconnect = false;
            mTalkLimit = false;
            mNextGC = 0;
            mNico.Comment = "";
            mTargetVisitorCnt = 10;

            mSpeakList.Clear();
            mLiveInfo.Clear();

            mCurrentLiveID = "";
            // 放送ＩＤをフォーマット
            mLiveID.Text = ParseLiveID();

            // 切断
            mNico.Close();

            if (mLogger != null)
            {
                mLogger.Close();
                mLogger = null;
            }

            // 放送IDが空
            if (this.mLiveID.Text.Length == 0)
            {
                MessageBox.Show("放送ＩＤが空です。","NicoLive");
                return;
            }


            // 外部コメントウィンド
            this.Invoke((Action)delegate()
            {
                this.mCommentList.Rows.Clear();
           		this.mConnectBtn.Enabled = false;
                mCommentForm.Clear();
            }); 
            
            mLastChatTime = DateTime.Now;
            mLastCompctTime = DateTime.Now;
            mCompactForcast = false;

            // iTunesとの接続を開始
            if (Properties.Settings.Default.use_iTunes) ITunes.connect();

            // iTunesのコメントデリゲート設定
            ITunes.setSendCommentDelegate(SendComment);

            // プレイリストを設定
            ITunes.setPlaylistName(Properties.Settings.Default.itunes_playlist);

            GC.Collect();

            // ログインワーカースタート
            mLoginWorker.RunWorkerAsync();
        }

        //-------------------------------------------------------------------------
        // 放送ＩＤ取得
        //-------------------------------------------------------------------------
        public string ParseLiveID()
        {
            string live_id = "lv";
            if (this.mLiveID == null || this.mLiveID.Text == null) return "";

            string tmp = this.mLiveID.Text;

            int idx = tmp.LastIndexOf("lv");
            int len = tmp.Length;

            for (int i = idx + 2; i < len; i++)
            {
                if ('0' <= tmp[i] && tmp[i] <= '9')
                    live_id += tmp[i].ToString();
                else
                    break;
            }

            return live_id;
        }

        //-------------------------------------------------------------------------
        // 現在放送中のLV取得
        //-------------------------------------------------------------------------
        private string GetCurrentLive()
        {
            Nico nico = Nico.Instance;

            string result = nico.GetCurrentLive(
                                Properties.Settings.Default.user_id,
                                Properties.Settings.Default.password
                             );

            nico = null;
            return result;
        }
        //-------------------------------------------------------------------------
        // 配信プレイヤー取得
        //-------------------------------------------------------------------------
        private void GetPlayer()
        {
            string uri = mConsoleUri + LiveID;
            if( uri.Equals(mFlash.Movie))
            {
                mFlash.LoadMovie(0, mConsoleUri);
            }
            mFlash.LoadMovie(0, uri);
        }

        //-------------------------------------------------------------------------
        // 来場者数通知
        //-------------------------------------------------------------------------
        private void SpeakVisitor()
        {
            // 読み上げが有効になってない
            if (!this.mVisitorBtn.Checked)
                return;

            // 読み上げる
            int cnt = mVisitorCnt;
            if (cnt <= 0) return;

            if (mTargetVisitorCnt <= cnt)
            {
                string str = String.Format(mMsg.GetMessage("只今の来場者すう、{0}人です"), cnt);
                mBouyomi.Talk(str);

                mTargetVisitorCnt = cnt + 10;
                mTargetVisitorCnt /= 10;
                mTargetVisitorCnt *= 10;
            }
        }

        //-------------------------------------------------------------------------
        // 自動枠取り画面起動
        //-------------------------------------------------------------------------
        private void MakeWakutori(bool iAuto)
        {
            Wakutori mk = new Wakutori();
            mk.MyOwner = this;
            mk.AutoWaku = iAuto;
            mk.Show();
        }

        //-------------------------------------------------------------------------
        // 自動枠取待ち画面起動 (2012.05.12)
        //-------------------------------------------------------------------------
        public void MakeWakumachi()
        {
            Wakutori mk = new Wakutori();
            mk.MyOwner = this;
            mk.AutoWakumachi = true;
            mk.Show();
        }

        private void mXSplitTimer_Tick(object sender, EventArgs e)
        {
            XSplit.HandlingStatus(LiveID, this);
            NicoLiveEncoder.HandlingStatus(LiveID, this);
        }
    }
}
//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
