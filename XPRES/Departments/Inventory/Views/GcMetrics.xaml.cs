using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using XPRES.DAL;

namespace XPRES.Departments.Inventory.Views
{
    /// <summary>
    /// Interaction logic for GeoCountsMetrics.xaml
    /// </summary>
    public partial class GcMetrics : Window
    {
        //private XpresEntities xps;
        //private DataTable dt;
        //private DataTable dtDetails;
        //private DateTime sdate;
        //private DateTime edate;
        //private double[] MetricsArr;
        //private double?[] DetailsArr;

        public GcMetrics()
        {
            InitializeComponent();
            //xps = new XpresEntities();
            //sdate = new DateTime();
            //edate = new DateTime();
            //dt = new DataTable();
            //dtDetails = new DataTable();
            //FillYearList();
        }

        

        //#region Lists

        //private void FillYearList()
        //{
        //    List<int?> _yearSelect = new List<int?>();
        //    var year = (from a in xps.MetricsInvs
        //                select a.CountYear).Distinct().ToList();
        //    _yearSelect = year;
        //    CboYear.ItemsSource = _yearSelect;
        //}

        //private void FillMonthList()
        //{
        //    List<int?> _monthSelect = new List<int?>();
        //    int _selectedYear = Convert.ToInt32(CboYear.SelectedItem);

        //    var month = (from a in xps.MetricsInvs
        //                 where a.CountYear == _selectedYear
        //                 select a.CountMonth).Distinct().ToList();
        //    _monthSelect = month;
        //    CboMonth.ItemsSource = _monthSelect;
        //}

        //#endregion Lists

        //#region Button Events

        //private void btnCalc_Click(object sender, RoutedEventArgs e)
        //{
        //    CalcMetrics();
        //}

        //private void btnView_Click(object sender, RoutedEventArgs e)
        //{
        //    PopulateHighlvl();
        //}

        //#endregion Button Events

        //#region Methods

        //#region Metrics Grid Methods

        //private void CalcMetrics()
        //{
        //    if (DtpSdMetrics.SelectedDate.ToString() != "")
        //    {
        //        try
        //        {
        //            DateTime.TryParse(DtpSdMetrics.SelectedDate.ToString(), out sdate);
        //        }
        //        catch
        //        {
        //            System.Windows.Forms.MessageBox.Show("Invalid From date.");
        //            return;
        //        }
        //    }
        //    if (DtpEdMetrics.SelectedDate.ToString() != "")
        //    {
        //        try
        //        {
        //            DateTime.TryParse(DtpEdMetrics.SelectedDate.ToString(), out edate);
        //        }
        //        catch
        //        {
        //            System.Windows.Forms.MessageBox.Show("Invalid To date.");
        //            return;
        //        }
        //    }

        //    double? _tUnit = 0;
        //    double? _negUnit = 0;
        //    double? _posUnit = 0;
        //    double? _netUnit = 0;
        //    double? _absUnit = 0;
        //    double? _tDolCnt = 0;
        //    double? _negDol = 0;
        //    double? _posDol = 0;
        //    double? _netDol = 0;
        //    double? _absDol = 0;
        //    int _locCnt = 0;
        //    int _firPass = 0;
        //    int _secPass = 0;
        //    const int _expZones = 5;
        //    int _cntZones = 0;
        //    string _area = TxtArea.Text;
        //    int _month = edate.Month;
        //    int _qtr = (_month - 1) / 3 + 1;
        //    bool _ovrWrite = false;

        //    if (_area.ToString() == "")
        //    {
        //        System.Windows.Forms.MessageBox.Show("Count area must be entered.");
        //        return;
        //    }

        //    var metCheck = (from a in xps.MetricsInvs
        //                    select a.Date).ToList();
        //    foreach (DateTime _metCheck in metCheck)
        //    {
        //        if (_metCheck.Date == edate)
        //        {
        //            System.Windows.Forms.DialogResult _dlg = System.Windows.Forms.MessageBox.Show(
        //                "The metrics for week ending " + edate.ToString() + "has already been entered.\n" +
        //                "Would you like to overwrite the previously entered metrics?", "Metrics Already Entered", System.Windows.Forms.MessageBoxButtons.YesNoCancel);

        //            if (_dlg == System.Windows.Forms.DialogResult.Yes)
        //            {
        //                _ovrWrite = true;
        //            }
        //            else if (_dlg == System.Windows.Forms.DialogResult.Cancel)
        //            {
        //                return;
        //            }
        //        }
        //    }

