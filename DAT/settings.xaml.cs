using DAT.Models;
using System.Windows;

namespace DAT
{
    /// <summary>
    /// Interaction logic for settings.xaml
    /// </summary>
    public partial class settings : Window
    {
        public ColorEnum Color { get; set; } = ColorEnum.Blue;
        public string MatchTime { get; set; }

        public settings()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {            
            MatchTime = matchTid.Text;
            if (Röd.IsChecked.HasValue && Röd.IsChecked.Value)
            {
                Color = ColorEnum.Red;
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
