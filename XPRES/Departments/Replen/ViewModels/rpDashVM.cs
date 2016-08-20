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
    public class rpDashVM : INotifyPropertyChanged
    {
        private XpresEntities xps;
        private GetPercentage percents;
        public HomeCommand HomeEvent;

        public rpDashVM()
        {
            xps = new XpresEntities();
            percents = new GetPercentage();
            HomeEvent = new HomeCommand();
            FillReplenInfo();
        }

        #region Properties

        private int _operators;

        public int Operators
        {
            get { return _operators; }
            set
            {
                _operators = value;
                NotifyPropertyChanged();
            }
        }

        private double _reqs;

        public double Reqs
        {
            get { return _reqs; }
            set
            {
                _reqs = value;
                NotifyPropertyChanged();
            }
        }

        private double _compReqs;

        public double CompReqs
        {
            get { return _compReqs; }
            set
            {
                _compReqs = value;
                NotifyPropertyChanged();
            }
        }

        private double _lph;

        public double LPH
        {
            get { return _lph; }
            set
            {
                _lph = value;
                NotifyPropertyChanged();
            }
        }

        private double _compPcnt;

        public double CompPcnt
        {
            get { return _compPcnt; }
            set
            {
                _compPcnt = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        private ICommand _whViewCommand;

        public ICommand WhViewCommand
        {
            get
            {
                if (_whViewCommand == null)
                    _whViewCommand = new RelayCommand(param => WhViewCommandExecute(param));
                return _whViewCommand;
            }
            set
            {
                _whViewCommand = value;
            }
        }

        #endregion ICommand Members

        #region Methods

        #region ICommand Methods

        private void WhViewCommandExecute(object param)
        {
            OpenWhView();
        }

        private void OpenWhView()
        {
            bool _open = false;
            foreach (Window wnd in Application.Current.Windows)
            {
                if (wnd is WarehouseView)
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
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Warehouse View")
                    {
                        wnd.Activate();
                    }
                }
            }
            foreach (Window wnd in Application.Current.Windows)
            {
                if (wnd is RepDash)
                {
                    wnd.Close();
                }
            }
        }

        #endregion ICommand Methods

        private void FillReplenInfo()
        {
            xps = new XpresEntities();
            DateTime _sdate = DateTime.Today.Date;
            DateTime _edate = DateTime.Today.Date.AddDays(1);

            try
            {
                var replens = (from a in xps.ReplenSAAGs
                               where a.TimeStamp >= _sdate && a.TimeStamp < _edate
                               select a);

                if (replens != null && replens.Count() > 0)
                {
                    List<string> opers = new List<string>();
                    opers = replens.Select(a => a.Employee).Distinct().ToList();
                    _operators = opers.Count;
                }

                var reqs = (from a in xps.MaterialRequests
                            where a.SubTimestamp >= _sdate && a.SubTimestamp < _edate
                            select a).ToList();

                if (reqs != null && reqs.Count > 0)
                {
                    _reqs = reqs.Count;
                    _compReqs = reqs.Count(a => a.ReqStatus == "Delivered");
                    _compPcnt = (_compReqs / _reqs) * 100;
                    _compPcnt = Math.Round(_compPcnt, 2);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error retrieving replen info: " + ex.Message);
            }
        }

        #endregion Methods

        #region INotify Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotify Implementation
    }
}