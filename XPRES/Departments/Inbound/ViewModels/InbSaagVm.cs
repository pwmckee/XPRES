using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Departments.Inbound.Controls;
using XPRES.Helpers;

namespace XPRES.Departments.Inbound.ViewModels
{
    public class InbSaagVm : ViewModelBase
    {
        #region Constructor

        public InbSaagVm()
        {
            _carrierList = new List<string>();
            _ltlCtrls = new ObservableCollection<LtlStackControl>();
            _smlPkCtrls = new ObservableCollection<SmlPkStackControl>();
            GetCarriers();
        }

        #endregion Constructor

        #region Properties

        private ObservableCollection<LtlStackControl> _ltlCtrls;

        public ObservableCollection<LtlStackControl> LtlCtrls
        {
            get { return _ltlCtrls; }
            set
            {
                _ltlCtrls = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SmlPkStackControl> _smlPkCtrls;

        public ObservableCollection<SmlPkStackControl> SmlPkCtrls
        {
            get { return _smlPkCtrls; }
            set
            {
                _smlPkCtrls = value;
                OnPropertyChanged();
            }
        }

        private string _selDate;

        public string SelDate
        {
            get { return _selDate; }
            set
            {
                _selDate = value;
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

        #endregion Properties

        #region ICommand Members

        public ICommand AddLtlCommand => new RelayCommand(x => AddLtl());

        public ICommand GetLtlSchedCommand => new RelayCommand(x => GetSched(true));

        public ICommand AddSmlPkCommand => new RelayCommand(x => AddSmlPkTruck());

        public ICommand GetSmlPkSchedCommand => new RelayCommand(x => GetSched(false));

        public ICommand AddCarrierCommand => new RelayCommand(x => AddCarrier());

        public ICommand DeleteCarrierCommand => new RelayCommand(x => DeleteCarrier());

        #endregion ICommand Members

        #region Methods

        private void GetCarriers()
        {
            try
            {
                if(_carrierList.Count > 0) _carrierList.Clear();
                var _carrQuery = (from _a in new XpresEntities().CarrierLists
                                  orderby _a.Carrier select _a.Carrier).ToList();
                CarrierList = _carrQuery;
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving carrier list: " + _ex.Message);
            }
        }

        private void AddLtl()
        {
            LtlStackControl _ltlCtrl = new LtlStackControl();
            _ltlCtrls.Add(_ltlCtrl);
        }

        private void AddSmlPkTruck()
        {
            SmlPkStackControl _smlPkCtrl = new SmlPkStackControl();
            _smlPkCtrls.Add(_smlPkCtrl);
        }

        private void GetSched(bool ltl)
        {
            if (_ltlCtrls.Any()) _ltlCtrls.Clear();
            if (_smlPkCtrls.Any()) _smlPkCtrls.Clear();

            XpsDates _xdate = new XpsDates();
            _xdate.CheckDateEntry(_selDate, DateTime.Today.Date.ToString());
            DateTime _schedDate = _xdate.ValidDate;
            DateTime _tomorrow = _schedDate.AddDays(1);

            List<RcvSchedule> _schedList = new List<RcvSchedule>();

            try
            {
                if(ltl)
                {
                    _schedList = (from _a in new XpresEntities().RcvSchedules
                                  where _a.Appt >= _schedDate && _a.Appt < _tomorrow && _a.Ltl == true
                                  orderby _a.Appt
                                  select _a).ToList();
                }
                else
                {
                    _schedList = (from _a in new XpresEntities().RcvSchedules
                                  where _a.Arrive >= _schedDate && _a.Arrive < _tomorrow && _a.Ltl == false
                                  orderby _a.Appt
                                  select _a).ToList();
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving schedule information: " + _ex.Message);
            }

            if (_schedList.Count == 0) return;

            foreach (RcvSchedule _apt in _schedList)
            {
                SchedStackVm _vm = new SchedStackVm
                {
                    ApptId = _apt.ApptID,
                    ApptTime = _apt.Appt,
                    Carrier = _apt.Carrier.Trim(),
                    PltNum = _apt.PalletNum,
                    ArvTime = _apt.Arrive,
                    UlStart = _apt.UnloadStart,
                    UlStop = _apt.UnloadStop,
                    RecStop = _apt.RecStop,
                    PutStop = _apt.PutawayStop,
                    PoRcvd = _apt.PoRcvd,
                    PoNotRcvd = _apt.PoNotRcvd,
                    PoPtwy = _apt.PoPtwy,
                    PoNotPtwy = _apt.PoNotPtwy,
                    Vnc = _apt.VNC
                };

                if(ltl)
                {
                    LtlStackControl _stkCtrl = new LtlStackControl
                    {
                        DataContext = _vm,
                        CboCarrier = { SelectedItem = _vm.Carrier },
                        GrdEdit = { Visibility = Visibility.Hidden },
                        GrdDisp = { Visibility = Visibility.Visible }
                    };

                    _ltlCtrls.Add(_stkCtrl);
                }
                else
                {
                    SmlPkStackControl _stkCtrl = new SmlPkStackControl
                    {
                        DataContext = _vm,
                        CboCarrier = { SelectedItem = _vm.Carrier },
                        GrdEdit = { Visibility = Visibility.Hidden },
                        GrdDisp = { Visibility = Visibility.Visible }
                    };

                    _smlPkCtrls.Add(_stkCtrl);
                }
            }
        }

        private void AddCarrier()
        {
            if (string.IsNullOrEmpty(_carrier)) return;
            try
            {
                XpresEntities _xps = new XpresEntities();
                try
                {
                    var _carrQuery = (from _a in _xps.CarrierLists
                                      where _a.Carrier == _carrier
                                      select _a.Carrier).First();
                    System.Windows.Forms.MessageBox.Show(@"This carrier already exists in the list.");
                }
                catch
                {
                    CarrierList _carrEntry = new CarrierList { Carrier = _carrier };
                    _xps.CarrierLists.Add(_carrEntry);
                    _xps.SaveChanges();
                    GetCarriers();
                    System.Windows.Forms.MessageBox.Show(@"Carrier successfuly saved!");
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error while trying to save to the database: " + _ex.Message);
            }
        }

        private void DeleteCarrier()
        {
            if (string.IsNullOrEmpty(_carrier)) return;
            System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                @"Are you sure you want to delete this carrier from the list?", @"Delete Carrier Entry",
                System.Windows.Forms.MessageBoxButtons.YesNo);
            if (_diag == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    XpresEntities _xps = new XpresEntities();
                    var _q = (from _a in _xps.CarrierLists
                              where _a.Carrier == _carrier
                              select _a);
                    foreach (var _item in _q) _xps.CarrierLists.Remove(_item);
                    _xps.SaveChanges();
                    System.Windows.Forms.MessageBox.Show(@"Carrier removed from the database");
                    GetCarriers();
                }
                catch (Exception _ex)
                {
                    System.Windows.Forms.MessageBox.Show(@"Error while trying to delete carrier from the database: " + _ex.Message);
                }
            }
        }

        #endregion Methods
    }
}