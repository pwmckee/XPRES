using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;
using XPRES.Departments.Inventory.Views.Controls;
using XPRES.Departments.Inventory.Views;
using XPRES.Main.Views;
using XPRES.Helpers;

namespace XPRES.Departments.Inventory.GeoCounts
{
    /// <summary>
    /// Interaction logic for GeoCountsMain.xaml
    /// </summary>
    public partial class GeoCountsMain : Window
    {
        private DataTable dtTracker;
        private DataTable dtCS;
        private DataTable dtDelim;
        private DataTable dtCountRange;
        private DateTime SDate;
        private DateTime EDate;
        private List<CCTracker> Counts;
        private XpresEntities xps;
        private int countID;
        private int selectedZone;
        private string PID;
        private string CountType;
        private string CountTypeUpdate;
        private string countStatus;
        private bool open;

        public GeoCountsMain()
        {
            InitializeComponent();
            InitializeVars();
            TestConnection();
            FillCountYears();
        }

        #region Initialization

        public void InitializeVars()
        {
            dtTracker = new DataTable();
            dtCS = new DataTable();
            dtDelim = new DataTable();
            dtCountRange = new DataTable();
            SDate = new DateTime();
            EDate = new DateTime();
            Counts = new List<CCTracker>();
            xps = new XpresEntities();
            countStatus = "Complete";
            countID = 0;
            open = false;
        }

