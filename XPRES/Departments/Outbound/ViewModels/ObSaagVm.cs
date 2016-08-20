using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Departments.Outbound.Controls;
using XPRES.Helpers;

namespace XPRES.Departments.Outbound.ViewModels
{
    public class ObSaagVm : ViewModelBase
    {
        #region Constructor

        public ObSaagVm()
        {
            _mainPickCtrls = new ObservableCollection<PickStackControl>();
            _compPicksCtrls = new ObservableCollection<CompStackControl>();
            _multiPickCtrls = new ObservableCollection<MultiAddControl>();
            _pickerList = new List<string>();
            PickStackTimer = new DispatcherTimer { Interval = new TimeSpan(0,0,3) };
            PickStackTimer.Tick += new EventHandler(PickStackTimer_Tick);
            PickStackTimer.Start();
            RefreshPicks();
        }

        private void PickStackTimer_Tick(object sender, EventArgs e)
        {
            RefreshPicks();
        }

        #endregion Constructor

        #region Properties

        public DispatcherTimer PickStackTimer;

        private int? _multiPickNum;

        public int? MultiPickNum
        {
            get { return _multiPickNum; }
            set
            {
                _multiPickNum = value;
                OnPropertyChanged();
            }
        }

        private string _commandScan;

        public string CommandScan
        {
            get { return _commandScan; }
            set
            {
                _commandScan = value.Trim();
                OnPropertyChanged();
            }
        }

        private DateTime _expTime;

        private ObservableCollection<PickStackControl> _mainPickCtrls;

