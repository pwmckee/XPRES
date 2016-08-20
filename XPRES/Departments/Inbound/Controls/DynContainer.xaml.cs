using System.Windows;
using System.Windows.Controls;
using XPRES.Departments.Inbound.ViewModels;

namespace XPRES.Departments.Inbound.Controls
{
    /// <summary>
    /// Interaction logic for DynContainer.xaml
    /// </summary>
    public partial class DynContainer : UserControl
    {
        public DynContainer()
        {            
            InitializeComponent();
            LblDate.Content = "Date";
            TxbName.Text = "Date";
            
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}