//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;

namespace NicoLive
{
    partial class Form1
    {
        //-------------------------------------------------------------------------
        // 外部コメントウィンド
        //-------------------------------------------------------------------------
        private void CommentBtn_Click(object sender, EventArgs e)
        {
            mCommentForm.Show();
        }

        //-------------------------------------------------------------------------
        // 接続ボタン押し
        //-------------------------------------------------------------------------
        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            Connect(false);
        }
        //-------------------------------------------------------------------------
        // 放送ＩＤでエンターキーが押された時
        //-------------------------------------------------------------------------
        private void LiveID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                Connect(false);
            }
        }
        //-------------------------------------------------------------------------
        // 自動枠取り画面起動
        //-------------------------------------------------------------------------
        private void WakutoriBtn_Click(object sender, EventArgs e)
        {
            //if (Properties.Settings.Default.UseBrowserCookie)
            //{
            //    mNico.Login(Properties.Settings.Default.user_id, Properties.Settings.Default.password);
            //}
            mNico.Login(Properties.Settings.Default.user_id, Properties.Settings.Default.password);

            MakeWakutori(false);
        }
        //-------------------------------------------------------------------------
        // コメント読み上げ起動
        //-------------------------------------------------------------------------
        private void BouyomiBtn_Click(object sender, EventArgs e)
        {
            if (mBouyomiBtn.Checked)
            {
                mBouyomi.Talk(mMsg.GetMessage("読み上げを開始します"));
            }
        }

        //-------------------------------------------------------------------------
        // コメント読み上げインジケーター
        //-------------------------------------------------------------------------
        private void mBouyomiBtn_CheckStateChanged(object sender, EventArgs e)
        {
            if (mBouyomiBtn.Checked)
            {
                mBouyomiBtn.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                mBouyomiBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }

        //-------------------------------------------------------------------------
        // 来場者読み上げインジケーター
        //-------------------------------------------------------------------------
        private void mVisitorBtn_CheckStateChanged(object sender, EventArgs e)
        {
            if (mVisitorBtn.Checked)
            {
                mVisitorBtn.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                mVisitorBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }

        //-------------------------------------------------------------------------
        // 自動延長インジケーター
        //-------------------------------------------------------------------------
        private void mAutoExtendBtn_CheckStateChanged(object sender, EventArgs e)
        {
            if (mAutoExtendBtn.Checked)
            {
                mAutoExtendBtn.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                mAutoExtendBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }

        //-------------------------------------------------------------------------
        // 連続枠取りインジケーター
        //-------------------------------------------------------------------------
        private void mContWaku_CheckStateChanged(object sender, EventArgs e)
        {
            if (mContWaku.Checked)
            {
                mContWaku.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                mContWaku.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }
    

        //-------------------------------------------------------------------------
        // 自動無料延長
        //-------------------------------------------------------------------------
        private void AutoExtendBtn_Click(object sender, EventArgs e)
        {
            if (mAutoExtendBtn.Checked)
            {
                mBouyomi.Talk(mMsg.GetMessage("自動無料延長を開始します"));
            }
        }

        //-------------------------------------------------------------------------
        // 前枠の続き
        //-------------------------------------------------------------------------
        private void mCopyBtn_Click(object sender, EventArgs e)
        {

            WakuDlg dlg = new WakuDlg(ParseLiveID(), true);
            dlg.ShowDialog();

            if (dlg.mState == WakuResult.NO_ERR)
            {
                //using (Bouyomi bm = new Bouyomi())
                //{
                //    bm.Talk(mMsg.GetMessage("枠が取れたよ"));
                //}

                this.LiveID = dlg.mLv;
                MakeWakumachi();    // Wakumachi にならぶ 2012.05.12
//                Connect(true);
            }
            else if (dlg.mState == WakuResult.JUNBAN)
            {
//                MakeWakutori(false);
                MakeWakumachi();    // Wakumachi にならぶ 2012.05.12
            }
        }

        //-------------------------------------------------------------------------
        // 今ココ起動
        //-------------------------------------------------------------------------
        private void ImakokoBtn_Click(object sender, EventArgs e)
        {
            Imakoko imk = new Imakoko();
            imk.MyOwner = this;
            imk.Show();

            /*
            // --  DEBUG --
            using (ScreenCapture scr = new ScreenCapture())
            {
                Bitmap bmp2 = scr.Capture(this.Bounds);
                string filePath = @"screen.bmp";
                bmp2.Save(filePath, ImageFormat.Bmp);
                Process.Start(filePath);
            }
            */

            /*
            using (AutoExtend ax = new AutoExtend())
            {
                using (ScreenCapture scr = new ScreenCapture())
                {
                    //Bitmap bmp1 = NicoLive.Properties.Resources._500;
                    //Bitmap bmp1 = NicoLive.Properties.Resources.bro_ok;
                    //Bitmap bmp1 = NicoLive.Properties.Resources.bro_ok2;
                    Bitmap bmp1 = NicoLive.Properties.Resources.stop_fme;

                    Bitmap bmp2 = new Bitmap("screen4.bmp");
                    //Bitmap bmp2 = scr.Capture(this.Bounds);
                    
                    //string filePath = @"C:\screen.bmp";
                    //bmp2.Save(filePath, ImageFormat.Bmp);
                    //Process.Start(filePath);
                    
                    int padding = 10;
                    Point start_offs = new Point(408, 202);    // 放送開始ボタンのオフセット
                    Point pos = new Point();
                    ax.ContainBitmap(bmp1, bmp2, 0, 0, 1024, 768, ref pos,10);
                    //ax.ContainBitmap(bmp1, bmp2, start_offs.X - padding, start_offs.Y - padding, start_offs.X + padding, start_offs.Y + padding, ref pos);
                    
                    //ax.CheckExtend(this.Bounds, this.Location.X, this.Location.Y);
                }
            }
            */

            // --  DEBUG --
        }

    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------