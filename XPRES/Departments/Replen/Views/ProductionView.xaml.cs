using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;
using XPRES.Departments.Replen.Controls;

namespace XPRES.Departments.Replen.Views
{
    /// <summary>
    /// Interaction logic for rpAndonProdView.xaml
    /// </summary>
    public partial class ProductionView : Window
    {
        private XpresEntities xps;
        private Timer requestTimer;

        public ProductionView()
        {
            InitializeComponent();
            requestTimer = new Timer();
            requestTimer.Interval = 15000;
            requestTimer.Elapsed += MetricsTimer_Elapsed;
            requestTimer.Start();
            GetOpenReqs();
            FillProdAreas();
        }

        private void MetricsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() => GetOpenReqs()));
        }

        private void btnReqView_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is RequestInterface)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                RequestInterface _reqView = new RequestInterface();
                _reqView.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Request Interface")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        #region Methods

        private void FillProdAreas()
        {
            try
            {
                xps = new XpresEntities();
                List<string> _prodAreas = new List<string>();
                var prodAreas = (from a in xps.ProductionAreas
                                 select a.ProdLine).ToList();
                _prodAreas = prodAreas;
                cboProdArea.ItemsSource = _prodAreas;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error retrieving production areas: " + ex.Message);
            }
        }

        private void GetOpenReqs()
        {
            try
            {
                xps = new XpresEntities();
                List<MaterialRequest> _openReqs = new List<MaterialRequest>();
                rpProdViewStackControl _stackCtrl;
                string _lastTouch = string.Empty;
                string _prodArea = string.Empty;
                if (cboProdArea.SelectedIndex > -1)
                {
                    _prodArea = cboProdArea.SelectedItem.ToString();
                }

                var openReqs = (from a in xps.MaterialRequests
                                where a.ReqStatus != "Delivered"
                                orderby a.SubTimestamp descending
                                select a);

                if (_prodArea == "")
                {
                    _openReqs = openReqs.ToList();
                }
                else
                {
                    _openReqs = openReqs.Where(a => a.ProdLine.ToString() == _prodArea).ToList();
                }

                if (spRepPanel.Children.Count > 0)
                {
                    spRepPanel.Children.Clear();
                }

                foreach (var r in _openReqs)
                {
                    _stackCtrl = new rpProdViewStackControl();
                    _stackCtrl.lblPID.Content = r.PartNum.ToString();
                    _stackCtrl.Name = r.RequestNum.ToString();

                    if (r.ReqStatus.ToString() == "Submitted")
                        _lastTouch = Convert.ToDateTime(r.SubTimestamp).ToString("M/dd/yy hh:mm");

                    if (r.ReqStatus.ToString() == "Acknowledged")
                        _lastTouch = Convert.ToDateTime(r.AckTimestamp).ToString("M/dd/yy hh:mm");

                    if (r.ReqStatus.ToString() == "Filled")
                        _lastTouch = Convert.ToDateTime(r.FillTimestamp).ToString("M/dd/yy hh:mm");

                    _stackCtrl.lblLastTouch.Content = _lastTouch;
                    _stackCtrl.lblStatus.Content = r.ReqStatus.ToString();
                    spRepPanel.Children.Add(_stackCtrl);
                }

                string _pid = string.Empty;
                string _desc = string.Empty;

                foreach (rpProdViewStackControl ctrl in spRepPanel.Children)
                {
                    _pid = ctrl.lblPID.Content.ToString();
                    var desc = (from a in xps.UnitCosts
                                where a.Item == _pid
                                select a.Description).SingleOrDefault();
                    if (desc == null || !desc.Any())
                    {
                        _desc = "Unknown";
                    }
                    else
                    {
                        _desc = desc.ToString();
                    }
                    ctrl.lblDesc.Content = _desc;
                }
                spRepPanel.UpdateLayout();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error retrieving material requests: " + ex.Message);
            }
        }

        private void cboProdArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() => GetOpenReqs()));
        }

        #endregion Methods
    }
}