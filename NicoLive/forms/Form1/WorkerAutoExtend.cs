//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NicoLive
{
    partial class Form1
    {
        bool NEED_FREE_EXTEND_0 = false;
        bool NEED_FREE_EXTEND_1 = false;
        bool NEED_FREE_EXTEND_2 = false;

        //-------------------------------------------------------------------------
        // 自動無料延長用ワーカー
        //-------------------------------------------------------------------------
        private void AutoExtendWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int wait = 1000 * 2;
            bool push_extend;
            bool extend_wait = false;

            using (AutoExtend ax = new AutoExtend())
            {
                while (true)
                {
                    push_extend = false;

                    if (mNico == null) return;

                    //if (!mNico.IsLogin) continue;

                    // ウェイト
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        Thread.Sleep(wait);
                        continue;
                    }

                    if (this.Bounds.Width < 50 || this.Bounds.Height < 50)
                    {
                        //Thread.Sleep(wait);
                        continue;
                    }

                    // 延長が完了したらしばらく休憩
                    if (extend_wait)
                    {
                        Thread.Sleep(1000 * 60 * 5);
                        extend_wait = false;
                        continue;
                    }

                    mIsExtend = false;


                    UInt32 time = mLiveInfo.Time;
                    UInt32 btime = mLiveInfo.StartTime;

                    if (btime < time)
                    {
                        int sub = Utils.CalcTime();
                        int min = sub / 60;

                       //--------------------- 無料延長開始 -----------------------
                        if (mAutoExtendBtn.Checked && min <= 4 &&  !NEED_FREE_EXTEND_0)  // 25分
                        {
                            NEED_FREE_EXTEND_0=true;
                            NEED_FREE_EXTEND_1=true;
                        }

                        string id="";
                        try
                        {
                            this.Invoke((Action)delegate()
                            {
                                id = LiveID;
                            });
                        }
                        catch (Exception)
                        {

                        }
                        //--------------------- 無料延長処理 -----------------------
                        if (id.Length>0 && !mIsExtend)
                        {
                            if (mOwnLive && NEED_FREE_EXTEND_1)
                            {

                                List<SaleList> list = new List<SaleList>();
                                mNico.GetSaleList(id, ref list);

                                // 無料延長探し
                                foreach (SaleList sale in list)
                                {
                                    if (sale.mItem != null && sale.mItem.Equals("freeextend"))
                                    {
                                        mFreeExtendItem = sale;
                                        mPushExtend        = true;
                                        push_extend        = true;
                                        NEED_FREE_EXTEND_2 = true;
                                        NEED_FREE_EXTEND_1 = false;
                                    }
                                }
                            }

                            if (mOwnLive && NEED_FREE_EXTEND_2)
                            {
                                if (mFreeExtendItem.mItem != null && mFreeExtendItem.mItem.Equals("freeextend"))
                                {
                                    if (mNico.Purchase(id, mLiveInfo.Token, mFreeExtendItem))
                                    {
                                        NEED_FREE_EXTEND_2=false;

                                        //this.SendComment(mMsg.GetMessage("延長完了"), true);
                                        // 2012.05.06
                                        mBouyomi.Talk(mMsg.GetMessage("延長を完了しました"));

                                        mPushExtend = false;
                                        push_extend = true;
                                        extend_wait = true;
                                        mIsExtend = true;

                                        // フォームをリロード
                                        // 2012.05.06
                                        if (Properties.Settings.Default.use_fme)
                                        {
                                            this.Invoke((Action)delegate()
                                            {
                                                this.GetPlayer();
                                            });
                                        }
                                        //else
                                        //{
                                        //    this.Invoke((Action)delegate()
                                        //    {
                                        //        this.Connect(false);
                                        //        extend_wait = false;
                                        //    });
                                        //} 
                                    }
                                }
                            }

                            if (sub > 29 * 60)
                            {
                                mIsExtend=false;
                                NEED_FREE_EXTEND_0=false;
                            }
                        }
                    }

                    if (Properties.Settings.Default.use_fme)
                    {
                        if (!mStartFME && mNico.IsLogin)
                        {
                            int r = ax.AutoFME(this.Bounds, this.Location.X, this.Location.Y);
                            if (r == 1)
                            {
                                FMLE_Exec();
                                mStartFME = true;

                                // Compact 予告時刻設定
                                mLastCompctTime = DateTime.Now;
                                mCompactForcast = false;
                            }
                        }
                    }
                    else
                    {
                        if (!push_extend && !mPushExtend)
                        {
                            // 自動配信開始
                            if (Properties.Settings.Default.auto_connect)
                            {
                                int r = ax.AutoStart(this.Bounds, this.Location.X, this.Location.Y);
                                switch (r)
                                {
                                    // 配信開始
                                    case 1:
                                        mPushStart = true;
                                        break;
                                    // OK
                                    case 2:
                                        if (mPushStart)
                                            mBouyomi.Talk(mMsg.GetMessage("配信を開始しました"));
                                        mPushStart = false;
                                        this.mLastChatTime = DateTime.Now;
                                        this.mLastCompctTime = DateTime.Now;
                                        this.mCompactForcast = false;
                                        break;
                                }

                            }
                        }
                    }

                    // ウェイト
                    Thread.Sleep(wait);
                }
            }
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
