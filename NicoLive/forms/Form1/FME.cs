//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace NicoLive
{
    partial class Form1
    {
        //-------------------------------------------------------------------------
        // FMLE起動
        //-------------------------------------------------------------------------
        private void FMLE_Exec()
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\nicovideo_fme.xml";
            if (!File.Exists(path))
            {
                MessageBox.Show("nicovideo_fme.xmlが見つかりません", "NicoLive");
                return;
            }

            Thread th = new Thread(delegate()
            {
                Nico nico = Nico.Instance;
                string lv = LiveID;
                if (lv.Length > 2)
                {
                    Dictionary<string, string> arr = nico.GetFMEProfile(lv);
                    if (arr["status"].Equals("ok"))
                    {
                        FMLE.Start(arr);
                    }
                }
            });
            th.Start();
        }
    }
}
//-------------------------------------------------------------------------
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------