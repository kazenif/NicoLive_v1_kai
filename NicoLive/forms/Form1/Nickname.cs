//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Threading;
using System.Windows.Forms;


namespace NicoLive
{
    partial class Form1
    {
        //-------------------------------------------------------------------------
        // ユーザー名取得
        //-------------------------------------------------------------------------
        private void GetUsername(string iUserID)
        {
            if (!Properties.Settings.Default.auto_username) return;

            int id;
            if (!int.TryParse(iUserID, out id)) return;

            if (!mGatherUserID.Contains(iUserID))
            {
                mGatherLock.AcquireWriterLock(Timeout.Infinite);
                mGatherUserID.Add(iUserID);
                mGatherLock.ReleaseWriterLock();
            }
        }

        //-------------------------------------------------------------------------
        // コテハン記憶
        //-------------------------------------------------------------------------
        public void SetNickname(string iID, string iName)
        {
            mUid.SetNickname(iID, iName);

            // メインのコメントリスト修正
            Utils.SetNickname(ref mCommentList, iID, iName);

            // 外部コメントリスト修正
            this.Invoke((Action)delegate()
            {
                mCommentForm.SetNickname(iID, iName);
            });
        }

        //-------------------------------------------------------------------------
        // コテハン記憶
        //-------------------------------------------------------------------------
        private void AddNickname(string iID, string iName)
        {
            if (iName.Length <= 0) return;

            if (mUid.AddNickname(iID, iName))
            {
                Utils.SetNickname(ref mCommentList, iID, iName);

                // 外部コメントリスト修正
                this.Invoke((Action)delegate()
                {
                    mCommentForm.SetNickname(iID, iName);
                });
            }
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
