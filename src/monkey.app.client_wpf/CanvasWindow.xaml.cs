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
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;


namespace monkey.app.client_wpf
{
    /// <summary>
    /// CanvasWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CanvasWindow : Window
    {
        public CanvasWindow()
        {
            InitializeComponent();
        }

        private void getEll() {
            Ellipse r = new Ellipse();

            r.Fill = new SolidColorBrush(Colors.Red);

            r.Stroke = new SolidColorBrush(Colors.Red);

            r.Width = 5;

            r.Height = 5;

            r.SetValue(Canvas.LeftProperty, (double)rad.Next(400));

            r.SetValue(Canvas.TopProperty, (double)rad.Next(500));
            canv.Children.Add(r);
        }
        static Random rad = new Random();
        static int max = 10;

        private Canvas canv;

        private void getEllipses_Click(object sender, RoutedEventArgs e)
        {
            canv = new Canvas();
            canv.Margin = new Thickness(0, 0, 0, 0);
            this.Content = canv;
            for (int i = 0; i < max; i++) {
                new Thread(() =>
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        getEll();
                    }));
                }).Start();
            }
        }
    }
}
