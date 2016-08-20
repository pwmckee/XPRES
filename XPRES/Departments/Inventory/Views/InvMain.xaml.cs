using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;
using XPRES.Departments.Inventory.Adjustments;
using XPRES.Departments.Inventory.GeoCounts;
using XPRES.Helpers;
using XPRES.Main.Views;

namespace XPRES.Departments.Inventory.Views
{
    /// <summary>
    /// Interaction logic for InvMain.xaml
    /// </summary>
    public partial class InvMain : Window
    {
        private bool open;
        private XpresEntities xps;
        private XpsDates xdate;
        private GetPercentage percent;
        private DateTime weekStart;
        private DateTime endDate;
        private string gridName;

        public InvMain()
        {
            InitializeComponent();
            open = false;
            xps = new XpresEntities();
            xdate = new XpsDates();
            percent = new GetPercentage();
            weekStart = new DateTime();
            endDate = new DateTime();
            gridName = string.Empty;
            InvSAAGTop();
            GeoCountInfo();
            AdjustmentInfo();
        }

        #region Button Events

        #region Main Menu

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            InvSAAGTop();
            GeoCountInfo();
            AdjustmentInfo();
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

        private void btnOutPrev_Click(object sender, RoutedEventArgs e)
        {
            if (GrdSaag.Visibility == Visibility.Visible)
            {
                gridName = "grdAdjValueChart";
                CycleVisuals(gridName);
            }
            else if (GrdAdjValueChart.Visibility == Visibility.Visible)
            {
                gridName = "grdLocChart";
                CycleVisuals(gridName);
            }
            else if (GrdLocChart.Visibility == Visibility.Visible)
            {
                gridName = "grdSAAG";
                CycleVisuals(gridName);
            }
        }

        private void btnOutNext_Click(object sender, RoutedEventArgs e)
        {
            if (GrdSaag.Visibility == Visibility.Visible)
            {
                gridName = "grdLocChart";
                CycleVisuals(gridName);
            }
            else if (GrdLocChart.Visibility == Visibility.Visible)
            {
                gridName = "grdAdjValueChart";
                CycleVisuals(gridName);
            }
            else if (GrdAdjValueChart.Visibility == Visibility.Visible)
            {
                gridName = "grdSAAG";
                CycleVisuals(gridName);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            DtpStart.ClearValue(DatePicker.SelectedDateProperty);
            DtpEnd.ClearValue(DatePicker.SelectedDateProperty);
        }

        #endregion Main Menu

        #region Left Nav

        private void btnGC_Click(object sender, RoutedEventArgs e)
        {
            open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is GeoCountsMain)
                {
                    open = true;
                }
            }
            if (!open)
            {
                GeoCountsMain gc = new GeoCountsMain();
                gc.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Geographic Counts")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        private void btnAdj_Click(object sender, RoutedEventArgs e)
        {
            open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is AdjMain)
                {
                    open = true;
                }
            }
            if (!open)
            {
                AdjMain adj = new AdjMain();
                adj.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Adjustments")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        #endregion Left Nav

        #region Left Displays

        private void btnSAAG_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterMain.Children)
            {
                if (grd.Name == "GrdSaag")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnLocAcc_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterMain.Children)
            {
                if (grd.Name == "GrdLocChart")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnAdjVal_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterMain.Children)
            {
                if (grd.Name == "GrdAdjValueChart")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnOffs_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Coming Soon!");
            return;
        }

        private void btnOns_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Coming Soon!");
            return;
        }

        #endregion Left Displays

        #endregion Button Events

        #region Methods

        #region Charts