        //    List<CCTracker> _valueData = new List<CCTracker>();
        //    var valueData = (from a in xps.CCTrackers
        //                     where a.CountDate >= sdate && a.CountDate <= edate
        //                     select a).ToList();
        //    _valueData = valueData;

        //    foreach (CCTracker cc in _valueData)
        //    {
        //        _tUnit += cc.CountedQTY;
        //        _tDolCnt += cc.CountedQTY * cc.UnitCost;
        //        _netDol += cc.Difference * cc.UnitCost;
        //        _netUnit += cc.Difference;

        //        if (cc.Difference != 0)
        //        {
        //            _secPass++;
        //            if (cc.Difference < 0)
        //            {
        //                _negUnit += cc.Difference;
        //                _negDol += cc.Difference * cc.UnitCost;
        //                _absUnit += cc.Difference * (-1);
        //                _absDol += ((cc.Difference * cc.UnitCost) * (-1));
        //            }
        //            else if (cc.Difference > 0)
        //            {
        //                _posUnit += cc.Difference;
        //                _posDol += cc.Difference * cc.UnitCost;
        //                _absUnit += cc.Difference;
        //                _absDol += cc.Difference * cc.UnitCost;
        //            }
        //        }
        //        _locCnt++;
        //        if (cc.Action != null && cc.Action.ToString() != "")
        //        {
        //            _firPass++;
        //        }
        //    }

        //    List<int?> _zoneData = new List<int?>();
        //    var zoneData = (from a in xps.CountSchedules
        //                    where a.ActualDate >= sdate && a.ActualDate <= edate
        //                    select a.Zone).Distinct().ToList();
        //    _zoneData = zoneData;
        //    _cntZones = _zoneData.Count();

        //    MetricsInv metrics = new MetricsInv();
        //    if (_ovrWrite)
        //    {
        //        var _metrics = (from a in xps.MetricsInvs
        //                        where a.Date == edate
        //                        select a).SingleOrDefault();
        //        metrics = _metrics;
        //    }
        //    metrics.Date = edate;
        //    metrics.TotalUnits = _tUnit;
        //    metrics.UnitsSub = _negUnit;
        //    metrics.UnitsAdd = _posUnit;
        //    metrics.UnitsNet = _netUnit;
        //    metrics.UnitsAbs = _absUnit;
        //    metrics.TotalValue = _tDolCnt;
        //    metrics.ValueSub = _negDol;
        //    metrics.ValueAdd = _posDol;
        //    metrics.ValueNet = _netDol;
        //    metrics.ValueAbs = _absDol;
        //    metrics.TotalLocations = _locCnt;
        //    metrics.FirstPassLocations = _firPass;
        //    metrics.SecondPassLocations = _secPass;
        //    metrics.ExpectedZones = _expZones;
        //    metrics.CountedZones = _cntZones;
        //    metrics.CountArea = _area;
        //    metrics.CountYear = DateTime.Now.Year;
        //    metrics.Qtr = _qtr;
        //    metrics.CountMonth = _month;
        //    if (!_ovrWrite)
        //    {
        //        xps.MetricsInvs.Add(metrics);
        //    }
        //    xps.SaveChanges();
        //    System.Windows.Forms.MessageBox.Show("Metrics saved");
        //}

        //private void PopulateHighlvl()
        //{
        //    int _month = DateTime.Now.Month;
        //    int currQtr = (_month - 1) / 3 + 1;
        //    int _selYear = Convert.ToInt32(CboYear.SelectedItem);
        //    int _selMon = Convert.ToInt32(CboMonth.SelectedItem);
        //    DateTime _priorWk = new DateTime();
        //    DateTime _repWk = new DateTime();
        //    if (DtpPrior.SelectedDate.ToString() != "")
        //    {
        //        try
        //        {
        //            DateTime.TryParse(DtpPrior.SelectedDate.ToString(), out _priorWk);
        //        }
        //        catch
        //        {
        //            System.Windows.Forms.MessageBox.Show("Invalid Prior Week date.");
        //            return;
        //        }
        //    }
        //    if (DtpReport.SelectedDate.ToString() != "")
        //    {
        //        try
        //        {
        //            DateTime.TryParse(DtpReport.SelectedDate.ToString(), out _repWk);
        //        }
        //        catch
        //        {
        //            System.Windows.Forms.MessageBox.Show("Invalid From date.");
        //            return;
        //        }
        //    }
        //    dt = new DataTable();
        //    dt.Columns.Add("PulsePoints");
        //    dt.Columns.Add("Goals");
        //    dt.Columns.Add("Q4", typeof(Double));
        //    dt.Columns.Add("Q3", typeof(Double));
        //    dt.Columns.Add("Q2", typeof(Double));
        //    dt.Columns.Add("Q1", typeof(Double));
        //    dt.Columns.Add("YTD", typeof(Double));
        //    dt.Columns.Add("MTD", typeof(Double));
        //    dt.Columns.Add("PriorWeek", typeof(Double));
        //    dt.Columns.Add("ReportWeek", typeof(Double));

