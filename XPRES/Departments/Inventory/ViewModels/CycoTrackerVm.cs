using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Helpers;

namespace XPRES.Departments.Inventory.ViewModels
{
    public class CycoTrackerVm : ViewModelBase
    {
        #region Constructor
        public CycoTrackerVm()
        {
            _trackerCollection = new ObservableCollection<CCTracker>();
        }

        #endregion Constructor

        #region Properties

        private ObservableCollection<CCTracker> _trackerCollection;

        public ObservableCollection<CCTracker> TrackerCollection
        {
            get { return _trackerCollection; }
            set
            {
                _trackerCollection = value;
                OnPropertyChanged();
            }
        }

        private string _sdate;

        public string SDate
        {
            get { return _sdate; }
            set
            {
                _sdate = value;
                OnPropertyChanged();
            }
        }

        private string _edate;

        public string EDate
        {
            get { return _edate; }
            set
            {
                _edate = value;
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

        private string _pid;

        public string Pid
        {
            get { return _pid; }
            set
            {
                _pid = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        public ICommand GetCountsCommand { get { return new RelayCommand(x => GetCounts()); } }

        public ICommand ClearTrackerCommand { get { return new RelayCommand(x => ClearTracker()); } }

        public ICommand ExportTrackerCommand { get { return new RelayCommand(x => ExportTracker()); } }

        #endregion ICommand Members

        #region CCTracker Methods

        private void ExportTracker()
        {
            if (!_trackerCollection.Any()) return;
            StringBuilder _sb = new StringBuilder();
            
            IEnumerable<string> _columnNames = new List<string>
            {
                nameof(CCTracker.CountDate),
                nameof(CCTracker.ORG),
                nameof(CCTracker.Subinventory),
                nameof(CCTracker.Locator),
                nameof(CCTracker.PID),
                nameof(CCTracker.Description),
                nameof(CCTracker.SystemQTY),
                nameof(CCTracker.CountedQTY),
                nameof(CCTracker.Difference),
                nameof(CCTracker.Action),
                nameof(CCTracker.Error),
                nameof(CCTracker.FirstCount),
                nameof(CCTracker.SecondCount),
                nameof(CCTracker.Zone),
                nameof(CCTracker.UnitCost),
                nameof(CCTracker.CountStatus),
                nameof(CCTracker.CountID),
            };
            _sb.AppendLine(string.Join(",", _columnNames));
            foreach (CCTracker _line in _trackerCollection)
            {
                string _rowData = string.Join
                    (
                        ",",_line.CountDate.ToString(),
                        _line.ORG,
                        _line.Subinventory,
                        _line.Locator,
                        _line.PID,
                        _line.Description.Replace(@",", " "),
                        _line.SystemQTY,
                        _line.CountedQTY,
                        _line.Difference,
                        _line.Action,
                        _line.Error,
                        _line.FirstCount,
                        _line.SecondCount,
                        _line.Zone,
                        _line.UnitCost,
                        _line.CountStatus,
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
        }

        private void GetCounts()
        {
            if (_trackerCollection.Any()) _trackerCollection.Clear();

            XpsDates _xdate = new XpsDates();
            _xdate.CheckDateEntry(_sdate, @"1/1/2012");
            DateTime _sd = _xdate.ValidDate;
            _xdate.CheckDateEntry(_edate, DateTime.Now.Date.ToShortDateString());
            DateTime _ed = _xdate.ValidDate;

            int _cid = 0;

            if (!string.IsNullOrEmpty(_countId))
            {
                try
                {
                    _cid = Convert.ToInt32(_countId.Trim());
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show(@"Invalid Count ID.");
                    return;
                }
            }

            try
            {
                IQueryable<CCTracker> _countQuery = (from _a in new XpresEntities().CCTrackers
                                   where _a.CountDate >= _sd && _a.CountDate <= _ed
                                   select _a);

                if (!_countQuery.Any())
                {
                    System.Windows.Forms.MessageBox.Show(@"No results for this date range found.");
                    return;
                }

                if (string.IsNullOrEmpty(_pid) && _cid != 0)
                {
                    _countQuery = _countQuery.Where(a => a.CountID == _cid);
                }
                else if (!string.IsNullOrEmpty(_pid) && _cid == 0)
                {
                    _countQuery = _countQuery.Where(a => a.PID == _pid);
                }
                else if (!string.IsNullOrEmpty(_pid) && _cid != 0)
                {
                    _countQuery = _countQuery.Where(a => a.PID == _pid && a.CountID == _cid);
                }
                
                
                foreach (CCTracker _cs in _countQuery)
                {
                    _trackerCollection.Add(_cs);
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while trying to retrieve cycle count data: " + _ex.Message);
            }
        }

        private void ClearTracker()
        {
            if (_trackerCollection.Any()) _trackerCollection.Clear();
        }

        #endregion CCTracker Methods
    }
}