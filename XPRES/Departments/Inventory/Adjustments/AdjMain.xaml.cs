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
using XPRES.Departments.Inventory.Adjustments.Controls;
using XPRES.Departments.Inventory.Views;
using XPRES.Helpers;
using XPRES.Main.Views;

namespace XPRES.Departments.Inventory.Adjustments
{
    public partial class AdjMain : Window
    {
        private DataTable dtTracker;
        private DataTable dtReq;
        private DataTable dtCountSheet;
        private DataTable dtErpPaste;
        private DateTime sDate;
        private DateTime eDate;
        private List<Adjustment> Adjustments;
        private XpresEntities xps;
        private string pid;
        private string seq;
        private string adjType;
        private string reqNum;
        private string adjStatusUpdate;
        private string adjStatus;

        public AdjMain()
        {
            InitializeComponent();
            dtTracker = new DataTable();
            dtReq = new DataTable();
            dtCountSheet = new DataTable();
            dtErpPaste = new DataTable();
            sDate = new DateTime();
            eDate = new DateTime();
            Adjustments = new List<Adjustment>();
            xps = new XpresEntities();
            adjStatus = "Complete";
        }

        private void TestConnection()
        {
            DbConnection conn = xps.Database.Connection;
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while connecting to the database: " + ex.Message);
                Environment.Exit(0);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        #region Button Click Events

        #region Main Menu Buttons

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is MainWindow)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                MainWindow home = new MainWindow();
                home.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "XPRES")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        private void btnDash_Click(object sender, RoutedEventArgs e)
        {
            bool _open = false;
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is InvMain)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                InvMain dash = new InvMain();
                dash.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Inventory Dashboard")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        private void btnAdjTracker_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in grdMain.Children)
            {
                if (sp.Name == "spAdjTracker" || sp.Name == "spMainMenu")
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

        private void btnAdjMgmt_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in grdMain.Children)
            {
                if (sp.Name == "spAdjMgmt" || sp.Name == "spMainMenu")
                {
                    sp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnAdjRecon_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in grdMain.Children)
            {
                if (sp.Name == "spReconTools" || sp.Name == "spMainMenu")
                {
                    sp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnAdjMetrics_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel sp in grdMain.Children)
            {
                if (sp.Name == "spAdjMetrics" || sp.Name == "spMainMenu")
                {
                    sp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Coming Soon!");
            return;
        }

        #endregion Main Menu Buttons

        #region Adj Tracker Buttons

        private void btnFindAdjTracker_Click(object sender, RoutedEventArgs e)
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
            GetAdj();
        }

        private void btnClearAdjTracker_Click(object sender, RoutedEventArgs e)
        {
            dtpSdAdjtracker.Text = "";
            dtpEdAdjtracker.Text = "";
            txtPIDAdjTracker.Text = "";
            txtSeqAdjTracker.Text = "";
            txtAdjType.Text = "";
            txtAdjReqNum.Text = "";
            dgTracker.ItemsSource = null;
            GC.Collect();
        }

        #endregion Adj Tracker Buttons

        #region Adj Management Buttons

        #region Create

        private void btnCreateAdj_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
            {
                if (grd.Name == "grdAdjCreate")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            //grdAdjCreate.Children.Clear();
            if (grdAdjCreate.Children.Count == 0)
            {
                AdjCreateControl _adjCtrl = new AdjCreateControl();
                grdAdjCreate.Children.Add(_adjCtrl);
            }

            txtRevReqNum.Text = "";
        }

        private void btnAdjExport_Click(object sender, RoutedEventArgs e)
        {
            ExportSheet();
            txtRevReqNum.Text = "";
        }

        #endregion Create

        #region Review

        private void btnReviewAdj_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
            {
                if (grd.Name == "grdAdjControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid grd in grdAdjControls.Children)
            {
                if (grd.Name == "grdRevControls" || grd.Name == "grdReqStack" || grd.Name == "grdReqStackHeader")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            adjStatus = "Needs Review";
            GetOpenAdj(adjStatus);
            txtRevReqNum.Text = "";
        }

        private void btnRevAdjLoad_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
            {
                if (grd.Name == "grdAdjControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid grd in grdAdjControls.Children)
            {
                if (grd.Name == "grdRevControls" || grd.Name == "grdAdjLoad")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (DataGrid dg in grdAdjLoad.Children)
            {
                if (dg.Name == "dgAdjLoad")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
            adjStatus = "Needs Review";
            LoadAdj(adjStatus);
        }

        private void btnRevAdjSubmit_Click(object sender, RoutedEventArgs e)
        {
            adjStatus = "Needs Review";
            adjStatusUpdate = "Review Complete";
            SubmitAdj(adjStatus, adjStatusUpdate);
            txtRevReqNum.Text = "";
        }

        private void btnRevAdjReject_Click(object sender, RoutedEventArgs e)
        {
            adjStatus = "Needs Review";
            RejectSequence(adjStatus);
        }

        #endregion Review

        #region Finalize

        private void btnFinalizeAdj_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
            {
                if (grd.Name == "grdAdjControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid grd in grdAdjControls.Children)
            {
                if (grd.Name == "grdFinControls" || grd.Name == "grdReqStack" || grd.Name == "grdReqStackHeader")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }

            adjStatus = "Review Complete";
            GetOpenAdj(adjStatus);
            txtFinReqNum.Text = "";
        }

        private void btnFinAdjLoad_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
            {
                if (grd.Name == "grdAdjControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid grd in grdAdjControls.Children)
            {
                if (grd.Name == "grdFinControls" || grd.Name == "grdAdjLoad")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (DataGrid dg in grdAdjLoad.Children)
            {
                if (dg.Name == "dgAdjLoad")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
            adjStatus = "Review Complete";
            LoadAdj(adjStatus);
        }

        private void btnFinAdjSubmit_Click(object sender, RoutedEventArgs e)
        {
            adjStatus = "Review Complete";
            adjStatusUpdate = "Complete";
            SubmitAdj(adjStatus, adjStatusUpdate);
            txtFinReqNum.Text = "";
        }

        #endregion Finalize

        #endregion Adj Management Buttons

        #region Reconciliation Buttons

        private void btnBomChanges_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
            {
                if (grd.Name == "grdAdjControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid grd in grdAdjControls.Children)
            {
                if (grd.Name == "grdBOMInquiry" || grd.Name == "grdAdjLoad")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (DataGrid dg in grdAdjLoad.Children)
            {
                if (dg.Name == "dgBomLoad")
                {
                    dg.Visibility = Visibility.Visible;
                }
                else
                {
                    dg.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnFindBomChange_Click(object sender, RoutedEventArgs e)
        {
            FindBomChange();
        }

        private void btnVarCalc_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
            {
                if (grd.Name == "grdAdjControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid grd in grdAdjControls.Children)
            {
                if (grd.Name == "grdReconSheetPanel" || grd.Name == "grdReconSheetMenu")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnViewCounts_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
            {
                if (grd.Name == "grdAdjControls")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
            foreach (Grid grd in grdAdjControls.Children)
            {
                if (grd.Name == "grdViewCountsMenu" || grd.Name == "grdViewCountsDatagrid")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnPasteConvert_Click(object sender, RoutedEventArgs e)
        {
            PasteConvert();
        }

        private void btnSaveConvert_Click(object sender, RoutedEventArgs e)
        {
            SaveRecon();
        }

        private void btnClearRecon_Click(object sender, RoutedEventArgs e)
        {
            if (dtErpPaste != null)
            {
                //dtErpPaste.Clear();
                dtErpPaste = new DataTable();
                txtErpPaste.Text = string.Empty;
                spReconSteps.Children.Clear();
                dgReconSheet.ItemsSource = dtErpPaste.DefaultView;
            }
        }

        private void btnDeleteEmptyRows_Click(object sender, RoutedEventArgs e)
        {
            if (dtErpPaste != null)
            {
                for (int i = 0; i < dtErpPaste.Rows.Count; i++)
                {
                    DataRow dr = dtErpPaste.Rows[i];
                    if (dr[0].ToString() == string.Empty)
                    {
                        dr.Delete();
                    }
                }
            }
        }

        private void btnSugstTrns_Click(object sender, RoutedEventArgs e)
        {
            ReconTransferSuggest();
        }

        private void btnSugstAdj_Click(object sender, RoutedEventArgs e)
        {
            ReconAdjustSuggest();
        }

        private void btnExportRecon_Click(object sender, RoutedEventArgs e)
        {
            ExportRecon();
        }

        private void btnRemoveTrns_Click(object sender, RoutedEventArgs e)
        {
            RemoveTransfer();
        }

        private void btnRemoveAdj_Click(object sender, RoutedEventArgs e)
        {
            RemoveAdjustment();
        }

        #endregion Reconciliation Buttons

        #region ViewCounts Buttons

        private void btnFindCounts_Click(object sender, RoutedEventArgs e)
        {
            GetCounts();
        }

        private void btnClearCounts_Click(object sender, RoutedEventArgs e)
        {
            dtpSdCountSheet.Text = "";
            dtpEdCountSheet.Text = "";
            txtPIDCountSheet.Text = "";
            txtNameCountSheet.Text = "";
            dgViewCounts.ItemsSource = null;
            GC.Collect();
        }

        private void btnExportCounts_Click(object sender, RoutedEventArgs e)
        {
            ExportCounts();
        }

        #endregion ViewCounts Buttons

        #region Metrics

        private void btnMaadCtrl_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
            {
                if (grd.Name != "grdMaadCtrl")
                {
                    grd.Visibility = Visibility.Hidden;
                }
                else
                {
                    grd.Visibility = Visibility.Visible;
                }
                if (grdMaadCtrl.Children.Count == 0)
                {
                    AdjMetricsControl _adjCtrl = new AdjMetricsControl();
                    grdMaadCtrl.Children.Add(_adjCtrl);
                }
            }
        }

        private void btnWklyRep_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdCenterView.Children)
            {
                if (grd.Name != "grdAdjMetrics")
                {
                    grd.Visibility = Visibility.Hidden;
                }
                else
                {
                    grd.Visibility = Visibility.Visible;
                }
            }
            WeeklyMetrics();
        }

        #endregion Metrics

        #endregion Button Click Events

        #region Methods

        #region Adjustment Tracker Methods

        private void GetAdj()
        {
            bool _byReqDate = false;
            bool _byCompDate = false;
            if (chkReqDate.IsChecked.Value)
                _byReqDate = true;
            if (chkCompDate.IsChecked.Value)
                _byCompDate = true;
            if (!_byReqDate && !_byCompDate)
            {
                System.Windows.Forms.MessageBox.Show("Please select whether you want to search by Request Date or Completion Date");
                return;
            }
            dtTracker = null;
            if (dtpSdAdjtracker.SelectedDate.ToString() != "")
            {
                try
                {
                    DateTime.TryParse(dtpSdAdjtracker.SelectedDate.ToString(), out sDate);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid From date.");
                    return;
                }
            }
            else
            {
                sDate = Convert.ToDateTime("1/1/2000");
            }

            if (dtpEdAdjtracker.SelectedDate.ToString() != "")
            {
                try
                {
                    DateTime.TryParse(dtpEdAdjtracker.SelectedDate.ToString(), out eDate);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid To date.");
                    return;
                }
            }
            else
            {
                eDate = DateTime.Today.Date;
            }

            pid = txtPIDAdjTracker.Text.Trim().ToUpper();
            seq = txtSeqAdjTracker.Text.Trim().ToUpper();
            adjType = txtAdjType.Text;
            reqNum = txtAdjReqNum.Text.Trim().ToUpper();
            if (pid == "") pid = null;
            if (seq == "") seq = null;
            if (adjType == "") adjType = null;
            if (reqNum == "") reqNum = null;

            var q = (from a in xps.Adjustments
                     select a);
            if (_byReqDate)
                q = q.Where(a => a.ReqDate >= sDate && a.ReqDate <= eDate);
            if (_byCompDate)
                q = q.Where(a => a.CompDate >= sDate && a.CompDate <= eDate);
            if (pid != null)
                q = q.Where(a => a.PID == pid);
            if (seq != null)
                q = q.Where(a => a.SEQ == seq);
            if (adjType != null)
                q = q.Where(a => a.Type == adjType);
            if (reqNum != null)
                q = q.Where(a => a.ReqID == reqNum);

            q = q.OrderBy(a => a.ReqDate);

            Adjustments = q.ToList();

            dtTracker = new DataTable();
            dtTracker.Columns.Add("Part Number");
            dtTracker.Columns.Add("Sequence");
            dtTracker.Columns.Add("Description");
            dtTracker.Columns.Add("Adj Qty");
            dtTracker.Columns.Add("Adj Value");
            dtTracker.Columns.Add("Comments");
            dtTracker.Columns.Add("Request Date");
            dtTracker.Columns.Add("Completion Date");
            dtTracker.Columns.Add("Adj Type");
            dtTracker.Columns.Add("Status");
            dtTracker.Columns.Add("File ID");
            dtTracker.Columns.Add("Request ID");
            dtTracker.Columns.Add("Requester");

            foreach (Adjustment adj in Adjustments)
            {
                DataRow dr = dtTracker.NewRow();
                dr[0] = adj.PID;
                dr[1] = adj.SEQ;
                dr[2] = adj.Desc;
                dr[3] = adj.Qty;
                dr[4] = adj.NetValue;
                dr[5] = adj.Comments;
                dr[6] = Convert.ToDateTime(adj.ReqDate).ToString("MM/dd/yyyy");
                if (adj.CompDate != null)
                    dr[7] = Convert.ToDateTime(adj.CompDate).ToString("MM/dd/yyyy");
                dr[8] = adj.Type;
                dr[9] = adj.Status;
                dr[10] = adj.FileID;
                dr[11] = adj.ReqID;
                dr[12] = adj.Requester;
                dtTracker.Rows.Add(dr);
            }
            dgTracker.ItemsSource = dtTracker.DefaultView;

            dgTracker.Columns[2].MaxWidth = 200;
            dgTracker.Columns[5].MaxWidth = 200;
        }

        #endregion Adjustment Tracker Methods

        #region Adj Management Methods

        private void GetOpenAdj(string adjStatus)
        {
            if (spReqPanel.Children.Count > 0)
            {
                spReqPanel.Children.Clear();
            }
            AdjStackControl _tempUserControl = new AdjStackControl();
            List<Adjustment> _controlData = new List<Adjustment>();
            DataTable _dtAdjReq = new DataTable();
            _dtAdjReq.Columns.Add("ReqID");
            _dtAdjReq.Columns.Add("NetValue");
            _dtAdjReq.Columns.Add("AdjType");
            _dtAdjReq.Columns.Add("ReqDate");
            _dtAdjReq.Columns.Add("Requester");
            try
            {
                var q = (from a in xps.Adjustments
                         where a.Status == adjStatus
                         select a).ToList();
                _controlData = q;
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to the database.");
                return;
            }

            foreach (Adjustment adj in _controlData)
            {
                DataRow dr = _dtAdjReq.NewRow();
                dr[0] = adj.ReqID;
                dr[1] = adj.NetValue;
                dr[2] = adj.Type;
                dr[3] = adj.ReqDate;
                dr[4] = adj.Requester;
                _dtAdjReq.Rows.Add(dr);
            }
            for (int i = 0; i < _dtAdjReq.Rows.Count - 1; i++)
            {
                if (_dtAdjReq.Rows[i][0].ToString() == _dtAdjReq.Rows[i + 1][0].ToString())
                {
                    _dtAdjReq.Rows[i + 1].Delete();
                    i--;
                }
            }
            foreach (DataRow dr in _dtAdjReq.Rows)
            {
                _tempUserControl = new AdjStackControl();
                _tempUserControl.Name = "Request" + dr[2];
                _tempUserControl.CtrlReqNum.Text = dr[0].ToString();
                _tempUserControl.CtrlNetCost.Content = "$" + Math.Round(Convert.ToDouble(dr[1]), 2).ToString();
                _tempUserControl.CtrlAdjType.Content = dr[2].ToString();
                _tempUserControl.CtrlReqDate.Content = dr[3].ToString();
                _tempUserControl.CtrlRequester.Content = dr[4].ToString();
                _tempUserControl.Height = 52;
                addStackPanelChild(_tempUserControl);
                //counts.Add(_tempUserControl);
                _tempUserControl = null;
            }
            _controlData.Clear();
            _dtAdjReq.Clear();

            //return counts;
        }

        private void addStackPanelChild(AdjStackControl _tempUserControl)
        {
            spReqPanel.Children.Add(_tempUserControl);
            spReqPanel.UpdateLayout();
        }

        private void ExportSheet()
        {
            if (dtReq == null)
            {
                System.Windows.Forms.MessageBox.Show("There is no request sheet available to export.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dtReq.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dtReq.Rows)
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
                }
            }
        }

        public object LoadAdj(string adjStatus)
        {
            Adjustments = new List<Adjustment>();
            dtReq = new DataTable();

            if (adjStatus == "Needs Review")
            {
                reqNum = txtRevReqNum.Text.Trim().ToUpper();
            }
            else if (adjStatus == "Review Complete")
            {
                reqNum = txtFinReqNum.Text.Trim().ToUpper();
            }

            if (reqNum == null || reqNum == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter a Request ID number to load a request.");
                return null;
            }
            var q = (from a in xps.Adjustments
                     where a.ReqID == reqNum && a.Status == adjStatus
                     select a).ToList();
            Adjustments = q;

            dtReq = new DataTable();
            dtReq.Columns.Add("Part Number");
            dtReq.Columns.Add("Sequence");
            dtReq.Columns.Add("Description");
            dtReq.Columns.Add("Adj QTY");
            dtReq.Columns.Add("Unit Cost");
            dtReq.Columns.Add("Ext Cost");
            dtReq.Columns.Add("Comments");
            dtReq.Columns.Add("Status");
            dtReq.Columns.Add("Req ID");

            foreach (Adjustment adj in Adjustments)
            {
                DataRow dr = dtReq.NewRow();
                dr[0] = adj.PID;
                dr[1] = adj.SEQ;
                dr[2] = adj.Desc;
                dr[3] = adj.Qty;
                dr[4] = adj.UnitCost;
                dr[5] = adj.NetValue;
                dr[6] = adj.Comments;
                dr[7] = adj.Status;
                dr[8] = adj.ReqID;
                dtReq.Rows.Add(dr);
            }
            dgAdjLoad.ItemsSource = dtReq.DefaultView;
            dgAdjLoad.Columns[7].Visibility = Visibility.Hidden;
            dgAdjLoad.Columns[8].Visibility = Visibility.Hidden;
            return dtReq;
        }

        private void SubmitAdj(string adjStatus, string adjStatusUpdate)
        {
            if (adjStatus == "Needs Review")
            {
                reqNum = txtRevReqNum.Text.Trim().ToUpper();
            }
            else if (adjStatus == "Review Complete")
            {
                reqNum = txtFinReqNum.Text.Trim().ToUpper();
            }

            #region Primary Validation

            try
            {
                if (reqNum == null || reqNum == "")
                {
                    System.Windows.Forms.MessageBox.Show("No count sheet was loaded.");
                    return;
                }
                for (int i = 0; i < dtReq.Rows.Count; i++)
                {
                    if (dtReq.Rows[i][7].ToString() != adjStatus)
                    {
                        System.Windows.Forms.MessageBox.Show("This is not a the correct request status to submit under this menu. Please make sure you if this is a request in Review or Completion status to use the proper menu to submit.");
                        return;
                    }
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("No count sheet was loaded.");
                return;
            }

            #endregion Primary Validation

            #region Request Entry Validation

            foreach (DataRow _checkRow in dtReq.Rows)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (_checkRow[col].ToString() == "")
                    {
                        System.Windows.Forms.MessageBox.Show("Not all required information was entered. Please make sure that all cells are not blank");
                        return;
                    }
                }
            }

            #endregion Request Entry Validation

            XpsDates xdates = new XpsDates();
            xdates.CheckDateEntry(dtpCompDate.SelectedDate.ToString(), DateTime.Today.Date.ToString());
            DateTime _compDate = xdates.ValidDate;

            for (int i = 0; i < dtReq.Rows.Count; i++)
            {
                DataRow dr = dtReq.Rows[i];
                double? _adjQty = Convert.ToDouble(dr[3]);
                double? _extCost = Convert.ToDouble(dr[5]);
                string _comments = dr[6].ToString();
                string _status = dr[7].ToString();
                string _reqID = dr[8].ToString();
                string _seq = dr[1].ToString();
                Adjustment req = (from r in xps.Adjustments
                                  where r.ReqID == _reqID && r.SEQ == _seq
                                  select r).SingleOrDefault();

                req.Qty = _adjQty;
                req.NetValue = _extCost;
                req.Comments = _comments;
                req.Status = adjStatusUpdate;
                if (adjStatusUpdate == "Complete")
                {
                    req.CompDate = _compDate;
                }
            }
            xps.SaveChanges();

            System.Windows.Forms.MessageBox.Show("Adjustment Request Saved");
            dgAdjLoad.ItemsSource = null;
            dtReq = null;
            txtRevReqNum.Text = "";
            txtFinReqNum.Text = "";
        }

        private void RejectSequence(string adjStatus)
        {
            string _rejSeq = string.Empty;
            string _reqID = string.Empty;
            if (adjStatus == "Needs Review")
            {
                _rejSeq = txtRevSeqReject.Text.Trim().ToUpper();
            }
            else if (adjStatus == "Review Complete")
            {
                _rejSeq = txtFinSeqReject.Text.Trim().ToUpper();
            }
            if (_rejSeq == null || _rejSeq == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter the sequence number you would like to reject");
                return;
            }

            try
            {
                for (int i = 0; i < dtReq.Rows.Count; i++)
                {
                    if (dtReq.Rows[i][1].ToString() == _rejSeq)
                    {
                        _reqID = dtReq.Rows[i][8].ToString();
                        dtReq.Rows.Remove(dtReq.Rows[i]);
                    }
                }
                var rejSeq = (from a in xps.Adjustments
                              where a.SEQ == _rejSeq && a.ReqID == _reqID
                              select a).First();
                rejSeq.Status = "Rejected";
                xps.SaveChanges();
                dgAdjLoad.ItemsSource = dtReq.DefaultView;
                dgAdjLoad.Columns[7].Visibility = Visibility.Hidden;
                dgAdjLoad.Columns[8].Visibility = Visibility.Hidden;
                System.Windows.Forms.MessageBox.Show("Seqence: " + _rejSeq + " has been set to 'Rejected' status");
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("No count sheet was loaded.");
                return;
            }
        }

        #endregion Adj Management Methods

        #region Reconciliation Methods

        private void FindBomChange()
        {
            string _pid = txtPidBomInquiry.Text.Trim().ToUpper();
            string _asm = string.Empty;

            if (_pid == null || _pid == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter a part number.");
                return;
            }

            DataTable dtBOM = new DataTable();
            dtBOM.Columns.Add("PID", typeof(string));
            dtBOM.Columns.Add("Desc", typeof(string));
            dtBOM.Columns.Add("Action", typeof(string));
            dtBOM.Columns.Add("Assembly", typeof(string));
            dtBOM.Columns.Add("AssemblyDesc", typeof(string));
            dtBOM.Columns.Add("DR", typeof(string));
            dtBOM.Columns.Add("Status", typeof(string));
            dtBOM.Columns.Add("MipDate", typeof(DateTime));
            dtBOM.Columns.Add("Comments", typeof(string));

            List<string> _asmList = new List<string>();
            var pidQ = (from p in xps.BomChanges
                        where p.ComponentItem == _pid
                        select p.RevisedItem).Distinct().ToList();
            _asmList = pidQ;

            foreach (string assy in _asmList)
            {
                List<BomChanx> _drList = new List<BomChanx>();
                var asmQ = (from a in xps.BomChanges
                            where a.RevisedItem == assy
                            select a).ToList();
                _drList = asmQ;
                foreach (BomChanx drItem in _drList)
                {
                    DataRow dr = dtBOM.NewRow();
                    dr[0] = drItem.ComponentItem;
                    dr[1] = drItem.ComponentDesc;
                    dr[2] = drItem.ComponentAction;
                    dr[3] = drItem.RevisedItem;
                    dr[4] = drItem.RevisedItemDesc;
                    dr[5] = drItem.DR;
                    dr[6] = drItem.RevisedItemStatus;
                    dr[7] = drItem.MIP;
                    dr[8] = drItem.remarks;
                    dtBOM.Rows.Add(dr);
                }
            }
            dgBomLoad.ItemsSource = dtBOM.DefaultView;
        }

        public object PasteConvert()
        {
            string _erpPaste = txtErpPaste.Text;
            StringReader sr;
            sr = new StringReader(_erpPaste);
            dtErpPaste = new DataTable();
            try
            {
                int _rc = 0;
                string[] _columnNames = null;
                string[] _streamDataValues = null;
                string _streamRowData;
                while ((_streamRowData = sr.ReadLine()) != null)
                {
                    _streamRowData = _streamRowData.Trim();
                    if (_streamRowData.Length > 0)
                    {
                        _streamRowData = _streamRowData.Replace(@"""", "");
                        _streamRowData = _streamRowData.Replace(@"..", "");
                        _streamDataValues = _streamRowData.Split('\t');
                        if (_rc == 0)
                        {
                            _rc = 1;
                            _columnNames = _streamDataValues;
                            foreach (string _header in _columnNames)
                            {
                                DataColumn col = new DataColumn(_header.ToUpper(), typeof(string));
                                col.DefaultValue = string.Empty;
                                dtErpPaste.Columns.Add(col);
                            }
                        }
                        else
                        {
                            DataRow dr = dtErpPaste.NewRow();
                            for (int i = 0; i < _streamDataValues.Length; i++)
                            {
                                dr[_columnNames[i]] = _streamDataValues[i] == null ?
                                    string.Empty : _streamDataValues[i].ToString();
                            }
                            dtErpPaste.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while converting text: " + ex.Message);
                return null;
            }
            finally
            {
                sr.Close();
                sr.Dispose();
            }
            for (int i = 0; i < dtErpPaste.Columns.Count; i++)
            {
                if (i < dtErpPaste.Columns.Count)
                {
                    DataColumn col = dtErpPaste.Columns[i];
                    string _colName = col.ColumnName.ToString().ToLower();

                    switch (_colName)
                    {
                        case "org": break;
                        case "sub": break;
                        case "locator": break;
                        case "item": break;
                        case "item description": break;
                        case "on-hand": break;
                        case "unpacked": break;
                        case "packed": break;
                        default:
                            dtErpPaste.Columns.Remove(col);
                            i--;
                            break;
                    }
                }
            }
            dtErpPaste.Columns.Add("Count", typeof(int));
            dtErpPaste.Columns.Add("System Variance", typeof(int));
            dtErpPaste.Columns.Add("Correction Needed", typeof(int));
            dtErpPaste.Columns.Add("Action", typeof(string));
            List<string> _locList = new List<string>();
            for (int i = 0; i < dtErpPaste.Rows.Count; i++)
            {
                string _loc = dtErpPaste.Rows[i][2].ToString();
                _locList.Add(_loc);
            }

            cboFromTrns.ItemsSource = _locList;
            cboToTrns.ItemsSource = _locList;
            cboFromAdj.ItemsSource = _locList;
            cboToAdj.ItemsSource = _locList;

            dgReconSheet.ItemsSource = dtErpPaste.DefaultView;
            return dtErpPaste;
        }

        private void SaveRecon()
        {
            if (dtErpPaste == null)
            {
                System.Windows.Forms.MessageBox.Show("There was no count sheet loaded.");
                return;
            }
            string _counter = txtCID.Text;
            if (_counter == null || _counter == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter your name in the Counter box before saving.");
                return;
            }
            foreach (DataRow dr in dtErpPaste.Rows)
            {
                dr[11] = string.Empty;
            }
            foreach (ReconStackControl ctrl in spReconSteps.Children)
            {
                string _type = ctrl.lblTrnType.Content.ToString();
                string _qty = ctrl.lblQty.Content.ToString();
                string _from = ctrl.lblFrom.Text;
                string _to = ctrl.lblTo.Text;
                string _action = string.Empty;
                if (_type == "Transfer")
                {
                    foreach (DataRow dr in dtErpPaste.Rows)
                    {
                        if (dr[2].ToString() == _from)
                        {
                            _action = _type + " " + _qty + " To " + _to + @" \ ";
                            dr[11] = dr[11].ToString() + _action;
                        }
                    }
                }
                if (_type == "Adjust")
                {
                    foreach (DataRow dr in dtErpPaste.Rows)
                    {
                        if (dr[2].ToString() == _from)
                        {
                            _action = _type + " " + _qty + @" \ ";
                            dr[11] = dr[11].ToString() + _action;
                        }
                        if (dr[2].ToString() == _to)
                        {
                            _action = _type + " +" + _qty + @" \ ";
                            dr[11] = dr[11].ToString() + _action;
                        }
                    }
                }
            }
            WorkBenchCount _entry;
            foreach (DataRow dr in dtErpPaste.Rows)
            {
                _entry = new WorkBenchCount();
                _entry.CountDate = DateTime.Today.Date;
                _entry.ORG = dr[0].ToString();
                _entry.Subinventory = dr[1].ToString();
                _entry.Locator = dr[2].ToString();
                _entry.PID = dr[3].ToString();
                _entry.Description = dr[4].ToString();
                _entry.SystemQTY = Convert.ToDouble(dr[5]);
                _entry.Unpacked = Convert.ToDouble(dr[6]);
                _entry.Packed = Convert.ToDouble(dr[7]);
                _entry.CountedQTY = Convert.ToDouble(dr[8]);
                _entry.SysVar = Convert.ToDouble(dr[9]);
                _entry.CorrAmnt = Convert.ToDouble(dr[10]);
                _entry.Action = dr[11].ToString();
                _entry.CounterName = _counter;
                xps.WorkBenchCounts.Add(_entry);
            }
            xps.SaveChanges();
        }

        private void ReconTransferSuggest()
        {
            string _loc = string.Empty;
            double _sysQty = 0;
            double _varQty = 0;
            double _corrQty = 0;
            double x = 0;
            double y = 0;
            int _transCount = 0;
            DataTable _dtLtZ = new DataTable();
            DataTable _dtGtZ = new DataTable();
            DataTable _dtSortLtZ = new DataTable();
            DataTable _dtSortGtZ = new DataTable();
            DataTable _dtTrX = new DataTable();
            DataView _dv;

            _dtLtZ.Columns.Add("locator", typeof(string));
            _dtLtZ.Columns.Add("SysQty", typeof(string));
            _dtLtZ.Columns.Add("CorrQty", typeof(double));

            _dtGtZ.Columns.Add("locator", typeof(string));
            _dtGtZ.Columns.Add("SysQty", typeof(string));
            _dtGtZ.Columns.Add("CorrQty", typeof(double));

            _dtTrX.Columns.Add("TrxID", typeof(string));
            _dtTrX.Columns.Add("TrxType", typeof(string));
            _dtTrX.Columns.Add("FromLoc", typeof(string));
            _dtTrX.Columns.Add("Qty", typeof(string));
            _dtTrX.Columns.Add("ToLoc", typeof(string));

            string _toLoc = string.Empty;
            string _fromLoc = string.Empty;

            for (int i = 0; i < dtErpPaste.Rows.Count; i++)
            {
                try
                {
                    if (Convert.ToDouble(dtErpPaste.Rows[i][9]) < 0)
                    {
                        DataRow dr = _dtLtZ.NewRow();
                        _loc = dtErpPaste.Rows[i][2].ToString();
                        _sysQty = Convert.ToDouble(dtErpPaste.Rows[i][5]);
                        _varQty = Convert.ToDouble(dtErpPaste.Rows[i][9]);
                        dr[0] = _loc;
                        dr[1] = _sysQty;
                        dr[2] = _varQty;
                        _dtLtZ.Rows.Add(dr);
                    }
                    else if (Convert.ToDouble(dtErpPaste.Rows[i][9]) > 0)
                    {
                        DataRow dr = _dtGtZ.NewRow();
                        _loc = dtErpPaste.Rows[i][2].ToString();
                        _sysQty = Convert.ToDouble(dtErpPaste.Rows[i][5]);
                        _varQty = Convert.ToDouble(dtErpPaste.Rows[i][9]);
                        dr[0] = _loc;
                        dr[1] = _sysQty;
                        dr[2] = _varQty;
                        _dtGtZ.Rows.Add(dr);
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Please make sure all counts are entered.");
                    return;
                }
            }

            if (_dtLtZ.Rows.Count > 0)
            {
                _dv = _dtLtZ.DefaultView;
                _dv.Sort = "CorrQty DESC";
                _dtSortLtZ = _dv.ToTable();
            }
            if (_dtGtZ.Rows.Count > 0)
            {
                _dv = _dtGtZ.DefaultView;
                _dv.Sort = "CorrQty ASC";
                _dtSortGtZ = _dv.ToTable();
            }

            if (_dtSortLtZ.Rows.Count > 0)
            {
                for (int i = 0; i < _dtSortLtZ.Rows.Count; i++)
                {
                    _loc = _dtSortLtZ.Rows[i][0].ToString();
                    x = Convert.ToDouble(_dtSortLtZ.Rows[i][2]);
                    if (_dtSortGtZ.Rows.Count > 0)
                    {
                        for (int j = 0; j < _dtSortGtZ.Rows.Count; j++)
                        {
                            y = Convert.ToDouble(_dtSortGtZ.Rows[j][2]);
                            if (y != 0)
                            {
                                if ((x + y) <= 0)
                                {
                                    if (_dtTrX.Rows.Count > 0)
                                    {
                                        _dtTrX.Rows.RemoveAt(0);
                                    }
                                    _transCount++;
                                    _fromLoc = _dtSortGtZ.Rows[j][0].ToString();
                                    _toLoc = _dtSortLtZ.Rows[i][0].ToString();

                                    DataRow dr = _dtTrX.NewRow();
                                    dr[0] = @"Transfer_" + _transCount;
                                    dr[1] = "Transfer";
                                    dr[2] = y;
                                    dr[3] = _fromLoc;
                                    dr[4] = _toLoc;
                                    _dtTrX.Rows.Add(dr);

                                    x += y;

                                    _dtSortLtZ.Rows[i][2] = x;
                                    _dtSortGtZ.Rows[j][2] = 0;
                                    AddTransfer(_dtTrX);
                                    foreach (DataRow erp in dtErpPaste.Rows)
                                    {
                                        if (erp[2].ToString() == _fromLoc)
                                        {
                                            erp[5] = Convert.ToDouble(erp[5]) - y;
                                        }
                                        else if (erp[2].ToString() == _toLoc)
                                        {
                                            erp[5] = Convert.ToDouble(erp[5]) + y;
                                        }
                                    }
                                }
                                else
                                {
                                    while (y >= 0 && (x + y) > 0)
                                    {
                                        y--;
                                    }
                                    if (_dtTrX.Rows.Count > 0)
                                    {
                                        _dtTrX.Rows.RemoveAt(0);
                                    }
                                    _transCount++;
                                    _fromLoc = _dtSortGtZ.Rows[j][0].ToString();
                                    _toLoc = _dtSortLtZ.Rows[i][0].ToString();

                                    DataRow dr = _dtTrX.NewRow();
                                    dr[0] = @"Transfer_" + _transCount;
                                    dr[1] = "Transfer";
                                    dr[2] = y;
                                    dr[3] = _fromLoc;
                                    dr[4] = _toLoc;
                                    _dtTrX.Rows.Add(dr);

                                    x += y;

                                    _dtSortLtZ.Rows[i][2] = x;
                                    _dtSortGtZ.Rows[j][2] = Convert.ToDouble(_dtSortGtZ.Rows[j][2]) - y;
                                    AddTransfer(_dtTrX);
                                    foreach (DataRow erp in dtErpPaste.Rows)
                                    {
                                        if (erp[2].ToString() == _fromLoc)
                                        {
                                            erp[5] = Convert.ToDouble(erp[5]) - y;
                                        }
                                        else if (erp[2].ToString() == _toLoc)
                                        {
                                            erp[5] = Convert.ToDouble(erp[5]) + y;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (DataRow dr in dtErpPaste.Rows)
                {
                    for (int i = 0; i < _dtSortLtZ.Rows.Count; i++)
                    {
                        if (dr[2] == _dtSortLtZ.Rows[i][0])
                        {
                            _corrQty = Convert.ToDouble(_dtSortLtZ.Rows[i][2]) * -1;

                            dr[10] = _corrQty;
                        }
                    }
                    for (int i = 0; i < _dtSortGtZ.Rows.Count; i++)
                    {
                        if (dr[2] == _dtSortGtZ.Rows[i][0])
                        {
                            _corrQty = Convert.ToDouble(_dtSortGtZ.Rows[i][2]) * -1;
                            dr[10] = _corrQty;
                        }
                    }
                }
                List<string> _trnsID = new List<string>();
                string _tID = string.Empty;
                string _type = string.Empty;
                foreach (ReconStackControl ctrl in spReconSteps.Children)
                {
                    _tID = ctrl.lblTrnId.Content.ToString();
                    _type = ctrl.lblTrnType.Content.ToString();
                    if (_type == "Transfer")
                    {
                        _trnsID.Add(_tID);
                    }
                }
                cboRemoveTrns.ItemsSource = _trnsID;
                dgReconSheet.ItemsSource = dtErpPaste.DefaultView;
            }
        }

        private void ReconAdjustSuggest()
        {
            string _loc = string.Empty;
            string _fromLoc = string.Empty;
            string _toLoc = string.Empty;
            double _corrQty = 0;
            int _transCount = 0;
            DataTable _dtTrX = new DataTable();
            _dtTrX.Columns.Add("TrxID", typeof(string));
            _dtTrX.Columns.Add("TrxType", typeof(string));
            _dtTrX.Columns.Add("Qty", typeof(double));
            _dtTrX.Columns.Add("FromLoc", typeof(string));
            _dtTrX.Columns.Add("ToLoc", typeof(string));

            for (int i = 0; i < dtErpPaste.Rows.Count; i++)
            {
                try
                {
                    if (Convert.ToDouble(dtErpPaste.Rows[i][10]) != 0)
                    {
                        if (_dtTrX.Rows.Count > 0)
                        {
                            _dtTrX.Rows.RemoveAt(0);
                        }
                        _transCount++;
                        _corrQty = Convert.ToDouble(dtErpPaste.Rows[i][10]);

                        if (_corrQty < 0)
                        {
                            _fromLoc = dtErpPaste.Rows[i][2].ToString();
                        }
                        else if (_corrQty > 0)
                        {
                            _toLoc = dtErpPaste.Rows[i][2].ToString();
                        }

                        DataRow dr = _dtTrX.NewRow();
                        dr[0] = @"Adjust_" + _transCount;
                        dr[1] = "Adjust";
                        dr[2] = _corrQty;
                        dr[3] = _fromLoc;
                        dr[4] = _toLoc;
                        _dtTrX.Rows.Add(dr);
                        AddTransfer(_dtTrX);
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Please make sure all counts are entered.");
                    return;
                }
            }
            List<string> _adjID = new List<string>();
            string _aID = string.Empty;
            string _type = string.Empty;
            foreach (ReconStackControl ctrl in spReconSteps.Children)
            {
                _aID = ctrl.lblTrnId.Content.ToString();
                _type = ctrl.lblTrnType.Content.ToString();
                if (_type == "Adjust")
                {
                    _adjID.Add(_aID);
                }
            }
            cboRemoveAdj.ItemsSource = _adjID;
        }

        private void AddTransfer(DataTable _dtTrX)
        {
            foreach (ReconStackControl rc in spReconSteps.Children)
            {
                if (rc.Name == _dtTrX.Rows[0][0].ToString())
                {
                    return;
                }
            }
            if (_dtTrX.Rows[0][2].ToString() != "0")
            {
                ReconStackControl _rcStack = new ReconStackControl();
                _rcStack.Name = _dtTrX.Rows[0][0].ToString();
                _rcStack.lblTrnId.Content = _dtTrX.Rows[0][0].ToString();
                _rcStack.lblTrnType.Content = _dtTrX.Rows[0][1].ToString();
                _rcStack.lblQty.Content = _dtTrX.Rows[0][2].ToString();
                _rcStack.lblFrom.Text = _dtTrX.Rows[0][3].ToString().Replace("..", "");
                _rcStack.lblTo.Text = _dtTrX.Rows[0][4].ToString().Replace("..", "");
                spReconSteps.Children.Add(_rcStack);
                //List<string> _rcItems = new List<string>();
                //foreach (ReconStackControl rc in spReconSteps.Children)
                //{
                //    _rcItems.Add(rc.Name);
                //}
                //cboRemoveTrns.ItemsSource = _rcItems;
            }
        }

        private void ExportRecon()
        {
            if (dtErpPaste == null)
            {
                System.Windows.Forms.MessageBox.Show("There was no count sheet loaded.");
                return;
            }
            foreach (DataRow dr in dtErpPaste.Rows)
            {
                dr[11] = string.Empty;
            }
            foreach (ReconStackControl ctrl in spReconSteps.Children)
            {
                string _type = ctrl.lblTrnType.Content.ToString();
                string _qty = ctrl.lblQty.Content.ToString();
                string _from = ctrl.lblFrom.Text;
                string _to = ctrl.lblTo.Text;
                string _action = string.Empty;
                if (_type == "Transfer")
                {
                    foreach (DataRow dr in dtErpPaste.Rows)
                    {
                        if (dr[2].ToString() == _from)
                        {
                            _action = _type + " " + _qty + " To " + _to + @" \ ";
                            dr[11] = dr[11].ToString() + _action;
                        }
                    }
                }
                if (_type == "Adjust")
                {
                    foreach (DataRow dr in dtErpPaste.Rows)
                    {
                        if (dr[2].ToString() == _from)
                        {
                            _action = _type + " " + _qty + @" \ ";
                            dr[11] = dr[11].ToString() + _action;
                        }
                        if (dr[2].ToString() == _to)
                        {
                            _action = _type + " +" + _qty + @" \ ";
                            dr[11] = dr[11].ToString() + _action;
                        }
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dtErpPaste.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dtErpPaste.Rows)
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
                }
            }
            dgReconSheet.ItemsSource = dtErpPaste.DefaultView;
        }

        private void RemoveTransfer()
        {
            string _selID = string.Empty;
            string _trnID = string.Empty;
            if (cboRemoveTrns.SelectedIndex > -1)
            {
                foreach (ReconStackControl ctrl in spReconSteps.Children)
                {
                    _selID = cboRemoveTrns.SelectedItem.ToString();
                    _trnID = ctrl.lblTrnId.Content.ToString();
                    if (_trnID == _selID)
                    {
                        spReconSteps.Children.Remove(ctrl);
                        return;
                    }
                }
            }
        }

        private void RemoveAdjustment()
        {
            string _selID = string.Empty;
            string _trnID = string.Empty;
            if (cboRemoveAdj.SelectedIndex > -1)
            {
                foreach (ReconStackControl ctrl in spReconSteps.Children)
                {
                    _selID = cboRemoveAdj.SelectedItem.ToString();
                    _trnID = ctrl.lblTrnId.Content.ToString();
                    if (_trnID == _selID)
                    {
                        spReconSteps.Children.Remove(ctrl);
                        return;
                    }
                }
            }
        }

        #endregion Reconciliation Methods

        #region ViewCounts Methods

        private void GetCounts()
        {
            DateTime _sDate = new DateTime();
            DateTime _eDate = new DateTime();

            List<WorkBenchCount> _counts = new List<WorkBenchCount>();

            if (dtpSdCountSheet.SelectedDate.ToString() != "")
            {
                try
                {
                    DateTime.TryParse(dtpSdCountSheet.SelectedDate.ToString(), out _sDate);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid From date.");
                    return;
                }
            }
            else
            {
                _sDate = Convert.ToDateTime("1/1/2000");
            }

            if (dtpEdCountSheet.SelectedDate.ToString() != "")
            {
                try
                {
                    DateTime.TryParse(dtpEdCountSheet.SelectedDate.ToString(), out _eDate);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid To date.");
                    return;
                }
            }
            else
            {
                _eDate = DateTime.Today.Date;
            }

            string _pid = txtPIDCountSheet.Text.Trim().ToUpper();
            string _counter = txtNameCountSheet.Text;

            if (_pid != null && _pid.ToString() == "")
            {
                _pid = null;
            }
            if (_counter != null && _counter.ToString() == "")
            {
                _counter = null;
            }

            if (_pid == null && _counter == null)
            {
                var q = (from a in xps.WorkBenchCounts
                         where a.CountDate >= _sDate && a.CountDate <= _eDate
                         select a).ToList();
                _counts = q;
            }
            else if (_pid == null && _counter != null)
            {
                var q = (from a in xps.WorkBenchCounts
                         where a.CountDate >= _sDate && a.CountDate <= _eDate && a.CounterName == _counter
                         select a).ToList();
                _counts = q;
            }
            else
            {
                var q = (from a in xps.WorkBenchCounts
                         where a.CountDate >= _sDate && a.CountDate <= _eDate && a.PID == _pid
                         select a).ToList();
                _counts = q;
            }
            dtCountSheet = new DataTable();
            dtCountSheet.Columns.Add("CountDate");
            dtCountSheet.Columns.Add("ORG");
            dtCountSheet.Columns.Add("Subinventory");
            dtCountSheet.Columns.Add("Locator");
            dtCountSheet.Columns.Add("Item");
            dtCountSheet.Columns.Add("Description");
            dtCountSheet.Columns.Add("Onhand");
            dtCountSheet.Columns.Add("Unpacked");
            dtCountSheet.Columns.Add("Packed");
            dtCountSheet.Columns.Add("Count");
            dtCountSheet.Columns.Add("System Variance");
            dtCountSheet.Columns.Add("Correction Amount");
            dtCountSheet.Columns.Add("Action");
            dtCountSheet.Columns.Add("Counter");
            foreach (WorkBenchCount cs in _counts)
            {
                DataRow dr = dtCountSheet.NewRow();
                dr[0] = Convert.ToDateTime(cs.CountDate).ToString("MM/dd/yyyy");
                dr[1] = cs.ORG;
                dr[2] = cs.Subinventory;
                dr[3] = cs.Locator;
                dr[4] = cs.PID;
                dr[5] = cs.Description;
                dr[6] = cs.SystemQTY;
                dr[7] = cs.Unpacked;
                dr[8] = cs.Packed;
                dr[9] = cs.CountedQTY;
                dr[10] = cs.SysVar;
                dr[11] = cs.CorrAmnt;
                dr[12] = cs.Action;
                dr[13] = cs.CounterName;
                dtCountSheet.Rows.Add(dr);
            }
            dgViewCounts.ItemsSource = dtCountSheet.DefaultView;
        }

        private void ExportCounts()
        {
            if (dtCountSheet == null)
            {
                System.Windows.Forms.MessageBox.Show("There was no count sheet loaded.");
            }

            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dtCountSheet.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dtCountSheet.Rows)
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
                }
            }
        }

        #endregion ViewCounts Methods

        #region Weekly Metrics

        private void WeeklyMetrics()
        {
            DataTable _dtWtd = new DataTable();
            DataTable _dtYtd = new DataTable();
            DateTime _sdate = new DateTime();
            DateTime _edate = new DateTime();

            _dtWtd.Columns.Add("WkCats");
            _dtWtd.Columns.Add("WkActual");
            _dtWtd.Columns.Add("WkPlan");
            _dtWtd.Columns.Add("WkValueCount");
            _dtWtd.Columns.Add("WkValueAdj");
            _dtWtd.Columns.Add("WkValueAbs");

            _dtYtd.Columns.Add("YrCats");
            _dtYtd.Columns.Add("YrActual");
            _dtYtd.Columns.Add("YrPlan");
            _dtYtd.Columns.Add("YrValueCount");
            _dtYtd.Columns.Add("YrValueAdj");
            _dtYtd.Columns.Add("YrValueAbs");

            if (dtpStartWkRep.SelectedDate.ToString() != "")
            {
                try
                {
                    DateTime.TryParse(dtpStartWkRep.SelectedDate.ToString(), out _sdate);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid start date entered.");
                    return;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please enter a start date");
                return;
            }

            if (dtpEndWkRep.SelectedDate.ToString() != "")
            {
                try
                {
                    DateTime.TryParse(dtpEndWkRep.SelectedDate.ToString(), out _edate);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid start date entered.");
                    return;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please enter an end date");
                return;
            }

            List<MetricsAdj> adj = new List<MetricsAdj>();
            List<MetricsInv> gc = new List<MetricsInv>();

            try
            {
                gc = (from a in xps.MetricsInvs
                      where a.Date >= _sdate && a.Date <= _edate
                      select a).ToList();

                adj = (from a in xps.MetricsAdjs
                       where a.Date >= _sdate && a.Date <= _edate
                       select a).ToList();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to retrive weekly adjustment metrics: " + ex.Message);
            }

            for (int i = 0; i < 6; i++)
            {
                _dtWtd.Rows.Add();
                _dtYtd.Rows.Add();
            }

            _dtWtd.Rows[0][0] = "GC";
            _dtYtd.Rows[0][0] = "GC";

            _dtWtd.Rows[1][0] = "Adhoc";
            _dtYtd.Rows[1][0] = "Adhoc";

            _dtWtd.Rows[2][0] = "FSL";
            _dtYtd.Rows[2][0] = "FSL";

            _dtWtd.Rows[3][0] = "MISC";
            _dtYtd.Rows[3][0] = "MISC";

            _dtWtd.Rows[4][0] = "Negatives";
            _dtYtd.Rows[4][0] = "Negatives";

            _dtWtd.Rows[5][0] = "Returns";
            _dtYtd.Rows[5][0] = "Returns";

            double? _loc = gc.Sum(c => c.TotalLocations);
            double? _valCnt = gc.Sum(c => c.TotalValue);

            double? _valAdj = 0;
            double? _valAbs = 0;

            _dtWtd.Rows[0][1] = _loc;
            _dtWtd.Rows[0][2] = _loc;
            _dtWtd.Rows[0][3] = _valCnt;

            foreach (DataRow dr in _dtWtd.Rows)
            {
                string _type = dr[0].ToString();
                foreach (var a in adj)
                {
                    if (a.Type.Trim().ToUpper() == _type.ToUpper())
                    {
                        _valAdj += a.AdjValue;
                        _valAbs += a.AbsValue;
                    }
                }
                dr[4] = _valAdj;
                dr[5] = _valAbs;
                _valAbs = 0;
                _valAdj = 0;
            }

            List<MetricsAdj> ytdAdj = new List<MetricsAdj>();
            List<MetricsInv> ytdGc = new List<MetricsInv>();

            try
            {
                int _currYear = _sdate.Year;
                ytdGc = (from a in xps.MetricsInvs
                         where a.CountYear == _currYear
                         select a).ToList();

                DateTime _sAdjYr = Convert.ToDateTime("1/1/" + _currYear);
                //DateTime _eAdjYr = Convert.ToDateTime("12/31/" + _currYear);

                ytdAdj = (from a in xps.MetricsAdjs
                          where a.Date >= _sAdjYr && a.Date <= _edate
                          select a).ToList();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to retrieve YTD adjustment metrics: " + ex.Message);
            }

            _loc = ytdGc.Sum(c => c.TotalLocations);
            _valCnt = ytdGc.Sum(c => c.TotalValue);

            _dtYtd.Rows[0][1] = _loc;
            _dtYtd.Rows[0][2] = _loc;
            _dtYtd.Rows[0][3] = _valCnt;

            foreach (DataRow dr in _dtYtd.Rows)
            {
                string _type = dr[0].ToString();
                foreach (var a in ytdAdj)
                {
                    if (a.Type.Trim().ToUpper() == _type.ToUpper())
                    {
                        _valAdj += a.AdjValue;
                        _valAbs += a.AbsValue;
                    }
                }
                dr[4] = _valAdj;
                dr[5] = _valAbs;
                _valAbs = 0;
                _valAdj = 0;
            }

            dgWtdResults.ItemsSource = _dtWtd.DefaultView;
            dgYtdResults.ItemsSource = _dtYtd.DefaultView;
        }

        #endregion Weekly Metrics

        #endregion Methods

        #region Special Events

        private void dgAdjLoad_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtReq != null)
                {
                    for (int row = 0; row < dtReq.Rows.Count; row++)
                    {
                        DataRow dr = dtReq.Rows[row];
                        double _countQty = Convert.ToDouble(dr[3]);
                        double _unitCost = Convert.ToDouble(dr[4]);
                        dr[5] = _countQty * _unitCost;
                    }
                    dgAdjLoad.ItemsSource = dtReq.DefaultView;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error calculating values: " + ex);
                return;
            }
        }

        private void dgReconSheet_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtErpPaste != null)
                {
                    List<string> _locList = new List<string>();
                    for (int row = 0; row < dtErpPaste.Rows.Count; row++)
                    {
                        DataRow dr = dtErpPaste.Rows[row];
                        if (dr[8].ToString() != string.Empty && dr[5].ToString() != string.Empty)
                        {
                            double _countQty = Convert.ToDouble(dr[8]);
                            double _sysQty = Convert.ToDouble(dr[5]);
                            dr[9] = _sysQty - _countQty;
                            dr[10] = _countQty - _sysQty;
                            dgReconSheet.ItemsSource = dtErpPaste.DefaultView;
                        }
                        string _loc = dtErpPaste.Rows[row][2].ToString();
                        _locList.Add(_loc);
                    }
                    cboFromTrns.ItemsSource = _locList;
                    cboToTrns.ItemsSource = _locList;
                    cboFromAdj.ItemsSource = _locList;
                    cboToAdj.ItemsSource = _locList;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error calculating values: " + ex);
                return;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtpSdAdjtracker.SelectedDate = DateTime.Today.Date;
            dtpEdAdjtracker.SelectedDate = DateTime.Today.Date;
            foreach (StackPanel sp in grdMain.Children)
            {
                if (sp.Name == "spAdjTracker" || sp.Name == "spMainMenu")
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
                if (grd.Name == "grdDataGrid")
                {
                    grd.Visibility = Visibility.Visible;
                }
                else
                {
                    grd.Visibility = Visibility.Hidden;
                }
            }

            TestConnection();
        }

        #endregion Special Events
    }
}