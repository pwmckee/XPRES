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
using MessageBox = System.Windows.Forms.MessageBox;

namespace XPRES.Departments.Inbound.ViewModels
{
    public class DynCtrlVm : ViewModelBase
    {
        #region Constructor

        public DynCtrlVm()
        {
            _dynCtrls = new ObservableCollection<DynCtrl>();
            _ctrlInfoSet = new List<DynamicInbMetric>();
        }

        #endregion Constructor

        #region Properties

        private List<DynamicInbMetric> _ctrlInfoSet;

        private ObservableCollection<DynCtrl> _dynCtrls;

        public ObservableCollection<DynCtrl> DynCtrls => _dynCtrls ?? (_dynCtrls = new ObservableCollection<DynCtrl>());

        private string _ctrlParent;

        public string CtrlParent
        {
            get { return _ctrlParent; }
            set
            {
                _ctrlParent = value;
                OnPropertyChanged();
            }
        }

        private string _ctrlTreeLvl;

        public string CtrlTreeLvl
        {
            get { return _ctrlTreeLvl; }
            set
            {
                _ctrlTreeLvl = value;
                OnPropertyChanged();
            }
        }

        private string _ctrlCatName;

        public string CtrlCatName
        {
            get { return _ctrlCatName; }
            set
            {
                _ctrlCatName = value;
                OnPropertyChanged();
            }
        }

        private string _ctrlIsSum;

        public string CtrlIsSum
        {
            get { return _ctrlIsSum; }
            set
            {
                _ctrlIsSum = value;
                OnPropertyChanged();
            }
        }

        private List<string> _sumCats;

        public List<string> SumCats
        {
            get { return _sumCats; }
            set
            {
                _sumCats = value;
                OnPropertyChanged();
            }
        }
        
        #endregion Properties

        #region ICommand Members

        public ICommand AddChildCommand { get { bool IsSum = false; return new RelayCommand(x => AddChildCtrl(IsSum)); } }

        public ICommand AddSumChildCommand { get { bool IsSum = true; return new RelayCommand(x => AddChildCtrl(IsSum)); } }

        public ICommand SaveCtrlsCommand { get { return new RelayCommand(x => SaveCtrls("Date")); } }

        #endregion ICommand Members

        #region Methods
        
        private void AddChildCtrl(bool isSum)
        {
            int _ctrlIdNum = 1;
            if (_ctrlCatName == "")
            {
                MessageBox.Show(@"Please enter the name of the Main Category first.");
                return;
            }

            if (_dynCtrls.Any())
            {
                _ctrlIdNum += _dynCtrls.Count;
                foreach (DynCtrl _ctrl in _dynCtrls)
                {
                    if (_ctrl.Visibility == Visibility.Collapsed) _ctrlIdNum--;
                }
            }
            if (isSum)
            {
                if (_dynCtrls.Count > 1)
                {
                    _sumCats = new List<string>();
                    foreach (DynCtrl _d in _dynCtrls)
                    {
                        if (_d.CbxSum.IsChecked == true)
                        {
                            if (string.IsNullOrEmpty(_d.LblSubCat.Content.ToString()))
                            {
                                MessageBox.Show(@"Categories must be named before being added to the sum list.");
                                return;
                            }
                            _sumCats.Add(_d.LblSubCat.Content.ToString().Trim());
                        }
                        else _ctrlIdNum--;
                    }

                }
                else
                {
                    MessageBox.Show(@"There must be at least two sub categories selected to create a sum box.");
                    return;
                }
                string _sumNames = "";
                foreach (string _s in _sumCats)
                {
                    if (!string.IsNullOrEmpty(_sumNames)) _sumNames += "_";
                    _sumNames += _s;
                }
                DynCtrl _subMetricCtrl = new DynCtrl
                {
                    CboSum = { ItemsSource = _sumCats },
                    TxbCtrlParent = { Text = _ctrlCatName },
                    TxbTreeLevel = { Text = _ctrlIdNum.ToString() },
                    TxbCtrlName = {Text = _sumNames},
                    TxbCtrlIsSum = { Text = "true"},
                    GrdSum = { Visibility = Visibility.Visible },
                    GrdView = { Visibility = Visibility.Hidden },
                    GrdAlter = { Visibility = Visibility.Hidden }
                };
                _dynCtrls.Add(_subMetricCtrl);
            }
            else
            {
                DynCtrl _subMetricCtrl = new DynCtrl
                {
                    TxbCtrlIsSum = { Text = "false" },
                    TxbCtrlParent = { Text = _ctrlCatName.Trim() },
                    TxbTreeLevel = { Text = _ctrlIdNum.ToString() }
                };
                _dynCtrls.Add(_subMetricCtrl);
            }
        }

