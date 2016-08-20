using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Helpers;

namespace XPRES.Departments.Inbound.ViewModels
{
    public class SchedStackVm : ViewModelBase
    {
        #region Constructor

        public SchedStackVm()
        {
            _carrierList = new List<string>();
            FillCarriers();
        }

        #endregion Constructor

        #region Properties

        private List<string> _carrierList;

        public List<string> CarrierList
        {
            get { return _carrierList; }
            set
            {
                _carrierList = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _apptTime;

        public DateTime? ApptTime
        {
            get { return _apptTime; }
            set
            {
                _apptTime = value;
                OnPropertyChanged();
            }
        }

        private string _carrier;

        public string Carrier
        {
            get { return _carrier; }
            set
            {
                _carrier = value;
                OnPropertyChanged();
            }
        }

        private int? _pltNum;

        public int? PltNum
        {
            get { return _pltNum; }
            set
            {
                _pltNum = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _arvTime;

        public DateTime? ArvTime
        {
            get { return _arvTime; }
            set
            {
                _arvTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _ulStart;

        public DateTime? UlStart
        {
            get { return _ulStart; }
            set
            {
                _ulStart = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _ulStop;

        public DateTime? UlStop
        {
            get { return _ulStop; }
            set
            {
                _ulStop = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _recStop;

        public DateTime? RecStop
        {
            get { return _recStop; }
            set
            {
                _recStop = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _putStop;

        public DateTime? PutStop
        {
            get { return _putStop; }
            set
            {
                _putStop = value;
                OnPropertyChanged();
            }
        }

        private string _apptId;

        public string ApptId
        {
            get { return _apptId; }
            set
            {
                _apptId = value;
                OnPropertyChanged();
            }
        }

        private int? _poRcvd;

        public int? PoRcvd
        {
            get { return _poRcvd; }
            set
            {
                _poRcvd = value;
                OnPropertyChanged();
            }
        }

        private int? _poNotRcvd;

        public int? PoNotRcvd
        {
            get { return _poNotRcvd; }
            set
            {
                _poNotRcvd = value;
                OnPropertyChanged();
            }
        }

        private int? _poPtwy;

        public int? PoPtwy
        {
            get { return _poPtwy; }
            set
            {
                _poPtwy = value;
                OnPropertyChanged();
            }
        }

        private int? _poNotPtwy;

        public int? PoNotPtwy
        {
            get { return _poNotPtwy; }
            set
            {
                _poNotPtwy = value;
                OnPropertyChanged();
            }
        }

        private int? _vnc;

        public int? Vnc
        {
            get { return _vnc; }
            set
            {
                _vnc = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        public ICommand SaveLtlApptCommand => new RelayCommand(x => SaveAppt(true));

        public ICommand SaveSmlPkApptCommand => new RelayCommand(x => SaveAppt(false));

        #endregion ICommand Members

        #region Methods


        private void FillCarriers()
        {
            try
            {
                List<string> _car = (from _a in new XpresEntities().CarrierLists
                                     select _a.Carrier).Distinct().ToList();
                _carrierList = _car;
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving carrier list" + _ex.Message);
            }
        }

        private void SaveAppt(bool ltl)
        {
            int _id;
            try
            {
                IQueryable<RcvSchedule> _idQuery = (from _a in new XpresEntities().RcvSchedules select _a);

                _idQuery = _idQuery.Where(x => x.Ltl == ltl);

                _id = _idQuery.Select(x => x.Id).Max() + 1;
            }
            catch
            {
                _id = 1;
            }

            XpresEntities _xps = new XpresEntities();

            try
            {
                if (string.IsNullOrEmpty(_apptId))
                {
                    if (ltl)
                    {
                        if (_apptTime == null || _carrier == null)
                        {
                            System.Windows.Forms.MessageBox.Show(@"Please make sure to select both a scheduled appt time and a carrier.");
                            return;
                        }
                        _apptId = "LTL" + _id;
                    }
                    else
                    {
                        if (_arvTime == null || _carrier == null)
                        {
                            System.Windows.Forms.MessageBox.Show(@"Please make sure to select both an arrival time and a carrier.");
                            return;
                        }
                        _apptId = "SMLPK" + _id;
                    }

                    RcvSchedule _sch = new RcvSchedule
                    {
                        Appt = _apptTime,
                        Carrier = _carrier,
                        PalletNum = _pltNum,
                        Arrive = _arvTime,
                        UnloadStart = _ulStart,
                        UnloadStop = _ulStop,
                        RecStop = _recStop,
                        PutawayStop = _putStop,
                        Ltl = ltl,
                        ApptID = _apptId,
                        PoRcvd = _poRcvd,
                        PoNotRcvd = _poNotRcvd,
                        PoPtwy = _poPtwy,
                        PoNotPtwy = _poNotPtwy,
                        VNC = _vnc
                    };
                    _xps.RcvSchedules.Add(_sch);
                }
                else
                {
                    RcvSchedule _schQuery = (from _a in _xps.RcvSchedules
                                             where _a.ApptID == _apptId
                                             select _a).SingleOrDefault();

                    _schQuery.Appt = _apptTime;
                    _schQuery.Carrier = _carrier;
                    _schQuery.PalletNum = _pltNum;
                    _schQuery.Arrive = _arvTime;
                    _schQuery.UnloadStart = _ulStart;
                    _schQuery.UnloadStop = _ulStop;
                    _schQuery.RecStop = _recStop;
                    _schQuery.PutawayStop = _putStop;
                    _schQuery.PoRcvd = _poRcvd;
                    _schQuery.PoNotRcvd = _poNotRcvd;
                    _schQuery.PoPtwy = _poPtwy;
                    _schQuery.PoNotPtwy = _poNotPtwy;
                    _schQuery.VNC = _vnc;
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