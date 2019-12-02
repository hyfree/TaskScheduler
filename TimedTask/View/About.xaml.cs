using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TimedTask.View
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            //TimedTask.Utility.ControlHelper.Instance.SetBackground(this.mainBoder, TimedTask.Model.PModel.AppBgImg);

            StringBuilder sbMsg = new StringBuilder();
            sbMsg.Append("版 本 号：V");
            sbMsg.Append(Helper.Instance.GetVersion() + "\r\n");
            sbMsg.Append("版权所有：m.sh.lin0328@163.com \r\n");
            sbMsg.Append("Copyright@ 2014 Lin.All Right Reserved.\r\n");
            sbMsg.Append("作品说明：本作品只用于学习交流, 如若转载, 请注明作者与出处。");

            this.txtInfo.Text = sbMsg.ToString();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", TimedTask.Model.PM.StartPath + "\\" + "说明.txt");
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Documents.Hyperlink).CommandParameter != null)
            {
                string link = (sender as System.Windows.Documents.Hyperlink).CommandParameter.ToString();
                System.Diagnostics.Process.Start("iexplore.exe", link);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
