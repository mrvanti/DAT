
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DAT
{
    /// <summary>
    /// Interaction logic for DomarTecken.xaml
    /// </summary>
    public partial class DomarTecken : Window
    {
        public DomarTecken()
        {
            InitializeComponent();
            this.DomarTeckenImage.Source = new BitmapImage(new Uri("Content/domartecken_judo.png", UriKind.Relative));
        }

        private void StängClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
