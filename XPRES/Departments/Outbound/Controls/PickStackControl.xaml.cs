using System.Windows.Controls;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.Departments.Outbound.ViewModels;

namespace XPRES.Departments.Outbound.Controls
{
    /// <summary>
    /// Interaction logic for PickStackControl.xaml
    /// </summary>
    public partial class PickStackControl : UserControl
    {
        public PickStackControl()
        {
            InitializeComponent();
        }

        private void DeletePickEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                @"Are you sure you want to delete this schedule entry?", @"Delete Schedule Entry",
                System.Windows.Forms.MessageBoxButtons.YesNo);
            if (_diag == System.Windows.Forms.DialogResult.Yes)
            {
                Visibility = System.Windows.Visibility.Collapsed;
                var _vm = DataContext as PickStackCtrlVm;
                _vm.DeletePick();
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}