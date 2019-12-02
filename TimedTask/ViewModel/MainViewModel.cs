//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TimedTask.BLL;
using TimedTask.Utility;

namespace TimedTask.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 初始化 MainViewModel
        /// </summary>
        public MainViewModel()
        {
            if (!IsInDesignMode)
            {
                Loading();
            }
        }
        #region 属性
        /// <summary> 程序版本 </summary>
        public string Verson { get; set; }
        #endregion

        #region 方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Loading()
        {
            #region 温馨提示

            Utility.Calendar calender = new Utility.Calendar(DateTime.Now);
            string calendar = "农历：" + calender.ChineseDateString + "\r\n";
            calendar += " 时辰：" + calender.ChineseHour + "\r\n";
            calendar += " 属相：" + calender.AnimalString + "\r\n";
            calendar += (calender.ChineseTwentyFourDay.Length > 0) ? " 节气：" + calender.ChineseTwentyFourDay + "\r\n" : "";
            calendar += (calender.DateHoliday.Length > 0) ? " 节日：" + calender.DateHoliday + "\r\n" : "";
            calendar += " 星座：" + calender.Constellation + "\r\n";

            Helper.Instance.ShowPupUp("温馨提示！", null, calendar);
            #endregion

            this.Verson = "主程序版本 V" + Helper.Instance.GetVersion();
        }
        #endregion
    }
}
