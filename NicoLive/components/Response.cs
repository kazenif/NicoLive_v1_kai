//-------------------------------------------------------------------------
// 返信メッセージクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.IO;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
	public class Response
	{
		// アクション管理構造体
        public struct Action
        {
            public string mMsg;      // キー文字
            public Int16  mType;     // アクションタイプ
            public string mContent;  // コンテント
        };

        // アクションタイプ
        enum ActionType
        {
            ACTION_TEXT = 0,
            ACTION_EXE,
        };

		private static Response mInstance = null;
        private Dictionary<string, Action> mMsgHash = null;

        public Dictionary<string, Action> Hash
        {
            get{ return mMsgHash; }
            set { mMsgHash = value; }
        }

		//-------------------------------------------------------------------------
		// コンストラクタ
		//-------------------------------------------------------------------------
		private Response()
		{
            mMsgHash = new Dictionary<string, Action>();
		}

		//-------------------------------------------------------------------------
		// シングルトン用
		//-------------------------------------------------------------------------
		public static Response Instance
		{
			get 
			{
				if (mInstance == null)
				{
					mInstance = new Response();
				}
				return mInstance;
			}
		}

        //-------------------------------------------------------------------------
        // リロード
        //-------------------------------------------------------------------------
        public void Reload()
        {
            mMsgHash.Clear();
            LoadResponse();
        }

        //-------------------------------------------------------------------------
        // 返信メッセージ読み込み
        //-------------------------------------------------------------------------
        public void LoadResponse()
        {
            const string file = "response.xml";

            using (XmlTextReader xml = new XmlTextReader(file))
            {
                if (xml == null)
                {
                    Debug.WriteLine(file + "が見つかりません");
                    return;
                }


                string msg = "";
				string res = "";

                try
                {
                    while (xml.Read())
                    {
                        if (xml.NodeType == XmlNodeType.Element)
                        {
                            if (xml.LocalName.Equals("response"))
                            {
                                Int16 type = 0;
                                bool enabled = true;
                                for (int i = 0; i < xml.AttributeCount; i++)
                                {
                                    xml.MoveToAttribute(i);
                                    if (xml.Name == "message")
                                    {
                                        msg = xml.Value;
                                    }else
                                    if (xml.Name == "type")
                                    {
                                        Int16.TryParse(xml.Value, out type);
                                    }else
                                    if (xml.Name == "enabled")
                                    {
                                        enabled = xml.Value.ToString().Equals("True");
                                    }
                                }
                               
								res = xml.ReadString();

                                if (enabled)
                                {
                                    Action act = new Action();
                                    act.mMsg = msg;
                                    act.mContent = res;
                                    act.mType = type;

                                    mMsgHash[msg] = act;
                                }

								//Debug.WriteLine( String.Format("res: {0}  msg: {1}", res, msg));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("LoadUser:" + e.Message);
                }
            }
        }

		//-------------------------------------------------------------------------
		// 返信メッセージ取得
		//-------------------------------------------------------------------------
		public string GetResponse( string iText )
		{
            if (!Properties.Settings.Default.auto_res) return "";

			if( iText.Length <= 0 ) return "";
			
			string str = "";

            foreach (string key in mMsgHash.Keys)
            {
                if (iText.Contains(key))
                {
                    return GetAction(key);
                }
            }

			return str;
		}

        //-------------------------------------------------------------------------
        // アクション取得
        //-------------------------------------------------------------------------
        private string GetAction(string iKey)
        {
            Action act = mMsgHash[iKey];

            int type = (int)act.mType;
            string ret = "";

            switch (type)
            {
                // そのまま返信
                case (int)ActionType.ACTION_TEXT:
                    ret = act.mContent;
                    break;
                // EXEの戻り値取得
                case (int)ActionType.ACTION_EXE:
                    ret = GetExeResponse(act.mContent);
                    break;
            }

            return ret;
        }

        //-------------------------------------------------------------------------
        // EXEの戻り値取得
        //-------------------------------------------------------------------------
        private string GetExeResponse(string iExe)
        {
            string ret = "";
            try
            {
                if (File.Exists(iExe))
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = iExe;
                    proc.StartInfo.Arguments = "";
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.Start();
                    ret = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();
                }
            }catch(Exception e){
                 Debug.WriteLine(e.Message);
            }
            return ret;
        }
	}
}

//-------------------------------------------------------------------------
// 返信メッセージクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
