using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Departments.Replen.Views;
using XPRES.Helpers;

namespace XPRES.Departments.Replen.ViewModels
{
    public class WarehouseViewVM : ViewModelBase
    {
        #region Constructor

        public WarehouseViewVM()
        {
            FillProdLines();
        }

        #endregion Constructor

        #region Properties

        private List<string> _prodLineList;

        public List<string> ProdLineList
        {
            get { return _prodLineList; }
            set
            {
                _prodLineList = value;
                OnPropertyChanged();
            }
        }

        private string _prodLine;

        public string ProdLine
        {
            get { return _prodLine; }
            set
            {
                _prodLine = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        public ICommand ProdViewCommand => new RelayCommand(x => OpenProdView());

        public ICommand DashViewCommand => new RelayCommand(x => OpenDashView());

        #endregion ICommand Members

        #region Methods

        private void OpenProdView()
        {
            bool _open = false;
            foreach (Window _wnd in Application.Current.Windows)
            {
                if (_wnd is ProductionView)
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
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Production View")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        private void OpenDashView()
        {
            bool _open = false;
            foreach (Window _wnd in Application.Current.Windows)
            {
                if (_wnd is RepDash)
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
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == "Replen Dash")
                    {
                        _wnd.Activate();
                    }
                }
            }
        }

        private void FillProdLines()
        {
            try
            {
                _prodLineList = (from a in new XpresEntities().ProductionAreas
                                 select a.ProdLine).ToList();
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving production line list:" + _ex.Message);
            }
        }

        #endregion Methods
    }
}