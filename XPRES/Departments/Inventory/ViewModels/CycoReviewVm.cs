using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using XPRES.Helpers;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Departments.Inventory.Views.Controls;

namespace XPRES.Departments.Inventory.ViewModels
{
    public class CycoReviewVm : ViewModelBase
    {
        #region Constructor

        public CycoReviewVm()
        {
            _countSheet = new DataTable
            {
                Columns = {"Count Date","Org","Sub","Locator","Item","Description","Onhand","Count","Difference",
                            "Action","Error","First Counter","Second Counter","Zone","Unit Cost","Status","Count ID", "ID"}
            };
            _countView = _countSheet.DefaultView;
            _cycoCtrlCollection = new ObservableCollection<CycoStackControl>();
        }

        #endregion Constructor

        #region Properties

        private string _countId;

        public string CountId
        {
            get { return _countId; }
            set
            {
                _countId = value;
                OnPropertyChanged();
            }
        }

        private readonly DataTable _countSheet;

        private DataView _countView;

        public DataView CountView
        {
            get { return _countView; }
            set
            {
                _countView = value;
                OnPropertyChanged();
            }
        }

        

        private ObservableCollection<CycoStackControl> _cycoCtrlCollection;

        public ObservableCollection<CycoStackControl> CycoCtrlCollection
        {
            get { return _cycoCtrlCollection; }
            set
            {
                _cycoCtrlCollection = value;
                OnPropertyChanged();
            }
        }

        private string _writeInLoc;

        public string WriteInLoc
        {
            get { return _writeInLoc; }
            set
            {
                _writeInLoc = value;
                OnPropertyChanged();
            }
        }

        private string _writeInPid;

        public string WriteInPid
        {
            get { return _writeInPid; }
            set
            {
                _writeInPid = value;
                OnPropertyChanged();
            }
        }

        private string _writeInCnt;

