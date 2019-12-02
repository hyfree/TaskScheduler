using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TimedTask.Model
{
    /// <summary>
    /// 记事本实体
    /// </summary>
    public class Note //: ModelBase
    {
        /// <summary>
        /// 唯一编码
        /// </summary>
        public Int64 Id { get; set; }
        /// <summary>
        /// 记事本id 
        /// </summary>
        public Int64 TypeId { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        public Note()
        {

        }

        public Note(Int64 id, Int64 typeId, DateTime createDate, DateTime modifyDate, string title, string content)
        {
            Id = id;
            TypeId = typeId;
            CreateDate = createDate;
            ModifyDate = modifyDate;
            Title = title;
            Content = content;
        }
    }

    public class Notes
    {
        private readonly ObservableCollection<Note> _person = new ObservableCollection<Note>();

        public ObservableCollection<Note> NoteObservable
        {
            get { return _person; }
        }

        public Notes()
        {
            try
            {
                GetCollection();
            }
            catch (Exception e)
            {

            }
        }
        private void GetCollection()
        {

        }
    }
}
