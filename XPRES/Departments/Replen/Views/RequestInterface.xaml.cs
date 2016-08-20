using System.Windows;
using System.Windows.Controls;

namespace XPRES.Departments.Replen.Views
{
    /// <summary>
    /// Interaction logic for rpProdViewSubRequest.xaml
    /// </summary>
    public partial class RequestInterface : Window
    {
        public RequestInterface()
        {
            InitializeComponent();
            cboRequestQty.Items.Add("1");
            cboRequestQty.Items.Add("2");
        }

        private void cboRequestQty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtPartNum.Text = "";
            cboRequestQty.SelectedIndex = -1;
        }
    }
}