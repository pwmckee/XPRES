using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Departments.Replen.Views;

namespace XPRES.Departments.Replen.ViewModels
{
    public class RequestInterfaceVM : INotifyPropertyChanged
    {
        private XpresEntities xps;

        public RequestInterfaceVM()
        {
            FillProdLines();
            SetupTable();
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

        private string _partNum;

        public string PartNumber
        {
            get { return _partNum; }
            set
            {
                _partNum = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        #endregion Properties

        #region ICommand Members

        private ICommand _submitRequestCommand;

        public ICommand SubmitRequestCommand
        {
            get
            {
                if (_submitRequestCommand == null)
                    _submitRequestCommand = new RelayCommand(param => SubmitRequestCommandExecute(param));
                return _submitRequestCommand;
            }
            set
            {
                _submitRequestCommand = value;
            }
        }

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

        private ICommand _editProdListCommand;

        public ICommand EditProdListCommand
        {
            get
            {
                if (_editProdListCommand == null)
                    _editProdListCommand = new RelayCommand(param => EditProdListCommandExecute(param));
                return _editProdListCommand;
            }
            set
            {
                _editProdListCommand = value;
            }
        }

        private ICommand _saveProdListCommand;

        public ICommand SaveProdListCommand
        {
            get
            {
                if (_saveProdListCommand == null)
                    _saveProdListCommand = new RelayCommand(param => SaveProdListCommandExecute(param));
                return _saveProdListCommand;
            }
            set
            {
                _saveProdListCommand = value;
            }
        }

        #endregion ICommand Members

        #region Methods

        #region Command Methods

        private void SaveProdListCommandExecute(object param)
        {
            SaveProdArea();
        }

        private void SaveProdArea()
        {
            try
            {
                xps = new XpresEntities();
                ProductionArea p = new ProductionArea();
                if (_prodLine != null || _prodLine != "")
                    p.ProdLine = _prodLine;
                xps.ProductionAreas.Add(p);
                xps.SaveChanges();
                System.Windows.Forms.MessageBox.Show("Production area successfully saved.");
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd is EditProdAreas)
                    {
                        wnd.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error saving production area: " + ex.Message);
            }
        }

        public void SubmitRequestCommandExecute(object param)
        {
            SubmitRequest();
        }

        private void SubmitRequest()
        {
            if (_dtRequestList.Rows.Count > 0)
            {
                xps = new XpresEntities();
                MaterialRequest _request = new MaterialRequest();

                try
                {
                    var id = (from a in xps.MaterialRequests
                              select a.ID);
                    int _id = 0;

                    if (id.Count() > 0)
                        _id = id.Max();

                    _id++;

                    foreach (DataRow dr in _dtRequestList.Rows)
                    {
                        dr[2] = _prodLine.ToString();
                        dr[3] = DateTime.Now;

                        _request.RequestNum = "Req" + _id.ToString();
                        _request.PartNum = dr[0].ToString();
                        _request.ReqQty = dr[1].ToString();
                        _request.ProdLine = dr[2].ToString();
                        _request.SubTimestamp = Convert.ToDateTime(dr[3]);
                        _request.ReqStatus = "Submitted";
                        xps.MaterialRequests.Add(_request);
                    }
                    xps.SaveChanges();
                    _dtRequestList = new DataTable();
                    System.Windows.Forms.MessageBox.Show("Request successfully saved.");
                    OpenProdView();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error saving request: " + ex.Message);
                }
            }
        }

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
            foreach (Window wnd in Application.Current.Windows)
            {
                if (wnd is RequestInterface)
                {
                    wnd.Close();
                }
            }
        }

        private void EditProdListCommandExecute(object param)
        {
            OpenEditProdArea();
        }

        private void OpenEditProdArea()
        {
            bool _open = false;
            foreach (Window wnd in Application.Current.Windows)
            {
                if (wnd is EditProdAreas)
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
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "Edit Production Areas")
                    {
                        wnd.Activate();
                    }
                }
            }
        }

        #endregion Command Methods

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
            if (_partNum != "" && _partNum != null && _qty != "" && _qty != null)
            {
                DataRow dr = _dtRequestList.NewRow();
                dr[0] = _partNum;
                dr[1] = _qty;

                _dtRequestList.Rows.Add(dr);
                _partNum = "";
                _qty = "";
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