//-------------------------------------------------------------------------
// 放送情報変更ダイアログ
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System;

namespace NicoLive
{
    public partial class ChangeProp : Form
    {
        Dictionary<string, string> mCom;
        Dictionary<int, string> mDescList;
        Dictionary<int, string> mPublicStatusList;
        Dictionary<int, string> mFaceList;
        Dictionary<int, string> mTotuList;
        Dictionary<int, string> mTimeShiftList;
        Dictionary<int, string> mComList;
        Dictionary<int, string> mTagLockAllList;
        Dictionary<int, string> mTagLockList;
        Dictionary<int, string> mTagList;
        Dictionary<int, string> mAdList;
        Dictionary<int, string> mCruiseList;

        public string Title
        {
            get { return mTitle.Text; }
            set { mTitle.Text = value; }
        }
        public string Desc
        {
            get {
                string desc = mDesc.Text;
                desc = desc.Replace("<br>", "");
                desc = desc.Replace("<br />", "");
                desc = desc.Replace("\r\n", "<br />\r\n");

                return desc;
            }
            set {
                string desc = value;
                desc = desc.Replace("<br />\r\n", "\r\n");
                desc = desc.Replace("<br />", "\r\n");
                mDesc.Text = desc;
            }
        }
        
        public bool CommunityOnly
        {
            get { return mComuOnly.Checked; }
            set { mComuOnly.Checked = value; }
        }

        public bool TimeShift
        {
            get { return mTimeShift.Checked; }
            set { mTimeShift.Checked = value; }
        }
        public bool Totumachi
        {
            get { return mTotumachi.Checked; }
            set { mTotumachi.Checked = value; }
        }
        public bool Face
        {
            get { return mFace.Checked; }
            set { mFace.Checked = value; }
        }
        public bool Ad
        {
            get { return mAd.Checked; }
            set { mAd.Checked = value; }
        }

        public bool Cruise
        {
            get { return mCruise.Checked; }
            set { mCruise.Checked = value; }
        }
        public bool LiveTagLockAll
        {
            get { return mLiveTagLockAll.Checked; }
            set { mLiveTagLockAll.Checked = value; }
        }
        public string LiveTag
        {
            get
            {
                string ret = "";
                int cnt = mLivetags.RowCount;

                for (int i = 0; i < cnt; i++)
                {
                    string val = "";
                    if (mLivetags.Rows[i].Cells["mTag"].Value != null)
                        val = mLivetags.Rows[i].Cells["mTag"].Value.ToString();
                    ret += val;
                    if (i != cnt - 1)
                        ret += ",";
                }
                return ret;
            }
        }
        public string LiveTagLock
        {
            get
            {
                string ret = "";
                int cnt = mLivetags.RowCount;

                for (int i = 0; i < cnt; i++)
                {
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)mLivetags.Rows[i].Cells["mLock"];

                    string val = (chk.Value != null && (bool)chk.FormattedValue) ? "True" : "False";
                    ret += val;
                    if (i != cnt - 1)
                        ret += ",";
                }
                return ret;
            }
        }

        public string Community
        {
            get {
                return mCom[mCommunity.SelectedItem.ToString()]; 
            }
        }

        public ChangeProp()
        {
            mCom = new Dictionary<string, string>();

            mDescList = new Dictionary<int, string>();
            mTotuList = new Dictionary<int, string>();
            mPublicStatusList = new Dictionary<int, string>();
            mTimeShiftList = new Dictionary<int, string>();
            mFaceList = new Dictionary<int, string>();
            mComList = new Dictionary<int, string>();
            mTagLockAllList = new Dictionary<int, string>();
            mTagList = new Dictionary<int, string>();
            mTagLockList = new Dictionary<int, string>();
            mAdList = new Dictionary<int, string>();
            mCruiseList = new Dictionary<int, string>();
            InitializeComponent();
        }


        public void SetCommunity(Dictionary<string, string> iCommunity)
        {
            foreach (string key in iCommunity.Keys)
            {
                mCommunity.Items.Add(iCommunity[key]);
                mCom[iCommunity[key]] = key;
            }
            mCommunity.SelectedIndex = 0;
        }

        public void SetTag(Dictionary<string, string> iTag, Dictionary<string, string> iTaglock)
        {
            foreach (string val in iTag.Values)
            {
                mLivetags.Rows.Add(val);
            }
            int i = 0;
            foreach (string val in iTaglock.Values)
            {
                if( val.Contains("ロックする"))
                    mLivetags.Rows[i].Cells["mLock"].Value = true;
                i++;
            }
        }

