
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
            var textColor = new SolidColorBrush(Color.FromArgb(255, 0, 153, 51));
            HoldScoreTypeBlueExt.Foreground = textColor;
            HoldScoreTypeWhiteExt.Foreground = textColor;
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
