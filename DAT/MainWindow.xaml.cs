using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DAT
{
    public partial class MainWindow : Window
    {
        private int _altColorScore = 7;
        private int _primaryColorScore = 7;
        private ExternalWindow _externalWindow;
        private readonly Thickness fatBorder = new(4.0);
        private readonly Thickness lightBorder = new(2.0);

        private readonly System.Timers.Timer _altColorHoldTimer = new System.Timers.Timer(1000.0);
        private readonly System.Timers.Timer _primaryColorHoldTimer = new System.Timers.Timer(1000.0);
        private DispatcherTimer previewTimer;


        public MainWindow()
        {
            InitializeComponent();
            _externalWindow = new ExternalWindow();
            Clock_label.Text = _startTimeDisplay;
            _externalWindow.AltColorScore_external.Text = _altColorScore.ToString();
            _externalWindow.PrimaryColorScore_external.Text = _primaryColorScore.ToString();
            _externalWindow.Clock_external.Text = _startTimeDisplay;


            var textColor = new SolidColorBrush(Color.FromArgb(255, 0, 153, 51));
            PrimaryColorHoldScoreTypeMain.Foreground = textColor;
            AltColorHoldScoreTypeMain.Foreground = textColor;

            //update label text            
            _clockTimer.Elapsed += OnTimeChanged;
            Closing += OnClose;

            _altColorHoldTimer.Elapsed += OnAltColorHoldTimerChanged;
            _primaryColorHoldTimer.Elapsed += OnPrimaryColorHoldTimerChanged;

            previewTimer = new DispatcherTimer
            {
                Interval = System.TimeSpan.FromMilliseconds(250)
            };
            previewTimer.Tick += UpdatePreview;

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = await SettingsLoader.LoadSettingsAsync();

            SolidColorBrush background;

            if (settings.UseRedColor)
            {
                altColorHeader.Text = "Röd";
                background = new SolidColorBrush(Color.FromArgb(255, 204, 43, 29));
            }
            else
            {
                background = new SolidColorBrush(Color.FromArgb(255, 27, 73, 242));
            }
            
            _externalWindow ??= new ExternalWindow();

            _externalWindow.AltColorScore_external.Background = background;
            AltColorScore.Background = background;

            MatchLength = ConvertToSeconds(settings.MatchLength);
        }


        private void CheckWinner()
        {
            if (_externalWindow != null)
            {
                if (!_altColorHoldTimer.Enabled || _primaryColorHoldTimer.Enabled)
                {
                    if (_altColorScore - _primaryColorScore >= 30 || _primaryColorScore - _altColorScore >= 30 || MustStop)
                    {
                        CheckWinningScore();
                    }
                }
            }
        }


        #region WinnerLogic

        private void CheckWinningScore()
        {
            ResetTextboxBorder();
            _externalWindow.Result_external.Visibility = Visibility.Hidden;
            var trophyImg = new BitmapImage(new Uri("Content/trophy.png", UriKind.Relative));
            if (_altColorScore > _primaryColorScore)
            {
                _externalWindow.AltColorScore_external.BorderThickness = fatBorder;
                _externalWindow.AltColorScore_external.BorderBrush = Brushes.Red;
                _externalWindow.AltColorImage.Source = trophyImg;
            }

            if (_primaryColorScore > _altColorScore)
            {
                _externalWindow.PrimaryColorScore_external.BorderThickness = fatBorder;
                _externalWindow.PrimaryColorScore_external.BorderBrush = Brushes.Red;
                _externalWindow.PrimaryColorImage.Source = trophyImg;
            }

            if (_primaryColorScore == _altColorScore)
            {
                var scalesImg = new BitmapImage(new Uri("Content/scales.png", UriKind.Relative));
                _externalWindow.AltColorImage.Source = scalesImg;
                _externalWindow.PrimaryColorImage.Source = scalesImg;
                _externalWindow.PrimaryColorScore_external.BorderBrush = Brushes.Red;
                _externalWindow.AltColorScore_external.BorderBrush = Brushes.Red;
                _externalWindow.AltColorScore_external.BorderThickness = fatBorder;
                _externalWindow.PrimaryColorScore_external.BorderThickness = fatBorder;
                _externalWindow.Result_external.Text = "Oavgjort";
                _externalWindow.Result_external.Visibility = Visibility.Visible;
            }
        }

        #endregion

        private void ResetTextboxBorder()
        {
            _externalWindow.PrimaryColorScore_external.BorderBrush = Brushes.Black;
            _externalWindow.PrimaryColorScore_external.BorderThickness = lightBorder;

            _externalWindow.AltColorScore_external.BorderBrush = Brushes.Black;
            _externalWindow.AltColorScore_external.BorderThickness = lightBorder;
        }


        #region Clock

        private string _startTimeDisplay = "2:00";
        private readonly System.Timers.Timer _clockTimer = new System.Timers.Timer(1000.0);
        private int MatchLength { get; set; }
        private bool MustStop => (MatchLength - ClockTicks) < 0;
        private int ClockTicks { get; set; }
        public TimeSpan TimeLeft =>
           (MatchLength - ClockTicks) > 0
           ? TimeSpan.FromSeconds(MatchLength - ClockTicks)
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


        private int ConvertToSeconds(string time)
        {
            string pattern = @"^(\d?):(\d{2})$"; // Match "m:ss" or ":ss"
            Match match = Regex.Match(time, pattern);

            if (!match.Success)
            {
                return 120; // Invalid format, return default 120s
            }
            else
            {
                _externalWindow.Clock_external.Text = Clock_label.Text;

                int minutes = string.IsNullOrEmpty(match.Groups[1].Value) ? 0 : int.Parse(match.Groups[1].Value);
                int seconds = int.Parse(match.Groups[2].Value);

                return (minutes * 60) + seconds;
            }

        }


        private void StartClock_Click(object sender, RoutedEventArgs e)
        {

            MatchLength = ConvertToSeconds(Clock_label.Text);

            ClockTicks = 0;
            _clockTimer.Start();
        }

        private void StopClock_Click(object sender, RoutedEventArgs e)
        {
            _clockTimer.Stop();
            PrimaryColorHoldReset_Click(sender, e);
            AltColorHoldReset_Click(sender, e);
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


        #region AltColor 
        private void AltColorHoldStart_Click(object sender, RoutedEventArgs e)
        {
            PrimaryColorHoldReset_Click(sender, e);
            _altColorHoldTimer.Start();
        }

        private void AltColorHoldStop_Click(object sender, RoutedEventArgs e)
        {
            _altColorHoldTimer.Stop();
            CheckWinner();
        }

        private void AltColorHoldReset_Click(object sender, RoutedEventArgs e)
        {
            _altColorHoldTimer.Stop();
            _externalWindow.HoldTimerAltColorExt.Text = "";
            _externalWindow.HoldScoreTypeAltColorExt.Text = "";
            AltColorHoldScoreTypeMain.Text = "";
            AltColorHoldTimer.Text = _holderReset;
            HolderTimerTicks = 0;
        }

        private void OnAltColorHoldTimerChanged(object sender, EventArgs e)
        {
            HolderTimerTicks += 1;
            var holdScoreType = CheckHoldScoreType(HolderTimerTicks);
            Application.Current.Dispatcher.Invoke(() =>
            {
                var currentHoldTime = ":" + HolderTimerTicks.ToString();
                AltColorHoldTimer.Text = currentHoldTime;
                if (_externalWindow != null)
                {
                    _externalWindow.HoldTimerAltColorExt.Visibility = Visibility.Visible;
                    _externalWindow.HoldTimerAltColorExt.Text = currentHoldTime;
                    _externalWindow.HoldScoreTypeAltColorExt.Text = holdScoreType;
                    AltColorHoldScoreTypeMain.Text = holdScoreType;

                    if (!string.IsNullOrEmpty(holdScoreType))
                    {
                        _externalWindow.HoldScoreTypeAltColorExt.Visibility = Visibility.Visible;
                        AltColorHoldScoreTypeMain.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _externalWindow.HoldScoreTypeAltColorExt.Visibility = Visibility.Hidden;
                        AltColorHoldScoreTypeMain.Visibility = Visibility.Hidden;
                    }
                }
            });
        }
        #endregion

        #region PrimaryColor

        private void PrimaryColorHoldStart_Click(object sender, RoutedEventArgs e)
        {
            AltColorHoldReset_Click(sender, e);
            _primaryColorHoldTimer.Start();
        }

        private void PrimaryColorHoldStop_Click(object sender, RoutedEventArgs e)
        {
            _primaryColorHoldTimer.Stop();
            CheckWinner();
        }

        private void PrimaryColorHoldReset_Click(object sender, RoutedEventArgs e)
        {
            _primaryColorHoldTimer.Stop();
            _externalWindow.HoldTimerPrimaryColorExt.Text = "";
            _externalWindow.HoldScoreTypePrimaryColorExt.Text = "";
            PrimaryColorHoldScoreTypeMain.Text = "";
            PrimaryColorHoldTimer.Text = _holderReset;
            HolderTimerTicks = 0;
        }

        private void OnPrimaryColorHoldTimerChanged(object sender, EventArgs e)
        {
            HolderTimerTicks += 1;
            var holdScoreType = CheckHoldScoreType(HolderTimerTicks);
            Application.Current.Dispatcher.Invoke(() =>
            {
                var currentHoldTime = ":" + HolderTimerTicks.ToString();
                PrimaryColorHoldTimer.Text = currentHoldTime;
                if (_externalWindow != null)
                {
                    _externalWindow.HoldTimerPrimaryColorExt.Text = currentHoldTime;
                    _externalWindow.HoldScoreTypePrimaryColorExt.Text = holdScoreType;
                    PrimaryColorHoldScoreTypeMain.Text = holdScoreType;

                    if (!string.IsNullOrEmpty(holdScoreType))
                    {
                        _externalWindow.HoldScoreTypePrimaryColorExt.Visibility = Visibility.Visible;
                        PrimaryColorHoldScoreTypeMain.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _externalWindow.HoldScoreTypePrimaryColorExt.Visibility = Visibility.Hidden;
                        PrimaryColorHoldScoreTypeMain.Visibility = Visibility.Hidden;
                    }
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
        private void AltColorWazaPlus_Click(object sender, RoutedEventArgs e)
        {
            _altColorScore += 7;
            AltColorAfterScoreChange();
        }

        private void AltColorWazaMinus_Click(object sender, RoutedEventArgs e)
        {
            _altColorScore -= 7;
            AltColorAfterScoreChange();
        }

        private void AltColorIpponPlus_Click(object sender, RoutedEventArgs e)
        {
            _altColorScore += 10;
            AltColorAfterScoreChange();
        }

        private void AltColorIpponMinus_Click(object sender, RoutedEventArgs e)
        {
            _altColorScore -= 10;
            AltColorAfterScoreChange();
        }

        private void AltColorAfterScoreChange()
        {
            AltColorScore.Text = _altColorScore.ToString();
            CheckWinner();
            if (_externalWindow != null)
            {
                _externalWindow.AltColorScore_external.Text = _altColorScore.ToString();
            }
        }

        private void PrimaryColorWazaPlus_Click(object sender, RoutedEventArgs e)
        {
            _primaryColorScore += 7;
            PrimaryColorAfterScoreChange();
        }

        private void PrimaryColorWazaMinus_Click(object sender, RoutedEventArgs e)
        {
            _primaryColorScore -= 7;
            PrimaryColorAfterScoreChange();
        }

        private void PrimaryColorIpponPlus_Click(object sender, RoutedEventArgs e)
        {
            _primaryColorScore += 10;
            PrimaryColorAfterScoreChange();
        }

        private void PrimaryColorIpponMinus_Click(object sender, RoutedEventArgs e)
        {
            _primaryColorScore -= 10;
            PrimaryColorAfterScoreChange();
        }

        private void PrimaryColorAfterScoreChange()
        {
            PrimaryColorScore.Text = _primaryColorScore.ToString();
            CheckWinner();
            if (_externalWindow != null)
            {
                _externalWindow.PrimaryColorScore_external.Text = _primaryColorScore.ToString();
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

            //PrimaryColor
            PrimaryColorHoldReset_Click(sender, e);
            _externalWindow.PrimaryColorScore_external.Text = resetScore;
            PrimaryColorScore.Text = resetScore;
            _primaryColorScore = 7;
            _externalWindow.PrimaryColorImage.Source = null;

            //AltColor
            AltColorHoldReset_Click(sender, e);
            _externalWindow.AltColorScore_external.Text = resetScore;
            AltColorScore.Text = resetScore;
            _altColorScore = 7;
            _externalWindow.AltColorImage.Source = null;
        }

        private void UpdatePreview(object sender, System.EventArgs e)
        {
            if (AllowMiniature.IsChecked == true)
            {
                // Update the preview whenever the event is triggered
                if (_externalWindow != null && _externalWindow.IsVisible)
                {
                    var rtb = new RenderTargetBitmap(
                        (int)_externalWindow.ActualWidth,
                        (int)_externalWindow.ActualHeight,
                        96, 96,
                        System.Windows.Media.PixelFormats.Pbgra32);

                    rtb.Render(_externalWindow.Content as Visual);

                    PreviewImage.Source = rtb;
                }
            }
        }



        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void VisaMiniChanged(object sender, RoutedEventArgs e)
        {
            if (AllowMiniature.IsChecked == false)
            {
                PreviewImage.Source = null;
                previewTimer.Stop();
            }
            else
            {
                previewTimer.Start();
            }
        }

        private void AvslutaClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SettingsClicked(object sender, RoutedEventArgs e)
        {
            //TODO:Lägg in möjlighet att spara inställningar här
        }
    }
}
