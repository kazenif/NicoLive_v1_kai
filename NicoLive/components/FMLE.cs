//-------------------------------------------------------------------------
// FMLEcmdクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

namespace NicoLive
{
    class FMLE
    {
        // FME開始フラグ
        private static bool mFMEStarted = false;

        // FME設定文字列
        private static string mFMEsetting = null;

        public static bool FMEsettingExist
        {
            get;
            set;
        }

        public static string FMEsetting
        {
            get
            {
                if (mFMEsetting == null) return "";
                return mFMEsetting;
            }
        }


        public static bool FMEStarted
        {
            set { mFMEStarted = value; }
            get { return mFMEStarted; }
        }

        //-------------------------------------------------------------------------
        // 開始
        //-------------------------------------------------------------------------
        public static void Start(Dictionary<string, string> iParams)
        {
            // XSplit
            if (Properties.Settings.Default.use_xsplit)
            {
                XSplit.Start();
                using (Bouyomi bm = new Bouyomi())
                {
                    bm.Talk("エックスプリット配信を開始します。");
                }
                return;
            }

            // NLE
            if (Properties.Settings.Default.use_NLE)
            {
                NicoLiveEncoder.Start();
                using (Bouyomi bm = new Bouyomi())
                {
                    bm.Talk("ニコニコライブエンコーダ配信を開始します。");
                }
                return;
            }

            DeleteSessionfile();
            if (Properties.Settings.Default.fme_gui)
            {
                Thread th = new Thread(delegate()
                {
                    mFMEStarted = FMEGUI.Start(iParams);
                });
                th.Start();
            }
            else
            {

                if (hasFME()) return;
                // FMLECmdのパス
                string APP_PATH = Properties.Settings.Default.fmle_path;
                // プロファイルのパス
                string profile_path = Path.GetTempPath() + "nicovideo_fme.xml";

                Thread th = new Thread(delegate()
                {
                    // プロファイル作成
                    if (!MakeProfile(profile_path, iParams))
                    {
                        mFMEStarted = false;
                        return;
                    }

                    // FMLECmd起動
                    string args = " /p \"" + profile_path + "\"";

                    if (Properties.Settings.Default.fme_dos)
                    {
                        Process.Start(APP_PATH, args);
                    }
                    else
                    {
                        ProcessStartInfo psInfo = new ProcessStartInfo();
                        psInfo.FileName = APP_PATH;
                        psInfo.CreateNoWindow = true;
                        psInfo.UseShellExecute = false;
                        psInfo.Arguments = args;
                        Process.Start(psInfo);
                    }
                    using (Bouyomi bm = new Bouyomi())
                    {
                        bm.Talk("えふえむいー配信を開始しました");
                    }
                    mFMEStarted = true;
                });
                th.Start();
            }
        }

        //-------------------------------------------------------------------------
        // FMEが動作中かチェック
        //-------------------------------------------------------------------------
		public static bool hasFME() 
		{
            Process[] ps;

            // XSplit
            if (Properties.Settings.Default.use_xsplit)
            {
                return XSplit.IsBroadCast;
            }

            // NLE
            if (Properties.Settings.Default.use_NLE)
            {
                return NicoLiveEncoder.IsBroadCast;
            }

            if (Properties.Settings.Default.fme_gui)
            {
                return FMEGUI.hasFME();
            }
            else
            {
                ps = Process.GetProcessesByName("FMLECmd");
            }

            return (ps.Length > 0);
		}

        //-------------------------------------------------------------------------
        // セッションファイルを消去
        //-------------------------------------------------------------------------
        public static void DeleteSessionfile()
        {
            if (Properties.Settings.Default.fme_session == null) return;
            try
            {
                System.IO.FileInfo cFileInfo = new System.IO.FileInfo(Properties.Settings.Default.fme_session);
                if (cFileInfo.Exists)
                {
                    if ((cFileInfo.Attributes & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly)
                    {
                        cFileInfo.Attributes = System.IO.FileAttributes.Normal;
                    }
                    // ファイルを削除する
                    cFileInfo.Delete();
                }
            }
            catch (Exception)
            {
            }
        }

        //-------------------------------------------------------------------------
        // 停止
        //-------------------------------------------------------------------------
        public static void Stop()
        {
            // XSplit
            if (Properties.Settings.Default.use_xsplit)
            {
                XSplit.Stop();
                return;
            }

            // NLE
            if (Properties.Settings.Default.use_NLE)
            {
                NicoLiveEncoder.Stop();
                return;
            }

            if (Properties.Settings.Default.fme_gui)
            {
                FMEGUI.Kill();
            }
            else
            {
                Process[] ps = Process.GetProcessesByName("FMLECmd");

                foreach (System.Diagnostics.Process p in ps)
                {
                    p.Kill();
                }

                ps = Process.GetProcessesByName("FlashMediaLiveEncoder");

                foreach (System.Diagnostics.Process p in ps)
                {
                    p.Kill();
                }

                // すべてのFMEがいなくなったことを確認する
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    ps = Process.GetProcessesByName("FlashMediaLiveEncoder");
                } while (ps.Length > 0);
            }
            mFMEStarted = false;
            DeleteSessionfile();
        }

        //-------------------------------------------------------------------------
        // プロファイル生成
        //-------------------------------------------------------------------------
		public static bool MakeProfile(string iPath,Dictionary<string, string> iParams)
		{
            // テンプレート読み込み
            XmlDocument doc = new XmlDocument();
            try
            {
                string path = System.Windows.Forms.Application.StartupPath + "\\nicovideo_fme.xml";

                doc.Load(path);

                string url = iParams["url"];
                string stream = iParams["stream"];

                doc.SelectSingleNode("//flashmedialiveencoder_profile/output/rtmp/url").InnerText = url;
                doc.SelectSingleNode("//flashmedialiveencoder_profile/output/rtmp/stream").InnerText = stream;
                using (XmlTextWriter writer = new XmlTextWriter(iPath, null))
                {
                    doc.Save(writer);
                }

                // FME 配信文字列
                mFMEsetting = "";
                try
                {
                    mFMEsetting = "■映像：" + doc.SelectSingleNode("//flashmedialiveencoder_profile/encode/video/outputsize").InnerText +
                            ", " + doc.SelectSingleNode("//flashmedialiveencoder_profile/capture/video/frame_rate").InnerText +
                            "fps(" + doc.SelectSingleNode("//flashmedialiveencoder_profile/encode/video/datarate").InnerText +
                            "kbps)";
                    mFMEsetting = mFMEsetting + " ■音声：" + doc.SelectSingleNode("//flashmedialiveencoder_profile/capture/audio/sample_rate").InnerText +
                            "Hz (" + doc.SelectSingleNode("//flashmedialiveencoder_profile/encode/audio/datarate").InnerText +
                            "kbps)";
                }
                catch (Exception)
                {
                }
                mFMEsetting = mFMEsetting.Replace(";", "");
                FMEsettingExist = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
            return true;
		}
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
