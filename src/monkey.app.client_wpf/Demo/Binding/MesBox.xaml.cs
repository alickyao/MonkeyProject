using System.Windows;
using monkey.service.Logs;
using monkey.service.Db;
using System.Collections.Generic;

namespace monkey.app.client_wpf.Demo.Binding
{
    /// <summary>
    /// MesBox.xaml 的交互逻辑
    /// </summary>
    public partial class MesBox : Window
    {
        public MesBox()
        {
            InitializeComponent();
            var logs = BaseLog.searchList(new BaseLogSearchReqeust());
            //MsgListBox.DataContext = logs.rows;
        }
    }
}