        //    for (int i = 0; i <= 7; i++)
        //    {
        //        dt.Rows.Add();
        //    }

        //    dt.Rows[0][0] = "Unit Net";
        //    dt.Rows[1][0] = "Unit ABS";
        //    dt.Rows[2][0] = "Value Net";
        //    dt.Rows[3][0] = "Value ABS";
        //    dt.Rows[4][0] = "1st Pass Loc Acc";
        //    dt.Rows[5][0] = "2nd Pass Loc Acc";
        //    dt.Rows[6][0] = "Geo Comp Trk";
        //    dt.Rows[7][0] = "GCNIL";
        //    dt.Rows[0][1] = "99%";
        //    dt.Rows[1][1] = "99%";
        //    dt.Rows[2][1] = "99%";
        //    dt.Rows[3][1] = "99%";
        //    dt.Rows[4][1] = "99%";
        //    dt.Rows[5][1] = "99%";
        //    dt.Rows[6][1] = "100%";
        //    dt.Rows[7][1] = "25,000";

        //    dtDetails = new DataTable();
        //    dtDetails.Columns.Add("PulsePoints");
        //    dtDetails.Columns.Add("Goals");
        //    dtDetails.Columns.Add("Q4", typeof(Double));
        //    dtDetails.Columns.Add("Q3", typeof(Double));
        //    dtDetails.Columns.Add("Q2", typeof(Double));
        //    dtDetails.Columns.Add("Q1", typeof(Double));
        //    dtDetails.Columns.Add("YTD", typeof(Double));
        //    dtDetails.Columns.Add("MTD", typeof(Double));
        //    dtDetails.Columns.Add("PriorWeek", typeof(Double));
        //    dtDetails.Columns.Add("ReportWeek", typeof(Double));

        //    for (int i = 0; i < 16; i++)
        //    {
        //        dtDetails.Rows.Add();
        //    }

        //    dtDetails.Rows[0][0] = "Total Units Counts";
        //    dtDetails.Rows[1][0] = "Qty of minus (ADJ)";
        //    dtDetails.Rows[2][0] = "Qty of plus (ADJ)";
        //    dtDetails.Rows[3][0] = "Total Variance (NET)";
        //    dtDetails.Rows[4][0] = "Total Variance (ABS)";
        //    dtDetails.Rows[5][0] = "Total $$ Count";
        //    dtDetails.Rows[6][0] = "$$ of minus (ADJ)";
        //    dtDetails.Rows[7][0] = "$$ of plus (ADJ)";
        //    dtDetails.Rows[8][0] = "Total Variance (NET)";
        //    dtDetails.Rows[9][0] = "Total Variance (ABS)";
        //    dtDetails.Rows[10][0] = "# of Locations Counted";
        //    dtDetails.Rows[11][0] = "1st pass # of Locations Error";
        //    dtDetails.Rows[12][0] = "2nd pass # of Locations Error";
        //    dtDetails.Rows[13][0] = "Total Expected Zones";
        //    dtDetails.Rows[14][0] = "Total Counted Zones";
        //    dtDetails.Rows[15][0] = "$$ in GCNIL";

        //    bool _Q4 = false;
        //    bool _Q3 = false;
        //    bool _Q2 = false;
        //    bool _Q1 = false;

        //    List<MetricsInv> metrics;

        //    List<int?> _qtrs = new List<int?>();

        //    var qtr = (from a in xps.MetricsInvs
        //               where a.CountYear == _selYear
        //               select a.Qtr).Distinct().ToList();
        //    _qtrs = qtr;

