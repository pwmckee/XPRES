using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using XPRES.DAL;
using XPRES.Departments.Manager;
using XPRES.Departments.Replen.Controls;
using XPRES.Main.Views;

namespace XPRES.Departments.Replen.Views
{
    /// <summary>
    /// Interaction logic for RepSAAG.xaml
    /// </summary>
    public partial class RepSAAG : Window
    {
        private XpresEntities xps;
        private DateTime selDate;
        private DateTime tomorrow;

        public RepSAAG()
        {
            InitializeComponent();
            xps = new XpresEntities();
            selDate = new DateTime();
            tomorrow = new DateTime();
            getRepl(selDate);
        }

        private void getRepl(DateTime selDate)
        {
            if (spRepStackPanel.Children.Count > 0)
                spRepStackPanel.Children.Clear();

            tomorrow = selDate.AddDays(1);
            List<ReplenSAAG> _repl = new List<ReplenSAAG>();
            var repl = (from a in xps.ReplenSAAGs
                        where a.TimeStamp >= selDate && a.TimeStamp < tomorrow
                        orderby a.Employee
                        select a).ToList();
            _repl = repl;

            foreach (var r in _repl)
            {
                RepSaagStackControl _stkCtrl = new RepSaagStackControl();
                _stkCtrl.Name = r.CtrlID;
                _stkCtrl.lblName.Content = r.Employee;
                if (r.LK00001 != null)
                    _stkCtrl.lblLK.Content = r.LK00001.ToString();
                if (r.KARDEX != null)
                    _stkCtrl.lblKDX.Content = r.KARDEX.ToString();
                if (r.PL != null)
                    _stkCtrl.lblPL.Content = r.PL.ToString();
                if (r.HUBSECURE != null)
                    _stkCtrl.lblHSEC.Content = r.HUBSECURE.ToString();

                _stkCtrl.grdEdit.Visibility = Visibility.Hidden;
                _stkCtrl.grdDisp.Visibility = Visibility.Visible;
                spRepStackPanel.Children.Add(_stkCtrl);
                spRepStackPanel.UpdateLayout();
            }
        }

        private void btnEditRunners_Click(object sender, RoutedEventArgs e)
        {
            string Dept = "Replen";
            foreach (Window wnd in Application.Current.Windows)
            {
                if (wnd is EmployeeControl)
                {
                    wnd.Close();
                }
            }
            EmployeeControl _empCtrl = new EmployeeControl(Dept);
            _empCtrl.Show();
        }

        private void btnAddRepl_Click(object sender, RoutedEventArgs e)
        {
            RepSaagStackControl _repCtrl = new RepSaagStackControl();
            spRepStackPanel.Children.Add(_repCtrl);
        }

        private void btnViewDate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime.TryParse(dtpViewDate.SelectedDate.ToString(), out selDate);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Invalid Date Entered");
            }
            getRepl(selDate);
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is MainWindow)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                MainWindow home = new MainWindow();
                home.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "XPRES")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        private void btnDash_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is RepDash)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                RepDash dash = new RepDash();
                dash.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Replen Dash")
                    {
                        wnd.Activate();
                    }
                }
            }
        }
    }
}