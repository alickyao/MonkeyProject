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
using monkey.service.Users;
using System.Collections.ObjectModel;
using System.Globalization;

namespace monkey.app.client_wpf.Demo.Baodian
{
    /// <summary>
    /// SimpFormTest.xaml 的交互逻辑
    /// </summary>
    public partial class SimpFormTest : Window
    {
        public SimpFormTest()
        {
            InitializeComponent();
        }

        ObservableCollection<UserManager> userListRow;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var userList = UserManager.searchList(new UserManagerSearchRequest() {
                cId = new service.BaseBatchRequest<string>()
            });

            userListRow = new ObservableCollection<UserManager>();
            foreach (var item in userList.rows) {
                userListRow.Add(item);
            }
            UserList.ItemsSource = userListRow;
            UserList.DisplayMemberPath = "fullName";
        }

        private void SaveUserInfo_Click(object sender, RoutedEventArgs e)
        {
            //if (userBdingGroup.CommitEdit()) {
            //    FocusManager.SetFocusedElement(this, (Button)sender);
            //    var user = (UserManager)UserInfo.DataContext;
            //    UpdateUserInfo.Text = string.Format("{0},{1},{2}", user.fullName, user.mobilePhone, user.loginName);
            //}
            
        }

        private void DelUserInfo_Click(object sender, RoutedEventArgs e)
        {
            userListRow.Remove((UserManager)UserList.SelectedItem);
        }

        private void AddUserInfo_Click(object sender, RoutedEventArgs e)
        {
            var user = (UserManager)UserInfo.DataContext;
            userListRow.Add(user);
        }

        private void UserInfo_Error(object sender, ValidationErrorEventArgs e)
        {
            MessageBox.Show(e.Error.ErrorContent.ToString());
        }
    }

    public class StringLengthVali : ValidationRule {
        private int max = int.MaxValue;
        private int min = 0;

        public int Max {
            get { return max; }
            set { max = value; }
        }

        public int Min {
            get { return min; }
            set { min = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string v = (string)value;
            if (Min > 0)
            {
                if (string.IsNullOrEmpty(v))
                {
                    return new ValidationResult(false, "必填");
                }
                if (v.Length < Min)
                {
                    return new ValidationResult(false, "太短");
                }
                
            }
            if (v.Length > Max)
            {
                return new ValidationResult(false, "太长");
            }
            return new ValidationResult(true, null);
        }
    }

    public class GridFromRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            BindingGroup bindingGroup = (BindingGroup)value;
            UserManager user = (UserManager)bindingGroup.Items[0];

            string fullName = (string)bindingGroup.GetValue(user, "fullName");
            if (string.IsNullOrEmpty(fullName))
            {
                return new ValidationResult(false, "姓名不能为空");
            }
            else {
                return new ValidationResult(true, null);
            }
        }
    }
}
