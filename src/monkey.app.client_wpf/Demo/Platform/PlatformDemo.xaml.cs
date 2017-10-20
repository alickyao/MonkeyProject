using System;
using System.Collections.Generic;
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
namespace monkey.app.client_wpf.Demo.Platform
{
    /// <summary>
    /// PlatformDemo.xaml 的交互逻辑
    /// </summary>
    public partial class PlatformDemo : Window
    {
        public List<UserInfo> UserList { get; set; }

        public PlatformDemo()
        {
            this.ContentRendered += WindowThd_ContentRendered;

            UserList = new List<UserInfo>();

            UserList.Add(new UserInfo()
            {
                FullName = "Alick",
                LastName = "Sheng"
            });
            UserList.Add(new UserInfo() {
                FullName = "Jenny",
                LastName = "Zhang"
            });
            InitializeComponent();
            UserName.DataContext = new UserInfo() { FullName = "YaoSheng", LastName = "X" };
            UserListBox.DataContext = UserList;
            Time.DataContext = DateTime.Now;

            //Thread thread = new Thread(SetTime);
            //thread.Start();
        }

        private void WindowThd_ContentRendered(object sender, EventArgs e)
        {
            WorkTable.Content = new WorkTablePage();
        }

        private void SetTime() {
            Thread.Sleep(1000);
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                Time.DataContext = DateTime.Now;
            });
            Thread thread = new Thread(SetTime);
            thread.Start();
        }
    }

    public class UserInfo {
        public string FullName { get; set; }

        public string LastName { get; set; }
    }
}
