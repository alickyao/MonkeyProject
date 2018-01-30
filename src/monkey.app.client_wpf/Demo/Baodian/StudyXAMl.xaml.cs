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

namespace monkey.app.client_wpf.Demo.Baodian
{
    /// <summary>
    /// StudyXAMl.xaml 的交互逻辑
    /// </summary>
    public partial class StudyXAMl : Window
    {
        public StudyXAMl()
        {
            //this.Cursor = Cursors.Wait;
            InitializeComponent();
            CommandBinding cb = new CommandBinding(ApplicationCommands.New);
            cb.Executed += Cb_Executed;
            this.CommandBindings.Add(cb);
        }

        private void Cb_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void lit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lit.SelectedItem == null) return;
            lit_SelectedItem.Text = "您已选择：" + lit.SelectedIndex;
            ((CheckBox)lit.SelectedItem).IsChecked = true;
        }

        private void shutdownbtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ImgBtn_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush b = (ImageBrush)this.Resources["TitleBrush"];
            b.Viewport = new Rect(0, 0, 100, 100);
        }


        private void Btn_MonseEnter(object sender, MouseEventArgs e) {
            Button b = (Button)sender;
            b.Background = new SolidColorBrush(Colors.Yellow);
        }
    }
}
