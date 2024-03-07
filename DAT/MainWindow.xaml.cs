
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace DAT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _startTimeDisplay = "2:00";
        private int blueScore = 0;
        private int whiteScore = 0;
        private System.Timers.Timer _timer = new System.Timers.Timer(1000.0);
        private ExternalWindow _externalWindow;
        private int _maxTime = 2 * 60;
        private Stopwatch _stopwatch = new Stopwatch();
        private bool _mustStop => (_maxTime - _ticks) < 0;
        private int _ticks = 0;
        public TimeSpan TimeLeft =>
           (_maxTime - _ticks) > 0
           ? TimeSpan.FromSeconds(_maxTime - _ticks)
           : TimeSpan.FromMilliseconds(0);

        public MainWindow()
        {
            InitializeComponent();
            _externalWindow = new ExternalWindow();
            Clock_label.Text = _startTimeDisplay;


            //update label text            
            _timer.Elapsed += onTimeChanged;

            // show messageBox on timer = 0:00.000
            //_timer.CountDownFinished += () => MessageBox.Show("Timer finished the work!");
        }

        private void onTimeChanged(object sender, EventArgs e)
        {
            _ticks += 1;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_mustStop)
                {
                    CheckWinner();
                }
                Clock_label.Text = TimeLeft.ToString(@"m\:ss"); ;
                if (_externalWindow != null)
                {
                    _externalWindow.Clock_external.Text = TimeLeft.ToString(@"m\:ss"); ;
                }
            });
        }

        private void CheckWinner()
        {
            
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
            _stopwatch.Start();
        }

        private void StopClock_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _stopwatch.Stop();
        }

        private void ResetClock_Click(object sender, RoutedEventArgs e)
        {
            _stopwatch.Reset();
            _timer.Stop();
            Reset();
        }

        private void Reset()
        {
            Clock_label.Text = _startTimeDisplay;
            _externalWindow.Clock_external.Text = _startTimeDisplay;
            _ticks = 0;
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

        #region Score
        private void BlueWazaPlus_Click(object sender, RoutedEventArgs e)
        {
            blueScore += 7;

        }

        private void BlueWazaMinus_Click(object sender, RoutedEventArgs e)
        {
            blueScore -= 7;
        }

        private void BlueIpponPlus_Click(object sender, RoutedEventArgs e)
        {
            blueScore += 10;
        }

        private void BlueIpponMinus_Click(object sender, RoutedEventArgs e)
        {
            blueScore -= 10;
        }

        private void WhiteWazaPlus_Click(object sender, RoutedEventArgs e)
        {
            whiteScore += 7;
        }

        private void WhiteWazaMinus_Click(object sender, RoutedEventArgs e)
        {
            whiteScore -= 7;
        }

        private void WhiteIpponPlus_Click(object sender, RoutedEventArgs e)
        {
            whiteScore += 10;
        }

        private void WhiteIpponMinus_Click(object sender, RoutedEventArgs e)
        {
            whiteScore -= 10;
        }

        #endregion
    }
}
