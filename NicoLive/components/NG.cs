//-------------------------------------------------------------------------
// ＮＧワード管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
	public class NG
	{
        // ＮＧワードリスト
        private List<string> mNGWord = null;

		private static NG mInstance = null;

		//-------------------------------------------------------------------------
		// コンストラクタ
		//-------------------------------------------------------------------------
		private NG()
		{
			mNGWord = new List<string>();
		}

		//-------------------------------------------------------------------------
		// シングルトン用
		//-------------------------------------------------------------------------
		public static NG Instance
		{
			get 
			{
				if (mInstance == null)
				{
					mInstance = new NG();
				}
				return mInstance;
			}
		}

       //-------------------------------------------------------------------------
        // NGワード読み込み
        //-------------------------------------------------------------------------
        public void LoadNGWord()
        {
            using (XmlTextReader xml = new XmlTextReader("configurengword.xml"))
            {
                if (xml == null)
                {
                    Debug.WriteLine("ＮＧワードファイルが見つかりません");
                    return;
                }

                try
                {
                    while (xml.Read())
                    {
                        if (xml.NodeType == XmlNodeType.Element)
                        {
                            if (xml.LocalName.Equals("source"))
                            {
                                mNGWord.Add(xml.ReadString());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("LoadNGWord:" + e.Message);
                }
            }
            Debug.WriteLine("NGワード:" + mNGWord.Count);
        }
	    //-------------------------------------------------------------------------
        // NGワードチェック
        //-------------------------------------------------------------------------
        public bool CheckNG(string iStr)
        {
            foreach (string s in mNGWord)
            {
                if (iStr.Contains(s))
                    return true;
            }
            return false;
        }
	}
}

//-------------------------------------------------------------------------
// ＮＧワード管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
