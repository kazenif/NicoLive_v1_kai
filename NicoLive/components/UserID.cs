//-------------------------------------------------------------------------
// ユーザーＩＤ管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.Threading;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
	public class UserID
	{
		// コテハン用ハッシュテーブル
        private Dictionary<string, string> mNickHash = null;

        private ReaderWriterLock mLock = new ReaderWriterLock();

		private static UserID mInstance = null;

		//-------------------------------------------------------------------------
		// コンストラクタ
		//-------------------------------------------------------------------------
		private UserID()
		{
            mNickHash = new Dictionary<string, string>();
		}

		//-------------------------------------------------------------------------
		// シングルトン用
		//-------------------------------------------------------------------------
		public static UserID Instance
		{
			get 
			{
				if (mInstance == null)
				{
					mInstance = new UserID();
				}
				return mInstance;
			}
		}

        //-------------------------------------------------------------------------
        // USER ID読み込み
        //-------------------------------------------------------------------------
        public void LoadUserID(bool iAnon)
        {
            string file = (iAnon) ? "anonymous.xml" : "userid.xml";

            using (XmlTextReader xml = new XmlTextReader(file))
            {
                if (xml == null)
                {
                    Debug.WriteLine(file + "が見つかりません");
                    return;
                }


                string id = "";
                string nick = "";

                try
                {
                    while (xml.Read())
                    {
                        if (xml.NodeType == XmlNodeType.Element)
                        {
                            // 古いXMLファイルは削除
                            if (iAnon)
                            {
                                if (xml.LocalName.Equals("date"))
                                {
                                    string d = xml.ReadString();
                                    long create = long.Parse(d);

                                    TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - create);

                                    // 七日以上経過は削除
                                    if (ts.Days > 7)
                                    {
                                        return;
                                    }

                                    // 木曜11時以降は作成日が木曜じゃない設定は削除
                                    DayOfWeek week = DateTime.Now.DayOfWeek;
                                    if (week == DayOfWeek.Thursday && DateTime.Now.Hour >= 11)
                                    {
                                        DateTime dt = new DateTime(create);
                                        DayOfWeek w = dt.DayOfWeek;
                                        if (w != DayOfWeek.Thursday || (w == DayOfWeek.Thursday && dt.Hour < 11))
                                        {
                                            Debug.WriteLine("REFRESH ID");
                                            return;
                                        }
                                    }

                                }
                            }

                            if (xml.LocalName.Equals("user"))
                            {
                                for (int i = 0; i < xml.AttributeCount; i++)
                                {
                                    xml.MoveToAttribute(i);
                                    if (xml.Name == "id")
                                    {
                                        id = xml.Value;
                                    }
                                }
                                nick = xml.ReadString();

                                mNickHash[id] = nick;
                                //Debug.WriteLine(id + "   " + nick);
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
        // ユーザーＩＤセーブ
        //-------------------------------------------------------------------------
        public void SaveUserID(bool iAnon)
        {
            string file = (iAnon) ? "anonymous.xml" : "userid.xml";

            using (XmlTextWriter writer = new XmlTextWriter(file, null))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();

                long t = DateTime.Now.Ticks;
                string time = t.ToString();
                int r;

                writer.WriteStartElement("nick");
                // 作成時刻書き込み
                writer.WriteElementString("date", time);

                foreach (string id in mNickHash.Keys)
                {
                    if (iAnon != int.TryParse(id, out r))
                    {
                        if (mNickHash[id].ToString().Length > 0)
                        {
                            writer.WriteStartElement("user");
                            writer.WriteAttributeString("id", id);
                            writer.WriteString(mNickHash[id]);
                            writer.WriteEndElement();
                        }
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }
        }

        //-------------------------------------------------------------------------
        // ユーザーＩＤに対応するニックネームがあるかどうかチェック
        //-------------------------------------------------------------------------
		public bool Contains( string iID)
		{
            return mNickHash.ContainsKey(iID);
		}
	    //-------------------------------------------------------------------------
        // コテハンチェック
        //------------------------------------------------------------------------
        public string CheckNickname(string iID)
        {
            if (!mNickHash.ContainsKey(iID))
                return null;

            return mNickHash[iID];
        }
	    //-------------------------------------------------------------------------
        // コテハン記憶
        //-------------------------------------------------------------------------
        public void SetNickname(string iID, string iName)
        {
            try
            {
                mLock.AcquireWriterLock(Timeout.Infinite);

                if (mNickHash.ContainsKey(iID)) {

                    mNickHash.Remove(iID);
				}

                mNickHash[iID] = iName;
            }
            finally
            {
                mLock.ReleaseWriterLock();
            }
        }

        //-------------------------------------------------------------------------
        // コテハン記憶
        //-------------------------------------------------------------------------
        public bool AddNickname(string iID, string iName)
        {
            if (mNickHash.ContainsKey(iID)) return false;

            try
            {
                mLock.AcquireWriterLock(Timeout.Infinite);
                mNickHash[iID] = iName;
            }
            finally
            {
                mLock.ReleaseWriterLock();
            }
            return true;
        }

	}
}

//-------------------------------------------------------------------------
// ユーザーＩＤ管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
