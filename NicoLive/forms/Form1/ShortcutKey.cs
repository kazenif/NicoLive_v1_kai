//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System.Windows.Forms;

namespace NicoLive
{
    partial class Form1
    {
        //-------------------------------------------------------------------------
        // ショートカットキー
        //-------------------------------------------------------------------------
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            
            if ((int)keyData == (int)Keys.F8)
            {
                this.mBouyomiBtn.PerformClick();
                return true;
            }
            if ((int)keyData == (int)Keys.F7)
            {
                this.mVisitorBtn.PerformClick();
                return true;
            }
            if ((int)keyData == (int)Keys.F6)
            {
                this.mContWaku.PerformClick();
                return true;
            }
            if ((int)keyData == (int)Keys.F5)
            {
                this.mAutoExtendBtn.PerformClick();
                return true;
            }
            if ((int)keyData == (int)Keys.F4)
            {
                this.mCommentBtn.PerformClick();
                return true;
            }
            if ((int)keyData == (int)Keys.F3)
            {
                this.mImakokoBtn.PerformClick();
                return true;
            }
            if ((int)keyData == (int)Keys.F2)
            {
                this.mCopyBtn.PerformClick();
                return true;
            }
            if ((int)keyData == (int)Keys.F1)
            {
                this.mWakutoriBtn.PerformClick();
                return true;
            }

            if ((int)keyData == (int)Keys.F12)
            {
                this.mConnectBtn.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------