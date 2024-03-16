
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace DAT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _startTimeDisplay = "2:00";
        private const string _blueWin = "Blue wins";
        private const string _whiteWin = "White wins";
        private const string _draw = "Draw";
        private int _blueScore = 7;
        private int _whiteScore = 7;
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
            _timer.Elapsed += OnTimeChanged;
        }

        private void CheckWinner(bool timeUp = false)
        {
            if (_externalWindow != null)
            {
                if (_blueScore - _whiteScore >= 30 || _whiteScore - _blueScore >= 30)
                {
                    CheckWinningScore();
                }
                if (timeUp)
                {
                    CheckWinningScore();
                }
            }
        }
        #region WinnerLogic

        private void CheckWinningScore()
        {
            if (_blueScore > _whiteScore)
            {
                ResetTextboxBorder();
                _externalWindow.Result_external.Text = _blueWin;
                _externalWindow.Result_external.Background = Brushes.Blue;
                _externalWindow.BlueScore_external.BorderBrush = Brushes.Red;
                _externalWindow.BlueScore_external.BorderThickness = new Thickness(4.0);
                SetWinnerTextboxBorder();
                return;
            }

            if (_whiteScore > _blueScore)
            {
                ResetTextboxBorder();
                _externalWindow.Result_external.Text = _whiteWin;
                _externalWindow.Result_external.Background = Brushes.White;
                _externalWindow.WhiteScore_external.BorderBrush = Brushes.Red;
                _externalWindow.WhiteScore_external.BorderThickness = new Thickness(4.0);
                SetWinnerTextboxBorder();
                return;
            }
            if (_whiteScore == _blueScore)
            {
                _externalWindow.Result_external.Text = _draw;
                _externalWindow.Result_external.Background = Brushes.White;
                _externalWindow.WhiteScore_external.BorderBrush = Brushes.Red;
                _externalWindow.Result_external.Background = Brushes.Blue;
                _externalWindow.BlueScore_external.BorderBrush = Brushes.Red;
                _externalWindow.WhiteScore_external.BorderThickness = new Thickness(4.0);
            }
            ResetTextboxBorder();
        }

        private void SetWinnerTextboxBorder()
        {
            _externalWindow.Result_external.BorderBrush = Brushes.Red;
            _externalWindow.Result_external.BorderThickness = new Thickness(4.0);
        }

        #endregion

        private void ResetTextboxBorder()
        {
            _externalWindow.Result_external.BorderBrush = Brushes.Black;
            _externalWindow.Result_external.BorderThickness = new Thickness(2.0);

            _externalWindow.WhiteScore_external.BorderBrush = Brushes.Black;
            _externalWindow.WhiteScore_external.BorderThickness = new Thickness(2.0);

            _externalWindow.BlueScore_external.BorderBrush = Brushes.Black;
            _externalWindow.BlueScore_external.BorderThickness = new Thickness(2.0);
        }


        #region Clock

        private void OnTimeChanged(object sender, EventArgs e)
        {
            _ticks += 1;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_mustStop)
                {
                    CheckWinner(timeUp: true);
                }
                Clock_label.Text = TimeLeft.ToString(@"m\:ss"); ;
                if (_externalWindow != null)
                {
                    _externalWindow.Clock_external.Text = TimeLeft.ToString(@"m\:ss"); ;
                }
            });
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

        #endregion

        private void Reset()
        {
            Clock_label.Text = _startTimeDisplay;
            _externalWindow.Clock_external.Text = _startTimeDisplay;
            _ticks = 0;
            ResetTextboxBorder();
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
            _blueScore += 7;
            BlueAfterScoreChange();
        }

        private void BlueWazaMinus_Click(object sender, RoutedEventArgs e)
        {
            _blueScore -= 7;
            BlueAfterScoreChange();
        }

        private void BlueIpponPlus_Click(object sender, RoutedEventArgs e)
        {
            _blueScore += 10;
            BlueAfterScoreChange();
        }

        private void BlueIpponMinus_Click(object sender, RoutedEventArgs e)
        {
            _blueScore -= 10;
            BlueAfterScoreChange();
        }

        private void BlueAfterScoreChange()
        {
            BlueScore.Text = _blueScore.ToString();
            CheckWinner();
            if (_externalWindow != null)
            {
                _externalWindow.BlueScore_external.Text = _blueScore.ToString();
            }
        }

        private void WhiteWazaPlus_Click(object sender, RoutedEventArgs e)
        {
            _whiteScore += 7;
            WhiteAfterScoreChange();
        }

        private void WhiteWazaMinus_Click(object sender, RoutedEventArgs e)
        {
            _whiteScore -= 7;
            WhiteAfterScoreChange();
        }

        private void WhiteIpponPlus_Click(object sender, RoutedEventArgs e)
        {
            _whiteScore += 10;
            WhiteAfterScoreChange();
        }

        private void WhiteIpponMinus_Click(object sender, RoutedEventArgs e)
        {
            _whiteScore -= 10;
            WhiteAfterScoreChange();
        }

        private void WhiteAfterScoreChange()
        {
            WhiteScore.Text = _whiteScore.ToString();
            CheckWinner();
            if (_externalWindow != null)
            {
                _externalWindow.WhiteScore_external.Text = _whiteScore.ToString();
            }
        }

        #endregion

        private void ResetApp_Click(object sender, RoutedEventArgs e)
        {
            string resetScore = "7";
            ResetTextboxBorder();
            _externalWindow.Result_external.Text = "";

            //White
            ResetTextboxBorder();
            _externalWindow.WhiteScore_external.Text = resetScore;
            WhiteScore.Text = resetScore;
            _whiteScore = 7;

            //Blue
            ResetTextboxBorder();
            _externalWindow.BlueScore_external.Text = resetScore;
            BlueScore.Text = resetScore;
            _blueScore = 7;
        }
    }
}
