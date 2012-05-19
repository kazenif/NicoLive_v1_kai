using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Diagnostics;

namespace NicoLive
{
    class ImaDoko
    {
        private const double MINIMAM_INTERVAL = 20;
        private static SendCommentDelegate dSendComment = null;
        private static Object thisLock = new Object();
        private static DateTime next_enter_time = DateTime.Now;

        private static double mNowSpeed;
        private static string mNowPlace;

        private static double mLastSpeed;
        private static string mLastPlace;

        public static double Speed
        {
            get { return mNowSpeed; }
        }

        public static string Place
        {
            get { return mNowPlace; }
        }

        const string AddressURI2 = "http://geocode.didit.jp/reverse/";
        const string GetPosURI = "http://imakoko-gps.appspot.com/api/latest";
        static Regex Reg_Lon_Lat = new Regex("\"lon\":\"(\\d+\\.\\d+)\".*?\"lat\":\"(\\d+\\.\\d+)\"");
        static Regex Reg_Speed = new Regex("\"velocity\":\"(\\d+\\.\\d+)\"");

        public static bool IsCheckOnGoing = false;

        //-------------------------------------------------------------------------
        // SendCommentデリゲートの設定
        //-------------------------------------------------------------------------
        public static void setSendCommentDelegate(SendCommentDelegate i_say)
        {
            dSendComment = i_say;
        }

        //-------------------------------------------------------------------------
        // 逆ジオコーディング
        //-------------------------------------------------------------------------
        public static string ReverseGeocode(double lat, double lon)
        {
            try
            {
                // HTTP用WebRequestの作成
                string url = AddressURI2 + "?lat=" + lat.ToString("##0.000") + "&lon=" + lon.ToString("##0.000");
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = WebRequestMethods.Http.Get;
                req.Timeout = 6000;
                req.ContentType = "application/x-www-form-urlencoded";

                // レスポンスを取得
                WebResponse res = req.GetResponse();
                using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                {
                    using (XmlTextReader reader = new XmlTextReader(sr))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element &&
                                reader.Name == "AddressShort")
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    string result = reader.Value;
                                    //Console.WriteLine(result);
                                    return result;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return "";
        }

        //-------------------------------------------------------------------------
        // 現在の速度を取得し、出力
        //-------------------------------------------------------------------------
        public static void UpdateSpeedAndPlace()
        {
            lock (thisLock)
            {
                // 現在地機能が使用可能な状態か？
                if ((!Properties.Settings.Default.imakoko_genzaichi && !Properties.Settings.Default.imakoko_speed) ||
                    Properties.Settings.Default.imakoko_user == "")
                {
                    return;
                }

                // 連続してリクエストを発行させない
                if (next_enter_time > DateTime.Now)
                {
                    return;
                }

                // 一度にリクエストが1回しか発行できない
                if (IsCheckOnGoing)
                {
                    return;
                }

                // チェック機能が稼動したことを記録
                IsCheckOnGoing = true;

                // 次回は、すくなくとも MINIMAM_INTERVAL 立たないと実行できない
                next_enter_time = DateTime.Now + TimeSpan.FromSeconds(MINIMAM_INTERVAL);
            }

            try
            {

                string User = Properties.Settings.Default.imakoko_user;
                string result = "";

                // HTTP用WebRequestの作成
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(GetPosURI);
                req.Method = WebRequestMethods.Http.Post;
                req.Timeout = 6000;
                req.ContentType = "application/x-www-form-urlencoded";
                //            req.UserAgent = version + " " + Properties.Settings.Default.User;
                //if (Properties.Settings.Default.ProxyServer.Length > 0)
                //{
                //    req.Proxy = new WebProxy(string.Format("http://{0}:{1}", Properties.Settings.Default.ProxyServer, Properties.Settings.Default.ProxyPort));
                //}

                // HTTPで送信するデータ
                string body = "user=" + HttpUtility.UrlEncode(User);

                // 送信データを書き込む
                req.ContentLength = body.Length;
                using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
                {
                    sw.Write(body);
                }

                // レスポンスを取得
                WebResponse res = req.GetResponse();
                using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                {
                    result = sr.ReadLine();
                    if (result == null)
                    {
                        //dSendComment("いまココ、接続し忘れたかも", true);
                        Debug.WriteLine("いまココ、接続し忘れたかも");
                        IsCheckOnGoing = false;
                        return;
                    }
                }

                mLastSpeed = mNowSpeed;
                mNowSpeed = -1;
                if (Reg_Speed.IsMatch(result))
                {
                    MatchCollection matchCol = Reg_Speed.Matches(result);
                    mNowSpeed = double.Parse(matchCol[0].Groups[1].Value);
                    //dSendComment("時速" + ((int)mNowSpeed).ToString() + "kmです", true);
                }

                if (Reg_Lon_Lat.IsMatch(result) && Properties.Settings.Default.imakoko_genzaichi)
                {
                    MatchCollection matchCol = Reg_Lon_Lat.Matches(result);
                    double lon = double.Parse(matchCol[0].Groups[1].Value);
                    double lat = double.Parse(matchCol[0].Groups[2].Value);

                    result = ReverseGeocode(lat, lon);
                    mLastPlace = mNowPlace;
                    mNowPlace = "不明";
                    if (result != "")
                    {
                        mLastPlace = mNowPlace;
                        // 大字を取り除く
                        mNowPlace = result.Replace("（大字）", "");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                IsCheckOnGoing = false;
            }
        }

    }
}
