//-------------------------------------------------------------------------
// 外部コメントウィンド
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

//-------------------------------------------------------------------------
// クラス実装
//-------------------------------------------------------------------------
namespace NicoLive
{
    public partial class CommentForm : Form
    {
        #region 変数
        private Form1 mOwner = null;
        #endregion

        #region 列挙列
        // コメントカラム
        private enum CommentColumn
        {
            COLUMN_NUMBER = 0,			// 番号
            COLUMN_ID,					// ＩＤ
            COLUMN_HANDLE,				// コテハン
            COLUMN_COMMENT,				// コメント
        }
        #endregion

        //-------------------------------------------------------------------------
        // 来場者数
        //-------------------------------------------------------------------------
        public string TotalCnt
        {
            set { mTotalCnt.Text = value; }
        }
        //-------------------------------------------------------------------------
        // アクティブ数
        //-------------------------------------------------------------------------
        public string ActiveCnt
        {
            set { mActiveCnt.Text = value; }
        }

        //-------------------------------------------------------------------------
        // トータル数
        //-------------------------------------------------------------------------
        public string UniqCnt
        {
            set { mUniqCnt.Text = value; }
        }

        //-------------------------------------------------------------------------
        // CPU
        //-------------------------------------------------------------------------
        public string CpuInfo
        {
            set { mCpuInfo.Text = value; }
        }
        //-------------------------------------------------------------------------
        // Battery
        //-------------------------------------------------------------------------
        public string Battery
        {
            set { mBattery.Text = value; }
        }
        //-------------------------------------------------------------------------
        // UpLink
        //-------------------------------------------------------------------------
        public string UpLink
        {
            set { mUpLink.Text = value; }
        }

        public Color UpLinkForColor
        {
            set { mUpLink.ForeColor = value; }
        }

        //-------------------------------------------------------------------------
        // 残り時間
        //-------------------------------------------------------------------------
        public string RestTime
        {
            set { mRestTime.Text = value; }
        }

        public Color RestTimeForeColor
        {
            set { mRestTime.ForeColor = value; }
        }

