using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;

namespace XPRES.Departments.Inbound.Controls
{
    /// <summary>
    /// Interaction logic for InbActivityStackControl.xaml
    /// </summary>
    public partial class InbActStackControl : UserControl
    {
        public InbActStackControl()
        {
            InitializeComponent();
        }

        #region Button Events

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            GrdDisp.Visibility = Visibility.Hidden;
            GrdEdit.Visibility = Visibility.Visible;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            GrdEdit.Visibility = Visibility.Hidden;
            GrdDisp.Visibility = Visibility.Visible;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            GrdEdit.Visibility = Visibility.Hidden;
            GrdDisp.Visibility = Visibility.Visible;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(LblPoMidNum.Content.ToString()))
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                @"Are you sure you want to delete this entry?", "Delete Entry",
                System.Windows.Forms.MessageBoxButtons.YesNo);
            if (_diag == System.Windows.Forms.DialogResult.Yes)
            {
                Visibility = Visibility.Collapsed;
                try
                {
                    string _ctrlId = this.Name.ToString();
                    IQueryable<InboundActivity> _q = (from _a in new XpresEntities().InboundActivities
                             where _a.CtrlId == _ctrlId
                             select _a);
                    if (_q != null)
                    {
                        foreach (InboundActivity _item in _q)
                        {
                            new XpresEntities().InboundActivities.Remove(_item);
                        }
                        new XpresEntities().SaveChanges();
                        System.Windows.Forms.MessageBox.Show(@"Item removed from the database");
                    }
                }
                catch (Exception _ex)
                {
                    System.Windows.Forms.MessageBox.Show(@"Error while trying to delete entry from the database: Either this was never saved or - " + _ex.Message);
                }
            }
        }

        #endregion Button Events
    }
}