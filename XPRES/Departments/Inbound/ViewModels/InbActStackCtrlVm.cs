using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Helpers;

namespace XPRES.Departments.Inbound.ViewModels
{
    public class InbActStackCtrlVm : ViewModelBase
    {
        #region Constructor

        public InbActStackCtrlVm()
        {
            _operatorList = new List<string>();
            FillOperators();
        }

        #endregion Constructor

        #region Properties

        private string _transId;

        public string TransId
        {
            get { return _transId; }
            set
            {
                _transId = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _start;

        public DateTime? Start
        {
            get { return _start; }
            set
            {
                _start = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _finish;

        public DateTime? Finish
        {
            get { return _finish; }
            set
            {
                _finish = value;
                OnPropertyChanged();
            }
        }

        private int? _lineItems;

        public int? LineItems
        {
            get { return _lineItems; }
            set
            {
                _lineItems = value;
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

        private string _ctrlType;

        public string CtrlType
        {
            get { return _ctrlType; }
            set
            {
                _ctrlType = value;
                OnPropertyChanged();
            }
        }

        private string _ctrlId;

        public string CtrlId
        {
            get { return _ctrlId; }
            set
            {
                _ctrlId = value;
                OnPropertyChanged();
            }
        }

        private double? _lph;

        public double? Lph
        {
            get { return _lph; }
            set
            {
                _lph = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        public ICommand SaveCtrlCommand { get { return new RelayCommand(x => SaveCtrl()); } }

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

        private void SaveCtrl()
        {
            int _id;
            try
            {
                IQueryable<InboundActivity> _idQuery = (from _a in new XpresEntities().InboundActivities select _a);

                _idQuery = _idQuery.Where(x => x.Type == CtrlType);

                _id = _idQuery.Select(x => x.Id).Max() + 1;
            }
            catch
            {
                _id = 1;
            }
            if (string.IsNullOrEmpty(_transId))
            {
                if (_ctrlType == "REC")
                {
                    System.Windows.Forms.MessageBox.Show(@"Please enter the PO being received.");
                    return;
                }
                else if(_ctrlType == "PUT")
                {
                    System.Windows.Forms.MessageBox.Show(@"Please enter the LPN being putaway.");
                    return;
                }
            }

            try
            {
                XpresEntities _xps = new XpresEntities();
                if (!string.IsNullOrEmpty(_finish.ToString()) && !string.IsNullOrEmpty(_lineItems.ToString()))
                {
                    TimeSpan _mins = (DateTime)_finish - (DateTime)_start;
                    double _minsNum = Convert.ToDouble(_mins.TotalMinutes);
                    if (_minsNum < 1)
                        _minsNum = 1;
                    _lph = (60 / _minsNum) * _lineItems;
                }
                if (string.IsNullOrEmpty(_ctrlId))
                {
                    if (_ctrlType == "REC")
                    {
                        _ctrlId = "REC" + _id;
                    }
                    else if(_ctrlType == "PUT")
                    {
                        _ctrlId = "PUT" + _id;
                    }

                    

                    InboundActivity _inbAct = new InboundActivity
                    {
                        PO_LPN = _transId,
                        Start = _start,
                        Finish = _finish,
                        LineItems = _lineItems,
                        Operator = _selOper,
                        Type = _ctrlType,
                        CtrlId = _ctrlId,
                        LPH = _lph
                    };
                    _xps.InboundActivities.Add(_inbAct);
                    _xps.InboundActivities.Add(_inbAct);
                }
                else
                {
                    InboundActivity _actQuery = (from _a in _xps.InboundActivities
                                                 where _a.CtrlId == _ctrlId
                                                 select _a).SingleOrDefault();

                    _actQuery.PO_LPN = _transId;
                    _actQuery.Start = _start;
                    _actQuery.Finish = _finish;
                    _actQuery.LineItems = _lineItems;
                    _actQuery.Operator = _selOper;
                    _actQuery.Type = _ctrlType;
                    _actQuery.CtrlId = _ctrlId;
                    _actQuery.LPH = _lph;
                }
                _xps.SaveChanges();
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while saving record: " + _ex.Message);
            }
        }

        #endregion Methods
    }
}
