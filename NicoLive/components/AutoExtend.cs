//-------------------------------------------------------------------------
// 自動無料延長クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{   
    class AutoExtend : IDisposable
    {
        //-------------------------------------------------------------------------
        [DllImport("user32.dll")]
        extern static uint SendInput(
            uint nInputs,
            INPUT[] pInputs,
            int cbSize
            );

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }
        // dwFlags
        const int MOUSEEVENTF_MOVED      = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN   = 0x0002;  // 左ボタン Down
        const int MOUSEEVENTF_LEFTUP     = 0x0004;  // 左ボタン Up
        const int MOUSEEVENTF_RIGHTDOWN  = 0x0008;  // 右ボタン Down
        const int MOUSEEVENTF_RIGHTUP    = 0x0010;  // 右ボタン Up
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;  // 中ボタン Down
        const int MOUSEEVENTF_MIDDLEUP   = 0x0040;  // 中ボタン Up
        const int MOUSEEVENTF_WHEEL      = 0x0080;
        const int MOUSEEVENTF_XDOWN      = 0x0100;
        const int MOUSEEVENTF_XUP        = 0x0200;
        const int MOUSEEVENTF_ABSOLUTE   = 0x8000;

        //-------------------------------------------------------------------------
        private Bitmap mEndingBmp = null;			// 残り３分前画像
        private Bitmap mEnchoBmp  = null;           // 時間延長タブ
        private Bitmap mFreeBmp   = null;           // 無料延長画像
        private Bitmap mStartBmp  = null;           // 放送開始画像
        private Bitmap mYesBmp    = null;           // はいボタン
        private Bitmap mOk3Bmp    = null;           // OKボタン
        private Bitmap mBroOkBmp  = null;           // 配信開始OKボタン
        private Bitmap mBroOk2Bmp = null;           // 配信開始OKボタン
        private Bitmap m500Bmp    = null;           // 500円延長ボタン
        private Bitmap m5002Bmp   = null;           // 500円延長ボタン
        private Bitmap mExtFailBmp= null;           // 延長ボタン失敗
        private Bitmap mErrBmp    = null;           // 延長エラー
        private Bitmap mPreChat   = null;
        private Bitmap mOpenChat  = null;
        private Bitmap mFME       = null;
        private Bitmap mStartFME  = null;
        private Bitmap mStopFME   = null;

        //-------------------------------------------------------------------------
        // コンストラクタ
        //-------------------------------------------------------------------------
		public AutoExtend()
		{
            mEnchoBmp  = NicoLive.Properties.Resources.encho;
            mEndingBmp = NicoLive.Properties.Resources.ending;
            mFreeBmp   = NicoLive.Properties.Resources.free2;
            mStartBmp  = NicoLive.Properties.Resources.start;
            mOk3Bmp    = NicoLive.Properties.Resources.ok3;
            mYesBmp    = NicoLive.Properties.Resources.yes2;
            mBroOkBmp  = NicoLive.Properties.Resources.bro_ok;
            mBroOk2Bmp = NicoLive.Properties.Resources.bro_ok2;
            m500Bmp    = NicoLive.Properties.Resources._500;
            m5002Bmp   = NicoLive.Properties.Resources._5002;
            mPreChat   = NicoLive.Properties.Resources.pre_chat;
            mOpenChat  = NicoLive.Properties.Resources.open_chat;
            mExtFailBmp= NicoLive.Properties.Resources.extend_fail;
            mErrBmp    = NicoLive.Properties.Resources.err;
            mFME       = NicoLive.Properties.Resources.fme;
            mStartFME  = NicoLive.Properties.Resources.start_fme;
            mStopFME   = NicoLive.Properties.Resources.stop_fme;
		}


        //-------------------------------------------------------------------------
        // 解放
        //-------------------------------------------------------------------------
        public void Dispose()
        {
            mEnchoBmp.Dispose();
            mEndingBmp.Dispose();
            mFreeBmp.Dispose();
            mStartBmp.Dispose();
            mOk3Bmp.Dispose();
            mYesBmp.Dispose();
            mBroOkBmp.Dispose();
            mBroOk2Bmp.Dispose();
            m5002Bmp.Dispose();
            m500Bmp.Dispose();
            mPreChat.Dispose();
            mOpenChat.Dispose();
            mExtFailBmp.Dispose();
            mErrBmp.Dispose();
            mFME.Dispose();
            mStartFME.Dispose();
            mStopFME.Dispose();
        }

        //-------------------------------------------------------------------------
        // 無料延長チェック
        //-------------------------------------------------------------------------
        public int CheckExtend( Rectangle iRc, int iX, int iY )
        {
            const int padding = 10;
            const int padding2 = 10;

            Point ending_offs = new Point(220, 106);     // 残り時間のオフセット
            Point encho_offs  = new Point(565, 104);     // 延長タブのオフセット
            Point free_offs   = new Point(510, 172);     // 購入ボタン
            Point _500_offs   = new Point(542, 138);     // 500円延長ボタンのオフセット

            Point yes_offs = new Point(394, 208);     // はいボタンのオフセット
            Point ok3_offs = new Point(463, 206);     // OK3ボタンのオフセット
            Point fail_offs = new Point(457, 164);     // 延長失敗OKボタンのオフセット
            Point err_offs = new Point(459, 167);     // 予期せぬOKボタンのオフセット

            using( ScreenCapture scr = new ScreenCapture() ) {
            	Bitmap bmp = scr.Capture(iRc);
                
                Point outPos = new Point();
				const int thres = 10;

				// 残り３分
				if (ContainBitmap(mEnchoBmp, bmp, encho_offs.X - padding, encho_offs.Y - padding, encho_offs.X + padding, encho_offs.Y + padding, ref outPos, 10))　     // 時間延長タブが選択されていない
				{
					// 時間延長タブを選択する
					MouseClick(iX + outPos.X + 10, iY + outPos.Y + 8);
				}
				else
				{
					if (ContainBitmap(mFreeBmp, bmp, free_offs.X - padding, free_offs.Y - padding, free_offs.X + padding, free_offs.Y + padding, ref outPos,5))          // 無料延長チェック
					{
						// 購入ボタンクリック
						MouseClick(iX + 712, iY + outPos.Y + 7);
                        bmp.Dispose();
                        bmp = null;
						return 1;
					}
					else if (ContainBitmap(m500Bmp, bmp, _500_offs.X - padding, _500_offs.Y - padding, _500_offs.X + padding, _500_offs.Y + padding, ref outPos, 5) ||
							ContainBitmap(m5002Bmp, bmp, _500_offs.X - padding, _500_offs.Y - padding, _500_offs.X + padding, _500_offs.Y + padding, ref outPos, 5))
					{
						// 500円延長時は更新ボタンをクリックする
						MouseClick(iX + outPos.X + 45, iY + outPos.Y + 12);
					}
				}

				// 延長失敗OKボタンをクリック
                if (ContainBitmap(mErrBmp, bmp, err_offs.X - padding2, err_offs.Y - padding, err_offs.X + padding2, err_offs.Y + padding, ref outPos, 5))
                {
                    MouseClick(iX + outPos.X + 19, iY + outPos.Y + 41);
                }
                else // 延長失敗OKボタンをクリック
                if (ContainBitmap(mExtFailBmp, bmp, fail_offs.X - padding2, fail_offs.Y - padding, fail_offs.X + padding2, fail_offs.Y + padding, ref outPos, 5))
                {
                    MouseClick(iX + outPos.X + 21, iY + outPos.Y + 52);
                }
                else // OKボタンをクリック
                if (ContainBitmap(mOk3Bmp, bmp, ok3_offs.X - padding2, ok3_offs.Y - padding, ok3_offs.X + padding2, ok3_offs.Y + padding, ref outPos, thres))
                {
                    MouseClick(iX + outPos.X + 17, iY + outPos.Y + 8);
                    bmp.Dispose();
                    bmp = null;
                    return 2;
                }

                // はいボタンをクリック
                if (ContainBitmap(mYesBmp, bmp, yes_offs.X - padding2, yes_offs.Y - padding, yes_offs.X + padding2, yes_offs.Y + padding, ref outPos, thres))
                {
                    MouseClick(iX + outPos.X + 5, iY + outPos.Y + 8);
                }
                bmp.Dispose();
                bmp = null;
            }
            
            return 0;
        }
         //-------------------------------------------------------------------------
        // 自動放送開始チェック
        //-------------------------------------------------------------------------
        public int AutoFME(Rectangle iRc, int iX, int iY)
        {
            const int padding = 10;
            const int padding2 = 10;
            const int thres = 10;

            Point fme_offs = new Point(665, 104); 
            Point start_offs = new Point(451, 213);
            Point stop_offs = new Point(450, 185);
           
            using (ScreenCapture scr = new ScreenCapture())
            {
                Bitmap bmp = scr.Capture(iRc);
                Point outPos = new Point();

                if (ContainBitmap(mFME, bmp, fme_offs.X - padding2, fme_offs.Y - padding, fme_offs.X + padding2, fme_offs.Y + padding, ref outPos, thres))
                {
                    MouseClick(iX + outPos.X + 12, iY + outPos.Y + 8);
                    bmp.Dispose();
                    bmp = null;
                    return 0;
                }
                if (ContainBitmap(mStopFME, bmp, stop_offs.X - padding2, stop_offs.Y - padding, stop_offs.X + padding2, stop_offs.Y + padding, ref outPos, thres))
                {
                    bmp.Dispose();
                    bmp = null;
                    return 1;
                }
                if (ContainBitmap(mStartFME, bmp, start_offs.X - padding2, start_offs.Y - padding, start_offs.X + padding2, start_offs.Y + padding, ref outPos, thres))
                {
                    MouseClick(iX + outPos.X + 12, iY + outPos.Y + 11);
                    bmp.Dispose();
                    bmp = null;
                    return 0;
                }
                bmp.Dispose();
                bmp = null;
                return 0;
            }
        }
        //-------------------------------------------------------------------------
        // 自動放送開始チェック
        //-------------------------------------------------------------------------
		public int AutoStart( Rectangle iRc, int iX, int iY )
		{
            const int padding = 10;
            const int padding2 = 10;
            const int thres = 10;

            Point start_offs = new Point(829, 115);    		// 放送開始ボタンのオフセット
            Point ok_offs    = new Point(424, 169);     	// 放送開始後のはいボタン
            Point prechat_offs = new Point(776, 108); 

            using (ScreenCapture scr = new ScreenCapture())
            {
                Point outPos = new Point();
                Bitmap bmp = scr.Capture(iRc);

                if (ContainBitmap(mPreChat, bmp, prechat_offs.X - padding2, prechat_offs.Y - padding, prechat_offs.X + padding2, prechat_offs.Y + padding, ref outPos, thres))
                {
                    MouseClick(iX + outPos.X + 12, iY + outPos.Y + 11);
                    bmp.Dispose();
                    bmp = null;
                    return 0;
                }

                if (ContainBitmap(mStartBmp, bmp, start_offs.X - padding, start_offs.Y - padding, start_offs.X + padding, start_offs.Y + padding, ref outPos, thres))
                {
                    MouseClick(iX + outPos.X + 50, iY + outPos.Y + 30);
                    bmp.Dispose();
                    bmp = null;
                    return 1;
                }
                if (ContainBitmap(mBroOkBmp, bmp, ok_offs.X - padding2, ok_offs.Y - padding, ok_offs.X + padding2, ok_offs.Y + padding, ref outPos, thres))
                {
                    MouseClick(iX + outPos.X + 20, iY + outPos.Y + 40);
                    bmp.Dispose();
                    bmp = null;
                    return 2;
                }
                if (ContainBitmap(mBroOk2Bmp, bmp, ok_offs.X - padding2, ok_offs.Y - padding, ok_offs.X + padding2, ok_offs.Y + padding, ref outPos, thres))
                {
                    MouseClick(iX + outPos.X + 20, iY + outPos.Y + 40);
                    bmp.Dispose();
                    bmp = null;
                    return 2;
                }
                if (ContainBitmap(mOpenChat, bmp, prechat_offs.X - padding2, prechat_offs.Y - padding, prechat_offs.X + padding2, prechat_offs.Y + padding, ref outPos, thres))
                {
                    MouseClick(iX + outPos.X + 12, iY + outPos.Y + 11);
                    bmp.Dispose();
                    bmp = null;
                    return 0;
                }
                bmp.Dispose();
                bmp = null;
            }
            return 0;
		}

        //-------------------------------------------------------------------------
        // 画像ピクセル値比較（完全一致版）
        //-------------------------------------------------------------------------
        public bool CompareBitmap( Bitmap iBmp,  Bitmap iTgt, int iOffsX, int iOffsY)
        {
            Debug.Assert(iBmp.PixelFormat == PixelFormat.Format24bppRgb, "iBmpのピクセルフォーマットがおかしい");
            Debug.Assert(iTgt.PixelFormat == PixelFormat.Format24bppRgb, "iTgtのピクセルフォーマットがおかしい");
            
            bool rc = true;

            BitmapData bmpData = iBmp.LockBits(new Rectangle(Point.Empty, iBmp.Size),
                                                ImageLockMode.ReadOnly,
                                                PixelFormat.Format24bppRgb);

            BitmapData tgtData = iTgt.LockBits(new Rectangle(Point.Empty, iTgt.Size),
                                                ImageLockMode.ReadOnly,
                                                PixelFormat.Format24bppRgb);
            int width = bmpData.Width;
            int height = bmpData.Height;
            
            if (width < 0 || height < 0) return false;

            unsafe
            {
                byte* val1 = (byte*)(void*)bmpData.Scan0;
                byte* val2 = (byte*)(void*)tgtData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int pos1 = x * 3 + bmpData.Stride * y;
                        int pos2 = (iOffsX+x) * 3 + tgtData.Stride * (y+iOffsY);

                        // 範囲チェック
                        if (pos1 + 2 >= bmpData.Stride * bmpData.Height  ||
                            pos2 + 2 >= tgtData.Stride * tgtData.Height )
                        {
                            rc = false;
                            goto done;
                        }
                        
                        // ピクセル値比較
                        if (val1[pos1 + 0] != val2[pos2 + 0] ||
                            val1[pos1 + 1] != val2[pos2 + 1] ||
                            val1[pos1 + 2] != val2[pos2 + 2])
                        {
                            rc = false;
                            goto done;
                        }
                    }
                }
            }

        done:
            iBmp.UnlockBits(bmpData);
            iTgt.UnlockBits(tgtData);
            return rc;
        }
        //-------------------------------------------------------------------------
        // 画像ピクセル値比較（エラー率版）
        //-------------------------------------------------------------------------
        public bool CompareBitmap2(Bitmap iBmp, Bitmap iTgt, int iOffsX, int iOffsY, int iThres)
        {
            Debug.Assert(iBmp.PixelFormat == PixelFormat.Format24bppRgb, "iBmpのピクセルフォーマットがおかしい");
            Debug.Assert(iTgt.PixelFormat == PixelFormat.Format24bppRgb, "iTgtのピクセルフォーマットがおかしい");

            bool rc = false;
            BitmapData bmpData;
            BitmapData tgtData;
            
            try
            {
                bmpData = iBmp.LockBits(new Rectangle(Point.Empty, iBmp.Size),
                                                    ImageLockMode.ReadOnly,
                                                    PixelFormat.Format24bppRgb);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
            try
            {
                tgtData = iTgt.LockBits(new Rectangle(Point.Empty, iTgt.Size),
                                                ImageLockMode.ReadOnly,
                                                PixelFormat.Format24bppRgb);
            }
            catch (Exception e)
            {
                iBmp.UnlockBits(bmpData);
                Debug.WriteLine(e.Message);
                return false;
            }

            int width = bmpData.Width;
            int height = bmpData.Height;

            int tot_pix = width*height;             // トータルピクセル数
            int inv_pix = 0;                        // 不正ピクセル数
            int toler = iThres;                     // エラー率の閾値
            int pix_thres = 3;

            if (width < 0 || height < 0) return false;

            unsafe
            {
                byte* val1 = (byte*)(void*)bmpData.Scan0;
                byte* val2 = (byte*)(void*)tgtData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int pos1 = x * 3 + bmpData.Stride * y;
                        int pos2 = (iOffsX + x) * 3 + tgtData.Stride * (y + iOffsY);

                        // 範囲チェック
                        if (pos1 + 2 >= bmpData.Stride * bmpData.Height ||
                            pos2 + 2 >= tgtData.Stride * tgtData.Height)
                        {
                            continue;
                        }

                        // ピクセル値比較
                        if (Math.Abs(val1[pos1 + 0] - val2[pos2 + 0]) > pix_thres ||
                            Math.Abs(val1[pos1 + 1] - val2[pos2 + 1]) > pix_thres ||
                            Math.Abs(val1[pos1 + 2] - val2[pos2 + 2]) > pix_thres)
                        {
                            inv_pix++;  
                            if ( (inv_pix * 100 / tot_pix) >= toler)
                                goto done;
                        }
                    }
                }
            }
            
            // エラー率5%以下はマッチしたと見なす
            if ((inv_pix * 100 / tot_pix) < toler)
                rc = true;