        public ObservableCollection<PickStackControl> MainPickCtrls
        {
            get { return _mainPickCtrls; }
            set
            {
                _mainPickCtrls = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CompStackControl> _compPicksCtrls;

        public ObservableCollection<CompStackControl> CompPickCtrls
        {
            get { return _compPicksCtrls; }
            set
            {
                _compPicksCtrls = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MultiAddControl> _multiPickCtrls;

        public ObservableCollection<MultiAddControl> MultiPickCtrls
        {
            get { return _multiPickCtrls; }
            set
            {
                _multiPickCtrls = value;
                OnPropertyChanged();
            }
        }

        private List<string> _pickerList;

        public List<string> PickerList
        {
            get { return _pickerList; }
            set
            {
                _pickerList = value;
                OnPropertyChanged();
            }
        }

        private int? _delId;

        public int? DelId
        {
            get { return _delId; }
            set
            {
                _delId = value;
                OnPropertyChanged();
            }
        }

        private int? _lineCount;

        public int? LineCount
        {
            get { return _lineCount; }
            set
            {
                _lineCount = value;
                OnPropertyChanged();
            }
        }

        private string _picker;

        public string Picker
        {
            get { return _picker; }
            set
            {
                _picker = value;
                OnPropertyChanged();
            }
        }

        private int? _lph;

        public int? Lph
        {
            get { return _lph; }
            set
            {
                _lph = value;
                OnPropertyChanged();
            }
        }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        public ICommand RefreshListCommand => new RelayCommand(x => RefreshPicks());

        public ICommand StartSinglePickCommand => new RelayCommand(x => StartSinglePick());

        public ICommand AddMultiPickCommand => new RelayCommand(x => AddMultiPick());

        public ICommand StartMultiPickCommand => new RelayCommand(x => StartMultiPick());

        public ICommand CompletePickCommand => new RelayCommand(x => CompletePick());

        #endregion ICommand Members

        #region Methods

        
        private void RefreshPicks()
        {
            GetPicks();
            GetCompletePicks();
            PickStackTimer.Start();
        }

        private void GetPicks()
        {
            if (_mainPickCtrls.Any()) _mainPickCtrls.Clear();
            List<Order> _orders = new List<Order>();
            try
            {
                _orders = (from a in new XpresEntities().Orders
                           where a.EndTime == null
                           orderby a.StartTime
                           select a).ToList();
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to retrieve orders from the database: " + _ex.Message);
                return;
            }

            if (_orders.Count == 0) return;

            foreach (Order _o in _orders)
            {
                int _lines = (int)_o.LineCount;
                bool _multi = (bool)_o.MultiPick;
                int _estTimeNum = 3 * _lines;
                TimeSpan _picktime = new TimeSpan(0, 0, _estTimeNum, 0);
                DateTime _start = (DateTime)_o.StartTime;
                DateTime _estTime = _start.Add(_picktime);

                if (!_multi)
                {
                    DateTime _startTime = (DateTime)_o.StartTime;
                    PickStackCtrlVm _vm = new PickStackCtrlVm
                    {
                        DelId = _o.DeliveryID,
                        StartPick = _o.StartTime,
                        LineCount = _o.LineCount,
                        EstFinish = _estTime,
                        Picker = _o.Picker
                    };
                    var _pickControl = new PickStackControl
                    {
                        DataContext = _vm,
                    };
                    _mainPickCtrls.Add(_pickControl);
                }
                else if(_multi)
                {
                    _lines = 0;
                    foreach (Order _multiLines in _orders)
                    {
                        DateTime? _origTime = _o.StartTime;
                        DateTime? _checkTime = _multiLines.StartTime;
                        if (_origTime == _checkTime)
                        {
                            _lines += (int)_multiLines.LineCount;
                        }
                    }
                    var _vm = new PickStackCtrlVm
                    {
                        DelId = _o.DeliveryID,
                        StartPick = _o.StartTime,
                        LineCount = _o.LineCount,
                        Picker = _o.Picker
                    };

                    GetEstTime(_lines, (DateTime)_o.StartTime);
                    _vm.EstFinish = _expTime;
                    var _pickControl = new PickStackControl{DataContext = _vm};
                    _mainPickCtrls.Add(_pickControl);
                }
            }
        }

        private void GetCompletePicks()
        {
            if (_compPicksCtrls.Any()) _compPicksCtrls.Clear();

            DateTime _today = DateTime.Today.Date;
            try
            {
                var _compOrders = (from _a in new XpresEntities().Orders
                                   where _a.EndTime != null && _a.StartTime >= _today
                                   orderby _a.StartTime
                                   select _a).ToList();

                foreach (var _o in _compOrders)
                {
                    var _vm = new PickStackCtrlVm
                    {
                        DelId = _o.DeliveryID,
                        StartPick = (DateTime)_o.StartTime,
                        LineCount = _o.LineCount,
                        EstFinish = _o.EndTime,
                        Picker = _o.Picker
                    };

                    SolidColorBrush _testColor = _o.LPH >= 20 ? Brushes.Green : Brushes.Red;
                    var _ctrl = new CompStackControl
                    {
                        DataContext = _vm
                    };

                    _ctrl.rctTest.Fill = _testColor;

                    _compPicksCtrls.Add(_ctrl);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving information from the database: " + ex.Message);
            }
        }

        public void StartSinglePick()
        {
            if (string.IsNullOrEmpty(_delId.ToString())
                || string.IsNullOrEmpty(_lineCount.ToString())
                || string.IsNullOrEmpty(_picker))
            {
                System.Windows.Forms.MessageBox.Show(@"Please make sure to enter all required info before starting a pick.");
                return;
            }

            try
            {
                var _delIdQuery = (from _a in new XpresEntities().Orders
                                   where _a.DeliveryID == _delId
                                   select _a).SingleOrDefault();

                if (_delIdQuery != null)
                {
                    System.Windows.Forms.MessageBox.Show(@"This delivery ID has already been started previously.");
                    return;
                }
            }
            catch {/*ignore*/}

            try
            {
                int? _pickNum = 1;
                XpresEntities _xps = new XpresEntities();
                try
                {
                    DateTime _now = DateTime.Now.Date;
                    _pickNum = (from a in _xps.Orders
                                where a.Picker == _picker && a.StartTime >= _now
                                select a.PickNum).Max();
                    _pickNum++;
                }
                catch {/*Ignore*/}

                Order _pick = new Order
                {
                    DeliveryID = _delId,
                    StartTime = DateTime.Now,
                    LineCount = _lineCount,
                    Picker = _picker,
                    MultiPick = false,
                    PickNum = _pickNum
                };

                _xps.Orders.Add(_pick);
                _xps.SaveChanges();
                GetPicks();
                System.Windows.Forms.MessageBox.Show(@"Delivery ID " + _delId + " successfully added to the list.");
                DelId = null;
                LineCount = null;
                Picker = null;
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while trying to add delivery id " + _delId + @" to the list: " + _ex.Message);
            }
            PickStackTimer.Start();
        }

        public void AddMultiPick()
        {
            if (string.IsNullOrEmpty(_delId.ToString()) || string.IsNullOrEmpty(_lineCount.ToString()) || string.IsNullOrEmpty(_picker))
            {
                System.Windows.Forms.MessageBox.Show(@"Please make sure all required information is entered in before adding order.");
                return;
            }
            if (_multiPickCtrls.Any())
            {
                foreach (MultiAddControl _ctrlTest in _multiPickCtrls)
                {
                    PickStackCtrlVm _vmTest = _ctrlTest.DataContext as PickStackCtrlVm;

                    if (_vmTest.DelId == _delId)
                    {
                        System.Windows.Forms.MessageBox.Show(@"This delivery ID has already been added.");
                        return;
                    }
                }
            }

            try
            {
                var _delIdQuery = (from _a in new XpresEntities().Orders
                                   where _a.DeliveryID == _delId
                                   select _a).SingleOrDefault();

                if (_delIdQuery != null)
                {
                    System.Windows.Forms.MessageBox.Show(@"This delivery ID has already been started previously.");
                    return;
                }
            }
            catch {/*ignore*/}

            PickStackCtrlVm _vm = new PickStackCtrlVm
            {
                DelId = _delId,
                LineCount = _lineCount,
                Picker = _picker
            };

            MultiAddControl _ctrl = new MultiAddControl
            {
                DataContext = _vm
            };
            _multiPickCtrls.Add(_ctrl);
            DelId = null;
            LineCount = null;
        }

        public void StartMultiPick()
        {
            if (!_multiPickCtrls.Any())
            {
                System.Windows.Forms.MessageBox.Show("No picks were added to the list.");
                return;
            }
            int _pickNum = 1;
            DateTime _startTime = DateTime.Now;
            DateTime _now = DateTime.Now.Date;
            XpresEntities _xps = new XpresEntities();

            try
            {
                var pickNum = (from a in _xps.Orders
                               where a.Picker == _picker && a.StartTime >= _now
                               select a.PickNum).Max();
                _pickNum = Convert.ToInt32(pickNum);
                _pickNum++;
            }
            catch {/*ignore*/}

            foreach (MultiAddControl _ctrl in _multiPickCtrls)
            {
                var _vm = _ctrl.DataContext as PickStackCtrlVm;
                int _ctrlDelId = (int)_vm.DelId;
                int _ctrlLineCount = (int)_vm.LineCount;
                Order _pick = new Order
                {
                    DeliveryID = _ctrlDelId,
                    StartTime = _startTime,
                    LineCount = _ctrlLineCount,
                    Picker = _picker,
                    PickNum = _pickNum,
                    MultiPick = true
                };

                _xps.Orders.Add(_pick);
                _pickNum++;
            }

            try
            {
                _xps.SaveChanges();
                GetPicks();
                System.Windows.Forms.MessageBox.Show(@"Multi-Pick list successfully added to the database.");
                DelId = null;
                LineCount = null;
                Picker = null;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while saving to the database: " + ex.Message);
                return;
            }
            _multiPickCtrls.Clear();
            PickStackTimer.Start();
        }

        private DateTime? GetEstTime(int _lines, DateTime _startTime)
        {
            try
            {
                int _expTimeNum = 3 * _lines;
                TimeSpan _picktime = new TimeSpan(0, 0, _expTimeNum, 0);
                _expTime = _startTime.Add(_picktime);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while calculating Estimated Pick Time: " + ex.Message);
                return null;
            }
            return _expTime;
        }

        public void CompletePick()
        {
            foreach (var _ctrl in _mainPickCtrls)
            {
                var _vm = _ctrl.DataContext as PickStackCtrlVm;
                if (_vm.Picker.Trim() == _picker)
                {
                    try
                    {
                        int? _delIdComp = _vm.DelId;
                        var _xps = new XpresEntities();
                        var _order = (from _a in _xps.Orders
                                      where _a.DeliveryID == _delIdComp
                                      select _a).SingleOrDefault();

                        double _lph = 0;
                        double _lines = 0;
                        double _minsNum = 0;

                        var _mins = new TimeSpan();
                        var _start = new DateTime();
                        var _end = new DateTime();

                        if (_order.MultiPick == true)
                        {
                            var _multiPickQuery = (from _a in _xps.Orders
                                                   where _a.StartTime == _order.StartTime
                                                   select _a).ToList();
                            foreach (Order _o in _multiPickQuery)
                                _lines += (double)_o.LineCount;
                        }
                        else
                        {
                            _lines = (double)_order.LineCount;
                        }
                        _start = (DateTime)_order.StartTime;
                        _end = DateTime.Now;
                        _mins = _end - _start;
                        _minsNum = Convert.ToDouble(_mins.TotalMinutes);

                        if (_minsNum < 1) _minsNum = 1;

                        _lph = (60 / _minsNum) * _lines;

                        _order.EndTime = _end;
                        _order.LPH = Convert.ToInt32(_lph);
                        _xps.SaveChanges();
                    }
                    catch (Exception _ex)
                    {
                        System.Windows.Forms.MessageBox.Show(@"Error trying to find delivery ID " + _delId + ": " + _ex.Message);
                        return;
                    }
                }
            }
            System.Windows.Forms.MessageBox.Show(@"Picks for " + _picker + " marked as completed.");
            Picker = null;
            GetCompletePicks();
            GetPicks();
            PickStackTimer.Start();
        }

        #endregion Methods
    }
}