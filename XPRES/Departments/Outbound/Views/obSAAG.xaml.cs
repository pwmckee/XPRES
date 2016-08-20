using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;
using XPRES.Departments.Manager;
using XPRES.Departments.Outbound.Controls;
using XPRES.Main.Views;

namespace XPRES.Departments.Outbound.Views
{
    /// <summary>
    /// Interaction logic for PickBoard.xaml
    /// </summary>
    public partial class obSAAG : Window
    {
        public obSAAG()
        {
            InitializeComponent();
        }

        #region Button Events

        #region Main Menu

        private void btnEditPickers_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window wnd in Application.Current.Windows)
            {
                if (wnd is EmployeeControl)
                    wnd.Close();
            }

            string Dept = "OutBound";
            EmployeeControl EmpCtrl = new EmployeeControl(Dept);
            EmpCtrl.Show();
        }

        #endregion Main Menu

        #region Sub Menu

        private void btnCreateMultiPick_Click(object sender, RoutedEventArgs e)
        {
            grdMultiPick.Visibility = grdMultiPick.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
            grdFinishedPicks.Visibility = grdFinishedPicks.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        private void btnStartMulti_Click(object sender, RoutedEventArgs e)
        {
            grdFinishedPicks.Visibility = Visibility.Visible;
            grdMultiPick.Visibility = Visibility.Hidden;
        }

        private void btnEndPick_Click(object sender, RoutedEventArgs e)
        {
            grdFinishedPicks.Visibility = grdFinishedPicks.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
            grdMultiPick.Visibility = grdMultiPick.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        #endregion Sub Menu

        #endregion Button Events

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
                if (wnd is MainWindow) _open = true;

            if (!_open)
            {
                MainWindow home = new MainWindow();
                home.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                    if (wnd.Title == "XPRES") wnd.Activate();
            }
        }

        private void btnDash_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
                if (wnd is obDash) _open = true;

            if (!_open)
            {
                obDash dash = new obDash();
                dash.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                    if (wnd.Title == "Outbound Dashboard") wnd.Activate();
            }
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            spEditPickerList.Visibility = spEditPickerList.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}