using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DAT
{
    /// <summary>
    /// Interaction logic for ExternalWindow.xaml
    /// </summary>
    public partial class ExternalWindow : Window
    {
        public ExternalWindow()
        {
            InitializeComponent();
            this.Closing += OnClose;
        }

        private void Clock_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void ExternalWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double controlsize = ((e.NewSize.Width / 12) / 3 * 4);
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlsize);            
        }
    }
}
