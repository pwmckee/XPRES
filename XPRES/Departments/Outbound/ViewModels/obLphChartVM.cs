using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using XPRES.DAL;
using XPRES.Helpers;

namespace XPRES.Departments.Outbound.ViewModels
{
    public class obLphChartVM : INotifyPropertyChanged
    {
        public obLphChartVM()
        {
            CreateOutChart();
        }

        private List<KeyValuePair<string, double>> orderList;

        public List<KeyValuePair<string, double>> OrderList
        {
            get { return orderList; }
            set
            {
                orderList = value;
                NotifyPropertyChanged();
            }
        }

        public void CreateOutChart()
        {
            orderList = new List<KeyValuePair<string, double>>();
            XpsDates xdates = new XpsDates();
            XpresEntities xps = new XpresEntities();
            DateTime _sdate = xdates.StartWeek;
            DateTime _edate = DateTime.Now.Date.AddDays(1);
            DateTime _date = new DateTime();
            DateTime _nextDay = new DateTime();
            List<DateTime> dates = new List<DateTime>();
            double _lph = 0;
            double _count = 0;
            double _avg = 0;

            var ords = (from a in xps.Orders
                        where a.EndTime >= _sdate && a.EndTime <= _edate
                        select a);

            foreach (var o in ords)
            {
                _date = Convert.ToDateTime(o.EndTime).Date;
                dates.Add(_date);
            }

            dates = dates.Distinct().ToList();

            foreach (var d in dates)
            {
                _nextDay = d.AddDays(1);
                _count = 0;
                foreach (var o in ords)
                {
                    if (o.EndTime >= d && o.EndTime < _nextDay)
                    {
                        _count++;
                        _lph += Convert.ToDouble(o.LPH);
                    }
                }
                _avg = (_lph / _count);
                orderList.Add(new KeyValuePair<string, double>(d.ToShortDateString(), _avg));
            }
        }

        #region INotify Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotify Implementation
    }
}