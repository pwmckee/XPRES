using System;
using System.Collections.Generic;
using System.Linq;
using XPRES.DAL;

namespace XPRES.Helpers
{
    public class LocAccChart
    {
        private void GetChartData(int start, int stop)
        {
            GetPercentage _percent = new GetPercentage();
            List<KeyValuePair<string, double>> _firstPass = new List<KeyValuePair<string, double>>();
            List<KeyValuePair<string, double>> _secondPass = new List<KeyValuePair<string, double>>();
            var _metricsQtr = new List<MetricsInv>();
            if (stop > 0)
            {
                List<DateTime?> _dateQuery;
                try
                {
                    var _dq = (from _a in new XpresEntities().MetricsInvs
                              orderby _a.Date descending
                              select _a.Date).ToList();
                    _dateQuery = _dq;
                }
                catch (Exception _ex)
                {
                    System.Windows.Forms.MessageBox.Show(@"Error retrieving metrics data: " + _ex.Message);
                    return;
                }

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

            if (_metricsQtr.Count == 0) return;
            foreach (MetricsInv _item in _metricsQtr)
            {
                DateTime _countWeek = Convert.ToDateTime(_item.Date);
                var _tCnt = _item.TotalLocations;
                var _firPass = _item.FirstPassLocations;
                var _secPass = _item.SecondPassLocations;

                _percent.CalcLoc(Convert.ToDouble(_tCnt), Convert.ToDouble(_firPass));
                _firstPass.Add(new KeyValuePair<string, double>(_countWeek.ToString("MM/dd/yyyy"), _percent.PerLoc));

                _percent.CalcLoc(Convert.ToDouble(_tCnt), Convert.ToDouble(_secPass));
                _secondPass.Add(new KeyValuePair<string, double>(_countWeek.ToString("MM/dd/yyyy"), _percent.PerLoc));
            }
        }
    }
}