//-------------------------------------------------------------------------
// Viewer
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id: Nickname.cs 697 2010-07-05 06:13:32Z kintoki $
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;

namespace NicoLive
{
    public partial class Viewer : Form
    {
        string mLv;

        //-------------------------------------------------------------------------
        //　コンストラクタ
        //-------------------------------------------------------------------------
        public Viewer(string iLv)
        {
            mLv = iLv;

            InitializeComponent();
        }

        //-------------------------------------------------------------------------
        //　SWFロード
        //-------------------------------------------------------------------------
        private void LoadMovie()
        {
            Nico nico = Nico.Instance;
            if (!nico.IsLogin)
            {
                string user_id = Properties.Settings.Default.user_id;
                string passwd = Properties.Settings.Default.password;
                nico.Login(user_id, passwd);
            }

            //string url = "http://live.nicovideo.jp/liveplayer.swf?110519134121&allowscale=FALSE&v=" + mLiveID.Text + "&watchVideoID" + mLiveID.Text;
            string url = "http://nl.nimg.jp/public/swf/liveplayer.swf?120329182902&allowscale=FALSE&v=" + mLiveID.Text + "&watchVideoID" + mLiveID.Text;

            mFlash.Size = new System.Drawing.Size(950,520);
            mFlash.LoadMovie(0, url);
        }

        //-------------------------------------------------------------------------
        //　Formロード
        //-------------------------------------------------------------------------
        private void Viewer_Load(object sender, System.EventArgs e)
        {
            SetLiveID( mLv);
        }

        //-------------------------------------------------------------------------
        //　SWFロード
        //-------------------------------------------------------------------------
        private void mUITimer_Tick(object sender, System.EventArgs e)
        {

        }

        //-------------------------------------------------------------------------
        //　視聴する放送ＩＤ設定
        //-------------------------------------------------------------------------
        public void SetLiveID(string iLv)
        {
            this.Invoke((Action)delegate()
            {
                mLiveID.Text = iLv;
                LoadMovie();
            });
        }

        //-------------------------------------------------------------------------
        //　視聴する放送ＩＤ変更
        //-------------------------------------------------------------------------
        private void mLiveID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                string tmp = mLiveID.Text;
                string live_id = "lv";

                int idx = tmp.LastIndexOf("lv");
                int len = tmp.Length;

                for (int i = idx + 2; i < len; i++)
                {
                    if ('0' <= tmp[i] && tmp[i] <= '9')
                        live_id += tmp[i].ToString();
                    else
                        break;
                }

                mLiveID.Text = live_id;

                LoadMovie();
            }
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id: Nickname.cs 697 2010-07-05 06:13:32Z kintoki $
//-------------------------------------------------------------------------