        public Dictionary<string, string> GetTag()
        {
            Dictionary<string,string> arr = new Dictionary<string,string>();

            int cnt = mLivetags.RowCount;

            for( int i = 0;i <cnt;i++)
            {
                string val = "";
                if( mLivetags.Rows[i].Cells["mTag"].Value != null)
                    val = mLivetags.Rows[i].Cells["mTag"].Value.ToString();
                string key = "livetags" + (i + 1).ToString();
                arr.Add(key, val);
            }
            return arr;
        }

        public Dictionary<string, string> GetTaglock()
        {
            Dictionary<string, string> arr = new Dictionary<string, string>();

            int cnt = mLivetags.RowCount;

            for (int i = 0; i < cnt; i++)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)mLivetags.Rows[i].Cells["mLock"];

                string val = (chk.Value != null && (bool)chk.FormattedValue) ? "ロックする" : "";
                if (val.Length > 0)
                {
                    string key = "taglock" + (i + 1).ToString();
                    arr.Add(key, val);
                }
                //Console.WriteLine(mLivetags.Rows[i].Cells[1].Value);
            }
            return arr;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            SaveHistory();
            Close();
        }

        private void ChangeProp_Load(object sender, System.EventArgs e)
        {
            LoadHistory();
        }

        //-------------------------------------------------------------------------
        // タイトル履歴保存
        //-------------------------------------------------------------------------
        private void SaveHistory()
        {
            using (XmlTextWriter writer = new XmlTextWriter("prop_history.xml", null))
            {
                int cnt = mTitle.Items.Count;
                int max = 10;

                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();

                writer.WriteStartElement("history");

                if (cnt == 0 ||
                   (
                    !Title.Equals((string)mTitle.Items[0]) || 
                    !Desc.Equals(mDescList[0]) ||
                    Face.ToString() != mFaceList[0] ||
                    CommunityOnly.ToString() != mPublicStatusList[0] ||
                    TimeShift.ToString() != mTimeShiftList[0] ||
                    Totumachi.ToString() != mTotuList[0] ||
                    LiveTagLockAll.ToString() != mTagLockAllList[0] ||
                    LiveTag != mTagList[0] ||
                    LiveTagLock != mTagLockList[0] ||
                    Ad.ToString() != mAdList[0] ||
                    Cruise.ToString() != mCruiseList[0]
                   )
                 )
                {
                    writer.WriteStartElement("item");
                    writer.WriteElementString("title", mTitle.Text);
                    writer.WriteElementString("description", mDesc.Text);
                    writer.WriteElementString("public_status", CommunityOnly.ToString());
                    writer.WriteElementString("time_shift", TimeShift.ToString());
                    writer.WriteElementString("face", Face.ToString());
                    writer.WriteElementString("totsu", Totumachi.ToString());
                    writer.WriteElementString("community", Community);
                    writer.WriteElementString("livetaglockall", LiveTagLockAll.ToString());
                    writer.WriteElementString("livetaglock", LiveTagLock);
                    writer.WriteElementString("livetag", LiveTag);
                    writer.WriteElementString("ad", Ad.ToString());
                    writer.WriteElementString("cruise", Cruise.ToString());
                    writer.WriteEndElement();
                    max--;
                }

                if (cnt > max)
                    cnt = max;

                for( int i = 0;i < cnt ;i++)
                {
                    writer.WriteStartElement("item");
                        writer.WriteElementString("title", (string)mTitle.Items[i]);
                        writer.WriteElementString("description", mDescList[i]);
                        writer.WriteElementString("public_status", mPublicStatusList[i].ToString());
                        writer.WriteElementString("time_shift", mTimeShiftList[i].ToString());
                        writer.WriteElementString("face", mFaceList[i].ToString());
                        writer.WriteElementString("totsu", mTotuList[i].ToString());
                        writer.WriteElementString("community", mComList[i]);
                        writer.WriteElementString("livetaglockall", mTagLockAllList[i]);
                        writer.WriteElementString("livetaglock", mTagLockList[i]);
                        writer.WriteElementString("livetag", mTagList[i]);
                        if (i<mAdList.Count) writer.WriteElementString("ad", mAdList[i]);
                        if (i<mCruiseList.Count) writer.WriteElementString("cruise", mCruiseList[i]);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }
        }

        //-------------------------------------------------------------------------
        // タイトル履歴ロード
        //-------------------------------------------------------------------------
        public void LoadHistory()
        {
            int i = -1;
            int taglockallcnt = 0,tagcnt=0,taglockcnt=0;

            using (XmlTextReader xml = new XmlTextReader("prop_history.xml"))
            {
                if (xml == null)
                {
                    return;
                }

                try
                {
                    while (xml.Read())
                    {
                        if (xml.NodeType == XmlNodeType.Element)
                        {
                            if (xml.LocalName.Equals("item"))
                            {
                                i++;
                                if (i != taglockallcnt)
                                {
                                    mTagLockAllList[taglockallcnt++] = "False";
                                }
                                if (i != taglockcnt)
                                {
                                    mTagLockList[taglockcnt++] = "";
                                }
                                if (i != tagcnt)
                                {
                                    mTagList[tagcnt++] = "";
                                }
                            }
                            else if (xml.LocalName.Equals("title"))
                            {
                                mTitle.Items.Add(xml.ReadString());
                            }
                            else if (xml.LocalName.Equals("description"))
                            {
                                mDescList[i] = xml.ReadString();
                            }
                            else if (xml.LocalName.Equals("public_status"))
                            {
                                mPublicStatusList[i] = xml.ReadString();
                            }
                            else if (xml.LocalName.Equals("time_shift"))
                            {
                                mTimeShiftList[i] = xml.ReadString();
                            }
                            else if (xml.LocalName.Equals("face"))
                            {
                                mFaceList[i] = xml.ReadString();
                            }
                            else if (xml.LocalName.Equals("totsu"))
                            {
                                mTotuList[i] = xml.ReadString();
                            }
                            else if (xml.LocalName.Equals("community"))
                            {
                                mComList[i] = xml.ReadString();
                            }
                            else if (xml.LocalName.Equals("livetaglockall"))
                            {
                                mTagLockAllList[i] = xml.ReadString();
                                taglockallcnt++;
                            }
                            else if (xml.LocalName.Equals("livetaglock"))
                            {
                                mTagLockList[i] = xml.ReadString();
                                taglockcnt++;
                            }
                            else if (xml.LocalName.Equals("livetag"))
                            {
                                mTagList[i] = xml.ReadString();
                                tagcnt++;
                            }
                            else if (xml.LocalName.Equals("ad"))
                            {
                                mAdList[i] = xml.ReadString();
                                tagcnt++;
                            }
                            else if (xml.LocalName.Equals("cruise"))
                            {
                                mCruiseList[i] = xml.ReadString();
                                tagcnt++;
                            }
                        }
                    }

                    i++;
                    if (i != taglockallcnt)
                    {
                        mTagLockAllList[taglockallcnt++] = "False";
                    }
                    if (i != taglockcnt)
                    {
                        mTagLockList[taglockcnt++] = "";
                    }
                    if (i != tagcnt)
                    {
                        mTagList[tagcnt++] = "";
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("LoadHistory:" + e.Message);
                }
            }
        }

        //-------------------------------------------------------------------------
        // 詳細履歴設定
        //-------------------------------------------------------------------------
        private void mTitle_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int sel = mTitle.SelectedIndex;
            if (sel < 0) return;

            if (mDescList.ContainsKey(sel))
            {
                Desc = mDescList[sel];
                if (sel < mPublicStatusList.Count) CommunityOnly = mPublicStatusList[sel].Equals("True");
                if (sel < mFaceList.Count)         Face = mFaceList[sel].Equals("True");
                if (sel < mTotuList.Count)         Totumachi = mTotuList[sel].Equals("True");
                if (sel < mTimeShiftList.Count)    TimeShift = mTimeShiftList[sel].Equals("True");
                if (sel < mTagLockAllList.Count)   LiveTagLockAll = mTagLockAllList[sel].Equals("True");
                if (sel < mAdList.Count)           Ad = mAdList[sel].Equals("True");
                if (sel < mCruiseList.Count)       Cruise = mCruiseList[sel].Equals("True");

                int i = 0;
                foreach (string val in mCom.Values)
                {
                    if (val.Equals(mComList[sel]))
                    {
                        mCommunity.SelectedIndex = i;
                        break;
                    }
                    i++;
                }

				// タグ
                mLivetags.Rows.Clear();

                string livetags = mTagList[sel];
                string livelocks = mTagLockList[sel];
                
                string[] tags = livetags.Split(new Char[] { ',' });
                string[] locks = livelocks.Split(new Char[] { ',' });
                int n = 0;
                foreach (string tg in tags)
                {
                    mLivetags.Rows.Add(tg);
                    mLivetags.Rows[n].Cells["mLock"].Value = locks[n].Equals("True");
                    n++;
                }
            }
        }

        private void mLivetags_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            mLivetags.AllowUserToAddRows = (mLivetags.RowCount <= 10);
        }

        private void mLivetags_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            mLivetags.AllowUserToAddRows = (mLivetags.RowCount <= 10);
        }
    }
}
//-------------------------------------------------------------------------
// 放送情報変更ダイアログ
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
