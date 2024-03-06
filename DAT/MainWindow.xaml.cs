
using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows;

namespace DAT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _startTimeDisplay = "2:00";
        private readonly CountdownTimer _timer;
        private ExternalWindow _externalWindow;

        public MainWindow()
        {
            _externalWindow = new ExternalWindow();

            InitializeComponent();
            Clock_label.Text = _startTimeDisplay;

            _timer = new CountdownTimer();

            _timer.SetTime(2, 0);

            //update label text
            _timer.TimeChanged += onTimeChanged;

            // show messageBox on timer = 0:00.000
            //_timer.CountDownFinished += () => MessageBox.Show("Timer finished the work!");
        }

        private void onTimeChanged(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => Clock_label.Text = sender.ToString());
        }

        private void onCountdownFinished(object sender, EventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void StartClock_Click(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void StopClock_Click(object sender, RoutedEventArgs e)
        {
            _timer.Pause();
        }

        private void ResetClock_Click(object sender, RoutedEventArgs e)
        {
            _timer.Reset();
        }

        private void VisaExtern_Click(object sender, RoutedEventArgs e)
        {
            if (_externalWindow == null)
            {
                _externalWindow = new ExternalWindow();
            }
            if (_externalWindow.Visibility == Visibility.Hidden)
            {
                _externalWindow.Visibility = Visibility.Visible;
            }
            else
            {
                _externalWindow.Show();
                _externalWindow.Owner = this;

            }
        }


    }
}