        //    for (int i = 0; i < _qtrs.Count; i++)
        //    {
        //        switch (_qtrs[i])
        //        {
        //            case 4:
        //                _Q4 = true;
        //                break;

        //            case 3:
        //                _Q3 = true;
        //                break;

        //            case 2:
        //                _Q2 = true;
        //                break;

        //            case 1:
        //                _Q1 = true;
        //                break;
        //        }
        //    }

        //    if (_Q4)
        //    {
        //        metrics = new List<MetricsInv>();
        //        var q4 = (from a in xps.MetricsInvs
        //                  where a.CountYear == _selYear && a.Qtr == 4
        //                  select a).ToList();
        //        metrics = q4;

        //        GetPercents(metrics);
        //        GetDetails(metrics);

        //        for (int i = 0; i < dt.Rows.Count - 1; i++)
        //        {
        //            dt.Rows[i][2] = MetricsArr[i];
        //        }

        //        for (int i = 0; i < dtDetails.Rows.Count - 1; i++)
        //        {
        //            dtDetails.Rows[i][2] = DetailsArr[i];
        //        }
        //    }

        //    if (_Q3)
        //    {
        //        metrics = new List<MetricsInv>();
        //        var q3 = (from a in xps.MetricsInvs
        //                  where a.CountYear == _selYear && a.Qtr == 3
        //                  select a).ToList();
        //        metrics = q3;

        //        GetPercents(metrics);
        //        GetDetails(metrics);

        //        for (int i = 0; i < dt.Rows.Count - 1; i++)
        //        {
        //            dt.Rows[i][3] = MetricsArr[i];
        //        }

        //        for (int i = 0; i < dtDetails.Rows.Count - 1; i++)
        //        {
        //            dtDetails.Rows[i][3] = DetailsArr[i];
        //        }
        //    }

        //    if (_Q2)
        //    {
        //        metrics = new List<MetricsInv>();
        //        var q2 = (from a in xps.MetricsInvs
        //                  where a.CountYear == _selYear && a.Qtr == 2
        //                  select a).ToList();
        //        metrics = q2;

        //        GetPercents(metrics);
        //        GetDetails(metrics);

        //        for (int i = 0; i < dt.Rows.Count - 1; i++)
        //        {
        //            dt.Rows[i][4] = MetricsArr[i];
        //        }

        //        for (int i = 0; i < dtDetails.Rows.Count - 1; i++)
        //        {
        //            dtDetails.Rows[i][4] = DetailsArr[i];
        //        }
        //    }

        //    if (_Q1)
        //    {
        //        metrics = new List<MetricsInv>();
        //        var q1 = (from a in xps.MetricsInvs
        //                  where a.CountYear == _selYear && a.Qtr == 1
        //                  select a).ToList();
        //        metrics = q1;

        //        GetPercents(metrics);
        //        GetDetails(metrics);

        //        for (int i = 0; i < dt.Rows.Count - 1; i++)
        //        {
        //            dt.Rows[i][5] = MetricsArr[i];
        //        }

        //        for (int i = 0; i < dtDetails.Rows.Count; i++)
        //        {
        //            dtDetails.Rows[i][5] = DetailsArr[i];
        //        }
        //    }

        //    metrics = new List<MetricsInv>();
        //    var year = (from a in xps.MetricsInvs
        //                where a.CountYear == _selYear
        //                select a).ToList();

        //    metrics = year;

        //    GetPercents(metrics);
        //    GetDetails(metrics);

        //    for (int i = 0; i < dt.Rows.Count - 1; i++)
        //    {
        //        dt.Rows[i][6] = MetricsArr[i];
        //    }

        //    for (int i = 0; i < dtDetails.Rows.Count - 1; i++)
        //    {
        //        dtDetails.Rows[i][6] = DetailsArr[i];
        //    }

        //    metrics = new List<MetricsInv>();
        //    var month = (from a in xps.MetricsInvs
        //                 where a.CountYear == _selYear && a.CountMonth == _selMon
        //                 select a).ToList();

        //    metrics = month;

        //    if (metrics.Count > 0)
        //    {
        //        GetPercents(metrics);
        //        GetDetails(metrics);

        //        for (int i = 0; i < dt.Rows.Count - 1; i++)
        //        {
        //            dt.Rows[i][7] = MetricsArr[i];
        //        }

