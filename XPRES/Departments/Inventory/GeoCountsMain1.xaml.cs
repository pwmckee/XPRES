using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using XPRES.DAL;
using XPRES.Inventory.Views.Controls;
using XPRES.Main;

namespace XPRES.Inventory.Views
{
    /// <summary>
    /// Interaction logic for GeoCounts.xaml
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
        
        public GeoCountsMain1()
        {
            InitializeComponent();
            dtTracker = new DataTable();
            dtCS = new DataTable();
            dtDelim = new DataTable();
            dtCountRange = new DataTable();
            SDate = new DateTime();
            EDate = new DateTime();
            Counts = new List<CCTracker>();
            xps = new XpresEntities();
            countStatus = "Complete";
            TestConnection();
            FillCountYears();
            countID = 0;            
        }

        private void TestConnection()
        {
            DbConnection conn =  xps.Database.Connection;
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while connecting to the database: " + ex.Message);
                Environment.Exit(0);
            }
        }

        #region Button Click Events

        #region Main Menu Buttons

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is InventoryMain)
                {
                    _open = true;

                }
            }
            if (!_open)
            {
                InventoryMain home = new InventoryMain();
                home.Show();
                this.Close();
            }
        }

        private void btnCCTracker_Click(object sender, RoutedEventArgs e)
        {
            dtpSdCCtracker.SelectedDate = DateTime.Today.Date;
            dtpEdCCtracker.SelectedDate = DateTime.Today.Date;
            foreach (StackPanel sp in grdMain.Children)
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
            foreach (StackPanel sp in grdLeftPanel.Children)
            {
                sp.Visibility = Visibility.Hidden;
            }
            foreach (Grid grd in grdCenterView.Children)
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
            foreach (StackPanel sp in grdMain.Children)
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

        #endregion

        #region CCTracker Buttons

        private void btnFindCCTracker_Click(object sender, RoutedEventArgs e)
            {
                foreach (DataGrid dg in grdDataGrid.Children)
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
                dtpSdCCtracker.Text = "";
                dtpEdCCtracker.Text = "";
                txtPIDCCTracker.Text = "";
                dgTracker.ItemsSource = null;
                GC.Collect();            
            }

        #endregion

        #region Count Management Buttons

        private void btnCreateCounts_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in grdLeftPanel.Children)
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
            txtCountID.Text = "";
            txtName.Text = "";
        }

        private void btnFirstCounts_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in grdLeftPanel.Children)
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
            foreach (Grid grd in grdCenterView.Children)
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
            txtCountID.Text = "";
            txtName.Text = "";

        }

        private void btnSecondCounts_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in grdLeftPanel.Children)
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
            foreach (Grid grd in grdCenterView.Children)
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
            txtCountID.Text = "";
            txtName.Text = "";
        }

        private void btnReviewCounts_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in grdLeftPanel.Children)
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
            foreach (Grid grd in grdCenterView.Children)
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
            spReqPanel.Children.Clear();
        }

        #region First Count Buttons

        private void btnFirstCountCreate_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
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
            foreach (DataGrid dg in grdDataGrid.Children)
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
            cbxCountArea.SelectedItem = -1;
            cbxZone.SelectedItem = -1;
        }

        private void btnFirstExport_Click(object sender, RoutedEventArgs e)
        {
            ExportSheet();
            txtCountID.Text = "";
            txtName.Text = "";


        }

        private void btnLoadFirst_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
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
            foreach (DataGrid dg in grdDataGrid.Children)
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
            txtCountID.Text = "";
            txtName.Text = "";
        }

        private void btnSubmitFirst_Click(object sender, RoutedEventArgs e)
        {
            CountType = "NewCreate";
            CountTypeUpdate = "FirstSubmit";
            SubmitCount(CountType, CountTypeUpdate);
            txtCountID.Text = "";
            txtName.Text = "";
        }

        #endregion

        #region Second Count Buttons

        private void btnSecondCountExport_Click(object sender, RoutedEventArgs e)
        {
            ExportSheet();
            txtCountID.Text = "";
            txtName.Text = "";
            countStatus = "SecondReady";
            GetOpenCounts(countStatus);
        }

        private void btnLoadSecond_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
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
            foreach (DataGrid dg in grdDataGrid.Children)
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

        #endregion

        #region Review Count Buttons

        private void btnViewFirstReview_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
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
            foreach (Grid grd in grdCenterView.Children)
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
            foreach (DataGrid dg in grdDataGrid.Children)
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
            ReviewSubmit(CountType, CountTypeUpdate);
        }

        private void btnViewSecondReview_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
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
            foreach (Grid grd in grdCenterView.Children)
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
            foreach (DataGrid dg in grdDataGrid.Children)
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
            ReviewSubmit(CountType, CountTypeUpdate);
        }

        private void btnDeleteCount_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #endregion

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

        #endregion

        #endregion

        #region Methods

        #region CCTracker Methods

        private void GetCounts()
        {
            dtTracker = null;                    
            if (dtpSdCCtracker.SelectedDate.ToString() != "")
            {
                try
                {
                    DateTime.TryParse(dtpSdCCtracker.SelectedDate.ToString(), out SDate);
                }
                catch
                {
                    MessageBox.Show("Invalid From date.");
                    return;
                }
            }
            else SDate = Convert.ToDateTime("1/1/2000");            
            if (dtpEdCCtracker.SelectedDate.ToString() != "")
            {
                try
                {
                    DateTime.TryParse(dtpEdCCtracker.SelectedDate.ToString(), out EDate);
                }
                catch
                {
                    MessageBox.Show("Invalid To date.");
                    return;
                }
            }
            else EDate = DateTime.Today.Date;            
            PID = txtPIDCCTracker.Text;            
            if (PID != null && PID.ToString() == "")
            {
                PID = null;
            }

            if (PID == null)
            {
                var q = (from a in xps.CCTrackers
                        where a.CountDate >= SDate && a.CountDate <= EDate
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
                dtTracker.Rows.Add(dr);
            }
            dgTracker.ItemsSource = dtTracker.DefaultView;
            
            dgTracker.Columns[8].MaxWidth = 200;
            dgTracker.Columns[9].MaxWidth = 200;
        }

        #endregion

        #region Count Schedule Methods

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
                MessageBox.Show("Error while connecting to the database: " + ex);
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

        private void FillCountRange()
        {
            string range = cbxCountArea.SelectedItem.ToString();
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
            dgCountRange.ItemsSource = dtCountRange.DefaultView;
            dgCountRange.IsReadOnly = true;
        }

        private void FillZones()
        {
            string range = cbxCountArea.SelectedItem.ToString();
            var q = (from a in xps.CountSchedules
                     where a.CountArea == range
                     select a.Zone).ToList().Distinct();
            cbxZone.ItemsSource = q;
        }

        #endregion

        #region Count Management Methods

        private void GetOpenCounts(string countStatus)
        {
            if (spReqPanel.Children.Count > 0)
            {               
                    spReqPanel.Children.Clear();             
            }
            CycleCountsControl _tempUserControl = new CycleCountsControl();
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
                _tempUserControl = new CycleCountsControl();
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

        private void addStackPanelChild(CycleCountsControl _tempUserControl)
        {           
            spReqPanel.Children.Add(_tempUserControl);
            spReqPanel.UpdateLayout();            
        }

        public object NewConvertCSV()
        {
            string path = "";
            Microsoft.Win32.OpenFileDialog selectFile = new Microsoft.Win32.OpenFileDialog();
            if (selectFile.ShowDialog() == true)
            {
                path = selectFile.FileName;
                System.Windows.MessageBox.Show("File: " + path + " successfuly loaded");
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
                System.Windows.MessageBox.Show("Error while converting CSV file: " + ex.Message);
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
                System.Windows.MessageBox.Show("Error while deliminating CSV file: " + path + ": " + ex.Message);
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
                    System.Windows.MessageBox.Show("Error while creating table.  Please make sure the proper type of ERP count data was loaded.");
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
                            MessageBox.Show("If you need to add this item to the database, please ask with a supervisor.");
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
                dgTracker.ItemsSource = dtCS.DefaultView;
                dgTracker.Columns[6].Visibility = Visibility.Hidden;
                dgTracker.Columns[8].Visibility = Visibility.Hidden;
                dgTracker.Columns[9].Visibility = Visibility.Hidden;
            }
            else
            {
                System.Windows.MessageBox.Show("Error while converting CSV file: No data was entered");
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
                    MessageBox.Show("This is not a newly created count sheet.  Please create a new sheet before trying to save using this menu.");
                    return;
                }

            }
            catch
            {
                MessageBox.Show("No count sheet was loaded.");
                return;
            }
            selectedZone = Convert.ToInt32(cbxZone.SelectedItem);
            if (selectedZone == 0)
            {
                MessageBox.Show("Please select which zone this count is for.");
                return;
            }
            string _cFrom = txtCountFrom.Text;
            string _cTo = txtCountTo.Text;
            int _idFrom = 0;
            int _idTo = 0;
            if (_cFrom.ToString() == "" || _cTo.ToString() == "")
            {
                MessageBox.Show("Please enter both the from and to count range.");
                return;
            }
            CountSchedule _from = new CountSchedule();
            var from = (from a in xps.CountSchedules
                        where a.CountRange == _cFrom
                        select a).SingleOrDefault();

            _from = from;
            try
            {
                _idFrom = Convert.ToInt32(_from.ID);
            }
            catch
            {
                MessageBox.Show("Invalid 'From' count range entered.");
                return;
            }
            CountSchedule _to = new CountSchedule();
            var to = (from a in xps.CountSchedules
                        where a.CountRange == _cTo
                        select a).SingleOrDefault();
            _to = to;
            try
            {
                _idTo = Convert.ToInt32(_to.ID);
            }
            catch
            {
                MessageBox.Show("Invalid 'To' count range entered.");
                return;
            }
            int _year = Convert.ToInt32(cbxYear.SelectedItem);
            int _qtr = Convert.ToInt32(cbxQuarter.SelectedItem);
            int countID = int.Parse(_year.ToString() + _qtr.ToString() + _idTo.ToString());
            for (int i = _idFrom; i <= _idTo; i++)
            {
                CountSchedule cs = (from a in xps.CountSchedules
                                    where a.ID == i
                                    select a).SingleOrDefault();
                if (cs.CountID != null)
                {
                    MessageBox.Show("One or more locations in this count have already had a count ID created for them.");
                    return;
                }
                cs.ActualDate = DateTime.Now;
                cs.CountStatus = "Created";
                cs.CountID = countID;
                xps.SaveChanges();
            }
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
            MessageBox.Show("Count Sheet Saved.");
            dgTracker.ItemsSource = null;
            cbxZone.SelectedIndex = -1;
            txtCountFrom.Text = "";
            txtCountTo.Text = "";
            txtCountID.Text = "";
        }

        private void ExportSheet()
        {
            string _counter = txtName.Text;
            int countID = 0;
            if (dtCS == null)
            {
                MessageBox.Show("There is no count sheet available to export.");
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
                        MessageBox.Show("This count already has a first counter.");
                        return;
                    }
                }
                if (CountType == "SecondReady")
                {
                    if (_firstCount != _counter.ToLower())
                    {
                        if (dtCS.Rows[0][11].ToString() != "")
                        {
                            MessageBox.Show("This count already has a second counter.");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("The First and Second counter cannot be the same person.");
                        return;
                    }
                }
                if (dtCS.Rows[0][8].ToString() != CountType)
                {
                    MessageBox.Show("This is not a the correct type of sheet to submit under this menu. Please make sure you if this is a first or second count to use the proper menu to submit.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("No count sheet was loaded.");
                return;
            }            
            if (dtCS == null)
            {
                MessageBox.Show("No count sheet was loaded.");
                return;
            }            
            if (_counter == "")
            {
                MessageBox.Show("Counter Name cannot be blank");
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

                    System.Windows.MessageBox.Show(name + " saved succefully");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Could not save file: " + ex.Message);
                }
            }
        }

        public object LoadCount(string CountType)
        {
            Counts = new List<CCTracker>();
            dtCS = new DataTable();
            try
            {
                countID = int.Parse(txtCountID.Text);
                if (countID == 0)
                {
                    MessageBox.Show("Please enter a count ID number to load a count.");
                    return null;
                }
            }
            catch
            {
                foreach (Grid grd in grdCenterView.Children)
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
                MessageBox.Show("Count ID is Invalid");                
                return null;
            }
            var q = (from a in xps.CCTrackers
                     where a.CountID == countID && a.CountStatus != "Complete"
                     select a).ToList();
            Counts = q;

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
                dtCS.Rows.Add(dr);
            }
            dgTracker.ItemsSource = dtCS.DefaultView;
            dgTracker.Columns[6].Visibility = Visibility.Hidden;
            dgTracker.Columns[8].Visibility = Visibility.Hidden;
            dgTracker.Columns[10].Visibility = Visibility.Hidden;
            dgTracker.Columns[11].Visibility = Visibility.Hidden;
            return dtCS;
        }

        private void SubmitCount(string CountType, string CountTypeUpdate)
        {
            string _countStatus = "";
            int _writeIns = 0;
            #region Primary Validation
            try
            {
                countID = Convert.ToInt32(dtCS.Rows[0][9]);
                string _counter;
                if (dtCS.Rows[0][8].ToString() != CountType)
                {
                    MessageBox.Show("This is not a the correct type of sheet to submit under this menu. Please make sure you if this is a first or second count to use the proper menu to submit.");
                    return;
                }
                if (countID == 0)
                {
                    MessageBox.Show("No count sheet was loaded.");
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
                    MessageBox.Show("This count has not had a Counter assigned to it yet.  Please export the count first.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("No count sheet was loaded.");
                return;
            }
            #endregion

            #region Count Validation and Add "Write-ins"

            for (int i = 0; i < dtCS.Rows.Count; i++)
            {
                DataRow _checkRow = dtCS.Rows[i];
                if (_checkRow[7].ToString() == "" || _checkRow[3].ToString() == "" || _checkRow[4].ToString() == "")
                {
                    MessageBox.Show("Not all required information was entered. Please make sure that all count qty, part number, and locator information is not blank");
                    return;
                }

                string _date = _checkRow[0].ToString();
                string _org = _checkRow[1].ToString();
                string _sub = _checkRow[2].ToString();
                string _pid = _checkRow[4].ToString();

                if (_date == "" || _org == "" || _sub == "")
                {
                    CCTracker cc = new CCTracker();
                    var _cc = (from a in xps.CCTrackers
                                  where a.CountID == countID
                                  select a).First();
                    cc = _cc;
                    UnitCost uc = new UnitCost();
                    try
                    {
                        
                        var _uc = (from a in xps.UnitCosts
                                   where a.Item == _pid
                                   select a).First();
                        uc = _uc;
                    }
                    catch
                    {
                        MessageBox.Show("There is no cost listed for item " + _pid + ".  Please have the cost sheet updated with the unit information before creating this count.");
                        return;
                    }
                    CCTracker ccNew = new CCTracker();
                    ccNew = cc;
                    ccNew.Locator = _checkRow[3].ToString();
                    ccNew.PID = _pid;
                    ccNew.Description = uc.Description;
                    ccNew.SystemQTY = 0;
                    ccNew.Difference = Convert.ToInt32(_checkRow[7]) - 0;
                    ccNew.CountedQTY = Convert.ToDouble(_checkRow[7]);
                    ccNew.CountStatus = CountTypeUpdate + "Review";
                    ccNew.UnitCost = uc.Unit_Cost;
                    xps.CCTrackers.Add(ccNew);
                    _writeIns++;
                }
            }
            xps.SaveChanges();

            #endregion

            for (int i = 0; i < dtCS.Rows.Count - _writeIns; i++)
            {
                DataRow dr = dtCS.Rows[i];
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
                CCTracker countQ = (from c in xps.CCTrackers
                                    where c.CountDate == _countDate && c.Locator == _loc && c.PID == _pid && c.CountID == countID
                                    select c).SingleOrDefault();
                countQ.CountedQTY = Convert.ToInt32(dr[7]);
                countQ.Difference = Convert.ToInt32(dr[7]) - Convert.ToInt32(dr[6]);
                countQ.CountStatus = _countStatus;
            }
            xps.SaveChanges();
            List<CountSchedule> cs = (from a in xps.CountSchedules
                                      where a.CountID == countID
                                      select a).ToList();
            foreach (CountSchedule _csID in cs)
            {
                _csID.CountStatus = _countStatus;
                xps.SaveChanges();
            }
            MessageBox.Show("Count Saved");
            dgTracker.ItemsSource = null;
            dtCS = null;
            txtCountID.Text = "";
            txtName.Text = "";
        }

        public object ReviewLoad(string CountType)
        {
            Counts = new List<CCTracker>();
            dtCS = new DataTable();
            try
            {
                countID = int.Parse(txtCountID.Text);
                if (countID == 0)
                {
                    MessageBox.Show("Please enter a count ID number to load a count.");
                    return null;
                }
            }
            catch
            {
                MessageBox.Show("Count ID is Invalid");
                txtCountID.Text = "";
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
                dtCS.Rows.Add(dr);
            }
            dgFinal.ItemsSource = dtCS.DefaultView;
            return dtCS;
        }

        private void ReviewSubmit(string CountType, string CountTypeUpdate)
        {
            string _countStatus = "";
            string _counter = txtName.Text;
            string _countType = "";
            try
            {
                _countType = dtCS.Rows[0][13].ToString();
                countID = Convert.ToInt32(dtCS.Rows[0][14]);
            }
            catch
            {
                System.Windows.MessageBox.Show("No count sheet was loaded.");
                return;
            }
            if (_countType != CountType)
            {
                MessageBox.Show("This is not a the correct type of sheet to submit under this menu. Please make sure you if this is a first or second count to use the proper menu to submit.");
                return;
            }
            for (int i = 0; i < dtCS.Rows.Count; i++)
            {
                DataRow dr = dtCS.Rows[i];
                string _countQtyString = dr[7].ToString();
                if (_countQtyString == "")
                {
                    MessageBox.Show("Not all counts were entered. Please enter all counts without any blank entries");
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
                DateTime _countDate = Convert.ToDateTime(dr[0]);
                if (_countQty - _sysQty == 0)
                {
                    _countStatus = "Complete";
                }
                else
                {
                    if (_countType == "SecondSubmitReview")
                    {
                        if(_action == "" || _error == "")
                        {
                            MessageBox.Show("Please make sure the Action and Error column are filled in for any counts that needed to be reconciled.");
                            return;
                        }
                    }
                    _countStatus = CountTypeUpdate;
                }
                CCTracker countQ = (from c in xps.CCTrackers
                                    where c.CountDate == _countDate && c.Locator == _loc && c.PID == _pid && c.CountID == countID
                                    select c).SingleOrDefault();
                countQ.CountedQTY = _countQty;
                countQ.Difference = _countQty - _sysQty;
                countQ.Action = _action;
                countQ.Error = _error;
                countQ.CountStatus = _countStatus;
                xps.SaveChanges();
            }
            List<CountSchedule> cs = (from a in xps.CountSchedules
                                      where a.CountID == countID
                                      select a).ToList();
            foreach (CountSchedule _csID in cs)
            {
                _csID.CountStatus = _countStatus;
                xps.SaveChanges();
            }
            MessageBox.Show("Count Saved");
            dgFinal.ItemsSource = null;
            dtCS = null;
        }

        #endregion

        #endregion

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
                    dgFinal.ItemsSource = dtCS.DefaultView;
                }
            }
            catch
            {
                MessageBox.Show("Cannot add 'write-ins' from this form.  Convert to first or second count status to add 'write-in' counts.");
                return;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {   
            foreach (StackPanel sp in grdMain.Children)
            {
                if (sp.Name == "spMainMenu")
                {
                    sp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp.Visibility = Visibility.Hidden;
                }

            }
            foreach (StackPanel sp in grdLeftPanel.Children)
            {
                sp.Visibility = Visibility.Hidden;
            }
        }


        #endregion

        private void btnCountSched_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming Soon!");            
        }

        private void btnCountMetrics_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming Soon!");
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming Soon!");
        }
    }
}
