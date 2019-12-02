//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;
//----------------------------------------------------------------*/
// 版权所有：
// 文 件  名：TaskRunLogViewModel.cs
// 功能描述：
// 创建标识：MaShanLin 2014/6/21 16:35:29
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Text;

namespace TimedTask.ViewModel
{
    public class SysLogViewModel : ViewModelBase
    {
        private BLL.SysLogBLL _bllLog = new BLL.SysLogBLL();
        public SysLogViewModel()
        {
            this.LoadCommand = new ViewModelCommand((Object id) =>
            {
                TaskLogList = this._bllLog.GetList(" TaskId=" + (Int64)id, "", "CreateDate DESC");
            });
        }
        /// <summary>
        /// 列表选择
        /// </summary>
        public ICommand LoadCommand { get; set; }

        private List<TimedTask.Model.SysLog> _taskLogList;
        public List<TimedTask.Model.SysLog> TaskLogList
        {
            get { return _taskLogList; }
            set
            {
                _taskLogList = value;
                base.RaisePropertyChanged("TaskLogList");
            }
        }
    }
}
