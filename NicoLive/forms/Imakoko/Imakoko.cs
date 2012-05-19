//-------------------------------------------------------------------------
// 今ココなう！用ウィンド
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    public partial class Imakoko : Form
    {
        // オーナーフォーム
        private Form mOwner = null;

        public Form MyOwner
        {
            get { return mOwner; }
            set { mOwner = value; }
        }

        //-------------------------------------------------------------------------
        // 初期化
        //-------------------------------------------------------------------------
        public Imakoko()
        {
            InitializeComponent();
        }

        //-------------------------------------------------------------------------
        // ロード時
        //-------------------------------------------------------------------------
        private void Imakoko_Load(object sender, EventArgs e)
        {
            string uri = String.Format("http://imakoko-gps.appspot.com/view?trace={0}&hide_userlist=1",
                                        Properties.Settings.Default.imakoko_user);

            if (mOwner != null)
            {
                this.Left = mOwner.Left + 400;
                this.Top = mOwner.Top+60;
            }
            mBrowser.Navigate(uri);
        }

        
    }
}
//-------------------------------------------------------------------------
// 今ココなう！用ウィンド
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
