using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Helpers;

namespace XPRES.Departments.Inventory.ViewModels
{
    public class CycoMetricsVm : ViewModelBase
    {
        #region Constructor

        public CycoMetricsVm()
        {
            SetupTables();
            _listYear = new List<int?>();
            _listMonth = new ObservableCollection<int?>();
            FillYears();
        }

        #endregion Constructor

        #region Properties

        private double?[] _detailsArr;
        private double?[] _metricsArr;
        private DataTable _dt;
        private DataTable _dtDetails;

        private DataView _detailsView;

        public DataView DetailsView
        {
            get { return _detailsView; }
            set
            {
                _detailsView = value;
                OnPropertyChanged();
            }
        }

        private DataView _metricsView;

        public DataView MetricsView
        {
            get { return _metricsView; }
            set
            {
                _metricsView = value;
                OnPropertyChanged();
            }
        }

        private List<int?> _listYear;

        public List<int?> ListYear
        {
            get { return _listYear; }
            set
            {
                _listYear = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<int?> _listMonth;

        public ObservableCollection<int?> ListMonth
        {
            get { return _listMonth; }
            set
            {
                _listMonth = value;
                OnPropertyChanged();
            }
        }

        private int? _selYear;

        public int? SelYear
        {
            get { return _selYear; }
            set
            {
                _selYear = value;
                OnPropertyChanged();
                FillMonths();
            }
        }

        private int? _selMonth;

        public int? SelMonth
        {
            get { return _selMonth; }
            set
            {
                _selMonth = value;
                OnPropertyChanged();
            }
        }

        private string _fromDate;

        public string FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                OnPropertyChanged();
            }
        }

        private string _toDate;

