using System.Windows;
using XPRES.Main.Views;

namespace XPRES.Departments.Inbound.Views
{
    /// <summary>
    /// Interaction logic for InbActivity.xaml
    /// </summary>
    public partial class InbActivity : Window
    {
        #region Constructor

        public InbActivity()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Button Events

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is MainWindow)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                MainWindow home = new MainWindow();
                home.Show();
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

        private void btnDash_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is InbDash)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                InbDash dash = new InbDash();
                dash.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Inbound Dashboard")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        private void btnEditOperatorMenu_Click(object sender, RoutedEventArgs e)
        {
            SpEditOperControls.Visibility = SpEditOperControls.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion Button Events
    }
}
