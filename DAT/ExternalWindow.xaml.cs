
using System.Windows;
using System.Windows.Media;
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
            Closing += OnClose;            
            Background = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
            BlueScore_external.Background = new SolidColorBrush(Color.FromArgb(255, 27, 73, 242));
            BlueCanvas.Background = new SolidColorBrush(Color.FromArgb(255, 27, 73, 242));
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void ExternalWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double controlsize = ((e.NewSize.Width / 12) / 3 * 4);
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlsize);
        }
    }
}
