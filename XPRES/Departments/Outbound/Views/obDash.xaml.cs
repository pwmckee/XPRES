using System.Windows;

namespace XPRES.Departments.Outbound.Views
{
    /// <summary>
    /// Interaction logic for obDash.xaml
    /// </summary>
    public partial class obDash : Window
    {
        public obDash()
        {
            InitializeComponent();
        }

        private void btnObSched_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is obSAAG)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                obSAAG _saag = new obSAAG();
                _saag.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Picking SAAG")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        //private void btnObMetrics_Click(object sender, RoutedEventArgs e)
        //{
        //}

        //private void btnObAudit_Click(object sender, RoutedEventArgs e)
        //{
        //}

        //private void btnObResources_Click(object sender, RoutedEventArgs e)
        //{
        //}
    }
}