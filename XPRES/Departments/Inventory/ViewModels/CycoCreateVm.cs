using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Helpers;

namespace XPRES.Departments.Inventory.ViewModels
{
    public class CycoCreateVm : ViewModelBase
    {
        #region Constructor

        public CycoCreateVm()
        {
            _yearList = new ObservableCollection<string>();
            _qtrList = new ObservableCollection<string>();
            _areaList = new ObservableCollection<string>();
            _zoneList = new ObservableCollection<string>();
            _rangeCollection = new ObservableCollection<CountSchedule>();
            _schedCollection = new ObservableCollection<CountSchedule>();
            _cycoCollection = new ObservableCollection<CCTracker>();
            FillCountYears();
        }

        #endregion Constructor
        
        #region Properties

        #region CountSchedule Properties

        private ObservableCollection<CountSchedule> _schedCollection;

        public ObservableCollection<CountSchedule> SchedCollection
        {
            get { return _schedCollection; }
            set
            {
                _schedCollection = value;
                OnPropertyChanged();
            }
        }

        private DataTable _dtCountRange;

        public DataTable DtCountRange
        {
            get { return _dtCountRange; }
            set
            {
                _dtCountRange = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _yearList;

        public ObservableCollection<string> YearList
        {
            get { return _yearList; }
            set
            {
                _yearList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _qtrList;

        public ObservableCollection<string> QtrList
        {
            get { return _qtrList; }
            set
            {
                _qtrList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _areaList;

        public ObservableCollection<string> AreaList
        {
            get { return _areaList; }
            set
            {
                _areaList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _zoneList;

        public ObservableCollection<string> ZoneList
        {
            get { return _zoneList; }
            set
            {
                _zoneList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CountSchedule> _rangeCollection;

        public ObservableCollection<CountSchedule> RangeCollection
        {
            get { return _rangeCollection; }
            set
            {
                _rangeCollection = value;
                OnPropertyChanged();
            }
        }

        private string _selYear;

        public string SelYear
        {
            get { return _selYear; }
            set
            {
                _selYear = value;
                OnPropertyChanged();
                FillQuarters();
            }
        }

        private string _selQtr;

        public string SelQtr
        {
            get { return _selQtr; }
            set
            {
                _selQtr = value;
                OnPropertyChanged();
                FillSchedCollection();
                FillCountAreas();
            }
        }

        private string _selArea;

        public string SelArea
        {
            get { return _selArea; }
            set
            {
                _selArea = value;
                OnPropertyChanged();
                FillCountRange();
                FillZones();
            }
        }

        private string _selZone;

        public string SelZone
        {
            get { return _selZone; }
            set
            {
                _selZone = value;
                OnPropertyChanged();
            }
        }

        private string _selRange;

        public string SelRange
        {
            get { return _selRange; }
            set
            {
                _selRange = value;
                OnPropertyChanged();
            }
        }

        #endregion CountSchedule Properties

        #region CreateCount Properties

        private ObservableCollection<CCTracker> _cycoCollection;

        public ObservableCollection<CCTracker> CycoCollection
        {
            get { return _cycoCollection; }
            set
            {
                _cycoCollection = value;
                OnPropertyChanged();
            }
        }

        private string _countFrom;

        public string CountFrom
        {
            get { return _countFrom; }
            set
            {
                _countFrom = value;
                OnPropertyChanged();
            }
        }

        private string _countTo;

        public string CountTo
        {
            get { return _countTo; }
            set
            {
                _countTo = value;
                OnPropertyChanged();
            }
        }

        #endregion CreateCount Properties

        #endregion Properties

        #region ICommand Members

        public ICommand CreateCycoCommand { get { return new RelayCommand(x => NewCreate());} }

        public ICommand SaveCycoCommand { get { return new RelayCommand(x => SaveNewCountSheet()); } }

        #endregion ICommand Members

        #region Methods

        #region CountSchedule Methods

        private void FillCountYears()
        {
            try
            {
                List<string> _q = (from _a in new XpresEntities().CountSchedules
                                   orderby _a.GoalYear where _a.CountStatus != "Complete"
                                   select _a.GoalYear).Distinct().ToList();
                foreach (var _item in _q)
                {
                    _yearList.Add(_item);
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while connecting to the database: " + _ex);
            }
        }

        private void FillQuarters()
        {
            try
            {
                int _dt = DateTime.Now.Month;
                int _currQtr = (_dt - 1) / 3 + 1;
                if (_qtrList.Any()) _qtrList.Clear();
                List<int?> _q = (from _a in new XpresEntities().CountSchedules
                                 where _a.GoalYear == _selYear && _a.CountStatus == null || _a.CountStatus == ""
                                 select _a.GoalQuarter).Distinct().ToList();
                foreach (int? _item in _q)
                {
                    _qtrList.Add(_item.ToString());
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while connecting to the database: " + _ex);
            }
        }

        private void FillCountAreas()
        {
            try
            {
                int? _qtr = Convert.ToInt32(_selQtr);
                if (_areaList.Any()) _areaList.Clear();
                List<string> _q = (from _a in new XpresEntities().CountSchedules
                                   where _a.GoalYear == _selYear && _a.GoalQuarter == _qtr && _a.CountStatus == null || _a.CountStatus == ""
                                   orderby _a.Zone
                                   select _a.CountArea).Distinct().ToList();
                foreach (var _item in _q)
                {
                    _areaList.Add(_item);
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while connecting to the database: " + _ex);
            }
        }

        private void FillZones()
        {
            try
            {
                if (_zoneList.Any()) _zoneList.Clear();
                List<int?> _q = (from _a in new XpresEntities().CountSchedules
                                 where _a.CountArea == _selArea && _a.CountStatus == null || _a.CountStatus == ""
                                 select _a.Zone).Distinct().ToList();
                foreach (int? _item in _q)
                {
                    _zoneList.Add(_item.ToString());
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while connecting to the database: " + _ex);
            }
        }

        private void FillCountRange()
        {
            try
            {
                if (_rangeCollection.Any()) _rangeCollection.Clear();
                var _countRangeList = (from _a in new XpresEntities().CountSchedules
                                       where _a.CountArea == _selArea && _a.CountStatus == null || _a.CountStatus == ""
                                       select _a);
                foreach (CountSchedule _cs in _countRangeList)
                {
                    _rangeCollection.Add(_cs);
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while connecting to the database: " + _ex);
            }
        }

        private void FillSchedCollection()
        {
            int? _qtr = Convert.ToInt32(_selQtr);
            if (_schedCollection.Any()) _schedCollection.Clear();
            var _schedList = (from _a in new XpresEntities().CountSchedules
                              where _a.GoalQuarter == _qtr && _a.CountStatus == null || _a.CountStatus == ""
                              select _a);
            foreach (var _item in _schedList)
            {
                _schedCollection.Add(_item);
            }
        }

        #endregion CountSchedule Methods

        #region CreateCount Methods

        private void NewCreate()
        {
            DataTable _dtDelim = new DataTable();
            string _path = "";
            Microsoft.Win32.OpenFileDialog _selectFile = new Microsoft.Win32.OpenFileDialog();
            if (_selectFile.ShowDialog() == true)
            {
                _path = _selectFile.FileName;
                System.Windows.Forms.MessageBox.Show(@"File: " + _path + @" successfuly loaded");
            }

            //Convert CSV Onhand to DataTable _dtDelim
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
                string[] _columnNames = null;
                while (!_sr.EndOfStream)
                {
                    string _streamRowData = _sr.ReadLine().Trim();
                    if (_streamRowData.Length > 0)
                    {
                        _streamRowData = _streamRowData.Replace(@"""", "");
                        string[] _streamDataValues = _streamRowData.Split(',');
                        if (_rowCount == 0)
                        {
                            _rowCount = 1;
                            _columnNames = _streamDataValues;
                            foreach (string _csvHeader in _columnNames)
                            {
                                DataColumn _dataColumn = new DataColumn(_csvHeader.ToUpper(), typeof(string))
                                {
                                    DefaultValue = string.Empty
                                };
                                _dtDelim.Columns.Add(_dataColumn);
                            }
                        }
                        else
                        {
                            DataRow _dr = _dtDelim.NewRow();
                            for (int _i = 0; _i < _columnNames.Length; _i++)
                            {
                                _dr[_columnNames[_i]] = _streamDataValues[_i] == null ?
                                    string.Empty : _streamDataValues[_i].ToString();
                            }
                            _dtDelim.Rows.Add(_dr);
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while deliminating CSV file: " + _path + @": " + _ex.Message);
                return;
            }
            finally
            {
                _sr.Close();
                _sr.Dispose();
            }

            //Remove unnecessary columns carried over from CSV File
            try
            {
                for (int _i = 1; _i <= 6; _i++)
                {
                    _dtDelim.Columns.RemoveAt(5);
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(@"Error while creating table.  Please make sure the proper type of ERP count data was loaded.");
                return;
            }
            //Consolidate line items
            for (int _i = 0; _i < _dtDelim.Rows.Count - 1; _i++)
            {
                if (_dtDelim.Rows[_i][2].ToString() == _dtDelim.Rows[_i + 1][2].ToString() & _dtDelim.Rows[_i][3].ToString() == _dtDelim.Rows[_i + 1][3].ToString())
                {
                    _dtDelim.Rows[_i][4] = Convert.ToSingle(_dtDelim.Rows[_i][4]) + Convert.ToSingle(_dtDelim.Rows[_i + 1][4]);
                    _dtDelim.Rows[_i + 1].Delete();
                    _i--;
                }
            }

            _dtDelim.Columns.Add("Description");
            _dtDelim.Columns.Add("UnitCost");

            for (int _i = 0; _i < _dtDelim.Rows.Count; _i++)
            {
                DateTime _date = DateTime.Now.Date;
                string _org = _dtDelim.Rows[_i][0].ToString();
                string _sub = _dtDelim.Rows[_i][1].ToString();
                string _loc = _dtDelim.Rows[_i][2].ToString();
                string _pid = _dtDelim.Rows[_i][3].ToString();
                string _desc = "";
                double _sys = Convert.ToDouble(_dtDelim.Rows[_i][4]);
                string _countStatus = "NewCreate";
                double? _cost = 0;

                try
                {
                    UnitCost _uc = (from _a in new XpresEntities().UnitCosts
                                    where _a.Item == _pid
                                    select _a).First();
                    _desc = _uc.Description;
                    _cost = _uc.Unit_Cost;
                }
                catch
                {
                    System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                        @"No item cost or description found for " + _pid + @". Would you like to add " +
                        @"it to the database with a cost of $0.00?",
                        @"Item not found", System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (_diag == System.Windows.Forms.DialogResult.Yes)
                    {
                        UnitCost _uc = new UnitCost
                        {
                            Item = _pid,
                            Description = "No Description",
                            Unit_Cost = 0
                        };
                        XpresEntities _xps = new XpresEntities();
                        _xps.UnitCosts.Add(_uc);
                        _xps.SaveChanges();
                        var _q = (from _a in _xps.UnitCosts
                                  where _a.Item == _pid
                                  select _a).First();
                        _desc = _q.Description;
                        _cost = _q.Unit_Cost;
                    }
                    else if (_diag == System.Windows.Forms.DialogResult.No)
                    {
                        System.Windows.Forms.MessageBox.Show(@"If you need to add this item to the database, please ask a lead or supervisor.");
                        return;
                    }
                }

                CCTracker _cc = new CCTracker
                {
                    CountDate = _date,
                    ORG = _org,
                    Subinventory = _sub,
                    Locator = _loc,
                    PID = _pid,
                    Description = _desc,
                    SystemQTY = _sys,
                    CountStatus = _countStatus,
                    UnitCost = _cost
                };
                _cycoCollection.Add(_cc);
            }
        }

        private void SaveNewCountSheet()
        {
            int? _selectedZone = Convert.ToInt32(_selZone);
            if (_selectedZone == 0)
            {
                System.Windows.Forms.MessageBox.Show(@"Please select which zone this count is for.");
                return;
            }

            if (string.IsNullOrEmpty(_selQtr))
            {
                System.Windows.Forms.MessageBox.Show(@"Please select a quarter from the dropdown list.");
                return;
            }

            if (string.IsNullOrEmpty(_countFrom) || string.IsNullOrEmpty(_countTo) )
            {
                System.Windows.Forms.MessageBox.Show(@"Please enter both the from and to count range.");
                return;
            }

            string _cFrom = _countFrom.Trim();
            string _cTo = _countTo.Trim();
            int? _idFrom;
            int? _idTo;
            int? _qtr = Convert.ToInt32(_selQtr);
            int? _year = Convert.ToInt32(_selYear);

            try
            {
                _idFrom = (from _a in new XpresEntities().CountSchedules
                    where _a.CountRange == _cFrom && _a.GoalQuarter == _qtr
                    select _a.ID).First();
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(@"Invalid count reange entered.");
                return;
            }

            CountSchedule _to = new CountSchedule();

            try
            {
                _idTo = (from _a in new XpresEntities().CountSchedules
                          where _a.CountRange == _cTo && _a.GoalQuarter == _qtr
                          select _a.ID).First();
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(@"Invalid count reange entered.");
                return;
            }

            int _countId = int.Parse(_year + _qtr.ToString() + _idTo);
            XpresEntities _xps = new XpresEntities();
            for (int? _i = _idFrom; _i <= _idTo; _i++)
            {
                CountSchedule _cs;
                
                try
                {
                    _cs = (from _a in _xps.CountSchedules
                          where _a.ID == _i
                          select _a).SingleOrDefault();
                }
                catch (Exception _ex)
                {
                    System.Windows.Forms.MessageBox.Show(@"Error while trying to retrieve count schedule info: " + _ex.Message);
                    return;
                }

                if (_cs?.CountID != null)
                {
                    System.Windows.Forms.MessageBox.Show(@"One or more locations in this count have already had a count ID created for them.");
                    return;
                }
                if (_cs != null)
                {
                    _cs.ActualDate = DateTime.Now;
                    _cs.CountStatus = "Created";
                    _cs.CountID = _countId;
                }
                _xps.SaveChanges();
            }
            try
            {
                foreach (CCTracker _ccTracker in _cycoCollection)
                {
                    _ccTracker.Zone = _selectedZone;
                    _ccTracker.CountID = _countId;
                    _xps.CCTrackers.Add(_ccTracker);
                }
                _xps.SaveChanges();
                System.Windows.Forms.MessageBox.Show(@"Count Sheet Saved.");
                _cycoCollection.Clear();
                _schedCollection.Clear();
                _rangeCollection.Clear();
                _countFrom = "";
                _countTo = "";
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while saving count sheet: " + _ex.Message);
                return;
            }
        }

        #endregion CreateCount Methods

        #endregion Methods
    }
}