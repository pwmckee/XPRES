using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;

namespace XPRES.Departments.Inventory.Adjustments.Controls
{
    public partial class AdjMetricsControl : UserControl
    {
        private XpresEntities xps;
        private DataTable dtErpPaste;

        public AdjMetricsControl()
        {
            InitializeComponent();
            xps = new XpresEntities();
            dtErpPaste = new DataTable();
            FillTypes();
        }

        #region Button Events

        private void btnViewGrid_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdMain.Children)
            {
                if (grd.Name != "grdMaadReport")
                {
                    grd.Visibility = Visibility.Hidden;
                }
                else
                {
                    grd.Visibility = Visibility.Visible;
                }
            }
        }

        private void btnViewPaste_Click(object sender, RoutedEventArgs e)
        {
            foreach (Grid grd in grdMain.Children)
            {
                if (grd.Name != "grdMaadInput")
                {
                    grd.Visibility = Visibility.Hidden;
                }
                else
                {
                    grd.Visibility = Visibility.Visible;
                }
            }
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            PasteConvert();
        }

        private void btnFillCodes_Click(object sender, RoutedEventArgs e)
        {
            FillCodes();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            ViewMetrics();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveMetrics();
        }

        #endregion Button Events

        #region Methods

        #region GridView

        private void FillTypes()
        {
            xps = new XpresEntities();
            try
            {
                var q = (from a in xps.MetricsAdjs
                         select a.Type).Distinct().ToList();
                List<string> _types = new List<string>();
                _types = q;
                cboAdjType.ItemsSource = _types;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error trying to get adjustment type list: " + ex.Message);
            }
        }

        public object FillCodes()
        {
            if (dtErpPaste == null || dtErpPaste.Rows.Count < 1)
            {
                System.Windows.Forms.MessageBox.Show("There was no data table available to save");
                return null;
            }

            DateTime _date = new DateTime();
            string _pid = string.Empty;
            double _qty = 0;

            foreach (DataRow dr in dtErpPaste.Rows)
            {
                try
                {
                    _date = Convert.ToDateTime(dr[0]);
                    _pid = dr[1].ToString();
                    _qty = Convert.ToDouble(dr[6]);
                    var adj = (from a in xps.Adjustments
                               where a.CompDate == _date && a.PID == _pid && a.Qty == _qty
                               select a.Type).First();
                    dr[9] = adj;
                }
                catch
                {
                    dr[9] = "Not Found";
                }
            }

            System.Windows.Forms.MessageBox.Show("Adjustment type codes successfully retrieved");
            dgMaadReport.ItemsSource = dtErpPaste.DefaultView;
            return dtErpPaste;
        }

        private void ViewMetrics()
        {
            DateTime _start = new DateTime();
            DateTime _end = new DateTime();

            if (dtpStart.SelectedDate.ToString() == "")
            {
                _start = Convert.ToDateTime("1/1/2001");
            }
            else
            {
                try
                {
                    DateTime.TryParse(dtpStart.SelectedDate.ToString(), out _start);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid start date entered.");
                }
            }

            if (dtpEnd.SelectedDate.ToString() == "")
            {
                _end = DateTime.Now.Date.AddDays(1);
            }
            else
            {
                try
                {
                    DateTime.TryParse(dtpEnd.SelectedDate.ToString(), out _end);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid end date entered.");
                }
            }

            DataTable dtViewMetric = new DataTable();
            dtViewMetric.Columns.Add("ID");
            dtViewMetric.Columns.Add("Txn Date");
            dtViewMetric.Columns.Add("Item");
            dtViewMetric.Columns.Add("Description");
            dtViewMetric.Columns.Add("Source");
            dtViewMetric.Columns.Add("Subinv");
            dtViewMetric.Columns.Add("UnitCost");
            dtViewMetric.Columns.Add("AdjQTY");
            dtViewMetric.Columns.Add("AdjValue");
            dtViewMetric.Columns.Add("ABSValue");
            dtViewMetric.Columns.Add("Type");
            DataRow dr;

            try
            {
                xps = new XpresEntities();
                List<MetricsAdj> _metrics = new List<MetricsAdj>();

                var metrics = (from a in xps.MetricsAdjs
                               where a.Date >= _start && a.Date <= _end
                               select a);

                if (cboAdjType.SelectedIndex > -1)
                {
                    string _type = cboAdjType.SelectedItem.ToString();
                    metrics = metrics.Where(a => a.Type == _type);
                    _metrics = metrics.ToList();
                }
                else
                {
                    _metrics = metrics.ToList();
                }

                foreach (var m in _metrics)
                {
                    dr = dtViewMetric.NewRow();
                    dr[0] = m.ID;
                    dr[1] = m.Date;
                    dr[2] = m.PID;
                    dr[3] = m.Description;
                    dr[4] = m.Source;
                    dr[5] = m.Sub;
                    dr[6] = m.UnitCost;
                    dr[7] = m.AdjQty;
                    dr[8] = m.AdjValue;
                    dr[9] = m.AbsValue;
                    dr[10] = m.Type;
                    dtViewMetric.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to retrieve adjustment metrics: " + ex.Message);
            }

            dgMaadReport.ItemsSource = dtViewMetric.DefaultView;
        }

        private void SaveMetrics()
        {
            if (dtErpPaste == null || dtErpPaste.Rows.Count < 1)
            {
                System.Windows.Forms.MessageBox.Show("There was no data table available to save");
                return;
            }

            string _checkType = string.Empty;

            foreach (DataRow dr in dtErpPaste.Rows)
            {
                _checkType = dr[9].ToString().ToLower();

                if (_checkType == "" || _checkType == "not found")
                {
                    System.Windows.Forms.MessageBox.Show("Adjustment Type Codes were not completed.  Please make sure there are no blanks or entries marked as 'Not Found'");
                    return;
                }
            }

            xps = new XpresEntities();

            MetricsAdj m;
            DateTime _date = new DateTime();
            string _pid = string.Empty;
            string _desc = string.Empty;
            string _src = string.Empty;
            string _sub = string.Empty;
            string _type = string.Empty;
            double _uc = 0;
            double _qty = 0;
            double _adjValue = 0;
            double _absValue = 0;

            try
            {
                foreach (DataRow dr in dtErpPaste.Rows)
                {
                    m = new MetricsAdj();
                    _date = Convert.ToDateTime(dr[0]);
                    _pid = dr[1].ToString();
                    _desc = dr[2].ToString();
                    _src = dr[3].ToString();
                    _sub = dr[4].ToString();
                    _uc = Convert.ToDouble(dr[5]);
                    _qty = Convert.ToDouble(dr[6]);
                    _adjValue = Convert.ToDouble(dr[7]);
                    _absValue = Convert.ToDouble(dr[8]);
                    _type = dr[9].ToString();

                    m.Date = _date;
                    m.PID = _pid;
                    m.Description = _desc;
                    m.Source = _src;
                    m.Sub = _sub;
                    m.UnitCost = _uc;
                    m.AdjQty = _qty;
                    m.AdjValue = _adjValue;
                    m.AbsValue = _absValue;
                    m.Type = _type;
                    xps.MetricsAdjs.Add(m);
                }
                xps.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error trying to save record: " + ex.Message);
                return;
            }
            FillTypes();
            System.Windows.Forms.MessageBox.Show("Record saved successfully");
            dtErpPaste.Clear();
            dgMaadReport.ItemsSource = null;
        }

        #endregion GridView

        #region Paste

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
                        case "txn date": break;
                        case "item": break;
                        case "description": break;
                        case "source": break;
                        case "subinv": break;
                        case "unitcost": break;
                        case "adjqty": break;
                        case "adjvalue": break;
                        case "absvalue": break;
                        default:
                            dtErpPaste.Columns.Remove(col);
                            i--;
                            break;
                    }
                }
            }

            for (int i = 0; i < dtErpPaste.Rows.Count - 1; i++)
            {
                DataRow dr = dtErpPaste.Rows[i];
                DataRow cr = dtErpPaste.Rows[i + 1];
                if (dr[0].ToString() == cr[0].ToString() && dr[1].ToString() == cr[1].ToString() && dr[4].ToString() == cr[4].ToString() && dr[6].ToString() == cr[6].ToString())
                {
                    dr[6] = Convert.ToSingle(dr[6]) + Convert.ToSingle(cr[6]);
                    dr[7] = Convert.ToSingle(dr[7]) + Convert.ToSingle(cr[7]);
                    dr[8] = Convert.ToSingle(dr[8]) + Convert.ToSingle(cr[8]);
                    cr.Delete();
                    i--;
                }
            }
            dtErpPaste.Columns.Add("Type", typeof(string));
            dgMaadReport.ItemsSource = dtErpPaste.DefaultView;
            foreach (Grid grd in grdMain.Children)
            {
                if (grd.Name != "grdMaadReport")
                {
                    grd.Visibility = Visibility.Hidden;
                }
                else
                {
                    grd.Visibility = Visibility.Visible;
                }
            }
            txtErpPaste.Clear();
            return dtErpPaste;
        }

        #endregion Paste

        #endregion Methods
        
    }
}