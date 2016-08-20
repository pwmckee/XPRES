using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Departments.Inventory.Views.Controls;
using XPRES.Helpers;

namespace XPRES.Departments.Inventory.ViewModels
{
    public class CycoMgmtVm : ViewModelBase
    {
        #region Constructor

        public CycoMgmtVm()
        {
            _cycoCtrlCollection = new ObservableCollection<CycoStackControl>();
            _cycoSheetCollection = new BindingList<CCTracker>();
        }

        #endregion Constructor

        #region Properties

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

        private BindingList<CCTracker> _cycoSheetCollection;

        public BindingList<CCTracker> CycoSheetCollection
        {
            get { return _cycoSheetCollection; }
            set
            {
                _cycoSheetCollection = value;
                OnPropertyChanged();
            }
        }

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

        private string _counter;

        public string Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
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

        #region ICommandMembers

        public ICommand FirstCountsCommand { get { return new RelayCommand(x => GetOpenCounts("NewCreate")); } }

        public ICommand SecondCountsCommand { get { return new RelayCommand(x => GetOpenCounts("SecondReady")); } }

        public ICommand LoadCountCommand { get { return new RelayCommand(x => LoadCount()); } }

        public ICommand ExportCountCommand { get { return new RelayCommand(x => ExportCount()); } }

        public ICommand SubmitCountCommand { get { return new RelayCommand(x => SubmitCount()); } }

        public ICommand AddWriteInCommand { get { return new RelayCommand(x => AddWriteIn()); } }

        #endregion ICommandMembers

        #region Methods

