//-------------------------------------------------------------------------
// Twitterアクセスクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Diagnostics;
using oAuthExample;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    class Twitter :IDisposable
    {
        //-------------------------------------------------------------------------
        // 解放
        //-------------------------------------------------------------------------
        public void Dispose()
        {
        }

        //-------------------------------------------------------------------------
        // xAuth認証
        //-------------------------------------------------------------------------
		public bool xAuth( string iUserID, string iPasswd )
		{
            oAuthTwitter oAuth = new oAuthTwitter();
            oAuth.xAuthAccessTokenGet(iUserID, iPasswd);
            
            if (oAuth.TokenSecret.Length > 0)
            {
                // TokenとTokenSecretを待避
				Debug.WriteLine(oAuth.Token);
				Debug.WriteLine(oAuth.TokenSecret);

                Properties.Settings.Default.tw_passwd = "";
                Properties.Settings.Default.tw_user = "";

                Properties.Settings.Default.tw_token = oAuth.Token;
                Properties.Settings.Default.tw_token_secret = oAuth.TokenSecret;
				return true;
            }
            return false;
		}

        //-------------------------------------------------------------------------
        // ポスト(xAuth版)
        //-------------------------------------------------------------------------
        public bool Post(string iMessage,string iTopic)
        {
            if (Properties.Settings.Default.tw_token.Length == 0 ||
                Properties.Settings.Default.tw_token_secret.Length == 0)
                return false;

            if (iTopic.Length > 0)
                iMessage += " " + iTopic;

            oAuthTwitter oAuth = new oAuthTwitter();
            oAuth.Token = Properties.Settings.Default.tw_token;
            oAuth.TokenSecret = Properties.Settings.Default.tw_token_secret;
            string url = "http://twitter.com/statuses/update.xml";
            string xml = oAuth.oAuthWebRequest(oAuthTwitter.Method.POST, url, "status=" + oAuth.UrlEncode(iMessage));
			oAuth = null;
            return true;
		}
    }
}
//-------------------------------------------------------------------------
// Twitterアクセスクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
