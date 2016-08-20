using System;
using System.Windows;
using System.Windows.Controls;
using XPRES.Helpers;

namespace XPRES.Departments.Inventory.Views.Charts
{
    /// <summary>
    /// Interaction logic for AdjValuesControl.xaml
    /// </summary>
    public partial class AdjValuesControl : UserControl
    {
        public AdjValuesControl()
        {
            InitializeComponent();
            AdjValuesChart();
        }

        private void AdjValuesChart()
        {
            AdjValueChart adjChart = new AdjValueChart();
            XpsDates xdate = new XpsDates();
            DateTime sdate = new DateTime();
            DateTime edate = new DateTime();

            //Previous Week

            xdate.CheckDateEntry(dtpPrev.SelectedDate.ToString(), xdate.StartWeek.AddDays(-14).ToString());
            xdate.GetWeekStart(xdate.ValidDate);
            sdate = xdate.StartWeek;

            xdate.CheckDateEntry(dtpPrev.SelectedDate.ToString(), sdate.AddDays(6).ToString());
            edate = xdate.ValidDate;

            adjChart.GetAdjValues(sdate, edate);

            if (adjChart.NetTotals.Count > 0 || adjChart.WriteOffTotals.Count > 0 || adjChart.WriteOnTotals.Count > 0)
            {
                PrevNetSeries.DataContext = adjChart.NetTotals;
                PrevOffSeries.DataContext = adjChart.WriteOffTotals;
                PrevOnSeries.DataContext = adjChart.WriteOnTotals;
            }

            //Current Week
            xdate = new XpsDates();
            xdate.CheckDateEntry(dtpCurr.SelectedDate.ToString(), xdate.StartWeek.AddDays(-7).ToString());
            xdate.GetWeekStart(xdate.ValidDate);
            sdate = xdate.StartWeek;

            xdate.CheckDateEntry(dtpCurr.SelectedDate.ToString(), sdate.AddDays(6).ToString());
            edate = xdate.ValidDate;

            adjChart.GetAdjValues(sdate, edate);
            if (adjChart.NetTotals.Count > 0 || adjChart.WriteOffTotals.Count > 0 || adjChart.WriteOnTotals.Count > 0)
            {
                CurrNetSeries.DataContext = adjChart.NetTotals;
                CurrOffSeries.DataContext = adjChart.WriteOffTotals;
                CurrOnSeries.DataContext = adjChart.WriteOnTotals;
            }

            //YTD
            sdate = Convert.ToDateTime("1/1/" + DateTime.Today.Year);
            edate = DateTime.Today.AddDays(1);

            adjChart.GetAdjValues(sdate, edate);
            YTDNetSeries.DataContext = adjChart.NetTotals;
            YTDOffSeries.DataContext = adjChart.WriteOffTotals;
            YTDOnSeries.DataContext = adjChart.WriteOnTotals;
        }

        private void btnRefreshPrev_Click(object sender, RoutedEventArgs e)
        {
            AdjValuesChart();
        }

        private void btnRefreshCurr_Click(object sender, RoutedEventArgs e)
        {
            AdjValuesChart();
        }
    }
}