using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Departments.Replen.Views;
using XPRES.Helpers;

namespace XPRES.Departments.Replen.ViewModels
{
    public class RequestInterfaceVM : ViewModelBase
    {
        #region Constructor

        public RequestInterfaceVM()
        {
            FillProdLines();
            SetupTable();
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

        private string _partNum;

        public string PartNumber
        {
            get { return _partNum; }
            set
            {
                _partNum = value;
                OnPropertyChanged();
                FillReqList();
            }
        }

        private string _qty;

        public string Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                OnPropertyChanged();
                FillReqList();
            }
        }

        private DataTable _dtRequestList;

        public DataTable DtRequestList
        {
            get { return _dtRequestList; }
            set
            {
                value = _dtRequestList;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        public ICommand SubmitRequestCommand => new RelayCommand(x => SubmitRequest());

        public ICommand ProdViewCommand => new RelayCommand(x => OpenProdView());

        public ICommand EditProdListCommand => new RelayCommand(x => OpenEditProdArea());

        public ICommand SaveProdListCommand => new RelayCommand(x => SaveProdArea());

        #endregion ICommand Members

        #region Methods

        private void SaveProdArea()
        {
            try
            {
                var _xps = new XpresEntities();
                var _p = new ProductionArea();
                if (_prodLine != null || _prodLine != "")
                    _p.ProdLine = _prodLine;
                _xps.ProductionAreas.Add(_p);
                _xps.SaveChanges();
                System.Windows.Forms.MessageBox.Show(@"Production area successfully saved.");
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd is EditProdAreas)
                    {
                        _wnd.Close();
                    }
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error saving production area: " + _ex.Message);
            }
        }

        private void SubmitRequest()
        {
            if (_dtRequestList.Rows.Count > 0)
            {
                var _xps = new XpresEntities();
                MaterialRequest _request = new MaterialRequest();

                try
                {
                    var _id = (from a in _xps.MaterialRequests
                              select a.ID).Max();
                    _id++;

                    foreach (DataRow _dr in _dtRequestList.Rows)
                    {
                        _dr[2] = _prodLine.ToString();
                        _dr[3] = DateTime.Now;

                        _request.RequestNum = "Req" + _id.ToString();
                        _request.PartNum = _dr[0].ToString();
                        _request.ReqQty = _dr[1].ToString();
                        _request.ProdLine = _dr[2].ToString();
                        _request.SubTimestamp = Convert.ToDateTime(_dr[3]);
                        _request.ReqStatus = "Submitted";
                        _xps.MaterialRequests.Add(_request);
                    }
                    _xps.SaveChanges();
                    _dtRequestList = new DataTable();
                    System.Windows.Forms.MessageBox.Show(@"Request successfully saved.");
                    OpenProdView();
                }
                catch (Exception _ex)
                {
                    System.Windows.Forms.MessageBox.Show(@"Error saving request: " + _ex.Message);
                }
            }
        }

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
                var _prodView = new ProductionView();
                _prodView.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == @"Production View")
                    {
                        _wnd.Activate();
                    }
                }
            }
            foreach (Window _wnd in Application.Current.Windows)
            {
                if (_wnd is RequestInterface)
                {
                    _wnd.Close();
                }
            }
        }

        private void OpenEditProdArea()
        {
            bool _open = false;
            foreach (Window _wnd in Application.Current.Windows)
            {
                if (_wnd is EditProdAreas)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                EditProdAreas _editProdAreas = new EditProdAreas();
                _editProdAreas.Show();
            }
            else
            {
                foreach (Window _wnd in Application.Current.Windows)
                {
                    if (_wnd.Title == @"Edit Production Areas")
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
                _prodLineList = new List<string>();

                var _prodLines = (from a in new XpresEntities().ProductionAreas
                                 select a.ProdLine).ToList();
                foreach (var _p in _prodLines)
                {
                    _prodLineList.Add(_p.ToString());
                }
            }
            catch (Exception _ex)
            {
                System.Windows.Forms.MessageBox.Show(@"Error retrieving production line list:" + _ex.Message);
            }
        }

        private void SetupTable()
        {
            _dtRequestList = new DataTable();
            _dtRequestList.Columns.Add("PartNum", typeof(string));
            _dtRequestList.Columns.Add("Qty", typeof(string));
            _dtRequestList.Columns.Add("Line", typeof(string));
            _dtRequestList.Columns.Add("SubmittedTimestamp", typeof(DateTime));
        }

        private void FillReqList()
        {
            if (string.IsNullOrEmpty(_partNum) && string.IsNullOrEmpty(_qty))
            {
                DataRow _dr = _dtRequestList.NewRow();
                _dr[0] = _partNum;
                _dr[1] = _qty;

                _dtRequestList.Rows.Add(_dr);
                _partNum = "";
                _qty = "";
            }
        }

        #endregion Methods

    }
}