using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using monkey.app.client_wpf.Panel;

namespace monkey.app.client_wpf
{
    /// <summary>
    /// NewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewWindow : Window
    {
        public NewWindow()
        {

            //窗口打开的事件执行顺序
            this.SourceInitialized += NewWindow_SourceInitialized;//窗口被打开并开始加载
            this.Activated += NewWindow_Activated;//窗口获得焦点
            this.Loaded += NewWindow_Loaded;//窗口加载完毕
            this.ContentRendered += NewWindow_ContentRendered;//如果窗体焦点没有失去，则事件到此为止
            
            
            //关闭窗体的事件执行循序
            this.Closing += NewWindow_Closing; // 1
            this.Deactivated += NewWindow_Deactivated; // 2
            this.Closed += NewWindow_Closed; // 3
            this.Unloaded += NewWindow_Unloaded;// 4

            InitializeComponent();
        }

        private void NewWindow_SourceInitialized(object sender, EventArgs e)
        {
            Console.WriteLine("1---SourceInitialized！");
        }

        private void NewWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void NewWindow_Closed(object sender, EventArgs e)
        {
            
        }

        private void NewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void NewWindow_Deactivated(object sender, EventArgs e)
        {
            Console.WriteLine("窗口失去焦点");
        }

        private void NewWindow_ContentRendered(object sender, EventArgs e)
        {
            
        }

        private void NewWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void NewWindow_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("窗口已到前台");
        }

        private void ModifyUI() {
            Thread.Sleep(TimeSpan.FromSeconds(2));
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () {
                lblHello.Content = "欢迎你光临WPF的世界,Dispatche  同步方法 ！！";
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAppBeginInvoke_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    this.lblHello.Content = "欢迎你光临WPF的世界,Dispatche  异步方法 ！！";
                }));
            }).Start();
        }

        private void btnThd_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(ModifyUI);
            thread.Start();
        }


        /// <summary>
        /// 打开新窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canvasBtn_Click(object sender, RoutedEventArgs e)
        {
            CanvasWindow cWin = new CanvasWindow();
            cWin.ShowDialog();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            WrapPanelWindow pWin = new WrapPanelWindow();
            pWin.ShowDialog();
        }
    }
}
