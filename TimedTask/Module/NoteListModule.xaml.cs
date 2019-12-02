using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimedTask.BLL;
using TimedTask.Utility;
using TimedTask.ViewModel;

namespace TimedTask.Module
{
    /// <summary>
    /// NoteListModule.xaml 的交互逻辑
    /// </summary>
    public partial class NoteListModule : UserControl
    {
        private BLL.NoteBLL _bllNote = new BLL.NoteBLL();
        private TimedTask.Model.Note _model = new TimedTask.Model.Note();

        public NoteListModule()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.uPager.PageSize = 11;
            this.uPager.EventPaging += new EventHandler(cPager_OnPageChanged);
            BindList();
            this.uPager.Bind();
        }
        //翻页
        private void cPager_OnPageChanged(object sender, EventArgs e)
        {
            BindList();
        }
        private void Bind(string whereStr)
        {
            int rowCount = 0;
            if (String.IsNullOrEmpty(whereStr))
                whereStr = "1=1";

            List<Model.Note> list = this._bllNote.GetList(null, null, uPager.PageSize, uPager.PageIndex, "*", whereStr, " ModifyDate DESC", out rowCount);
            this.uPager.RecordCount = rowCount;
            this.uPager.list = list;
            this.lstNote.ItemsSource = list;
            this.uPager.Bind();
        }
        protected void Delete_Click(object sender, EventArgs e)
        {
            TimedTask.Model.Note note = (sender as Button).DataContext as TimedTask.Model.Note;
            Delete(note.Id);
            ((ViewModel.NoteViewModel)base.DataContext).LoadCommand.Execute(null);
        }
        private void Delete(object id)
        {
            Helper.Instance.AlertConfirm(null, "您确定要删除吗？", () =>
            {
                try
                {
                    int result = _bllNote.Delete(" Id=" + id);
                    Helper.Instance.Alert(result > 0 ? "操作成功！" : "操作失败！");
                }
                catch (Exception ex)
                {
                    MSL.Tool.LogHelper.Instance.WriteLog("MainWindow DropList 删除选中项\r\n" + ex.ToString());
                }
            });
            BindList();
        }
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtQueryTitle.Text.Trim().Length == 0 && this.cboQueryType.SelectedValue == null)
            {
                Helper.Instance.Alert("查询条件不能为空！");
                return;
            }
            uPager.PageIndex = 1;
            BindList();
        }
        private string GetWhere()
        {
            string whereStr = " 1=1 ";
            string title = this.txtQueryTitle.Text.Trim();
            string typeId = "";
            if (this.cboQueryType.SelectedValue != null)
                typeId = this.cboQueryType.SelectedValue.ToString();

            if (title.Length > 0)
                whereStr += " AND Title LIKE '%" + title + "%' ";
            if (typeId.Length > 0 && typeId != "0")
                whereStr += " AND TypeId= " + typeId;

            return whereStr;
        }
        private void BindList()
        {
            string str = GetWhere();
            Bind(str);
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Model.Note model = new Model.Note();
            if ((sender as Button).CommandParameter != null)
            {
                model.Id = Int64.Parse((sender as Button).CommandParameter.ToString());
            }
            model.Title = this.txtTitle.Text;
            model.Content = this.txtContent.Text;
            if (model.Title.Length == 0 || model.Content.Length == 0)
            {
                Helper.Instance.AlertWarning("标题或内容不能为空！");
                return;
            }
            if (this.cboType.SelectedValue == null)
            {
                Helper.Instance.AlertWarning("类别不能为空！");
                return;
            }
            model.ModifyDate = DateTime.Now;
            model.TypeId = Int64.Parse(this.cboType.SelectedValue.ToString());
            Save(model);
        }
        private void Save(Model.Note model)
        {
            if (model == null || model.Title.Length == 0 || model.Content.Length == 0)
            {
                Helper.Instance.AlertWarning("标题或内容不能为空！");
                return;
            }
            try
            {
                if (model.Id == 0)//新增
                {
                    model.CreateDate = DateTime.Now;
                    _bllNote.Add(model);
                    Helper.Instance.AlertSuccess("添加成功！");
                    return;
                }

                model.ModifyDate = DateTime.Now;
                _bllNote.Update(model, " Id=" + model.Id);
                Helper.Instance.AlertSuccess("修改成功！");
            }
            catch (Exception ex)
            {
                MSL.Tool.LogHelper.Instance.WriteLog("NoteListModule btnOK_Click\r\n" + ex.ToString());
                Helper.Instance.AlertError("系统异常，操作失败！");
            }
            this.cboType.SelectedIndex = 0;
        }

        private void btnQuery_Click(object sender, SelectionChangedEventArgs e)
        {
            this.uPager.PageIndex = 1;
            BindList();
        }
    }
}
