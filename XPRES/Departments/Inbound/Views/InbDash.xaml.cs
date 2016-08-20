using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;
using XPRES.Helpers;
using XPRES.Main.Views;

namespace XPRES.Departments.Inbound.Views
{
    public partial class InbDash : Window, IDisposable
    {
        private XpresEntities xps;
        private string gridName;
        private DataTable dtMetrics;
        private Timer metricsTimer;

        public InbDash()
        {
            InitializeComponent();
            xps = new XpresEntities();
            dtMetrics = new DataTable();
            gridName = string.Empty;
            CreateTimer();
            InbActivityChart("REC");
            InbActivityChart("PUT");
            GetLtlInfo();
            CreateMetricsGrid();
            GetMetrics();
        }

        public void Dispose()
        {
            xps.Dispose();
            dtMetrics.Dispose();
            metricsTimer.Dispose();
        }

        private void CreateTimer()
        {
            metricsTimer = new Timer();
            metricsTimer.Interval = 15000;
            metricsTimer.Elapsed += MetricsTimer_Elapsed;
            metricsTimer.Start();
        }

        private void MetricsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispose();
            RefreshCharts();
            CreateTimer();
        }

        private void RefreshCharts()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                InbActivityChart("REC");
                InbActivityChart("PUT");
                GetLtlInfo();
            }));
        }

        #region Button Events

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            InbActivityChart("REC");
            InbActivityChart("PUT");
            GetLtlInfo();
            CreateMetricsGrid();
            GetMetrics();
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

        private void btnInboundSched_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is InbSAAG)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                InbSAAG saag = new InbSAAG();
                saag.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Inbound SAAG")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        //private void btnInbMetrics_Click(object sender, RoutedEventArgs e)
        //{
        //    bool _open = false;
        //    foreach (var wnd in Application.Current.Windows)
        //    {
        //        if (wnd is InbMetrics)
        //        {
        //            _open = true;
        //        }
        //    }
        //    if (!_open)
        //    {
        //        InbMetrics metrics = new InbMetrics();
        //        metrics.Show();
        //    }
        //    else
        //    {
        //        foreach (Window wnd in Application.Current.Windows)
        //        {
        //            if (wnd.Title == "Inbound Metrics")
        //            {
        //                wnd.Activate();
        //            }
        //        }
        //    }
        //}

        //private void btnMetrics_Click(object sender, RoutedEventArgs e)
        //{
        //    gridName = "grdMetrics";
        //    CycleVisuals(gridName);
        //}

        private void btnRcpts_Click(object sender, RoutedEventArgs e)
        {
            gridName = "grdRcpts";
            CycleVisuals(gridName);
        }

        private void btnPtwys_Click(object sender, RoutedEventArgs e)
        {
            gridName = "grdPtwys";
            CycleVisuals(gridName);
        }

        private void btnOutPrev_Click(object sender, RoutedEventArgs e)
        {
            if (grdMetrics.Visibility == Visibility.Visible)
            {
                gridName = "grdPtwys";
                CycleVisuals(gridName);
            }
            else if (grdPtwys.Visibility == Visibility.Visible)
            {
                gridName = "grdRcpts";
                CycleVisuals(gridName);
            }
            else if (grdRcpts.Visibility == Visibility.Visible)
            {
                gridName = "grdMetrics";
                CycleVisuals(gridName);
            }
        }

        private void btnOutNext_Click(object sender, RoutedEventArgs e)
        {
            if (grdMetrics.Visibility == Visibility.Visible)
            {
                gridName = "grdRcpts";
                CycleVisuals(gridName);
            }
            else if (grdPtwys.Visibility == Visibility.Visible)
            {
                gridName = "grdMetrics";
                CycleVisuals(gridName);
            }
            else if (grdRcpts.Visibility == Visibility.Visible)
            {
                gridName = "grdPtwys";
                CycleVisuals(gridName);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            dtpStart.ClearValue(DatePicker.SelectedDateProperty);
            dtpEnd.ClearValue(DatePicker.SelectedDateProperty);
        }

        private void btnInbAudit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Coming Soon!");
            return;
        }

        private void btnInbResources_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Coming Soon!");
            return;
        }

        #endregion Button Events

        #region Methods

        private void CycleVisuals(string GridName)
        {
            foreach (Grid grd in grdCenterMain.Children)
            {
                if (grd.Name == GridName)
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
        }

        private void InbActivityChart(string Type)
        {
            //XpsDates xdates = new XpsDates();
            //InbActivityCharts inbCharts = new InbActivityCharts();
            //DateTime _sdate = new DateTime();
            //DateTime _edate = new DateTime();

            //xdates.CheckDateEntry(dtpStart.SelectedDate.ToString(), xdates.StartWeek.ToString());
            //_sdate = xdates.ValidDate;

            //xdates.CheckDateEntry(dtpEnd.SelectedDate.ToString(), xdates.StartWeek.AddDays(6).ToString());
            //_edate = xdates.ValidDate;

            //inbCharts.GetActivityChart(_sdate, _edate, Type);

            //if (Type == "REC")
            //    SeriesRPH.DataContext = inbCharts.ActsPerHour;
            //if (Type == "PUT")
            //    SeriesPPH.DataContext = inbCharts.ActsPerHour;
        }

        private void GetLtlInfo()
        {
            xps = new XpresEntities();
            GetPercentage percent = new GetPercentage();
            double? _count = 0;
            double? _totPoRcvd;
            double? _poNotRcvd;
            double? _rcvd = 0;
            double _pct = 0;
            DateTime _today = DateTime.Today.Date;
            DateTime _tomorrow = _today.AddDays(1);

            try
            {
                var ltl = (from a in xps.RcvSchedules
                           where a.Appt >= _today && a.Appt < _tomorrow && a.Ltl == true
                           select a);

                //LTL Info
                _count = ltl.Count();
                lblTrucks.Content = _count.ToString();

                _rcvd = ltl.Where(a => a.RecStop != null).Count();

                if (_count != 0)
                {
                    _pct = (Convert.ToDouble(_rcvd) / Convert.ToDouble(_count)) * 100;
                    _pct = Math.Round(_pct, 0);
                    lblRcvd.Content = _pct.ToString();
                }

                _count = ltl.Sum(a => a.PalletNum);
                lblPlts.Content = _count.ToString();

                _count = ltl.Where(a => a.Arrive > a.Appt).Count();
                lblLate.Content = _count.ToString();

                _count = ltl.Sum(a => a.VNC);
                lblVnc.Content = _count;

                //Small Pack Info
                var sml = (from a in xps.RcvSchedules
                           where a.Arrive >= _today && a.Arrive < _tomorrow && a.Ltl == false
                           select a);

                _count = sml.Count();
                lblDlvr.Content = _count.ToString();
                _poNotRcvd = sml.Sum(a => a.PoNotRcvd);
                _rcvd = sml.Sum(a => a.PoRcvd);
                _totPoRcvd = _poNotRcvd + _rcvd;
                if (_totPoRcvd != 0)
                {
                    _pct = (Convert.ToDouble(_rcvd) / Convert.ToDouble(_totPoRcvd)) * 100;
                    _pct = Math.Round(_pct, 0);
                    lblRcvdS.Content = _pct.ToString();
                }

                _count = sml.Sum(a => a.PalletNum);
                lblPrcl.Content = _count.ToString();

                _count = sml.Sum(a => a.VNC);
                lblVncS.Content = _count.ToString();

                //Putaway Info
                var ptwy = (from a in xps.InboundActivities
                            where a.Start >= _today && a.Start < _tomorrow
                            select a);

                _count = ptwy.Select(a => a.Operator).Distinct().Count();
                lblPrs.Content = _count.ToString();

                _count = ptwy.Count();
                lblTotPtwy.Content = _count.ToString();

                double _comp = ptwy.Where(a => a.Finish != null).Count();
                lblComp.Content = _comp.ToString();

                if (_count != 0)
                {
                    _pct = (_comp / Convert.ToDouble(_count)) * 100;
                    _pct = Math.Round(_pct, 0);
                    lblPut.Content = _pct.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error retrieving LTL Schedule data: " + ex.Message);
            }
        }

        public object CreateMetricsGrid()
        {
            dtMetrics = new DataTable();
            dtMetrics.Columns.Add("Receiving Metrics");

            List<string> MetricCtg = new List<string>();
            MetricCtg.Add("Receipts: Total Pallets");
            MetricCtg.Add("Receipts: Age > 7 Hours");
            MetricCtg.Add("Receipts: Pallets Not Rcvd - XPO");
            MetricCtg.Add("Receipts: Pallets Not Rcvd - NCR");
            MetricCtg.Add("Putaway: Total Pallets");
            MetricCtg.Add("Putaway: Total");
            MetricCtg.Add("Putaway: Age > EOD");
            MetricCtg.Add("Audits: Putaway Errors");
            MetricCtg.Add("Audits: PO Receipts Audited");
            MetricCtg.Add("Audits: PO Receipt Errors");
            MetricCtg.Add("VNC: Open Total");
            MetricCtg.Add("VNC: < 24 Hours");
            MetricCtg.Add("VNC: > 48 Hours");
            MetricCtg.Add("Returns: Total");
            MetricCtg.Add("Returns: AGE > 24 Hours");
            MetricCtg.Add("NON Inventory: Total Pallets");
            MetricCtg.Add("Expedited Parts: Total");
            MetricCtg.Add("Empty Locators: Total");
            MetricCtg.Add("Empty Locators: MFGHUB");
            MetricCtg.Add("Empty Locators: Available");
            MetricCtg.Add("Awaiting Putaway: Total");
            MetricCtg.Add("Awaiting Putaway: MFGHUB");
            MetricCtg.Add("Awaiting Putaway: Available");
            MetricCtg.Add("Scheduling: Scheduled Pallets");
            MetricCtg.Add("Scheduling: Actual Delivered");
            MetricCtg.Add("Head Count: LTL");
            MetricCtg.Add("Head Count: Small Parcel");
            MetricCtg.Add("PIT Equipments issues: Hours Down");
            MetricCtg.Add("Consolidation: LTL");
            MetricCtg.Add("Consolidation: Small Parcel");

            foreach (var ctg in MetricCtg)
            {
                dtMetrics.Rows.Add(ctg);
            }
            return dtMetrics;
        }

        private void GetMetrics()
        {
            //DateTime _sdate = new DateTime();
            //DateTime _edate = new DateTime();

            //if (dtpStart.SelectedDate.ToString() != "")
            //{
            //    try
            //    {
            //        DateTime.TryParse(dtpStart.SelectedDate.ToString(), out _sdate);
            //    }
            //    catch
            //    {
            //        System.Windows.Forms.MessageBox.Show("Invalid From date.");
            //        return;
            //    }
            //}
            //else
            //{
            //    int _month = DateTime.Now.Month;
            //    int _year = DateTime.Now.Year;
            //    _sdate = Convert.ToDateTime(_month + "/1/" + _year);
            //}
            //if (dtpEnd.SelectedDate.ToString() != "")
            //{
            //    try
            //    {
            //        DateTime.TryParse(dtpEnd.SelectedDate.ToString(), out _edate);
            //    }
            //    catch
            //    {
            //        System.Windows.Forms.MessageBox.Show("Invalid To date.");
            //        return;
            //    }
            //}
            //else _edate = DateTime.Today.Date;

            //List<MetricsInbound> _metrics = new List<MetricsInbound>();
            //try
            //{
            //    var metrics = (from a in xps.MetricsInbounds
            //                   where a.Date >= _sdate && a.Date <= _edate
            //                   select a).ToList();
            //    _metrics = metrics;
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show("Error While retrieving from the database: " + ex.Message);
            //    return;
            //}

            //string _colHeader = string.Empty;

            //foreach (var m in _metrics)
            //{
            //    _colHeader = Convert.ToDateTime(m.Date).ToString("MM-dd-yyyy");
            //    dtMetrics.Columns.Add(_colHeader);

            //    for (int i = 0; i < dtMetrics.Columns.Count; i++)
            //    {
            //        DataColumn col = dtMetrics.Columns[i];
            //        if (col.ColumnName == _colHeader)
            //        {
            //            dtMetrics.Rows[0][col] = m.Receipts_Total_Pallets;
            //            dtMetrics.Rows[1][col] = m.Receipts_Aged;
            //            dtMetrics.Rows[2][col] = m.Receipts_XPO_NotRcvd;
            //            dtMetrics.Rows[3][col] = m.Receipts_NCR_NotRcvd;
            //            dtMetrics.Rows[4][col] = m.Putaway_Total_Pallets;
            //            dtMetrics.Rows[5][col] = m.Putaway_Total;
            //            dtMetrics.Rows[6][col] = m.Putaway_Aged;
            //            dtMetrics.Rows[7][col] = m.Audits_Putaway_Errors;
            //            dtMetrics.Rows[8][col] = m.Audits_Rec_Audited;
            //            dtMetrics.Rows[9][col] = m.Audits_Rec_Errors;
            //            dtMetrics.Rows[10][col] = m.VNC_Open;
            //            dtMetrics.Rows[11][col] = m.VNC_Under_24Hours;
            //            dtMetrics.Rows[12][col] = m.VNC_Over_48Hours;
            //            dtMetrics.Rows[13][col] = m.Returns_Total;
            //            dtMetrics.Rows[14][col] = m.Returns_Aged;
            //            dtMetrics.Rows[15][col] = m.NonInventory;
            //            dtMetrics.Rows[16][col] = m.Expedited_Parts;
            //            dtMetrics.Rows[17][col] = m.Empty_Total;
            //            dtMetrics.Rows[18][col] = m.Empty_MFGHUB;
            //            dtMetrics.Rows[19][col] = m.Empty_Available;
            //            dtMetrics.Rows[20][col] = m.Awaiting_Putaway_Total;
            //            dtMetrics.Rows[21][col] = m.Awaiting_Putaway_MFGHUB;
            //            dtMetrics.Rows[22][col] = m.Awaiting_Putaway_Available;
            //            dtMetrics.Rows[23][col] = m.Scheduling_Pallets_Scheduled;
            //            dtMetrics.Rows[24][col] = m.Scheduling_Pallets_Actual;
            //            dtMetrics.Rows[25][col] = m.HeadCount_LTL;
            //            dtMetrics.Rows[26][col] = m.HeadCount_SmallPack;
            //            dtMetrics.Rows[27][col] = m.PIT_Issues_Hours_Down;
            //            dtMetrics.Rows[28][col] = m.Consol_LTL;
            //            dtMetrics.Rows[29][col] = m.Consol_SmallPack;
            //        }

            //    }
            //}
            //dgInbMetrics.ItemsSource = dtMetrics.DefaultView;
        }

        #endregion Methods

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
        }
    }
}