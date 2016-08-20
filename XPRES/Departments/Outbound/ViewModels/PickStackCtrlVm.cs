using System;
using System.Linq;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Helpers;

namespace XPRES.Departments.Outbound.ViewModels
{
    public class PickStackCtrlVm : ViewModelBase
    {
        #region Properties

        private bool _compCheck;

        public bool CompCheck
        {
            get { return _compCheck; }
            set
            {
                _compCheck = value;
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

        private DateTime? _startPick;

        public DateTime? StartPick
        {
            get { return _startPick; }
            set
            {
                _startPick = value;
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

        private DateTime? _estFinish;

        public DateTime? EstFinish
        {
            get { return _estFinish; }
            set
            {
                _estFinish = value;
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

        #endregion Properties

        #region Methods

        public void DeletePick()
        {
            try
            {
                var _xps = new XpresEntities();
                Order _pick = (from _a in _xps.Orders
                               where _a.DeliveryID == _delId
                               select _a).SingleOrDefault();

                _xps.Orders.Remove(_pick);
                _xps.SaveChanges();
                System.Windows.Forms.MessageBox.Show(_delId + " removed from the order list");
            }
            catch{/*Ignore*/}
        }

        #endregion Methods
    }
}