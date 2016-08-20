using System.Windows;
using XPRES.Main.Views;

namespace XPRES.Departments.Replen.Views
{
    /// <summary>
    /// Interaction logic for RepDash.xaml
    /// </summary>
    public partial class RepDash : Window
    {
        public RepDash()
        {
            InitializeComponent();
        }

        private void btnRepSched_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnRepResources_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (Window wnd in Application.Current.Windows)
            {
                if (wnd is MainWindow)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                MainWindow _home = new MainWindow();
                _home.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "XPRES")
                    {
                        wnd.Activate();
                    }
                }
            }
        }
    }
}