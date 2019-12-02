using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using TimedTask.Model;
using TimedTask.Utility;

namespace TimedTask.View
{
    /// <summary>
    /// TaskEdit.xaml 的交互逻辑
    /// </summary>
    public partial class TaskEdit : Window
    {
        private RunType _runType = 0;
        private string _audio = "";//声音
        private string _startTime = "";
        private string _stopTime = "";
        private BLL.AutoTaskBLL _bllTask = new BLL.AutoTaskBLL();
        //ObservableCollection实现了INotifyPropertyChanged和INotifyCollectionChanged接口 表示一个动态数据集合，在添加项、移除项或刷新整个列表时，此集合将提供通知
        public Int64 ID { get; set; }
        //窗体加载
        public TaskEdit()
        {
            InitializeComponent();

            //天数 1-31
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //DateTime.Now.d
            for (int i = 1; i < 32; i++)
            {
                dic.Add(i.ToString(), i.ToString());
            }
            XamlHelper.Instance.CboAdd(this.cboDay, dic);
            dic.Clear();
            //分钟 0-59
            for (int i = 0; i < 60; i++)
            {
                dic.Add(i.ToString(), i.ToString());
            }
            XamlHelper.Instance.CboAdd(this.cboMinute, dic);
            XamlHelper.Instance.CboAdd(this.cboStartMinute, dic);
            XamlHelper.Instance.CboAdd(this.cboStopMinute, dic);
            dic.Clear();
            //小时 0-23
            for (int i = 0; i < 24; i++)
            {
                dic.Add(i.ToString(), i.ToString());
            }
            XamlHelper.Instance.CboAdd(this.cboStartHour, dic);
            XamlHelper.Instance.CboAdd(this.cboStopHour, dic);
            dic.Clear();
            //月份 1-12
            for (int i = 1; i < 13; i++)
            {
                dic.Add(i.ToString(), i.ToString());
            }
            XamlHelper.Instance.CboAdd(this.cboMonth, dic);
            dic.Clear();
            this.cboAudio.Items.Add("无");
            if (TimedTask.Model.PM.AudioHt.Count > 0)
            {
                foreach (DictionaryEntry m in TimedTask.Model.PM.AudioHt)
                {
                    this.cboAudio.Items.Add(m.Key);
                }
            }
            string ttype = "";
            foreach (object s in Enum.GetValues(typeof(TaskType)))
            {
                ttype = ((int)s).ToString();
                if (TimedTask.Model.PM.HtTaskType.Count == 0)
                    break;

                object tmp = TimedTask.Model.PM.HtTaskType[ttype];
                if (tmp == null)
                    continue;
                this.cboTaskType.Items.Add(tmp);
            }

            this.cboMonth.SelectedIndex = 0;
            this.cboDay.SelectedIndex = 0;
            this.cboMinute.SelectedIndex = 0;
            this.cboTaskType.SelectedIndex = 0;
            this.cboAudio.SelectedIndex = 0;

            this.cboStartHour.SelectedIndex = DateTime.Now.Hour;
            this.cboStartMinute.SelectedIndex = DateTime.Now.AddMinutes(5).Minute;
            this.cboStopHour.SelectedIndex = DateTime.Now.Hour;
            this.cboStopMinute.SelectedIndex = DateTime.Now.AddMinutes(5).Minute;
        }
        //窗体初始化
        private void OnInit()
        {
            if (ID == 0)
            {
                this.btnOK.Content = "添加任务";
                this.rbtMinute.IsChecked = true;
                this.cboEnable.IsChecked = true;
                this.dp_StartDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
                this.dp_StopDate.Text = DateTime.Now.AddDays(10).ToString("yyyy/MM/dd");
                rbtItem_Click(null, null);

                return;
            }
            else
            {
                this.btnOK.Content = "保 存";
                this.cboMinute.SelectedIndex = 0;
            }

            TimedTask.Model.AutoTask model = _bllTask.GetEntity(" Id=" + ID);
            // 开始与停止时间
            this.cboStartHour.SelectedValue = model.StartDate.Value.Hour;
            this.cboStartMinute.SelectedValue = model.StartDate.Value.Minute;
            this.cboStopHour.SelectedValue = model.StopDate.Value.Hour;
            this.cboStopMinute.SelectedValue = model.StopDate.Value.Minute;

            if (model != null)
            {
                this.txtPath.Text = model.ApplicationPath;
                this.txtTitle.Text = model.Title;
                this.txtStartParameter.Text = model.StartParameters;
                this.cboEnable.IsChecked = model.Enable == "1" ? true : false;
                this.txtRemark.Text = model.Remark;
                this.cboTaskType.SelectedIndex =
                    (model.TaskType == "" || model.TaskType == TaskType.TimingTask.ToString())
                    ? 0
                    : Convert.ToInt32(model.TaskType);

                this.cboAudio.SelectedItem = model.AudioPath.Length == 0 ? "无" : model.AudioPath;

                this.cboMinute.SelectedIndex = 0;
                this.cboDay.SelectedIndex = 0;

                switch (model.RunType)
                {
                    case "1": this.rbtMonth.IsChecked = true;
                        this._runType = RunType.Month;
                        this.cboDay.SelectedValue = model.Dayth;
                        break;
                    case "2":
                        this.rbtDay.IsChecked = true;
                        this._runType = RunType.Day;
                        break;
                    case "3": this.rbtHour.IsChecked = true; this._runType = RunType.Hour; break;
                    case "4":
                        this.rbtMinute.IsChecked = true;
                        this._runType = RunType.Minute;
                        this.cboMinute.SelectedValue = model.Interval;
                        break;
                    case "5": this.rbtOnce.IsChecked = true; this._runType = RunType.Once; break;
                    case "6":
                        this.rbtWeek.IsChecked = true; this._runType = RunType.Week;
                        if (model.Weeks != null)
                        {
                            if (model.Weeks.Contains("1")) this.chkWeek1.IsChecked = true;
                            if (model.Weeks.Contains("2")) this.chkWeek2.IsChecked = true;
                            if (model.Weeks.Contains("3")) this.chkWeek3.IsChecked = true;
                            if (model.Weeks.Contains("4")) this.chkWeek4.IsChecked = true;
                            if (model.Weeks.Contains("5")) this.chkWeek5.IsChecked = true;
                            if (model.Weeks.Contains("6")) this.chkWeek6.IsChecked = true;
                            if (model.Weeks.Contains("7")) this.chkWeek7.IsChecked = true;
                        }
                        break;
                }
                try
                {
                    switch (this._runType)
                    {
                        case RunType.Once: this.rbtOnce.IsChecked = true; break;
                        case RunType.Month: this.rbtMonth.IsChecked = true; break;
                        case RunType.Day: this.rbtDay.IsChecked = true; break;
                        case RunType.Hour: this.rbtHour.IsChecked = true; break;
                        case RunType.Minute: this.rbtMinute.IsChecked = true; break;
                        case RunType.Week: this.rbtWeek.IsChecked = true; break;
                    }
                    rbtItem_Click(null, null);

                    this.dp_StartDate.Text = Convert.ToDateTime(model.StartDate).ToString("yyyy-MM-dd");
                    this.dp_StopDate.Text = Convert.ToDateTime(model.StopDate).ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    MSL.Tool.LogHelper.Instance.WriteLog("TaskEdit OnInit\r\n" + ex.ToString());
                }
            }
        }
        //窗体加载
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OnInit();
        }
        //窗体移动
        private void bg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        //窗体关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //浏览文件
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "可执行文件|*.exe";//全部|*.*||批处理|*.bat";
            if (ofd.ShowDialog() == true)
            {
                this.txtPath.Text = ofd.FileName;
                //this.txtRemark.Text = Helper.Instance.GetFileDetailInfo(ofd.FileName, 34);
            }
        }
        //保存
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string path = this.txtPath.Text.Trim();
            string title = this.txtTitle.Text.Trim();
            string remark = this.txtRemark.Text.Trim();
            string startParameter = this.txtStartParameter.Text.Trim();
            string startDate = "", stopDate = "", nextStartDate = "";//起始 停止 下次启动时间

            if (this.rbtWeek.IsChecked == true && GetWeeks().Length == 0)
            {
                Helper.Instance.AlertError("保存失败，没有星期被选中！");
                return;
            }
            AutoTask model = new AutoTask();
            model.Weeks = "";

            if (this.rbtMonth.IsChecked == true) this._runType = RunType.Month;
            if (this.rbtDay.IsChecked == true) this._runType = RunType.Day;
            if (this.rbtHour.IsChecked == true) this._runType = RunType.Hour;
            if (this.rbtMinute.IsChecked == true) this._runType = RunType.Minute;
            if (this.rbtOnce.IsChecked == true) this._runType = RunType.Once;
            if (this.rbtWeek.IsChecked == true)
            {
                this._runType = RunType.Week;
                model.Weeks = GetWeeks();
            }

            try
            {
                bool flag = ID == 0 ? true : false;//是否是新增

                if (this.cboTaskType.SelectedIndex == 0)
                {
                    if (path.Length == 0 || !path.EndsWith(".exe"))
                    {
                        Helper.Instance.AlertError("不是可执行文件或文件路径不能为空！");
                        return;
                    }
                }
                this._startTime = this.cboStartHour.SelectedValue + ":" + this.cboStartMinute.Text + ":00";
                this._stopTime = this.cboStopHour.SelectedValue + ":" + this.cboStopMinute.Text + ":00";

                startDate = this.dp_StartDate.Text + " " + this._startTime;
                stopDate = this.dp_StopDate.Text + " " + this._stopTime;
                nextStartDate = GetFirstStartDate();

                model.StartParameters = startParameter;
                model.ApplicationPath = path;
                model.Title = title;
                model.Enable = (bool)this.cboEnable.IsChecked ? "1" : "0";
                model.StartDate = Convert.ToDateTime(startDate);
                model.StopDate = Convert.ToDateTime(stopDate);
                model.Remark = remark;
                model.AudioPath = this._audio;
                model.TaskType = this.cboTaskType.SelectedIndex.ToString();
                model.Status = "";
                model.RunType = ((int)this._runType).ToString();
                model.Interval = (this.rbtMinute.IsChecked == true) ? Convert.ToInt32(this.cboMinute.SelectedValue.ToString()) : 0;
                model.Dayth = (this.rbtMonth.IsChecked == true) ? Convert.ToInt32(this.cboDay.SelectedValue.ToString()) : 0;
                model.NextStartDate = Convert.ToDateTime(nextStartDate);

                if (!flag)//修改
                {
                    this._bllTask.Update(model, " Id=" + ID);
                    Helper.Instance.AlertSuccess("保存设置成功！" + (DateTime.Now > model.StopDate ? " 警告：任务已过期！" : ""));
                }
                else//新增
                {
                    BLL.AutoTaskBLL bllTask = new BLL.AutoTaskBLL();
                    bllTask.Add(model);

                    Helper.Instance.AlertError("新增成功！" + (DateTime.Now > model.StopDate ? " 警告：任务已过期！" : ""));
                    btnReset_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Helper.Instance.AlertError("保存设置失败！");
                MSL.Tool.LogHelper.Instance.WriteLog("TaskEdit btnOK_Click\r\n" + ex.ToString());
            }
        }
        //重置
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.ID = 0;
            this.txtPath.Text = "";
            this.txtRemark.Text = "";
            this.txtStartParameter.Text = "";
            this.txtTitle.Text = "";
            this.cboTaskType.SelectedIndex = 0;
            this.cboAudio.SelectedIndex = 0;

            OnInit();
        }
        //获取首次启动时间
        private string GetFirstStartDate()
        {
            string result = "";
            DateTime nowTime = DateTime.Now;
            string startDate = DateTime.Now.ToString("yyyy-MM-dd");
            if (Convert.ToDateTime(this.dp_StartDate.Text + " " + this._startTime) < DateTime.Now)//如果开始时间<当前--->使用当前时间计算下次启动
                startDate = Convert.ToDateTime(this.dp_StartDate.Text).ToString("yyyy-MM-dd");

            switch (this._runType)
            {
                case RunType.Once://一次
                case RunType.Day:
                case RunType.Hour:
                case RunType.Minute:
                    result = startDate + " " + this._startTime;
                    break;
                case RunType.Month:
                    result = nowTime.Year + "-" + nowTime.Month + "-" + this.cboDay.SelectedValue + " " + this._startTime;
                    break;
                case RunType.Week:
                    return TimedTask.BLL.TaskBLL.Instance.GetNextStartDateByWeek(GetWeeks(), Convert.ToDateTime(startDate + " " + this._startTime));
            }
            result = Convert.ToDateTime(result).ToString("yyyy-MM-dd HH:mm:00");
            if (Convert.ToDateTime(result) < DateTime.Now)
            {
                string interval = (this.rbtMinute.IsChecked == true) ? this.cboMinute.SelectedValue.ToString() : "0";
                string dayth = (this.rbtMonth.IsChecked == true) ? this.cboDay.SelectedValue.ToString() : "0";
                result = TimedTask.BLL.TaskBLL.Instance.GetNextStartDateByType((int)this._runType, Convert.ToInt32(dayth), null, Convert.ToInt32(interval));
            }
            return result;
        }
        //选中的星期
        private string GetWeeks()
        {
            string weeks = String.Empty;
            if (this.chkWeek1.IsChecked == true) weeks += "1|";
            if (this.chkWeek2.IsChecked == true) weeks += "2|";
            if (this.chkWeek3.IsChecked == true) weeks += "3|";
            if (this.chkWeek4.IsChecked == true) weeks += "4|";
            if (this.chkWeek5.IsChecked == true) weeks += "5|";
            if (this.chkWeek6.IsChecked == true) weeks += "6|";
            if (this.chkWeek7.IsChecked == true) weeks += "7|";
            if (weeks.Length > 0)
                weeks = weeks.Remove(weeks.Length - 1);

            return weeks;
        }
        //运行周期
        private void rbtItem_Click(object sender, RoutedEventArgs e)
        {
            this.cboMonth.IsEnabled = true;
            this.cboDay.IsEnabled = true;
            this.dp_StartDate.IsEnabled = true;
            this.dp_StopDate.IsEnabled = true;
            this.cboMinute.IsEnabled = true;
            this.cboStartHour.IsEnabled = true;
            this.cboStartMinute.IsEnabled = true;
            this.cboStopHour.IsEnabled = true;
            this.cboStopMinute.IsEnabled = true;

            this.spWeek.Visibility = Visibility.Collapsed;
            this.spMode.Visibility = Visibility.Visible;

            if (this.rbtOnce.IsChecked == true)//一次
            {
                this.cboMonth.IsEnabled = false;
                this.cboDay.IsEnabled = false;
                this.dp_StopDate.IsEnabled = false;
                this.cboMinute.IsEnabled = false;
                this.cboStopHour.IsEnabled = false;
                this.cboStopMinute.IsEnabled = false;
            }
            else if (this.rbtMonth.IsChecked == true)//每月
            {
                this.cboMonth.IsEnabled = false;
                this.cboMinute.IsEnabled = false;
            }
            else if (this.rbtWeek.IsChecked == true)//每周
            {
                this.cboMonth.IsEnabled = false;
                this.cboDay.IsEnabled = false;
                this.cboMinute.IsEnabled = false;
                this.spWeek.Visibility = Visibility.Visible;
                this.spMode.Visibility = Visibility.Collapsed;
            }
            else if (this.rbtDay.IsChecked == true)//每天
            {
                this.cboMonth.IsEnabled = false;
                this.cboDay.IsEnabled = false;
                this.dp_StartDate.IsEnabled = false;
                this.cboMinute.IsEnabled = false;
            }
            else if (this.rbtHour.IsChecked == true)//每小时
            {
                this.cboMonth.IsEnabled = false;
                this.cboDay.IsEnabled = false;
                this.cboMinute.IsEnabled = false;
                this.dp_StartDate.IsEnabled = false;
            }
            else if (this.rbtMinute.IsChecked == true)//间隔分钟
            {
                this.cboMonth.IsEnabled = false;
                this.cboDay.IsEnabled = false;
            }
        }
        //任务类型
        private void cboTaskType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cboTaskType.SelectedIndex != 0)
            {
                this.txtPath.IsEnabled = false;
                this.txtStartParameter.IsEnabled = false;
                this.btnOpenFile.IsEnabled = false;
                if (this.txtTitle.Text.Length == 0)
                {
                    this.txtTitle.Text = this.cboTaskType.SelectedValue.ToString();
                    this.txtRemark.Text = "开启" + this.cboTaskType.SelectedValue.ToString();
                }
            }
            else//定时任务
            {
                this.txtPath.IsEnabled = true;
                this.txtStartParameter.IsEnabled = true;
                this.btnOpenFile.IsEnabled = true;
                if (this.txtTitle.Text.StartsWith("定时")) this.txtTitle.Text = "";
            }
        }
        //声音选项
        private void cboAudio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._audio = this.cboAudio.SelectedValue.ToString();
            Helper.Instance.StopAudio();
            Helper.Instance.PalyAudio(this._audio, 100);
        }
        //窗口关闭
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Helper.Instance.StopAudio();
        }
    }
}
