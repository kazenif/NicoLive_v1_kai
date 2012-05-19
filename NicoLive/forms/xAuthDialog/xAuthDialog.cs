//-------------------------------------------------------------------------
// xAuth認証ダイアログクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NicoLive
{
    public partial class xAuthDialog : Form
    {
        public xAuthDialog()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Auth();
        }

        private void Auth()
        {
            mGroupBox.Text = "認証中です。";

            Twitter tw = new Twitter();
            if (tw.xAuth(mID.Text, mPassword.Text))
            {
                Close();
            }
            else
            {
                mGroupBox.Text = "認証に失敗しました。";
            }
        }

        private void mPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                Auth();
            }
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------