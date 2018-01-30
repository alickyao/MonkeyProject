using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace monkey.app.client_wpf.Demo.Baodian
{
    /// <summary>
    /// DefaultCtrXaml.xaml 的交互逻辑
    /// </summary>
    public partial class DefaultCtrXaml : Window
    {
        public DefaultCtrXaml()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Type controlType = typeof(Control);
            List<Type> derivedTypes = new List<Type>();
            Assembly assembly = Assembly.GetAssembly(typeof(Control));
            foreach (Type type in assembly.GetTypes()) {
                if (type.IsSubclassOf(controlType) && !type.IsAbstract && type.IsPublic) {
                    derivedTypes.Add(type);
                }
            }
            listTypes.ItemsSource = derivedTypes;
        }

        private void listTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Type type = (Type)listTypes.SelectedItem;
            var info = type.GetConstructor(System.Type.EmptyTypes);
            Control control = (Control)info.Invoke(null);
            control.Visibility = Visibility.Hidden;
            detailGrid.Children.Add(control);

            var template = control.Template;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            XamlWriter.Save(template, writer);
            txtTemplate.Text = sb.ToString();
            detailGrid.Children.Remove(control);
        }
    }
}
