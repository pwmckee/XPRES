using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Departments.Inbound.Controls;
using XPRES.Helpers;

namespace XPRES.Departments.Inbound.ViewModels
{
    public class InbActivityVm : ViewModelBase
    {
        #region Constructor

        public InbActivityVm()
        {
            _putCtrls = new ObservableCollection<InbActStackControl>();
            _recCtrls = new ObservableCollection<InbActStackControl>();
            _operatorList = new List<string>();
            FillOperators();
        }

        #endregion Constructor

        #region Properties

        private ObservableCollection<InbActStackControl> _putCtrls;

        public ObservableCollection<InbActStackControl> PutCtrls
        {
            get { return _putCtrls; }
            set
            {
                _putCtrls = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<InbActStackControl> _recCtrls;

        public ObservableCollection<InbActStackControl> RecCtrls
        {
            get { return _recCtrls; }
            set
            {
                _recCtrls = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _sDate;

        public DateTime? SDate
        {
            get { return _sDate;}
            set
            {
                _sDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _eDate;

        public DateTime? EDate
        {
            get { return _eDate; }
            set
            {
                _eDate = value;
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

        private string _selOper;

        public string SelOper
        {
            get { return _selOper; }
            set
            {
                _selOper = value;
                OnPropertyChanged();
            }
        }

        private List<string> _operatorList;

        public List<string> OperatorList
        {
            get { return _operatorList; }
            set
            {
                _operatorList = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        public ICommand ViewDateRangeCommand => new RelayCommand(x => ViewDateRange());

        public ICommand AddOperatorCommand => new RelayCommand(x => AddOperator());

        public ICommand DeleteOperatorCommand => new RelayCommand(x => DeleteOperator());

        public ICommand AddRecCtrlCommand => new RelayCommand(x => AddCtrl(true));

        public ICommand AddPutCtrlCommand => new RelayCommand(x => AddCtrl(false));

        #endregion ICommand Members

        #region Methods

        private void FillOperators()
        {
            try
            {
                List<string> _oper = (from _a in new XpresEntities().Employees
                                      where _a.InBound == true
                                      select _a.FullName).ToList();
                _operatorList = _oper;
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while retrieving operator list from the database: " + _ex.Message);
            }
        }

        private void ViewDateRange()
        {
            if (string.IsNullOrEmpty(_sDate.ToString())) _sDate = DateTime.Today.Date;
            try
            {
                var _actQuery = (from _a in new XpresEntities().InboundActivities
                                 where _a.Start >= _sDate
                                 select _a);
                if (!string.IsNullOrEmpty(_eDate.ToString()))
                    _actQuery = _actQuery.Where(x => x.Finish <= _eDate);
                var _recQuery = _actQuery.Where(x => x.Type == "REC");
                var _putQuery = _actQuery.Where(x => x.Type == "PUT");

                foreach (var _rec in _recQuery)
                {
                    InbActStackCtrlVm _vm = new InbActStackCtrlVm
                    {
                        TransId = _rec.PO_LPN,
                        Start = _rec.Start,
                        Finish = _rec.Finish,
                        LineItems = _rec.LineItems,
                        CtrlType = _rec.Type,
                        CtrlId = _rec.CtrlId
                    };
                    InbActStackControl _ctrl = new InbActStackControl { DataContext = _vm };
                    _recCtrls.Add(_ctrl);
                }

                foreach (var _put in _putQuery)
                {
                    InbActStackCtrlVm _vm = new InbActStackCtrlVm
                    {
                        TransId = _put.PO_LPN,
                        Start = _put.Start,
                        Finish = _put.Finish,
                        LineItems = _put.LineItems,
                        CtrlType = _put.Type,
                        CtrlId = _put.CtrlId
                    };
                    InbActStackControl _ctrl = new InbActStackControl { DataContext = _vm };
                    _putCtrls.Add(_ctrl);
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving Inbounc Activities: " + _ex.Message);
            }
        }

        private void AddOperator()
        {
            if (string.IsNullOrEmpty(_firstName) || string.IsNullOrEmpty(_lastName))
            {
                System.Windows.Forms.MessageBox.Show(@"Please enter both a first and a last name.");
                return;
            }
            try
            {
                string _fullName = _firstName + @" " + _lastName;
                XpresEntities _xps = new XpresEntities();
                Employee _employee;
                var _empQuery = (from _a in _xps.Employees
                                 where _a.FullName == _fullName
                                 select _a);
                if (_empQuery == null)
                {
                    _employee = new Employee
                    {
                        FirstName = _firstName,
                        LastName = _lastName,
                        FullName = _fullName,
                        InBound = true
                    };
                    _xps.Employees.Add(_employee);
                }
                else
                {
                    _employee = _empQuery.SingleOrDefault();
                    _employee.InBound = true;
                }
                _xps.SaveChanges();
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error adding employee to the Inbound Operator list: " + _ex.Message);
            }
        }

        private void DeleteOperator()
        {
            if (string.IsNullOrEmpty(_selOper)) return;
            System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                @"Are you sure you want to delete this employee from the Inbound Operator list?", @"Delete Inbound Employee Entry",
                System.Windows.Forms.MessageBoxButtons.YesNo);
            if (_diag == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    XpresEntities _xps = new XpresEntities();
                    var _employee = (from _a in _xps.Employees
                              where _a.FullName == _selOper
                              select _a).SingleOrDefault();
                    _employee.InBound = false;
                    _xps.SaveChanges();
                    System.Windows.Forms.MessageBox.Show(@"Employee removed from the Inbound Operator list");
                    FillOperators();
                }
                catch (Exception _ex)
                {
                    System.Windows.Forms.MessageBox.Show(@"Error while trying to delete employee from the Inbound Operator list: " + _ex.Message);
                }
            }
        }

        private void AddCtrl(bool rec)
        {
            InbActStackControl _ctrl = new InbActStackControl();
            InbActStackCtrlVm _vm = _ctrl.DataContext as InbActStackCtrlVm;

            if (rec)
            {
                _vm.CtrlType = "REC";
                _recCtrls.Add(_ctrl);
            }
            else
            {
                _vm.CtrlType = "PUT";
                _putCtrls.Add(_ctrl);
            }
        }

        #endregion Methods
    }
}
