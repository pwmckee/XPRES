using System.Windows;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.Departments.Inbound.Views;
using XPRES.Departments.Inventory.Adjustments;
using XPRES.Departments.Inventory.Views;
using XPRES.Departments.Outbound.Views;
using XPRES.Departments.Replen.Views;
using XPRES.Main.Views;

namespace XPRES.Main.ViewModels
{
    public class MainWindowVm
    {
        #region FullScreen Metrics

        #region ICommand Members

        public ICommand FullscreenMetricsCommand => new RelayCommand(x => this.OpenFullScreenMetrics());

        #endregion ICommand Members

        #region Methods

        private void OpenFullScreenMetrics()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is FullCharts)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                FullCharts _nameVar = new FullCharts();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Metric Charts")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        #endregion Methods

        #endregion FullScreen Metrics

        #region Inbound

        #region ICommand Members

        public ICommand InbSaagCommand => new RelayCommand(x => OpenInbSaag());

        public ICommand InbActivityCommand => new RelayCommand(x => OpenInbActivity());

        public ICommand InbDashCommand => new RelayCommand(x => OpenInbDash());

        #endregion ICommand Members

        #region Methods

        private void OpenInbSaag()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is InbSAAG)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                InbSAAG _nameVar = new InbSAAG();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Inbound SAAG")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        private void OpenInbActivity()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is InbActivity)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                InbActivity _nameVar = new InbActivity();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Inbound Activity")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        //private void OpenInbMetrics()
        //{
        //    bool _open = false;
        //    foreach (var wnd in Application.Current.Windows)
        //    {
        //        if (wnd is InbMetrics)
        //        {
        //            _open = true;
        //        }
        //    }
        //    if (!_open)
        //    {
        //        InbMetrics _nameVar = new InbMetrics();
        //        _nameVar.Show();
        //    }
        //    else
        //    {
        //        foreach (Window wnd in Application.Current.Windows)
        //        {
        //            if (wnd.Title == "Inbound Metrics")
        //            {
        //                wnd.Activate();
        //            }
        //        }
        //    }
        //}

        private void OpenInbDash()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is InbDash)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                InbDash _nameVar = new InbDash();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Inbound Dashboard")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        #endregion Methods

        #endregion Inbound

        #region Outbound

        #region ICommand Members

        public ICommand OutSaagCommand => new RelayCommand(x => this.OpenOutSaag());

        public ICommand OutDashCommand => new RelayCommand(x => this.OpenOutDash());

        #endregion ICommand Members

        #region Methods

        private void OpenOutSaag()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is obSAAG)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                obSAAG _nameVar = new obSAAG();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Picking SAAG")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        private void OpenOutDash()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is obDash)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                obDash _nameVar = new obDash();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Outbound Dashboard")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        #endregion Methods

        #endregion Outbound

        #region Inventory

        #region ICommand Members

        public ICommand InvDashCommand => new RelayCommand(x => this.OpenInvDash());

        public ICommand InvGeoCountsCommand => new RelayCommand(x => this.OpenGeoCounts());

        public ICommand InbAdjCommand => new RelayCommand(x => this.OpenAdjMain());

        #endregion ICommand Members

        #region Methods

        private void OpenInvDash()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is InvMain)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                InvMain _nameVar = new InvMain();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Inventory Dashboard")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        private void OpenGeoCounts()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is GcMain)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                GcMain _nameVar = new GcMain();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Geographic Counts")
                    {
                        _wnd.Activate();
                    }
                }
            }
            //foreach (var wnd in Application.Current.Windows)
            //{
            //    if (wnd is GeoCountsMain)
            //    {
            //        _open = true;
            //    }
            //}
            //if (!_open)
            //{
            //    GeoCountsMain _nameVar = new GeoCountsMain();
            //    _nameVar.Show();
            //}
            //else
            //{
            //    foreach (Window wnd in Application.Current.Windows)
            //    {
            //        if (wnd.Title == "Geographic Counts")
            //        {
            //            wnd.Activate();
            //        }
            //    }
            //}
        }

        private void OpenAdjMain()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is AdjMain)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                AdjMain _nameVar = new AdjMain();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Adjustments")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        #endregion Methods

        #endregion Inventory

        #region Replen

        #region ICommand Members

        public ICommand RepDashCommand => new RelayCommand(x => this.OpenRepDash());

        public ICommand RepSaagCommand => new RelayCommand(x => this.OpenRepSaag());

        public ICommand RepAndonCommand => new RelayCommand(x => this.OpenRepAndon());

        #endregion ICommand Members

        #region Methods

        private void OpenRepDash()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is RepDash)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                RepDash _nameVar = new RepDash();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Replen Dash")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        private void OpenRepSaag()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is RepSAAG)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                RepSAAG _nameVar = new RepSAAG();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Replen SAAG")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        private void OpenRepAndon()
        {
            bool _open = false;
            foreach (var _wnd in Application.Current.Windows)
            {
                if (_wnd is WarehouseView)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                WarehouseView _nameVar = new WarehouseView();
                _nameVar.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Warehouse View")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        #endregion Methods

        #endregion Replen
    }
}