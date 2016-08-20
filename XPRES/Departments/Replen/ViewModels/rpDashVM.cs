using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Departments.Replen.Views;
using XPRES.Helpers;

namespace XPRES.Departments.Replen.ViewModels
{
    public class rpDashVM : ViewModelBase
    {
        #region Contructor

        public rpDashVM()
        {
            percents = new GetPercentage();
            FillReplenInfo();
        }

        #endregion Contructor

        #region Properties

        private GetPercentage percents;

        private int _operators;

        public int Operators
        {
            get { return _operators; }
            set
            {
                _operators = value;
                OnPropertyChanged();
            }
        }

        private double _reqs;

        public double Reqs
        {
            get { return _reqs; }
            set
            {
                _reqs = value;
                OnPropertyChanged();
            }
        }

        private double _compReqs;

        public double CompReqs
        {
            get { return _compReqs; }
            set
            {
                _compReqs = value;
                OnPropertyChanged();
            }
        }

        private double _lph;

        public double LPH
        {
            get { return _lph; }
            set
            {
                _lph = value;
                OnPropertyChanged();
            }
        }

        private double _compPcnt;

        public double CompPcnt
        {
            get { return _compPcnt; }
            set
            {
                _compPcnt = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        public ICommand WhViewCommand => new RelayCommand(x => OpenWhView());

        #endregion ICommand Members

        #region Methods

        private void OpenWhView()
        {
            bool _open = false;
            foreach (Window _wnd in Application.Current.Windows)
            {
                if (_wnd is WarehouseView)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                WarehouseView _whView = new WarehouseView();
                _whView.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == @"Warehouse View")
                    {
                        _wnd.Activate();
                    }
                }
            }
            foreach (Window _wnd in Application.Current.Windows)
            {
                if (_wnd is RepDash)
                {
                    _wnd.Close();
                }
            }
        }

        private void FillReplenInfo()
        {
            DateTime _sdate = DateTime.Today.Date;
            DateTime _edate = DateTime.Today.Date.AddDays(1);

            try
            {
                var _replens = (from a in new XpresEntities().ReplenSAAGs
                               where a.TimeStamp >= _sdate && a.TimeStamp < _edate
                               select a);

                if (_replens != null && _replens.Count() > 0)
                {
                    var _opers = new List<string>();
                    _opers = _replens.Select(a => a.Employee).Distinct().ToList();
                    _operators = _opers.Count;
                }

                var _reqsQuery = (from a in new XpresEntities().MaterialRequests
                            where a.SubTimestamp >= _sdate && a.SubTimestamp < _edate
                            select a).ToList();

                if (_reqsQuery != null && _reqsQuery.Count > 0)
                {
                    _reqs = _reqsQuery.Count;
                    _compReqs = _reqsQuery.Count(a => a.ReqStatus == @"Delivered");
                    _compPcnt = (_compReqs / _reqs) * 100;
                    _compPcnt = Math.Round(_compPcnt, 2);
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving replen info: " + _ex.Message);
            }
        }

        #endregion Methods
    }
}