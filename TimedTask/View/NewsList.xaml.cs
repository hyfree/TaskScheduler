using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TimedTask.View
{
    /// <summary>
    /// NewsList.xaml 的交互逻辑
    /// </summary>
    public partial class NewsList : Window
    {
        private WebBrowser wb = null;
        public NewsList()
        {
            InitializeComponent();
            Helper.Instance.Loading(BLL.TaskBLL.Instance.SpiderNewsToHtml, null);

            LoadNewsList();
        }
        //加载新闻资讯
        private void LoadNewsList()
        {
            string path = Model.PM.StartPath + "\\News\\news.htm";
            if (!File.Exists(path))
                path = Model.PM.NewsUrl;
            TimedTask.Control.WebBrowserOverlay wbo = new TimedTask.Control.WebBrowserOverlay(brMain);
            wb = wbo.WebBrowser;
            wb.Navigate(new Uri(path));
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
