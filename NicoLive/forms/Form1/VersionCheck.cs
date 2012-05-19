//-------------------------------------------------------------------------
// Main Form
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System.Threading;
using System.Windows.Forms;

namespace NicoLive
{
    partial class Form1
    {
        //-------------------------------------------------------------------------
        // バージョンチェック
        //-------------------------------------------------------------------------
        private void CheckVersion()
        {
			Thread th = new Thread(delegate()
			{
				VersionCheck checker = new VersionCheck();

				if (!checker.Check(Application.ProductVersion))
				{
					NewVersion dlg = new NewVersion();
					dlg.ShowDialog();
					dlg.Dispose();
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
