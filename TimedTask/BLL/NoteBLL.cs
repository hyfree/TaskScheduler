using System;
using System.Collections.Generic;
using System.Text;

namespace TimedTask.BLL
{
    /// <summary>
    /// 记事
    /// </summary>
    public class NoteBLL : MSL.Tool.Data.DBAccessBase<Model.Note>
    {
        public NoteBLL()
            : base("[Note]", "Id", true)
        {

        }
        public NoteBLL(string connString)
            : base(connString, "[Note]", "Id", true)
        {

        }
    }
}
