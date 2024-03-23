using System;
using System.Windows;
using System.Windows.Media;

namespace DAT
{
    public partial class MainWindow : Window
    {
        private const string _blueWin = "Blå vinner";
        private const string _whiteWin = "Vit vinner";
        private const string _draw = "Oavgjort";
        private int _blueScore = 7;
        private int _whiteScore = 7;
        private ExternalWindow _externalWindow;
        private readonly Thickness fatBorder = new Thickness(4.0);
        private readonly Thickness lightBorder = new Thickness(2.0);


        public MainWindow()
        {
            InitializeComponent();
            _externalWindow = new ExternalWindow();
            Clock_label.Text = _startTimeDisplay;
            _externalWindow.BlueScore_external.Text = _blueScore.ToString();
            _externalWindow.WhiteScore_external.Text = _whiteScore.ToString();
            _externalWindow.Clock_external.Text = _startTimeDisplay;

            //update label text            
            _clockTimer.Elapsed += OnTimeChanged;
            Closing += OnClose;
        }

        private void CheckWinner()
        {
            if (_externalWindow != null)
            {
                if (_blueScore - _whiteScore >= 30 || _whiteScore - _blueScore >= 30 || MustStop)
                {
                    CheckWinningScore();
                }
            }
        }
        #region WinnerLogic

        private void CheckWinningScore()
        {
            ResetTextboxBorder();
            var winner = "";
            if (_blueScore > _whiteScore)
            {
                winner = _blueWin;
                _externalWindow.Result_external.Background = Brushes.Red;
                _externalWindow.BlueScore_external.BorderThickness = fatBorder;
                SetWinnerTextboxBorder();
            }

            if (_whiteScore > _blueScore)
            {
                winner = _whiteWin;
                _externalWindow.Result_external.Background = Brushes.Red;
                _externalWindow.WhiteScore_external.BorderThickness = fatBorder;
                SetWinnerTextboxBorder();
            }

            if (_whiteScore == _blueScore)
            {
                winner = _draw;
                _externalWindow.Result_external.BorderBrush = Brushes.Red;
                _externalWindow.WhiteScore_external.BorderBrush = Brushes.Red;
                _externalWindow.BlueScore_external.BorderBrush = Brushes.Red;
                _externalWindow.BlueScore_external.BorderThickness = fatBorder;
                _externalWindow.WhiteScore_external.BorderThickness = fatBorder;
            }
            _externalWindow.Result_external.Text = winner;
        }

        private void SetWinnerTextboxBorder()
        {
            _externalWindow.Result_external.BorderBrush = Brushes.Red;
            _externalWindow.Result_external.BorderThickness = fatBorder;
        }

        #endregion

        private void ResetTextboxBorder()
        {
            _externalWindow.Result_external.BorderBrush = Brushes.Black;
            _externalWindow.Result_external.BorderThickness = fatBorder;

            _externalWindow.WhiteScore_external.BorderBrush = Brushes.Black;
            _externalWindow.WhiteScore_external.BorderThickness = fatBorder;

            _externalWindow.BlueScore_external.BorderBrush = Brushes.Black;
            _externalWindow.BlueScore_external.BorderThickness = fatBorder;
        }


        #region Clock

        private const string _startTimeDisplay = "2:00";
        private readonly System.Timers.Timer _clockTimer = new System.Timers.Timer(1000.0);
        private static int MaxTime => 2 * 60;
        private bool MustStop => (MaxTime - ClockTicks) < 0;
        private int ClockTicks { get; set; }
        public TimeSpan TimeLeft =>
           (MaxTime - ClockTicks) > 0
           ? TimeSpan.FromSeconds(MaxTime - ClockTicks)
           : TimeSpan.FromMilliseconds(0);

