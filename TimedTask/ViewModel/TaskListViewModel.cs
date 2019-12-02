using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TimedTask.BLL;
using TimedTask.Enums;
using TimedTask.Utility;

//----------------------------------------------------------------*/
// 版权所有：
// 文 件 名：TaskListViewModel.cs
// 功能描述：
// 创建标识：m.sh.lin0328@163.com 2014/6/22 14:33:16
// 修改描述：
//----------------------------------------------------------------*/
namespace TimedTask.ViewModel
{
    public class TaskListViewModel : ViewModelBase
    {
        private BLL.AutoTaskBLL _bllTask = new BLL.AutoTaskBLL();
        public TaskListViewModel()
        {
            if (!IsInDesignMode)
            {
                Load();
                this._autoTask = new TimedTask.Model.AutoTask();
                this.LoadCommand = new ViewModelCommand((Object parameter) => { Load(); });
                this.SaveCommand = new ViewModelCommand((Object parameter) => Save());
                this.AddCommand = new ViewModelCommand((Object parameter) =>
                {
                    View.TaskEdit edit = new View.TaskEdit();
                    edit.Closed += new EventHandler(Edit_Closed);//注册关闭事件 
                    edit.Show();
                });
                this.ContextMenuCommand = new ViewModelCommand((Object n) => { TaskListContextClick(n.ToString()); });
            }

            //this.ShowDispModeCommand = new ViewModelCommand((Object m) =>
            //{
            //    this.IsShowTitleList = !this.IsShowTitleList;
            //    this.TaskDisplayMode = this.IsShowTitleList ? TaskListDisplay.List : TaskListDisplay.Summary;
            //    this.ShowDispModeImagePath = this.IsShowTitleList ? "/Theme/Images/Button/frm_left_show.png" : "/Theme/Images/Button/frm_left_hide.png";
            //});
            this.TaskListItemCommand = new ViewModelCommand((Object m) => TaskListItemClick(m.ToString()));

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000;//1分钟执行一次
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Task);
            timer.Start();
        }


        private void Edit_Closed(object sender, EventArgs e)
        {
            Load();
        }
        private void Timer_Task(object sender, System.Timers.ElapsedEventArgs e)
        {
            Load();
        }

        #region 属性

