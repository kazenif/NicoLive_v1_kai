//-------------------------------------------------------------------------
// 今ココなう！クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id: Bouyomi.cs 720 2010-07-13 01:23:24Z kintoki $
//-------------------------------------------------------------------------
using System.Diagnostics;
using System.Threading;

namespace NicoLive
{
    class ImakokoNow
    {
        //-------------------------------------------------------------------------
        // 起動
        //-------------------------------------------------------------------------
        public static void Launch()
        {
            if (!Properties.Settings.Default.launch_imk) return;
            Process[] ps = Process.GetProcessesByName("ImacocoNow");
            if (ps.Length > 0) return;

            Thread th = new Thread(delegate()
            {
                try
                {
                    Process.Start(Properties.Settings.Default.imk_path, "");
                }
                catch (System.Exception)
                {
                    System.Windows.Forms.MessageBox.Show("今ココなう！の起動に\n失敗しました", "NicoLive");
                }
            });
            th.Start();
        }

        //-------------------------------------------------------------------------
        // 終了
        //-------------------------------------------------------------------------
        public static void Exit()
        {
            if (!Properties.Settings.Default.launch_imk) return;
            Process[] ps = Process.GetProcessesByName("ImacocoNow");

            foreach (System.Diagnostics.Process p in ps)
            {
                p.CloseMainWindow();
                p.Kill();
            }
        }

    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id: Bouyomi.cs 720 2010-07-13 01:23:24Z kintoki $
//-------------------------------------------------------------------------