using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Text;
using System.Collections;
using System.IO;

using TimedTask.BLL;
using TimedTask.ViewModel;
using TimedTask.Utility;

namespace TimedTask
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Module.TaskListModule _tlModule = new Module.TaskListModule();  //任务列表模块
        private Module.NoteListModule _nlModule = new Module.NoteListModule();  //记事本列表模块
        private Module.MainModule _mmModule = new Module.MainModule();          //主页模块
        public MainWindow()
        {
            InitializeComponent();
            XamlHelper.Instance.SetBackground(this.mainBoder, TimedTask.Model.PM.AppBgImg);
            #region 窗体事件

            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
            };
            //窗体最小化
            this.btnMin.Click += (s, e) =>
            {
                this.WindowState = System.Windows.WindowState.Minimized;
                this.Visibility = Visibility.Hidden;//最小化到托盘
            };
            //窗体关闭
            this.btnClose.Click += (s, e) =>
            {
                if (TimedTask.Model.PM.MinToTray)
                {
                    Minimized();
                    NotifyIconHelper.Instance().ShowBalloonTip("提示", "程序已经最小化到系统托盘！");
                    return;
                }
                BackgroundWindow.ExitApp();
            };
            #endregion
            #region 主菜单

            //首页
            this.btnMain.Click += (s, e) =>
            {
                if (this.brMain.Child != this._mmModule) this.brMain.Child = this._mmModule;
            };
            //设置
            this.btnSet.Click += (s, e) =>
            {
                View.Config config = new View.Config();
                config.ShowDialog();
            };
            //记事
            this.btnNote.Click += (s, e) =>
            {
                if (this.brMain.Child != this._nlModule) this.brMain.Child = this._nlModule;
            };
            //任务
            this.btnTask.Click += (s, e) =>
            {
                if (this.brMain.Child != this._tlModule) this.brMain.Child = this._tlModule;
            };

            #endregion
            #region 弹出资讯
            
            if (Model.PM.ShowNews)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
                View.NewsList newsList = new View.NewsList();
                newsList.Show();
            }
            #endregion
            if (this.brMain.Child != this._tlModule) this.brMain.Child = this._tlModule;
            NotifyIconHelper.NotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(TaskBarLeftDown_Click);
        }

        #region 托盘相关

        /// <summary>
        /// 任务栏单击击图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskBarLeftDown_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Show();
                if (this.WindowState == System.Windows.WindowState.Minimized)
                {
                    this.WindowState = System.Windows.WindowState.Normal;
                }
                this.Activate();
            }
        }
        #endregion

        #region 窗体

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void Minimized()
        {
            this.WindowState = System.Windows.WindowState.Minimized;
            this.Visibility = Visibility.Hidden;//最小化到托盘
        }

        #endregion

        #region 菜单事件
        //下拉菜单
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string arg = ((sender as MenuItem).CommandParameter).ToString();
            if (arg.Length == 0)
                return;

            switch (arg)
            {
                case "1"://启动宠物
                    Helper.Instance.Alert("开发中，敬请期待！");
                    break;
                case "2"://检查更新
                    Helper.Instance.Alert("开发中，敬请期待！");
                    break;
                case "3"://新闻资讯
                    View.NewsList news = new View.NewsList();
                    news.ShowDialog();
                    break;
                case "4"://关于
                    View.About about = new View.About();
                    about.Show();
                    break;
                case "5"://退出
                    Application.Current.Shutdown();
                    break;
                case "0-1"://网页图片下载
                    View.PageImageDown pageImage = new View.PageImageDown();
                    pageImage.Show();
                    break;
                default:
                    break;
            }
            if (this.brMain.Child != this._tlModule)
                this.brMain.Child = this._tlModule;
        }
        #endregion
    }
}
