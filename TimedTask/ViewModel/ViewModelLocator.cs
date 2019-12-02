﻿//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;
//using GalaSoft.MvvmLight.Ioc;//控制反转，用来创建实例
//using Microsoft.Practices.ServiceLocation;          
//using System.ServiceModel:分析rss和atom都是非常便捷的    
//using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimedTask.ViewModel
{
    /// <summary>
    /// 用来管理ViewModel类的
    /// </summary>
    public class ViewModelLocator : ViewModelBase
    {
        static ViewModelLocator()
        {
            // 方法执行顺序：
            // 1.App构造函数
            // 2.ViewModelLocator构造函数（App.xaml中的资源添加了ViewModelLocator）
            // 3.App的Application_Launching方法
            // 4.Navigate方法(App.RootFrame不为空)
            // 5.取得对应的ViewModel(MainViewModel)，执行对应的依赖注入的委托
            // 正确写法,不立即注入(默认值)
            // 此处每个ViewModel中的navigationService相同（可考虑改进，适合使用单例模式）

            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);//添加 Ioc 容器
            //SimpleIoc.Default.Register<MainViewModel>();//添加M ainViewModel 到管理器当中
            //SimpleIoc.Default.Register<NoteViewModel>();
            //SimpleIoc.Default.Register<SysLogViewModel>();
            //SimpleIoc.Default.Register<ConfigViewModel>();
            //SimpleIoc.Default.Register<TaskListViewModel>();
            //SimpleIoc.Default.Register<TaskEditViewModel>();
            //SimpleIoc.Default.Register<PageImageDownViewModel>();
        }
        private MainViewModel _MainWindowViewModel;          
        private TaskListViewModel _TaskViewModel;
        private ConfigViewModel _ConfigWindowViewModel;
        private SysLogViewModel _SystemLogViewModel;
        private NoteViewModel _NoteListViewModel;
        private PageImageDownViewModel _ImageDownViewModel;
        public MainViewModel MainWindowViewModel
        {
            get
            {
                if (_MainWindowViewModel == null)
                    _MainWindowViewModel = new MainViewModel();
                return _MainWindowViewModel;// ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public PageImageDownViewModel ImageDownViewModel
        {
            get
            {
                if (_ImageDownViewModel == null)
                    _ImageDownViewModel = new PageImageDownViewModel();
                return _ImageDownViewModel;// ServiceLocator.Current.GetInstance<PageImageDownViewModel>();
            }
        }
        public NoteViewModel NoteListViewModel
        {
            get
            {
                if (_NoteListViewModel == null)
                    _NoteListViewModel = new NoteViewModel();
                return _NoteListViewModel;// ServiceLocator.Current.GetInstance<NoteViewModel>();
            }
        }
        public SysLogViewModel SystemLogViewModel
        {
            get
            {
                if (_SystemLogViewModel == null)
                    _SystemLogViewModel = new SysLogViewModel();
                return _SystemLogViewModel;// ServiceLocator.Current.GetInstance<SysLogViewModel>();
            }
        }
        public ConfigViewModel ConfigWindowViewModel
        {
            get
            {
                if (_ConfigWindowViewModel == null)
                    _ConfigWindowViewModel = new ConfigViewModel();
                return _ConfigWindowViewModel;// ServiceLocator.Current.GetInstance<ConfigViewModel>();
            }
        }
        public TaskListViewModel TaskViewModel
        {
            get
            {
                if (_TaskViewModel == null)
                    _TaskViewModel = new TaskListViewModel();
                return _TaskViewModel;// ServiceLocator.Current.GetInstance<TaskListViewModel>();
            }
        }
    }
}
