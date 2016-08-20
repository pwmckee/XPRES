using System.Windows;
using System.Windows.Controls;

namespace XPRES.Departments.Inbound.Controls
{
    public partial class DynamicMetric : UserControl
    {
        public DynamicMetric()
        {
            InitializeComponent();
            string _ctrlName = Name;
            TxbCtrlName.Text = _ctrlName;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            GrdAlter.Visibility = Visibility.Hidden;
            GrdView.Visibility = Visibility.Visible;
        }

        private void btnAlter_Click(object sender, RoutedEventArgs e)
        {
            GrdAlter.Visibility = Visibility.Visible;
            GrdView.Visibility = Visibility.Hidden;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}