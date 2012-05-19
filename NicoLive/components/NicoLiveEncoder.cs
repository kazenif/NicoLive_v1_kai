//-------------------------------------------------------------------------
// バージョンチェック用クラス
//
// Copyright (c) 風に吹かれて(http://com.nicovideo.jp/community/co204623)
// $Id$
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace NicoLive
{
    class NicoLiveEncoder
    {
        //
        // (1). Formを作成時に、インスタンスを作成
        //      NicoLiveEncoder mNLE = new NicoLiveEncoder();
        //
        // (2). Timerを用いて、上記インスタンスから、
        //      HandlingStatus() を定期的に呼び出す。
        //      mNLE.HandlingStatus();
        //
        // (3). 配信をスタートする時には、
        //      mNLE.Start()
        //
        // (4). 配信を停止するときには、
        //      mNLE.Stop();
        //
        // (5). XSplitが放送中かチェック
        //      mNLE.IsBroadCast でチェック可能。
        //
        // (6). XSplitプロセスが存在するかチェック
        //      mNLE.IsAlive でチェック可能
        //


        const string NicoLiveEncoderName = "Nicoliveenc";
        const string NicoLiveEncoderTitle = "Niconico Live Encoder";
        private const double waitSeconds = 15;


        const string NicoLiveEncoderDialogTitle = "Nicoliveenc";
        const string NicoLiveEncoderDialogClassName = "#32770";
        const string NicoStartBtnCaption = "スタート";

        const string NicoLiveEncoderNoLive = "番組が作成されていないか、終了しました。";
        private const int DialogOK_ID    = 0x2;
        private const int DialogMess_ID  = 0xffff;
        private const int DialogStart_ID = 0x405;

        private const int RETRY_START_MAX  = 2;
        private static int mReryStartCount = RETRY_START_MAX;

        private static DateTime waitUntil = DateTime.Now;

        //
        //  ステータスの定義
        //
        public enum NLE_Status { NLE_IDLE, NLE_NEED_STOP, NLE_NEED_START };
        private static NLE_Status require_status = NLE_Status.NLE_IDLE;


        public enum NLE_InnerStatus { NLE_IDLE, NLE_RUN, NLE_STOP, NLE_WAIT_FOR_STOP, NLE_WAIT_FOR_RUN };
        private static NLE_InnerStatus inner_status = NLE_InnerStatus.NLE_IDLE;
        
        private static IntPtr hNicoLiveEncoderWnd = IntPtr.Zero;
        private static IntPtr NicoLiveEncoderStartButtonWnd = IntPtr.Zero;
        private static IntPtr NicoDlgWnd = IntPtr.Zero;

        #region Win32 Functions

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string ClassName, string WindowName);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr PostMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public delegate int EnumWindowsDelegate(IntPtr Window, IntPtr Parameter);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private extern static IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int EnumChildWindows(IntPtr Parent, EnumWindowsDelegate EnumFunction, IntPtr Parameters);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        #endregion

        #region Win32 Constant

        private const int WM_GETTEXT = 0x000D;
        private const int WM_CLOSE   = 0x0010;
        private const int WM_COMMAND = 0x111;

        #endregion

        //
        // ウインドウのキャプションをウインドウハンドルから取得
        //
        private static string GetWindowText(IntPtr hWnd, int size = 1024)
        {
            StringBuilder sb = new StringBuilder(size + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        //
        // NicoLiveEncoderのメインウインドウを探す
        //
        private static void FindMainWindow()
        {
            foreach (Process p in Process.GetProcessesByName(NicoLiveEncoderName))
            {
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    hNicoLiveEncoderWnd = p.MainWindowHandle;
                    return;
                }
            }
            hNicoLiveEncoderWnd = IntPtr.Zero;
        }

        //
        // NicoLiveEncoderプロセスが生きているか否かのチェック
        //
        public static bool IsAlive
        {
            get
            {
                Process [] pro;
                pro = Process.GetProcessesByName("*.exe");
                return Process.GetProcessesByName(NicoLiveEncoderName).Length > 0;
            }
        }

        //
        // 「配信開始」ボタン押下
        //
        private static void PushStart()
        {
            // NicoLiveEncoder が立ち上がっていなければ、何もしない
            if (!IsAlive) return;

            // 配信開始ボタンが有効かチェック。無効であれば、探しに行く
            if (NicoLiveEncoderStartButtonWnd == IntPtr.Zero ||
                GetWindowText(NicoLiveEncoderStartButtonWnd).Contains(NicoStartBtnCaption))
            {
                // ニコライブのメインウインドウを探す
                NicoLiveEncoderStartButtonWnd = IntPtr.Zero;
                hNicoLiveEncoderWnd = FindWindow(null, NicoLiveEncoderTitle);

                // 子ウインドから、スタートボタンを探す
                EnumWindowsDelegate EnumDelegate = new EnumWindowsDelegate(FindStartButton);
                EnumChildWindows(hNicoLiveEncoderWnd, EnumDelegate, IntPtr.Zero);
            }

            // 「配信開始」ボタンが有効であれば、コマンド発行
            if (NicoLiveEncoderStartButtonWnd != IntPtr.Zero)
            {
                PostMessage(NicoDlgWnd, WM_COMMAND, DialogStart_ID, (int)NicoLiveEncoderStartButtonWnd);
            }
        }

        //
        // 放送用のスタートボタンを探す（内部関数）
        //
        private static int FindStartButton(IntPtr Window, IntPtr Params)
        {
            StringBuilder ClassName = new StringBuilder(256);

            if (GetClassName(Window, ClassName, ClassName.Capacity) != 0)
            {
                // ダイアログの取得
                if (string.Compare(ClassName.ToString(), NicoLiveEncoderDialogClassName) == 0)
                {
                    IntPtr hBtn = GetDlgItem(Window, DialogStart_ID);
                    if (hBtn != IntPtr.Zero && GetWindowText(hBtn).Contains(NicoStartBtnCaption))
                    {
                        NicoDlgWnd = Window;
                        NicoLiveEncoderStartButtonWnd = hBtn;
                        return 0;
                    }
                }
            }
            return 1;
        }

        //
        // エラーダイアログのチェック
        //
        private static bool checkErrorDialog() 
        {
            IntPtr hDialog =  FindWindow(NicoLiveEncoderDialogClassName, NicoLiveEncoderDialogTitle);
            // エラーダイアログが出ているか?
            if (hDialog != IntPtr.Zero)
            {
                // メッセージが書かれたエディットウインドウを取得
                IntPtr hMessWnd = GetDlgItem(hDialog, DialogMess_ID);
                // "番組が作成されていないか、終了しました。" のチェック
                if (hMessWnd!=IntPtr.Zero && GetWindowText(hMessWnd).Contains(NicoLiveEncoderNoLive))
                {
                    // ウインドウをクローズ
                    PostMessage(hDialog, WM_CLOSE, 0, 0);
                    return true;
                }
            }
            return false;
        }


        //
        // 放送がアクティブか否かのチェック
        //

        public static bool IsBroadCast
        {
            get
            {

                return Process.GetProcessesByName("VHMultiWriterExt2").Length > 0;
            }
        }


        //
        //  放送開始
        //
        public static void Start()
        {
            require_status = NLE_Status.NLE_NEED_START;
            mReryStartCount = RETRY_START_MAX;
        }

        //
        //  放送停止
        //
        public static void Stop()
        {
            require_status = NLE_Status.NLE_NEED_STOP;
        }

        //
        //  この関数を定期的に呼ぶ
        //
        public static void HandlingStatus(string id, Form mf = null)
        {

            if (inner_status == NLE_InnerStatus.NLE_STOP ||
                inner_status == NLE_InnerStatus.NLE_IDLE)
            {
                if (require_status == NLE_Status.NLE_NEED_START)
                {
                    // すでに起動していれば、起動モードに即移行
                    if (IsAlive && IsBroadCast)
                    {
                        inner_status = NLE_InnerStatus.NLE_RUN;
                        return;
                    }
                    else
                    {
                        PushStart();
                        inner_status = NLE_InnerStatus.NLE_WAIT_FOR_RUN;
                        waitUntil = DateTime.Now.AddSeconds(waitSeconds);
                        return;
                    }
                }
            }

            if (inner_status==NLE_InnerStatus.NLE_RUN) {
                //if (!IsAlive || !IsBroadcasting() || checkErrorDialog())
                if (!IsBroadCast)
                {
                    inner_status = NLE_InnerStatus.NLE_STOP;

                    // もし、通信状態からNLEをクローズした場合は、
                    // リトライは行わない
                    if (!IsAlive)
                    {
                        require_status = NLE_Status.NLE_IDLE;
                        return;
                    }
                }
                else
                {
                    if (require_status == NLE_Status.NLE_NEED_STOP)
                    {
                        PushStart();
                        inner_status = NLE_InnerStatus.NLE_WAIT_FOR_STOP;
                        waitUntil = DateTime.Now.AddSeconds(waitSeconds);
                        return;
                    }
                    else if (require_status == NLE_Status.NLE_NEED_START)
                    {
                        mReryStartCount = RETRY_START_MAX;
                    }
                }
            }

            if (inner_status==NLE_InnerStatus.NLE_WAIT_FOR_STOP) {
                if (!IsBroadCast)
                {
                    inner_status = NLE_InnerStatus.NLE_STOP;
                    return;
                }
                else if (waitUntil < DateTime.Now)
                {
                    inner_status = NLE_InnerStatus.NLE_RUN;
                    return;
                }
            }

            if (inner_status==NLE_InnerStatus.NLE_WAIT_FOR_RUN) {
                if (IsBroadCast)
                {
                    inner_status=NLE_InnerStatus.NLE_RUN;
                }
                else if (waitUntil < DateTime.Now)
                {
                    checkErrorDialog();
                    // 起動に失敗した場合、最大 RETRY_START_MAX回リトライします
                    inner_status = NLE_InnerStatus.NLE_STOP;
                    if (mReryStartCount > 0)
                    {
                        mReryStartCount--;
                    }
                    else
                    {
                        require_status = NLE_Status.NLE_IDLE;
                    }
                }
            }
        }
    }
}