done:
            iBmp.UnlockBits(bmpData);
            iTgt.UnlockBits(tgtData);
            
            return rc;
        }
        //-------------------------------------------------------------------------
        // iBaseにiBmpが含まれているかチェック
        //-------------------------------------------------------------------------
        public bool ContainBitmap(Bitmap iBmp, Bitmap iBase, int iStartX, int iStartY, int iEndX, int iEndY, ref Point oPos, int iThres )
        {
            for (int y = iStartY; y < iEndY; y++)
            {
                for (int x = iStartX; x < iEndX; x++)
                {
                    if (CompareBitmap2(iBmp, iBase, x, y, iThres))
                    {
                        oPos.X = x;
                        oPos.Y = y;
                        Debug.WriteLine("POSITION -> x: " + x + "    y: " + y);
                        return true;
                    }
                }
            }
            //Debug.WriteLine("NOT FOUND");
            return false;
        }

        //-------------------------------------------------------------------------
        // 指定位置をマウスでクリック
        //-------------------------------------------------------------------------
        private void MouseClick(int iX, int iY)
        {
            Cursor.Position = new Point(iX,iY);

            INPUT[] input = new INPUT[2];  // 計3イベントを格納

            input[0].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            input[1].mi.dwFlags = MOUSEEVENTF_LEFTUP;

            //input[2].mi.dx = 0;
            //input[2].mi.dy = 0;
            //input[2].mi.dwFlags = MOUSEEVENTF_MOVED | MOUSEEVENTF_ABSOLUTE;

            // 操作の実行
            SendInput(2, input, Marshal.SizeOf(input[0]));
        }

    }
}
//-------------------------------------------------------------------------
// 自動無料延長クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
