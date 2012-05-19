//-------------------------------------------------------------------------
// FMEステータス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id: UserID.cs 575 2010-06-08 09:43:08Z kintoki $
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    public partial class FMEStatus : Form
    {
        private Form1 mOwner = null;

        public FMEStatus(Form1 iOwner)
        {
            InitializeComponent();
            mOwner = iOwner;
        }

        private void FMEStatus_Load(object sender, EventArgs e)
        {
            int btm = mOwner.Bottom;
            int right = mOwner.Right;

            Top = btm - 93;
            Left = right - 268;
        }

        private void Exec()
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\nicovideo_fme.xml";

            // XSplit配信の設定付加
            if (!File.Exists(path) && !Properties.Settings.Default.use_xsplit && !Properties.Settings.Default.use_NLE)
            {
                MessageBox.Show("nicovideo_fme.xmlが見つかりません", "NicoLive");
                return;
            }

            Thread th = new Thread(delegate()
            {
                Nico nico = Nico.Instance;
                string lv = "";
                this.Invoke((MethodInvoker)delegate()
                {
                    lv = mOwner.LiveID;
                });

                if (lv.Length > 2)
                {
                    Dictionary<string, string> arr = nico.GetFMEProfile(lv);
                    if (arr["status"].Equals("ok"))
                    {
                        FMLE.Start(arr);
                    }
                    else
                    {
                        MessageBox.Show("番組情報の取得に失敗しました", "NicoLive");
                    }
                }
                else
                {
                    MessageBox.Show("放送ＩＤが設定されていません", "NicoLive");
                }
            });
            th.Start();
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if( !FMLE.hasFME() )
                Exec();
        }

        private void Restart_Click(object sender, EventArgs e)
        {
            FMLE.Stop();
            // wait for FMLE stop (2011.1.11) by Kazenif
            // 非同期に修正 (2012.2.26)
            Thread th = new Thread(delegate()
            {
                while (FMLE.hasFME()) System.Threading.Thread.Sleep(500);
                Exec();
            });
            th.Start();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            FMLE.Stop();
        }

        private void mUITimer_Tick(object sender, EventArgs e)
        {
            // XSplit 関連設定
            if (Properties.Settings.Default.use_xsplit)
            {
                Start.Enabled = XSplit.IsAlive && ! XSplit.IsBroadCast;
                Restart.Enabled = XSplit.IsBroadCast;
                Stop.Enabled = XSplit.IsBroadCast;

                if (XSplit.IsAlive)
                {
                    mLabel.Text = (XSplit.IsBroadCast) ? "XSplitが放送中です" : "XSplitは停止中です";
                }
                else
                {
                    mLabel.Text = "XSplitは未起動です";
                }
                
                mLabel.ForeColor = (XSplit.IsBroadCast) ? Color.Red : Color.Black;
            }
            // NLE 関連設定
            else if (Properties.Settings.Default.use_NLE)
            {
                Start.Enabled = NicoLiveEncoder.IsAlive && !NicoLiveEncoder.IsBroadCast;
                Restart.Enabled = NicoLiveEncoder.IsBroadCast;
                Stop.Enabled = NicoLiveEncoder.IsBroadCast;

                if (NicoLiveEncoder.IsAlive)
                {
                    mLabel.Text = (NicoLiveEncoder.IsBroadCast) ? "NLEが放送中です" : "NLEは停止中です";
                }
                else
                {
                    mLabel.Text = "NLEは未起動です";
                }

                mLabel.ForeColor = (NicoLiveEncoder.IsBroadCast) ? Color.Red : Color.Black;
            }
            else
            {
                Start.Enabled = !FMLE.hasFME();
                Restart.Enabled = FMLE.hasFME();
                Stop.Enabled = FMLE.hasFME();

                mLabel.Text = (FMLE.hasFME()) ? "FMLEが作動中です" : "FMLEは停止中です";
                mLabel.ForeColor = (FMLE.hasFME()) ? Color.Red : Color.Black;
            }
        }

    }
}
//-------------------------------------------------------------------------
// FMEステータス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------