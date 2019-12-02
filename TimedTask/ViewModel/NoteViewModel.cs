
//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using TimedTask.Model;
using TimedTask.Module;
using System.Windows.Input;

using TimedTask.Utility;

namespace TimedTask.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class NoteViewModel : ViewModelBase
    {
        private TimedTask.Model.Note _note = new Note();

        private BLL.NoteBLL _bllNote = new BLL.NoteBLL();
        private BLL.TypeListBLL _bllType = new BLL.TypeListBLL();

        /// <summary>
        /// 构造
        /// </summary>
        public NoteViewModel()
        {
            if (!IsInDesignMode)
            {
                List<TimedTask.Model.TypeList> list = _bllType.GetList(" FatherId=1 ", "Id,Name", "Id");
                if (list != null)
                    NoteTypeList = new ObservableCollection<TimedTask.Model.TypeList>(list);
            }
            this.AddBtnText = "添加";
            this._note = new TimedTask.Model.Note();
            CloseCommand = new ViewModelCommand((Object parameter) => { Close(); });
            LoadCommand = new ViewModelCommand((Object parameter) => { Load(); });
            ResetCommand = new ViewModelCommand((Object parameter) =>
            {
                this.NoteModel = new Note();
                this.NoteModel.TypeId = 3;
                this.AddBtnText = "添加";
            });
            if (!IsInDesignMode)
            {

            }
        }

        #region 命令

        public ICommand _noteSelectedChangedCommand;
        public ICommand ResetCommand { get; set; }
        /// <summary>
        /// 加载
        /// </summary>
        public ICommand LoadCommand { set; get; }
        /// <summary>
        /// 关闭
        /// </summary>
        public ICommand CloseCommand { set; get; }

        /// <summary>
        /// 列表选择
        /// </summary>
        public ICommand NoteSelectedChangedCommand
        {
            get
            {
                if (_noteSelectedChangedCommand == null)
                {
                    _noteSelectedChangedCommand = new ViewModelCommand((Object n) =>
                    {
                        this.NoteModel = (TimedTask.Model.Note)n;
                        this.AddBtnText = "修改";
                    });
                }
                return _noteSelectedChangedCommand;
            }
        }

        #endregion

        #region 属性
        public List<TimedTask.Model.Note> _notelist;
        /// <summary>
        /// 笔记列表
        /// </summary>
        public List<TimedTask.Model.Note> NoteList
        {
            get { return _notelist; }
            set
            {
                if (_notelist == value) return;
                _notelist = value;
                base.RaisePropertyChanged("NoteList");
            }
        }
        /// <summary>
        /// 笔记类型
        /// </summary>
        public ObservableCollection<TimedTask.Model.TypeList> NoteTypeList { get; set; }

        private string _btnText = "添加";
        /// <summary>
        /// 添加按钮文字
        /// </summary>
        public string AddBtnText
        {
            get { return _btnText; }
            set
            {
                if (_btnText == value) return;
                this._btnText = value;
                base.RaisePropertyChanged("AddBtnText");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimedTask.Model.Note NoteModel
        {
            get { return _note; }
            set
            {
                if (_note == value) return;
                _note = value;
                base.RaisePropertyChanged("NoteModel");
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 关闭
        /// </summary>
        private void Close()
        {
            //this.SettingsService.SaveSettings(this.UISettings);
        }
        /// <summary>
        /// 加载
        /// </summary>
        private void Load()
        {
            //Bind();
        }

        #endregion
    }
}
