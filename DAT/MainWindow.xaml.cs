﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DAT
{
    public partial class MainWindow : Window
    {
        private int _blueScore = 7;
        private int _whiteScore = 7;
        private ExternalWindow _externalWindow;
        private readonly Thickness fatBorder = new(4.0);
        private readonly Thickness lightBorder = new(2.0);


        public MainWindow()
        {
            InitializeComponent();
            _externalWindow = new ExternalWindow();
            Clock_label.Text = _startTimeDisplay;
            _externalWindow.BlueScore_external.Text = _blueScore.ToString();
            _externalWindow.WhiteScore_external.Text = _whiteScore.ToString();
            _externalWindow.Clock_external.Text = _startTimeDisplay;
            BlueScore.Background = new SolidColorBrush(Color.FromArgb(255, 27, 73, 242));

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
            _externalWindow.Result_external.Visibility = Visibility.Hidden;
            var trophyImg = new BitmapImage(new Uri("Content/trophy.png", UriKind.Relative));
            if (_blueScore > _whiteScore)
            {
                _externalWindow.BlueScore_external.BorderThickness = fatBorder;
                _externalWindow.BlueScore_external.BorderBrush = Brushes.Red;
                _externalWindow.BlueImage.Source = trophyImg;
            }

            if (_whiteScore > _blueScore)
            {
                _externalWindow.WhiteScore_external.BorderThickness = fatBorder;
                _externalWindow.WhiteScore_external.BorderBrush = Brushes.Red;
                _externalWindow.WhiteImage.Source = trophyImg;
            }

            if (_whiteScore == _blueScore)
            {
                var scalesImg = new BitmapImage(new Uri("Content/scales.png", UriKind.Relative));
                _externalWindow.BlueImage.Source = scalesImg;
                _externalWindow.WhiteImage.Source = scalesImg;
                _externalWindow.WhiteScore_external.BorderBrush = Brushes.Red;
                _externalWindow.BlueScore_external.BorderBrush = Brushes.Red;
                _externalWindow.BlueScore_external.BorderThickness = fatBorder;
                _externalWindow.WhiteScore_external.BorderThickness = fatBorder;
                _externalWindow.Result_external.Text = "Oavgjort";
                _externalWindow.Result_external.Visibility = Visibility.Visible;
            }
        }

        #endregion

        private void ResetTextboxBorder()
        {
            _externalWindow.WhiteScore_external.BorderBrush = Brushes.Black;
            _externalWindow.WhiteScore_external.BorderThickness = lightBorder;

            _externalWindow.BlueScore_external.BorderBrush = Brushes.Black;
            _externalWindow.BlueScore_external.BorderThickness = lightBorder;
        }


        #region Clock

        private const string _startTimeDisplay = "2:00";
        private readonly System.Timers.Timer _clockTimer = new System.Timers.Timer(1000.0);
        private int MaxTime { get; set; }
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
           var time = Clock_label.Text.Split(':');
            if(time.Length < 2)
            {
                Clock_label.Text = _startTimeDisplay;
                //Two minutes
                MaxTime = 120;
            }
            else
            {
                var seconds = 0;
                //Parse the minutes and seconds
                if (int.TryParse(time[0], out int mins))
                    seconds = mins * 60;
                if (int.TryParse(time[1], out int secs))
                    seconds += secs;
                
                MaxTime = seconds;
                _externalWindow.Clock_external.Text = Clock_label.Text;
            }

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
        private static int HolderWazari => 10;
        private static int HolderIppon => 20;

        private readonly System.Timers.Timer _holdTimer = new System.Timers.Timer(1000.0);
        #region Blue 
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
        #endregion

        #region White

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

        #endregion

        private static string CheckHoldScoreType(int holdTime)
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
            _externalWindow = _externalWindow ?? new ExternalWindow();
            ResetClock_Click(sender, e);
            ResetTextboxBorder();
            _externalWindow.Result_external.Visibility = Visibility.Hidden;

            //White
            WhiteHoldReset_Click(sender, e);
            _externalWindow.WhiteScore_external.Text = resetScore;
            WhiteScore.Text = resetScore;
            _whiteScore = 7;
            _externalWindow.WhiteImage.Source = null;

            //Blue
            BlueHoldReset_Click(sender, e);
            _externalWindow.BlueScore_external.Text = resetScore;
            BlueScore.Text = resetScore;
            _blueScore = 7;
            _externalWindow.BlueImage.Source = null;
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
