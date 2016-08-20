using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;
using XPRES.Departments.Inbound.Controls;
using XPRES.Helpers;
using XPRES.Main.Views;
using MessageBox = System.Windows.Forms.MessageBox;

namespace XPRES.Departments.Inbound.Views
{
    /// <summary>
    /// Interaction logic for InbMetrics.xaml
    /// </summary>
    public partial class InbMetrics : Window
    {
        private string SpName { get; set; }
        private string GrdName { get; set; }
        private List<DateTime?> CtrlSets { get; set; }

        public InbMetrics()
        {
            InitializeComponent();
            FillCtrlSets();
        }

        private void FillCtrlSets()
        {
            try
            {
                List<DateTime?> _sets = (from _a in new XpresEntities().DynamicInbMetrics
                                         where _a.State == true
                                         select _a.Date).Distinct().ToList();
                CtrlSets = _sets;
                CboCtrlSets.ItemsSource = CtrlSets;
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error retrieving dynamic metric sets: " + _ex.Message);
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
                MainWindow _home = new MainWindow();
                _home.Show();
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
                if (_wnd is InbDash)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                InbDash _dash = new InbDash();
                _dash.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Inbound Dashboard")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        private void btnEditCtrl_Click(object sender, RoutedEventArgs e)
        {
            SpName = "SpEditControl";
            GrdName = "GrdCtrlEdit";
            MainMenuViewControl(SpName, GrdName);
        }

        private void btnViewDeptMenu_Click(object sender, RoutedEventArgs e)
        {
            SpName = "SpViewMetrics";
            GrdName = "GrdMetricsGrid";
            MainMenuViewControl(SpName, GrdName);
        }

        private void btnEnterDeptMenu_Click(object sender, RoutedEventArgs e)
        {
            SpName = "SpEnterMetrics";
            GrdName = "GrdMetricsGrid";
            MainMenuViewControl(SpName, GrdName);
        }

        private void btnAddDyn_Click(object sender, RoutedEventArgs e)
        {
            DynContainer _con = new DynContainer();
            SpEditDyn.Children.Add(_con);
            SpEditDyn.UpdateLayout();
        }
        
        private void MainMenuViewControl(string spName, string grdName)
        {
            foreach (StackPanel _sp in GrdSubMenu.Children)
            {
                if (_sp.Name == spName)
                {
                    _sp.Visibility = Visibility.Visible;
                }
                else
                {
                    _sp.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid _grd in GrdCenterView.Children)
            {
                if (_grd.Name == grdName)
                {
                    _grd.Visibility = Visibility.Visible;
                }
                else
                {
                    _grd.Visibility = Visibility.Hidden;
                }
            }
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel _sp in GrdSubMenu.Children)
            {
                _sp.Visibility = Visibility.Hidden;
            }
            foreach (Grid _grd in GrdCenterView.Children)
            {
                _grd.Visibility = Visibility.Hidden;
            }
        }
    }
}