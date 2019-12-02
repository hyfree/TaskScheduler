﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TimedTask.Utility;

namespace TimedTask.View
{
    /// <summary>
    /// 锁屏窗口
    /// </summary>
    public partial class ScreenLock : Window
    {
        private HookHelper _hook = new HookHelper();
        private DispatcherTimer _timer;
        private DateTime _openTime = DateTime.Now;
        private TimeSpan _tsRun;
        private bool _isTest = false;
        private string _pointText = String.Empty;
        private int _timeKeep = 0;// 锁屏时间 单位秒

        #region 属性

        /// <summary>
        /// 锁屏提示文字
        /// </summary>
        public string PointText
        {
            get { return _pointText; }
            set { _pointText = value; }
        }
        /// <summary>
        /// 是否是测试
        /// </summary>
        public bool IsTest
        {
            get { return _isTest; }
            set { _isTest = value; }
        }
        #endregion
        public ScreenLock()
        {
            InitializeComponent();
            XamlHelper.Instance.SetBackground(this.mainBoder, TimedTask.Model.PM.LockBgImg);
            this.Loaded += ScreenLock_Loaded;
        }
        private void ScreenLock_Loaded(object sender, RoutedEventArgs e)
        {
            TimedTask.Model.PM.IsScreenLock = true;
            this._openTime = DateTime.Now;
            this._timeKeep = 20;

            if (!this.IsTest)
            {
                this._timeKeep = TimedTask.Model.PM.LockMinute * 60;
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            this._hook.HookStart();//安装钩子

            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(300);
            this._timer.Tick += Timer_Click;
            this._timer.Start();
        }

        private void Timer_Click(object sender, EventArgs e)
        {
            #region 解除锁屏

            this._tsRun = DateTime.Now.Subtract(_openTime);

            if (this._tsRun.Minutes * 60 + this._tsRun.Seconds > this._timeKeep)
            {
                try
                {
                    this._hook.HookStop();
                    this._timer.Stop();
                }
                catch (Exception ex)
                {
                    MSL.Tool.LogHelper.Instance.WriteLog("View ScreenLock Timer_Click\r\n" + ex.ToString());
                }
                finally
                {
                    TimedTask.Model.PM.IsScreenLock = false;
                    if (_timer != null)
                    {
                        _timer.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _timer.Stop();
                            this.Close();
                        }));
                    }
                }
            }

            this.txtPoint.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.PointText = String.IsNullOrEmpty(this.PointText) ? "主人：休息时刻到了！" : this.PointText;
                this.txtPoint.Text = this.PointText + "\r\n距解锁剩余 " + (this._timeKeep - (this._tsRun.Minutes * 60 + this._tsRun.Seconds)) + " 秒！";
            }));

            #endregion

            foreach (Process p in Process.GetProcesses())//遍历进程
            {
                try
                {
                    if (p.ProcessName.ToLower().Trim() == "taskmgr")
                    {
                        p.Kill();//关闭任务管理器
                        return;
                    }
                }
                catch
                {
                    return;
                }
            }
        }
    }
}
