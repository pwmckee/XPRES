using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using XPRES.Commands;
using XPRES.DAL;

namespace XPRES.Outbound.ViewModels
{
    public class obDashViewModel : INotifyPropertyChanged
    {
        public HomeCommand HomeEvent { get; set; }
        private XpresEntities xps;

        public obDashViewModel()
        {
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

        private double _totPicks;

        public double TotPicks
        {
            get { return _totPicks; }
            set
            {
                _totPicks = value;
                NotifyPropertyChanged();
            }
        }

        private double _compPicks;

        public double CompPicks
        {
            get { return _compPicks; }
            set
            {
                _compPicks = value;
                NotifyPropertyChanged();
            }
        }

        private double _totlines;

        public double TotLines
        {
            get { return _totlines; }
            set
            {
                _totlines = value;
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

        #region Methods

        private void FillReplenInfo()
        {
            xps = new XpresEntities();
            DateTime _sdate = DateTime.Today.Date;
            DateTime _edate = DateTime.Today.Date.AddDays(1);

            try
            {
                var orders = (from a in xps.Orders
                              where a.StartTime >= _sdate && a.StartTime < _edate
                              select a);

                if (orders != null && orders.Count() > 0)
                {
                    List<string> opers = new List<string>();
                    opers = orders.Select(a => a.Picker).Distinct().ToList();
                    _operators = opers.Count;
                    _totPicks = orders.Count();
                    _compPicks = orders.Where(a => a.EndTime != null).Count();
                    _totlines = Convert.ToDouble(orders.Sum(a => a.LineCount));
                    _lph = Convert.ToDouble(orders.Sum(a => a.LPH));
                    _lph = (_lph / _totPicks);
                    _lph = Math.Round(_lph, 0);
                    _compPcnt = (_compPicks / _totPicks) * 100;
                    _compPcnt = Math.Round(_compPcnt, 0);
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