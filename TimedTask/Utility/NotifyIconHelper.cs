using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;

//----------------------------------------------------------------*/
// 版权所有：
// 文 件 名：TimedTaskNotifyIcon.cs
// 功能描述：系统托盘类
// 创建标识：m.sh.lin0328@163.com 2014/9/2 20:53:59
// 修改描述：
//----------------------------------------------------------------*/
namespace TimedTask
{
    public class NotifyIconHelper
    {
        private static NotifyIconHelper instance = null;
        public static NotifyIcon NotifyIcon = new NotifyIcon();

        #region 单例

        private NotifyIconHelper()
        {
            SetupNotifyIcon();
        }
        public static NotifyIconHelper Instance()
        {
            if (instance == null)
            {
                instance = new NotifyIconHelper();
            }
            return instance;
        }
        #endregion

        #region 提示信息

        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        public void ShowBalloonTip(string title, string text)
        {
            if (String.IsNullOrEmpty(text))
                return;
            try
            {
                NotifyIcon.BalloonTipTitle = title;
                NotifyIcon.BalloonTipText = text;
                NotifyIcon.BalloonTipIcon = ToolTipIcon.None;

                NotifyIcon.ShowBalloonTip(5000);
            }
            catch (Exception ex)
            {
                MSL.Tool.LogHelper.Instance.WriteLog("ShowBalloonTip BalloonTipTitle:[" + title + "]\r\nBalloonTipText:[" + text + "]\r\n\r\n" + ex.ToString());
            }
        }
        #endregion

        #region 托盘菜单

        /// <summary>
        /// 托盘菜单
        /// </summary>
        private void SetupNotifyIcon()
        {
            NotifyIcon.Icon = TimedTask.Properties.Resources.App;
            NotifyIcon.ContextMenuStrip = new ContextMenuStrip();

            ToolStripSeparator separatorA = new ToolStripSeparator();//分隔线
            ToolStripMenuItem homePageMenuItem = new ToolStripMenuItem("访问主页");
            homePageMenuItem.Image = TimedTask.Properties.Resources.Home;// MotivateDesktop.Properties.Resources.Home;
            homePageMenuItem.Click += new EventHandler(HomePageMenuItem_Click);

            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("退出");
            exitMenuItem.Click += new EventHandler(ExitMenuItem_Click);

            NotifyIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { homePageMenuItem, separatorA, exitMenuItem });
            NotifyIcon.Visible = true;
        }
        #endregion

        public void HomePageMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Model.PM.HomePageUrl);
        }
        #region ##

        //void latestWallpaperMenuItem_Click(object sender, EventArgs e)
        //{
        //    BackgroundWindow.ShouldForceShowWallpaperPreviewWindow = true;

        //    ShowBalloonTip("正在检测壁纸", "稍后会有提示...");
        //    if (!WallpaperChecker.Instance().IsChecking)
        //    {
        //        System.Threading.Thread checkWallpaperThread = new System.Threading.Thread(new System.Threading.ThreadStart(
        //           delegate
        //           {
        //               WallpaperChecker.Instance().BeginCheckWallpaper();
        //           }
        //         ));
        //        checkWallpaperThread.Start();
        //    }
        //}
        //void settingMenuItem_Click(object sender, EventArgs e)
        //{
        //    SettingsWindow.Instance().Show();
        //    SettingsWindow.Instance().Activate();
        //}
        #endregion
        // 退出
        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Helper.Instance.AlertConfirm(null, "确定要退出托盘应用程序吗？", () =>
            {
                BackgroundWindow.ExitApp();
                NotifyIconHelper.Instance().Hide();
            });
        }
        // 隐藏
        public void Hide()
        {
            NotifyIcon.Visible = false;
        }
    }
}
