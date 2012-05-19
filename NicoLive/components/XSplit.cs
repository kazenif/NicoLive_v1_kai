using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;


namespace NicoLive
{
    class XSplit
    {
        //
        //
        // (1). Formを作成時に、インスタンスを作成
        //      XSplit mXSplit = new XSplit();
        //
        // (2). Timerを用いて、上記インスタンスから、
        //      HandlingStatus() を定期的に呼び出す。
        //      mXSplit.HandlingStatus();
        //
        // (3). 配信をスタートする時には、
        //      mXSplit.Start()
        //
        // (4). 配信を停止するときには、
        //      mXSplit.Stop();
        //
        // (5). XSplitが放送中かチェック
        //      mXSplit.IsBroadCast でチェック可能。
        //
        // (6). XSplitプロセスが存在するかチェック
        //      XSplit.IsAlive でチェック可能
        //
        //

        const string XSplitClassName = "WindowsForms10.Window.8.app.0.3ce0bb8";
        const string XSplitNewsTitle = "最新情報";
        const string XSplitName = "XSplit.Core";

        public enum XSplit_Status { XS_IDLE = 0, XS_NEED_STOP, XS_NEED_START, XS_NEED_CHECK_CLIPBOARD, XS_NEED_RESTART };

        private static IntPtr hXSplitWnd;

        private static XSplit_Status require_status = XSplit_Status.XS_IDLE;

        private static Bitmap mHaishinBmp = null;
        private static Bitmap mNicoBmp = null;
        private static string mLastBroadcast = "";