        private void CycleVisuals(string GridName)
        {
            foreach (Grid grd in GrdCenterMain.Children)
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

        #endregion Charts

        #region Right Side Details

        private void GeoCountInfo()
        {
            xps = new XpresEntities();
            GeoCountPlacard gcInfo = new GeoCountPlacard();

            gcInfo.GetCounts("Created");
            LblFirOpen.Content = gcInfo.Counts.ToString();
            gcInfo.GetCounts("FirstSubmitReview");
            LblFirRev.Content = gcInfo.Counts.ToString();
            gcInfo.GetCounts("SecondReady");
            LblSecOpen.Content = gcInfo.Counts.ToString();
            gcInfo.GetCounts("SecondSubmitReview");
            LblSecRev.Content = gcInfo.Counts.ToString();
        }

        private void AdjustmentInfo()
        {
            xps = new XpresEntities();
            List<Adjustment> adj = new List<Adjustment>();
            List<double?> adjMetrics = new List<double?>();
            int _year = DateTime.Today.Year;
            DateTime _sdate = Convert.ToDateTime("1/1/" + _year);
            DateTime _edate = Convert.ToDateTime("12/31/" + _year);
            int _openReq = 0;
            //int _penReq = 0;
            int _finReq = 0;
            double? _adjValue = 0;

            try
            {
                adj = (from a in xps.Adjustments
                       where a.Status == "Needs Review"
                       select a).ToList();

                _openReq = adj.Count;

                adj = (from a in xps.Adjustments
                       where a.Status == "Review Complete"
                       select a).ToList();

                _finReq = adj.Count;

                adjMetrics = (from a in xps.MetricsAdjs
                              where a.Date >= _sdate && a.Date <= _edate
                              select a.AdjValue).ToList();

                _adjValue = adjMetrics.Sum();
                _adjValue = Math.Round(Convert.ToDouble(_adjValue), 2);

                //adj = (from a in xps.CountSchedules
                //          where a.CountStatus == "SecondReady"
                //          select a.CountID).Distinct().ToList();

                //_openSec = adj.Count();

                //adj = (from a in xps.CountSchedules
                //          where a.CountStatus == "SecondSubmitReview"
                //          select a.CountID).Distinct().ToList();

                //_revSec = adj.Count();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error retrieving Adjustment info: " + ex.Message);
                return;
            }

            LblAdjRev.Content = _openReq.ToString();
            LblAdjSub.Content = _finReq.ToString();
            LblYtdNet.Content = "$ " + _adjValue.ToString();
        }

        #endregion Right Side Details

        #region SAAG

        public object InvSAAGTop()
        {
            double? _totLoc = 0;
            double? _errLoc = 0;
            double _locs = 0;
            double _errs = 0;
            double _locAcc = 0;
            string _firLoc = string.Empty;
            string _lastLoc = string.Empty;
            string _comment = string.Empty;
            string _action = string.Empty;
            xps = new XpresEntities();
            InvSaagA SAAG_a = new InvSaagA();
            GetPercentage percent = new GetPercentage();

            xdate.CheckDateEntry(DtpEnd.SelectedDate.ToString(), DateTime.Today.Date.ToString());
            endDate = xdate.ValidDate;
            xdate.GetWeekStart(endDate);
            weekStart = xdate.StartWeek;

            SAAG_a.CreateTable(weekStart, endDate);

            try
            {
                foreach (DataRow dr in SAAG_a.dtSaagA.Rows)
                {
                    DateTime _checkDate = Convert.ToDateTime(dr[0]);
                    var locs = (from a in xps.CCTrackers
                                where a.CountDate == _checkDate && a.CountStatus != "NewCreate"
                                select a);
                    _locs = locs.Count();
                    if (_locs > 0)
                    {
                        dr[1] = _locs;
                        dr[2] = _locs;

                        foreach (var c in locs)
                        {
                            if (c.Action != null)
                            {
                                _action = c.Action.ToString();
                            }
                            else
                            {
                                _action = "";
                            }

                            if (c.CountStatus == "Complete" && _action != "")
                            {
                                _errs++;
                            }
                            else if (c.Difference != 0)
                            {
                                _errs++;
                            }
                        }

                        dr[3] = _errs;

                        percent.CalcLoc(_locs, _errs);

                        if (_locAcc == 100)
                        {
                            dr[4] = 99.99;
                        }
                        else
                        {
                            dr[4] = percent.PerLoc;
                        }

                        try
                        {
                            IQueryable<string> _locators = locs.Select(a => a.Locator);
                            _firLoc = _locators.First();
                            _lastLoc = _locators.Max();
                        }
                        catch
                        {
                            _lastLoc = _firLoc + "(Error getting last location)";
                        }

                        _comment = _firLoc + "-" + _lastLoc;
                        dr[5] = _comment;

                        _totLoc += _locs;
                        _errLoc += _errs;
                        _errs = 0;
                    }
                }

                DataRow _row = SAAG_a.dtSaagA.NewRow();
                _row[0] = "WTD";
                _row[1] = _totLoc;
                _row[2] = _totLoc;
                _row[3] = _errLoc;
                percent.CalcLoc(Convert.ToDouble(_totLoc), Convert.ToDouble(_errLoc));
                _row[4] = percent.PerLoc;
                SAAG_a.dtSaagA.Rows.Add(_row);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while calculating daily totals: " + ex.Message);
            }

            DgInvSaagA.ItemsSource = SAAG_a.dtSaagA.DefaultView;
            InvSAAGBottom();
            return SAAG_a.dtSaagA;
        }

        private void InvSAAGBottom()
        {
            double? _totUnits = 0;
            double? _errUnits = 0;
            double? _errAbsUnits = 0;
            double? _totLoc = 0;
            double? _errLoc = 0;
            string _action = string.Empty;
            int _month = DateTime.Now.Month;
            int _year = DateTime.Now.Year;
            xps = new XpresEntities();
            InvSaagB SAAG_b = new InvSaagB();
            DataTable dtSaagBot = new DataTable();
            DateTime _edate = new DateTime();

            xdate.CheckDateEntry(DtpEnd.SelectedDate.ToString(), DateTime.Today.Date.ToString());
            _edate = xdate.ValidDate;

            xdate.GetWeekStart(_edate);

            DateTime _entryDate = xdate.StartWeek;

            SAAG_b.CreateTable();

            try
            {
                //Get WTD Data from CCTracker
                var wkMetrics = (from a in xps.CCTrackers
                                 where a.CountDate >= _entryDate && a.CountDate <= _edate && a.CountStatus != "NewCreate"
                                 select a);
                _totUnits = wkMetrics.Sum(a => a.CountedQTY);
                _errUnits = wkMetrics.Sum(a => a.Difference);
                _totLoc = wkMetrics.Count();

                foreach (var cc in wkMetrics)
                {
                    if (cc.Difference < 0)
                    {
                        _errAbsUnits += cc.Difference * (-1);
                    }
                    else
                    {
                        _errAbsUnits += cc.Difference;
                    }

                    if (cc.Action != null)
                    {
                        _action = cc.Action.ToString();
                    }
                    else
                    {
                        _action = "";
                    }

                    if (cc.CountStatus == "Complete" && _action != "")
                    {
                        _errLoc++;
                    }
                    else if (cc.Difference != 0)
                    {
                        _errLoc++;
                    }
                }

                percent.CalcNet(Convert.ToDouble(_totUnits), Convert.ToDouble(_errUnits));
                percent.CalcAbs(Convert.ToDouble(_totUnits), Convert.ToDouble(_errAbsUnits));
                percent.CalcLoc(Convert.ToDouble(_totLoc), Convert.ToDouble(_errLoc));

                SAAG_b.FillRows("WTD", percent.PerNet, percent.PerAbs, percent.PerLoc);

                //Get MTD and YTD data from Inventory Metrics

                var moMetrics = (from a in xps.MetricsInvs select a);

                //Calculate and enter MTD

                IQueryable<MetricsInv> mData = moMetrics.Where(a => a.CountMonth == _month);

                _totUnits = mData.Sum(a => a.TotalUnits);
                _errUnits = mData.Sum(a => a.UnitsNet);
                _errAbsUnits = mData.Sum(a => a.UnitsAbs);
                _totLoc = mData.Sum(a => a.TotalLocations);
                _errLoc = mData.Sum(a => a.FirstPassLocations);

                percent.CalcNet(Convert.ToDouble(_totUnits), Convert.ToDouble(_errUnits));
                percent.CalcAbs(Convert.ToDouble(_totUnits), Convert.ToDouble(_errAbsUnits));
                percent.CalcLoc(Convert.ToDouble(_totLoc), Convert.ToDouble(_errLoc));

                SAAG_b.FillRows("MTD", percent.PerNet, percent.PerAbs, percent.PerLoc);

                //Calculate and enter YTD
                IQueryable<MetricsInv> yData = moMetrics.Where(a => a.CountYear == _year);
                _totUnits = yData.Sum(a => a.TotalUnits);
                _errUnits = yData.Sum(a => a.UnitsNet);
                _errAbsUnits = yData.Sum(a => a.UnitsAbs);
                _totLoc = yData.Sum(a => a.TotalLocations);
                _errLoc = yData.Sum(a => a.FirstPassLocations);

                percent.CalcNet(Convert.ToDouble(_totUnits), Convert.ToDouble(_errUnits));
                percent.CalcAbs(Convert.ToDouble(_totUnits), Convert.ToDouble(_errAbsUnits));
                percent.CalcLoc(Convert.ToDouble(_totLoc), Convert.ToDouble(_errLoc));

                SAAG_b.FillRows("YTD", percent.PerNet, percent.PerAbs, percent.PerLoc);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error retrieving metrics data: " + ex.Message);
            }

            DgInvSaagB.ItemsSource = SAAG_b.dtSaagB.DefaultView;
        }

        #endregion SAAG

        #endregion Methods
    }
}