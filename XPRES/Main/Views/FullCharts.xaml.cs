using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using XPRES.Departments.Inventory.Views.Charts;
using XPRES.Departments.Inbound.Views.Charts;
using XPRES.Departments.Replen.Views.Charts;
using XPRES.Departments.Outbound.Views.Charts;

namespace XPRES.Main.Views
{
    /// <summary>
    /// Interaction logic for FullCharts.xaml
    /// </summary>
    public partial class FullCharts : Window, IDisposable
    {
        private readonly Collection<UserControl> _charts;
        private Timer _metricsTimer;
        private int _c;

        public FullCharts()
        {
            InitializeComponent();
            _charts = new Collection<UserControl>();
            _c = 0;
            CollectCharts();
            CreateTimer();
            RefreshCharts();
        }

        public void Dispose()
        {
            _metricsTimer.Dispose();
        }

        private void CreateTimer()
        {
            _metricsTimer = new Timer {Interval = 3000};
            _metricsTimer.Elapsed += MetricsTimer_Elapsed;
            _metricsTimer.Start();
        }

        private void MetricsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispose();
            RefreshCharts();
            CreateTimer();
        }

        private void CollectCharts()
        {
            IbLphChart _inbC = new IbLphChart();
            ObLphChart _outC = new ObLphChart();
            ReplenChart _repC = new ReplenChart();
            InvLocChart _invC = new InvLocChart();
            _charts.Add(_inbC);
            _charts.Add(_outC);
            _charts.Add(_repC);
            _charts.Add(_invC);
        }

        private void RefreshCharts()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                RotateCharts(_c);
                _c++;
                if (_c == _charts.Count)
                    _c = 0;
            }));
        }

        private void RotateCharts(int counter)
        {
            grdChart.Children.Clear();
            grdChart.Children.Add(_charts[counter]);
        }

        private void btnTimer_Click(object sender, RoutedEventArgs e)
        {
            btnTimer.Visibility = Visibility.Hidden;
            btnStopTimer.Visibility = Visibility.Visible;
            _metricsTimer.Stop();
        }

        private void btnStopTimer_Click(object sender, RoutedEventArgs e)
        {
            btnTimer.Visibility = Visibility.Visible;
            btnStopTimer.Visibility = Visibility.Hidden;
            _metricsTimer.Start();
        }
    }
}