        private void OnTimeChanged(object sender, EventArgs e)
        {
            ClockTicks += 1;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (MustStop)
                {
                    CheckWinner();
                }

                Clock_label.Text = TimeLeft.ToString(@"m\:ss");

                if (_externalWindow != null)
                {
                    _externalWindow.Clock_external.Text = TimeLeft.ToString(@"m\:ss");
                }
            });
        }

        private void StartClock_Click(object sender, RoutedEventArgs e)
        {
            _clockTimer.Start();
        }

        private void StopClock_Click(object sender, RoutedEventArgs e)
        {
            _clockTimer.Stop();
        }

        private void ResetClock_Click(object sender, RoutedEventArgs e)
        {
            _clockTimer.Stop();
            Clock_label.Text = _startTimeDisplay;
            _externalWindow.Clock_external.Text = _startTimeDisplay;
            ClockTicks = 0;
            ResetTextboxBorder();
        }

        #endregion

        #region HolderTimer

        private int HolderTimerTicks { get; set; }
        private const string _holderReset = ":00";
        private int HolderWazari => 10;
        private int HolderIppon => 20;

        private readonly System.Timers.Timer _holdTimer = new System.Timers.Timer(1000.0);

        private void BlueHoldStart_Click(object sender, RoutedEventArgs e)
        {
            WhiteHoldReset_Click(sender, e);
            _holdTimer.Elapsed += OnBlueHoldTimerChanged;
            _holdTimer.Start();
        }

        private void BlueHoldStop_Click(object sender, RoutedEventArgs e)
        {
            _holdTimer.Stop();
        }

        private void BlueHoldReset_Click(object sender, RoutedEventArgs e)
        {
            _holdTimer.Stop();
            _holdTimer.Elapsed -= OnBlueHoldTimerChanged;
            _externalWindow.HoldTimerBlueExt.Text = "";
            _externalWindow.HoldScoreTypeBlueExt.Text = "";
            BlueHoldTimer.Text = _holderReset;
            HolderTimerTicks = 0;
        }

        private void OnBlueHoldTimerChanged(object sender, EventArgs e)
        {
            HolderTimerTicks += 1;
            var holdScoreType = CheckHoldScoreType(HolderTimerTicks);
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (MustStop)
                {
                    _holdTimer.Stop();
                }
                var currentHoldTime = ":" + HolderTimerTicks.ToString();
                BlueHoldTimer.Text = currentHoldTime;
                if (_externalWindow != null)
                {
                    _externalWindow.HoldTimerBlueExt.Visibility = Visibility.Visible;
                    _externalWindow.HoldTimerBlueExt.Text = currentHoldTime;
                    _externalWindow.HoldScoreTypeBlueExt.Text = holdScoreType;

                    if (!string.IsNullOrEmpty(holdScoreType))
                    {
                        _externalWindow.HoldScoreTypeBlueExt.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _externalWindow.HoldScoreTypeBlueExt.Visibility = Visibility.Hidden;
                    }
                }
            });
        }

        private void WhiteHoldStart_Click(object sender, RoutedEventArgs e)
        {
            BlueHoldReset_Click(sender, e);
            _holdTimer.Elapsed += OnWhiteHoldTimerChanged;
            _holdTimer.Start();
        }

        private void WhiteHoldStop_Click(object sender, RoutedEventArgs e)
        {
            _holdTimer.Stop();
        }

        private void WhiteHoldReset_Click(object sender, RoutedEventArgs e)
        {
            _holdTimer.Stop();
            _holdTimer.Elapsed -= OnWhiteHoldTimerChanged;
            _externalWindow.HoldTimerWhiteExt.Text = "";
            _externalWindow.HoldScoreTypeWhiteExt.Text = "";
            WhiteHoldTimer.Text = _holderReset;
            HolderTimerTicks = 0;
        }

        private void OnWhiteHoldTimerChanged(object sender, EventArgs e)
        {
            HolderTimerTicks += 1;
            var holdScoreType = CheckHoldScoreType(HolderTimerTicks);
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (MustStop)
                {
                    _holdTimer.Stop();
                }
                var currentHoldTime = ":" + HolderTimerTicks.ToString();
                WhiteHoldTimer.Text = currentHoldTime;
                if (_externalWindow != null)
                {
                    _externalWindow.HoldTimerWhiteExt.Text = currentHoldTime;

                    if (!string.IsNullOrEmpty(holdScoreType))
                    {
                        _externalWindow.HoldScoreTypeWhiteExt.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _externalWindow.HoldScoreTypeWhiteExt.Visibility = Visibility.Hidden;
                    }
                    _externalWindow.HoldScoreTypeWhiteExt.Text = holdScoreType;
                }
            });
        }

        private string CheckHoldScoreType(int holdTime)
        {
            if (holdTime >= HolderWazari && holdTime < HolderIppon)
            {
                return "Wazari";
            }
            if (holdTime >= HolderIppon)
            {
                return "Ippon";
            }
            return "";
        }


        #endregion

        private void VisaExtern_Click(object sender, RoutedEventArgs e)
        {
            _externalWindow ??= new ExternalWindow();

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
            ResetClock_Click(sender, e);
            ResetTextboxBorder();
            _externalWindow.Result_external.Text = "";
            _externalWindow.Result_external.Foreground = Brushes.Black;
            _externalWindow.Result_external.Background = Brushes.White;

            //White
            WhiteHoldReset_Click(sender, e);
            _externalWindow.WhiteScore_external.Text = resetScore;
            WhiteScore.Text = resetScore;
            _whiteScore = 7;

            //Blue
            BlueHoldReset_Click(sender, e);
            _externalWindow.BlueScore_external.Text = resetScore;
            BlueScore.Text = resetScore;
            _blueScore = 7;
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