        private void GetOpenCounts(string countStatus)
        {
            if (_cycoCtrlCollection.Any()) _cycoCtrlCollection.Clear();

            try
            {
                CycoStackControl _tempUserControl = new CycoStackControl();

                var _countInfo = (from _a in new XpresEntities().CCTrackers
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
                        _tempUserControl = new CycoStackControl
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

        private void LoadCount()
        {
            if (_cycoSheetCollection.Any()) _cycoSheetCollection.Clear();
            if (string.IsNullOrEmpty(_countId))
            {
                System.Windows.Forms.MessageBox.Show(@"No count ID was entered.");
                return;
            }

            int? _id;

            try
            {
                _id = Convert.ToInt32(_countId.Trim());
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(@"Invalid count ID entered.");
                return;
            }

            try
            {
                var _countQuery = (from _a in new XpresEntities().CCTrackers
                                   where _a.CountID == _id && _a.CountStatus != "Complete"
                                   select _a);

                if (!_countQuery.Any())
                {
                    System.Windows.Forms.MessageBox.Show(@"No records found using that count ID");
                    return;
                }

                foreach (CCTracker _cc in _countQuery)
                {
                    _cc.CountedQTY = null;
                    _cycoSheetCollection.Add(_cc);
                }
                _countId = "";
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving count sheet: " + _ex.Message);
            }
        }

        private void ExportCount()
        {
            if (!_cycoSheetCollection.Any()) return;
            if (string.IsNullOrEmpty(_counter))
            {
                System.Windows.Forms.MessageBox.Show(@"Please enter the name of the person performing the count.");
                return;
            }
            int? _id = 0;
            string _status = "";
            XpresEntities _xps = new XpresEntities();

            foreach (CCTracker _cc in _cycoSheetCollection)
            {
                if (_id == 0) _id = _cc.CountID;
                if (string.IsNullOrEmpty(_status)) _status = _cc.CountStatus;
                switch (_cc.CountStatus)
                {
                    case "NewCreate":
                        _cc.FirstCount = _counter;
                        break;

                    case "SecondReady":
                        _cc.SecondCount = _counter;
                        break;

                    default:
                        System.Windows.Forms.MessageBox.Show(@"This count is not ready for First or Second count.  Please make sure it has been reviewed and submitted by the department lead.");
                        return;
                }

                try
                {
                    CCTracker _countUpdate = (from _a in _xps.CCTrackers
                                              where _a.ID == _cc.ID
                                              select _a).SingleOrDefault();
                    if (_status == "NewCreate")
                    {
                        _countUpdate.FirstCount = _counter;
                    }
                    else
                    {
                        _countUpdate.SecondCount = _counter;
                    }
                }
                catch (Exception _ex)
                {
                    System.Windows.Forms.MessageBox.Show(@"Error updating count record: " + _ex.Message);
                    return;
                }
            }

            try
            {
                var _schedUpdate = (from _a in _xps.CountSchedules
                                    where _a.CountID == _id
                                    select _a);
                foreach (CountSchedule _cs in _schedUpdate)
                {
                    if (_status == "NewCreate")
                    {
                        _cs.FirstCount = _counter;
                    }
                    else
                    {
                        _cs.SecondCount = _counter;
                    }
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error updating count schedule: " + _ex.Message);
                return;
            }

            _xps.SaveChanges();

            StringBuilder _sb = new StringBuilder();

            IEnumerable<string> _columnNames = new List<string>
            {
                nameof(CCTracker.CountDate),
                nameof(CCTracker.ORG),
                nameof(CCTracker.Subinventory),
                nameof(CCTracker.Locator),
                nameof(CCTracker.PID),
                nameof(CCTracker.Description),
                nameof(CCTracker.CountedQTY),
                nameof(CCTracker.FirstCount),
                nameof(CCTracker.SecondCount),
                nameof(CCTracker.CountID),
            };
            _sb.AppendLine(string.Join(",", _columnNames));
            foreach (CCTracker _line in _cycoSheetCollection)
            {
                string _rowData = string.Join
                    (
                        ",", _line.CountDate.ToString(),
                        _line.ORG,
                        _line.Subinventory,
                        _line.Locator,
                        _line.PID,
                        _line.Description.Replace(@",", " "),
                        _line.CountedQTY,
                        _line.FirstCount,
                        _line.SecondCount,
                        _line.CountID
                    );
                _sb.AppendLine(_rowData);
            }

            Microsoft.Win32.SaveFileDialog _exportFile = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = "csv",
                RestoreDirectory = true
            };

            if (_exportFile.ShowDialog() == true)
            {
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
            _cycoSheetCollection.Clear();
            _counter = "";
        }

        private void AddWriteIn()
        {
            if (!CycoSheetCollection.Any())
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
            double? _count = Convert.ToDouble(_writeInCnt);
            string _desc = "";

            try
            {
                _count = Convert.ToDouble(_writeInCnt);
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

            foreach (CCTracker _cc in _cycoSheetCollection)
            {
                if (_cc.Locator.ToUpper() == _writeInLoc.ToUpper().Trim() && _cc.PID.ToUpper() == _writeInPid.ToUpper().Trim())
                {
                    System.Windows.Forms.MessageBox.Show(@"This item and location is already on this count sheet");
                    return;
                }
                if (_date == null)
                {
                    _date = _cc.CountDate;
                    _org = _cc.ORG;
                    _sub = _cc.Subinventory;
                    _first = _cc.FirstCount;
                    _second = _cc.SecondCount;
                    _zone = _cc.Zone;
                    _status = _cc.CountStatus;
                    _id = _cc.CountID;
                }
            }

            try
            {
                CCTracker _writeIn = new CCTracker
                {
                    CountDate = _date,
                    ORG = _org,
                    Subinventory = _sub,
                    Locator = _writeInLoc,
                    PID = _writeInPid,
                    Description = _desc,
                    SystemQTY = 0,
                    CountedQTY = _count,
                    Zone = _zone,
                    CountStatus = _status,
                    CountID = _id
                };

                _writeIn.Difference = _writeIn.CountedQTY - _writeIn.SystemQTY;

                if (_status == "NewCreate")
                {
                    _writeIn.FirstCount = _first;
                }
                else
                {
                    _writeIn.SecondCount = _second;
                }
                _cycoSheetCollection.Add(_writeIn);
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error adding write-in count:" + _ex.Message);
            }
        }

        private void SubmitCount()
        {
            if (!_cycoSheetCollection.Any()) return;

            foreach (CCTracker _test in _cycoSheetCollection)
            {
                _countId = _test.CountID.ToString().Trim();
                if ((_test.CountStatus == "NewCreate" && string.IsNullOrEmpty(_test.FirstCount)) ||
                    (_test.CountStatus == "SecondReady" && string.IsNullOrEmpty(_test.SecondCount)))
                {
                    System.Windows.Forms.MessageBox.Show(@"Counts must be exported first before they can be submitted.");
                    return;
                }
                if (_test.CountedQTY == null || _test.PID == null || _test.Locator == null)
                {
                    System.Windows.Forms.MessageBox.Show(@"Please make sure all count information is entered.");
                    return;
                }
            }

            try
            {
                XpresEntities _xps = new XpresEntities();
                int? _id = Convert.ToInt32(_countId);
                var _countQuery = (from _a in _xps.CCTrackers
                    where _a.CountID == _id
                    select _a);
                foreach (CCTracker _cc in _cycoSheetCollection)
                {
                    _cc.Difference = _cc.CountedQTY - _cc.SystemQTY;
                    if (_cc.Difference == 0) _cc.CountStatus = "Complete";
                    if (_cc.CountStatus == "NewCreate")
                    {
                        _cc.CountStatus = "FirstSubmitReview";
                    }
                    else if (_cc.CountStatus == "SecondReady")
                    {
                        _cc.CountStatus = "SecondSubmitReview";
                    }
                    if (_cc.ID == 0)
                    {
                        _xps.CCTrackers.Add(_cc);
                    }
                    else
                    {
                        CCTracker _record = _countQuery.SingleOrDefault(x => x.ID == _cc.ID);
                        _record.CountedQTY = _cc.CountedQTY;
                        _record.Difference = _cc.Difference;
                        _record.CountStatus = _cc.CountStatus;
                    }
                }
                _xps.SaveChanges();
                _cycoSheetCollection.Clear();
                _countId = "";
                System.Windows.Forms.MessageBox.Show(@"Count successfully saved!");
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error submitting count: " + _ex.Message);
            }
        }

        #endregion Methods
    }
}