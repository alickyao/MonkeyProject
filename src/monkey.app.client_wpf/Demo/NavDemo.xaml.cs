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
using monkey.app.client_wpf.Demo.Panel;
using monkey.app.client_wpf.Demo.Platform;
using monkey.app.client_wpf.Demo.Binding;
using monkey.app.client_wpf.Demo.ColorPicker;

namespace monkey.app.client_wpf.Demo
{
    /// <summary>
    /// NavDemo.xaml 的交互逻辑
    /// </summary>
    public partial class NavDemo : Page
    {

        public NavDemo()
        {
            
            InitializeComponent();
            //获取硬件显卡加速级别  0 无 1 差 2 良
            int rendertier = RenderCapability.Tier >> 16;
            RendertierTextBlock.Text = rendertier.ToString();
        }

        #region -- 布局基本
        /// <summary>
        /// 栈面板，可以将元素排列成一行或者一列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StackPanel_Click(object sender, RoutedEventArgs e)
        {
            var pWin = Window.GetWindow(this);
            StackPanelDemo p = new StackPanelDemo();
            pWin.Content = p;
        }

        /// <summary>
        /// 环绕面板，当元素布局到达边界时，可以自动换行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WrapPanel_Click(object sender, RoutedEventArgs e)
        {
            var pWin = Window.GetWindow(this);
            WrapPanelDemo p = new WrapPanelDemo();
            pWin.Content = p;
        }

        /// <summary>
        /// 停靠面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockPanel_Click(object sender, RoutedEventArgs e)
        {
            var pWin = Window.GetWindow(this);
            DockPanelDemo p = new DockPanelDemo();
            pWin.Content = p;
        }

        /// <summary>
        /// 画布
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_Click(object sender, RoutedEventArgs e)
        {
            var pWin = Window.GetWindow(this);
            CanvasDemo p = new CanvasDemo();
            pWin.Content = p;
        }

        /// <summary>
        /// 网格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            var pWin = Window.GetWindow(this);
            GridDemo p = new GridDemo();
            pWin.Content = p;
        }

        /// <summary>
        /// 均布网格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UniformGrid_Click(object sender, RoutedEventArgs e)
        {
            var pWin = Window.GetWindow(this);
            UniformGridDemo p = new UniformGridDemo();
            pWin.Content = p;
        }
        #endregion

        #region 布局-实例
        /// <summary>
        /// 通用后台界面布局 - 演示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Platform_Click(object sender, RoutedEventArgs e)
        {
            PlatformDemo p = new PlatformDemo();
            p.Show();
        }
        #endregion

        private void SimpBinding_Click(object sender, RoutedEventArgs e)
        {
            ImgBinding win = new ImgBinding();
            win.Show();
        }

        private void ColorPickerBtn_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDemo win = new ColorPickerDemo();
            win.Show();
        }

        private void MesBox_Click(object sender, RoutedEventArgs e)
        {
            MesBox win = new MesBox();
            win.Show();
        }
    }
}
