//-------------------------------------------------------------------------
// ITunesクラス
//
// Copyright (c) kazenif(http://ch.nicovideo.jp/community/co204623)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using iTunesLib;
using System.Runtime.InteropServices;

namespace NicoLive
{

    delegate int DeleteAddObject(object x);

    class ITunes
    {
        private static iTunesApp itunes = null;
        private static IITTrackCollection tracks = null;
        private static SendCommentDelegate dSendComment = null;
        private static string playlist_name = "Library";

        //-------------------------------------------------------------------------
        // SendCommentデリゲートの設定
        //-------------------------------------------------------------------------
        public static void setSendCommentDelegate(SendCommentDelegate i_say)
        {
            dSendComment = i_say;
        }

        //-------------------------------------------------------------------------
        // iTunes への接続
        //-------------------------------------------------------------------------
        public static void connect()
        {
            if (itunes == null)
            {
                itunes = new iTunesApp();
                if (itunes != null)
                {
                    itunes.OnPlayerPlayEvent +=
                        new _IiTunesEvents_OnPlayerPlayEventEventHandler(itunes_OnPlayerPlayEvent);
                }
            }
        }

        public static void addAllPlaylist(DeleteAddObject addObject)
        {
            // iTunesとの接続があるか？
            if (itunes == null) return;

            // iTunes の中から、"Library"をソースに選択
            IITSourceCollection sources = itunes.Sources;
            if (sources != null)
            {
                int idx;
                IITSource source=null;
                for (idx = 1; idx <= sources.Count; idx++)
                {
                    source = sources[idx];
                    if (source.Kind == ITSourceKind.ITSourceKindLibrary) break;
                }

                if (source != null)
                {
                    // プレイリストをすべて取得
                    IITPlaylistCollection lists = source.Playlists;

                    if (lists != null)
                    {
                        int i;
                        for (i = 1; i <= lists.Count; i++)
                        {
                            addObject(lists[i].Name);
                        }
                        Marshal.ReleaseComObject(lists);
                    }
                    Marshal.ReleaseComObject(source);
                }
                Marshal.ReleaseComObject(sources);
            }
        }

        //-------------------------------------------------------------------------
        // プレイリスト名 設定
        //-------------------------------------------------------------------------
        public static void setPlaylistName(string i_playlist_name)
        {
            playlist_name = i_playlist_name;

            // プレイリストの更新
            RefreshPlaylist();
        }

        //-------------------------------------------------------------------------
        // プレイリスト更新
        //-------------------------------------------------------------------------
        public static void RefreshPlaylist()
        {
            // iTunesとの接続があるか？
            if (itunes == null) return;

            // 既存のトラックリストをリリース
            if (tracks != null)
            {
                Marshal.ReleaseComObject(tracks);
                tracks = null;
            }

            // iTunes の中から、"Library"をソースに選択
            IITSourceCollection sources = itunes.Sources;
            if (sources != null)
            {
                int idx;
                IITSource source = null;
                for (idx = 1; idx <= sources.Count; idx++)
                {
                    source = sources[idx];
                    if (source.Kind == ITSourceKind.ITSourceKindLibrary) break;
                }

                if (source != null)
                {
                    // プレイリストをすべて取得
                    IITPlaylistCollection lists = source.Playlists;

                    if (lists != null)
                    {
                        IITPlaylist playlist;
                        // プレイリストを１つ選択
                        //                    IITPlaylist playlist = itunes.LibraryPlaylist;   // すべてのトラック
                        if (playlist_name==null || playlist_name.Equals(""))
                        {
                            playlist = itunes.LibraryPlaylist;   // すべてのトラック
                        }
                        else
                        {
                            playlist = lists.get_ItemByName(playlist_name);
                        }


                        if (playlist != null)
                        {
                            tracks = playlist.Tracks;
                            Marshal.ReleaseComObject(playlist);
                        }
                        Marshal.ReleaseComObject(lists);
                    }
                    Marshal.ReleaseComObject(source);
                }
                Marshal.ReleaseComObject(sources);
            }
        }

        //-------------------------------------------------------------------------
        // 曲を探して、演奏
        //-------------------------------------------------------------------------
        public static void Play(string title)
        {
            if (tracks != null)
            {
                int i;
                for (i = 1; i <= tracks.Count; i++)
                {
                    if (tracks[i].Name.Contains(title))
                    {
                        tracks[i].Play();
                        break;
                    }
                }

                // 演奏する曲を選ぶ
//                IITTrack track = tracks.get_ItemByName(title);

//                // title で示される曲名が存在すれば、演奏
//                if (track != null)
//                {
//                    track.Play();
//                    Marshal.ReleaseComObject(track);
//                }
            }
        }

        //-------------------------------------------------------------------------
        // Playイベントハンドラ
        //-------------------------------------------------------------------------
        private static void itunes_OnPlayerPlayEvent(object iTrack)
        {
            IITTrack track = itunes.CurrentTrack;
            if (track != null)
            {
                // SendComment のデリゲートが設定されていれば、出力
                if (dSendComment != null)
                {
                    dSendComment("♫" + track.Name+"("+track.Artist+")", true);
                }
                Marshal.ReleaseComObject(track);
            }
        }


        //-------------------------------------------------------------------------
        // iTunesとの接続を切る
        //-------------------------------------------------------------------------
        public static void Close()
        {
            if (itunes != null)
            {
                // イベントハンドラを取り除く
                itunes.OnPlayerPlayEvent -=
                  new _IiTunesEvents_OnPlayerPlayEventEventHandler(itunes_OnPlayerPlayEvent);

                // tracks が存在すれば、参照を解除
                if (tracks != null)
                {
                    Marshal.ReleaseComObject(tracks);
                    tracks = null;
                }

                // itunes の参照を解除
                Marshal.ReleaseComObject(itunes);
                itunes = null;
            }
        }
    }
}
