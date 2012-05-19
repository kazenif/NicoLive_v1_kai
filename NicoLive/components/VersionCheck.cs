//-------------------------------------------------------------------------
// バージョンチェック用クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Diagnostics;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    class VersionCheck
    {
        // バージョンチェック用ＵＲＩ
        private readonly string URI = "http://sourceforge.jp/projects/nicolive/releases";

        // アプリケーション名
        private readonly string APP = "NicoLive-";

        //-------------------------------------------------------------------------
        // バージョンチェック
        //-------------------------------------------------------------------------
        public bool Check(string iVersion)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    Stream st = wc.OpenRead(URI);

                    Encoding enc = Encoding.GetEncoding("Shift_JIS");
                    StreamReader sr = new StreamReader(st, enc);

                    string html = sr.ReadToEnd();
                    string app = APP + iVersion + ".zip";
                    bool rc = (html.Contains(app));

                    sr.Close();
                    st.Close();

                    return rc;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return true;
        }
    }
}

//-------------------------------------------------------------------------
// バージョンチェック用クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
