//-------------------------------------------------------------------------
// 自動枠取り
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using System.Diagnostics;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    public partial class Wakutori : Form
    {
        private Form1 mOwner = null;

        private readonly string MY_URL = "http://live.nicovideo.jp/my";
        private readonly string EDIT_URL = "http://live.nicovideo.jp/editstream";
        private readonly string WATCH_URL = "http://live.nicovideo.jp/watch";
        private readonly string LOGIN_URL = "https://secure.nicovideo.jp/secure/login_form";

        private string mReuseUrl = "";

        private bool mAutoWaku = false;  // 自動枠取り（画像認証まで）

        private bool mAutoWakumachi = false; // 自動枠待ち(前枠の続きなどで必要)

        //private Thread mCaptchaThread = null;   // CaptchaBreaker用スレッド

        private string mCaptcha = "";

        private int mReloadWait = 0;

        private bool mPostTweet = false;

        private bool mWakumachiClick = false;

        // メッセージ設定
        private MessageSettings mMsg;

        public bool AutoWaku
        {
            set { mAutoWaku = value; }
            get { return mAutoWaku; }
        }

        // 自動枠待ちモード設定
        public bool AutoWakumachi
        {
            set { mAutoWakumachi = value; }
            get { return mAutoWakumachi; }
        }

        public Form1 MyOwner
        {
            set { mOwner = value; }
            get { return mOwner; }
        }

        //-------------------------------------------------------------------------
        // コンポーネント初期化
        //-------------------------------------------------------------------------
        public Wakutori()
        {
            InitializeComponent();
            mMsg = MessageSettings.Instance;
        }

         //-------------------------------------------------------------------------
        // ショートカットキー
        //-------------------------------------------------------------------------
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((int)keyData == (int)Keys.F1)
            {
                this.mBackBtn.PerformClick();
                return true;
            }else
            if ((int)keyData == (int)Keys.F2)
            {
                this.mFwdBtn.PerformClick();
                return true;
            }else
            if ((int)keyData == (int)Keys.F3)
            {
                this.mReloadBtn.PerformClick();
                return true;
            }else
            if ((int)keyData == (int)Keys.F4)
            {
                this.mEnableBtn.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        //-------------------------------------------------------------------------
        // フレームロード時
        //-------------------------------------------------------------------------
        private void Wakutori_Load(object sender, EventArgs e)
        {
            mReuseUrl = "";
            mPostTweet = false;

            if (mAutoWaku && Properties.Settings.Default.auto_wakutori)
            {
                // 自動で「使い回す」ページへ遷移
                string lv = mOwner.LiveID;
                if (lv.Length <= 0)
                {
                    this.mBrowser.Navigate(MY_URL);
                }
                else
                {
                    //lv = lv.Substring(2);
                    //mReuseUrl = EDIT_URL + "?" + "reuseid=" + lv;
                    //this.mBrowser.Navigate(mReuseUrl);
                    this.mBrowser.Navigate(EDIT_URL);
                }
            }
            else
            {
                // マイページへ遷移
                this.mBrowser.Navigate(MY_URL);
            }
        }

        //-------------------------------------------------------------------------
        // DNSエラー時のリロード
        //-------------------------------------------------------------------------
		private void DnsReload()
		{
			if (this.mBrowser.Url != null && mReloadWait == 0)
			{
				string uri = this.mBrowser.Document.Url.ToString();

				if (uri.StartsWith("res://ieframe.dll/dnserrordiagoff_webOC.htm#") )
					mReloadWait = 1;
			}

			if (mReloadWait >= 1)
			{
				mReloadWait++;
				if (mReloadWait > 20)
				{
					mReloadWait = 0;
                    if (this.mBrowser.CanGoBack)
                        this.mBrowser.GoBack();
                    else
				    	this.mBrowser.Refresh();
				}
			}
		}

        //-------------------------------------------------------------------------
        // Captcha入力
        //-------------------------------------------------------------------------
		private void InputCaptcha()
		{
            if (mCaptcha.Length <= 0) return;

            if (SetElementTextByName("captcha", mCaptcha))
            {
                InvokeButtonById("submit_ok");
            }
            mCaptcha = "";
        }

        //-------------------------------------------------------------------------
        // ステータス更新
        //-------------------------------------------------------------------------
		private void UpdateStatus()
		{
			this.mEnableLabel.Text = (this.mEnableBtn.Checked) ? "自動枠取り動作中" : "自動枠取り待機中";
			this.mEnableLabel.ForeColor = (this.mEnableBtn.Checked) ? Color.Red : Color.Black;

			if (this.mBrowser.Url != null && this.mBrowser.Url.ToString().StartsWith(EDIT_URL))
			{
				this.mEnableBtn.Enabled = true;
			}
			else
			{
				this.mEnableBtn.Enabled = false;
			}

			if (mBrowser.Url != null /*&& this.mEnableBtn.Checked*/ )
			{
				string uri = mBrowser.Url.ToString();

				if (uri.StartsWith(WATCH_URL))
				{
					// 枠取り成功
					SystemSounds.Beep.Play();
					int idx = uri.IndexOf("lv");
					uri = uri.Substring(idx);

					using (Bouyomi bm = new Bouyomi())
					{
						bm.Talk(mMsg.GetMessage("枠が取れたよ"));
					}

					// 接続
                    this.Invoke((MethodInvoker)delegate()
                    {
                        this.mOwner.LiveID = uri;
                        this.mOwner.Connect(false);
                    });
					this.mUITimer.Enabled = false;

                    try
                    {
                        this.Hide();
                        this.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Close:"+e.Message);
                    }
                }
                // 2012.05.12 順番待ちの表示に無理矢理移行するため。
                else if (uri.StartsWith(MY_URL))
                {
                    HtmlElementCollection all = this.mBrowser.Document.All;
                    foreach (HtmlElement elem in all)
                    {
                        // [番組情報を編集する]クリック
                        if (elem.GetAttribute("title") == "番組情報を編集する" &&
                            mAutoWakumachi)
                        {
                            if (!mWakumachiClick)
                            {
                                elem.InvokeMember("click");
                                mWakumachiClick = true;
                            }
                            return;
                        }
                        // [生放送ページへ戻る]クリック
                        else if (elem.GetAttribute("title") == "生放送ページへ戻る" &&
                                (this.mEnableBtn.Checked || mAutoWakumachi))
                        {
                            if (!mWakumachiClick)
                            {
                                elem.InvokeMember("click");
                                mWakumachiClick = true;
                            }
                            return;
                        }
                    }
                }
                else if (uri.StartsWith(EDIT_URL))
                {
                    if (this.mBrowser.Document.GetElementById("res_done") != null)
                    {
                        HtmlElementCollection all = this.mBrowser.Document.All;
                        HtmlElementCollection elem = all.GetElementsByName("blog_parts");

                        if (elem.Count > 0)
                        {
                            string val = elem[0].GetAttribute("value");
                            int idx1 = val.IndexOf("lv");
                            Debug.WriteLine(val);

                            if (idx1 > 0)
                            {
                                int idx2 = val.IndexOf("\"", idx1);
                                if (idx2 > 0)
                                {
                                    string lv = val.Substring(idx1, idx2 - idx1);

                                    Debug.WriteLine(lv);

                                    this.Invoke((MethodInvoker)delegate()
                                    {
                                        this.mOwner.LiveID = lv;
                                        this.mOwner.Connect(false);
                                    });
                                    this.mUITimer.Enabled = false;

                                    try
                                    {
                                        this.Hide();
                                        this.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }

                                }
                            }
                        }
                    }
                }
			}
		}

        //-------------------------------------------------------------------------
        // タイマー
        //-------------------------------------------------------------------------
        private void mUITimer_Tick(object sender, EventArgs e)
        {
            if (this.mBrowser == null) return;
            if (this.mBrowser.Url == null) return;

            //if (this.mBrowser.IsBusy) return;
            
            CheckElementById("kiyaku_accept", true);

            string uri = mBrowser.Url.ToString();

            // 画像認証ページ
            if (uri.StartsWith(EDIT_URL) && this.mEnableBtn.Checked)
            {
                // POSTボタン
                InvokeButtonById("submit_ok");
            }

			// DNSエラー時のリロード
			DnsReload();
        
            // キャプチャ入力
			InputCaptcha();
        
            // その他情報変更
			UpdateStatus();

            // 放送開始クリック
            ClickStartBoardcast();
        }

        //-------------------------------------------------------------------------
        // 指定テキスト位置までスクロール
        //-------------------------------------------------------------------------
        private void ScrollToText(string iElemID)
        {
            if (mBrowser.Document == null) return;

            HtmlElement elem = this.mBrowser.Document.GetElementById(iElemID);
            if (elem != null)
            {
                elem.ScrollIntoView(false);
            }
        }

        //-------------------------------------------------------------------------
        // ドキュメント読み込み完了
        //-------------------------------------------------------------------------
        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            CheckDocument(true);
        }

        //-------------------------------------------------------------------------
        // 指定エレメントのテキストを変更する
        //-------------------------------------------------------------------------
        private bool SetElementTextByName(string iElemName, string iText)
        {
            if (this.mBrowser.Document == null) return false;

            HtmlElementCollection all = this.mBrowser.Document.All;
            HtmlElementCollection elem = all.GetElementsByName(iElemName);

            if (elem.Count > 0)
            {
                elem[0].InnerText = iText;
                return true;
            }
            return false;
        }
        //-------------------------------------------------------------------------
        // 指定エレメントのテキストを取得する
        //-------------------------------------------------------------------------
        private string GetElementTextByName(string iElemName)
        {
            if (this.mBrowser.Document == null) return null;

            HtmlElementCollection all = this.mBrowser.Document.All;
            HtmlElementCollection elem = all.GetElementsByName(iElemName);

            if (elem.Count > 0)
            {
                return elem[0].InnerText;
            }
            return null;
        }

        //-------------------------------------------------------------------------
        // エレメントがあるかどうか
        //-------------------------------------------------------------------------
        private bool ContainElementByName(string iElemName)
        {
            if (this.mBrowser.Document == null) return false;

            HtmlElementCollection all = this.mBrowser.Document.All;
            HtmlElementCollection forms = all.GetElementsByName(iElemName);

            return (forms.Count > 0);
        }
        //-------------------------------------------------------------------------
        // エレメントがあるかどうか
        //-------------------------------------------------------------------------
        private bool ContainElementById(string iElemID)
        {
            if (this.mBrowser.Document == null) return false;

            HtmlElement check = this.mBrowser.Document.GetElementById(iElemID);

            return (check != null);
        }
        //-------------------------------------------------------------------------
        // チェックボックスのチェック
        //-------------------------------------------------------------------------
        private void CheckElementById(string iElemID, bool iChecked)
        {
            if (this.mBrowser.Document == null) return;

            HtmlElement check = this.mBrowser.Document.GetElementById(iElemID);
            if (check == null) return;

            if (iChecked)
            {
                if (check.GetAttribute("checked") == "False")
                    check.InvokeMember("click");
            }
            else
            {
                if (check.GetAttribute("checked") == "True")
                    check.InvokeMember("click");
            }
        }

        //-------------------------------------------------------------------------
        // チェックボックスのチェック
        //-------------------------------------------------------------------------
        private bool IsCheckElementById(string iElemID)
        {
            if (this.mBrowser.Document == null) return false;

            HtmlElement check = this.mBrowser.Document.GetElementById(iElemID);
            if (check == null) return false;

            return (bool)(check.GetAttribute("checked") == "True");
        }


        //-------------------------------------------------------------------------
        // ボタン実行
        //-------------------------------------------------------------------------
        private void InvokeButtonById(string iElemID)
        {
            if (this.mBrowser.Document == null) return;

            HtmlElement btn = this.mBrowser.Document.GetElementById(iElemID);
            if (btn != null)
                btn.InvokeMember("click");
        }
        //-------------------------------------------------------------------------
        // 放送開始
        //-------------------------------------------------------------------------
        private void ClickStartBoardcast()
        {
            try
            {
                if (this.mBrowser == null) return;
                if (this.mBrowser.Url == null) return;

                if (this.mBrowser.IsBusy) return;
                string uri = mBrowser.Url.ToString();

                Debug.WriteLine(uri);

                // 半自動枠取りボタンがチェックされてない
                // かつ、自動枠待ちモードではない (2012.05.12)
                if (!this.mEnableBtn.Checked && !mAutoWakumachi) return;

                // queueカウント待ち
                HtmlElement check = this.mBrowser.Document.GetElementById("que_count");
                if( check != null )
                {   
                    int queue = 0;
                    int.TryParse(check.InnerText, out queue);
                    
                    // 100人以上順番待ちの時はTweet
                    if (queue >= 102)
                    {
                        mPostTweet = true;
                    }

                    // 順番待ち100人未満に成ったらTweet
                    if (mPostTweet && queue < 100)
                    {
                        Utils.TweetWait(queue);
                        mPostTweet = false;
                    }

                    if (check.InnerText == "0")
                    {
                        // 入場クリック
                        HtmlElementCollection all = this.mBrowser.Document.All;
                        foreach (HtmlElement elem in all)
                        {
                            if (elem.GetAttribute("title") == "入場して放送を開始する")
                            {
                                elem.InvokeMember("click");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        //-------------------------------------------------------------------------
        // 枠取り
        //-------------------------------------------------------------------------
        private void CheckDocument(bool iLoaded)
        {
            if (this.mBrowser.Url == null) return;

            string uri = mBrowser.Url.ToString();

            Debug.WriteLine(uri);
            
            // 画像認証ページ
            if (uri.StartsWith(EDIT_URL))
            {
                if (mAutoWaku && Properties.Settings.Default.auto_wakutori)
                    this.mEnableBtn.Checked = true;
            }

            // エラーページの場合は戻る
            if (ContainElementById("error_box"))
            {
                if (mAutoWaku && Properties.Settings.Default.auto_wakutori)
                    this.mReloadBtn.PerformClick();
                else
                    this.mBackBtn.PerformClick();
                return;
            }
            
            if( mReuseUrl.Length > 0 && uri.Equals(mReuseUrl))
            {
                // ＩＤとパスワード入力
                SetElementTextByName("mail", Properties.Settings.Default.user_id);
                SetElementTextByName("password", Properties.Settings.Default.password);
                // ログインボタンを押す
                InvokeButtonById("submit");
            }
            else if (uri.StartsWith(LOGIN_URL))
            {
                // ＩＤとパスワード入力
                SetElementTextByName("mail", Properties.Settings.Default.user_id);
                SetElementTextByName("password", Properties.Settings.Default.password);
                // ログインボタンを押す
                InvokeButtonById("login_submit");
            }
            else if (uri.StartsWith(MY_URL))
            {
                // マイページ
                ScrollToText("toTop");
                // ＩＤとパスワード入力
                SetElementTextByName("mail", Properties.Settings.Default.user_id);
                SetElementTextByName("password", Properties.Settings.Default.password);
                // ログインボタンを押す
                InvokeButtonById("submit");
            }
            else if (uri.StartsWith(EDIT_URL))
            {
                // 画像認証位置までスクロール
                if (ContainElementById("error_message"))
                {
                    ScrollToText("captcha");
                    HtmlElement btn = this.mBrowser.Document.GetElementById("error_message");
                    if (btn != null && btn.InnerText.Contains("既に順番待ちに並んでいるか"))
                    {
                        return;
                    }
                }
            }

            // 半自動枠取りボタンがチェックされてない
            if (!this.mEnableBtn.Checked) return;

            // 順番待ち
            if (ContainElementById("waiting"))
            {
                HtmlElementCollection all = this.mBrowser.Document.All;
                foreach (HtmlElement elem in all)
                {
                    if (elem.GetAttribute("title") == "最後尾に並ぶ")
                    {
                        elem.InvokeMember("click");
                        return;
                    }
                }
                return;
            }
        }

        //-------------------------------------------------------------------------
        // 半自動枠取り有効かボタン
        //-------------------------------------------------------------------------
        private void EnableBtn_Click(object sender, EventArgs e)
        {
            CheckDocument(false);
        }
        //-------------------------------------------------------------------------
        // 戻るボタン
        //-------------------------------------------------------------------------
        private void BackBtn_Click(object sender, EventArgs e)
        {
            this.mBrowser.GoBack();
        }
        //-------------------------------------------------------------------------
        // 進むボタン
        //-------------------------------------------------------------------------
        private void FwdBtn_Click(object sender, EventArgs e)
        {
            this.mBrowser.GoForward();
        }

        //-------------------------------------------------------------------------
        // リロードボタン
        //-------------------------------------------------------------------------
        private void ReloadBtn_Click(object sender, EventArgs e)
        {
            this.mBrowser.Refresh();
        }
        //-------------------------------------------------------------------------
        // プログレスバー更新
        //-------------------------------------------------------------------------
        private void Browser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (e.MaximumProgress == 0)
            {
                this.mProgress.Visible = false;
            }
            else
            {
                this.mProgress.Visible = true;
                this.mProgress.Value = (int)((e.CurrentProgress / e.MaximumProgress) * 100);
            }
        }
    }
}
//-------------------------------------------------------------------------
// 自動枠取り
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
