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

namespace monkey.app.client_wpf
{
    /// <summary>
    /// ExpenseItHome.xaml 的交互逻辑
    /// </summary>
    public partial class ExpenseItHome : Page
    {
        public ExpenseItHome()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 点击查看按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // View Expense Report 
            ExpenseReportPage expenseReportPage = new ExpenseReportPage();
            this.NavigationService.Navigate(expenseReportPage); 
        }
    }
}
