// 版权所有：
// 文 件  名：AutoTask.cs
// 功能描述：自动运行实体
// 创建标识：Seven Song(m.sh.lin0328@163.com) 2014/1/18 14:14:47
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Text;

namespace TimedTask.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoTask //: ModelBase
    {
        /// <summary>
        /// 唯一编码
        /// </summary>
        public Int64 Id { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 自动启动参数
        /// </summary>
        public string StartParameters { get; set; }
        /// <summary>
        /// 可执行程序路径
        /// </summary>
        public string ApplicationPath { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 终止时间
        /// </summary>
        public DateTime? StopDate { get; set; }
        /// <summary>
        /// 下次启动时间
        /// </summary>
        public DateTime? NextStartDate { get; set; }
        /// <summary>
        /// 任务描述
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否启用 0：禁用，1：启用，2：失效，3：过期
        /// </summary>
        public string Enable { get; set; }
        /// <summary>
        /// 运行状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 第几天
        /// </summary>
        public Int64 Dayth { get; set; }
        /// <summary>
        /// 星期
        /// </summary>
        public string Weeks { set; get; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string TaskType { get; set; }
        /// <summary>
        /// 音乐路径
        /// </summary>
        public string AudioPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RunType { get; set; }
        /// <summary>
        /// 时间间隔分钟
        /// </summary>
        public Int64 Interval { get; set; }
        /// <summary>
        /// 是否响铃 
        /// </summary>
        public string AudioEnable { get; set; }
        /// <summary>
        /// 声音大小 
        /// </summary>
        public Int64 AudioVolume { get; set; }
        /// <summary>
        /// 程序运行状态
        /// </summary>
        public enum Application
        {
            Strat,
            End
        }
    }
}
