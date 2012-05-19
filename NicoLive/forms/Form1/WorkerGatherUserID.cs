//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace NicoLive
{
    partial class Form1
    {
        //-------------------------------------------------------------------------
        // ユーザーID収集用ワーカー
        //-------------------------------------------------------------------------
        private void GatherUserIDWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            const int wait = 1000;
            //const int wait = 3000;

            while (true)
            {
                if (mGatherUserID.Count == 0)
                {
                    Thread.Sleep(wait);
                    continue;
                }

                if (mNico == null) continue;

                mGatherLock.AcquireWriterLock(Timeout.Infinite);

                string user_id = mGatherUserID[0];

                mGatherUserID.Remove(user_id);

                // 既に取得ずみ
                if (mUid.Contains(user_id))
                {
                    Thread.Sleep(wait);
                    mGatherLock.ReleaseWriterLock();
                    continue;
                }

                string name = mNico.GetUsername(user_id);


                Debug.WriteLine("Gather UserID: " + user_id + " -> Name: " + name);
                if (name.Length > 0)
                {
                    this.Invoke((Action)delegate()
                    {
                        SetNickname(user_id, name);
                    });
                }
                else
                {
                    mGatherUserID.Add(user_id);
                    Thread.Sleep(wait*10);
                }

                mGatherLock.ReleaseWriterLock();

                Thread.Sleep(wait);
            }
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