        #region Win32 Functions
        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;

        }

        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

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
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, ref  RECT lpRect);

        [DllImport("user32.dll")]
        private static extern int GetClientRect(IntPtr hwnd, ref  RECT lpRect);

        [DllImport("user32.dll")]
        extern static bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr SetFocus(IntPtr hWnd);

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)]
            public uint type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        [DllImport("user32.dll")]
        extern static bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern int ClientToScreen(IntPtr hWnd, ref POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string ClassName, string WindowName);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        extern static int SendInput(int nInputs, INPUT[] pInputs, int cbSize);

        System.IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr handle, int command);

        [DllImport("user32.dll")]
        extern static IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int msg, int wParam, StringBuilder sb);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        private delegate int EnumWindowsDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsDelegate lpEnumFunc, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        extern static IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            ///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,

            /// <summary>
            ///     Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            SWP_DEFERERASE = 0x2000,

            /// <summary>
            ///     Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            SWP_DRAWFRAME = 0x0020,

            /// <summary>
            ///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,

            /// <summary>
            ///     Hides the window.
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,

            /// <summary>
            ///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x0010,

            /// <summary>
            ///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,

            /// <summary>
            ///     Retains the current position (ignores X and Y parameters).
            /// </summary>
            SWP_NOMOVE = 0x0002,

            /// <summary>
            ///     Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,

            /// <summary>
            ///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            SWP_NOREDRAW = 0x0008,

            /// <summary>
            ///     Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            SWP_NOREPOSITION = 0x0200,

            /// <summary>
            ///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,

            /// <summary>
            ///     Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x0001,

            /// <summary>
            ///     Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x0004,

            /// <summary>
            ///     Displays the window.
            /// </summary>
            SWP_SHOWWINDOW = 0x0040,

            // ReSharper restore InconsistentNaming
        }

        #endregion

        #region Win32 Constant

        private const int KEYEVENTF_EXTENDEDKEY = 0x1;
        private const int KEYEVENTF_KEYUP = 0x2;

        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;

        private const int WM_GETTEXT = 0x000D;
        private const int WM_CLOSE = 0x0010;

        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_COMMAND = 0x111;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_HOTKEY = 0x312;

        const int MOUSEEVENTF_LEFTDOWN = 0x0002;  // 左ボタン Down
        const int MOUSEEVENTF_LEFTUP = 0x0004;  // 左ボタン Up

        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        private const int MOD_ALT = 0x1;
        private const int MOD_CONTROL = 0x2;
        private const int MOD_SHIFT = 0x4;
        private const int MOD_WIN = 0x8;

        private const int VK_SHIFT = 0x10;
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;
        private const int VK_A = 0x41;
        private const int VK_F10 = 0x79;

        const int SW_SHOWNORMAL = 1;
        #endregion


        private static string Text
        {
            get
            {
                if (hXSplitWnd == IntPtr.Zero)
                {
                    FindXSplitMainWindow();
                }
                return GetWindowText(hXSplitWnd);
            }
        }

        Regex regex = new System.Text.RegularExpressions.Regex("Streaming Live - NicoVideo - \\S+\\s+Bitrate:\\d+\\.\\d+Kbps");

        public static bool IsBroadCast
        {
            get
            {
                if (IsAlive)
                {
                    string caption = Text;
                    if (caption==null || caption.Length == 0)
                    {
                        FindXSplitMainWindow();
                        caption = Text;
                    }
                    if (!mLastBroadcast.Contains("Bitrate:") &&
                        caption.Contains("Bitrate:"))
                    {
                        using (Bouyomi bm = new Bouyomi())
                        {
                            bm.Talk("配信ストリーム、ポジティブ");
                        }
                    }
                    if (mLastBroadcast.Contains("Bitrate:") &&
                        !caption.Contains("Bitrate:"))
                    {
                        using (Bouyomi bm = new Bouyomi())
                        {
                            bm.Talk("配信ストリーム、失速");
                        }
                    }
                    mLastBroadcast = caption;
                    return caption.Contains("Streaming Live - NicoVideo -");
//                    return regex.Match(caption).Success;
                }
                return false;
            }
        }


        // コンストラクタ
        //public static XSplit()
        //{
        //    FindXSplitMainWindow();
        //}

        //-------------------------------------------------------------------------
        // 静的コンストラクタ
        //-------------------------------------------------------------------------
        static XSplit()
		{
            mHaishinBmp = NicoLive.Properties.Resources.haishin;
            mNicoBmp   = NicoLive.Properties.Resources.nico;
        }

        //-------------------------------------------------------------------------
        // 静的ディストラクタ
        //-------------------------------------------------------------------------
        public static void Dispose()
        {
            mHaishinBmp.Dispose();
            mNicoBmp.Dispose();
        }

        public static void StartNicoLive(Form mainForm)
        {
            Clipboard.Clear();

            const int padding = 3;
            const int padding2 = 3;

            FindXSplitMainWindow();

            IntPtr hAfterWnd = IntPtr.Zero;

            // ウインドウが最小化状態だったか否か
            bool original_iconic = IsIconic(hXSplitWnd);

            if (!original_iconic)
            {
                hAfterWnd = GetWindow(hXSplitWnd, (uint)GetWindow_Cmd.GW_HWNDPREV);
                while (hAfterWnd != IntPtr.Zero && !IsWindowVisible(hAfterWnd))
                {
                    hAfterWnd = GetWindow(hAfterWnd, (uint)GetWindow_Cmd.GW_HWNDPREV);
                }
            }

            // クライアント領域のスクリーンをキャプチャする
            Bitmap bmp = null;

            //// ウィンドウを元に戻す
            ShowWindow(hXSplitWnd, SW_RESTORE);
            System.Threading.Thread.Sleep(500);

            //// ウインドウをフォアグラウンドに
            SetForegroundWindow(hXSplitWnd);  // アクティブにする
            System.Threading.Thread.Sleep(500);

            //// ウインドウが表示されるまで待つ
            WINDOWPLACEMENT wndplace = new WINDOWPLACEMENT();
            do
            {
                System.Threading.Thread.Sleep(500);
                GetWindowPlacement(hXSplitWnd, ref wndplace);
            } while ((wndplace.showCmd != SW_SHOWNORMAL));


            bool bStartPushed = false;

            using (ScreenCapture scr = new ScreenCapture())
            {
                // クライアント領域のサイズを取得
                RECT clientrect = new RECT();
                POINT p1, p2;
                Rectangle iRect;

                // クライアント領域の取得
                GetClientRect(hXSplitWnd, ref clientrect);

                p1.x = clientrect.left;
                p1.y = clientrect.top;
                p2.x = clientrect.right;
                p2.y = clientrect.bottom;

                //クライアント領域座標をスクリーン座標に変換
                ClientToScreen(hXSplitWnd, ref p1);
                ClientToScreen(hXSplitWnd, ref p2);

                //取り込む座標を矩形領域に設定
                iRect = new Rectangle(p1.x, p1.y, p2.x - p1.x + 1, p2.y - p1.y + 1);

                Point outPos = new Point();
                Point haishin_offs = new Point(125, 3);
                Point nico_offs = new Point(143, 25);

                int i;
                // 配信メニューを押す
                for (i = 0; i < 60; i++)
                {
                    bmp = scr.Capture(iRect);
                    //bmp.Save("D:\\start_wait.png");
                    if (ContainBitmap(mHaishinBmp, bmp, haishin_offs.X - padding2, haishin_offs.Y - padding, haishin_offs.X + padding2, haishin_offs.Y + padding, ref outPos, 5))
                    {
                        using (Bouyomi bm = new Bouyomi())
                        {
                            bm.Talk("配信メニュー選択");
                        }
                        System.Threading.Thread.Sleep(500);
//                        MouseClick(p1.x + 139, p1.y + 13);
                        bStartPushed = true;
                        // 立ち上がるのを待つ
                        break;
                    }
                    bmp.Dispose();
                    bmp = null;
                    //// ウィンドウを元に戻す
                    ShowWindow(hXSplitWnd, SW_RESTORE);
                    // アクティブにする
                    SetForegroundWindow(hXSplitWnd);

                    // ウエイト
                    System.Threading.Thread.Sleep(500);

                    // クライアント領域を再取得
                    GetClientRect(hXSplitWnd, ref clientrect);

                    p1.x = clientrect.left;
                    p1.y = clientrect.top;
                    p2.x = clientrect.right;
                    p2.y = clientrect.bottom;

                    //クライアント領域座標をスクリーン座標に変換
                    ClientToScreen(hXSplitWnd, ref p1);
                    ClientToScreen(hXSplitWnd, ref p2);

                    //取り込む座標を矩形領域に設定
                    iRect = new Rectangle(p1.x, p1.y, p2.x - p1.x + 1, p2.y - p1.y + 1);
                }

                //// ニコ生を押す
                if (bStartPushed)
                {

                    for (i = 0; i < 60; i++)
                    {
                        // ここでマウスクリック
                        MouseClick(p1.x + 139, p1.y + 13);

                        System.Threading.Thread.Sleep(500);
                        // もはや、座標は変えられない
                        bmp = scr.Capture(iRect);
                        //bmp.Save("D:\\nico_wait.png");
                        if (ContainBitmap(mNicoBmp, bmp, nico_offs.X - padding2, nico_offs.Y - padding, nico_offs.X + padding2, nico_offs.Y + 64 + padding, ref outPos, 5))
                        {
                            using (Bouyomi bm = new Bouyomi())
                            {
                                bm.Talk("ニコ生プッシュ");
                            }
                            MouseClick(p1.x + outPos.X + 8, p1.y + outPos.Y + 14);
//                            MouseClick(p1.x + 151, p1.y + 38);

                            break;　// 押し下げ成功
                        }
                        bmp.Dispose();
                        bmp = null;

                        //// ウィンドウを元に戻す
                        ShowWindow(hXSplitWnd, SW_RESTORE);
                        // アクティブにする
                        SetForegroundWindow(hXSplitWnd);

                        System.Threading.Thread.Sleep(500);

                        // クライアント領域を再取得
                        GetClientRect(hXSplitWnd, ref clientrect);

                        p1.x = clientrect.left;
                        p1.y = clientrect.top;
                        p2.x = clientrect.right;
                        p2.y = clientrect.bottom;

                        //クライアント領域座標をスクリーン座標に変換
                        ClientToScreen(hXSplitWnd, ref p1);
                        ClientToScreen(hXSplitWnd, ref p2);

                        //取り込む座標を矩形領域に設定
                        iRect = new Rectangle(p1.x, p1.y, p2.x - p1.x + 1, p2.y - p1.y + 1);
                    }
                }
            }

            System.Threading.Thread.Sleep(2000);

            // ウインドウを最小化する
            if (original_iconic)
            {
                ShowWindow(hXSplitWnd, SW_MINIMIZE);
            }
            else
            {
                Thread th = new Thread(delegate()
                {
                    Thread.Sleep(5000);
                    mainForm.Invoke((Action)delegate()
                    {
                        mainForm.Activate();
                    });

                    //if (hAfterWnd != null && hAfterWnd != (IntPtr)(-1) && hAfterWnd != (IntPtr)(-2))
                    //{
                    //    SetWindowPos(hXSplitWnd, hAfterWnd, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
                    //}
                });
                th.Start();
            }
        }

        public static void StopNicoLive()
        {
            FindXSplitMainWindow();

            // クライアント領域のスクリーンをキャプチャする
            //Bitmap bmp = null;

            //// ウィンドウを元に戻す
            ShowWindow(hXSplitWnd, SW_RESTORE);
            System.Threading.Thread.Sleep(500);

            //// ウインドウをフォアグラウンドに
            SetForegroundWindow(hXSplitWnd);  // アクティブにする
            System.Threading.Thread.Sleep(500);

            //// ウインドウが表示されるまで待つ
            WINDOWPLACEMENT wndplace = new WINDOWPLACEMENT();
            do
            {
                System.Threading.Thread.Sleep(500);
                GetWindowPlacement(hXSplitWnd, ref wndplace);
            } while ((wndplace.showCmd != SW_SHOWNORMAL));

            // クライアント領域のサイズを取得
            RECT clientrect = new RECT();

            GetClientRect(hXSplitWnd, ref clientrect);
            POINT p1, p2;
            p1.x = clientrect.left;
            p1.y = clientrect.top;
            p2.x = clientrect.right;
            p2.y = clientrect.bottom;

            //クライアント領域座標をスクリーン座標に変換
            ClientToScreen(hXSplitWnd, ref p1);
            ClientToScreen(hXSplitWnd, ref p2);

            // 配信ボタンを押下
            MouseClick(p1.x + 139, p1.y + 13);

            System.Threading.Thread.Sleep(1000);

            // クライアント領域のレクトアングルを設定
            //Rectangle iRect = new Rectangle(p1.x, p1.y, p2.x - p1.x + 1, p2.y - p1.y + 1);

            //using (ScreenCapture scr = new ScreenCapture())
            //{
            //    bmp = scr.Capture(iRect);
            //    bmp.Save("d:\\test.png");
            //}
            // 配信ボタンを押下
            MouseClick(p1.x + 151, p1.y + 38);

            System.Threading.Thread.Sleep(1000);

            // ウインドウを最小化する
            ShowWindow(hXSplitWnd, SW_MINIMIZE);
        }

        //
        //  この関数を定期的に呼ぶ
        //
        public static void HandlingStatus(string id, Form mf=null)
        {
            // 停止は、idなくても可能
            if (require_status != XSplit_Status.XS_NEED_STOP)
            {
                if (id == null) return;
                if (id.Length < 2) return;
            }

            // XSplitのプロセスが無いときは、何も出来ない。
            if (IsAlive)
            {
                if (require_status == XSplit_Status.XS_IDLE) return;

                string caption = Text;
                if (caption == null) return;

                // 過渡状態では、ダイアログのお掃除のみ
                if (caption.Contains("配信を初期化しています"))
                {
                    SweepDialogs();
                    return;
                }

                switch (require_status)
                {
                    case XSplit_Status.XS_NEED_START:
                        if (caption.Contains("Streaming Live - NicoVideo - "))
                        {
                            require_status = XSplit_Status.XS_IDLE;
                            return;
                        }
                        if (caption.Contains("XSplit Broadcaster - 配信中"))
                        {
                            require_status = XSplit_Status.XS_IDLE;
                            return;
                        }
                        StartNicoLive(mf);
                        //SHIFT_CONTROL_ALT_A();
                        require_status = XSplit_Status.XS_NEED_CHECK_CLIPBOARD;
                        break;

                    case XSplit_Status.XS_NEED_CHECK_CLIPBOARD:
                        if (caption.Equals("XSplit Broadcaster"))
                        {
                            using (Bouyomi bm = new Bouyomi())
                            {
                                bm.Talk("エックスプリット、ニコ生接続失敗");
                            }
                            require_status = XSplit_Status.XS_IDLE;
                            return;
                        }
                        if (Clipboard.ContainsText())
                        {
                            if (Clipboard.GetText().Contains(id))
                            {
                                require_status = XSplit_Status.XS_IDLE;
                            }
                            else
                            {
                                require_status = XSplit_Status.XS_NEED_RESTART;
                            }
                        }
                        break;

                    case XSplit_Status.XS_NEED_RESTART:
                        if (caption.Length == 0)
                        {
                            require_status = XSplit_Status.XS_NEED_START;
                            return;
                        }
                        if (caption.Equals("XSplit Broadcaster"))
                        {
                            require_status = XSplit_Status.XS_NEED_START;
                            return;
                        }
                        if (caption.Contains("Streaming Live - NicoVideo - "))
                        {
                            StopNicoLive();
                            //SHIFT_CONTROL_ALT_A();
                        }
                        break;

                    case XSplit_Status.XS_NEED_STOP:
                        if (caption.Length == 0)
                        {
                            require_status = XSplit_Status.XS_IDLE;
                            return;
                        }
                        if (caption.Equals("XSplit Broadcaster"))
                        {
                            require_status = XSplit_Status.XS_IDLE;
                            return;
                        }
                        if (caption.Contains("Streaming Live - NicoVideo - "))
                        {
                            StartNicoLive(mf);
                            //SHIFT_CONTROL_ALT_A();
                        }
                        break;
                }
            }
            else
            {
                require_status = XSplit_Status.XS_IDLE;
            }
        }


        //
        //  配信スタート
        //
        public static void Start()
        {
            require_status = XSplit_Status.XS_NEED_START;
        }

        //
        //  配信ストップ
        //
        public static void Stop()
        {
            require_status = XSplit_Status.XS_NEED_STOP;
        }

        //
        // ダイアログを消す
        //
        private static void CloseDialog(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_COMMAND, 2, 0);
        }

        //
        //  配信失敗のダイアログの検索＆消去
        //
        private static void SweepDialogs()
        {
            EnumWindows(new EnumWindowsDelegate(delegate(IntPtr hWnd, int lParam)
            {
                if (GetClassName(hWnd).Equals("#32770"))
                {
                    if (GetWindowText(hWnd).Contains("配信を初期化しています - NicoVideo - "))
                    {
                        CloseDialog(hWnd);
                        require_status = XSplit_Status.XS_NEED_STOP;
                    }
                }
                return 1;
            }), 0);
        }

        //
        // ウインドウのキャプションをウインドウハンドルから取得
        //
        private static string GetWindowText(IntPtr hWnd, int size=1024)
        {
            StringBuilder sb = new StringBuilder(size+1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        //
        // XSplitプロセスが生きているか否かのチェック
        //
        public static bool IsAlive
        {
            get
            {
                return Process.GetProcessesByName(XSplitName).Length > 0;
            }
        }

        //
        //  SHIFT+CONTROL+ALT+A
        //
        //private static void SHIFT_CONTROL_ALT_A()
        //{
        //    INPUT[] input = new INPUT[6];

        //    input[0].type = INPUT_KEYBOARD;
        //    input[0].ki.wScan = 0x2a;
        //    input[0].ki.wVk = VK_SHIFT;
        //    input[0].ki.time = 0;
        //    input[0].ki.dwExtraInfo = GetMessageExtraInfo();
        //    input[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | 0;

        //    input[1].type = INPUT_KEYBOARD;
        //    input[1].ki.wScan = 0x1d;
        //    input[1].ki.wVk = VK_CONTROL;
        //    input[1].ki.time = 0;
        //    input[1].ki.dwExtraInfo = GetMessageExtraInfo();
        //    input[1].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | 0;

        //    //input[2].type = INPUT_KEYBOARD;
        //    //input[2].ki.wScan = 0x38;
        //    //input[2].ki.wVk = VK_MENU;
        //    //input[2].ki.time = 0;
        //    //input[2].ki.dwExtraInfo = GetMessageExtraInfo();
        //    //input[2].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | 0;

        //    input[2].type = INPUT_KEYBOARD;
        //    input[2].ki.wScan = 0x1e;
        //    input[2].ki.wVk = VK_A;
        //    input[2].ki.time = 0;
        //    input[2].ki.dwExtraInfo = GetMessageExtraInfo();
        //    input[2].ki.dwFlags = 0;

        //    input[3].type = INPUT_KEYBOARD;
        //    input[3].ki.wScan = 0x1e;
        //    input[3].ki.wVk = VK_A;
        //    input[3].ki.time = 0;
        //    input[3].ki.dwExtraInfo = GetMessageExtraInfo();
        //    input[3].ki.dwFlags = KEYEVENTF_KEYUP;

        //    //input[5].type = INPUT_KEYBOARD;
        //    //input[5].ki.wScan = 0x38;
        //    //input[5].ki.wVk = VK_MENU;
        //    //input[5].ki.time = 0;
        //    //input[5].ki.dwExtraInfo = GetMessageExtraInfo();
        //    //input[5].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;

        //    input[4].type = INPUT_KEYBOARD;
        //    input[4].ki.wScan = 0x1d;
        //    input[4].ki.wVk = VK_CONTROL;
        //    input[4].ki.time = 0;
        //    input[4].ki.dwExtraInfo = GetMessageExtraInfo();
        //    input[4].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;

        //    input[5].type = INPUT_KEYBOARD;
        //    input[5].ki.wScan = 0x2a;
        //    input[5].ki.wVk = VK_SHIFT;
        //    input[5].ki.time = 0;
        //    input[5].ki.dwExtraInfo = GetMessageExtraInfo();
        //    input[5].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;

        //    //SetForegroundWindow(hXSplitWnd);
        //    //SetFocus(hXSplitWnd);
        //    SendInput(6, input, Marshal.SizeOf(input[0]));
        //}



        //
        //  CONTROL+NumPad
        //

        private static ushort[] ScanCode = { 0x4f, 0x50, 0x51, 0x4b, 0x4c, 0x4d, 0x47, 0x48, 0x49, 0x52, 0x0c, 0x27 };
        private static ushort[] NumPadVK_CODE = { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x60, 0xbd, 0xbb };

        public static void CONTROL_numPad(int num)
        {
            if (num < 1 || num > 12) return;

            INPUT[] input = new INPUT[4];

            input[0].type = INPUT_KEYBOARD;
            input[0].ki.wScan = 0x1d;
            input[0].ki.wVk = VK_CONTROL;
            input[0].ki.time = 0;
            input[0].ki.dwExtraInfo = GetMessageExtraInfo();
            input[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | 0;

            input[1].type = INPUT_KEYBOARD;
            input[1].ki.wScan = ScanCode[num - 1];
            input[1].ki.wVk = NumPadVK_CODE[num - 1];
            input[1].ki.time = 0;
            input[1].ki.dwExtraInfo = GetMessageExtraInfo();
            input[1].ki.dwFlags = 0;

            input[2].type = INPUT_KEYBOARD;
            input[2].ki.wScan = ScanCode[num - 1];
            input[2].ki.wVk = NumPadVK_CODE[num - 1];
            input[2].ki.time = 0;
            input[2].ki.dwExtraInfo = GetMessageExtraInfo();
            input[2].ki.dwFlags = KEYEVENTF_KEYUP;

            input[3].type = INPUT_KEYBOARD;
            input[3].ki.wScan = 0x1d;
            input[3].ki.wVk = VK_CONTROL;
            input[3].ki.time = 0;
            input[3].ki.dwExtraInfo = GetMessageExtraInfo();
            input[3].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;

            //SetForegroundWindow(hXSplitWnd);
            //SetFocus(hXSplitWnd);
            SendInput(4, input, Marshal.SizeOf(input[0]));
        }



        private static string GetClassName(IntPtr hWnd, int length = 256)
        {
            StringBuilder sb = new StringBuilder(length + 1);
            GetClassName(hWnd, sb, length);
            return sb.ToString();
        }

        //private string GetTextFromWindow(IntPtr hWnd, int length = 256)
        //{
        //    StringBuilder sb = new StringBuilder(length + 1);
        //    SendMessage(hWnd, WM_GETTEXT, length + 1, sb);
        //    return sb.ToString();
        //}

        //
        // "最新情報"ダイアログが存在したら、クローズ
        //
        public static void NewsDialogCloseIfExists()
        {
            IntPtr XSplitDialog = FindWindow(XSplitClassName, XSplitNewsTitle);
            if (XSplitDialog != null)
            {
                SendMessage(XSplitDialog, WM_CLOSE, 0, 0);
            }
        }

        //
        // XSplitのメインウインドウを探す
        //
        private static void FindXSplitMainWindow()
        {
            foreach (Process p in Process.GetProcessesByName(XSplitName))
            {
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    hXSplitWnd = p.MainWindowHandle;
                    return;
                }
            }
            hXSplitWnd=IntPtr.Zero;
        }

        //
        // XSplitのメインウインドウを探す
        //
        private static IntPtr FindNicoLive()
        {
            foreach (Process p in Process.GetProcessesByName("NicoLive"))
            {
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    return p.MainWindowHandle;
                }
            }
            return IntPtr.Zero;
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
                        if (Math.Abs(val1[pos1 + 0] - val2[pos2 + 0]) > 1 + val1[pos1 + 0] / 10 ||
                            Math.Abs(val1[pos1 + 1] - val2[pos2 + 1]) > 1 + val1[pos1 + 1] / 10 ||
                            Math.Abs(val1[pos1 + 2] - val2[pos2 + 2]) > 1 + val1[pos1 + 2] / 10)
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
