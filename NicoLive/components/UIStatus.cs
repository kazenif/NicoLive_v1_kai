//-------------------------------------------------------------------------
// ステータスエリア管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Drawing;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
	public class UIStatus
	{
		private static UIStatus mInstance = null;

		// NetworkSpeed計測用
        private float mPrevSent = 0;
        private float mMaxSent = 0;
        private float mPrevBat = 100;

        public enum HardInfo
        {
            HARD_INFO_NONE=0,
            HARD_INFO_BAT_LOW,
        };

		//-------------------------------------------------------------------------
		// コンストラクタ
		//-------------------------------------------------------------------------
		private UIStatus()
		{
		}

		//-------------------------------------------------------------------------
		// シングルトン用
		//-------------------------------------------------------------------------
		public static UIStatus Instance
		{
			get 
			{
				if (mInstance == null)
				{
					mInstance = new UIStatus();
				}
				return mInstance;
			}
		}

        //-------------------------------------------------------------------------
        // ネットワーク情報更新
        //-------------------------------------------------------------------------
        public void SetNetworkInfo(ref ToolStripStatusLabel iLabel)
        {
            float sent = 0;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPv4InterfaceStatistics interfaceStatistic = nic.GetIPv4Statistics();
                sent += interfaceStatistic.BytesSent * 8;
            }

            float bytesSentSpeed = (sent - mPrevSent) / 1024;
            string unit_sent = "K";

            // 最大送信速度計算
            if (mPrevSent != 0)
            {
                if (mMaxSent <= bytesSentSpeed)
                {
                    mMaxSent = bytesSentSpeed;
                }
            }

            // 送信速度計算
            if (bytesSentSpeed > 1024)
            {
                bytesSentSpeed /= 1024;
                unit_sent = "M";
            }


            if (mPrevSent != 0)
            {
//                iLabel.Text = "上り：" + bytesSentSpeed.ToString("F0") + unit_sent + "bps / 最大：" + max_sent.ToString("F0") + unit_max_sent + "bps";
                iLabel.Text = "上り：" + bytesSentSpeed.ToString("F0") + unit_sent + "bps";
                iLabel.ForeColor = (bytesSentSpeed < 100) ? Color.Red : ((bytesSentSpeed<200)?Color.Black:Color.Green);
            }

            mPrevSent = sent;
        }
        //-------------------------------------------------------------------------
        // 負荷情報更新
        //-------------------------------------------------------------------------
        public HardInfo SetHardInfo(ref PerformanceCounter iCounter, ref ToolStripStatusLabel iCpuLabel, ref ToolStripStatusLabel iBatLabel)
        {
            // CPU負荷
            float val = iCounter.NextValue();
            string str = String.Format("CPU：{0,3:f0}%", val);
            iCpuLabel.Text = str;
            iCpuLabel.ForeColor = (val > 90) ? Color.Red : Color.Black;

            // バッテリー残量
            PowerStatus ps = SystemInformation.PowerStatus;

            HardInfo info = HardInfo.HARD_INFO_NONE;

            //電源に接続されているか
            switch (ps.PowerLineStatus)
            {
                case PowerLineStatus.Online:
                    iBatLabel.Text = "";
                    iBatLabel.Visible = false;
                    mPrevBat = 100;
                    break;
                case PowerLineStatus.Offline:
                    float bat_thres = Properties.Settings.Default.rest_batt;       // 20%以下で通知

                    iBatLabel.Visible = true;
                    float bat = ps.BatteryLifePercent * 100;
                    iBatLabel.Text = String.Format("電池：{0,3}%", bat);
                    iBatLabel.ForeColor = (bat < 10) ? Color.Red : Color.Black;
                    if (Properties.Settings.Default.talk_bat)
                    {
                        if (mPrevBat > bat_thres && bat <= bat_thres)
                        {
                            info = HardInfo.HARD_INFO_BAT_LOW;
                        }
                    }
                    mPrevBat = bat;
                    break;
                case PowerLineStatus.Unknown:
                    iBatLabel.Text = "";
                    iBatLabel.Visible = false;
                    break;
            }

            return info;
        }

        //-------------------------------------------------------------------------
        // アクティブユーザー数更新
        //-------------------------------------------------------------------------
        public void UpdateActive(ref ToolStripStatusLabel iLabel)
        {
            LiveInfo info = LiveInfo.Instance;
            info.RefreshActive();
            iLabel.Text = "アクティブ数：" + info.GetActiveCount();
        }

        //-------------------------------------------------------------------------
        // ユニークユーザー数更新
        //-------------------------------------------------------------------------
		public void SetUniq( ref ToolStripStatusLabel iLabel )
		{
			LiveInfo info = LiveInfo.Instance;
			iLabel.Text = "ユニーク数：" + info.GetTotalCount();
		}

	}
}

//-------------------------------------------------------------------------
// ステータスエリア管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
