using System;
using System.Collections;
using System.Collections.Generic;
using TimedTask.Model;

namespace TimedTask.BLL
{
    /// <summary>
    /// 定时任务类
    /// </summary>
    public class AutoTaskBLL : MSL.Tool.Data.DBAccessBase<Model.AutoTask>
    {
        public AutoTaskBLL()
            : base("AutoTask", "Id", true)
        {

        }
        public AutoTaskBLL(string connString)
            : base(connString, "AutoTask", "Id", true)
        {

        }
    }
}
