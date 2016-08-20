using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;
using XPRES.Departments.Inbound.ViewModels;

namespace XPRES.Departments.Inbound.Controls
{
    /// <summary>
    /// Interaction logic for InbSchedStackControl.xaml
    /// </summary>
    public partial class LtlStackControl : UserControl
    {
        public LtlStackControl()
        {
            InitializeComponent();
        }

        #region Button Events

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            GrdEdit.Visibility = Visibility.Hidden;
            GrdDisp.Visibility = Visibility.Visible;
        }

        private void btnDispSettings_Click(object sender, RoutedEventArgs e)
        {
            GrdEdit.Visibility = Visibility.Visible;
            GrdDisp.Visibility = Visibility.Hidden;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DtpAppt.Value.ToString()))
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                @"Are you sure you want to delete this schedule entry?", @"Delete Schedule Entry",
                System.Windows.Forms.MessageBoxButtons.YesNo);
            if (_diag == System.Windows.Forms.DialogResult.Yes)
            {
                Visibility = Visibility.Collapsed;
                
                try
                {
                    SchedStackVm _vm = DataContext as SchedStackVm;
                    string _apptId = _vm?.ApptId;
                    XpresEntities _xps = new XpresEntities();
                    var _q = (from _a in _xps.RcvSchedules
                             where _a.ApptID == _apptId
                             select _a);

                    foreach (RcvSchedule _item in _q) _xps.RcvSchedules.Remove(_item);

                    _xps.SaveChanges();
                    System.Windows.Forms.MessageBox.Show(@"Appointment for " + LblCarr.Content.ToString() + @" at " + LblAppt.Content.ToString() + @" removed from the database");
                }
                catch (Exception _ex)
                {
                    System.Windows.Forms.MessageBox.Show(@"Error while trying to delete appointment from the database: Either appt was never saved or - " + _ex.Message);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            GrdEdit.Visibility = Visibility.Hidden;
            GrdDisp.Visibility = Visibility.Visible;
        }

        #endregion Button Events

    }
}