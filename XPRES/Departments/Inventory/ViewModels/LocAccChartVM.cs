using System;
using System.Collections.Generic;
using System.Linq;
using XPRES.DAL;
using XPRES.Helpers;

namespace XPRES.Departments.Inventory.ViewModels
{
    public class LocAccChartVm : ViewModelBase
    {
        #region Constructor
        public LocAccChartVm()
        {
            _startWeek = 4;
            _stopWeek = 0;
            GetChartData(_startWeek, _stopWeek, false);
            GetChartData(_startWeek, _stopWeek, true);
        }

        #endregion Constructor

        #region Properties

        private int _startWeek;

        public int StartWeek
        {
            get { return _startWeek; }
            set
            {
                _startWeek = value;
                OnPropertyChanged();
            }
        }

        private int _stopWeek;

        public int StopWeek
        {
            get { return _stopWeek; }
            set
            {
                _stopWeek = value;
                OnPropertyChanged();
            }
        }

        private List<KeyValuePair<string, double>> _prevQtrSecondPass;

        public List<KeyValuePair<string, double>> PrevQtrSecondPass
        {
            get { return _prevQtrSecondPass; }
            set
            {
                _prevQtrSecondPass = value;
                OnPropertyChanged();
            }
        }

        private List<KeyValuePair<string, double>> _prevQtrFirstPass;

        public List<KeyValuePair<string, double>> PrevQtrFirstPass
        {
            get { return _prevQtrFirstPass; }
            set
            {
                _prevQtrFirstPass = value;
                OnPropertyChanged();
            }
        }

        private List<KeyValuePair<string, double>> _currQtrSecondPass;

        public List<KeyValuePair<string, double>> CurrQtrSecondPass
        {
            get { return _currQtrSecondPass; }
            set
            {
                _currQtrSecondPass = value;
                OnPropertyChanged();
            }
        }

        private List<KeyValuePair<string, double>> _currQtrFirstPass;

        public List<KeyValuePair<string, double>> CurrQtrFirstPass
        {
            get { return _currQtrFirstPass; }
            set
            {
                _currQtrFirstPass = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        private void GetChartData(int start, int stop, bool qtrCompare)
        {
            GetPercentage _percent = new GetPercentage();
            List<MetricsInv> _metricsQtr;

            double? _tCnt = 0;
            double? _firPass = 0;
            double? _secPass = 0;

            try
            {
                var _dqList = (from _a in new XpresEntities().MetricsInvs
                          orderby _a.Date descending
                          select _a.Date).ToList();
                var _dateQuery = _dqList;

                _metricsQtr = new List<MetricsInv>();

                if (!qtrCompare)
                {
                    _currQtrFirstPass = new List<KeyValuePair<string, double>>();
                    _currQtrSecondPass = new List<KeyValuePair<string, double>>();
                }
                else
                {
                    _prevQtrFirstPass = new List<KeyValuePair<string, double>>();
                    _prevQtrSecondPass = new List<KeyValuePair<string, double>>();
                    start += 12;
                    stop += 12;
                }

                if (_dqList.Count > 0)
                {
                    for (int _i = start; _i >= stop; _i--)
                    {
                        DateTime? _dq = _dateQuery[_i];
                        var _selDates = (from _a in new XpresEntities().MetricsInvs
                                         where _a.Date == _dq
                                         select _a).SingleOrDefault();
                        MetricsInv _metricsRow = _selDates;
                        _metricsQtr.Add(_metricsRow);
                    }
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving metrics data: " + _ex.Message);
                return;
            }

            _tCnt = 0;
            _firPass = 0;
            _secPass = 0;

            if (_metricsQtr.Any())
            {
                foreach (var _item in _metricsQtr)
                {
                    DateTime _countWeek = Convert.ToDateTime(_item.Date);
                    _tCnt = _item.TotalLocations;
                    _firPass = _item.FirstPassLocations;
                    _secPass = _item.SecondPassLocations;

                    if (_tCnt == 0) continue;
                    if (!qtrCompare)
                    {
                        _percent.CalcLoc(Convert.ToDouble(_tCnt), Convert.ToDouble(_firPass));
                        _currQtrFirstPass.Add(new KeyValuePair<string, double>(_countWeek.ToString("MM/dd/yyyy"), _percent.PerLoc));
                        _percent.CalcLoc(Convert.ToDouble(_tCnt), Convert.ToDouble(_secPass));
                        _currQtrSecondPass.Add(new KeyValuePair<string, double>(_countWeek.ToString("MM/dd/yyyy"), _percent.PerLoc));
                    }
                    else
                    {
                        _percent.CalcLoc(Convert.ToDouble(_tCnt), Convert.ToDouble(_firPass));
                        _prevQtrFirstPass.Add(new KeyValuePair<string, double>(_countWeek.ToString("MM/dd/yyyy"), _percent.PerLoc));
                        _percent.CalcLoc(Convert.ToDouble(_tCnt), Convert.ToDouble(_secPass));
                        _prevQtrSecondPass.Add(new KeyValuePair<string, double>(_countWeek.ToString("MM/dd/yyyy"), _percent.PerLoc));
                    }
                }
            }
        }

        #endregion Methods
    }
}