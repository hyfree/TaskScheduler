using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TimedTask.BLL;
using TimedTask.Utility;

namespace TimedTask
{
    /// <summary>
    /// 后台任务 定时任务执行
    /// </summary>
    public partial class BackgroundWindow : Window
    {
        private static System.Timers.Timer timerTask = null;//
        // 程序退出
        public static void ExitApp()
        {
            App.Current.Dispatcher.Invoke(new Action(
                delegate
                {
                    if (timerTask != null)
                        timerTask.Dispose();

                    NotifyIconHelper.Instance().Hide();
                    App.Current.Shutdown();
                }
                ), null);
        }

        public BackgroundWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            NotifyIconHelper.Instance();
            NotifyIconHelper.Instance().ShowBalloonTip("提示", "程序运行成功！");

            BLL.SysLogBLL taskLog = new BLL.SysLogBLL();
            MSL.Tool.IOHelper.Instance.DeleteFiles(MSL.Tool.LogHelper.LogPath, new string[] { ".txt", ".log" }, 4);//删除4天前日志文件
            taskLog.DeleteHistory();
            MSL.Tool.IOHelper.Instance.CreateFolder(MSL.Tool.LogHelper.LogPath);

            TaskBLL.Instance.GetTaskList(true);

            StartTask();
            //StartSpiderTask();
        }

        #region 后台任务

        //启动任务
        private void StartTask()
        {
            timerTask = new System.Timers.Timer();
            timerTask.Interval = 60000;//1分钟执行一次
            timerTask.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Task);
            timerTask.Start();
        }
        //
        private void Timer_Task(object sender, System.Timers.ElapsedEventArgs e)
        {
            BLL.TaskBLL.Instance.StartTask();
        }
        #endregion
    }
}
