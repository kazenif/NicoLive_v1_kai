//-------------------------------------------------------------------------
// OSバージョン判定クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NicoLive
{
    class OSVersion
    {
        public string CheckVersion()
        {
            // GetVersionEx関数によりOSの情報を取得
            OSVERSIONINFOEX osInfo = new OSVERSIONINFOEX();
            osInfo.dwOSVersionInfoSize = OSVERSIONINFOEX_SIZE;
            int bVersionEx = GetVersionEx(ref osInfo);
            if (bVersionEx == 0)
            {
                osInfo.dwOSVersionInfoSize = OSVERSIONINFO_SIZE;
                GetVersionEx(ref osInfo);
            }

            string windowsName = "Unknown Windows";  // Windows名

            switch (osInfo.dwPlatformId)
            {
                case VER_PLATFORM_WIN32_WINDOWS:  // Windows 9x系
                    if (osInfo.dwMajorVersion == 4)
                    {
                        switch (osInfo.dwMinorVersion)
                        {
                            case 0:  // .NET Frameworkのサポートなし
                                windowsName = "Windows 95";
                                break;
                            case 10:
                                ;
                                if ((osInfo.szCSDVersion.Length <= 0) ||
                                   (osInfo.szCSDVersion.Replace(" ", "") != "A"))
                                {
                                    windowsName = "Windows 98";
                                }
                                else
                                {
                                    windowsName = "Windows 98 Second Edition";
                                }
                                break;
                            case 90:
                                windowsName = "Windows Me";
                                break;
                        }
                    }
                    break;

                case VER_PLATFORM_WIN32_NT:  // Windows NT系

                    if (osInfo.dwMajorVersion == 4)
                    {
                        // .NET Framework 2.0以降のサポートなし
                        windowsName = "Windows NT 4.0";
                        // ※判定処理を省略
                    }
                    else if (osInfo.dwMajorVersion == 5)
                    {
                        switch (osInfo.dwMinorVersion)
                        {
                            case 0:
                                windowsName = "Windows 2000";

                                if ((osInfo.wSuiteMask & VER_SUITE_DATACENTER)
                                      == VER_SUITE_DATACENTER)
                                {
                                    windowsName += " Datacenter Server";
                                }
                                else if ((osInfo.wSuiteMask & VER_SUITE_ENTERPRISE)
                                           == VER_SUITE_ENTERPRISE)
                                {
                                    windowsName += " Advanced Server";
                                }
                                else
                                {
                                    windowsName += " Server";
                                }
                                break;

                            case 1:
                                windowsName = "Windows XP";

                                if ((osInfo.wSuiteMask & VER_SUITE_PERSONAL)
                                      == VER_SUITE_PERSONAL)
                                {
                                    windowsName += " Home Edition";
                                }
                                else
                                {
                                    windowsName += " Professional";
                                }
                                break;

                            case 2:
                                windowsName = "Windows Server 2003";

                                if ((osInfo.wSuiteMask & VER_SUITE_DATACENTER)
                                      == VER_SUITE_DATACENTER)
                                {
                                    windowsName += " Datacenter Edition";
                                }
                                else if ((osInfo.wSuiteMask & VER_SUITE_ENTERPRISE)
                                           == VER_SUITE_ENTERPRISE)
                                {
                                    windowsName += " Enterprise Edition";
                                }
                                else if (osInfo.wSuiteMask == VER_SUITE_BLADE)
                                {
                                    windowsName += " Web Edition";
                                }
                                else
                                {
                                    windowsName += " Standard Edition";
                                }
                                break;
                        }
                    }
                    else if (osInfo.dwMajorVersion == 6)
                    {
                        switch (osInfo.dwMinorVersion)
                        {
                            case 0:
                                windowsName = "Windows Vista";
                                break;
                        }

                        // Vistaの場合にはGetProductInfo関数を使用して
                        // Edition情報を取得する
                        uint edition = PRODUCT_UNDEFINED;
                        if (GetProductInfo(
                          osInfo.dwMajorVersion,
                          osInfo.dwMinorVersion,
                          osInfo.wServicePackMajor,
                          osInfo.wServicePackMinor,
                          out edition))
                        {
                            switch (edition)
                            {
                                case PRODUCT_ENTERPRISE:
                                    windowsName += " Enterprise Edition";
                                    break;
                                case PRODUCT_ULTIMATE:
                                    windowsName += " Ultimate Edition";
                                    break;
                                case PRODUCT_BUSINESS:
                                    windowsName += " Business Edition";
                                    break;
                                case PRODUCT_HOME_PREMIUM:
                                    windowsName += " Home Premium Edition";
                                    break;
                                case PRODUCT_HOME_BASIC:
                                    windowsName += " Home Basic Edition";
                                    break;
                                default:
                                    windowsName += " Unknown Edition";
                                    break;
                            }
                        }
                    }
                    break;
            }

            string strPlatform = "Unknown Windows";
            if (osInfo.dwPlatformId == VER_PLATFORM_WIN32_WINDOWS)
            {
                strPlatform = "Win32Windows";
            }
            else if (osInfo.dwPlatformId == VER_PLATFORM_WIN32_NT)
            {
                strPlatform = "Win32NT";
            }

            // システム情報を出力
            Console.WriteLine(
              "{0} (Platform {1} Version {2}.{3} Build {4}) {5}",
              windowsName,
              strPlatform,
              osInfo.dwMajorVersion,
              osInfo.dwMinorVersion,
              osInfo.dwBuildNumber,
              osInfo.szCSDVersion);

            return windowsName;
        }

        // GetVersionEx関数を使うための定義
        [StructLayout(LayoutKind.Sequential)]
        public struct OSVERSIONINFOEX
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public short wServicePackMajor;
            public short wServicePackMinor;
            public short wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }
        [DllImport("kernel32.dll", EntryPoint = "GetVersionExA")]
        public static extern int GetVersionEx(ref OSVERSIONINFOEX o);

        // OSVERSIONINFOEX構造体サイズ
        public const short OSVERSIONINFOEX_SIZE = 156;
        public const short OSVERSIONINFO_SIZE = 148;
        // dwPlatformId定義値
        public const byte VER_PLATFORM_WIN32s = 0;
        public const byte VER_PLATFORM_WIN32_WINDOWS = 1;
        public const byte VER_PLATFORM_WIN32_NT = 2;
        // wSuiteMask定義値
        public const short VER_SUITE_PERSONAL = 0x0200;  // Windows XP Home Edition
        public const short VER_SUITE_DATACENTER = 0x0080;  // Windows 2000 Datacenter Server, or Windows Server 2003, Datacenter Edition
        public const short VER_SUITE_ENTERPRISE = 0x0002;  // Windows NT 4.0 Enterprise Edition or Windows 2000 Advanced Server, or Windows Server 2003, Enterprise Edition
        public const short VER_SUITE_BLADE = 0x0400;     // Windows Server 2003, Web Edition


        [DllImport("Kernel32.dll")]
        public static extern bool GetProductInfo(
          int dwOSMajorVersion,
          int dwOSMinorVersion,
          int dwSpMajorVersion,
          int dwSpMinorVersion,
          out uint pdwReturnedProductType);

        // pdwReturnedProductType定義値
        public const uint PRODUCT_BUSINESS = 0x00000006;  // Business Edition
        public const uint PRODUCT_ENTERPRISE = 0x00000004; // Enterprise Edition
        public const uint PRODUCT_HOME_BASIC = 0x00000002; // Home Basic Edition
        public const uint PRODUCT_HOME_PREMIUM = 0x00000003;   // Home Premium Edition
        public const uint PRODUCT_ULTIMATE = 0x00000001;   // Ultimate Edition
        public const uint PRODUCT_UNDEFINED = 0x00000000;  // An unknown product
    }
}
//-------------------------------------------------------------------------
// OSバージョン判定クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------