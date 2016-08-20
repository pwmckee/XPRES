using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;

namespace XPRES.Departments.Inventory.GeoCounts
{
    /// <summary>
    /// Interaction logic for GeoCountsSchedule.xaml
    /// </summary>
    public partial class GeoCountsSchedule : Window, IDisposable
    {
        private bool _open;

        //private XpresEntitiesTest xps;
        private XpresEntities xps;

        private DataTable dt;
        private int _yearNow;
        private int _qtrNow;
        private int _monthNow = DateTime.Now.Month;
        private string viewRange;
        private List<CountSchedule> schedList;

        public GeoCountsSchedule()
        {
            InitializeComponent();
            _open = false;
            //xps = new XpresEntitiesTest();
            xps = new XpresEntities();
            _yearNow = DateTime.Now.Year;
            _qtrNow = (_monthNow - 1) / 3 + 1;
            cbxYear.SelectedItem = _yearNow;
            cbxQuarter.SelectedItem = _qtrNow;
            schedList = new List<CountSchedule>();
            FillCountYears();
        }

        public void Dispose()
        {
            xps.Dispose();
            if (dt != null)
                dt.Dispose();
        }

        private void FillCountYears()
        {
            try
            {
                string currYear = DateTime.Now.Year.ToString();
                var q = (from a in xps.CountSchedules
                         orderby a.GoalYear
                         select a.GoalYear).Distinct().ToList();
                cbxYear.ItemsSource = q;
                cbxYear.SelectedItem = currYear;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while connecting to the database: " + ex);
                return;
            }
        }

        private void FillQuarters()
        {
            int dt = DateTime.Now.Month;
            int currQtr = (dt - 1) / 3 + 1;
            string selectedCountYear = cbxYear.SelectedItem.ToString();
            var q = (from a in xps.CountSchedules
                     where a.GoalYear == selectedCountYear
                     select a.GoalQuarter).Distinct().ToList();
            cbxQuarter.ItemsSource = q;
            cbxQuarter.SelectedItem = currQtr;
        }

        private void FillCountAreas()
        {
            string selectedCountYear = cbxYear.SelectedItem.ToString();
            int selectedQuarter = Convert.ToInt32(cbxQuarter.SelectedItem);
            var q = (from a in xps.CountSchedules
                     where a.GoalYear == selectedCountYear && a.GoalQuarter == selectedQuarter
                     orderby a.Zone
                     select a.CountArea).Distinct().ToList();
            cbxCountArea.ItemsSource = q;
        }

        private void FillGrid(string viewRange)
        {
            xps = new XpresEntities();
            if (viewRange == "All")
            {
                schedList = new List<CountSchedule>();
                var all = (from a in xps.CountSchedules
                           select a).ToList();
                schedList = all;
            }
            else if (viewRange == "Selection")
            {
                string _range = cbxCountArea.SelectedItem.ToString();
                schedList = new List<CountSchedule>();
                var sel = (from a in xps.CountSchedules
                           where a.CountArea == _range
                           select a).ToList();
                schedList = sel;
            }

            dt = new DataTable();
            dt.Columns.Add("Zone");
            dt.Columns.Add("Count Area");
            dt.Columns.Add("Count Range");
            dt.Columns.Add("Goal Date");
            dt.Columns.Add("First Pass");
            dt.Columns.Add("Second Pass");
            dt.Columns.Add("First Counter");
            dt.Columns.Add("Second Counter");
            dt.Columns.Add("Count Status");
            dt.Columns.Add("Count ID");

            foreach (CountSchedule cs in schedList)
            {
                DataRow dr = dt.NewRow();
                dr[0] = cs.Zone;
                dr[1] = cs.CountArea;
                dr[2] = cs.CountRange;
                if (cs.GoalDate.ToString() != "")
                    dr[3] = Convert.ToDateTime(cs.GoalDate).ToString("MM/dd");
                if (cs.ActualDate.ToString() != "")
                    dr[4] = Convert.ToDateTime(cs.ActualDate).ToString("MM/dd");
                if (cs.SecondPassDate.ToString() != "")
                    dr[5] = Convert.ToDateTime(cs.SecondPassDate).ToString("MM/dd");
                dr[6] = cs.FirstCount;
                dr[7] = cs.SecondCount;
                dr[8] = cs.CountStatus;
                dr[9] = cs.CountID;
                dt.Rows.Add(dr);
            }
            dgSchedule.ItemsSource = dt.DefaultView;
            dgSchedule.IsReadOnly = true;
        }

        private void cbxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillQuarters();
        }

        private void cbxQuarter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillCountAreas();
        }

        private void cbxCountArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewRange = "Selection";
            FillGrid(viewRange);
        }

        private void btnCounts_Click(object sender, RoutedEventArgs e)
        {
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is GeoCountsMain)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                GeoCountsMain gc = new GeoCountsMain();
                gc.Show();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            viewRange = "All";
            FillGrid(viewRange);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Coming Soon!");
            return;
        }

        private void btnFindCCTracker_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
        }
    }
}