using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using XPRES.DAL;

namespace XPRES.Departments.Replen.Views.Charts
{
    /// <summary>
    /// Interaction logic for ColumnChart.xaml
    /// </summary>
    public partial class ReplenChart : UserControl
    {
        public ReplenChart()
        {
            InitializeComponent();
            CreateReplenChart();
        }

        private void CreateReplenChart()
        {
            XpresEntities xps = new XpresEntities();
            DateTime _sdate = DateTime.Now.Date;
            DateTime _edate = DateTime.Now.Date.AddDays(1);

            List<KeyValuePair<string, int>> replMetricsChart = new List<KeyValuePair<string, int>>();

            int _lk = 0;
            int _kdx = 0;
            int _pl = 0;
            int _hsec = 0;

            var repl = (from a in xps.ReplenSAAGs
                        where a.TimeStamp == _sdate && a.TimeStamp <= _edate
                        select a);

            _lk = Convert.ToInt32(repl.Sum(a => a.LK00001));
            _kdx = Convert.ToInt32(repl.Sum(a => a.KARDEX));
            _pl = Convert.ToInt32(repl.Sum(a => a.PL));
            _hsec = Convert.ToInt32(repl.Sum(a => a.HUBSECURE));

            replMetricsChart.Add(new KeyValuePair<string, int>("LK", _lk));
            replMetricsChart.Add(new KeyValuePair<string, int>("KDX", _kdx));
            replMetricsChart.Add(new KeyValuePair<string, int>("PL", _pl));
            replMetricsChart.Add(new KeyValuePair<string, int>("HUBSECURE", _hsec));

            ColumnRepl.DataContext = null;
            ColumnRepl.DataContext = replMetricsChart;
        }
    }
}