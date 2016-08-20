using System.Windows;

namespace XPRES.Departments.Manager
{
    /// <summary>
    /// Interaction logic for ManagerFunctions.xaml
    /// </summary>
    public partial class ManagerFunctions : Window
    {
        private bool open;

        public ManagerFunctions()
        {
            InitializeComponent();
            open = false;
        }

        private void btnAddEmp_Click(object sender, RoutedEventArgs e)
        {
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is AddEmployee)
                {
                    open = true;
                }
            }
            if (!open)
            {
                AddEmployee _addemp = new AddEmployee();
                _addemp.Show();
                Close();
            }
        }

        private void btnEditEmp_Click(object sender, RoutedEventArgs e)
        {
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is EditEmployees)
                {
                    open = true;
                }
            }
            if (!open)
            {
                EditEmployees _editEmp = new EditEmployees();
                _editEmp.Show();
                Close();
            }
        }
    }
}