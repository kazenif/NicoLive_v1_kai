//-------------------------------------------------------------------------
// 雑多な関数管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;
using System;
using System.IO;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
	public class Utils
	{
        // コメントカラム
        private enum CommentColumn : int
        {
            COLUMN_NUMBER = 0,			// 番号
            COLUMN_ID,					// ＩＤ
            COLUMN_HANDLE,				// コテハン
            COLUMN_COMMENT,				// コメント
        }

        //-------------------------------------------------------------------------
        // 文字コード変換
        //-------------------------------------------------------------------------
        public static string ConvertEncoding(string _src
                                      , System.Text.Encoding _srcEncoding
                                      , System.Text.Encoding _destEncoding)
        {
            byte[] srcBytes = _srcEncoding.GetBytes(_src);
            return _destEncoding.GetString(srcBytes);
        }
        public static string ToBase64(string iStr)
        {
            byte[] byteData = System.Text.Encoding.UTF8.GetBytes(iStr);
            return System.Convert.ToBase64String(byteData);
        }
        public static string FromBase64(string iStr)
        {
            byte[] byteData = System.Convert.FromBase64String(iStr);
            return System.Text.Encoding.UTF8.GetString(byteData);
        }
        //-------------------------------------------------------------------------
        // テキスト内のURLを開く
        //-------------------------------------------------------------------------
        public static void OpenURL(string iText)
        {
            string text = Regex.Replace(
                iText,
               @"(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])",
               delegate(Match match)
               {
                   System.Diagnostics.Process.Start(match.ToString());
                   return match.ToString();
               });
        }

        //-------------------------------------------------------------------------
        // コメント追加
        //-------------------------------------------------------------------------
        public static void AddComment(ref DataGridView iView, Comment iCmt)
        {
            iView.Rows.Insert(0, iView.Rows);
            iView.Rows[0].Cells[(int)CommentColumn.COLUMN_NUMBER].Value  = iCmt.No;
            iView.Rows[0].Cells[(int)CommentColumn.COLUMN_ID].Value      = iCmt.Uid;
            iView.Rows[0].Cells[(int)CommentColumn.COLUMN_HANDLE].Value  = iCmt.Handle;
            iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Value = iCmt.Text;

            if (iCmt.Premium.Equals("3"))
            {
                iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Properties.Settings.Default.owner_color;
            }
            else if (iCmt.IsNG)
            {
                iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Properties.Settings.Default.ng_color;
            }
            else if (iCmt.Mail.Contains("docomo"))
            {
                iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Properties.Settings.Default.mobile_color;
            }

            // テロップの抜き出し
            if (iCmt.Text.StartsWith("/telop "))
            {
                // 色はパープル
                iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Color.Purple;

                //Regex deleteCommand = new Regex("^/telop (?:show|show0|perm) (.*)$");
                //Match m = deleteCommand.Match(iCmt.Text);
                //if (m.Success)
                //{
                //     iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Value =m.Groups[1].Value;
                //}
            }

            // change color for /press コマンド
            if (iCmt.Text.StartsWith("/press"))
            {
                Regex deleteCommand = new Regex("^/press show (white|red|pink|orange|green|cyan|blue|purple|black) ");
                Match m = deleteCommand.Match(iCmt.Text);
                if (m.Success)
                {
                    if (m.Groups[1].Value.Contains("red"))
                    {
                        iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Color.Red;
                    }
                    else if (m.Groups[1].Value.Contains("pink"))
                    {
                        iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Color.Pink;
                    }
                    else if (m.Groups[1].Value.Contains("orange"))
                    {
                        iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Color.Orange;
                    }
                    else if (m.Groups[1].Value.Contains("green"))
                    {
                        iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Color.Green;
                    }
                    else if (m.Groups[1].Value.Contains("cyan"))
                    {
                        iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Color.Cyan;
                    }
                    else if (m.Groups[1].Value.Contains("blue"))
                    {
                        iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Color.Blue;
                    }
                    else if (m.Groups[1].Value.Contains("purple"))
                    {
                        iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Color.Purple;
                    }
                }

                //deleteCommand = new Regex("^/press show (?:white |niconicowhite |red |pink |orange |green |cyan |blue |purple |black )?(.*)$");
                //m = deleteCommand.Match(iCmt.Text);
                //if (m.Success)
                //{
                //    iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Value = m.Groups[1].Value;
                //}
            }

            // /chukei の色制御
            if (iCmt.Text.StartsWith("/chukei"))
            {
                Regex deleteCommand = new Regex("^/chukei ([0-9a-f]{3})[0-9a-f]{2}");
                Match m = deleteCommand.Match(iCmt.Text);
                if (m.Success)
                {
                    int color = Convert.ToInt32(m.Groups[1].Value, 16);
                    int r = (color >> 8) * 17;
                    int g = ((color >> 4) & 15) * 17;
                    int b = (color & 15) * 17;
                    iView.Rows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Style.ForeColor = Color.FromArgb(r, g, b);
                }
            }

            int max_item = Properties.Settings.Default.comment_max;
            if (iView.Rows.Count > max_item)
            {
                iView.Rows.RemoveAt(max_item);
            }
        }

		//-------------------------------------------------------------------------
		// リスト中のニックネームを変更
		//-------------------------------------------------------------------------
        public static void SetNickname(ref DataGridView iView, string iID, string iName)
		{
			// 既存アイテムのIDをコテハンに書き換え
            int cnt = iView.Rows.Count;
            for (int i = 0; i < cnt; i++)
            {
                DataGridViewRow item = iView.Rows[i];
                if (item != null)
                {
                    string id = item.Cells[(int)CommentColumn.COLUMN_ID].Value.ToString();
                    if (id.Equals(iID))
                    {
                        item.Cells[(int)CommentColumn.COLUMN_HANDLE].Value = iName;
                    }
                }
            }
		}

		//-------------------------------------------------------------------------
		// コメント用フォントの設定
		//-------------------------------------------------------------------------
        public static void SetCommentFont(ref DataGridView iView)
		{
            iView.Font = new Font("MS P Gothic",
                                  Properties.Settings.Default.font_size,
                                  FontStyle.Bold);
		
		}

        //-------------------------------------------------------------------------
        // 順番待ちが残り２桁になったらTweet
        //-------------------------------------------------------------------------
        public static void TweetWait(int iWaitCnt)
        {
            if (!Properties.Settings.Default.tweet_wait) return;

            if (iWaitCnt <= 1)
                return;

            MessageSettings msgSet = MessageSettings.Instance;

            string msg = String.Format(msgSet.GetMessage("枠取り順番待ち、残り{0}人"), iWaitCnt);

            using (Twitter tw = new Twitter())
            {
                tw.Post(msg, "#nicolive");
            }
        }

        //-------------------------------------------------------------------------
        // ログ書き出し
        //-------------------------------------------------------------------------
        public static void WriteLog(string iTitle, string iStr)
        {
            using (StreamWriter writer = new StreamWriter("error_log.txt", true))
            {
                writer.WriteLine("【" + iTitle + "】");
                writer.WriteLine(iStr);
                writer.Flush();
                writer.Close();
            }
        }
        //-------------------------------------------------------------------------
        // LiveID抽出
        //-------------------------------------------------------------------------
        public static string ParseLiveID(string iText)
        {
            string tmp = iText;
            string live_id = "lv";

            int idx = tmp.LastIndexOf("lv");
            int len = tmp.Length;

            for (int i = idx + 2; i < len; i++)
            {
                if ('0' <= tmp[i] && tmp[i] <= '9')
                    live_id += tmp[i].ToString();
                else
                    break;
            }

            return live_id;
        }

        //-------------------------------------------------------------------------
        // 文字列の番号のインクリメント
        //-------------------------------------------------------------------------
        public static　string incString(string count)
        {
            string ret = (int.Parse(count.Replace("０", "0").Replace("１", "1").Replace("２", "2").Replace("３", "3").Replace("４", "4").Replace("５", "5").Replace("６", "6").Replace("７", "7").Replace("８", "8").Replace("９", "9")) + 1).ToString();
            if (count.Length > 0)
            {
                if ("０１２３４５６７８９０".Contains(count[0].ToString()))
                {
                    ret = ret.Replace("0", "０").Replace("1", "１").Replace("2", "２").Replace("3", "３").Replace("4", "４").Replace("5", "５").Replace("6", "６").Replace("7", "７").Replace("8", "８").Replace("9", "９");
                }
            }
            return ret;
        }

        //-------------------------------------------------------------------------
        // タイトル中の文字列のインクリメント
        //-------------------------------------------------------------------------
        public static string incTitle(string title)
        {
            Regex NumberedTitle = new Regex("^(.*?(?:＃|#|その[ |　]?|その[ |　]?\\(|その[ |　]?（|\\())([0-9０-９]+)([\\)|）]?)$");

            Match m = NumberedTitle.Match(title);
            if (m.Success)
            {
                return m.Groups[1].Value + incString(m.Groups[2].Value) + m.Groups[3].Value;
            }
            else
            {
                return title;
            }

        }

        // UNIXエポックを表すDateTimeオブジェクトを取得
        private static DateTime UNIX_EPOCH =
          new DateTime(1970, 1, 1, 0, 0, 0, 0);

        //-------------------------------------------------------------------------
        // UnixTimeを得る
        //-------------------------------------------------------------------------
        public static UInt32 GetUnixTime(DateTime targetTime)
        {
            // UTC時間に変換
            targetTime = targetTime.ToUniversalTime();

            // UNIXエポックからの経過時間を取得
            TimeSpan elapsedTime = targetTime - UNIX_EPOCH;

            // 経過秒数に変換
            return (UInt32)elapsedTime.TotalSeconds;
        }

        //-------------------------------------------------------------------------
        // 残り時間計算
        //-------------------------------------------------------------------------
        public static int CalcTime()
        {
            int sub;
            LiveInfo info = LiveInfo.Instance;

            if (info.EndTime != 0)
            {
                if (info.StartTime > info.Time)
                {
                    sub = (int)(info.Time - info.StartTime);
                }
                else
                {
                    sub = (int)(info.EndTime - info.Time);  // 残り時間
                    if (info.EndTime < info.Time)
                        sub = 0;
                }

                UInt32 passed_sec = (Utils.GetUnixTime(DateTime.Now) - info.UnixTime);
                if (info.StartTime > info.Time + passed_sec)
                {
                    sub = (int)(info.Time - info.StartTime + passed_sec);
                }
                else
                {
                    sub = (int)(info.EndTime - info.Time - passed_sec);  // 残り時間
                    if (info.EndTime < info.Time + passed_sec)
                        sub = 0;
                }
            }
            else
            {
                UInt32 passed_sec = (Utils.GetUnixTime(DateTime.Now) - info.UnixTime);
                sub = (int)(info.Time + passed_sec - info.StartTime);
            }
            return sub;
        }
	}
}

//-------------------------------------------------------------------------
// 雑多な関数管理クラス
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
//-------------------------------------------------------------------------
