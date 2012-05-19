//-------------------------------------------------------------------------
// コテハンリネームダイアログ
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
    public partial class Rename : Form
    {
        private Form1 mOwner = null;
        private string mID = null;

        public Form1 MyOwner
        {
            set { mOwner = value; }
            get { return mOwner; }
        }

        public string ID
        {
            set { mID = value; }
            get { return mID; }
        }

        //-------------------------------------------------------------------------
        // コンストラクタ
        //-------------------------------------------------------------------------
        public Rename()
        {
            InitializeComponent();
        }

        //-------------------------------------------------------------------------
        // フォームロード時
        //-------------------------------------------------------------------------
        private void Rename_Load(object sender, EventArgs e)
        {
            // センタリング
            this.Top = (this.mOwner.Height - this.Height) / 2;
            this.Left = (this.mOwner.Width - this.Width) / 2;
        }
        //-------------------------------------------------------------------------
        // ＯＫボタン
        //-------------------------------------------------------------------------
        private void mOK_Click(object sender, EventArgs e)
        {
            if (mName.Text.Length > 0)
            {
                if (mOwner != null)
                {
                    mOwner.SetNickname(this.mID, mName.Text);
                }
                Close();
            }
        }

        //-------------------------------------------------------------------------
        // キャンセルボタン
        //-------------------------------------------------------------------------
        private void mCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

 
    }
}
//-------------------------------------------------------------------------
// コテハンリネームダイアログ
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------