        private void TestConnection()
        {
            DbConnection _conn = xps.Database.Connection;
            try
            {
                _conn.Open();
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while connecting to the database: " + _ex.Message);
                Environment.Exit(0);
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
        }

        #endregion Initialization

        #region Button Click Events

        #region Navigation

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (object _wnd in Application.Current.Windows)
            {
                if (_wnd is MainWindow) _open = true;
            }
            if (!_open)
            {
                MainWindow _home = new MainWindow();
                _home.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "XPRES") _wnd.Activate();
                }
            }
        }

        private void btnDash_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window _wnd in Application.Current.Windows)
            {
                if (_wnd is InvMain) open = true;
            }
            if (!open)
            {
                InvMain _iMain = new InvMain();
                _iMain.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Inventory Dashboard") _wnd.Activate();
                }
            }
        }

        #endregion Navigation

        #region Main Menu Buttons

        private void btnCCTracker_Click(object sender, RoutedEventArgs e)
        {
            DtpEdCCtracker.SelectedDate = DateTime.Today.Date;
            foreach (StackPanel sp in GrdMain.Children)
            {
                if (sp.Name == "spCCTracker" || sp.Name == "spMainMenu")
                {
                    sp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp.Visibility = Visibility.Hidden;
                }
            }
            foreach (StackPanel sp in GrdLeftPanel.Children)
            {
                sp.Visibility = Visibility.Hidden;
            }
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdDataGrid")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnCountMgmt_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in GrdMain.Children)
            {
                if (sp.Name == "spCountMgmt" || sp.Name == "spMainMenu")
                {
                    sp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnCountSched_Click(object sender, RoutedEventArgs e)
        {
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is GeoCountsSchedule)
                {
                    open = true;
                }
            }
            if (!open)
            {
                GeoCountsSchedule _sched = new GeoCountsSchedule();
                _sched.Show();
            }
        }

        private void btnCountMetrics_Click(object sender, RoutedEventArgs e)
        {
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is GcMetrics)
                {
                    open = true;
                }
            }
            if (!open)
            {
                GcMetrics _metrics = new GcMetrics();
                _metrics.Show();
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Coming Soon!");
        }

        #endregion Main Menu Buttons

        #region CCTracker Buttons

        private void btnFindCCTracker_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataGrid dg in GrdDataGrid.Children)
            {
                if (dg.Name == "dgTracker")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
            GetCounts();
        }

        private void btnClearCCTracker_Click(object sender, RoutedEventArgs e)
        {
            DtpSdCCtracker.Text = "";
            DtpEdCCtracker.Text = "";
            TxtPidccTracker.Text = "";
            DgTracker.ItemsSource = null;
            GC.Collect();
        }

        private void btnExportCCTracker_Click(object sender, RoutedEventArgs e)
        {
            ExportTracker();
        }

        #endregion CCTracker Buttons

        #region Count Management Buttons

        #region Count Mgmt Menu

        private void btnCreateCounts_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in GrdLeftPanel.Children)
            {
                if (sp.Name == "spCreateCountLeft")
                {
                    sp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp.Visibility = Visibility.Hidden;
                }
            }
            TxtCountId.Text = "";
            TxtName.Text = "";
        }

        private void btnFirstCounts_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in GrdLeftPanel.Children)
            {
                if (sp.Name == "spFirstCountLeft")
                {
                    sp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdCountControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            countStatus = "NewCreate";
            GetOpenCounts(countStatus);
            TxtCountId.Text = "";
            TxtName.Text = "";
        }

        private void btnSecondCounts_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in GrdLeftPanel.Children)
            {
                if (sp.Name == "spSecondCountLeft")
                {
                    sp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdCountControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            countStatus = "SecondReady";
            GetOpenCounts(countStatus);
            TxtCountId.Text = "";
            TxtName.Text = "";
        }

        private void btnReviewCounts_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in GrdLeftPanel.Children)
            {
                if (sp.Name == "spReviewCountLeft")
                {
                    sp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdCountControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            SpReqPanel.Children.Clear();
        }

        #endregion Count Mgmt Menu

        #region First Count Buttons

        private void btnFirstCountCreate_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdDataGrid")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (DataGrid dg in GrdDataGrid.Children)
            {
                if (dg.Name == "dgTracker")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
            NewCreate();
        }

        private void btnFirstCountSave_Click(object sender, RoutedEventArgs e)
        {
            SaveNewCountSheet();
            CbxCountArea.SelectedItem = -1;
            CbxZone.SelectedItem = -1;
        }

        private void btnFirstExport_Click(object sender, RoutedEventArgs e)
        {
            ExportSheet();
            TxtCountId.Text = "";
            TxtName.Text = "";
        }

        private void btnLoadFirst_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdDataGrid" || grd.Name == "grdWriteIns")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (DataGrid dg in GrdDataGrid.Children)
            {
                if (dg.Name == "dgTracker")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
            CountType = "NewCreate";
            LoadCount(CountType);
            TxtCountId.Text = "";
            TxtName.Text = "";
        }

        private void btnSubmitFirst_Click(object sender, RoutedEventArgs e)
        {
            CountType = "NewCreate";
            CountTypeUpdate = "FirstSubmit";
            SubmitCount(CountType, CountTypeUpdate);
            TxtCountId.Text = "";
            TxtName.Text = "";
        }

        private void btnAddWritein_Click(object sender, RoutedEventArgs e)
        {
            WriteIns();
        }

        #endregion First Count Buttons

        #region Second Count Buttons

        private void btnSecondCountExport_Click(object sender, RoutedEventArgs e)
        {
            ExportSheet();
            TxtCountId.Text = "";
            TxtName.Text = "";
            countStatus = "SecondReady";
            GetOpenCounts(countStatus);
        }

        private void btnLoadSecond_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdDataGrid" || grd.Name == "grdWriteIns")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (DataGrid dg in GrdDataGrid.Children)
            {
                if (dg.Name == "dgTracker")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
            CountType = "SecondReady";
            LoadCount(CountType);
        }

        private void btnSubmitSecond_Click(object sender, RoutedEventArgs e)
        {
            CountType = "SecondReady";
            CountTypeUpdate = "SecondSubmit";
            SubmitCount(CountType, CountTypeUpdate);
        }

        #endregion Second Count Buttons

        #region Review Count Buttons

        private void btnViewFirstReview_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdCountControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            countStatus = "FirstSubmitReview";
            GetOpenCounts(countStatus);
        }

        private void btnLoadFirstReview_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdDataGrid")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (DataGrid dg in GrdDataGrid.Children)
            {
                if (dg.Name == "dgFinal")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
            CountType = "FirstSubmitReview";
            ReviewLoad(CountType);
        }

        private void btnSubmitFirstReview_Click(object sender, RoutedEventArgs e)
        {
            CountType = "FirstSubmitReview";
            CountTypeUpdate = "SecondReady";
            ReDepartmentsubmit(CountType, CountTypeUpdate);
        }

        private void btnDepartmentsecondReview_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdCountControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            countStatus = "SecondSubmitReview";
            GetOpenCounts(countStatus);
        }

        private void btnLoadSecondReview_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdDataGrid")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (DataGrid dg in GrdDataGrid.Children)
            {
                if (dg.Name == "dgFinal")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
            CountType = "SecondSubmitReview";
            ReviewLoad(CountType);
        }

        private void btnSubmitSecondReview_Click(object sender, RoutedEventArgs e)
        {
            CountType = "SecondSubmitReview";
            CountTypeUpdate = "Complete";
            ReDepartmentsubmit(CountType, CountTypeUpdate);
        }

        private void btnDeleteCount_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnViewReDepartmentsheet_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdDataGrid")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (DataGrid dg in GrdDataGrid.Children)
            {
                if (dg.Name == "dgFinal")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnExportReview_Click(object sender, RoutedEventArgs e)
        {
            ExportLocal();
        }

        private void btnImportReview_Click(object sender, RoutedEventArgs e)
        {
            ImportLocal();
            foreach (Grid grd in GrdCenterView.Children)
            {
                if (grd.Name == "grdDataGrid")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (DataGrid dg in GrdDataGrid.Children)
            {
                if (dg.Name == "dgFinal")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
            DgFinal.ItemsSource = dtCS.DefaultView;
        }

        #endregion Review Count Buttons

        #endregion Count Management Buttons

        #region Count Schedule Region

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
            FillCountRange();
            FillZones();
        }

        #endregion Count Schedule Region

        #endregion Button Click Events

        #region Methods

        #region CCTracker Methods

        private void GetCounts()
        {
            xps = new XpresEntities();

            dtTracker = null;
            if (DtpSdCCtracker.SelectedDate.ToString() != "")
            {
                try
                {
                    DateTime.TryParse(DtpSdCCtracker.SelectedDate.ToString(), out SDate);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid From date.");
                    return;
                }
            }
            else SDate = Convert.ToDateTime("1/1/2000");
            if (DtpEdCCtracker.SelectedDate.ToString() != "")
            {
                try
                {
                    DateTime.TryParse(DtpEdCCtracker.SelectedDate.ToString(), out EDate);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid To date.");
                    return;
                }
            }
            else EDate = DateTime.Today.Date;
            PID = TxtPidccTracker.Text.Trim().ToUpper();
            int _cid = 0;
            if (TxtCidccTracker.Text.Trim().ToUpper() != string.Empty)
            {
                try
                {
                    _cid = Convert.ToInt32(TxtCidccTracker.Text.Trim().ToUpper());
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid Count ID.");
                    return;
                }
            }

            if (PID != null && PID.ToString() == "")
            {
                PID = null;
            }

            try
            {
                if (PID == null && _cid == 0)
                {
                    var q = (from a in xps.CCTrackers
                             where a.CountDate >= SDate && a.CountDate <= EDate
                             select a).ToList();
                    Counts = q;
                }
                else if (PID == null && _cid != 0)
                {
                    var q = (from a in xps.CCTrackers
                             where a.CountDate >= SDate && a.CountDate <= EDate && a.CountID == _cid
                             select a).ToList();
                    Counts = q;
                }
                else
                {
                    var q = (from a in xps.CCTrackers
                             where a.CountDate >= SDate && a.CountDate <= EDate && a.PID == PID.ToString()
                             select a).ToList();
                    Counts = q;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to retrieve cycle count data: " + ex.Message);
                return;
            }

            dtTracker = new DataTable();
            dtTracker.Columns.Add("CountDate");
            dtTracker.Columns.Add("ORG");
            dtTracker.Columns.Add("Subinventory");
            dtTracker.Columns.Add("Locator");
            dtTracker.Columns.Add("Item");
            dtTracker.Columns.Add("Description");
            dtTracker.Columns.Add("Onhand");
            dtTracker.Columns.Add("Count");
            dtTracker.Columns.Add("Difference");
            dtTracker.Columns.Add("Action");
            dtTracker.Columns.Add("Error");
            dtTracker.Columns.Add("First Counter");
            dtTracker.Columns.Add("Second Counter");
            dtTracker.Columns.Add("Zone");
            dtTracker.Columns.Add("Unit Cost");
            dtTracker.Columns.Add("Count Status");
            dtTracker.Columns.Add("Count ID");
            foreach (CCTracker cs in Counts)
            {
                DataRow dr = dtTracker.NewRow();
                dr[0] = Convert.ToDateTime(cs.CountDate).ToString("MM/dd/yyyy");
                dr[1] = cs.ORG;
                dr[2] = cs.Subinventory;
                dr[3] = cs.Locator;
                dr[4] = cs.PID;
                dr[5] = cs.Description;
                dr[6] = cs.SystemQTY;
                dr[7] = cs.CountedQTY;
                dr[8] = cs.Difference;
                dr[9] = cs.Action;
                dr[10] = cs.Error;
                dr[11] = cs.FirstCount;
                dr[12] = cs.SecondCount;
                dr[13] = cs.Zone;
                dr[14] = cs.UnitCost;
                dr[15] = cs.CountStatus;
                dr[16] = cs.CountID;
                dtTracker.Rows.Add(dr);
            }
            DgTracker.ItemsSource = dtTracker.DefaultView;
        }

        private void ExportTracker()
        {
            if (dtTracker == null)
            {
                System.Windows.Forms.MessageBox.Show("There was no count sheet loaded.");
            }

            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dtTracker.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dtTracker.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(",", fields));
            }

            Microsoft.Win32.SaveFileDialog exportFile = new Microsoft.Win32.SaveFileDialog();
            exportFile.DefaultExt = "csv";
            exportFile.RestoreDirectory = true;

            if (exportFile.ShowDialog() == true)
            {
                string name = exportFile.FileName;
                try
                {
                    File.WriteAllText(name, sb.ToString());
                    System.Windows.Forms.MessageBox.Show(name + " saved succefully");
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Could not save file: " + ex.Message);
                    return;
                }
            }
        }

        #endregion CCTracker Methods

        #region Count Schedule Methods

        private void FillCountYears()
        {
            try
            {
                string currYear = DateTime.Now.Year.ToString();
                var q = (from a in xps.CountSchedules
                         orderby a.GoalYear
                         select a.GoalYear).Distinct().ToList();
                CbxYear.ItemsSource = q;
                CbxYear.SelectedItem = currYear;
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
            string selectedCountYear = CbxYear.SelectedItem.ToString();
            var q = (from a in xps.CountSchedules
                     where a.GoalYear == selectedCountYear
                     select a.GoalQuarter).Distinct().ToList();
            CbxQuarter.ItemsSource = q;
            CbxQuarter.SelectedItem = currQtr;
        }

        private void FillCountAreas()
        {
            string selectedCountYear = CbxYear.SelectedItem.ToString();
            int selectedQuarter = Convert.ToInt32(CbxQuarter.SelectedItem);
            var q = (from a in xps.CountSchedules
                     where a.GoalYear == selectedCountYear && a.GoalQuarter == selectedQuarter
                     orderby a.Zone
                     select a.CountArea).Distinct().ToList();
            CbxCountArea.ItemsSource = q;
        }

        private void FillCountRange()
        {
            string range = CbxCountArea.SelectedItem.ToString();
            List<CountSchedule> countRangeList = new List<CountSchedule>();
            var q = (from a in xps.CountSchedules
                     where a.CountArea == range
                     select a).ToList();
            countRangeList = q;
            dtCountRange = new DataTable();
            dtCountRange.Columns.Add("Date");
            dtCountRange.Columns.Add("CountRange");
            dtCountRange.Columns.Add("Zone");

            foreach (CountSchedule cs in countRangeList)
            {
                if (cs.CountID == null)
                {
                    DataRow dr = dtCountRange.NewRow();
                    dr[0] = Convert.ToDateTime(cs.GoalDate).ToString("MM/dd");
                    dr[1] = cs.CountRange;
                    dr[2] = cs.Zone;
                    dtCountRange.Rows.Add(dr);
                }
            }
            DgCountRange.ItemsSource = dtCountRange.DefaultView;
            DgCountRange.IsReadOnly = true;
        }

        private void FillZones()
        {
            string range = CbxCountArea.SelectedItem.ToString();
            var q = (from a in xps.CountSchedules
                     where a.CountArea == range
                     select a.Zone).ToList().Distinct();
            CbxZone.ItemsSource = q;
        }

        #endregion Count Schedule Methods

        #region Count Management Methods

        private void GetOpenCounts(string countStatus)
        {
            xps = new XpresEntities();

            if (SpReqPanel.Children.Count > 0)
            {
                SpReqPanel.Children.Clear();
            }
            CycoStackControl _tempUserControl = new CycoStackControl();
            List<CCTracker> _controlData = new List<CCTracker>();
            DataTable _dtCounts = new DataTable();
            _dtCounts.Columns.Add("CountDate");
            _dtCounts.Columns.Add("Zone");
            _dtCounts.Columns.Add("CountStatus");
            _dtCounts.Columns.Add("CountID");
            _dtCounts.Columns.Add("First Count");
            _dtCounts.Columns.Add("Second Count");
            var q = (from a in xps.CCTrackers
                     where a.CountStatus == countStatus
                     select a).ToList();
            _controlData = q;

            foreach (CCTracker cc in _controlData)
            {
                DataRow dr = _dtCounts.NewRow();
                dr[0] = Convert.ToDateTime(cc.CountDate).ToString("MM/dd/yyyy");
                dr[1] = cc.Zone;
                dr[2] = cc.CountStatus;
                dr[3] = cc.CountID;
                dr[4] = cc.FirstCount;
                dr[5] = cc.SecondCount;
                _dtCounts.Rows.Add(dr);
            }
            for (int i = 0; i < _dtCounts.Rows.Count - 1; i++)
            {
                if (Convert.ToInt32(_dtCounts.Rows[i][3]) == Convert.ToInt32(_dtCounts.Rows[i + 1][3]))
                {
                    _dtCounts.Rows[i + 1].Delete();
                    i--;
                }
            }
            foreach (DataRow dr in _dtCounts.Rows)
            {
                _tempUserControl = new CycoStackControl();
                _tempUserControl.Name = "CountZone" + dr[3];
                _tempUserControl.CtrlDate.Content = dr[0].ToString();
                _tempUserControl.CtrlZone.Content = dr[1].ToString();
                _tempUserControl.CtrlStatus.Content = dr[2].ToString();
                _tempUserControl.CtrlID.Text = dr[3].ToString();
                _tempUserControl.CtrlFirst.Content = dr[4].ToString();
                _tempUserControl.CtrlSecond.Content = dr[5].ToString();
                _tempUserControl.Height = 52;
                addStackPanelChild(_tempUserControl);
                //counts.Add(_tempUserControl);
                _tempUserControl = null;
            }
            _controlData.Clear();
            _dtCounts.Clear();

            //return counts;
        }

        private void addStackPanelChild(CycoStackControl _tempUserControl)
        {
            bool _idCheck = false;
            if (SpReqPanel.Children.Count > 0)
            {
                foreach (CycoStackControl ctrl in SpReqPanel.Children)
                {
                    if (ctrl.CtrlID.ToString() == _tempUserControl.CtrlID.ToString())
                    {
                        _idCheck = true;
                    }
                }
            }
            if (!_idCheck)
            {
                SpReqPanel.Children.Add(_tempUserControl);
                SpReqPanel.UpdateLayout();
            }
        }

        public object NewConvertCSV()
        {
            string path = "";
            Microsoft.Win32.OpenFileDialog selectFile = new Microsoft.Win32.OpenFileDialog();
            if (selectFile.ShowDialog() == true)
            {
                path = selectFile.FileName;
                System.Windows.Forms.MessageBox.Show("File: " + path + " successfuly loaded");
            }

            //Convert CSV Onhand to DataTable dtDelim
            StreamReader sr;
            dtDelim = new DataTable();
            try
            {
                sr = new StreamReader(path);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while converting CSV file: " + ex.Message);
                return null;
            }
            try
            {
                int rowCount = 0;
                string[] columnNames = null;
                string[] streamDataValues = null;
                while (!sr.EndOfStream)
                {
                    string streamRowData = sr.ReadLine().Trim();
                    if (streamRowData.Length > 0)
                    {
                        streamRowData = streamRowData.Replace(@"""", "");
                        streamDataValues = streamRowData.Split(',');
                        if (rowCount == 0)
                        {
                            rowCount = 1;
                            columnNames = streamDataValues;
                            foreach (string csvHeader in columnNames)
                            {
                                DataColumn dataColumn = new DataColumn(csvHeader.ToUpper(),
                                    typeof(string));
                                dataColumn.DefaultValue = string.Empty;
                                dtDelim.Columns.Add(dataColumn);
                            }
                        }
                        else
                        {
                            DataRow dr = dtDelim.NewRow();
                            for (int i = 0; i < columnNames.Length; i++)
                            {
                                dr[columnNames[i]] = streamDataValues[i] == null ?
                                    string.Empty : streamDataValues[i].ToString();
                            }
                            dtDelim.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while deliminating CSV file: " + path + ": " + ex.Message);
                return null;
            }
            finally
            {
                sr.Close();
                sr.Dispose();
            }
            return dtDelim;
        }

        public object NewCreate()
        {
            NewConvertCSV();

            if (dtDelim != null)
            {
                xps = new XpresEntities();
                //Remove unnecessary columns carried over from CSV File
                try
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        dtDelim.Columns.RemoveAt(5);
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Error while creating table.  Please make sure the proper type of ERP count data was loaded.");
                    return null;
                }
                //Consolidate line items
                for (int i = 0; i < dtDelim.Rows.Count - 1; i++)
                {
                    if (dtDelim.Rows[i][2].ToString() == dtDelim.Rows[i + 1][2].ToString() & dtDelim.Rows[i][3].ToString() == dtDelim.Rows[i + 1][3].ToString())
                    {
                        dtDelim.Rows[i][4] = Convert.ToSingle(dtDelim.Rows[i][4]) + Convert.ToSingle(dtDelim.Rows[i + 1][4]);
                        dtDelim.Rows[i + 1].Delete();
                        i--;
                    }
                }

                dtDelim.Columns.Add("Description");
                dtDelim.Columns.Add("UnitCost");
                string _desc = "";
                double? _cost = 0;

                dtCS = new DataTable();
                dtCS.Columns.Add("Date", typeof(DateTime));
                dtCS.Columns.Add("Org", typeof(String));
                dtCS.Columns.Add("Subinventory", typeof(String));
                dtCS.Columns.Add("Locator", typeof(String));
                dtCS.Columns.Add("PID", typeof(String));
                dtCS.Columns.Add("Description", typeof(String));
                dtCS.Columns.Add("Onhand", typeof(String));
                dtCS.Columns.Add("Count", typeof(Single));
                dtCS.Columns.Add("Count Status", typeof(String));
                dtCS.Columns.Add("Unit Cost");

                for (int i = 0; i < dtDelim.Rows.Count; i++)
                {
                    string _pid = dtDelim.Rows[i][3].ToString();
                    try
                    {
                        UnitCost _uc = new UnitCost();
                        var uc = (from a in xps.UnitCosts
                                  where a.Item == _pid
                                  select a).First();
                        _desc = uc.Description;
                        _cost = uc.Unit_Cost;
                    }
                    catch
                    {
                        System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                            "No item cost or description found for " + _pid + ". Would you like to add " +
                            "it to the database with a cost of $0.00?",
                            "Item not found", System.Windows.Forms.MessageBoxButtons.YesNo);
                        if (_diag == System.Windows.Forms.DialogResult.Yes)
                        {
                            UnitCost _uc = new UnitCost();
                            _uc.Item = _pid;
                            _uc.Description = "No Description";
                            _uc.Unit_Cost = 0;
                            xps.UnitCosts.Add(_uc);
                            xps.SaveChanges();
                            var uc = (from a in xps.UnitCosts
                                      where a.Item == _pid
                                      select a).First();
                            _desc = uc.Description;
                            _cost = uc.Unit_Cost;
                        }
                        else if (_diag == System.Windows.Forms.DialogResult.No)
                        {
                            System.Windows.Forms.MessageBox.Show("If you need to add this item to the database, please ask a lead or supervisor.");
                            return null;
                        }
                    }

                    double _sys = Convert.ToDouble(dtDelim.Rows[i][4]);
                    DataRow row = dtCS.NewRow();
                    for (int col = 0; col < 5; col++)
                    {
                        row[col + 1] = dtDelim.Rows[i][col];
                    }
                    dtCS.Rows.Add(row);
                    dtCS.Rows[i][0] = DateTime.Now.Date;
                    dtCS.Rows[i][5] = _desc;
                    dtCS.Rows[i][6] = _sys;
                    dtCS.Rows[i][8] = "NewCreate";
                    dtCS.Rows[i][9] = _cost;
                }
                DgTracker.ItemsSource = dtCS.DefaultView;
                DgTracker.Columns[6].Visibility = Visibility.Hidden;
                DgTracker.Columns[8].Visibility = Visibility.Hidden;
                DgTracker.Columns[9].Visibility = Visibility.Hidden;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Error while converting CSV file: No data was entered");
            }
            return dtCS;
        }

        private void SaveNewCountSheet()
        {
            try
            {
                if (dtCS != null && dtCS.Rows[0][8].ToString() == "NewCreate")
                {
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("This is not a newly created count sheet.  Please create a new sheet before trying to save using this menu.");
                    return;
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("No count sheet was loaded.");
                return;
            }
            selectedZone = Convert.ToInt32(CbxZone.SelectedItem);
            if (selectedZone == 0)
            {
                System.Windows.Forms.MessageBox.Show("Please select which zone this count is for.");
                return;
            }
            string _cFrom = TxtCountFrom.Text;
            string _cTo = TxtCountTo.Text;
            int _idFrom = 0;
            int _idTo = 0;
            if (_cFrom.ToString() == "" || _cTo.ToString() == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter both the from and to count range.");
                return;
            }
            CountSchedule _from = new CountSchedule();

            XpsDates xDates = new XpsDates();

            try
            {
                var from = (from a in xps.CountSchedules
                            where a.CountRange == _cFrom && a.GoalQuarter == xDates.CurrentQtr
                            select a).SingleOrDefault();

                _from = from;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to retrieve count schedule info: " + ex.Message);
                return;
            }

            try
            {
                _idFrom = Convert.ToInt32(_from.ID);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Invalid 'From' count range entered.");
                return;
            }

            CountSchedule _to = new CountSchedule();

            try
            {
                var to = (from a in xps.CountSchedules
                          where a.CountRange == _cTo && a.GoalQuarter == xDates.CurrentQtr
                          select a).SingleOrDefault();
                _to = to;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to retrieve count schedule info: " + ex.Message);
                return;
            }

            try
            {
                _idTo = Convert.ToInt32(_to.ID);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Invalid 'To' count range entered.");
                return;
            }
            int _year = Convert.ToInt32(CbxYear.SelectedItem);
            int _qtr = Convert.ToInt32(CbxQuarter.SelectedItem);
            int countID = int.Parse(_year.ToString() + _qtr.ToString() + _idTo.ToString());
            for (int i = _idFrom; i <= _idTo; i++)
            {
                CountSchedule cs = new CountSchedule();
                try
                {
                    cs = (from a in xps.CountSchedules
                          where a.ID == i
                          select a).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error while trying to retrieve count schedule info: " + ex.Message);
                    return;
                }

                if (cs.CountID != null)
                {
                    System.Windows.Forms.MessageBox.Show("One or more locations in this count have already had a count ID created for them.");
                    return;
                }
                cs.ActualDate = DateTime.Now;
                cs.CountStatus = "Created";
                cs.CountID = countID;
                xps.SaveChanges();
            }
            try
            {
                foreach (DataRow dr in dtCS.Rows)
                {
                    CCTracker ccTracker = new CCTracker();
                    ccTracker.CountDate = Convert.ToDateTime(dr[0]);
                    ccTracker.ORG = dr[1].ToString();
                    ccTracker.Subinventory = dr[2].ToString();
                    ccTracker.Locator = dr[3].ToString();
                    ccTracker.PID = dr[4].ToString();
                    ccTracker.Description = dr[5].ToString();
                    ccTracker.SystemQTY = Convert.ToDouble(dr[6]);
                    ccTracker.Zone = selectedZone;
                    ccTracker.CountStatus = dr[8].ToString();
                    ccTracker.UnitCost = Convert.ToDouble(dr[9]);
                    ccTracker.CountID = countID;
                    xps.CCTrackers.Add(ccTracker);
                }
                xps.SaveChanges();
                System.Windows.Forms.MessageBox.Show("Count Sheet Saved.");
                DgTracker.ItemsSource = null;
                CbxZone.SelectedIndex = -1;
                TxtCountFrom.Text = "";
                TxtCountTo.Text = "";
                TxtCountId.Text = "";
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while saving count sheet: " + ex.Message);
                return;
            }
        }

        private void ExportSheet()
        {
            System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                "Are you sure you want to export this count?\nThis should only be done if this is being counted for the first time.", "Delete Entry",
                System.Windows.Forms.MessageBoxButtons.YesNo);

            if (_diag == System.Windows.Forms.DialogResult.Yes)
            {
                string _counter = TxtName.Text;
                int countID = 0;
                if (dtCS == null)
                {
                    System.Windows.Forms.MessageBox.Show("There is no count sheet available to export.");
                    return;
                }
                try
                {
                    CountType = dtCS.Rows[0][8].ToString();
                    countID = Convert.ToInt32(dtCS.Rows[0][9]);
                    string _firstCount = dtCS.Rows[0][10].ToString().ToLower();
                    string _secondCount = dtCS.Rows[0][11].ToString().ToLower();
                    if (CountType == "NewCreate")
                    {
                        if (_firstCount != "")
                        {
                            System.Windows.Forms.MessageBox.Show("This count already has a first counter.");
                            return;
                        }
                    }
                    if (CountType == "SecondReady")
                    {
                        if (_firstCount != _counter.ToLower())
                        {
                            if (dtCS.Rows[0][11].ToString() != "")
                            {
                                System.Windows.Forms.MessageBox.Show("This count already has a second counter.");
                                return;
                            }
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("The First and Second counter cannot be the same person.");
                            return;
                        }
                    }
                    if (dtCS.Rows[0][8].ToString() != CountType)
                    {
                        System.Windows.Forms.MessageBox.Show("This is not a the correct type of sheet to submit under this menu. Please make sure you if this is a first or second count to use the proper menu to submit.");
                        return;
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("No count sheet was loaded.");
                    return;
                }
                if (dtCS == null)
                {
                    System.Windows.Forms.MessageBox.Show("No count sheet was loaded.");
                    return;
                }
                if (_counter == "")
                {
                    System.Windows.Forms.MessageBox.Show("Counter Name cannot be blank");
                    return;
                }
                dtCS.Columns.Remove("Onhand");

                StringBuilder sb = new StringBuilder();
                IEnumerable<string> columnNames = dtCS.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dtCS.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field =>
                    string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                    sb.AppendLine(string.Join(",", fields));
                }

                Microsoft.Win32.SaveFileDialog exportFile = new Microsoft.Win32.SaveFileDialog();
                exportFile.DefaultExt = "csv";
                exportFile.RestoreDirectory = true;

                if (exportFile.ShowDialog() == true)
                {
                    string name = exportFile.FileName;
                    try
                    {
                        File.WriteAllText(name, sb.ToString());
                        var q = (from a in xps.CCTrackers
                                 where a.CountID == countID
                                 select a).ToList();
                        Counts = new List<CCTracker>();
                        Counts = q;
                        foreach (CCTracker cc in Counts)
                        {
                            if (CountType == "NewCreate")
                            {
                                cc.FirstCount = _counter;
                            }
                            else
                            {
                                cc.SecondCount = _counter;
                            }
                        }
                        xps.SaveChanges();
                        List<CountSchedule> _sched = new List<CountSchedule>();
                        var qSched = (from a in xps.CountSchedules
                                      where a.CountID == countID
                                      select a).ToList();
                        _sched = qSched;
                        if (CountType == "NewCreate")
                        {
                            foreach (CountSchedule cs in _sched)
                            {
                                cs.FirstCount = _counter;
                            }
                        }
                        else
                        {
                            foreach (CountSchedule cs in _sched)
                            {
                                cs.SecondPassDate = DateTime.Now;
                                cs.SecondCount = _counter;
                            }
                        }
                        xps.SaveChanges();

                        System.Windows.Forms.MessageBox.Show(name + " saved succefully");
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Could not save file: " + ex.Message);
                        return;
                    }
                }
            }
        }

        public object LoadCount(string CountType)
        {
            xps = new XpresEntities();
            Counts = new List<CCTracker>();
            dtCS = new DataTable();

            try
            {
                countID = int.Parse(TxtCountId.Text);
                if (countID == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Please enter a count ID number to load a count.");
                    return null;
                }
            }
            catch
            {
                foreach (Grid grd in GrdCenterView.Children)
                {
                    if (grd.Name == "grdCountControls")
                    {
                        grd.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        grd.Visibility = Visibility.Hidden;
                    }
                }
                System.Windows.Forms.MessageBox.Show("Count ID is Invalid");
                return null;
            }

            try
            {
                var q = (from a in xps.CCTrackers
                         where a.CountID == countID && a.CountStatus != "Complete"
                         select a).ToList();
                Counts = q;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to retrieve count schedule info: " + ex.Message);
                return null;
            }

            dtCS = new DataTable();
            dtCS.Columns.Add("Count Date");
            dtCS.Columns.Add("ORG");
            dtCS.Columns.Add("Subinventory");
            dtCS.Columns.Add("Locator");
            dtCS.Columns.Add("Item");
            dtCS.Columns.Add("Description");
            dtCS.Columns.Add("Onhand");
            dtCS.Columns.Add("Count");
            dtCS.Columns.Add("Count Status");
            dtCS.Columns.Add("Count ID");
            dtCS.Columns.Add("First Count");
            dtCS.Columns.Add("Second Count");
            dtCS.Columns.Add("KeyID");

            foreach (CCTracker cs in Counts)
            {
                DataRow dr = dtCS.NewRow();
                dr[0] = Convert.ToDateTime(cs.CountDate).ToString("MM/dd/yyyy");
                dr[1] = cs.ORG;
                dr[2] = cs.Subinventory;
                dr[3] = cs.Locator;
                dr[4] = cs.PID;
                dr[5] = cs.Description;
                dr[6] = cs.SystemQTY;
                dr[8] = cs.CountStatus;
                dr[9] = cs.CountID;
                dr[10] = cs.FirstCount;
                dr[11] = cs.SecondCount;
                dr[12] = cs.ID;
                dtCS.Rows.Add(dr);
            }
            DgTracker.ItemsSource = dtCS.DefaultView;
            DgTracker.Columns[6].Visibility = Visibility.Hidden;
            DgTracker.Columns[8].Visibility = Visibility.Hidden;
            DgTracker.Columns[10].Visibility = Visibility.Hidden;
            DgTracker.Columns[11].Visibility = Visibility.Hidden;
            DgTracker.Columns[12].Visibility = Visibility.Hidden;
            return dtCS;
        }

        public object WriteIns()
        {
            xps = new XpresEntities();

            string _loc = TxtWriteinLoc.Text.Trim().ToUpper();
            string _pid = TxtWriteinPid.Text.Trim().ToUpper();
            double _count = 0;

            if (_loc == "" || _pid == "" || TxtWriteinCount.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please make sure the Locator, Part Number, and Count are entered.");
                return null;
            }

            try
            {
                _count = Convert.ToDouble(TxtWriteinCount.Text.Trim().ToUpper());
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Invalid Count entered.");
                return null;
            }

            for (int i = 0; i < dtCS.Rows.Count; i++)
            {
                DataRow _checkRow = dtCS.Rows[i];
                if (_loc == _checkRow[3].ToString() && _pid == _checkRow[4].ToString())
                {
                    System.Windows.Forms.MessageBox.Show("This location and part number is already on the count sheet.");
                    return null;
                }
            }

            CCTracker cc = new CCTracker();

            try
            {
                var _cc = (from a in xps.CCTrackers
                           where a.CountID == countID
                           select a).First();
                cc = _cc;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to check the cycle count database: " + ex.Message);
                return null;
            }

            UnitCost uc = new UnitCost();
            double? _newUC = 0;
            string _newDesc = string.Empty;
            try
            {
                var _uc = (from a in xps.UnitCosts
                           where a.Item == _pid
                           select a).First();
                uc = _uc;
                _newUC = uc.Unit_Cost;
                _newDesc = uc.Description;
            }
            catch
            {
                _newUC = 0;
                _newDesc = "No Listing";
            }

            int _countID = Convert.ToInt32(dtCS.Rows[0][9]);

            DataRow _addRow = dtCS.NewRow();
            _addRow[0] = dtCS.Rows[0][0].ToString();
            _addRow[1] = dtCS.Rows[0][1].ToString();
            _addRow[2] = dtCS.Rows[0][2].ToString();
            _addRow[3] = _loc;
            _addRow[4] = _pid;
            _addRow[5] = _newDesc;
            _addRow[6] = 0;
            _addRow[7] = _count;
            _addRow[8] = dtCS.Rows[0][8].ToString();
            _addRow[9] = _countID;
            _addRow[10] = dtCS.Rows[0][10].ToString();
            _addRow[11] = dtCS.Rows[0][11].ToString();

            CCTracker ccNew = new CCTracker();
            ccNew = cc;
            ccNew.Locator = _loc;
            ccNew.PID = _pid;
            ccNew.Description = _newDesc;
            ccNew.SystemQTY = 0;
            ccNew.CountedQTY = Convert.ToDouble(_addRow[7]);
            ccNew.Difference = Convert.ToInt32(_addRow[7]) - 0;
            ccNew.CountStatus = _addRow[8] + "Review";
            ccNew.UnitCost = _newUC;
            xps.CCTrackers.Add(ccNew);

            xps.SaveChanges();

            xps = new XpresEntities();
            int pkID = 0;
            try
            {
                var q = (from a in xps.CCTrackers
                         where a.CountID == _countID && a.Locator == _loc && a.PID == _pid
                         select a.ID).First();
                pkID = q;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to retrieve write-in Key ID: " + ex.Message);
                return null;
            }

            if (pkID == 0)
            {
                System.Windows.Forms.MessageBox.Show("Write-in was not saved properly please make sure all information was entered correctly.");
                return null;
            }

            _addRow[12] = pkID;
            dtCS.Rows.Add(_addRow);

            DgTracker.ItemsSource = dtCS.DefaultView;
            DgTracker.Columns[6].Visibility = Visibility.Hidden;
            DgTracker.Columns[8].Visibility = Visibility.Hidden;
            DgTracker.Columns[10].Visibility = Visibility.Hidden;
            DgTracker.Columns[11].Visibility = Visibility.Hidden;
            DgTracker.Columns[12].Visibility = Visibility.Hidden;

            return dtCS;
        }

        private void SubmitCount(string CountType, string CountTypeUpdate)
        {
            xps = new XpresEntities();

            string _countStatus = "";

            #region Primary Validation

            try
            {
                countID = Convert.ToInt32(dtCS.Rows[0][9]);
                string _counter;
                if (dtCS.Rows[0][8].ToString() != CountType)
                {
                    System.Windows.Forms.MessageBox.Show("This is not a the correct type of sheet to submit under this menu. Please make sure you if this is a first or second count to use the proper menu to submit.");
                    return;
                }
                if (countID == 0)
                {
                    System.Windows.Forms.MessageBox.Show("No count sheet was loaded.");
                    return;
                }
                if (CountType == "NewCreate")
                {
                    _counter = dtCS.Rows[0][10].ToString();
                }
                else
                {
                    _counter = dtCS.Rows[0][11].ToString();
                }
                if (_counter == "")
                {
                    System.Windows.Forms.MessageBox.Show("This count has not had a Counter assigned to it yet.  Please export the count first.");
                    return;
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("No count sheet was loaded.");
                return;
            }

            #endregion Primary Validation

            try
            {
                for (int i = 0; i < dtCS.Rows.Count; i++)
                {
                    DataRow _checkRow = dtCS.Rows[i];
                    if (_checkRow[7].ToString() == "" || _checkRow[3].ToString() == "" || _checkRow[4].ToString() == "")
                    {
                        System.Windows.Forms.MessageBox.Show("Not all required information was entered. Please make sure that all count qty, part number, and locator information is not blank");
                        return;
                    }

                    for (int j = 0; j < dtCS.Rows.Count; j++)
                    {
                        DataRow dr = dtCS.Rows[j];
                        if ((Convert.ToSingle(dr[7]) - Convert.ToSingle(dr[6])) == 0)
                        {
                            _countStatus = "Complete";
                        }
                        else
                        {
                            _countStatus = CountTypeUpdate + "Review";
                        }
                        DateTime _countDate = Convert.ToDateTime(dr[0]);
                        string _loc = dr[3].ToString();
                        string _pid = dr[4].ToString();
                        int pkID = Convert.ToInt32(dr[12]);

                        CCTracker countQ = (from c in xps.CCTrackers
                                            where c.ID == pkID
                                            select c).SingleOrDefault();
                        countQ.SystemQTY = Convert.ToSingle(dr[6]);
                        countQ.CountedQTY = Convert.ToSingle(dr[7]);
                        countQ.Difference = Convert.ToSingle(dr[7]) - Convert.ToSingle(dr[6]);
                        countQ.CountStatus = _countStatus;
                    }
                    xps.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while submitting this count: " + ex.Message);
                return;
            }

            List<CountSchedule> cs = new List<CountSchedule>();

            try
            {
                cs = (from a in xps.CountSchedules
                      where a.CountID == countID
                      select a).ToList();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while accessing the count schedule data: " + ex.Message);
                return;
            }

            foreach (CountSchedule _csID in cs)
            {
                _csID.CountStatus = _countStatus;
                xps.SaveChanges();
            }
            System.Windows.Forms.MessageBox.Show("Count Saved");
            DgTracker.ItemsSource = null;
            dtCS = null;
            TxtCountId.Text = "";
            TxtName.Text = "";
        }

        public object ReviewLoad(string CountType)
        {
            Counts = new List<CCTracker>();
            dtCS = new DataTable();
            try
            {
                countID = int.Parse(TxtCountId.Text.Trim().ToUpper());
                if (countID == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Please enter a count ID number to load a count.");
                    return null;
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Count ID is Invalid");
                TxtCountId.Text = "";
                return null;
            }
            var q = (from a in xps.CCTrackers
                     where a.CountID == countID && a.CountStatus != "Complete"
                     select a).ToList();
            Counts = q;

            dtCS = new DataTable();
            dtCS.Columns.Add("CountDate");
            dtCS.Columns.Add("ORG");
            dtCS.Columns.Add("Subinventory");
            dtCS.Columns.Add("Locator");
            dtCS.Columns.Add("Item");
            dtCS.Columns.Add("Description");
            dtCS.Columns.Add("Onhand");
            dtCS.Columns.Add("Count");
            dtCS.Columns.Add("Difference");
            dtCS.Columns.Add("Action");
            dtCS.Columns.Add("Error");
            dtCS.Columns.Add("First Counter");
            dtCS.Columns.Add("Second Counter");
            dtCS.Columns.Add("CountStatus");
            dtCS.Columns.Add("Count ID");
            dtCS.Columns.Add("Key ID");

            foreach (CCTracker cs in Counts)
            {
                DataRow dr = dtCS.NewRow();
                dr[0] = Convert.ToDateTime(cs.CountDate).ToString("MM/dd/yyyy");
                dr[1] = cs.ORG;
                dr[2] = cs.Subinventory;
                dr[3] = cs.Locator;
                dr[4] = cs.PID;
                dr[5] = cs.Description;
                dr[6] = cs.SystemQTY;
                dr[7] = cs.CountedQTY;
                dr[8] = cs.Difference;
                dr[9] = cs.Action;
                dr[10] = cs.Error;
                dr[11] = cs.FirstCount;
                dr[12] = cs.SecondCount;
                dr[13] = cs.CountStatus;
                dr[14] = cs.CountID;
                dr[15] = cs.ID;
                dtCS.Rows.Add(dr);
            }
            DgFinal.ItemsSource = dtCS.DefaultView;
            return dtCS;
        }

        private void ReDepartmentsubmit(string CountType, string CountTypeUpdate)
        {
            xps = new XpresEntities();

            string _countStatus = "";
            string _counter = TxtName.Text;
            string _countType = "";
            try
            {
                _countType = dtCS.Rows[0][13].ToString();
                countID = Convert.ToInt32(dtCS.Rows[0][14]);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("No count sheet was loaded.");
                return;
            }
            if (_countType != CountType)
            {
                System.Windows.Forms.MessageBox.Show("This is not a the correct type of sheet to submit under this menu. Please make sure you if this is a first or second count to use the proper menu to submit.");
                return;
            }
            for (int i = 0; i < dtCS.Rows.Count; i++)
            {
                DataRow dr = dtCS.Rows[i];
                string _countQtyString = dr[7].ToString();
                if (_countQtyString == "")
                {
                    System.Windows.Forms.MessageBox.Show("Not all counts were entered. Please enter all counts without any blank entries");
                    return;
                }
            }
            for (int i = 0; i < dtCS.Rows.Count; i++)
            {
                DataRow dr = dtCS.Rows[i];
                string _action = dr[9].ToString();
                string _error = dr[10].ToString();
                float _countQty = Convert.ToSingle(dr[7]);
                float _sysQty = Convert.ToSingle(dr[6]);
                string _loc = dr[3].ToString();
                string _pid = dr[4].ToString();
                int _pkID = Convert.ToInt32(dr[15]);
                DateTime _countDate = Convert.ToDateTime(dr[0]);
                if (_countQty - _sysQty == 0)
                {
                    _countStatus = "Complete";
                }
                else
                {
                    if (_countType == "SecondSubmitReview")
                    {
                        if (_action == "" || _error == "")
                        {
                            System.Windows.Forms.MessageBox.Show("Please make sure the Action and Error column are filled in for any counts that needed to be reconciled.");
                            return;
                        }
                    }
                    _countStatus = CountTypeUpdate;
                }

                CCTracker countQ = new CCTracker();

                try
                {
                    countQ = (from c in xps.CCTrackers
                              where c.ID == _pkID
                              select c).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error while trying to submit reviewed count sheet: " + ex.Message);
                    return;
                }

                countQ.CountedQTY = _countQty;
                countQ.Difference = _countQty - _sysQty;
                countQ.Action = _action;
                countQ.Error = _error;
                countQ.CountStatus = _countStatus;
                xps.SaveChanges();
            }

            List<CountSchedule> cs = new List<CountSchedule>();

            try
            {
                cs = (from a in xps.CountSchedules
                      where a.CountID == countID
                      select a).ToList();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while retrieving count schedule info: " + ex.Message);
                return;
            }

            foreach (CountSchedule _csID in cs)
            {
                _csID.CountStatus = _countStatus;
                xps.SaveChanges();
            }
            System.Windows.Forms.MessageBox.Show("Count Saved");
            DgFinal.ItemsSource = null;
            dtCS = null;
        }

        public object ImportLocal()
        {
            string _path = "";
            Microsoft.Win32.OpenFileDialog selectFile = new Microsoft.Win32.OpenFileDialog();
            if (selectFile.ShowDialog() == true)
            {
                _path = selectFile.FileName;
                System.Windows.Forms.MessageBox.Show("File: " + _path + " successfuly loaded");
            }

            StreamReader sr;
            dtDelim = new DataTable();
            try
            {
                sr = new StreamReader(_path);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while converting CSV file: " + ex.Message);
                return null;
            }
            dtCS = new DataTable();
            try
            {
                int _rowCount = 0;
                string[] _columnNames = null;
                string[] _streamDataValues = null;
                while (!sr.EndOfStream)
                {
                    string _streamRowData = sr.ReadLine().Trim();
                    if (_streamRowData.Length > 0)
                    {
                        _streamRowData = _streamRowData.Replace(@"""", "");
                        _streamRowData = _streamRowData.Replace(@", ", @"\");
                        _streamDataValues = _streamRowData.Split(',');
                        if (_rowCount == 0)
                        {
                            _rowCount = 1;
                            _columnNames = _streamDataValues;
                            foreach (string csvHeader in _columnNames)
                            {
                                DataColumn dataColumn = new DataColumn(csvHeader.ToUpper(),
                                    typeof(string));
                                dataColumn.DefaultValue = string.Empty;
                                dtCS.Columns.Add(dataColumn);
                            }
                        }
                        else
                        {
                            DataRow dr = dtCS.NewRow();
                            for (int i = 0; i < _columnNames.Length; i++)
                            {
                                dr[_columnNames[i]] = _streamDataValues[i] == null ?
                                    string.Empty : _streamDataValues[i].ToString();
                            }
                            dtCS.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while deliminating CSV file: " + _path + ": " + ex.Message);
                return null;
            }
            finally
            {
                sr.Close();
                sr.Dispose();
            }
            return dtCS;
        }

        private void ExportLocal()
        {
            if (dtCS == null)
            {
                System.Windows.Forms.MessageBox.Show("There was no count sheet loaded.");
            }

            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dtCS.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dtCS.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(",", fields));
            }

            Microsoft.Win32.SaveFileDialog exportFile = new Microsoft.Win32.SaveFileDialog();
            exportFile.DefaultExt = "csv";
            exportFile.RestoreDirectory = true;

            if (exportFile.ShowDialog() == true)
            {
                string name = exportFile.FileName;
                try
                {
                    File.WriteAllText(name, sb.ToString());
                    System.Windows.Forms.MessageBox.Show(name + " saved succefully");
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Could not save file: " + ex.Message);
                    return;
                }
            }
        }

        #endregion Count Management Methods

        #endregion Methods

        #region Special Events

        private void dgFinal_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtCS != null)
                {
                    for (int row = 0; row < dtCS.Rows.Count; row++)
                    {
                        DataRow dr = dtCS.Rows[row];
                        float _countQty = Convert.ToSingle(dr[7]);
                        float _sysQty = Convert.ToSingle(dr[6]);
                        if (_countQty.ToString() == "" || _sysQty.ToString() == "")
                        {
                            return;
                        }
                        else
                        {
                            dr[8] = _countQty - _sysQty;
                        }
                    }
                    DgFinal.ItemsSource = dtCS.DefaultView;
                }
            }
            catch
            {
                return;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in GrdMain.Children)
            {
                sp.Visibility = sp.Name == "spMainMenu" ? Visibility.Visible : Visibility.Hidden;
            }
            foreach (StackPanel sp in GrdLeftPanel.Children)
            {
                sp.Visibility = Visibility.Hidden;
            }
        }

        #endregion Special Events
    }
}