        public string ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                OnPropertyChanged();
            }
        }

        private string _countArea;

        public string CountArea
        {
            get { return _countArea; }
            set
            {
                _countArea = value;
                OnPropertyChanged();
            }
        }

        private string _prevWeek;

        public string PrevWeek
        {
            get { return _prevWeek; }
            set
            {
                _prevWeek = value;
                OnPropertyChanged();
            }
        }

        private string _currWeek;

        public string CurrWeek
        {
            get { return _currWeek; }
            set
            {
                _currWeek = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        public ICommand CalcMetricsCommand => new RelayCommand(x => CalcMetrics());

        public ICommand ViewMetricsCommand => new RelayCommand(x => ViewMetrics());

        #endregion ICommand Members

        #region Methods

        private void FillYears()
        {
            try
            {
                _listYear = (from _a in new XpresEntities().MetricsInvs
                                  select _a.CountYear).Distinct().ToList();
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving year list: " + _ex.Message);
            }
        }

        private void FillMonths()
        {
            try
            {
                if (_listMonth.Any()) _listMonth.Clear();
               List<int?> _monthQuery = (from _a in new XpresEntities().MetricsInvs
                             where _a.CountYear == _selYear
                             select _a.CountMonth).Distinct().ToList();
                foreach (int? _month in _monthQuery)
                {
                    _listMonth.Add(_month);
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving month list: " + _ex.Message);
            }
        }

        private void SetupTables()
        {
            _dt = new DataTable();
            _dt.Columns.Add("PulsePoints");
            _dt.Columns.Add("Goals");
            _dt.Columns.Add("Q4", typeof(Double));
            _dt.Columns.Add("Q3", typeof(Double));
            _dt.Columns.Add("Q2", typeof(Double));
            _dt.Columns.Add("Q1", typeof(Double));
            _dt.Columns.Add("YTD", typeof(Double));
            _dt.Columns.Add("MTD", typeof(Double));
            _dt.Columns.Add("PriorWeek", typeof(Double));
            _dt.Columns.Add("ReportWeek", typeof(Double));

            _dtDetails = new DataTable();
            _dtDetails.Columns.Add("PulsePoints");
            _dtDetails.Columns.Add("Goals");
            _dtDetails.Columns.Add("Q4", typeof(Double));
            _dtDetails.Columns.Add("Q3", typeof(Double));
            _dtDetails.Columns.Add("Q2", typeof(Double));
            _dtDetails.Columns.Add("Q1", typeof(Double));
            _dtDetails.Columns.Add("YTD", typeof(Double));
            _dtDetails.Columns.Add("MTD", typeof(Double));
            _dtDetails.Columns.Add("PriorWeek", typeof(Double));
            _dtDetails.Columns.Add("ReportWeek", typeof(Double));
            _metricsView = _dt.DefaultView;
            _detailsView = _dtDetails.DefaultView;
        }

        private void CalcMetrics()
        {
            DateTime _sdate;
            DateTime _edate;
            bool _ovrWrite = false;

            if (_fromDate != "" || _toDate != "")
            {
                try
                {
                    DateTime.TryParse(_fromDate, out _sdate);
                    DateTime.TryParse(_toDate, out _edate);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show(@"Invalid From date.");
                    return;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(@"Please enter a date range.");
                return;
            }

            var _metCheck = (from _a in new XpresEntities().MetricsInvs
                             select _a.Date).ToList();
            foreach (DateTime _date in _metCheck)
            {
                if (_date.Date == _edate)
                {
                    System.Windows.Forms.DialogResult _dlg = System.Windows.Forms.MessageBox.Show(
                        @"The metrics for week ending " + _edate.ToString() + @"has already been entered.\n" +
                        @"Would you like to overwrite the previously entered metrics?", @"Metrics Already Entered", System.Windows.Forms.MessageBoxButtons.YesNoCancel);

                    if (_dlg == System.Windows.Forms.DialogResult.Yes)
                    {
                        _ovrWrite = true;
                    }
                    else if (_dlg == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }

            double? _tUnit = 0;
            double? _negUnit = 0;
            double? _posUnit = 0;
            double? _netUnit = 0;
            double? _absUnit = 0;
            double? _tDolCnt = 0;
            double? _negDol = 0;
            double? _posDol = 0;
            double? _netDol = 0;
            double? _absDol = 0;
            int _locCnt = 0;
            int _firPass = 0;
            int _secPass = 0;
            const int expZones = 5;
            int _cntZones = 0;
            string _area = _countArea;
            int _month = _edate.Month;
            int _qtr = (_month - 1) / 3 + 1;

            if (_area.ToString() == "")
            {
                System.Windows.Forms.MessageBox.Show(@"Count area must be entered.");
                return;
            }

            List<CCTracker> _valueData = (from _a in new XpresEntities().CCTrackers
                             where _a.CountDate >= _sdate && _a.CountDate <= _edate
                             select _a).ToList();

            foreach (CCTracker _cc in _valueData)
            {
                _tUnit += _cc.CountedQTY;
                _tDolCnt += _cc.CountedQTY * _cc.UnitCost;
                _netDol += _cc.Difference * _cc.UnitCost;
                _netUnit += _cc.Difference;

                if (_cc.Difference != 0)
                {
                    _secPass++;
                    if (_cc.Difference < 0)
                    {
                        _negUnit += _cc.Difference;
                        _negDol += _cc.Difference * _cc.UnitCost;
                        _absUnit += _cc.Difference * (-1);
                        _absDol += ((_cc.Difference * _cc.UnitCost) * (-1));
                    }
                    else if (_cc.Difference > 0)
                    {
                        _posUnit += _cc.Difference;
                        _posDol += _cc.Difference * _cc.UnitCost;
                        _absUnit += _cc.Difference;
                        _absDol += _cc.Difference * _cc.UnitCost;
                    }
                }
                _locCnt++;
                if (_cc.Action != null && _cc.Action.ToString() != "")
                {
                    _firPass++;
                }
            }

            List<int?> _zoneData = (from _a in new XpresEntities().CountSchedules
                            where _a.ActualDate >= _sdate && _a.ActualDate <= _edate
                            select _a.Zone).Distinct().ToList();

            _cntZones = _zoneData.Count;

            XpresEntities _xps = new XpresEntities();
            MetricsInv _metrics = new MetricsInv();
            if (_ovrWrite)
            {
                var _metricsOverwrite = (from _a in _xps.MetricsInvs
                                where _a.Date == _edate
                                select _a).SingleOrDefault();
                _metrics = _metricsOverwrite;
            }
            _metrics.Date = _edate;
            _metrics.TotalUnits = _tUnit;
            _metrics.UnitsSub = _negUnit;
            _metrics.UnitsAdd = _posUnit;
            _metrics.UnitsNet = _netUnit;
            _metrics.UnitsAbs = _absUnit;
            _metrics.TotalValue = _tDolCnt;
            _metrics.ValueSub = _negDol;
            _metrics.ValueAdd = _posDol;
            _metrics.ValueNet = _netDol;
            _metrics.ValueAbs = _absDol;
            _metrics.TotalLocations = _locCnt;
            _metrics.FirstPassLocations = _firPass;
            _metrics.SecondPassLocations = _secPass;
            _metrics.ExpectedZones = expZones;
            _metrics.CountedZones = _cntZones;
            _metrics.CountArea = _area;
            _metrics.CountYear = DateTime.Now.Year;
            _metrics.Qtr = _qtr;
            _metrics.CountMonth = _month;
            if (!_ovrWrite)
            {
                _xps.MetricsInvs.Add(_metrics);
            }
            _xps.SaveChanges();
            System.Windows.Forms.MessageBox.Show(@"Metrics saved");
        }

        private void ViewMetrics()
        {
            _dt.Clear();
            _dtDetails.Clear();
            if (_dt.Rows.Count > 0) _dt.Clear();

            for (int _i = 0; _i <= 7; _i++) { _dt.Rows.Add(); }

            _dt.Rows[0][0] = "Unit Net";
            _dt.Rows[1][0] = "Unit ABS";
            _dt.Rows[2][0] = "Value Net";
            _dt.Rows[3][0] = "Value ABS";
            _dt.Rows[4][0] = "1st Pass Loc Acc";
            _dt.Rows[5][0] = "2nd Pass Loc Acc";
            _dt.Rows[6][0] = "Geo Comp Trk";
            _dt.Rows[7][0] = "GCNIL";
            _dt.Rows[0][1] = "99%";
            _dt.Rows[1][1] = "99%";
            _dt.Rows[2][1] = "99%";
            _dt.Rows[3][1] = "99%";
            _dt.Rows[4][1] = "99%";
            _dt.Rows[5][1] = "99%";
            _dt.Rows[6][1] = "100%";
            _dt.Rows[7][1] = "25,000";

            for (int _i = 0; _i < 16; _i++) { _dtDetails.Rows.Add(); }

            _dtDetails.Rows[0][0] = "Total Units Counts";
            _dtDetails.Rows[1][0] = "Qty of minus (ADJ)";
            _dtDetails.Rows[2][0] = "Qty of plus (ADJ)";
            _dtDetails.Rows[3][0] = "Total Variance (NET)";
            _dtDetails.Rows[4][0] = "Total Variance (ABS)";
            _dtDetails.Rows[5][0] = "Total $$ Count";
            _dtDetails.Rows[6][0] = "$$ of minus (ADJ)";
            _dtDetails.Rows[7][0] = "$$ of plus (ADJ)";
            _dtDetails.Rows[8][0] = "Total Variance (NET)";
            _dtDetails.Rows[9][0] = "Total Variance (ABS)";
            _dtDetails.Rows[10][0] = "# of Locations Counted";
            _dtDetails.Rows[11][0] = "1st pass # of Locations Error";
            _dtDetails.Rows[12][0] = "2nd pass # of Locations Error";
            _dtDetails.Rows[13][0] = "Total Expected Zones";
            _dtDetails.Rows[14][0] = "Total Counted Zones";
            _dtDetails.Rows[15][0] = "$$ in GCNIL";

            DateTime _priorWk;
            DateTime _repWk;
            if (_prevWeek != "" || _currWeek != "")
            {
                try
                {
                    DateTime.TryParse(_prevWeek, out _priorWk);
                    DateTime.TryParse(_currWeek, out _repWk);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show(@"Invalid From date.");
                    return;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(@"Please enter a date range.");
                return;
            }

            var _yearQuery = (from _a in new XpresEntities().MetricsInvs
                                 where _a.CountYear == _selYear
                                 select _a);
            GetMetrics(6, _yearQuery);

            var _monthsQuery = _yearQuery.Where(x => x.CountMonth == _selMonth);
            GetMetrics(7, _monthsQuery);

            if (!string.IsNullOrEmpty(_prevWeek))
            {
                var _prevQuery = _yearQuery.Where(x => x.Date == _priorWk);
                GetMetrics(8, _prevQuery);
            }

            if(!string.IsNullOrEmpty(_currWeek))
            {
                var _currQuery = _yearQuery.Where(x => x.Date == _repWk);
                GetMetrics(9, _currQuery);
            }

            var _qtrsQuery = _yearQuery.Select(x => x.Qtr);

            foreach (int? _q in _qtrsQuery)
            {
                switch (_q)
                {
                    case 4:
                        var _q4 = _yearQuery.Where(x => x.Qtr == _q);
                        GetMetrics(2, _q4);
                        break;

                    case 3:
                        var _q3 = _yearQuery.Where(x => x.Qtr == _q);
                        GetMetrics(3, _q3);
                        break;

                    case 2:
                        var _q2 = _yearQuery.Where(x => x.Qtr == _q);
                        GetMetrics(4, _q2);
                        break;

                    case 1:
                        var _q1 = _yearQuery.Where(x => x.Qtr == _q);
                        GetMetrics(5, _q1);
                        break;
                }
            }

            _metricsView = _dt.DefaultView;
        }

        private void GetMetrics(int colNum, IQueryable<MetricsInv> metricsQuery)
        {
            GetPercents(metricsQuery);
            GetDetails(metricsQuery);

            for (int _i = 0; _i < _dt.Rows.Count - 1; _i++)
            {
                _dt.Rows[_i][colNum] = _metricsArr[_i];
            }

            for (int _i = 0; _i < _dtDetails.Rows.Count - 1; _i++)
            {
                _dtDetails.Rows[_i][colNum] = _detailsArr[_i];
            }
        }

        private double?[] GetDetails(IQueryable<MetricsInv> metrics)
        {
            _detailsArr = new double?[16];
            _detailsArr[0] = metrics.Sum(sum => sum.TotalUnits);
            _detailsArr[1] = metrics.Sum(sum => sum.UnitsSub);
            _detailsArr[2] = metrics.Sum(sum => sum.UnitsAdd);
            _detailsArr[3] = metrics.Sum(sum => sum.UnitsNet);
            _detailsArr[4] = metrics.Sum(sum => sum.UnitsAbs);
            _detailsArr[5] = metrics.Sum(sum => sum.TotalValue);
            _detailsArr[6] = metrics.Sum(sum => sum.ValueSub);
            _detailsArr[7] = metrics.Sum(sum => sum.ValueAdd);
            _detailsArr[8] = metrics.Sum(sum => sum.ValueNet);
            _detailsArr[9] = metrics.Sum(sum => sum.ValueAbs);
            _detailsArr[10] = metrics.Sum(sum => sum.TotalLocations);
            _detailsArr[11] = metrics.Sum(sum => sum.FirstPassLocations);
            _detailsArr[12] = metrics.Sum(sum => sum.SecondPassLocations);
            _detailsArr[13] = metrics.Sum(sum => sum.ExpectedZones);
            _detailsArr[14] = metrics.Sum(sum => sum.CountedZones);
            _detailsArr[15] = metrics.Sum(sum => sum.GCNIL);

            for (int _i = 0; _i < _detailsArr.GetUpperBound(0); _i++)
            {
                _detailsArr[_i] = Math.Round(Convert.ToDouble(_detailsArr[_i]), 2);
            }
            return _detailsArr;
        }

        private double?[] GetPercents(IQueryable<MetricsInv> metrics)
        {
            _metricsArr = null;
            _metricsArr = new double?[7];
            double? _pcntUnitNet;
            double? _pcntValueNet;

            double? _tUnit = metrics.Sum(sum => sum.TotalUnits);
            double? _unitNet = metrics.Sum(sum => sum.UnitsNet);
            double? _unitAbs = metrics.Sum(sum => sum.UnitsAbs);
            if (_unitNet < 0)
            {
                _pcntUnitNet = ((_tUnit + _unitNet) / _tUnit) * 100;
            }
            else
            {
                _pcntUnitNet = ((_tUnit - _unitNet) / _tUnit) * 100;
            }
            double? _pcntUnitAbs = ((_tUnit - _unitAbs) / _tUnit) * 100;

            double? _tValue = metrics.Sum(sum => sum.TotalValue);
            double? _valueNet = metrics.Sum(sum => sum.ValueNet);
            double? _valueAbs = metrics.Sum(sum => sum.ValueAbs);
            if (_valueNet < 0)
            {
                _pcntValueNet = ((_tValue + _valueNet) / _tValue) * 100;
            }
            else
            {
                _pcntValueNet = ((_tValue - _valueNet) / _tValue) * 100;
            }
            double? _pcntValueAbs = ((_tValue - _valueAbs) / _tValue) * 100;

            double? _tCnt = metrics.Sum(sum => sum.TotalLocations);
            double? _firPass = metrics.Sum(sum => sum.FirstPassLocations);
            double? _secPass = metrics.Sum(sum => sum.SecondPassLocations);
            double? _pcntFirPass = ((_tCnt - _firPass) / _tCnt) * 100;
            double? _pcntSecPass = ((_tCnt - _secPass) / _tCnt) * 100;

            double? _tZones = metrics.Sum(sum => sum.ExpectedZones);
            double? _zoneComp = metrics.Sum(sum => sum.CountedZones);
#pragma warning disable RCS1032 // Remove redundant parentheses.
            double? _pcntZoneComp = ((1 - (_tZones - _zoneComp) / _tZones)) * 100;
#pragma warning restore RCS1032 // Remove redundant parentheses.

            _pcntUnitNet = Math.Round(Convert.ToDouble(_pcntUnitNet), 2);
            _pcntUnitAbs = Math.Round(Convert.ToDouble(_pcntUnitAbs), 2);
            _pcntValueNet = Math.Round(Convert.ToDouble(_pcntValueNet), 2);
            _pcntValueAbs = Math.Round(Convert.ToDouble(_pcntValueAbs), 2);
            _pcntFirPass = Math.Round(Convert.ToDouble(_pcntFirPass), 2);
            _pcntSecPass = Math.Round(Convert.ToDouble(_pcntSecPass), 2);
            _pcntZoneComp = Math.Round(Convert.ToDouble(_pcntZoneComp), 2);

            _metricsArr[0] = Convert.ToDouble(_pcntUnitNet);
            _metricsArr[1] = Convert.ToDouble(_pcntUnitAbs);
            _metricsArr[2] = Convert.ToDouble(_pcntValueNet);
            _metricsArr[3] = Convert.ToDouble(_pcntValueAbs);
            _metricsArr[4] = Convert.ToDouble(_pcntFirPass);
            _metricsArr[5] = Convert.ToDouble(_pcntSecPass);
            _metricsArr[6] = Convert.ToDouble(_pcntZoneComp);
            return _metricsArr;
        }

        #endregion Methods
    }
}
