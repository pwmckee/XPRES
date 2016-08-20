using System;
using System.Collections.Generic;
using System.Linq;
using XPRES.DAL;
using XPRES.Helpers;

namespace XPRES.Departments.Inbound.ViewModels
{
    public class IbLphChartVm : ViewModelBase
    {
        public IbLphChartVm()
        {
            CreateIbLphChart();
        }

        private List<KeyValuePair<string, double>> _inbActivityList;

        public List<KeyValuePair<string, double>> InbActivityList
        {
            get { return _inbActivityList; }
            set
            {
                _inbActivityList = value;
                OnPropertyChanged();
            }
        }

        private void CreateIbLphChart()
        {
            XpsDates _xdates = new XpsDates();
            DateTime _sdate = _xdates.StartWeek;
            DateTime _edate = DateTime.Now.Date.AddDays(1);

            List<DateTime> _dates = new List<DateTime>();
            _inbActivityList = new List<KeyValuePair<string, double>>();
            double _lph = 0;

            try
            {
                var _acts = (from _a in new XpresEntities().InboundActivities
                             where _a.Finish >= _sdate && _a.Finish <= _edate
                             select _a);

                if (_acts.Any())
                {
                    foreach (InboundActivity _a in _acts)
                    {
                        DateTime _date = Convert.ToDateTime(_a.Finish).Date;
                        _dates.Add(_date);
                    }

                    _dates = _dates.Distinct().ToList();

                    foreach (DateTime _d in _dates)
                    {
                        DateTime _nextDay = _d.AddDays(1);
                        double _count = 0;
                        foreach (InboundActivity _a in _acts)
                        {
                            if (_a.Finish >= _d && _a.Finish < _nextDay)
                            {
                                _count++;
                                _lph += Convert.ToDouble(_a.LPH);
                            }
                        }
                        double _avg = (_lph / _count);
                        _inbActivityList.Add(new KeyValuePair<string, double>(_d.ToShortDateString(), _avg));
                    }
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving location accuracy chart data: " + _ex.Message);
            }
        }
    }
}