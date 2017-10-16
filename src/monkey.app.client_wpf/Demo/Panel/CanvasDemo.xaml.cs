using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace monkey.app.client_wpf.Demo.Panel
{
    /// <summary>
    /// CanvasDemo.xaml 的交互逻辑
    /// </summary>
    public partial class CanvasDemo : Page
    {
        public CanvasDemo()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var pWin = Window.GetWindow(this);
            pWin.Content = new NavDemo();
        }
    }
}