        public string WriteInCnt
        {
            get { return _writeInCnt; }
            set
            {
                _writeInCnt = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        public ICommand GetFirstReviewStackCommand => new RelayCommand(x => GetOpenCounts("FirstSubmitReview"));

        public ICommand GetSecondReviewStackCommand => new RelayCommand(x => GetOpenCounts("SecondSubmitReview"));

        public ICommand LoadReviewCommand => new RelayCommand(x => LoadReview());

        public ICommand SubmitReviewCommand => new RelayCommand(x => ReviewSubmit());

        public ICommand ExportCommand => new RelayCommand(x => ExportLocal());

        public ICommand ImportCommand => new RelayCommand(x => ImportLocal());

        public ICommand AddWriteInCommand => new RelayCommand(x => AddWriteIn());

        #endregion ICommand Members

        #region Methods

        private void GetOpenCounts(string countStatus)
        {
            if (_cycoCtrlCollection.Any()) _cycoCtrlCollection.Clear();

            try
            {
                List<CCTracker> _countInfo = (from _a in new XpresEntities().CCTrackers
                                              where _a.CountStatus == countStatus
                                              select _a).ToList();

                foreach (CCTracker _cc in _countInfo)
                {
                    bool _idCheck = false;
                    if (_cycoCtrlCollection.Any())
                    {
                        foreach (CycoStackControl _ctrl in _cycoCtrlCollection)
                        {
                            if (_ctrl.CtrlID.Text == _cc.CountID.ToString())
                            {
                                _idCheck = true;
                            }
                        }
                    }

                    if (!_idCheck)
                    {
                        CycoStackControl _tempUserControl = new CycoStackControl
                        {
                            CtrlID = { Text = _cc.CountID.ToString() },
                            CtrlDate = { Content = _cc.CountDate.ToString() },
                            CtrlZone = { Content = _cc.Zone.ToString() },
                            CtrlStatus = { Content = _cc.CountStatus },
                            CtrlFirst = { Content = _cc.FirstCount },
                            CtrlSecond = { Content = _cc.SecondCount },
                            Height = 52
                        };
                        _cycoCtrlCollection.Add(_tempUserControl);
                    }
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving count list: " + _ex.Message);
            }
        }

        private void LoadReview()
        {
            if (_countSheet.Rows.Count > 0) _countSheet.Clear();

            int? _id = 0;
            try
            {
                _id = Convert.ToInt32(_countId);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(@"Invalid Count ID Entered.");
            }

            try
            {
                var _countQuery = (from _a in new XpresEntities().CCTrackers
                                   where _a.CountID == _id && _a.CountStatus != "Complete"
                                   select _a);

                foreach (CCTracker _cc in _countQuery)
                {
                    DataRow _dr = _countSheet.NewRow();
                    _dr[0] = _cc.CountDate;
                    _dr[1] = _cc.ORG;
                    _dr[2] = _cc.Subinventory;
                    _dr[3] = _cc.Locator;
                    _dr[4] = _cc.PID;
                    _dr[5] = _cc.Description;
                    _dr[6] = _cc.SystemQTY;
                    _dr[7] = _cc.CountedQTY;
                    _dr[8] = _cc.Difference;
                    _dr[9] = _cc.Action;
                    _dr[10] = _cc.Error;
                    _dr[11] = _cc.FirstCount;
                    _dr[12] = _cc.SecondCount;
                    _dr[13] = _cc.Zone;
                    _dr[14] = _cc.UnitCost;
                    _dr[15] = _cc.CountStatus;
                    _dr[16] = _cc.CountID;
                    _dr[17] = _cc.ID;
                    _countSheet.Rows.Add(_dr);
                }
                _countView = _countSheet.DefaultView;
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving count: " + _ex.Message);
            }
        }

        private void ReviewSubmit()
        {
            if (_countSheet.Rows.Count < 1) return;
            string _countType = _countSheet.Rows[0][15].ToString();
            string _countTypeUpdate = "";
            int? _id = Convert.ToInt32(_countSheet.Rows[0][16]);

            try
            {
                XpresEntities _xps = new XpresEntities();
                var _countQuery = (from _a in _xps.CCTrackers
                                   where _a.CountID == _id
                                   select _a);

                foreach (DataRow _dr in _countSheet.Rows)
                {
                    if (_dr[6].ToString() == "" || _dr[7].ToString() == "" || _dr[8].ToString() == "")
                    {
                        System.Windows.Forms.MessageBox.Show(@"Not all counts were entered. Please enter all counts without any blank entries");
                        return;
                    }
                    if (_countType == "SecondSubmitReview" && _dr[8].ToString() != "0")
                    {
                        if (_dr[9].ToString() == "" || _dr[10].ToString() == "")
                        {
                            System.Windows.Forms.MessageBox.Show(@"Please make sure the Action and Error column are filled in for any counts that needed to be reconciled.");
                            return;
                        }
                    }

                    if (Convert.ToInt32(_dr[8].ToString()) != 0)
                    {
                        _countTypeUpdate = _countType == "FirstSubmitReview" ? "SecondReady" : "Complete";
                    }
                    else
                    {
                        _countTypeUpdate = "Complete";
                    }
                    

                    if (_dr[17].ToString() == "")
                    {
                        CCTracker _newLine = new CCTracker
                        {
                            CountDate = Convert.ToDateTime(_dr[0]),
                            ORG = _dr[1].ToString(),
                            Subinventory = _dr[2].ToString(),
                            Locator = _dr[3].ToString(),
                            PID = _dr[4].ToString(),
                            Description = _dr[5].ToString(),
                            CountedQTY = Convert.ToDouble(_dr[6].ToString()),
                            SystemQTY = Convert.ToDouble(_dr[7].ToString()),
                            Difference = Convert.ToDouble(_dr[8].ToString()),
                            Action = _dr[9].ToString(),
                            Error = _dr[10].ToString(),
                            FirstCount = _dr[11].ToString(),
                            SecondCount = _dr[12].ToString(),
                            Zone = Convert.ToInt32(_dr[13].ToString()),
                            UnitCost = Convert.ToDouble(_dr[14].ToString()),
                            CountStatus = _countTypeUpdate,
                            CountID = Convert.ToInt32(_dr[16].ToString()),
                        };
                        _xps.CCTrackers.Add(_newLine);
                    }
                    else
                    {
                        int _pkId = Convert.ToInt32(_dr[17]);
                        CCTracker _cc = _countQuery.SingleOrDefault(x => x.ID == _pkId);
                        _cc.Locator = _dr[3].ToString();
                        _cc.PID = _dr[4].ToString();
                        _cc.SystemQTY = Convert.ToDouble(_dr[6]);
                        _cc.CountedQTY = Convert.ToDouble(_dr[7]);
                        _cc.Difference = Convert.ToDouble(_dr[8]);
                        _cc.Action = _dr[9].ToString();
                        _cc.Error = _dr[10].ToString();
                        _cc.CountStatus = _countTypeUpdate;
                    }
                }
                _xps.SaveChanges();
                _countSheet.Clear();
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error saving count review: " + _ex.Message);
            }
            System.Windows.Forms.MessageBox.Show(@"Count Saved");
        }

        private void ExportLocal()
        {
            if (_countSheet.Rows.Count < 1) return;

            StringBuilder _sb = new StringBuilder();
            IEnumerable<string> _columnNames = _countSheet.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            _sb.AppendLine(string.Join(",", _columnNames));

            foreach (DataRow _row in _countSheet.Rows)
            {
                IEnumerable<string> _fields = _row.ItemArray.Select(field =>
                string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                _sb.AppendLine(string.Join(",", _fields));
            }

            Microsoft.Win32.SaveFileDialog _exportFile = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = "csv",
                RestoreDirectory = true
            };

            if (_exportFile.ShowDialog() != true) return;
            string _name = _exportFile.FileName;
            try
            {
                File.WriteAllText(_name, _sb.ToString());
                System.Windows.Forms.MessageBox.Show(_name + @" saved succefully");
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Could not save file: " + _ex.Message);
            }
        }

        private void ImportLocal()
        {
            if (_countSheet.Rows.Count > 0) _countSheet.Clear();
            string _path = "";
            Microsoft.Win32.OpenFileDialog _selectFile = new Microsoft.Win32.OpenFileDialog();
            if (_selectFile.ShowDialog() == true)
            {
                _path = _selectFile.FileName;
                System.Windows.Forms.MessageBox.Show(@"File: " + _path + @" successfuly loaded");
            }

            StreamReader _sr;
            try
            {
                _sr = new StreamReader(_path);
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while converting CSV file: " + _ex.Message);
                return;
            }

            try
            {
                int _rowCount = 0;
                string[] _streamDataValues = null;
                while (!_sr.EndOfStream)
                {
                    string _streamRowData = _sr.ReadLine().Trim();
                    if (_streamRowData.Length > 0)
                    {
                        _streamRowData = _streamRowData.Replace(@"""", "");
                        _streamRowData = _streamRowData.Replace(@", ", @"\");
                        _streamDataValues = _streamRowData.Split(',');
                        if (_rowCount == 0)
                        {
                            _rowCount = 1;
                        }
                        else
                        {
                            DataRow _dr = _countSheet.NewRow();
                            for (int _i = 0; _i < _countSheet.Columns.Count; _i++)
                                _dr[_i] = _streamDataValues[_i] ?? string.Empty;
                            _countSheet.Rows.Add(_dr);
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while deliminating CSV file: " + _path + ": " + _ex.Message);
            }
            finally
            {
                _sr.Close();
                _sr.Dispose();
            }
        }

        private void AddWriteIn()
        {
            if (_countSheet.Rows.Count < 1)
            {
                System.Windows.Forms.MessageBox.Show(@"No count sheet is loaded.");
                return;
            }

            if (string.IsNullOrEmpty(_writeInLoc) || string.IsNullOrEmpty(_writeInPid) || string.IsNullOrEmpty(_writeInCnt))
            {
                System.Windows.Forms.MessageBox.Show(@"Please make sure the Location, Item Number, and Count are entered.");
                return;
            }

            double? _cost = 0;
            double? _count;
            string _desc = "";

            try
            {
                _count = Convert.ToDouble(_writeInCnt.Trim());
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(@"Invalid count entered.");
                return;
            }

            try
            {
                UnitCost _uc = (from _a in new XpresEntities().UnitCosts
                                where _a.Item == _writeInPid
                                select _a).First();
                _desc = _uc.Description;
                _cost = _uc.Unit_Cost;
            }
            catch
            {
                System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                    @"No item cost or description found for " + _writeInPid + @". Would you like to add " +
                    @"it to the database with a cost of $0.00?",
                    @"Item not found", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (_diag == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        UnitCost _uc = new UnitCost
                        {
                            Item = _writeInPid,
                            Description = "No Description",
                            Unit_Cost = 0
                        };
                        XpresEntities _xps = new XpresEntities();
                        _xps.UnitCosts.Add(_uc);
                        _xps.SaveChanges();
                        var _q = (from _a in _xps.UnitCosts
                                  where _a.Item == _writeInPid
                                  select _a).First();
                        _desc = _q.Description;
                        _cost = _q.Unit_Cost;
                    }
                    catch (Exception _ex)
                    {
                        System.Windows.Forms.MessageBox.Show(@"Error adding new cost record:" + _ex.Message);
                        return;
                    }
                }
                else if (_diag == System.Windows.Forms.DialogResult.No)
                {
                    System.Windows.Forms.MessageBox.Show(@"If you need to add this item to the database, please ask a lead or supervisor.");
                    return;
                }
            }

            DateTime? _date = null;
            string _org = "";
            string _sub = "";
            string _first = "";
            string _second = "";
            int? _zone = null;
            string _status = "";
            int? _id = null;

            foreach (DataRow _dr in _countSheet.Rows)
            {
                if (_dr[3].ToString().ToUpper() == _writeInLoc.ToUpper().Trim() && _dr[4].ToString().ToUpper() == _writeInPid.ToUpper().Trim())
                {
                    System.Windows.Forms.MessageBox.Show(@"This item and location is already on this count sheet");
                    return;
                }
                if (_date == null)
                {
                    _date = Convert.ToDateTime(_dr[0].ToString());
                    _org = _dr[1].ToString();
                    _sub = _dr[2].ToString();
                    _first = _dr[11].ToString();
                    _second = _dr[12].ToString();
                    _zone = Convert.ToInt32(_dr[13].ToString());
                    _status = _dr[15].ToString();
                    _id =Convert.ToInt32(_dr[16].ToString());
                }
            }

            try
            {
                DataRow _dataRow = _countSheet.NewRow();

                _dataRow[0] = _date;
                _dataRow[1] = _org;
                _dataRow[2] = _sub;
                _dataRow[3] = _writeInLoc.Trim();
                _dataRow[4] = _writeInPid.Trim();
                _dataRow[5] = _desc;
                _dataRow[6] = 0;
                _dataRow[7] = _count;
                _dataRow[8] = _count;
                _dataRow[11] = _first;
                _dataRow[12] = _second;
                _dataRow[13] = _zone;
                _dataRow[14] = _cost;
                _dataRow[15] = _status;
                _dataRow[16] = _id;

                _writeInCnt = "";
                _writeInLoc = "";
                _writeInPid = "";
                _countSheet.Rows.Add(_dataRow);
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error adding write-in count:" + _ex.Message);
            }
        }

        public void CalcDif()
        {
            foreach (DataRow _dr in _countSheet.Rows)
            {
                try
                {
                    if (_dr[7].ToString() != "")
                        _dr[8] = Convert.ToDouble(_dr[7]) - Convert.ToDouble(_dr[6]);
                }
                catch{ /*ignore*/ }
            }
        }

        #endregion Methods
    }
}