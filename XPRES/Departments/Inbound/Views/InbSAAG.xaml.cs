using System.Windows;
using System.Windows.Controls;
using XPRES.Main.Views;

namespace XPRES.Departments.Inbound.Views
{
    /// <summary>
    /// Interaction logic for InbSchedule.xaml
    /// </summary>
    public partial class InbSAAG : Window
    {
        #region Constructor

        public InbSAAG()
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

        private void btnLtlSched_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdSubMenu.Children)
                _grd.Visibility = _grd.Name == "GrdSchedMenu" ? Visibility.Visible : Visibility.Hidden;
            foreach (Grid _grd in GrdSchedInner.Children)
                _grd.Visibility = _grd.Name == "GrdLtl" ? Visibility.Visible : Visibility.Hidden;

            BtnGetLtlSchedule.Visibility = Visibility.Visible;
            BtnGetSmlPkSchedule.Visibility = Visibility.Hidden;
        }

        private void btnSmlPkSched_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdSubMenu.Children)
                _grd.Visibility = _grd.Name == "GrdSchedMenu" ? Visibility.Visible : Visibility.Hidden;
            foreach (Grid _grd in GrdSchedInner.Children)
                _grd.Visibility = _grd.Name == "GrdSmallPk" ? Visibility.Visible : Visibility.Hidden;

            BtnGetLtlSchedule.Visibility = Visibility.Hidden;
            BtnGetSmlPkSchedule.Visibility = Visibility.Visible;
        }

        private void btnCarrEdit_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdSubMenu.Children)
                _grd.Visibility = _grd.Name == "GrdCarrEditMenu" ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion Button Events

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdSubMenu.Children) _grd.Visibility = Visibility.Hidden;
            foreach (Grid _grd in GrdSchedInner.Children) _grd.Visibility = Visibility.Hidden;
        }

        #endregion Window Events
    }
}