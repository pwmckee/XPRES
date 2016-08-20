using System.Windows;

namespace XPRES.Departments.Replen.Views
{
    /// <summary>
    /// Interaction logic for EditProdAreas.xaml
    /// </summary>
    public partial class EditProdAreas : Window
    {
        public EditProdAreas()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}