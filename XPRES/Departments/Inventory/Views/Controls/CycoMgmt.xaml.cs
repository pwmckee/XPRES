using System.Windows;
using System.Windows.Controls;

namespace XPRES.Departments.Inventory.Views.Controls
{
    /// <summary>
    /// Interaction logic for CycoMgmt.xaml
    /// </summary>
    public partial class CycoMgmt : UserControl
    {
        public CycoMgmt()
        {
            InitializeComponent();
        }

        private void BtnCreateCounts_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdCenterView.Children)
            {
                _grd.Visibility = _grd.Name == "GrdCreate" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void BtnFirstCounts_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdCenterView.Children)
            {
                _grd.Visibility = _grd.Name == "GrdCountEntries" ? Visibility.Visible : Visibility.Collapsed;
            }
            foreach (Grid _grd in GrdCenterCtrls.Children)
            {
                _grd.Visibility = _grd.Name == "GrdStack" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void BtnLoadCount_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdCenterCtrls.Children)
            {
                _grd.Visibility = _grd.Name == "GrdSheet" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void BtnFirstCountExport_OnClick(object sender, RoutedEventArgs e)
        {
            TxtCountId.Text = "";
            TxtName.Text = "";
        }

        private void BtnSubmitFirst_OnClick(object sender, RoutedEventArgs e)
        {
            TxtCountId.Text = "";
            TxtName.Text = "";
        }

        private void BtnViewSheet_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdCenterCtrls.Children)
            {
                _grd.Visibility = _grd.Name == "GrdSheet" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void BtnAddWritein_OnClick(object sender, RoutedEventArgs e)
        {
            TxtWriteinCount.Text = "";
            TxtWriteinLoc.Text = "";
            TxtWriteinPid.Text = "";
        }

        private void BtnReviewCounts_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Grid _grd in GrdCenterView.Children)
            {
                _grd.Visibility = _grd.Name == "GrdReview" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}