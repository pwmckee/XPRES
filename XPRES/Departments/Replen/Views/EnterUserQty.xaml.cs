using System.Windows;

namespace XPRES.Departments.Replen.Views
{
    /// <summary>
    /// Interaction logic for EnterUserQty.xaml
    /// </summary>
    public partial class EnterUserQty : Window
    {
        public EnterUserQty()
        {
            InitializeComponent();
            cboRequestQty.Items.Add("1");
            cboRequestQty.Items.Add("2");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}