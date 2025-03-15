using DAT.Models;
using System.Windows;

namespace DAT
{
    public partial class settings : Window
    {
        private CurrentSettings CurrentSettings { get; set; }

        public settings(CurrentSettings currentSettings)
        {            
            CurrentSettings = currentSettings;

            InitializeComponent();

            if (CurrentSettings.CurrentColor == ColorEnum.Red)
            {
                Röd.IsChecked = true;
            }
            else
            {
                Blå.IsChecked = true;
            }

            matchTid.Text = CurrentSettings.CurrentMatchLength;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {            
            var (isOk, _) = Utility.CheckAndConvertTime(matchTid.Text);
            if (isOk)
            {
                CurrentSettings.CurrentMatchLength = matchTid.Text;
            }

            if (Röd.IsChecked.HasValue && Röd.IsChecked.Value)
            {
                CurrentSettings.CurrentColor = ColorEnum.Red;
            }
            else
            {
                CurrentSettings.CurrentColor = ColorEnum.Blue;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {            
            this.DialogResult = false;
            this.Close();
        }

        private void Blå_Click(object sender, RoutedEventArgs e)
        {
            Röd.IsChecked = false;
        }

        private void Röd_Click(object sender, RoutedEventArgs e)
        {
            Blå.IsChecked = false;
        }
    }
}