        private void SaveCtrls(string parentName)
        {
            try
            {
                XpresEntities _xps = new XpresEntities();
                string _ctrlId = CtrlParent + CtrlTreeLvl;
                
                if (!string.IsNullOrEmpty(CtrlCatName))
                {
                    try
                    {
                        DynamicInbMetric _oldEntry = (from _a in _xps.DynamicInbMetrics
                                            where _a.CtrlId == _ctrlId
                                            select _a).SingleOrDefault();
                        if (string.IsNullOrEmpty(parentName)) parentName = "Date";

                        if (_oldEntry != null)
                        {
                            _oldEntry.CategoryName = CtrlCatName;
                            _oldEntry.Parent = parentName;
                        }
                        else
                        {
                            DynamicInbMetric _newCtrl = new DynamicInbMetric
                            {
                                Date = DateTime.Now.Date,
                                CtrlId = _ctrlId,
                                CategoryName = CtrlCatName,
                                CategoryTreeLevel = Convert.ToInt32(CtrlTreeLvl),
                                Parent = CtrlParent,
                                State = true,
                                IsSum = CtrlIsSum == "true" ? true : false
                            };

                            _xps.DynamicInbMetrics.Add(_newCtrl);
                        }
                        _xps.SaveChanges();
                    }
                    catch { /*ignore*/ }
                }
               
                if (!_dynCtrls.Any()) return;
                foreach (DynCtrl _ctrl in _dynCtrls)
                {
                    if (_ctrl.Visibility == Visibility.Visible)
                    {
                        DynCtrlVm _vm = _ctrl.DataContext as DynCtrlVm;
                        _vm?.SaveCtrls(CtrlCatName);
                    }
                    else
                    {
                        try
                        {
                            var _delEntry = (from _a in _xps.DynamicInbMetrics
                                         where _a.CtrlId == _ctrlId
                                         select _a).SingleOrDefault();
                            _xps.DynamicInbMetrics.Remove(_delEntry);
                        }
                        catch { /*ignore*/ }
                        _xps.SaveChanges();
                    }
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error saving metric ctrl " + CtrlCatName + @": " + _ex.Message);
            }
        }

        public void LoadCtrlSet()
        {
            try
            {
                _ctrlInfoSet = (from _a in new XpresEntities().DynamicInbMetrics
                            where _a.State == true orderby _a.CategoryTreeLevel
                            select _a).ToList();
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error retrieving metrics control set: " + _ex.Message);
                return;
            }

            List<DynamicInbMetric> _rootInfoSet = _ctrlInfoSet.Where(x => x.Parent == "Date").ToList();
            LoadDynCtrls(_rootInfoSet, _ctrlInfoSet);
        }

        private void LoadDynCtrls(List<DynamicInbMetric> ctrlInfoSet, List<DynamicInbMetric> mainInfoSet)
        {
            if (!ctrlInfoSet.Any()) return;          

            foreach (DynamicInbMetric _ctrlInfo in ctrlInfoSet)
            {
                DynCtrlVm _vm = new DynCtrlVm();
                string _catName = _ctrlInfo.CategoryName;

                DynCtrl _newDynCtrl = new DynCtrl
                {
                    DataContext = _vm,
                    TxbCtrlName = {Text = _catName },
                    TxbCtrlParent = {Text = _ctrlInfo.Parent},
                    TxbTreeLevel = {Text = _ctrlInfo.CategoryTreeLevel.ToString()},
                    GrdAlter = {Visibility = Visibility.Hidden},
                    GrdView = {Visibility = Visibility.Visible}
                };

                if (_ctrlInfo.IsSum == true)
                {
                    _newDynCtrl.TxbCtrlIsSum.Text = "true";
                    _newDynCtrl.GrdView.Visibility = Visibility.Hidden;
                    _newDynCtrl.GrdSum.Visibility = Visibility.Visible;
                    string[] _sumArr = _catName.Split('_');
                    foreach (string _s in _sumArr) _newDynCtrl.CboSum.Items.Add(_s);
                }

                List<DynamicInbMetric> _branchSet = mainInfoSet.Where(x => x.Parent == _catName).ToList();

                if(_branchSet != null || _branchSet.Any()) _vm.LoadDynCtrls(_branchSet, mainInfoSet);

                _dynCtrls.Add(_newDynCtrl);
            }
        }

        #endregion Methods
    }
}