        //-------------------------------------------------------------------------
        // コンストラクタ
        //-------------------------------------------------------------------------
        public CommentForm(Form1 iOwner)
        {
            mOwner = iOwner;

            InitializeComponent();

            this.mCommentList.Columns[0].Width = Properties.Settings.Default.column_width_ext_0;
            this.mCommentList.Columns[1].Width = Properties.Settings.Default.column_width_ext_1;
            this.mCommentList.Columns[2].Width = Properties.Settings.Default.column_width_ext_2;
            this.mCommentList.Columns[3].Width = Properties.Settings.Default.column_width_ext_3;

            this.mCommentList.BackgroundColor = Properties.Settings.Default.back_color;
            this.mCommentList.RowsDefaultCellStyle.ForeColor = Properties.Settings.Default.text_color;

            // フォントを設定しておく
            ResetCommentFont();
        }
        //-------------------------------------------------------------------------
        // コメント追加
        //-------------------------------------------------------------------------
        public void AddComment( Comment iCmt)
        {
            // フォントを設定しておく
            if (this.mCommentList.Font.Size != Properties.Settings.Default.font_size)
                ResetCommentFont();

            Utils.AddComment(ref mCommentList, iCmt);
        }
        //-------------------------------------------------------------------------
        // フォント設定
        //-------------------------------------------------------------------------
        private void ResetCommentFont()
        {
            Utils.SetCommentFont(ref mCommentList);
        }
        //-------------------------------------------------------------------------
        // コメントクリア
        //-------------------------------------------------------------------------
        public void Clear()
        {
            this.mCommentList.Rows.Clear();
        }
        //-------------------------------------------------------------------------
        // コメントクリア
        //-------------------------------------------------------------------------
        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing||
                e.CloseReason == CloseReason.FormOwnerClosing)
            {
                e.Cancel = true;
                SaveProp();
                Hide();
            }
        }
        //-------------------------------------------------------------------------
        // ニックネーム変更
        //-------------------------------------------------------------------------
        public void SetNickname(string iID, string iName)
        {
            Utils.SetNickname(ref mCommentList, iID, iName);
        }

        #region イベントハンドラ
        //-------------------------------------------------------------------------
        // コメントのコピー
        //-------------------------------------------------------------------------
        private void CopyComment_Click(object sender, EventArgs e)
        {
            if (this.mCommentList.SelectedRows.Count > 0)
            {
                string cmt = this.mCommentList.SelectedRows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Value.ToString();
                Clipboard.SetDataObject(cmt, true);
            }
        }
        //-------------------------------------------------------------------------
        // コメントリストのコンテクストメニュー表示開始時
        //-------------------------------------------------------------------------
        private void mCmtCxtMenu_Opening(object sender, CancelEventArgs e)
        {
            this.mCopyComment.Enabled = (this.mCommentList.SelectedRows.Count > 0);
            this.mCopyID.Enabled = (this.mCommentList.SelectedRows.Count > 0);
            this.mRename.Enabled = (this.mCommentList.SelectedRows.Count > 0);
            //this.mNgID.Enabled = (this.mCommentList.SelectedRows.Count > 0);
            this.mNgID.Enabled = false;

            int r;
            if (this.mCommentList.SelectedRows.Count > 0 &&
                int.TryParse(mCommentList.SelectedRows[0].Cells[(int)CommentColumn.COLUMN_ID].Value.ToString(), out r))
            {
                this.mShowUserPage.Enabled = true;
            }
            else
            {
                this.mShowUserPage.Enabled = false;
            }
        }
        //-------------------------------------------------------------------------
        // IDのコピー
        //-------------------------------------------------------------------------
        private void CopyID_Click(object sender, EventArgs e)
        {
            if (this.mCommentList.SelectedRows.Count > 0)
            {
                string id = this.mCommentList.SelectedRows[0].Cells[(int)CommentColumn.COLUMN_ID].Value.ToString();
                Clipboard.SetDataObject(id, true);
            }
        }

        //-------------------------------------------------------------------------
        // URLを開く
        //-------------------------------------------------------------------------
        private void OpenURL_Click(object sender, EventArgs e)
        {
            if (this.mCommentList.SelectedRows.Count > 0)
            {
                string msg = this.mCommentList.SelectedRows[0].Cells[(int)CommentColumn.COLUMN_COMMENT].Value.ToString();

                if (msg.Length == 0) return;

                Utils.OpenURL(msg);
            }
        }

        //-------------------------------------------------------------------------
        // ユーザーページを開く
        //-------------------------------------------------------------------------
        private void ShowUserPage_Click(object sender, EventArgs e)
        {
            if (this.mCommentList.SelectedRows.Count > 0)
            {
                string id = this.mCommentList.SelectedRows[0].Cells[(int)CommentColumn.COLUMN_ID].Value.ToString();
                string uri = "http://www.nicovideo.jp/user/" + id;
                System.Diagnostics.Process.Start(uri);
            }
        }

        //-------------------------------------------------------------------------
        // コテハンリネーム
        //-------------------------------------------------------------------------
        private void Rename_Click(object sender, EventArgs e)
        {
            if (this.mCommentList.SelectedRows.Count > 0)
            {
                Rename rm = new Rename();
                rm.MyOwner = mOwner;
                rm.ID = this.mCommentList.SelectedRows[0].Cells[(int)CommentColumn.COLUMN_ID].Value.ToString();
                rm.Show();
            }
        }

        //-------------------------------------------------------------------------
        // NG ID に加える
        //-------------------------------------------------------------------------
        private void mNgID_Click(object sender, EventArgs e)
        {
            if (this.mCommentList.SelectedRows.Count > 0)
            {
                string id = this.mCommentList.SelectedRows[0].Cells[(int)CommentColumn.COLUMN_ID].Value.ToString();
                mOwner.Invoke((MethodInvoker)delegate()
                {
                    mOwner.addNgID(id);
                });
            }
        }

        #endregion

        //-------------------------------------------------------------------------
        // ニックネーム変更
        //-------------------------------------------------------------------------
        private void UITimer_Tick(object sender, EventArgs e)
        {
            // 色設定
            if (mCommentList.RowsDefaultCellStyle.BackColor != Properties.Settings.Default.back_color)
            {
                mCommentList.RowsDefaultCellStyle.BackColor = Properties.Settings.Default.back_color;
                mCommentList.BackgroundColor = Properties.Settings.Default.back_color;
            }

            if (mCommentList.RowsDefaultCellStyle.ForeColor != Properties.Settings.Default.text_color)
            {
                mCommentList.RowsDefaultCellStyle.ForeColor = Properties.Settings.Default.text_color;
            }
        }


        //-------------------------------------------------------------------------
        // ステータス表示設定
        //-------------------------------------------------------------------------
        public void UpdateStatusVisibility()
        {
            mTotalCnt.Visible = Properties.Settings.Default.TotalCnt_view;
            mActiveCnt.Visible = Properties.Settings.Default.ActiveCnt_view;
            mUniqCnt.Visible = Properties.Settings.Default.UniqCnt_view;
            mCpuInfo.Visible = Properties.Settings.Default.CpuInfo_view;
            mBattery.Visible = Properties.Settings.Default.Battery_view;
            mUpLink.Visible = Properties.Settings.Default.UpLink_view;
        }

        private void CommentForm_Load(object sender, EventArgs e)
        {
            // ウィンドステート復帰
            this.Width = Properties.Settings.Default.cw_size.Width;
            this.Height = Properties.Settings.Default.cw_size.Height;
            this.Top = Properties.Settings.Default.cw_pos.Y;
            this.Left = Properties.Settings.Default.cw_pos.X;
            this.WindowState = Properties.Settings.Default.cw_state;
        }

        private void SaveProp()
        {
            Properties.Settings.Default.column_width_ext_0 = this.mCommentList.Columns[0].Width;
            Properties.Settings.Default.column_width_ext_1 = this.mCommentList.Columns[1].Width;
            Properties.Settings.Default.column_width_ext_2 = this.mCommentList.Columns[2].Width;
            Properties.Settings.Default.column_width_ext_3 = this.mCommentList.Columns[3].Width;
  
            // ウィンドステート保存
            if (this.WindowState == FormWindowState.Normal)
            {
                Size size = new Size(this.Width, this.Height);
                Properties.Settings.Default.cw_size = size;
                Point pos = new Point(this.Left, this.Top);
                Properties.Settings.Default.cw_pos = pos;
            }
            Properties.Settings.Default.mw_state = this.WindowState;

            Properties.Settings.Default.Save();
        }

        private void CommentForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                // ウィンドウステート更新
                this.UpdateStatusVisibility();
            }
        }
    }
}
//-------------------------------------------------------------------------
// 外部コメントウィンド
//
// Copyright (c) 金時豆(http://ch.nicovideo.jp/community/co48276)
// $Id$
//-------------------------------------------------------------------------
