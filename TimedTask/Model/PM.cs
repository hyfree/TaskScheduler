// 版权所有：
// 文 件 名：PM.cs
// 功能描述：公共变量
// 创建标识：Seven Song(m.sh.lin0328@163.com) 2014/1/19 12:08:42
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections;
using System.Text;

namespace TimedTask.Model
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class PM
    {
        public const string ApplicationName = "TimedTask";//应用程序英文名
        private static System.Collections.Hashtable audioHt;
        private static Hashtable _htTaskType = new Hashtable();
        private static bool _mintoTray = false;
        private static string _appbgImg = String.Empty;
        private static char _spiderChar = '⊙';//字符串特殊分割符
        private static string _HomePageUrl = "http://home.cnblogs.com/u/shanlin/";
        /// <summary>
        /// 个人主页
        /// </summary>
        public static string HomePageUrl
        {
            get { return PM._HomePageUrl; }
            set { PM._HomePageUrl = value; }
        }

        /// <summary>
        /// 字符串特殊分割符
        /// </summary>
        public static char SpiderChar
        {
            get { return _spiderChar; }
            set { _spiderChar = value; }
        }

        /// <summary>
        /// 锁屏背景
        /// </summary>
        public static string LockBgImg { set; get; }
        /// <summary>
        /// 是否记录运行日志
        /// </summary>
        public static bool SaveLog { set; get; }
        /// <summary>
        /// 资讯列表抓取页面
        /// </summary>
        public static string NewsUrl { set; get; }
        /// <summary>
        /// 资讯列表页面标签
        /// </summary>
        public static SortedList NewsTag { set; get; }
        /// <summary>
        /// 锁屏时间 分钟
        /// </summary>
        public static int LockMinute { get; set; }
        /// <summary>
        /// 窗体背景
        /// </summary>
        public static string AppBgImg
        {
            get { return _appbgImg; }
            set { _appbgImg = value; }
        }
        /// <summary>
        /// 是否最小化到托盘
        /// </summary>
        public static bool MinToTray
        {
            get { return _mintoTray; }
            set { _mintoTray = value; }
        }
        /// <summary>
        /// 是否启动后自动弹出资讯
        /// </summary>
        public static bool ShowNews { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public static Hashtable HtTaskType
        {
            get
            {
                if (_htTaskType == null || _htTaskType.Count == 0)
                {
                    _htTaskType = new Hashtable();
                    _htTaskType.Add("0", "定时任务");
                    _htTaskType.Add("1", "定时提醒");
                    _htTaskType.Add("2", "定时关机");
                    _htTaskType.Add("3", "关闭显示器");
                    _htTaskType.Add("4", "打开显示器");
                    _htTaskType.Add("5", "定时锁屏");
                }
                return _htTaskType;
            }
        }
        /// <summary>
        /// 可执行文件启动目录
        /// </summary>
        public static string StartPath { get; set; }
        /// <summary>
        /// 计划任务配置文件路径
        /// </summary>
        public static string TaskConfig { get; set; }
        /// <summary>
        /// 是否已经锁屏
        /// </summary>
        public static bool IsScreenLock { get; set; }
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string Config { get; set; }

        /// <summary>
        /// 声音列表
        /// </summary>
        public static Hashtable AudioHt
        {
            get
            {
                if (audioHt == null || audioHt.Count == 0)
                {
                    string path = StartPath + "\\Audio\\";
                    if (System.IO.Directory.Exists(path))
                    {
                        string name = "";
                        audioHt = new System.Collections.Hashtable();
                        foreach (string f in System.IO.Directory.GetFileSystemEntries(path))
                        {
                            path = System.IO.Path.GetFullPath(f);
                            name = System.IO.Path.GetFileName(f);
                            name = name.Substring(0, name.LastIndexOf("."));

                            if (!audioHt.ContainsKey(name))
                                audioHt.Add(name, path);

                            //_htTaskType.Add(name, path);
                        }
                    }
                }
                return audioHt;
            }
        }
        /// <summary>
        /// 记事类别
        /// </summary>
        public static Hashtable NoteTypeHt { set; get; }
        /// <summary>
        /// 主窗口是否显示
        /// </summary>
        public static bool IsMainWinShow { set; get; }
    }
}
