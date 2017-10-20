using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

namespace monkey.app.client_wpf.Demo.ColorPicker
{
    public class ColorPicker:Control
    {
        public ColorPicker() { }

        public static DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color",
            typeof(Color),
            typeof(ColorPicker),
            new PropertyMetadata(Colors.Black)
            );

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
    }
}