        //        for (int i = 0; i < dtDetails.Rows.Count - 1; i++)
        //        {
        //            dtDetails.Rows[i][7] = DetailsArr[i];
        //        }
        //    }

        //    if (_priorWk.ToString() != "")
        //    {
        //        metrics = new List<MetricsInv>();
        //        var priorWk = (from a in xps.MetricsInvs
        //                       where a.Date == _priorWk
        //                       select a).ToList();
        //        metrics = priorWk;

        //        if (metrics.Count > 0)
        //        {
        //            GetPercents(metrics);
        //            GetDetails(metrics);

        //            for (int i = 0; i < dt.Rows.Count - 1; i++)
        //            {
        //                dt.Rows[i][8] = MetricsArr[i];
        //            }

        //            for (int i = 0; i < dtDetails.Rows.Count - 1; i++)
        //            {
        //                dtDetails.Rows[i][8] = DetailsArr[i];
        //            }
        //        }
        //    }

        //    if (_repWk.ToString() != "")
        //    {
        //        metrics = new List<MetricsInv>();
        //        var repWk = (from a in xps.MetricsInvs
        //                     where a.Date == _repWk
        //                     select a).ToList();
        //        metrics = repWk;

        //        if (metrics.Count > 0)
        //        {
        //            GetPercents(metrics);
        //            GetDetails(metrics);

        //            for (int i = 0; i < dt.Rows.Count - 1; i++)
        //            {
        //                dt.Rows[i][9] = MetricsArr[i];
        //            }

        //            for (int i = 0; i < dtDetails.Rows.Count - 1; i++)
        //            {
        //                dtDetails.Rows[i][9] = DetailsArr[i];
        //            }
        //        }
        //    }
        //    LblPrior.Content = _priorWk.ToString("MM/dd/yyyy");
        //    LblReport.Content = _repWk.ToString("MM/dd/yyyy");

        //    CollectionViewSource cvs;
        //    cvs = (CollectionViewSource)(FindResource("cvs"));
        //    cvs.Source = dt;
        //    DgMetricDetails.ItemsSource = dtDetails.DefaultView;
        //    //PopulateDetails();
        //}

        //public double?[] GetDetails(List<MetricsInv> metrics)
        //{
        //    DetailsArr = new double?[16];
        //    DetailsArr[0] = metrics.Sum(_sum => _sum.TotalUnits);
        //    DetailsArr[1] = metrics.Sum(_sum => _sum.UnitsSub);
        //    DetailsArr[2] = metrics.Sum(_sum => _sum.UnitsAdd);
        //    DetailsArr[3] = metrics.Sum(_sum => _sum.UnitsNet);
        //    DetailsArr[4] = metrics.Sum(_sum => _sum.UnitsAbs);
        //    DetailsArr[5] = metrics.Sum(_sum => _sum.TotalValue);
        //    DetailsArr[6] = metrics.Sum(_sum => _sum.ValueSub);
        //    DetailsArr[7] = metrics.Sum(_sum => _sum.ValueAdd);
        //    DetailsArr[8] = metrics.Sum(_sum => _sum.ValueNet);
        //    DetailsArr[9] = metrics.Sum(_sum => _sum.ValueAbs);
        //    DetailsArr[10] = metrics.Sum(_sum => _sum.TotalLocations);
        //    DetailsArr[11] = metrics.Sum(_sum => _sum.FirstPassLocations);
        //    DetailsArr[12] = metrics.Sum(_sum => _sum.SecondPassLocations);
        //    DetailsArr[13] = metrics.Sum(_sum => _sum.ExpectedZones);
        //    DetailsArr[14] = metrics.Sum(_sum => _sum.CountedZones);
        //    DetailsArr[15] = metrics.Sum(_sum => _sum.GCNIL);

        //    for (int i = 0; i < DetailsArr.GetUpperBound(0); i++)
        //    {
        //        DetailsArr[i] = Math.Round(Convert.ToDouble(DetailsArr[i]), 2);
        //    }
        //    return DetailsArr;
        //}

