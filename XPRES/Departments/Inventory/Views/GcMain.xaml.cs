using System.Windows;
using System.Windows.Controls;
using XPRES.Departments.Inventory.GeoCounts;
using XPRES.Departments.Inventory.Views.Controls;
using XPRES.Main.Views;

namespace XPRES.Departments.Inventory.Views
{
    /// <summary>
    /// Interaction logic for GcMain.xaml
    /// </summary>
    public partial class GcMain : Window
    {
        public GcMain()
        {
            InitializeComponent();
        }
        
        private void BtnCountSched_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is GeoCountsSchedule)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                GeoCountsSchedule _sched = new GeoCountsSchedule();
                _sched.Show();
            }
        }

        private void BtnCountMetrics_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is GcMetrics)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                GcMetrics _nameVar = new GcMetrics();
                _nameVar.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "GeoCountsMetrics")
                    {
                        wnd.Activate();
                    }
                }
            }
        }
        
        private void BtnCcTracker_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (UserControl _ctrl in GrdViewCtrls.Children)
            {
                if (_ctrl is CycoTracker)
                {
                    _ctrl.Visibility = Visibility.Visible;
                }
                else
                {
                    _ctrl.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BtnCountMgmt_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (UserControl _ctrl in GrdViewCtrls.Children)
            {
                if (_ctrl is CycoMgmt)
                {
                    _ctrl.Visibility = Visibility.Visible;
                }
                else
                {
                    _ctrl.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is MainWindow)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                MainWindow _nameVar = new MainWindow();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "XPRES")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        private void btnDash_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is InvMain)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                InvMain _nameVar = new InvMain();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Inventory Dashboard")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }
    }
}
