//-------------------------------------------------------------------------
// ニコニコアクセスクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Web;
using Hal.CookieGetterSharp;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    //-------------------------------------------------------------------------
    // エラーステータス
    //-------------------------------------------------------------------------
    public enum NicoErr
    {
        ERR_NO_ERR = 0,                             // エラー無し
        ERR_COULD_NOT_CONNECT_COMMENT_SERVER,       // コメントサーバーにログインできない
        ERR_COMMENT_SERVER_IS_FULL,                 // コメントサーバーが満員
        ERR_NOT_LIVE,                               // 放送中じゃない
        ERR_COMMUNITY_ONLY                          // コミュ限
    };

    public enum WakuErr
    {
        ERR_NO_ERR = 0,
        ERR_JUNBAN,                                 // 順番待ち
        ERR_KONZATU,                                // 混雑中
        ERR_TAJU,                                   // 多重投稿
        ERR_MAINTE,                                 // メンテ中
        ERR_UNKOWN,                                 // ハンドルしてないエラー
        ERR_ALREADY_LIVE,                           // 既に放送中
        ERR_KIYAKU,                                 // 規約確認
        ERR_LOGIN,                                  // ログインしてない
        ERR_JUNBAN_ALREADY,                         // 既に順番待ち
        ERR_JUNBAN_WAIT,                            // 順番待ち
		ERR_MOJI,									// 文字数制限エラー
        ERR_TAG,                                    // タグエラー
    };

    struct SaleList
    {
        public string mCode;
        public string mPrice;
        public string mNum;
        public string mItem;
        public string mLabel;
    }

    //-------------------------------------------------------------------------
    // ニコ生アクセスクラス
    //-------------------------------------------------------------------------
    class Nico
    {
        // for IE component cookie
        [DllImport("wininet.dll", EntryPoint = "InternetSetCookie", ExactSpelling = false, CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool InternetSetCookie(string lpszUrl, string lpszCookieName, string lpszCookieData);

        [DllImport("kernel32.dll")]
        public static extern Int32 GetLastError();
        
        [DllImport("msvcrt.dll", CharSet = CharSet.Auto)]
        public static extern long time(IntPtr tm);

        // Cookie
        private CookieContainer mCookieLogin = null;

        // uri of api to logging in
        private readonly string URI_LOGIN = "https://secure.nicovideo.jp/secure/login?site=niconico";

        // getplayerstatus uri
        private readonly string URI_GETPLAYERSTATUS = "http://live.nicovideo.jp/api/getplayerstatus?v=";

        // getpublishstatus uri
        private readonly string URI_GETPUBLISHSTATUS = "http://live.nicovideo.jp/api/getpublishstatus?v=";

		// profile uri
        private readonly string URI_GETFMEPROFILE = "http://live.nicovideo.jp/api/getfmeprofile?v=";

		// fme uri
        private readonly string URI_STARTFME = "http://live.nicovideo.jp/api/configurestream/";

        private readonly string URI_STARTFME2 = "http://live.nicovideo.jp/api/configurestream?version=2&v=";

		// コメントサーバーへのTCPソケット
		private TcpClient mTcp = null;
        
		// 取得したコメント
		private static string mComment = "";

		// コメント取得用バッファ
        private static byte[] mTmpBuffer = null;
       
		// ログイン済みかどうか
        private static bool mIsLogin = false;

		// コメント送信用パラメータ
        private static string mTicket = "";
        private static string mLastRes = "";

        private UInt32 mBaseTime = 0;
        private string mUserID = "";
        private string mThread = "";
		private UInt16 mPremium = 0;
    	//
        private static Nico mInstance = null;
        private bool mWakutoriMode = false;
        //

        public bool WakutoriMode
        {
            get { return mWakutoriMode; }
            set { mWakutoriMode = value; }
        }

        //-------------------------------------------------------------------------
        // LiveStart
        //-------------------------------------------------------------------------
        public bool LiveStart(string iLV, string iToken)
        {
            if (iLV.Length <= 2) return false;
            if (iToken.Length <= 0) return false;

            string url = URI_STARTFME2 + iLV + "&key=hq&value=0&token=" + iToken;

            string xml = HttpGet(url, ref this.mCookieLogin);
            if (!xml.Contains("status=\"ok\""))
                return false;
            url = URI_STARTFME2 + iLV + "&key=exclude&value=0&token=" + iToken;
            xml = HttpGet(url, ref this.mCookieLogin);

            return (xml.Contains("status=\"ok\""));
        }

		//-------------------------------------------------------------------------
		// コンストラクタ
		//-------------------------------------------------------------------------
		private Nico()
		{

		}

		//-------------------------------------------------------------------------
		// シングルトン用
		//-------------------------------------------------------------------------
		public static Nico Instance
		{
			get 
			{
				if (mInstance == null)
				{
					mInstance = new Nico();
				}
				return mInstance;
			}
		}

		//-------------------------------------------------------------------------
        // コメント
		//-------------------------------------------------------------------------
        public string Comment
        {
            set { mComment = value; }
			get { return mComment; }
        }

		//-------------------------------------------------------------------------
        // 切断
		//-------------------------------------------------------------------------
        public void Close()
        {
            try
            {
                if (mTcp != null)
                {
                    if(mTcp.Connected)
                        mTcp.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Close:"+e.Message);
            }
            IsLogin = false;
            WakutoriMode = false;
        }

        //-------------------------------------------------------------------------
        //  現在の放送ＩＤ取得
        //-------------------------------------------------------------------------
        public string GetCurrentLive(string username, string password)
        {
            const string uri = "http://live.nicovideo.jp/my";
            const string REGEX_pat = "http://live.nicovideo.jp/watch/lv(?<videoid>[0-9]+)\" class=\"now\"";
            //const string REGEX_pat = "immendStream\\('http://live.nicovideo.jp/','(?<videoid>[0-9]+)'";
            string result = "";
           
            if (Login(username, password))
            {
                string html = HttpGet(uri, ref this.mCookieLogin);
                Regex regex = new Regex(REGEX_pat);
                MatchCollection match = regex.Matches(html);

                if (match.Count > 0)
                {
                    result = "lv" + match[0].Groups["videoid"].Value;
                    Debug.WriteLine(result);
                }
                else
                {
                    regex = new Regex("<a href=\"http://live.nicovideo.jp/watch/lv(?<videoid>[0-9]+)\" title=\"生放送ページへ戻る\" class=\"nml\">");
                    match = regex.Matches(html);
                    if (match.Count > 0)
                    {
                        result = "lv" + match[0].Groups["videoid"].Value;
                        Debug.WriteLine(result);
                    }
                }
            }
            return result;
        }

		//-------------------------------------------------------------------------
        //  ログイン
		//-------------------------------------------------------------------------
        public bool Login(string username, string password)
        {
            // hashtable to hold the arguments of POST request.
            Dictionary<string,string> post_arg = new Dictionary<string,string>(3);

            post_arg["mail"] = username;
            post_arg["password"] = password;
            post_arg["next_url"] = "";

            mUserID = "";
            mThread = "";
            mBaseTime = 0;

            // create cookie-container
            this.mCookieLogin = new CookieContainer();

            string ret = "ログインエラー";
            // ブラウザのクッキーを用いてログインを試みる
            if (Properties.Settings.Default.UseBrowserCookie)
            {
                ICookieGetter[] cookieGetters = CookieGetter.CreateInstances(true);

                ICookieGetter s = null;
                foreach (ICookieGetter es in cookieGetters)
                {
                    if (es.ToString().Equals(Properties.Settings.Default.Browser))
                    {
                        s = es;
                        break;
                    }
                }
                if (s != null)
                {
                    try
                    {
                        System.Net.CookieCollection collection = s.GetCookieCollection(new Uri("http://live.nicovideo.jp/"));
                        if (collection["user_session"] != null)
                        {
                            this.mCookieLogin.Add(new Cookie("user_session", collection["user_session"].Value, "/", ".nicovideo.jp"));
                            if (HttpGet("http://live.nicovideo.jp/my", ref this.mCookieLogin).Contains("<title>マイページ - ニコニコ生放送</title>"))
                            {
                                ret = "ログイン成功";
                            }
                        }
                    }
                    catch (CookieGetterException ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                }

            }
            else   // 通常のログインを試みる
            {
                
                // send POST request
                ret = HttpPost(URI_LOGIN, post_arg, ref this.mCookieLogin);
                if (ret == null)
                {
                    this.mCookieLogin = null;
                    mIsLogin = false;
                    return false;
                }
            }

            // check if result contains "ログインエラー"
            if (ret.IndexOf("ログインエラー") != -1)
            {
                this.mCookieLogin = null;
                mIsLogin = false;
                return false;
            }

            //CookieからセッションＩＤ取得
            Uri uri = new Uri(URI_LOGIN);
            CookieCollection cc = this.mCookieLogin.GetCookies(uri);
            if (cc["user_session"] == null)
            {
                Debug.WriteLine("Could not get session id");
                return false;
            }
            string user_session = cc["user_session"].ToString();
            user_session += "; path=/; domain=.nicovideo.jp";
         
            // IEのCookieを書き換える
            if (!InternetSetCookie(URI_LOGIN, null,
                user_session))
            {
                Debug.WriteLine("Cound not write cookie");
                Debug.WriteLine(GetLastError().ToString());
            }

            // ログイン済みフラグを立てる
            mIsLogin = true;
            return true;
        }

		//-------------------------------------------------------------------------
        // ログインしてるかどうか
		//-------------------------------------------------------------------------
        public bool IsLogin
		{
 			get { return mIsLogin; }
 			set { mIsLogin = value; }
		}

		//-------------------------------------------------------------------------
        // ユーザーIDからユーザー名取得
		//-------------------------------------------------------------------------
		public string GetUsername(string iUserID)
		{
			if( iUserID.Length <= 0 ) return "";

			string name = "";
            string uri = "http://www.nicovideo.jp/user/" + iUserID;
            const string regex = "<h2><strong>(.*?)</strong>さん</h2>";

			string res = HttpGet(uri, ref this.mCookieLogin);

            if (res != null)
            {
                Match match = Regex.Match(res, regex);
                if (match.Success)
                {
                    name = match.Groups[1].Value;
                }
            }
			return name;
		}

        //-------------------------------------------------------------------------
        // 動画情報取得
        //-------------------------------------------------------------------------
        public Dictionary<string, string> GetPublishStatus(string iLiveID)
        {
            // check already logged in or not
            if (mIsLogin == false)
                return null;
            // check have a valid cookie or not
            if (this.mCookieLogin == null)
                return null;

            Dictionary<string, string> ret = new Dictionary<string, string>();

            // send request (GET)
            string uri = URI_GETPUBLISHSTATUS + iLiveID;
            string response = HttpGet(uri, ref this.mCookieLogin);

            if (response == null)
                return null;

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(response), false))
            using (XmlTextReader reader = new XmlTextReader(ms))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        // ステータス取得
                        if (reader.LocalName.Equals("getpublishstatus"))
                        {
                            for (int i = 0; i < reader.AttributeCount; i++)
                            {
                                reader.MoveToAttribute(i);
                                if (reader.Name == "status")
                                {
                                    ret["status"] = reader.Value;
                                }
                                else if (reader.Name == "time")
                                {
                                    ret["time"] = reader.Value;
                                }
                            }

                        }
                        // ベースタイム取得
                        else if (reader.LocalName.Equals("base_time"))
                        {
                            ret["base_time"] = reader.ReadString();
                        }
                        // 開始時間取得
                        else if (reader.LocalName.Equals("start_time"))
                        {
                            ret["start_time"] = reader.ReadString();
                        }
                        // 終了時間取得
                        else if (reader.LocalName.Equals("end_time"))
                        {
                            ret["end_time"] = reader.ReadString();
                        }
                        // コメントサーバーアドレス取得
                        else if (reader.LocalName.Equals("addr"))
                        {
                            ret["addr"] = reader.ReadString();
                        }
                        // コメントサーバーポート取得
                        else if (reader.LocalName.Equals("port"))
                        {
                            ret["port"] = reader.ReadString();
                        }
                        // スレッド取得
                        else if (reader.LocalName.Equals("thread"))
                        {
                            ret["thread"] = reader.ReadString();
                        }
                        // 来場者数取得
                        else if (reader.LocalName.Equals("watch_count"))
                        {
                            ret["watch_count"] = reader.ReadString();
                        }
                        // 来場者数取得
                        else if (reader.LocalName.Equals("code"))
                        {
                            ret["code"] = reader.ReadString();
                        }
                        // コミュニティー
                        else if (reader.LocalName.Equals("room_label"))
                        {
                            ret["room_label"] = reader.ReadString();
                        }
                        // ユーザーＩＤ
                        else if (reader.LocalName.Equals("user_id"))
                        {
                            ret["user_id"] = reader.ReadString();
                        }
                        // プレミアム
                        else if (reader.LocalName.Equals("is_premium"))
                        {
                            ret["is_premium"] = reader.ReadString();
                        }
                        // 名前
                        else if (reader.LocalName.Equals("nickname"))
                        {
                            ret["nickname"] = reader.ReadString();
                        }
                        // token
                        else if (reader.LocalName.Equals("token"))
                        {
                            ret["token"] = reader.ReadString();
                        }
                    }
                }
            }

            return ret;
        }

		//-------------------------------------------------------------------------
        // 動画情報取得
		//-------------------------------------------------------------------------
        public Dictionary<string,string> GetPlayerStatus(string iLiveID )
        {
            // check already logged in or not
            if (mIsLogin == false)
                return null;
            // check have a valid cookie or not
            if (this.mCookieLogin == null)
                return null;

            Dictionary<string,string> ret = new Dictionary<string,string>();

            // send request (GET)
            string uri = URI_GETPLAYERSTATUS + iLiveID;
            string response = HttpGet(uri, ref this.mCookieLogin);

            if (response == null)
                return null;

            //Debug.WriteLine(response);

			using( MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(response), false) )
            using (XmlTextReader reader = new XmlTextReader(ms))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        // ステータス取得
                        if (reader.LocalName.Equals("getplayerstatus"))
                        {
                            for (int i = 0; i < reader.AttributeCount; i++)
                            {
                                reader.MoveToAttribute(i);
                                if (reader.Name == "status")
                                {
                                    ret["status"] = reader.Value;
                                }
                                else if (reader.Name == "time")
                                {
                                    ret["time"] = reader.Value;
                                }
                            }

                        }
                        // ベースタイム取得
                        else if (reader.LocalName.Equals("base_time"))
                        {
                            ret["base_time"] = reader.ReadString();
                        }
                        // 開始時間取得
                        else if (reader.LocalName.Equals("start_time"))
                        {
                            ret["start_time"] = reader.ReadString();
                        }
                        // コメントサーバーアドレス取得
                        else if (reader.LocalName.Equals("addr"))
                        {
                            ret["addr"] = reader.ReadString();
                        }
                        // コメントサーバーポート取得
                        else if (reader.LocalName.Equals("port"))
                        {
                            ret["port"] = reader.ReadString();
                        }
                        // スレッド取得
                        else if (reader.LocalName.Equals("thread"))
                        {
                            ret["thread"] = reader.ReadString();
                        }
                        // 来場者数取得
                        else if (reader.LocalName.Equals("watch_count"))
                        {
                            ret["watch_count"] = reader.ReadString();
                        }
                        // 来場者数取得
                        else if (reader.LocalName.Equals("code"))
                        {
                            ret["code"] = reader.ReadString();
                        }
                        // コミュニティー
                        else if (reader.LocalName.Equals("room_label"))
                        {
                            ret["room_label"] = reader.ReadString();
                        }
                        // ユーザーＩＤ
                        else if (reader.LocalName.Equals("user_id"))
                        {
                            ret["user_id"] = reader.ReadString();
                        }
						// プレミアム
                        else if (reader.LocalName.Equals("is_premium"))
                        {
                            ret["is_premium"] = reader.ReadString();
                        }
                        // 名前
                        else if (reader.LocalName.Equals("nickname"))
                        {
                            ret["nickname"] = reader.ReadString();
                        }
                        // 名前
                        else if (reader.LocalName.Equals("title"))
                        {
                            ret["title"] = reader.ReadString();
                        }
                    }
                }
            }
            
            return ret;
        }

        //-------------------------------------------------------------------------
        // 主コメント送信
        //-------------------------------------------------------------------------
        public bool SendOwnerComment(string iLiveID, string iComment,string iName, string iToken)
        {
			if( !IsLogin )
				return false;

            string uri = "http://watch.live.nicovideo.jp/api/broadcast/"+iLiveID;

            Dictionary<string, string> post_arg = new Dictionary<string, string>();

            post_arg["mail"]  = "";
            post_arg["is184"] = "true";
			post_arg["token"] = iToken;
            post_arg["body"]  = Uri.EscapeUriString(iComment);
            if( iName.Length > 0 )
                post_arg["name"] = Uri.EscapeUriString(iName);

			Debug.WriteLine("OWNER:"+iComment );

            // send POST request
            string ret = HttpPost(uri, post_arg, ref this.mCookieLogin);
            Debug.WriteLine(ret);
			post_arg = null;
            return true;
        }

        //-------------------------------------------------------------------------
        // コメント送信
        //-------------------------------------------------------------------------
        public bool SendComment(string iLiveID,string iComment )
        {
            Int32 block_no=0;

			if( !IsLogin )
				return false;
            
            if (iComment.Length <= 0)
            {
                Debug.WriteLine("コメントが空");
                return false;
            }

			Debug.WriteLine("COMMENT:"+iComment );

            // MovieInfo取得
            if (mThread.Length <= 0 || mUserID.Length <= 0 || mBaseTime == 0)
            {
                //Dictionary<string,string> h = GetPlayerStatus(iLiveID);
                Dictionary<string,string> h = GetPublishStatus(iLiveID);

                if (h == null)
                {
                    Debug.WriteLine("unable get movieinfo");
                    return false;
                }
                mUserID = h["user_id"];
                mThread = h["thread"];
                UInt32.TryParse( h["base_time"], out mBaseTime);
                UInt16.TryParse( h["is_premium"], out mPremium );
            }

            // block_no
            Int32.TryParse(mLastRes, out block_no);
            block_no /= 100;

            // postkey取得
            string url = string.Format(
                            "http://live.nicovideo.jp/api/getpostkey?thread={0}&block_no={1}",
                            mThread,
                            block_no);

            string result = HttpGet(url, ref this.mCookieLogin);

            string postkey = "";
            Match match = Regex.Match(result, "postkey=(.+)");
            if (match.Success)
                postkey = match.Groups[1].Value;

            if (postkey.Length <= 0)
            {
                Debug.WriteLine("unable get postkey");
                return false;
            }

            // vpos計算time_tに変換
            UInt32 ret = (UInt32)time(IntPtr.Zero);
            UInt32 vpos = (ret - mBaseTime)*100;

            // コメント送信リクエスト作成
            string req = String.Format(
                    "<chat thread=\"{0}\" ticket=\"{2}\" postkey=\"{4}\" vpos=\"{1}\" mail=\" 184\" user_id=\"{3}\" premium=\"{5}\">{6}</chat>\0",
                    mThread,
                    vpos,
                    mTicket,
                    mUserID,
                    postkey,
                    mPremium,
                    iComment);

            Debug.WriteLine(req);

            // 送信
            this.Send(mTcp.Client, req);

            return true;
        }

		//-------------------------------------------------------------------------
        // コメントサーバーに接続
		//-------------------------------------------------------------------------
        public NicoErr ConnectToCommentServer(string iLiveID, int commentCount)
        {
            Dictionary<string,string> minfo = GetPlayerStatus( iLiveID );

            if (minfo == null)
            {
                mIsLogin = false;
                return NicoErr.ERR_COULD_NOT_CONNECT_COMMENT_SERVER;
            }
            if (minfo["status"].ToString().CompareTo("ok") != 0 ) {
                mIsLogin = false;

            	if( minfo["code"].ToString().CompareTo("full") == 0 ) {
                    return NicoErr.ERR_COMMENT_SERVER_IS_FULL;
				}
            	else if( minfo["code"].ToString().CompareTo("closed") == 0 ) {
                    return NicoErr.ERR_NOT_LIVE;
				}
            	else if( minfo["code"].ToString().CompareTo("require_community_member") == 0 ) {
                    return NicoErr.ERR_COMMUNITY_ONLY;
				}
                return NicoErr.ERR_COULD_NOT_CONNECT_COMMENT_SERVER;
			}

            // uri of message server
            string uri = minfo["addr"] as string;
            if (uri == null)
                return NicoErr.ERR_COULD_NOT_CONNECT_COMMENT_SERVER;

            // thread
            string tid = minfo["thread"] as string;
            if (tid == null)
                return NicoErr.ERR_COULD_NOT_CONNECT_COMMENT_SERVER;

            // port
			string port = minfo["port"] as string;
            if (port == null)
                return NicoErr.ERR_COULD_NOT_CONNECT_COMMENT_SERVER;

            // request argument
            string req = string.Format(
                "<thread thread=\"{1}\" version=\"20061206\" res_from=\"-{0}\"/>\0",
                commentCount, tid
                );

            // send request
			try {
				Debug.WriteLine( "Addr: "+ uri + "    Port: "+port );
                if (mTcp != null)
                    mTcp.Close();
                mTcp =　new TcpClient( uri, int.Parse(port) );
				Debug.WriteLine("サーバーと接続しました。");

				//NetworkStreamを取得する
				NetworkStream ns = mTcp.GetStream();

				// データ送信
				System.Text.Encoding enc = System.Text.Encoding.UTF8;
				byte[] sendBytes = enc.GetBytes(req);

				//データを送信する
				ns.Write(sendBytes, 0, sendBytes.Length);

                // 非同期受信開始
				StartReceive( mTcp.Client );

			}catch( Exception e ){
				Debug.WriteLine("GetCommentXML:"+e.Message);
                mIsLogin = false;
			}

			minfo = null;
            return NicoErr.ERR_NO_ERR;
        }

		//-------------------------------------------------------------------------
        //非同期データ受信のための状態オブジェクト
		//-------------------------------------------------------------------------
        private class AsyncStateObject
        {
            public System.Net.Sockets.Socket Socket;
            public byte[] ReceiveBuffer;

            public AsyncStateObject(System.Net.Sockets.Socket soc)
            {
                this.Socket = soc;
                this.ReceiveBuffer = new byte[1024*4];
            }
        }

		//-------------------------------------------------------------------------
        //データ受信スタート
		//-------------------------------------------------------------------------
        private static void StartReceive(System.Net.Sockets.Socket soc)
        {
            AsyncStateObject so = new AsyncStateObject(soc);
            //非同期受信を開始
            soc.BeginReceive(so.ReceiveBuffer,
                0,
                so.ReceiveBuffer.Length,
                System.Net.Sockets.SocketFlags.None,
                new System.AsyncCallback(ReceiveDataCallback),
                so);
            mTmpBuffer = new byte[0];
        }

		//-------------------------------------------------------------------------
        //BeginReceiveのコールバック
		//-------------------------------------------------------------------------
        private static void ReceiveDataCallback(System.IAsyncResult ar)
        {
            //状態オブジェクトの取得
            AsyncStateObject so = (AsyncStateObject)ar.AsyncState;

            //読み込んだ長さを取得
            int len = 0;
            try
            {
                len = so.Socket.EndReceive(ar);
            }
            catch (System.ObjectDisposedException)
            {
                //閉じた時
                Debug.WriteLine("閉じました。");
                return;
            }
            catch(SocketException )
            {
                //閉じた時
                Debug.WriteLine("閉じました。");
                mIsLogin = false;
                return;
            }

            //切断されたか調べる
            if (len <= 0)
            {
                Debug.WriteLine("切断されました。");
                so.Socket.Close();
                mIsLogin = false;
                mComment = "<chat date=\"\" no=\"\" premium=\"2\" thread=\"\" user_id=\"\" vpos=\"\">コメントサーバーから切断されました</chat>";
                return;
            }

            //受信したデータを蓄積する
            int org = mTmpBuffer.Length;
            int sz = len + org;
            Array.Resize<byte>(ref mTmpBuffer, sz);
            Buffer.BlockCopy( so.ReceiveBuffer, 0, mTmpBuffer, org, len );

            //最後まで受信した時
            //受信したデータを文字列に変換       
            if (so.Socket.Available == 0 )
            {
                string str = System.Text.Encoding.UTF8.GetString( mTmpBuffer, 0, mTmpBuffer.Length);
                if( str.EndsWith("</chat>\0") ) {
                    Array.Resize<byte>(ref mTmpBuffer, 0);
                    mComment += str;
                    
                    if (str.Contains("<thread"))
                    {
                        string thread;
                        Match match = Regex.Match(mComment, "<thread last_res=\"(.+)\" resultcode=\"0\" revision=\"1\" server_time=\"(.+)\" thread=\"(.+)\" ticket=\"(.+)\"/>");
                        if (match.Success)
                        {
                            mLastRes = match.Groups[1].Value;
                            thread = match.Groups[3].Value;
                            mTicket = match.Groups[4].Value;
                        }
                    }
                }
                if (str.Contains("<chat_result"))
                {
                    Debug.WriteLine(str);
                }
            }

            //再び受信開始
            so.Socket.BeginReceive(so.ReceiveBuffer,
                0,
                so.ReceiveBuffer.Length,
                System.Net.Sockets.SocketFlags.None,
                new System.AsyncCallback(ReceiveDataCallback),
                so);
        }

        //-------------------------------------------------------------------------
        // 非同期送信
        //-------------------------------------------------------------------------
        private void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None,
                new AsyncCallback(SendCallback), client);
        }

        //-------------------------------------------------------------------------
        // 送信コールバック
        //-------------------------------------------------------------------------
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Debug.WriteLine(String.Format("Sent {0} bytes to server.", bytesSent));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

		//-------------------------------------------------------------------------
        // FMEプロファイルの取得
		//-------------------------------------------------------------------------
		public Dictionary<string,string> GetFMEProfile( string iLV )
		{
			string api_url = URI_GETFMEPROFILE + iLV;
            string xml = HttpGet(api_url, ref this.mCookieLogin);

            Dictionary<string, string> ret = new Dictionary<string, string>();

			ret["status"] = "ok";

          	using( MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml), false))
            using (XmlTextReader reader = new XmlTextReader(ms))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
					{
                        if (reader.LocalName.Equals("getfmeprofile"))
						{
							ret["status"] = "failed";
							return ret;
						}
                        else if (reader.LocalName.Equals("url"))
							ret["url"] = reader.ReadString();
                        else if (reader.LocalName.Equals("stream"))
							ret["stream"] = reader.ReadString();
					}
				}
			}

			return ret;
		}

		//-------------------------------------------------------------------------
        // FMEプロファイルの取得
		//-------------------------------------------------------------------------
		public bool StartFME(string iLV, string iToken)
		{
            if( iLV.Length <= 2 ) return false;
            if( iToken.Length <= 0 ) return false;

			string url = URI_STARTFME+iLV+"?key=exclude&value=0&token="+iToken;
            string xml = HttpGet(url, ref this.mCookieLogin);

            return ( xml.Contains("status=\"ok\"") );
		}

        //-------------------------------------------------------------------------
        // 過去の放送情報の取得
        //-------------------------------------------------------------------------
        public bool GetOldLive(string iLv, ref Dictionary<string, string> iInfo, ref Dictionary<string, string> iCom, ref Dictionary<string, string> iTag, ref Dictionary<string, string> iTaglock)
        {
            string lv = iLv.Replace("lv", "");
            string url = "http://live.nicovideo.jp/editstream?reuseid="+lv;
            string res = HttpGet(url, ref mCookieLogin);

            if (res == null) return false;

            Match match = Regex.Match(res, "<input type=\"text\" name=\"title\" style=\"width:400px\" value=\"(.*?)\">");
            if (match.Success)
                iInfo["title"] = match.Groups[1].Value;

            match = Regex.Match(res, "<textarea name=\"description\" class=\"description\" rows=\"7\" style=\"width:400px\"  tabindex=\"0\">(.*?)</textarea>", RegexOptions.Singleline);
            if (match.Success)
                iInfo["description"] = HttpUtility.HtmlDecode(match.Groups[1].Value);
            else
                iInfo["description"] = "";

            // 広告設定取得
            match = Regex.Match(res, "<input type=\"radio\" name=\"ad_enable\" value=\"0\" checked", RegexOptions.Singleline);
            if (match.Success)
                iInfo["ad_enable"] = "0";
            else
                iInfo["ad_enable"] = "1";

            match = Regex.Match(res, "<option value=\"co(.*?)\" style=\"\" class=\".*\" selected >(.*?)</option>");
            if (match.Success)
            {
                iInfo["default_community"] = "co" + match.Groups[1].Value;
                iCom["co"+match.Groups[1].Value] = match.Groups[2].Value.Replace("<wbr />&#8203;","");
            }
            MatchCollection mc = Regex.Matches(res, "<option value=\"co(.*?)\" style=\"\" class=.*?>(.*?)</option>");
            if (mc.Count > 0)
            {   
                foreach (Match m in mc)
                {
                    if (!iInfo.ContainsKey("default_community"))
                    {
                        iInfo["default_community"] = "co" + m.Groups[1].Value;
                    }
                    iCom["co" + m.Groups[1].Value] = m.Groups[2].Value.Replace("<wbr />&#8203;", "");
                }
             }
            
            // タグ ----------------------------------------------------------------
            List<string> livetags = new List<string>();
            List<string> taglock = new List<string>();

            mc = Regex.Matches(res,@"<input type=""text"" value=""(.*?)""\s+?style=""width: 100px;"" name=""livetags(.*?)""",RegexOptions.Multiline);
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    livetags.Add(m.Groups[1].Value);
                }
            }

            mc = Regex.Matches(res, @"<input type=""checkbox"" value=""ロックする"" name=""taglock.*?"" id=""taglock.*?""\s+?(\w*?)\s+?>", RegexOptions.Multiline);
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    if( m.Groups[1].Value.ToString().Contains("checked"))
                        taglock.Add("ロックする");
                    else
                        taglock.Add("");
                }
            }

            int i = 0;
            foreach (string s in livetags)
            {
                string tagkey = "livetags" + (i+1).ToString();
                iTag[tagkey] = s;
                tagkey = "taglock"+(i+1).ToString();
                iTaglock[tagkey] = taglock[i];
                i++;
            }

            mc = Regex.Matches(res, @"<input type=""checkbox"" value=""ロックする"" name=""livetaglockall"" id=""livetaglockall""\s+?(\w*?)\s+?>", RegexOptions.Multiline);
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    Console.WriteLine(m.Groups[1].Value);
                    if (m.Groups[1].Value.ToString().Contains("checked"))
                        iInfo["livetaglockall"] = "ロックする";
                    else
                        iInfo["livetaglockall"] = "";
                }
            }

            //---------------------------------------------------------------------

            // カテゴリ
            string[] category = new string[] { "一般(その他)", "政治", "動物", "料理", "演奏してみた", "歌ってみた", "踊ってみた", "講座", "ゲーム", "動画紹介", "R18" };
            foreach (string s in category)
            {
                if (res.Contains("<option value=\""+s+"\" selected"))
                {
                    iInfo["tags[0]"] = s;
                }
            }
            if (!iInfo.ContainsKey("tags[0]"))
            {
                iInfo["tags[0]"] = "一般(その他)";
            }

            if (res.Contains("input id=\"face\" name=\"tags[]\" type=\"checkbox\" value=\"顔出し\" checked>"))
            {
                iInfo["tags[1]"] = "顔出し";
            }
            else
            {
                iInfo["tags[1]"] = "";
            }
            if (res.Contains("input id=\"totu\" name=\"tags[]\" type=\"checkbox\" value=\"凸待ち\" checked>"))
            {
                iInfo["tags[2]"] = "凸待ち";
            }
            else
            {
                iInfo["tags[2]"] = "";
            }
            if (res.Contains("input id=\"cruise\" name=\"tags[]\" type=\"checkbox\" value=\"クルーズ待ち\"checked>"))
            {
                iInfo["tags[3]"] = "クルーズ待ち";
            }
            else
            {
                iInfo["tags[3]"] = "";
            }
            
            if (res.Contains("<input id=\"community_only\" type=\"checkbox\" value=\"2\" name=\"public_status\" checked>"))
            {
                iInfo["public_status"] = "2";
            }
            else
            {
                iInfo["public_status"] = "";
            }

            if (res.Contains("<input type=\"radio\" value=\"0\" id=\"timeshift_disabled\" name=\"timeshift_enabled\"  checked >"))
            {
                iInfo["timeshift_enabled"] = "0";
            }
            else
            {
                iInfo["timeshift_enabled"] = "1";
            }

            
            iInfo["is_charge"] = "false";
            iInfo["usecoupon"] = "";
            iInfo["back"] = "false";
            iInfo["is_wait"] = "";

            match = Regex.Match(res, "<input type=\"hidden\" name=\"confirm\" value=\"(.*?)\">");

            if (match.Success)
            {
                Debug.WriteLine(match.Groups[1].Value);
                iInfo["confirm"] = match.Groups[1].Value;
            }

            match = Regex.Match(res, "<input type=\"hidden\" id=\"twitter_tag\" name=\"twitter_tag\" value=\"(.*?)\">" );
            if (match.Success)
                iInfo["twitter_tag"] = match.Groups[1].Value;
            else
                iInfo["twitter_tag"] = "";
            return (iInfo.ContainsKey("title") && iInfo["title"].Length > 0);
        }

        //-------------------------------------------------------------------------
        // 枠取り
        //-------------------------------------------------------------------------
        public WakuErr GetWaku(ref Dictionary<string, string> iParam, ref string iLv)
        {
            string url = "http://live.nicovideo.jp/editstream";
            string location = "";
            string res = HttpPost2(url, iParam,ref mCookieLogin, out location);

            if (res == null) return WakuErr.ERR_LOGIN;
            
            string org_res = res;
            Match match;
            
            if (location != null && location.Contains("watch/"))
            {
                match = Regex.Match(res, "watch/(lv[0-9]+)");
                if( match.Success ) {
                    iLv = match.Groups[1].Value;
                }
                return WakuErr.ERR_NO_ERR;
            }
            
            // 改行除去
            res = res.Replace("\n", "");
            res = res.Replace("\r", "");

            // confirmチェック
            match = Regex.Match(res, "<input type=\"hidden\" name=\"confirm\" value=\"(.*?)\">");

            if (match.Success)
            {
                Debug.WriteLine(match.Groups[1].Value);
                iParam["confirm"] = match.Groups[1].Value;
            }

            // エラーチェック
            match = Regex.Match(res, "<li id=\"error_message\">(.*?)</li>");
            if (match.Success)
            {
                string err = match.Groups[1].Value;
                if (err.Contains("タグは18文字以内にして下さい。"))
                    return WakuErr.ERR_TAG;
                if (err.Contains("文字数制限"))
					return WakuErr.ERR_MOJI;
                if (err.Contains("既にこの時間に予約をしているか"))
                    return WakuErr.ERR_ALREADY_LIVE;
                if (err.Contains("混み合って"))
                    return WakuErr.ERR_KONZATU;
                if (err.Contains("既に順番待ち"))
                    return WakuErr.ERR_JUNBAN_ALREADY;
                if (err.Contains("順番"))
                    return WakuErr.ERR_JUNBAN_WAIT;
                if (err.Contains("放送枠の確保が行えませんでした"))
                    return WakuErr.ERR_KONZATU;
                if( err.Contains("メンテナンス"))
                    return WakuErr.ERR_MAINTE;
                if (err.Contains("多重投稿"))
                    return WakuErr.ERR_TAJU;
                if (err.Contains("放送するコミュニティが選択されていません"))
                    return WakuErr.ERR_ALREADY_LIVE;
                else
                    Debug.WriteLine(match.Groups[1].Value);
            }
            if (res.Contains("<h2>メンテナンス中です</h2>"))
            {
                return WakuErr.ERR_MAINTE;
            }

            if (res.Contains("<title>ニコニコ動画　ログインフォーム</title>"))
            {
                return WakuErr.ERR_LOGIN;
            }

            if (res.Contains("<li id=\"error_message\">"))
            {
                Utils.WriteLog(match.Groups[1].Value, org_res);
                return WakuErr.ERR_UNKOWN;
            }
            if (res.Contains("番組が見つかりません"))
            {
                return WakuErr.ERR_KIYAKU;
            }
            if (res.Contains("<h2>エラーが発生しました</h2>ただいまアクセス集中、または不具合発生中のために、ニコニコ生放送に繋がりにくくなっております。"))
            {
                return WakuErr.ERR_KIYAKU;
            }
            if (res.Contains("関係者入り口</a>"))
            {
                return WakuErr.ERR_KIYAKU;
            }
            if (res.Contains("<div id=\"kiyaku_txt\">"))
            {
                return WakuErr.ERR_KIYAKU;
            }

            match = Regex.Match(res, "editstream/lv([0-9]+)");
            if (match.Success)
            {
                iLv = "lv"+match.Groups[1].Value;
                return (res.Contains("<td id=\"txt_wait\"")) ? WakuErr.ERR_JUNBAN : WakuErr.ERR_NO_ERR;
            }
            
            // 2012.05.05 uncomment
            match = Regex.Match(res, "<a href=\"http://live.nicovideo.jp/watch/lv(.*?)\" class=\"now\" title=\"放送中の自分の番組に移動します\">");
            if (match.Success)
            {
                iLv = "lv" + match.Groups[1].Value;
                return WakuErr.ERR_NO_ERR;
            }
            

			Utils.WriteLog( "UNKOWN", org_res );

            Debug.WriteLine(org_res);
            return WakuErr.ERR_UNKOWN;
        }

        //-------------------------------------------------------------------------
        // 直近の配信を取得
        //-------------------------------------------------------------------------
        public string GetRecentLive()
        {
            string url = "http://live.nicovideo.jp/my";
            string res = HttpGet(url, ref mCookieLogin);
            string lv = "";

            // confirmチェック
            Match match = Regex.Match(res, "http://live.nicovideo.jp/editstream/lv(.*?)\"");
            if (match.Success)
            {
                lv = "lv"+match.Groups[1].Value;
            }
            return lv;
        }

        //-------------------------------------------------------------------------
        // 順番待ち情報を取得
        //-------------------------------------------------------------------------
        public Dictionary<string,string> GetJunban(string iLv)
        {
            iLv.Replace("lv", "");

            Dictionary<string, string> arr = new Dictionary<string, string>();

            string url = "http://live.nicovideo.jp/api/waitinfo/" + iLv;

            string res = HttpGet(url, ref mCookieLogin);

            Debug.WriteLine(res);
            Match match;
            match= Regex.Match(res, "<count>(.*?)</count>");
            if (match.Success)
                arr["count"] = match.Groups[1].Value;
            match = Regex.Match(res, "<start_time>(.*?)</start_time>");
            if (match.Success)
                arr["start_time"] = match.Groups[1].Value;
            match = Regex.Match(res, "<stream_status>(.*?)</stream_status>");
            if (match.Success)
                arr["stream_status"] = match.Groups[1].Value;
            return arr;
        }

        //-------------------------------------------------------------------------
        // 順番待ち情報を取得
        //-------------------------------------------------------------------------
        public Dictionary<string, string> GetJunban2(string iLv)
        {
            string newLv = iLv.Replace("lv", "");

            Dictionary<string, string> arr = new Dictionary<string, string>();

            string url = "http://live.nicovideo.jp/editstream/" + iLv;

            string res = HttpGet(url, ref mCookieLogin);

            Debug.WriteLine(res);
            Match match;
            match = Regex.Match(res, "id=\"que_count\">(.*?)</span>番目です");
            if (match.Success)
                arr["count"] = match.Groups[1].Value;
            match = Regex.Match(res, "\"que_start_time\">(.*?)</span>ごろ開始予定");
            if (match.Success)
                arr["start_time"] = match.Groups[1].Value;
            //match = Regex.Match(res, "<stream_status>(.*?)</stream_status>");
            //if (match.Success)
            //    arr["stream_status"] = match.Groups[1].Value;
            arr["stream_status"] = "0";
            return arr;
        }

		//-------------------------------------------------------------------------
        // Get data using GET request
		//-------------------------------------------------------------------------
        private string HttpGet(string url, ref CookieContainer cc)
        {
            // Create HttpWebRequest
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cc;

            try
            {
                WebResponse res = req.GetResponse();

                // read response
                Stream resStream = res.GetResponseStream();
                using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8))
                {
                    string result = sr.ReadToEnd();
                    sr.Close();
                    resStream.Close();
                    return result;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("HttpGet:"+e.Message);
            }
            return null;
        }
        //-------------------------------------------------------------------------
        // Post( multipart/form-data版）
		//-------------------------------------------------------------------------
        private string HttpPost2(string url, Dictionary<string, string> vals, ref CookieContainer cc,out string oLocation)
        {
            string boundary = System.Environment.TickCount.ToString();
            oLocation = "";
            
            //WebRequestの作成
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            
            //メソッドにPOSTを指定
            req.Method = "POST";
            //ContentTypeを設定
            req.ContentType = "multipart/form-data; boundary=" + boundary;
            req.CookieContainer = cc;

            req.Referer = "http://live.nicovideo.jp/editstream";
            req.ServicePoint.Expect100Continue = false;
            req.KeepAlive = true;
            req.Accept = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.99 Safari/533.4";
            req.Headers["Accept-Encoding"] = "gzip,deflate,sdch";
            req.Headers["Accept-Language"] = "ja,en-US;q=0.8,en;q=0.6";
            req.Headers["Accept-Charset"] = "Shift_JIS,utf-8;q=0.7,*;q=0.3";
            req.Headers["Origin"] = "http://live.nicovideo.jp";
            req.Headers["Cache-Control"] = "max-age=0";

            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");

            //POST送信するデータを作成
            string postData = "";
            foreach (string k in vals.Keys)
            {
                postData += "--" + boundary + "\r\n";
                postData += "Content-Disposition: form-data; name=\"" + k + "\"\r\n\r\n";
                postData += vals[k] + "\r\n";
            }
            postData += "--" + boundary + "--\r\n";
            //Debug.WriteLine(postData);
            
            // バイト列に変換
            byte[] startData = enc.GetBytes(postData);

            // 送信
            req.ContentLength = startData.Length;
            System.IO.Stream reqStream = req.GetRequestStream();
            reqStream.Write(startData, 0, startData.Length);
        
            // 返答読み込み
            try
            {
                WebResponse res = req.GetResponse();

                /*
                foreach (string s in res.Headers)
                {
                    Console.WriteLine(s + ":" + res.Headers[s]);
                }
                */

                string location = res.Headers[HttpResponseHeader.Location];
                oLocation = location;

                // read response
                Stream resStream = res.GetResponseStream();
                try
                {
                    if (res.Headers["Content-Encoding"].ToLower().Contains("gzip"))
                        resStream = new GZipStream(resStream, CompressionMode.Decompress);
                    else if (res.Headers["Content-Encoding"].ToLower().Contains("deflate"))
                        resStream = new DeflateStream(resStream, CompressionMode.Decompress);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("HttpPost2:" + e.Message);
                }

                using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8))
                {
                    string result = sr.ReadToEnd();
                    sr.Close();
                    resStream.Close();
                    reqStream.Close();
                    return result;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("HttpPost2:" + e.Message);
            }
            reqStream.Close();
            return "";
        }

		//-------------------------------------------------------------------------
        // Get data using POST request (Hash version)（application/x-www-form-urlencoded版）
		//-------------------------------------------------------------------------
        private string HttpPost(string url, Dictionary<string,string> vals, ref CookieContainer cc)
        {
            // concatinate all key-value pair
            string param = "";
            foreach (string k in vals.Keys)
            {
                param += String.Format("{0}={1}&", k, vals[k]);
            }
            Debug.WriteLine(param);
            byte[] data = Encoding.ASCII.GetBytes(param);

            // create HttpWebRequest
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            req.CookieContainer = cc;

            // write POST data
            try
            {
                Stream reqStream = req.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine("HttpPost:"+e.Message);
                mIsLogin = false;
            }
            try
            {
                WebResponse res = req.GetResponse();

                // read response
                Stream resStream = res.GetResponseStream();
                using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8))
                {
                    string result = sr.ReadToEnd();
                    sr.Close();
                    resStream.Close();
                    return result;
                }
            }
            catch (Exception e)
            {
                mIsLogin = false;
                Debug.WriteLine("HttpPost:"+e.Message);
            }
            return null;
        }

		//-------------------------------------------------------------------------
        // Get data using POST request (Plain Text Version)
		//-------------------------------------------------------------------------
        private string HttpPost(string url, string arg, ref CookieContainer cc)
        {
            byte[] data = Encoding.ASCII.GetBytes(arg);

            // create HttpWebRequest
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            req.CookieContainer = cc;

            // write POST data
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();

            try
            {
                WebResponse res = req.GetResponse();
                
                // read response
                Stream resStream = res.GetResponseStream();
                using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8))
                {
                    string result = sr.ReadToEnd();
                    sr.Close();
                    resStream.Close();
                    return result;
                }
            }
            catch (Exception e)
            {
                mIsLogin = false;
                Debug.WriteLine("HttpPost:"+e.Message);
            }
            return null;
        }

        //-------------------------------------------------------------------------
        // GetSaleList
        //-------------------------------------------------------------------------
        public bool GetSaleList(string iLv, ref List<SaleList> iSaleList)
        {
            string url = "http://watch.live.nicovideo.jp/api/getsalelist?v=" + iLv;
            string xml = HttpGet(url, ref mCookieLogin);

            //Debug.WriteLine(xml);

            if (!xml.Contains("status=\"ok\""))
                return false;

            MatchCollection match;
            iSaleList = new List<SaleList>();
            match = Regex.Matches(xml, "<label>(.*?)</label>");
            if (match.Count > 0)
            {
                foreach (Match m in match)
                {
                    SaleList sale = new SaleList();
                    sale.mLabel = m.Groups[1].Value;
                    iSaleList.Add(sale);
                }
            }

            int n = 0;
            match = Regex.Matches(xml, "<code>(.*?)</code>");
            if (match.Count > 0)
            {
                foreach (Match m in match)
                {
                    SaleList sale = iSaleList[n];
                    sale.mCode = m.Groups[1].Value;
                    iSaleList[n++] = sale;
                }
            }

            n = 0;
            match = Regex.Matches(xml, "<num>(.*?)</num>");
            if (match.Count > 0)
            {
                foreach (Match m in match)
                {
                    SaleList sale = iSaleList[n];
                    sale.mNum = m.Groups[1].Value;
                    iSaleList[n++] = sale;
                }
            }

            n = 0;
            match = Regex.Matches(xml, "<price>(.*?)</price>");
            if (match.Count > 0)
            {
                foreach (Match m in match)
                {
                    SaleList sale = iSaleList[n];
                    sale.mPrice = m.Groups[1].Value;
                    iSaleList[n++] = sale;
                }
            }

            n = 0;
            match = Regex.Matches(xml, "</code><item>(.*?)</item>");
            if (match.Count > 0)
            {
                foreach (Match m in match)
                {
                    SaleList sale = iSaleList[n];
                    sale.mItem = m.Groups[1].Value;
                    iSaleList[n++] = sale;
                }
            }


            /*

            SaleList sale = new SaleList();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml), false))
            using (XmlTextReader reader = new XmlTextReader(ms))
            {
                while (reader.Read())
                {
                    
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.LocalName.Equals("code"))
                            sale.mCode = reader.ReadString();
                        else if (reader.LocalName.Equals("num"))
                            sale.mNum = reader.ReadString();
                        else if (reader.LocalName.Equals("price"))
                            sale.mPrice = reader.ReadString();
                        else if (reader.LocalName.Equals("label"))
                            sale.mLabel = reader.ReadString();
                        else if (reader.LocalName.Equals("item")) ;
                            sale.mItem = reader.ReadString();
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (sale.mItem.Length  > 0)
                        {
                            iSaleList.Add(sale);
                            sale = new SaleList();
                        }
                    }
                }
            }
             */
            return true;
        }

        //-------------------------------------------------------------------------
        // Purchase
        //-------------------------------------------------------------------------
        public bool Purchase(string iLv, string iToken, SaleList iItem)
        {
            string url = "http://watch.live.nicovideo.jp/api/usepoint?v=" + iLv + "&code=" + iItem.mCode + "&item=" + iItem.mItem + "&token=" + iToken + "&num=" + iItem.mNum;
            string xml = HttpGet(url, ref mCookieLogin);

            Debug.WriteLine(xml);

            if (!xml.Contains("status=\"ok\""))
                return false;

            return true;
        }

    }
}
//-------------------------------------------------------------------------
// ニコニコアクセスクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
