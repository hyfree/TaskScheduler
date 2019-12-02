using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using TimedTask.Model;
using System.Windows;
using TimedTask.Utility;
using System.Diagnostics;

namespace TimedTask.BLL
{
    public class TaskBLL
    {
        private static TaskBLL _instance;
        private static object _lock = new object();//使用static object作为互斥资源
        private static readonly object _obj = new object();
        private BLL.AutoTaskBLL _bllTask = new BLL.AutoTaskBLL();
        private BLL.SysLogBLL _bllLog = new BLL.SysLogBLL();

        #region 单例
        /// <summary>
        /// 
        /// </summary>
        private TaskBLL() { }

        /// <summary>
        /// 返回唯一实例
        /// </summary>
        public static TaskBLL Instance
        {
            get
            {
                if (_instance == null)
                {
                    //lock (_obj)
                    //{
                    //if (_instance == null)
                    //{
                    _instance = new TaskBLL();
                    //}
                    //}
                }
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// 抓取百度新闻内容
        /// </summary>
        public void SpiderNewsToHtml()
        {
            if (String.IsNullOrEmpty(Model.PM.NewsUrl) ||
                Model.PM.NewsTag == null ||
                Model.PM.NewsTag.Count == 0)
                return;

            string path = Model.PM.StartPath + "\\News\\news.htm";
            if (File.Exists(path))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(path);
                if (fi.CreationTime < DateTime.Now.AddHours(-2))//两小时前生成 删除
                    MSL.Tool.IOHelper.Instance.DeleteFile(path);
            }
            if (File.Exists(path))
                return;

            string html = Utility.HtmlHelper.Instance.GetHtml(Model.PM.NewsUrl);
            Utility.HtmlHelper.Instance.FixUrl(Model.PM.NewsUrl, html);
            #region #

            //string src = "http://news.baidu.com/";//百度新闻首页
            //Hashtable ht = new Hashtable();
            //ht.Add("焦点", "<ul class=\"ulist focuslistnews\">⊙</ul>");
            //ht.Add("国内", "<div class=\"l-left-col col-mod\" alog-group=log-civil-left>⊙</ul>");
            #endregion

            StringBuilder sbTag = new StringBuilder();
            StringBuilder sbList = new StringBuilder();
            string templatePath = Model.PM.StartPath + "\\News\\news.html";
            string newsHtml = MSL.Tool.IOHelper.Instance.ReadFile(templatePath);
            int i = 1;
            string time = DateTime.Now.ToString("MM-dd");
            string no = String.Empty;
            foreach (DictionaryEntry m in Model.PM.NewsTag)
            {
                string content = MSL.Tool.CString.CutString(html, m.Value.ToString().Split('⊙')[0], m.Value.ToString().Split('⊙')[1], false);
                ArrayList al = Utility.HtmlHelper.Instance.GetLinks(content);

                if (al != null && al.Count > 0)
                {
                    sbTag.AppendFormat("<li id='one{0}' onmouseover=\"setTab('one',{1},{2})\" {4}><a href='#'>{3}</a></li>\r\n", i, i, Model.PM.NewsTag.Count, m.Key, (i == 1) ? "class='hover'" : "");
                    string title;
                    string href;
                    sbList.AppendFormat("<div id='con_one_{0}' class='hover'>\r\n<ul class='news_list news_list2'>\r\n", i);
                    int k = 1;
                    foreach (string[] hyperLink in al)
                    {
                        if (k > 14)
                            break;

                        no = "<h2>" + k + "</h2>";
                        if (k < 5)
                            no = "<h1>" + k + "</h1>";

                        title = hyperLink[0];
                        href = hyperLink[1];
                        sbList.AppendFormat("<li><span>{2}</span>{3}<a href='{0}' target='_blank'>{1}</a></li>\r\n", href, title, time, no);
                        //sbList.AppendFormat("<li><span>05-06</span>·<a href='{0}'>{1}</a></li>\r\n", href, title);

                        k++;
                    }
                    sbList.Append("</ul>\r\n</div>\r\n");
                }
                i++;
            }
            newsHtml = newsHtml.Replace("$[NEWS_TAG]", sbTag.ToString());
            newsHtml = newsHtml.Replace("$[NEWS_LIST]", sbList.ToString());

            MSL.Tool.IOHelper.Instance.DeleteFile(path);
            MSL.Tool.IOHelper.Instance.WriteFile(path, newsHtml);
        }

        /// <summary>
        /// 任务启动
        /// </summary>
        public void StartTask()
        {
            string proccessName = "";
            bool isTask = true;//是否是定时任务 启动.exe程序
            List<TimedTask.Model.AutoTask> list = null;
            list = _bllTask.GetList(" 1=1 AND Enable='1' ", null, " CreateDate DESC ");
            if (list == null || list.Count == 0)
                return;

            foreach (Model.AutoTask model in list)
            {
                isTask = (model.TaskType.Length > 0 && model.TaskType != "0") ? false : true;
                if (model.NextStartDate == null) model.NextStartDate = DateTime.Now;
                #region 路径不存在 或 不到时间

                if (isTask)
                {
                    if (model.ApplicationPath.Length == 0
                        || (model.RunType == RunType.Month.ToString() && model.Dayth != DateTime.Now.Day))
                    {
                        continue;
                    }
                    if (!File.Exists(model.ApplicationPath))
                    {
                        MSL.Tool.LogHelper.Instance.WriteLog("exe_not_exists Task StartTask 任务路径错误，名称：" + model.Title + ",路径：" + model.ApplicationPath + "\r\n");
                        model.Status = "路径不存在";
                        model.Enable = "2";//失效
                        this._bllTask.Update(model, " Id=" + model.Id);
                        continue;
                    }
                }
                #endregion
                #region 失效

                if (model.StopDate != null && DateTime.Now >= model.StopDate)
                {
                    model.Status = "任务过期";
                    model.Enable = "3";
                    this._bllTask.Update(model, " Id=" + model.Id);

                    continue;
                }
                if (model.Enable != "1")
                {
                    model.Status = "任务禁用";
                    model.Enable = "0";
                    _bllTask.Update(model, " Id=" + model.Id);
                    continue;
                }
                #endregion
                try
                {
                    #region 结束进程

                    if (isTask)
                    {
                        proccessName = model.ApplicationPath.Substring(model.ApplicationPath.LastIndexOf("\\") + 1).Replace(".exe", "");
                        Helper.Instance.EndApp(proccessName);
                    }
                    #endregion
                    if (DateTime.Now < model.NextStartDate)
                        continue;
                    #region 启动

                    bool result = true;
                    result = isTask ? StartApplication(model, proccessName) : StartWarn(model, false);
                    string nextSTime = TaskBLL.Instance.GetNextStartDateByType(Int64.Parse(model.RunType), model.Dayth, null, model.Interval);

                    model.NextStartDate = Convert.ToDateTime(nextSTime);
                    model.Status = result ? "正常" : "启动失败";
                    model.Enable = (model.RunType == "5") ? "0" : "1";//运行一次 的执行后设置不可用 
                    this._bllTask.Update(model, " Id=" + model.Id);

                    #endregion
                }
                catch (Exception ex)
                {
                    MSL.Tool.LogHelper.Instance.WriteLog("Task StartTask 后台运行出错" + ex.ToString() + "\r\n");
                }
            }
        }

        /// <summary>
        /// 开始提醒
        /// </summary>
        /// <param name="taskType">任务类别</param>
        /// <param name="title">标题</param>
        /// <param name="remark">任务说明</param>
        /// <param name="audioName">声音名称</param>
        /// <param name="isTest">是否测试，测试关机时只提醒不关机</param>
        /// <returns></returns>
        public bool StartWarn(Model.AutoTask model, bool isTest)
        {
            bool result = true;
            string shutDownMsg = String.Empty;//关机提示信息
            string command = "";

            TimedTask.Model.SysLog modLog = new TimedTask.Model.SysLog();
            modLog.TaskId = model.Id;
            modLog.TaskType = model.TaskType;
            modLog.RunType = model.RunType;
            modLog.IsRun = "1";
            modLog.Title = model.Title;
            modLog.CreateDate = DateTime.Now;

            #region 关机/显示器/锁屏

            if (model.TaskType == ((Int32)TaskType.Shutdown).ToString())//关机
            {
                shutDownMsg = "系统将于 120 秒后关闭，此操作不能撤销，请保存好您的工作！";
                command = "shutdown -s -t 120";
                //isShutdown = true;
            }
            if (model.TaskType == ((Int32)TaskType.TurnOffMonitor).ToString())//关闭显示器
            {
                Helper.Instance.CloseMonitor();
                this._bllLog.Add(modLog);
                return true;
            }
            if (model.TaskType == ((Int32)TaskType.TurnOnMonitor).ToString())//打开显示器
            {
                Helper.Instance.OpenMonitor();
                this._bllLog.Add(modLog);
                return true;
            }
            if (model.TaskType == ((Int32)TaskType.LockMonitor).ToString())//锁屏
            {
                if (!PM.IsScreenLock)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        View.ScreenLock lockScreen = new View.ScreenLock();
                        lockScreen.IsTest = isTest;
                        lockScreen.PointText = model.Remark.Contains("⊙") ? model.Remark.Split('⊙')[1] : model.Remark;
                        lockScreen.ShowDialog();
                    }));
                }
                this._bllLog.Add(modLog);
                return true;
            }
            #endregion

            #region 声音 POP提醒
            try
            {
                if (model.AudioEnable == null)
                    model.AudioEnable = "0";

                if (model.AudioEnable == "1" || model.AudioEnable.Equals("True", StringComparison.CurrentCultureIgnoreCase))
                {
                    //System.Threading.Tasks.Task.Factory.StartNew(() =>
                    //{
                    Helper.Instance.PalyAudio(model.AudioPath, model.AudioVolume);
                    //});
                }
                if (shutDownMsg.Length == 0)
                {
                    string subject = model.Remark;
                    string msg = "";
                    if (model.Remark.Contains(PM.SpiderChar))
                    {
                        subject = model.Remark.Split(PM.SpiderChar)[0];
                        msg = model.Remark.Split(PM.SpiderChar)[1];
                    }
                    Helper.Instance.ShowPupUp(model.Title, subject, msg);
                }
            }
            catch (Exception ex)
            {
                MSL.Tool.LogHelper.Instance.WriteLog("Task StartWarn\r\n" + ex.ToString());
                result = false;
                modLog.IsRun = "0";
            }
            #endregion

            #region 关机

            //关机
            if (!isTest && command.Length > 0)
            {
                NotifyIconHelper.Instance().ShowBalloonTip("温馨提示", shutDownMsg);
                Helper.Instance.Run(command);
            }
            #endregion

            this._bllLog.Add(modLog);
            return result;
        }
        /// <summary>
        /// 启动程序
        /// </summary>
        /// <param name="model"></param>
        /// <param name="proccessName">进程名</param>
        /// <returns>是否启动成功</returns>
        public bool StartApplication(Model.AutoTask model, string proccessName)
        {
            //杀死
            Helper.Instance.EndApp(proccessName);
            //启动
            System.Threading.Thread.Sleep(2000);
            if (!File.Exists(model.ApplicationPath))//不存在
            {
                return false;
            }
            try
            {
                if (model.StartParameters.Length > 0)
                    Process.Start(model.ApplicationPath, model.StartParameters);
                else
                    Process.Start(model.ApplicationPath);
            }
            catch (Exception ex)
            {
                string msg = "程序启动错误，路径：" + model.ApplicationPath + (model.StartParameters.Length == 0 ? "" : ",参数为：" + model.StartParameters) + ex.ToString();
                MSL.Tool.LogHelper.Instance.WriteLog("Task StartApplication" + msg);
            }
            #region 启动日志

            TimedTask.Model.SysLog log = new TimedTask.Model.SysLog();
            log.TaskId = model.Id;
            log.Title = model.Title;
            log.IsRun = "0";
            log.RunType = model.RunType;
            log.TaskType = model.TaskType;
            log.CreateDate = DateTime.Now;

            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (proccessName == p.ProcessName)
                {
                    log.IsRun = "1";
                    break;
                }
            }
            #endregion
            return this._bllLog.Add(log) > 0 ? true : false;
        }
        /// <summary>
        /// 获取下次启动时间
        /// </summary>
        /// <param name="timeType">启动类型</param>
        /// <param name="dayth">每月第几日</param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public string GetNextStartDateByType(long timeType, long dayth, string weeks, long interval)
        {
            string result = "";
            /*
                Year = 0,
                Month = 1,
                Day = 2,
                Hour = 3,
                Minute = 4,
                Once = 5
                Week=6
                 */
            string dateFormat = "yyyy-MM-dd HH:mm:00";
            switch (timeType)
            {
                case 5://一次
                    result = DateTime.Now.ToString(dateFormat);
                    break;
                case 1://每月
                    if (DateTime.Now.Subtract(DateTime.Now).Minutes < 5)
                        result = Convert.ToDateTime(String.Format(DateTime.Now.AddMonths(1).ToString("yyyy-MM-{0} HH:mm:00"), dayth)).ToString(dateFormat); //DateTime.Now.AddMonths(1).ToString(dateFormat).ToString();
                    break;
                case 2:
                    result = (DateTime.Now.AddDays(1)).ToString(dateFormat);
                    break;
                case 3:
                    result = (DateTime.Now.AddHours(1)).ToString(dateFormat);
                    break;
                case 4:
                    result = (DateTime.Now.AddMinutes(interval)).ToString(dateFormat);
                    break;
                case 6:
                    result = GetNextStartDateByWeek(weeks, DateTime.Now);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获取 周启动 下次启动时间
        /// </summary>
        /// <param name="weekList">选中的星期 格式为：1|2|5|7</param>
        /// <param name="startTime">第一次启动时间</param>
        /// <returns></returns>
        public string GetNextStartDateByWeek(string weekList, DateTime? startTime)
        {
            string result = "";

            string dateFormat = "yyyy-MM-dd HH:mm:00";
            int currWeek = ((int)DateTime.Now.DayOfWeek);
            if (String.IsNullOrEmpty(weekList))
                return String.Empty;
            if (startTime == null)
                startTime = DateTime.Now;

            string[] weeks = weekList.Split('|');
            if (weeks.Contains(currWeek.ToString()))
            {
                if (startTime != null && startTime > DateTime.Now)
                    return Convert.ToDateTime(startTime).ToString(dateFormat);//当天启动
            }
            // >currWeek 的 第一个星期
            int first7Week = 0;
            foreach (var m in weeks)
            {
                if (Int32.Parse(m) > currWeek)
                {
                    first7Week = Int32.Parse(m);
                    break;
                }
            }
            //明天 currWeek<x< 7
            if (first7Week != 0)
                result = Convert.ToDateTime(startTime).AddDays(1).ToString(dateFormat);
            //明天 x<currWeek< 7
            else if (Int32.Parse(weeks.First()) < currWeek)
                result = Convert.ToDateTime(startTime).AddDays(7 - currWeek + Int32.Parse(weeks.First())).ToString(dateFormat);// 下周时间

            return result;
        }
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="isFirstLoad">是否是第一次加载</param>
        /// <returns></returns>
        public List<TimedTask.Model.AutoTask> GetTaskList(bool isFirstLoad)
        {
            BLL.AutoTaskBLL _bllTask = new BLL.AutoTaskBLL();
            List<TimedTask.Model.AutoTask> list = _bllTask.GetList(" 1=1 ", null, " CreateDate DESC");
            if (list == null || list.Count == 0)
                return null;

            if (isFirstLoad)
            {
                List<TimedTask.Model.AutoTask> listTmp = list.Where(m => m.TaskType == "5").ToList<TimedTask.Model.AutoTask>();//锁屏任务 下次启动时间从打开软件算起
                if (listTmp.Count > 0)
                {
                    foreach (TimedTask.Model.AutoTask m in listTmp)
                    {
                        m.NextStartDate = Convert.ToDateTime(TaskBLL.Instance.GetNextStartDateByType(Int32.Parse(m.RunType), m.Dayth, null, m.Interval));
                        _bllTask.Update(m, " Id=" + m.Id);
                    }
                }
                listTmp = list.Where(m => m.TaskType == "2").ToList<TimedTask.Model.AutoTask>();//关机任务 如果时间<当前 则修改下次启动时间 
                if (listTmp.Count > 0)
                {
                    foreach (TimedTask.Model.AutoTask m in listTmp)
                    {
                        if (m.NextStartDate == null)
                            m.NextStartDate = DateTime.Now;

                        if (m.NextStartDate.Value > DateTime.Now)
                            continue;

                        Int64 runType = Int64.Parse(m.RunType);
                        string nextSTime = "";
                        DateTime now = DateTime.Now;
                        if (runType == 5)
                            continue;

                        switch (Int64.Parse(m.RunType))
                        {
                            case 1://每月
                                nextSTime = now.AddMonths(1).ToString("yyyy-MM") + now.ToString("-dd HH:mm:00");
                                break;
                            case 2://每天
                                nextSTime = now.AddDays(1).ToString("yyyy-MM-dd") + now.ToString(" HH:mm:00");
                                break;
                            case 3://每小时
                                nextSTime = now.ToString("yyyy-MM-dd") + " " + now.AddHours(1).Hour + now.ToString(":mm:00");
                                break;
                            case 4://间隔分钟
                                nextSTime = now.AddMinutes(m.Interval).ToString("yyyy-MM-dd HH:mm:00");
                                break;
                            case 6://每周
                                nextSTime = now.ToString("yyyy-MM-dd") + m.NextStartDate.Value.ToString(" HH:mm:00");
                                nextSTime = GetNextStartDateByWeek(m.Weeks, Convert.ToDateTime(nextSTime));
                                break;
                        }

                        m.NextStartDate = Convert.ToDateTime(nextSTime);
                        _bllTask.Update(m, " Id=" + m.Id);
                    }
                }
            }
            return _bllTask.GetList(" 1=1 ", null, " CreateDate DESC"); ;
        }

        public void ItemClick(string type, Model.AutoTask mod)
        {
            // 1:查看 2：删除 3：禁用 
            if (type == "1")
            {
                View.TaskEdit vTask = new View.TaskEdit();
                vTask.ID = mod.Id;
                vTask.ShowDialog();
            }
            else if (type == "2")
            {
                Helper.Instance.AlertConfirm(null, "确定删除？", () =>
                {
                    try
                    {
                        _bllTask.Delete(" Id=" + mod.Id);
                        Helper.Instance.AlertSuccess("操作成功！");
                    }
                    catch (Exception ex)
                    {
                        Helper.Instance.AlertError("操作失败！");
                        MSL.Tool.LogHelper.Instance.WriteLog("MainWindow DropList 删除选中项\r\n" + ex.ToString());
                    }
                });
            }
            else if (type == "3")
            {
                mod.Enable = (mod.Enable == "0") ? "1" : "0";
                _bllTask.Update(mod, " Id=" + mod.Id);
                Helper.Instance.AlertSuccess("操作成功！");
            }
        }
    }
}
