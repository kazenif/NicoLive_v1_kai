//-------------------------------------------------------------------------
// メッセージテキスト管理クラス
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
	public class MessageSettings
	{
		private static MessageSettings mInstance = null;

		private Dictionary<string,string> mMsgHash  = null;
		//-------------------------------------------------------------------------
		// コンストラクタ
		//-------------------------------------------------------------------------
		private MessageSettings()
		{
            mMsgHash = new Dictionary<string, string>();
		}

		//-------------------------------------------------------------------------
		// シングルトン用
		//-------------------------------------------------------------------------
		public static MessageSettings Instance
		{
			get 
			{
				if (mInstance == null)
				{
					mInstance = new MessageSettings();
				}
				return mInstance;
			}
		}

		//-------------------------------------------------------------------------
		// 設定ファイルロード
		//-------------------------------------------------------------------------
		public void Load()
		{
			string path = "message.xml";

			using (XmlTextReader xml = new XmlTextReader( path ))
			{
				if (xml == null)
				{
					Debug.WriteLine("ファイルが見つかりません");
					return;
				}

				try
				{
					while (xml.Read())
					{
						if (xml.NodeType == XmlNodeType.Element)
						{
							if (xml.LocalName.Equals("message"))
							{
								string key = "";
                                for (int i = 0; i < xml.AttributeCount; i++)
								{
                                    xml.MoveToAttribute(i);
                                    if (xml.Name == "key")
									{
                                        key = xml.Value;
									}
								}
                                if (key.Length > 0)
                                {
                                    mMsgHash[key] = xml.ReadString();
                                }
							}
						}
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine("Message::Load:" + e.Message);
				}
			}
		}

		//-------------------------------------------------------------------------
		// メッセージの取得
		//-------------------------------------------------------------------------
		public string GetMessage( string iStr )
		{
            if (!mMsgHash.ContainsKey(iStr))
                return iStr;

			return mMsgHash[iStr];
		}
	}
}

//-------------------------------------------------------------------------
// メッセージテキスト管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
