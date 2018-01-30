using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace monkey.app.client_wpf
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// 当应用程序成为前台应用程序时触发。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("程序设置到前台");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Console.WriteLine("程序已经退出");
        }

        private void Application_Deactivated(object sender, EventArgs e)
        {
            Console.WriteLine("程序已到后台");
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(string.Format("error:type is {0}", e.Exception.Message));
            e.Handled = true;
        }
    }
}
