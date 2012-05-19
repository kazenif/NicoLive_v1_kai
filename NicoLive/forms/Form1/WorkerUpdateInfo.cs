//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System.ComponentModel;
using System.Threading;

namespace NicoLive
{
    partial class Form1
    {
        //-------------------------------------------------------------------------
        // 動画情報取得
        //-------------------------------------------------------------------------
        private void UpdateInfoWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (mNico != null && !mNico.WakutoriMode)
                {
                    // 放送情報更新
                    UpdateMovieInfo();

                    // 来場者数通知
                    SpeakVisitor();
                }

                // ちょっとウェイト
                Thread.Sleep(1000 * 15);
            }
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------