        private TimedTask.Model.AutoTask _autoTask;
        private List<TimedTask.Model.AutoTask> _taskList;
        private string _taskMsg = "您可在右侧面板添加或修改提醒铃声...";
        //private string _ShowDispModeImagePath = "/Theme/Images/Button/frm_left_hide.png";
        //private TaskListDisplay _taskListDisplayMode = TaskListDisplay.List;
        /// <summary>
        /// 是否以标题列表显示任务列表
        /// </summary>
        //public bool IsShowTitleList { get; set; }
        /// <summary>
        /// 任务显示方式
        /// </summary>
        //public TaskListDisplay TaskDisplayMode
        //{
        //    get { return _taskListDisplayMode; }
        //    set
        //    {
        //        if (this._taskListDisplayMode == value) return;
        //        this._taskListDisplayMode = value;
        //        base.RaisePropertyChanged("TaskDisplayMode");
        //    }
        //}
        ///// <summary> 列表显示图片 </summary>
        //public string ShowDispModeImagePath
        //{
        //    get { return _ShowDispModeImagePath; }
        //    set
        //    {
        //        if (this._ShowDispModeImagePath == value) return;
        //        this._ShowDispModeImagePath = value;
        //        base.RaisePropertyChanged("ShowDispModeImagePath");
        //    }
        //}
        /// <summary> 任务信息 </summary>
        public string TaskMsg
        {
            get { return _taskMsg; }
            set
            {
                if (this._taskMsg == value) return;
                this._taskMsg = value;
                base.RaisePropertyChanged("TaskCount");
            }
        }
        /// <summary> 任务 </summary>
        public TimedTask.Model.AutoTask AutoTaskModel
        {
            get { return _autoTask; }
            set
            {
                if (this._autoTask == value) return;
                this._autoTask = value;
                base.RaisePropertyChanged("AutoTaskModel");
            }
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        public List<TimedTask.Model.AutoTask> TaskList
        {
            get { return _taskList; }
            set
            {
                _taskList = value;
                base.RaisePropertyChanged("TaskList");
            }
        }
        /// <summary>
        /// 声音列表
        /// </summary>
        public ObservableCollection<TimedTask.Model.Audio> AudioList { get; set; }
        #endregion

        #region 命令

        public ICommand _taskSelectedChangedCommand;
        public ICommand _audioSelectedChangedCommand;
        public ICommand TaskListItemCommand { get; set; }
        /// <summary> 收缩展开 </summary>
        public ICommand ShowDispModeCommand { get; set; }
        /// <summary> 声音列表选择 </summary>
        public ICommand AudioSelectedChangedCommand
        {
            get
            {
                if (_audioSelectedChangedCommand == null)
                {
                    _audioSelectedChangedCommand = new ViewModelCommand((Object parameter) =>
                    {
                        Helper.Instance.StopAudio();
                        if (AutoTaskModel == null)
                            return;
                        Helper.Instance.PalyAudio(AutoTaskModel.AudioPath, AutoTaskModel.AudioVolume);
                    });
                }
                return _audioSelectedChangedCommand;
            }
        }
        /// <summary> 列表选择 </summary>
        public ICommand TaskSelectedChangedCommand
        {
            get
            {
                if (_taskSelectedChangedCommand == null)
                {
                    _taskSelectedChangedCommand = new ViewModelCommand((Object mod) =>
                    {
                        this.AutoTaskModel = (TimedTask.Model.AutoTask)mod;
                    });
                }
                return _taskSelectedChangedCommand;
            }
        }
        /// <summary> 保存  </summary>
        public ICommand SaveCommand { get; set; }
        /// <summary> 添加  </summary>
        public ICommand AddCommand { get; set; }
        /// <summary> 加载  </summary>
        public ICommand LoadCommand { get; set; }
        /// <summary> 上下文菜单  </summary>
        public ICommand ContextMenuCommand { get; set; }
        #endregion

        #region 方法

        private void TaskListItemClick(string p)
        {
            TaskBLL.Instance.ItemClick(p, this.AutoTaskModel);
            Load();
        }
        private void TaskListContextClick(string type)
        {
            string proccessName = "";
            if (type == "1" || type == "2" || type == "3" || type == "4")
            {
                if (this.AutoTaskModel.ApplicationPath != null)
                    proccessName = this.AutoTaskModel.ApplicationPath.Substring(this.AutoTaskModel.ApplicationPath.LastIndexOf("\\") + 1).Replace(".exe", "");
            }
            try
            {
                switch (type)
                {
                    case "1"://运行
                        StartItem(proccessName);
                        break;
                    case "2"://停止实例 0：定时任务 结束进程 否则 停止播放声音
                        if (this.AutoTaskModel.TaskType == "0") Helper.Instance.EndApp(proccessName);
                        else Helper.Instance.StopAudio();
                        break;
                    case "3": //运行记录
                        View.TaskRunLog trl = new View.TaskRunLog();
                        trl.ID = this.AutoTaskModel.Id;
                        trl.Show();
                        break;
                }
            }
            catch (Exception ex)
            {
                MSL.Tool.LogHelper.Instance.WriteLog("MainWindow cmClick 运行\r\n" + ex.ToString());
                Helper.Instance.AlertError("操作失败！");
                //MessageBox.Show(, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //运行选中项
        private void StartItem(string proccessName)
        {
            if (this.AutoTaskModel == null)
                return;

            if (this.AutoTaskModel.TaskType != null && this.AutoTaskModel.TaskType != "0")
            {
                BLL.TaskBLL.Instance.StartWarn(this.AutoTaskModel, true);
                return;
            }
            if (!File.Exists(this.AutoTaskModel.ApplicationPath))
            {
                Helper.Instance.AlertError("运行失败，程序没有找到！");
                return;
            }

            try
            {   //杀死进程
                Helper.Instance.EndApp(proccessName);
                if (this.AutoTaskModel.StartParameters.Length > 0)
                {
                    System.Diagnostics.Process.Start(this.AutoTaskModel.ApplicationPath, this.AutoTaskModel.StartParameters);
                }
                else
                {
                    System.Diagnostics.Process.Start(this.AutoTaskModel.ApplicationPath);//可调用bat文件
                }
            }
            catch (Exception ex)
            {
                Helper.Instance.AlertError("操作失败！");
                MSL.Tool.LogHelper.Instance.WriteLog("MainWindow DropList 删除选中项\r\n" + ex.ToString());
            }
        }
        private void Save()
        {
            if (this.AutoTaskModel == null)
            {
                Helper.Instance.AlertWarning("没有任何选中项！");
                return;
            }
            try
            {
                _bllTask.Update(this.AutoTaskModel, " Id=" + this.AutoTaskModel.Id);
                Helper.Instance.AlertSuccess("操作成功！");
            }
            catch (Exception ex)
            {
                MSL.Tool.LogHelper.Instance.WriteLog("NoteListModule btnOK_Click\r\n" + ex.ToString());
                Helper.Instance.AlertError("系统异常，操作失败！");
            }
        }
        private void Load()
        {
            //this.IsShowTitleList = true;
            this.TaskList = TimedTask.BLL.TaskBLL.Instance.GetTaskList(false);
            this.AudioList = new ObservableCollection<TimedTask.Model.Audio>();
            if (this.TaskList == null)
                return;

            if (this.TaskList.Count > 0)
            {
                this._taskMsg = "共有 " + this.TaskList.Count + " 条记录，您可在右侧面板添加或修改提醒铃声...";
            }
            if (this.AudioList.Count == 0)
            {
                foreach (DictionaryEntry de in TimedTask.Model.PM.AudioHt)
                {
                    this.AudioList.Add(new TimedTask.Model.Audio { Name = de.Key.ToString(), Path = de.Key.ToString() });
                }
            }
        }
        #endregion
    }
}
