using System.Windows.Controls;

namespace XPRES.Departments.Outbound.Controls
{
    /// <summary>
    /// Interaction logic for MultiAddControl.xaml
    /// </summary>
    public partial class MultiAddControl : UserControl
    {
        public MultiAddControl()
        {
            InitializeComponent();
        }

        private void DeletePickEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}