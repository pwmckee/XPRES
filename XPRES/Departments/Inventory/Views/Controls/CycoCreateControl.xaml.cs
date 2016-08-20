using System.Windows;
using System.Windows.Controls;

namespace XPRES.Departments.Inventory.Views.Controls
{
    /// <summary>
    /// Interaction logic for CycoCreateControl.xaml
    /// </summary>
    public partial class CycoCreateControl : UserControl
    {
        public CycoCreateControl()
        {
            InitializeComponent();
        }

        private void BtnFirstCountCreate_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (DataGrid _dg in GrdCountSheet.Children)
            {
                _dg.Visibility = _dg.Name == "DgCreateSheet" ?  Visibility.Visible : Visibility.Hidden;
            }
        }

        private void CbxQuarter_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (DataGrid _dg in GrdCountSheet.Children)
            {
                _dg.Visibility = _dg.Name == "DgMainSchedView" ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void BtnFirstCountSave_OnClick(object sender, RoutedEventArgs e)
        {
            TxtCountFrom.Clear();
            TxtCountTo.Clear();
        }
    }
}
