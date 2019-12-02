using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TimedTask.BLL;

namespace TimedTask.Module
{
    /// <summary>
    /// TaskListModule.xaml 的交互逻辑
    /// </summary>
    public partial class TaskListModule : UserControl
    {
        public TaskListModule()
        {
            InitializeComponent();
            this.IsShowTitleList = true;
            this.btnImgDispMode.PreviewMouseLeftButtonDown += (s, e) =>
            {
                this.IsShowTitleList = !this.IsShowTitleList;
                this.btnImgDispMode.Source = new BitmapImage(
                    new Uri(this.IsShowTitleList ? "/Theme/Images/Button/frm_left_hide.png" : "/Theme/Images/Button/frm_left_show.png", UriKind.Relative)
                    );
                string key = this.IsShowTitleList ? "TaskListTitleTemplate" : "TaskListSummaryTemplate";
                this.lstMain.ItemTemplate = (DataTemplate)this.FindResource(key);
            };
        }
        protected void ShowMenuItem_Click(object sender, EventArgs e)
        {
            TimedTask.Model.AutoTask task = (sender as Button).DataContext as TimedTask.Model.AutoTask;
            TaskBLL.Instance.ItemClick("1", task);
            ((ViewModel.TaskListViewModel)base.DataContext).LoadCommand.Execute(null);
        }
        protected void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            TimedTask.Model.AutoTask task = (sender as Button).DataContext as TimedTask.Model.AutoTask;
            TaskBLL.Instance.ItemClick("2", task);
            ((ViewModel.TaskListViewModel)base.DataContext).LoadCommand.Execute(null);
        }
        protected void LockMenuItem_Click(object sender, EventArgs e)
        {
            TimedTask.Model.AutoTask task = (sender as Button).DataContext as TimedTask.Model.AutoTask;
            TaskBLL.Instance.ItemClick("3", task);
            ((ViewModel.TaskListViewModel)base.DataContext).LoadCommand.Execute(null);
        }
        /// <summary>
        /// 是否以标题列表显示任务列表
        /// </summary>
        public bool IsShowTitleList { get; set; }
    }
}
