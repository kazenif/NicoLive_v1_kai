//-------------------------------------------------------------------------
// スクリーンキャプチャクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    class ScreenCapture : IDisposable
    {
		//-------------------------------------------------------------------------
        // スクリーンショット取得
        //-------------------------------------------------------------------------
        public Bitmap Capture(Rectangle iBounds)
        {
            Rectangle rc = iBounds;

            Bitmap bmp = new Bitmap( rc.Width, rc.Height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                try
                {
                    g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

            }

			//--------------- DEBUG --------------------
            //string filePath = @"C:\screen.bmp";
            //bmp.Save(filePath, ImageFormat.Bmp);
            //Process.Start(filePath);
			//------------------------------------------
			
            return bmp;
        }
        //-------------------------------------------------------------------------
        // 解放
        //-------------------------------------------------------------------------
        public void Dispose()
        {
        }
    }
}
//-------------------------------------------------------------------------
// スクリーンキャプチャクラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
