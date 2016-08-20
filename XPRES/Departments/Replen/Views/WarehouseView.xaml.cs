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
    /// Interaction logic for rpAndon.xaml
    /// </summary>
    public partial class WarehouseView : Window
    {
        private XpresEntities xps;
        private Timer requestTimer;

        public WarehouseView()
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
            Dispatcher.Invoke(() => GetOpenReqs());
        }

        private void FillProdAreas()
        {
            try
            {
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

        private void cboProdArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillProdAreas();
        }

        private void GetOpenReqs()
        {
            try
            {
                xps = new XpresEntities();
                List<MaterialRequest> _openReqs = new List<MaterialRequest>();
                rpAndonStackControl _stackCtrl;
                string _lastTouch = string.Empty;
                string _prodArea = string.Empty;
                string _location = string.Empty;
                string _pid = string.Empty;
                string _desc = string.Empty;
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
                    _stackCtrl = new rpAndonStackControl();
                    _stackCtrl.lblProdArea.Content = r.ProdLine.ToString();
                    _pid = r.PartNum.ToString();
                    _stackCtrl.lblPID.Text = _pid;
                    _stackCtrl.Name = r.RequestNum.ToString();

                    try
                    {
                        var location = (from a in xps.MaterialReferences
                                        where a.PID == _pid
                                        select a.Location).SingleOrDefault();
                        if (location == null || !location.Any())
                        {
                            _location = "Warehouse";
                        }
                        else
                        {
                            _location = location.ToString();
                        }
                    }
                    catch
                    {
                    }

                    _stackCtrl.lblLoc.Content = _location;

                    if (r.ReqStatus.ToString() == "Submitted")
                    {
                        _lastTouch = Convert.ToDateTime(r.SubTimestamp).ToString("M/dd/yy hh:mm");
                        _stackCtrl.btnAck.Visibility = Visibility.Visible;
                    }

                    if (r.ReqStatus.ToString() == "Acknowledged")
                    {
                        _lastTouch = Convert.ToDateTime(r.AckTimestamp).ToString("M/dd/yy hh:mm");
                        _stackCtrl.btnAck.Visibility = Visibility.Hidden;
                        _stackCtrl.btnFilled.Visibility = Visibility.Visible;
                    }

                    if (r.ReqStatus.ToString() == "Filled")
                    {
                        _lastTouch = Convert.ToDateTime(r.FillTimestamp).ToString("M/dd/yy hh:mm");
                        _stackCtrl.btnAck.Visibility = Visibility.Hidden;
                    }

                    _stackCtrl.lblQty.Content = r.ReqQty.ToString();
                    _stackCtrl.lblLastTouch.Content = _lastTouch;
                    _stackCtrl.lblStatus.Content = r.ReqStatus.ToString();
                    spRepPanel.Children.Add(_stackCtrl);
                }

                foreach (rpAndonStackControl ctrl in spRepPanel.Children)
                {
                    _pid = ctrl.lblPID.Text.ToString();
                    try
                    {
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
                    }
                    catch
                    {
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
    }
}