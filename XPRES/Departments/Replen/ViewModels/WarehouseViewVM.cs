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

namespace XPRES.Departments.Replen.ViewModels
{
    public class WarehouseViewVM : INotifyPropertyChanged
    {
        public HomeCommand HomeEvent { get; set; }
        private XpresEntities xps;

        public WarehouseViewVM()
        {
            HomeEvent = new HomeCommand();
            FillProdLines();
        }

        #region Properties

        private List<string> _prodLineList;

        public List<string> ProdLineList
        {
            get { return _prodLineList; }
            set
            {
                _prodLineList = value;
                NotifyPropertyChanged();
            }
        }

        private string _prodLine;

        public string ProdLine
        {
            get { return _prodLine; }
            set
            {
                _prodLine = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        private ICommand _prodViewCommand;

        public ICommand ProdViewCommand
        {
            get
            {
                if (_prodViewCommand == null)
                    _prodViewCommand = new RelayCommand(param => ProdViewCommandExecute(param));
                return _prodViewCommand;
            }
            set
            {
                _prodViewCommand = value;
            }
        }

        private ICommand _dashViewCommand;

        public ICommand DashViewCommand
        {
            get
            {
                if (_dashViewCommand == null)
                    _dashViewCommand = new RelayCommand(param => DashViewCommandExecute(param));
                return _dashViewCommand;
            }
            set
            {
                _dashViewCommand = value;
            }
        }

        #endregion ICommand Members

        #region Methods

        #region ICommand Methods

        private void ProdViewCommandExecute(object param)
        {
            OpenProdView();
        }

        private void OpenProdView()
        {
            bool _open = false;
            foreach (Window wnd in Application.Current.Windows)
            {
                if (wnd is ProductionView)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                ProductionView _prodView = new ProductionView();
                _prodView.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Production View")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        private void DashViewCommandExecute(object param)
        {
            OpenDashView();
        }

        private void OpenDashView()
        {
            bool _open = false;
            foreach (Window wnd in Application.Current.Windows)
            {
                if (wnd is RepDash)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                RepDash _dashView = new RepDash();
                _dashView.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Replen Dash")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        #endregion ICommand Methods

        private void FillProdLines()
        {
            try
            {
                xps = new XpresEntities();

                _prodLineList = new List<string>();

                var prodLines = (from a in xps.ProductionAreas
                                 select a.ProdLine).ToList();
                foreach (var p in prodLines)
                {
                    _prodLineList.Add(p.ToString());
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error retrieving production line list:" + ex.Message);
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