using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TimedTask.Control
{
    /// <summary>
    /// PopUP.xaml 的交互逻辑
    /// </summary>
    public partial class PopUP : Window
    {
        private DispatcherTimer _timer;
        private double _screenHeight;
        private double _endTop;
        private DateTime _startTime;

        /// <summary> 消息主题 </summary>
        public string Subject { get; set; }
        /// <summary>  消息内容 </summary>
        public string Msg { get; set; }
        /// <summary> 窗口标题 </summary>
        public string PopTitle { get; set; }
        public PopUP()
        {
            InitializeComponent();
            this._startTime = DateTime.Now;
            this._screenHeight = SystemParameters.WorkArea.Height;

            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this._endTop = _screenHeight - this.Height;
            this.Top = SystemParameters.WorkArea.Height;

            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(50);
            this._timer.Tick += Start_Tick;
            this._timer.Start();
        }
        private void Start_Tick(object sender, EventArgs e)
        {
            if (this.Top > this._endTop)
                this.Top -= 5;
            //15秒关闭
            if (((TimeSpan)(DateTime.Now - this._startTime)).Seconds >= 15)
            {
                this._timer.Stop();
                this._timer.Interval = TimeSpan.FromMilliseconds(100);
                this._timer.Tick -= Start_Tick;
                this._timer.Tick += End_Tick;
                this._timer.Start();
            }
        }
        private void End_Tick(object sender, EventArgs e)
        {
            if (this.Top <= this._screenHeight)
                this.Top += 10;

            if (this.Top >= this._screenHeight)
            {
                this._timer.Stop();
                this._timer.Tick -= End_Tick;
                this._timer = null;

                this.Close();
            }
        }
        private void PopWin_Loaded(object sender, RoutedEventArgs e)
        {
            this.Msg = this.Msg == null ? "" : this.Msg;
            this.Subject = this.Subject == null ? "" : this.Subject;
            this.lblTitle.Content = this.PopTitle == null ? "系统信息" : this.PopTitle;

            if (this.Subject.Length == 0)
            {
                this.txtInfo.Inlines.Remove(this.txtSubject);
            }
            else
            {
                this.txtSubject.Text = this.Subject.Length > 18 ? this.Subject.Substring(0, 18) + "..." : this.Subject;
                this.txtSubject.Text += "\r\n";
            }
            this.txtContent.Text = (this.Msg.Length > 58 ? this.Msg.Substring(0, 58) + "..." : this.Msg);
        }
        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Helper.Instance.StopAudio();
            if (this._timer != null)
                this._timer.Stop();
            this.Close();
        }
    }
}
