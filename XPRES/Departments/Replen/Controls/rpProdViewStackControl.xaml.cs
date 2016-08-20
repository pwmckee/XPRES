using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;

namespace XPRES.Departments.Replen.Controls
{
    /// <summary>
    /// Interaction logic for rpProdViewStackControl.xaml
    /// </summary>
    public partial class rpProdViewStackControl : UserControl
    {
        private XpresEntities xps;
        private string timestamp;

        public rpProdViewStackControl()
        {
            InitializeComponent();
            timestamp = DateTime.Now.ToString("M/dd/yy hh:mm");
        }

        private void UpdateRequest(string status, string time)
        {
            xps = new XpresEntities();
            string _reqID = this.Name.ToString();

            MaterialRequest req = (from a in xps.MaterialRequests
                                   where a.RequestNum == _reqID
                                   select a).SingleOrDefault();
            if (status == "Acknowledged")
                req.AckTimestamp = Convert.ToDateTime(time);
            if (status == "Filled")
                req.FillTimestamp = Convert.ToDateTime(time);
            if (status == "Delivered")
                req.DelvrTimestamp = Convert.ToDateTime(time);
            req.ReqStatus = status;
            xps.SaveChanges();
        }

        private void btnAck_Click(object sender, RoutedEventArgs e)
        {
            timestamp = DateTime.Now.ToString("M/dd/yy hh:mm");
            btnAck.Visibility = Visibility.Hidden;
            btnFill.Visibility = Visibility.Visible;
            UpdateRequest("Acknowledged", timestamp);
            lblStatus.Content = "Acknowledged";
            lblLastTouch.Content = timestamp;
        }

        private void btnFill_Click(object sender, RoutedEventArgs e)
        {
            timestamp = DateTime.Now.ToString("M/dd/yy hh:mm");
            btnFill.Visibility = Visibility.Hidden;
            UpdateRequest("Filled", timestamp);
            lblStatus.Content = "Filled";
            lblLastTouch.Content = timestamp;
        }

        private void btnDlvr_Click(object sender, RoutedEventArgs e)
        {
            timestamp = DateTime.Now.ToString("M/dd/yy hh:mm");
            UpdateRequest("Delivered", timestamp);
            lblStatus.Content = "Delivered";
            lblLastTouch.Content = timestamp;
        }
    }
}