        //public double[] GetPercents(List<MetricsInv> metrics)
        //{
        //    MetricsArr = new double[7];
        //    double? _tUnit = 0;
        //    double? _unitNet = 0;
        //    double? _pcntUnitNet = 0;
        //    double? _unitABS = 0;
        //    double? _pcntUnitABS = 0;
        //    double? _tValue = 0;
        //    double? _valueNet = 0;
        //    double? _pcntValueNet = 0;
        //    double? _valueABS = 0;
        //    double? _pcntValueABS = 0;
        //    double? _tCnt = 0;
        //    double? _firPass = 0;
        //    double? _pcntFirPass = 0;
        //    double? _secPass = 0;
        //    double? _pcntSecPass = 0;
        //    double? _tZones = 0;
        //    double? _zoneComp = 0;
        //    double? _pcntZoneComp = 0;

        //    _tUnit = metrics.Sum(_sum => _sum.TotalUnits);
        //    _unitNet = metrics.Sum(_sum => _sum.UnitsNet);
        //    _unitABS = metrics.Sum(_sum => _sum.UnitsAbs);
        //    if (_unitNet < 0)
        //    {
        //        _pcntUnitNet = ((_tUnit + _unitNet) / _tUnit) * 100;
        //    }
        //    else
        //    {
        //        _pcntUnitNet = ((_tUnit - _unitNet) / _tUnit) * 100;
        //    }
        //    _pcntUnitABS = ((_tUnit - _unitABS) / _tUnit) * 100;

        //    _tValue = metrics.Sum(_sum => _sum.TotalValue);
        //    _valueNet = metrics.Sum(_sum => _sum.ValueNet);
        //    _valueABS = metrics.Sum(_sum => _sum.ValueAbs);
        //    if (_valueNet < 0)
        //    {
        //        _pcntValueNet = ((_tValue + _valueNet) / _tValue) * 100;
        //    }
        //    else
        //    {
        //        _pcntValueNet = ((_tValue - _valueNet) / _tValue) * 100;
        //    }
        //    _pcntValueABS = ((_tValue - _valueABS) / _tValue) * 100;

        //    _tCnt = metrics.Sum(_sum => _sum.TotalLocations);
        //    _firPass = metrics.Sum(_sum => _sum.FirstPassLocations);
        //    _secPass = metrics.Sum(_sum => _sum.SecondPassLocations);
        //    _pcntFirPass = ((_tCnt - _firPass) / _tCnt) * 100;
        //    _pcntSecPass = ((_tCnt - _secPass) / _tCnt) * 100;

        //    _tZones = metrics.Sum(_sum => _sum.ExpectedZones);
        //    _zoneComp = metrics.Sum(_sum => _sum.CountedZones);
        //    _pcntZoneComp = ((1 - (_tZones - _zoneComp) / _tZones)) * 100;

        //    _pcntUnitNet = Math.Round(Convert.ToDouble(_pcntUnitNet), 2);
        //    _pcntUnitABS = Math.Round(Convert.ToDouble(_pcntUnitABS), 2);
        //    _pcntValueNet = Math.Round(Convert.ToDouble(_pcntValueNet), 2);
        //    _pcntValueABS = Math.Round(Convert.ToDouble(_pcntValueABS), 2);
        //    _pcntFirPass = Math.Round(Convert.ToDouble(_pcntFirPass), 2);
        //    _pcntSecPass = Math.Round(Convert.ToDouble(_pcntSecPass), 2);
        //    _pcntZoneComp = Math.Round(Convert.ToDouble(_pcntZoneComp), 2);

        //    MetricsArr[0] = Convert.ToDouble(_pcntUnitNet);
        //    MetricsArr[1] = Convert.ToDouble(_pcntUnitABS);
        //    MetricsArr[2] = Convert.ToDouble(_pcntValueNet);
        //    MetricsArr[3] = Convert.ToDouble(_pcntValueABS);
        //    MetricsArr[4] = Convert.ToDouble(_pcntFirPass);
        //    MetricsArr[5] = Convert.ToDouble(_pcntSecPass);
        //    MetricsArr[6] = Convert.ToDouble(_pcntZoneComp);

        //    return MetricsArr;
        //}

        //#endregion Metrics Grid Methods

        //#endregion Methods

        //#region Special Events

        //private void cboYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    FillMonthList();
        //}

        //#endregion Special Events
        
    }

    //#region Value Converter

    //public class NameToBrushConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (value is double)
    //        {
    //            double quantity = (double)value;
    //            if (quantity >= 99)
    //            { return Brushes.MediumSeaGreen; }
    //            else
    //            { return Brushes.IndianRed; }
    //        }
    //        return Brushes.White;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotSupportedException();
    //    }
    //}

    //#endregion Value Converter
}