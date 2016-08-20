using System;
using System.Windows;
using System.Windows.Controls;
using XPRES.Departments.Inventory.ViewModels;

namespace XPRES.Departments.Inventory.Views.Controls
{
    /// <summary>
    /// Interaction logic for CycoCountStackControl.xaml
    /// </summary>
    public partial class CycoReviewControl : UserControl
    {
        public CycoReviewControl()
        {
            InitializeComponent();
        }

        private void BtnViewReviewSheet_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdCenter.Children)
            {
                _grd.Visibility = _grd.Name == "GrdSheet" ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void BtnViewFirstReview_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdCenter.Children)
            {
                _grd.Visibility = _grd.Name == "GrdStack" ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void DataGrid_OnCurrentCellChanged(object sender, EventArgs e)
        {
            CycoReviewVm _vm = DataContext as CycoReviewVm;
            _vm?.CalcDif();
        }

        private void BtnAddWritein_OnClick(object sender, RoutedEventArgs e)
        {
            TxtWriteinPid.Clear();
            TxtWriteinCount.Clear();
            TxtWriteinLoc.Clear();
        }
    }
}
