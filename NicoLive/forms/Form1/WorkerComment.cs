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
        // コメント処理ワーカー
        //-------------------------------------------------------------------------
        private void CommentWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (mNico != null)
                {
                    // コメント処理
                    ParseComment();

                    // コメント読み上げ
                    SpeakComment();
                }
                // ちょっとウェイト
                Thread.Sleep(100);
            }
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------