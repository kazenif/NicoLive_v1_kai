using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace NicoLive 
{
    class FMEGUI
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int MoveWindow(IntPtr hwnd, int x, int y,
        int nWidth, int nHeight, int bRepaint);

        [DllImport("user32.dll")]
        extern static bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        extern static int AdjustWindowRect(ref RECT lpRect, int dwStyle, int bMenu);

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, ref  RECT lpRect);

        [DllImport("user32.dll")]
        private static extern int GetClientRect(IntPtr hwnd, ref  RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern int ClientToScreen(IntPtr hWnd, ref POINT pt);

        [DllImport("User32.Dll", CharSet = CharSet.Auto)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder s,int nMaxCount);

        [DllImport("user32.dll")]
        extern static bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        //-------------------------------------------------------------------------
        [DllImport("user32.dll")]
        extern static uint SendInput(
            uint nInputs,
            INPUT[] pInputs,
            int cbSize
            );

        [StructLayout(LayoutKind.Sequential)]
        struct WINDOWPLACEMENT
        {
            public int Length;
            public int flags;
            public int showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

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

        // Windowの表示・非表示
        [DllImport("user32.dll")]
        static extern int ShowWindow(IntPtr handle, int command);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint procId);

        // コールバックメソッドのデリゲート
        private delegate int EnumerateWindowsCallback(IntPtr hWnd, int lParam);

        [DllImport("user32", EntryPoint = "EnumWindows")]
        private static extern int EnumWindows(EnumerateWindowsCallback lpEnumFunc, int lParam);

        // dwFlags
        const int MOUSEEVENTF_MOVED = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;  // 左ボタン Down
        const int MOUSEEVENTF_LEFTUP = 0x0004;  // 左ボタン Up
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;  // 右ボタン Down
        const int MOUSEEVENTF_RIGHTUP = 0x0010;  // 右ボタン Up
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;  // 中ボタン Down
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;  // 中ボタン Up
        const int MOUSEEVENTF_WHEEL = 0x0080;
        const int MOUSEEVENTF_XDOWN = 0x0100;
        const int MOUSEEVENTF_XUP = 0x0200;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        const int SW_SHOWNORMAL = 1;
        const int SW_SHOW = 5;
        const int SW_MINIMIZE = 6;

        public const int GWL_HINSTANCE = (-6);
        public const int GWL_EXSTYLE = (-20);
        public const int GWL_STYLE= (-16);

        // FMEのプロセスを格納するための変数
        private static System.Diagnostics.Process mFMEprocess = null;

        // FMEのメインウインドウのウインドウハンドル
        private static IntPtr m_fme_hwnd = IntPtr.Zero;

        // マッチング用のビットマップ
        private static Bitmap mStartBmp = null;
        private static Bitmap mStopBmp  = null;
        private static Bitmap mNgBmp    = null;

        private static bool mFMEStarted = false;

        public static bool FMEStarted
        {
            get { return mFMEStarted; }
        }

        // ウィンドウを列挙するためのコールバックメソッド
        public static int EnumerateWindows(IntPtr hWnd, int lParam)
        {
            uint procId = 0;
            uint result = GetWindowThreadProcessId(hWnd, ref procId);
            if (procId == mFMEprocess.Id)
            {
                // 同じIDで複数のウィンドウが見つかる場合がある
                // とりあえず最初のウィンドウが見つかった時点で終了する
                StringBuilder sbWindwText = new StringBuilder(256);
                GetWindowText(hWnd, sbWindwText, sbWindwText.Capacity);
                if (sbWindwText.ToString().Contains("Adobe Flash Media Live Encoder")) {
                    m_fme_hwnd = hWnd;
                    return 0;
                }
            }

            // 列挙を継続するには0以外を返す必要がある
            return 1;
        }


        //-------------------------------------------------------------------------
        // 静的コンストラクタ
        //-------------------------------------------------------------------------
		static FMEGUI()
		{
            mStartBmp  = NicoLive.Properties.Resources.FMEstart;
            mStopBmp   = NicoLive.Properties.Resources.FMEstop;
            mNgBmp   = NicoLive.Properties.Resources.FMEng;
        }

        //-------------------------------------------------------------------------
        // 静的ディストラクタ
        //-------------------------------------------------------------------------
        public static void Dispose()
        {
            mStartBmp.Dispose();
            mStopBmp.Dispose();
            mNgBmp.Dispose();
        }

        //-------------------------------------------------------------------------
        // FMEが動作中かチェック
        //-------------------------------------------------------------------------
        public static bool hasFME()
        {
            Process[] ps = Process.GetProcessesByName("FlashMediaLiveEncoder");

            return (ps.Length > 0);
        }

        //-------------------------------------------------------------------------
        // FMEをすべてコロ助
        //-------------------------------------------------------------------------
        public static bool Kill()
        {
            Process[] ps = Process.GetProcessesByName("FlashMediaLiveEncoder");
            if (ps.Length > 0)
            {
                foreach (System.Diagnostics.Process p in ps)
                {
                    p.Kill();
                }
                using (Bouyomi bm = new Bouyomi())
                {
                    bm.Talk("えふえむいー配信を停止しました");
                }
            }

            // すべてのFMEがいなくなったことを確認する
            do
            {
                System.Threading.Thread.Sleep(1000);
                ps = Process.GetProcessesByName("FlashMediaLiveEncoder");
            } while (ps.Length > 0);

            // セッションファイルを殺す
            FMLE.DeleteSessionfile();

            if (mFMEprocess != null)
            {
                mFMEprocess.Close();
                mFMEprocess = null;
            }
            mFMEStarted = false;

            return (ps.Length > 0);
        }


        //--------------- DEBUG --------------------
        //string filePath = @"C:\screen.bmp";
        //bmp.Save(filePath, ImageFormat.Bmp);
        //Process.Start(filePath);
        //------------------------------------------

        public static bool FitWindowSize()
        {
            // プロセスが確保されていなければ、リターン
            if (mFMEprocess == null)
            {
                return false;
            }

            // プロセスが存在しても、終了していれば、リターン
            if (mFMEprocess.HasExited)
            {
                return false;
            }

            // FME のウインドウハンドルを探す
            EnumWindows(new EnumerateWindowsCallback(EnumerateWindows), 0);

            // ウインドウハンドルが見つからなければ、FALSE
            if (m_fme_hwnd == null)
            {
                return false;
            }

            // ウインドウをフォアグラウンドに
            SetForegroundWindow(m_fme_hwnd);  // アクティブにする

            // ウインドウが表示されるまで待つ
            WINDOWPLACEMENT wndplace = new WINDOWPLACEMENT();
            do
            {
                System.Threading.Thread.Sleep(500);
                GetWindowPlacement(m_fme_hwnd, ref wndplace);
            } while ((wndplace.showCmd != SW_SHOWNORMAL));

            // FMEの最小のサイズ
            RECT clientrect;
            clientrect.top = 0;
            clientrect.left = 0;
            clientrect.right = 984;
            clientrect.bottom = 541;

            // Windowのスタイルを取得
            int style = (int)GetWindowLong(m_fme_hwnd, GWL_STYLE);

            // クライアント領域から、Windowの大きさを求める
            AdjustWindowRect(ref clientrect, style, 0);

            RECT nowRect = new RECT();
            do
            {
                //ウィンドウの位置を(0, 0)に、サイズを984ｘ541に変更する
                MoveWindow(m_fme_hwnd, 0, 0,
                    clientrect.right - clientrect.left + 1,
                    clientrect.bottom - clientrect.top + 1, 1);

                //MoveWindow(m_fme_hwnd, 0, 0,
                //    1000,
                //    600, 1);

                //0.5秒間（500ミリ秒）停止する
                System.Threading.Thread.Sleep(500);
                GetWindowRect(m_fme_hwnd, ref nowRect);
            } while (nowRect.top!=0 || nowRect.left!=0);

            return true;
        }


 
        //-------------------------------------------
        // PushStart
        //-------------------------------------------
        public static bool PushStart()
        {
            const int padding = 10;
            const int padding2 = 10;

            mFMEStarted = false;

            // プロセスが確保されていなければ、リターン
            if (mFMEprocess == null)
            {
                return false;
            }

            // プロセスが存在しても、終了していれば、リターン
            if (mFMEprocess.HasExited)
            {
                return false;
            }

            // ウインドウのサイズをあわせる
            FitWindowSize();

            RECT clientrect = new RECT();


            // クライアント領域のスクリーンをキャプチャする
            Bitmap bmp=null;


            bool bStartPushed = false;
            bool bStartComplete = false;
            using (ScreenCapture scr = new ScreenCapture())
            {
                int i;
                Point outPos = new Point();

                Point start_offs = new Point(417, 512);
                Point stop_offs = new Point(556, 512);
                Point ng_offs = new Point(489, 357);


                // スタートボタンを探して押す
                for (i = 0; i < 60; i++)
                {
                    // ウインドウをフォアグラウンドに
                    SetForegroundWindow(m_fme_hwnd);  // アクティブにする

                    // クライアント領域のサイズを取得
                    GetClientRect(m_fme_hwnd, ref clientrect);
                    POINT p1, p2;
                    p1.x = clientrect.left;
                    p1.y = clientrect.top;
                    p2.x = clientrect.right;
                    p2.y = clientrect.bottom;

                    //クライアント領域座標をスクリーン座標に変換
                    ClientToScreen(m_fme_hwnd, ref p1);
                    ClientToScreen(m_fme_hwnd, ref p2);

                    // クライアント領域のレクトアングルを設定
                    Rectangle iRect = new Rectangle(p1.x, p1.y, p2.x - p1.x + 1, p2.y - p1.y + 1);

                    bmp = scr.Capture(iRect);
                    if (ContainBitmap(mStartBmp, bmp, start_offs.X - padding2, start_offs.Y - padding, start_offs.X + padding2, start_offs.Y + padding, ref outPos, 5))
                    {
                        MouseClick(p1.x + outPos.X + 18, p1.y + outPos.Y + 14);
                        bStartPushed = true;
                        // 立ち上がるのを待つ
                        using (Bouyomi bm = new Bouyomi())
                        {
                            bm.Talk("スタートボタン、プッシュ");
                        }
                        break;
                    }

                    bmp.Dispose();
                    bmp = null;
                    FitWindowSize();
                    // 0.5[s] wait
                    System.Threading.Thread.Sleep(500);
                }

                // スタートボタンが押せなかった場合
                if (!bStartPushed)
                {
                    if (bmp != null)
                    {
                        bmp.Dispose();
                        bmp = null;
                    }
                    // 立ち上がるのを待つ
                    using (Bouyomi bm = new Bouyomi())
                    {
                        bm.Talk("スタートボタンを代わりに押してください");
                    }
                }

                // スタートできたかのチェック
                for (i = 0; i < 40; i++)
                {
                    // ウインドウをフォアグラウンドに
                    SetForegroundWindow(m_fme_hwnd);  // アクティブにする

                    GetClientRect(m_fme_hwnd, ref clientrect);
                    POINT p1, p2;
                    p1.x = clientrect.left;
                    p1.y = clientrect.top;
                    p2.x = clientrect.right;
                    p2.y = clientrect.bottom;

                    //クライアント領域座標をスクリーン座標に変換
                    ClientToScreen(m_fme_hwnd, ref p1);
                    ClientToScreen(m_fme_hwnd, ref p2);

                    Rectangle iRect = new Rectangle(p1.x, p1.y, p2.x - p1.x + 1, p2.y - p1.y + 1);

                    bmp = scr.Capture(iRect);

                    // Stop が押せるようになれば、とりあえず成功判定
                    if (ContainBitmap(mStopBmp, bmp, stop_offs.X - padding2, stop_offs.Y - padding, stop_offs.X + padding2, stop_offs.Y + padding, ref outPos, 5))
                    {
                        // 立ち上がるのを待つ
                        using (Bouyomi bm = new Bouyomi())
                        {
                            bm.Talk("ストップボタン、点灯確認");
                        }
                        bStartComplete = true;
                        mFMEStarted = true;
                        bmp.Dispose();
                        bmp = null;
                        break;
                    }

                    // NGダイアログが出てきたら、NG判定
                    if (ContainBitmap(mNgBmp, bmp, ng_offs.X - padding2, ng_offs.Y - padding, ng_offs.X + padding2, ng_offs.Y + padding, ref outPos, 5))
                    {
                        MouseClick(p1.x + outPos.X + 10, p1.y + outPos.Y + 10);
                        if (bmp != null)
                        {
                            bmp.Dispose();
                            bmp = null;
                        }

                        // NG ダイアログが消えるのを待つ
                        int j;
                        for (j = 0; j < 10; j++)
                        {
                            GetClientRect(m_fme_hwnd, ref clientrect);

                            p1.x = clientrect.left;
                            p1.y = clientrect.top;
                            p2.x = clientrect.right;
                            p2.y = clientrect.bottom;

                            //クライアント領域座標をスクリーン座標に変換
                            ClientToScreen(m_fme_hwnd, ref p1);
                            ClientToScreen(m_fme_hwnd, ref p2);

                            iRect = new Rectangle(p1.x, p1.y, p2.x - p1.x + 1, p2.y - p1.y + 1);

                            bmp = scr.Capture(iRect);
                            if (!ContainBitmap(mNgBmp, bmp, ng_offs.X - padding2, ng_offs.Y - padding, ng_offs.X + padding2, ng_offs.Y + padding, ref outPos, 5))
                            {
                                if (bmp != null)
                                {
                                    bmp.Dispose();
                                    bmp = null;
                                }
                                break;
                            }
                            // 0.5[s] wait
                            System.Threading.Thread.Sleep(500);
                        }
                        break;
                    }
                    if (bmp != null)
                    {
                        bmp.Dispose();
                        bmp = null;
                    }

                    // 0.5[s] wait
                    FitWindowSize();
                    System.Threading.Thread.Sleep(500);
                }
            }

            //// Start領域
            //iRect = new Rectangle(p1.x+417, p1.y+512, 36, 28);
            //bmp = scr.Capture(iRect);
            //filePath = @"F:\FMEstart.bmp";
            //bmp.Save(filePath, ImageFormat.Bmp);

            //// Stop領域
            //iRect = new Rectangle(p1.x + 556, p1.y + 512, 34, 28);
            //bmp = scr.Capture(iRect);
            //filePath = @"F:\FMEstop.bmp";
            //bmp.Save(filePath, ImageFormat.Bmp);

            //// NG領域
            //iRect = new Rectangle(p1.x + 489, p1.y + 357, 21, 19);
            //bmp = scr.Capture(iRect);
            //filePath = @"F:\FMEng.bmp";
            //bmp.Save(filePath, ImageFormat.Bmp);

            //filePath = @"F:\screen.bmp";
            //Process.Start(filePath);

            //一秒間（1000ミリ秒）停止する
            System.Threading.Thread.Sleep(2000);

            if (bStartComplete)
            {
                using (Bouyomi bm = new Bouyomi())
                {
                    bm.Talk("えふえむいー配信を開始しました");
                }
            }
            else
            {
                using (Bouyomi bm = new Bouyomi())
                {
                    bm.Talk("えふえむいー起動失敗しました");
                }
                FMEGUI.Kill();
            }

            // ウインドウを最小化する
            ShowWindow(m_fme_hwnd, SW_MINIMIZE);
            return bStartComplete;
        }

        //-------------------------------------------
        // process をスタート
        //-------------------------------------------
        public static bool Start(Dictionary<string, string> iParams)
        {
            try
            {
                // 現在、動作しているプロセスをすべてコロ助
                Kill();

                // プロセスをスタートさせる

                // プロファイルのパス
                string profile_path = Path.GetTempPath() + "nicovideo_fme.xml";

                // FlashMediaEncoderのパス
                string path = Properties.Settings.Default.fmle_path.Replace("FMLECmd", "FlashMediaLiveEncoder");

                // プロファイル作成
                if (!FMLE.MakeProfile(profile_path, iParams))
                {
                    mFMEStarted = false;
                    return false;
                }

                // FMLECmd起動
                string args = " /g /p \"" + profile_path + "\"";
                mFMEprocess = System.Diagnostics.Process.Start(path, args);
                // ウインドウハンドルが生成されるまで待つ
                mFMEprocess.WaitForInputIdle();

                // 立ち上がるのを待つ
                using (Bouyomi bm = new Bouyomi())
                {
                    bm.Talk("えふえむいー起動中です");
                }
                System.Threading.Thread.Sleep(8000);

                return PushStart();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        //-------------------------------------------
        // process をクローズする
        //-------------------------------------------
        public static void Close()
        {
            if (mFMEprocess != null)
            {
                if (!mFMEprocess.HasExited)
                {
                    mFMEprocess.CloseMainWindow();
                    mFMEprocess.WaitForExit();
                }
                mFMEprocess.Close();
                mFMEprocess = null;
            }
        }


        //-------------------------------------------------------------------------
        // 画像ピクセル値比較（エラー率版）
        //-------------------------------------------------------------------------
        public static bool CompareBitmap2(Bitmap iBmp, Bitmap iTgt, int iOffsX, int iOffsY, int iThres)
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

            int tot_pix = width * height;             // トータルピクセル数
            int inv_pix = 0;                        // 不正ピクセル数
            int toler = iThres;                     // エラー率の閾値

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
                        if (Math.Abs(val1[pos1 + 0] - val2[pos2 + 0]) > 1+val1[pos1 + 0]/10 ||
                            Math.Abs(val1[pos1 + 1] - val2[pos2 + 1]) > 1+val1[pos1 + 1]/10 ||
                            Math.Abs(val1[pos1 + 2] - val2[pos2 + 2]) > 1+val1[pos1 + 2]/10)
                        {
                            inv_pix++;
                            if ((inv_pix * 100 / tot_pix) >= toler)
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
        public static bool ContainBitmap(Bitmap iBmp, Bitmap iBase, int iStartX, int iStartY, int iEndX, int iEndY, ref Point oPos, int iThres)
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
        private static void MouseClick(int iX, int iY)
        {
            Cursor.Position = new Point(iX, iY);

            //0.5秒間（500ミリ秒）停止する
            System.Threading.Thread.Sleep(500);

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
