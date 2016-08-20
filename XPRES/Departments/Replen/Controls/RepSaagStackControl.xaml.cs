using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;

namespace XPRES.Departments.Replen.Controls
{
    /// <summary>
    /// Interaction logic for RepSaagStackControl.xaml
    /// </summary>
    public partial class RepSaagStackControl : UserControl
    {
        private XpresEntities xps;

        public RepSaagStackControl()
        {
            InitializeComponent();
            xps = new XpresEntities();
            FillRunners();
        }

        #region Button Events

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveRepl();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            grdEdit.Visibility = Visibility.Hidden;
            grdDisp.Visibility = Visibility.Visible;
        }

        private void btnDispSettings_Click(object sender, RoutedEventArgs e)
        {
            grdEdit.Visibility = Visibility.Visible;
            grdDisp.Visibility = Visibility.Hidden;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                "Are you sure you want to delete this entry?", "Delete Entry",
                System.Windows.Forms.MessageBoxButtons.YesNo);
            if (_diag == System.Windows.Forms.DialogResult.Yes)
            {
                (this.Parent as StackPanel).Children.Remove(this);
                try
                {
                    string _ctrlID = this.Name.ToString();
                    var q = (from a in xps.ReplenSAAGs
                             where a.CtrlID == _ctrlID
                             select a);
                    foreach (var item in q)
                    {
                        xps.ReplenSAAGs.Remove(item);
                    }
                    xps.SaveChanges();
                    System.Windows.Forms.MessageBox.Show("Entry removed from the database");
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error while trying to delete entry from the database: Either entry was never saved or - " + ex.Message);
                    return;
                }
            }
        }

        #endregion Button Events

        #region Methods

        private void FillRunners()
        {
            List<string> _repl = new List<string>();

            try
            {
                var repl = (from a in xps.Employees
                            where a.Replen == true
                            select a.FullName).ToList();
                _repl = repl;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to retrieve runner list: " + ex.Message);
            }
            cboRunner.ItemsSource = _repl;
        }

        private void SaveRepl()
        {
            string _rrName = string.Empty;
            int _lk = 0;
            int _kdx = 0;
            int _pl = 0;
            int _hsec = 0;
            int _id = 0;
            try
            {
                var id = (from a in xps.ReplenSAAGs
                          select a.ID).Max();
                _id = id + 1;
            }
            catch
            {
                _id = 1;
            }

            #region Validation

            if (cboRunner.SelectedIndex >= 0)
            {
                _rrName = cboRunner.SelectedItem.ToString();
                lblName.Content = _rrName;
            }
            else if (lblName.Content == null)
            {
                System.Windows.Forms.MessageBox.Show("Please select a carrier");
                return;
            }

            if (txtLK.Text != "")
            {
                try
                {
                    _lk = Convert.ToInt32(txtLK.Text);
                    if (lblLK.Content != null)
                        _lk += Convert.ToInt32(lblLK.Content.ToString());
                    lblLK.Content = _lk.ToString();
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid LK QTY entered.");
                    return;
                }
            }

            if (txtKDX.Text != "")
            {
                try
                {
                    _kdx = Convert.ToInt32(txtKDX.Text);
                    if (lblKDX.Content != null)
                        _kdx += Convert.ToInt32(lblKDX.Content.ToString());
                    lblKDX.Content = _kdx.ToString();
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid KDX QTY entered.");
                    return;
                }
            }

            if (txtPL.Text != "")
            {
                try
                {
                    _pl = Convert.ToInt32(txtPL.Text);
                    if (lblPL.Content != null)
                        _pl += Convert.ToInt32(lblPL.Content.ToString());
                    lblPL.Content = _pl.ToString();
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid PL QTY entered.");
                    return;
                }
            }

            if (txtHSEC.Text != "")
            {
                try
                {
                    _hsec = Convert.ToInt32(txtHSEC.Text);
                    if (lblHSEC.Content != null)
                        _hsec += Convert.ToInt32(lblHSEC.Content.ToString());
                    lblHSEC.Content = _hsec.ToString();
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid HUBSECURE QTY entered.");
                    return;
                }
            }

            #endregion Validation

            if (this.Name == "")
            {
                this.Name = "REPLN" + _id;
                try
                {
                    ReplenSAAG repl = new ReplenSAAG();
                    repl.Employee = _rrName;
                    repl.TimeStamp = DateTime.Now.Date;
                    if (txtLK.Text != "")
                        repl.LK00001 = _lk;
                    if (txtKDX.Text != null)
                        repl.KARDEX = _kdx;
                    if (txtPL.Text != null)
                        repl.PL = _pl;
                    if (txtHSEC.Text != null)
                        repl.HUBSECURE = _hsec;
                    repl.CtrlID = this.Name.ToString();
                    xps.ReplenSAAGs.Add(repl);
                    xps.SaveChanges();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error while saving to the database: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    var repl = (from a in xps.ReplenSAAGs
                                where a.CtrlID == this.Name
                                select a).SingleOrDefault();
                    repl.Employee = _rrName;
                    if (txtLK.Text != "")
                        repl.LK00001 = _lk;
                    if (txtKDX.Text != null)
                        repl.KARDEX = _kdx;
                    if (txtPL.Text != null)
                        repl.PL = _pl;
                    if (txtHSEC.Text != null)
                        repl.HUBSECURE = _hsec;
                    xps.SaveChanges();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error while saving record: " + ex.Message);
                    return;
                }
            }
            txtLK.Clear();
            txtKDX.Clear();
            txtPL.Clear();
            txtHSEC.Clear();
            grdEdit.Visibility = Visibility.Hidden;
            grdDisp.Visibility = Visibility.Visible;
        }

        #endregion Methods
    }
}