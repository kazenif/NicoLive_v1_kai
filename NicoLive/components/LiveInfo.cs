//-------------------------------------------------------------------------
// 放送情報管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    //-------------------------------------------------------------------------
    // エラーステータス
    //-------------------------------------------------------------------------
    public enum InfoErr
    {
        ERR_NO_ERR = 0,
        ERR_COULD_NOT_GET,
        ERR_NOT_LOGIN
    };

	public class LiveInfo
	{
		private static LiveInfo mInstance = null;

	    // ユーザーリスト
        private List<string> mUserList = null;
		// アクティブ用ハッシュテーブル
        private Dictionary<string,long> mActiveHash = null;
        private ReaderWriterLock mActiveLock = new ReaderWriterLock();

        private string mRoomLabel = "";
        private UInt32 mTime = 0;
        private UInt32 mBaseTime = 0;
        private UInt32 mStartTime = 0;
        private UInt32 mEndTime = 0;
        private UInt32 mUnixTime = 0;
        private UInt32 mWatchCount = 0;
        private string mNickname = "";
		private string mToken = "";
        private string mTitle = "";

        // ルームラベル
        public string RoomLabel 
        {
            get { return mRoomLabel; }
            set { mRoomLabel = value; }
        }

        // 放送時間
        public UInt32 Time
        {
            get { return mTime; }
            set { mTime = value; }
        }
        // ベースタイム
        public UInt32 BaseTime
        {
            get { return mBaseTime; }
            set { mBaseTime = value; }
        }
        // スタートタイム
        public UInt32 StartTime
        {
            get { return mStartTime; }
            set { mStartTime = value; }
        }
        // 終了タイム
        public UInt32 EndTime
        {
            get { return mEndTime; }
            set { mEndTime = value; }
        }
        // Unixタイム
        public UInt32 UnixTime
        {
            get { return mUnixTime; }
            set { mUnixTime = value; }
        }
        // 来場者数
        public UInt32 WatchCount
        {
            get { return mWatchCount; }
            set { mWatchCount = value; }
        }
        // ユーザー名
        public string Nickname
        {
            get { return mNickname; }
            set { mNickname = value; }
        }
		// トークン 
		public string Token
		{
			get { return mToken; }
			set { mToken = value; }
		}
        // タイトル 
        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }
		//-------------------------------------------------------------------------
		// コンストラクタ
		//-------------------------------------------------------------------------
		private LiveInfo()
		{
			mActiveHash = new Dictionary<string,long>();
			mUserList = new List<string>();
		}

		//-------------------------------------------------------------------------
		// シングルトン用
		//-------------------------------------------------------------------------
		public static LiveInfo Instance
		{
			get 
			{
				if (mInstance == null)
				{
					mInstance = new LiveInfo();
				}
				return mInstance;
			}
		}

		//-------------------------------------------------------------------------
		// クリア
		//-------------------------------------------------------------------------
		public void Clear()
		{
			mUserList.Clear();
			mActiveHash.Clear();
			mEndTime = 0;
            UnixTime = Utils.GetUnixTime(DateTime.Now);
			mTime = 0;
            mToken = "";
            mTitle = "";
		}

		//-------------------------------------------------------------------------
		// アクティブ人数更新
		//-------------------------------------------------------------------------
		public void RefreshActive()
        {
            if (mActiveHash != null && mActiveHash.Count > 0)
            {
                // 10分以上経過したキーを列挙
                List<string> remKey = new List<string>();

                try
                {
                    mActiveLock.AcquireReaderLock(Timeout.Infinite);
                    // time_tに変換
                    DateTime startTime = new DateTime(1970, 1, 1);
                    UInt32 time_t = Convert.ToUInt32((DateTime.Now - startTime).TotalSeconds);

                    
                    foreach (string key in mActiveHash.Keys)
                    {
                        long t = (long)mActiveHash[key];

                        long diff = time_t - t - 32400;
                        if (diff > 60 * 10)    // 10分以内のコメント書き込みユーザー数
                        {
                            remKey.Add(key);
                        }
                    }
                }
                finally
                {
                    mActiveLock.ReleaseReaderLock();
                }

                try
                {
                    mActiveLock.AcquireWriterLock(Timeout.Infinite);

                    // 10分以上立ったIDを削除
                    foreach (string val in remKey)
                    {
                        mActiveHash.Remove(val);
                    }
                }
                finally
                {
                    mActiveLock.ReleaseWriterLock();
                }
            }
        }

		//-------------------------------------------------------------------------
		// アクティブ人数取得
		//-------------------------------------------------------------------------
		public int GetActiveCount()
		{
			return mActiveHash.Count;
		}

		//-------------------------------------------------------------------------
		// ユーザーをアクティブ化
		//-------------------------------------------------------------------------
		public void ActivateUser( string iID,string iDate )
		{
			// アクティブ数設定
            if (iDate.Length == 0) return;

            try
            {
                mActiveLock.AcquireWriterLock(Timeout.Infinite);
                mActiveHash[iID] = long.Parse(iDate);
            }
            finally
            {
                mActiveLock.ReleaseWriterLock();
            }
		}

		//-------------------------------------------------------------------------
		// ユーザー追加
		//-------------------------------------------------------------------------
		public void AddUser( string iID )
		{
			// ユーザーリストにＩＤを追加
            if (!mUserList.Contains(iID) )
                mUserList.Add(iID);
		}

		//-------------------------------------------------------------------------
		// トータル人数取得
		//-------------------------------------------------------------------------
		public int GetTotalCount()
		{
			return mUserList.Count;
		}

		//-------------------------------------------------------------------------
		// 情報取得
		//-------------------------------------------------------------------------
        public InfoErr GetInfo(string iLiveID)
		{
			Nico nico = Nico.Instance;
            Dictionary<string, string> info = nico.GetPublishStatus(iLiveID);
            Dictionary<string, string> info2 = nico.GetPlayerStatus(iLiveID);

            if (info2 != null)
            {
                // コミュ番号取得
                if (info2.ContainsKey("room_label"))
                {
                    RoomLabel = info2["room_label"];
                }
                if (info2.ContainsKey("title"))
                {
                    Title = info2["title"];
                }
            }

            if (info != null && info.ContainsKey("code") && info["code"].Equals("permission_denied"))
            {
                return InfoErr.ERR_NO_ERR;
            }

            if (info2 != null)
            {
                // 来場者数
                if (info2.ContainsKey("watch_count"))
                {
                    WatchCount = uint.Parse(info2["watch_count"].ToString());
                }
            }

			if( info != null )
			{
                if (!info.ContainsKey("token"))
                {
                    return InfoErr.ERR_NOT_LOGIN;
                }

				// 時間
				if (info.ContainsKey("time") &&
					info.ContainsKey("start_time") &&
					info.ContainsKey("base_time") )
				{
					Time = Convert.ToUInt32(info["time"]);
					BaseTime = Convert.ToUInt32(info["base_time"]);
                    StartTime = Convert.ToUInt32(info["start_time"]);
                    EndTime = Convert.ToUInt32(info["end_time"]);
                    UnixTime = Utils.GetUnixTime(DateTime.Now);
				}

				// ニックネーム
                if (info.ContainsKey("nickname"))
                {
                    Nickname = info["nickname"];
                }

				// トークン
				if( info.ContainsKey("token") ) 
				{
					Token = info["token"];
				}
			}

			InfoErr err = (info != null)?InfoErr.ERR_NO_ERR:InfoErr.ERR_COULD_NOT_GET;

			info = null;
			info2 = null;

			return err;
		}
	}
}

//-------------------------------------------------------------------------
// 放送情報管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
