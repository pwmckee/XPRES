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
    public class DynamicMetricVm : ViewModelBase
    {
        private string _ctrlType = string.Empty;

        public DynamicMetricVm()
        {
            _mainMetric = new ObservableCollection<DynamicMetric>();
            _subMetric = new ObservableCollection<DynamicSubMetric>();
            _micMetric = new ObservableCollection<DynamicMicro>();
            _dynCtrlInfos = new List<DynCtrlInfo>();
        }

        #region Properties

        #region Edit Ctrl Properties

        private ObservableCollection<DynamicMetric> _mainMetric;

        public ObservableCollection<DynamicMetric> MainMetric => _mainMetric ?? (_mainMetric = new ObservableCollection<DynamicMetric>());

        private ObservableCollection<DynamicSubMetric> _subMetric;

        public ObservableCollection<DynamicSubMetric> SubMetric => _subMetric ?? (_subMetric = new ObservableCollection<DynamicSubMetric>());

        private ObservableCollection<DynamicMicro> _micMetric;

        public ObservableCollection<DynamicMicro> MicMetric => _micMetric ?? (_micMetric = new ObservableCollection<DynamicMicro>());

        private List<DynCtrlInfo> _dynCtrlInfos;

        public List<DynCtrlInfo> DynCtrlInfos
        {
            get { return _dynCtrlInfos; }
            set
            {
                _dynCtrlInfos = value;
                OnPropertyChanged();
            }
        }

        private string _txtVal;

        public string TxtVal
        {
            get { return _txtVal; }
            set
            {
                _txtVal = value;
                OnPropertyChanged();
            }
        }

        private string _treeLvl;

        public string TreeLevel
        {
            get { return _treeLvl; }
            set
            {
                _treeLvl = value;
                OnPropertyChanged();
            }
        }

        private string _parent;

        public string Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged();
            }
        }

        private string _ctrlName;

        public string CtrlName
        {
            get { return _ctrlName; }
            set
            {
                _ctrlName = value;
                OnPropertyChanged();
            }
        }

        private string _catName;

        public string CatName
        {
            get { return _catName; }
            set
            {
                _catName = value;
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

        #endregion Edit Ctrl Properties

        #endregion Properties

        #region ICommand Members

        #region MainCategory Commands

        public ICommand AddMainCategory { get { _ctrlType = "Main"; return new RelayCommand(x => AddMainControl(_ctrlType)); } }

        public ICommand FinalizeMainCategory { get { _ctrlType = "Main"; return new RelayCommand(x => FinalizeMainControl(_ctrlType)); } }

        #endregion MainCategory Commands

        #region SubCategory Commands

        public ICommand AddSubCategory
        {
            get
            {
                _ctrlType = "Sub";
                return new RelayCommand(x => AddSubControl());
            }
        }

        public ICommand AddSubSumCategory
        {
            get
            {
                _ctrlType = "SubSum";
                return new RelayCommand(x => AddSubSumControl());
            }
        }

        public ICommand FinalizeSubCategory
        {
            get
            {
                _ctrlType = "Main";
                return new RelayCommand(x => FinalizeMicList());
            }
        }

        #endregion SubCategory Commands

        #region MicCategory Commands

        public ICommand AddMicCategory
        {
            get
            {
                _ctrlType = "Mic";
                return new RelayCommand(x => AddMicControl());
            }
        }

        public ICommand AddMicSumCategory
        {
            get
            {
                _ctrlType = "MicSum";
                return new RelayCommand(x => AddMicSumControl());
            }
        }

        #endregion MicCategory Commands

        #endregion ICommand Members

        #region Methods

        #region Main Methods

        public void LoadSets(DateTime selDate, string ctrlParent, string ctrlType)
        {
            try
            {
                XpresEntities _xps = new XpresEntities();

                List<DynamicInbMetric> _ctrlList = (from _a in _xps.DynamicInbMetrics
                                                    where _a.Date == selDate && _a.Parent == ctrlParent && _a.CategoryType == ctrlType
                                                    orderby _a.CategoryTreeLevel ascending
                                                    select _a).ToList();
                if (_ctrlList.Count <= 0) return;
                foreach (DynamicInbMetric _c in _ctrlList)
                {
                    if (ctrlType == "Main")
                    {
                        _catName = _c.CategoryName.Trim();
                        _treeLvl = _c.CategoryTreeLevel.ToString();
                        DynamicMetric _dynMain = new DynamicMetric
                        {
                            TxbTreeLevel = { Text = _c.CategoryTreeLevel.ToString() },
                            TxbCtrlParent = { Text = ctrlParent.Trim() },
                            DataContext = this
                        };
                        LoadSets(selDate, _c.CategoryName, "Sub");
                        LoadSets(selDate, _c.CategoryName, "SubSum");
                        _mainMetric.Add(_dynMain);
                    }
                    else
                    {
                        DynamicSubMetric _dynSub;
                        if (ctrlType == "Sub")
                        {
                            DynamicMetricVm _dynVm = new DynamicMetricVm { _catName = _c.CategoryName.Trim() };
                            _dynSub = new DynamicSubMetric
                            {
                                DataContext = _dynVm,
                                TxbTreeLevel = { Text = _c.CategoryTreeLevel.ToString() },
                                TxbCtrlParent = { Text = ctrlParent.Trim() },
                                GrdView = { Visibility = Visibility.Visible },
                                GrdAlter = { Visibility = Visibility.Hidden }
                            };
                            _subMetric.Add(_dynSub);
                            _dynVm.LoadSets(selDate, _c.CategoryName, "Mic");
                            _dynVm.LoadSets(selDate, _c.CategoryName, "MicSum");
                        }
                        else
                        {
                            string[] _sumCatArr;
                            string _sumConc;
                            if (ctrlType == "SubSum")
                            {
                                _dynSub = new DynamicSubMetric
                                {
                                    TxbTreeLevel = { Text = _c.CategoryTreeLevel.ToString() },
                                    TxbCtrlParent = { Text = ctrlParent.Trim() },
                                    GrdView = { Visibility = Visibility.Hidden },
                                    GrdAlter = { Visibility = Visibility.Hidden },
                                    GrdSum = { Visibility = Visibility.Visible }
                                };
                                _sumConc = _c.CategoryName.Trim();
                                _sumCatArr = _sumConc.Split('_');
                                foreach (string _s in _sumCatArr)
                                {
                                    _dynSub.CboSum.Items.Add(_s);
                                }
                                _subMetric.Add(_dynSub);
                            }
                            else
                            {
                                DynamicMicro _dynMic;
                                switch (ctrlType)
                                {
                                    case "Mic":
                                        DynamicMetricVm _dynVm = new DynamicMetricVm { _catName = _c.CategoryName.Trim() };
                                        _dynMic = new DynamicMicro
                                        {
                                            DataContext = _dynVm,
                                            TxbTreeLevel = { Text = _c.CategoryTreeLevel.ToString() },
                                            TxbCtrlParent = { Text = ctrlParent.Trim() },
                                            GrdAlter = { Visibility = Visibility.Hidden },
                                            GrdView = { Visibility = Visibility.Visible }
                                        };
                                        _micMetric.Add(_dynMic);
                                        break;

                                    case "MicSum":
                                        _dynMic = new DynamicMicro
                                        {
                                            TxbTreeLevel = { Text = _c.CategoryTreeLevel.ToString() },
                                            TxbCtrlParent = { Text = ctrlParent.Trim() },
                                            GrdView = { Visibility = Visibility.Hidden },
                                            GrdAlter = { Visibility = Visibility.Hidden },
                                            GrdSum = { Visibility = Visibility.Visible }
                                        };
                                        _sumConc = _c.CategoryName.Trim();
                                        _sumCatArr = _sumConc.Split('_');
                                        foreach (string _s in _sumCatArr)
                                        {
                                            _dynMic.CboSum.Items.Add(_s);
                                        }

                                        _micMetric.Add(_dynMic);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error loading main dynamic metric set: " + _ex);
            }
        }

        #endregion Main Methods

        #region Edit Ctrl Methods

        #region MainCategory Methods

        private void AddMainControl(string ctrlType)
        {
            int _ctrlId = 1;
            if (_mainMetric.Count > 0)
            {
                _ctrlId += _mainMetric.Count;
                foreach (DynamicMetric _d in _mainMetric)
                {
                    if (_d.Visibility == Visibility.Collapsed)
                    {
                        _ctrlId--;
                    }
                }
            }
            DynamicMetric _mainMetricCtrl = new DynamicMetric
            {
                TxbCtrlParent = { Text = ctrlType },
                TxbTreeLevel = { Text = _ctrlId.ToString() },
                Name = _catName + "_" + _ctrlId
            };
            _mainMetric.Add(_mainMetricCtrl);
        }

        private void FinalizeMainControl(string ctrlType)
        {
            try
            {
                int _ctrlCount = 0;
                if (_catName == "")
                {
                    MessageBox.Show(@"Please enter a category title first.");
                    return;
                }

                List<DynamicInbMetric> _dynList = (from _a in new XpresEntities().DynamicInbMetrics
                                                   where _a.CategoryType == ctrlType
                                                   select _a).ToList();
                XpresEntities _xps = new XpresEntities();
                foreach (DynamicInbMetric _a in _dynList)
                {
                    if (_a.CategoryTreeLevel != Convert.ToInt32(_treeLvl)) continue;
                    _ctrlCount = 0;
                    _a.CategoryName = _catName.Trim();
                    _a.CategoryType = ctrlType.Trim();
                    _xps.SaveChanges();
                    MessageBox.Show(@"Metric Updated");
                    foreach (DynamicSubMetric _dyn in _subMetric)
                    {
                        if (_dyn.Visibility != Visibility.Visible) continue;
                        _ctrlCount++;
                        if (_dyn.CboSum.Items.Count > 0)
                            FinalizeSubSumControl("SubSum", _dyn, _ctrlCount);
                        else
                            FinalizeSubControl("Sub", _dyn, _ctrlCount);
                    }
                    return;
                }
                _xps.DynamicInbMetrics.Add(new DynamicInbMetric
                {
                    Date = DateTime.Now.Date,
                    CategoryName = _catName.Trim(),
                    CategoryType = ctrlType.Trim(),
                    CategoryTreeLevel = Convert.ToInt32(_treeLvl),
                    State = true,
                    Parent = _parent.Trim()
                });
                _xps.SaveChanges();
                if (_subMetric.Count > 0)
                {
                    foreach (DynamicSubMetric _dyn in _subMetric)
                    {
                        if (_dyn.Visibility == Visibility.Visible)
                        {
                            _ctrlCount++;
                            if (_dyn.CboSum.Items.Count > 0)
                            {
                                FinalizeSubSumControl("SubSum", _dyn, _ctrlCount);
                            }
                            else
                            {
                                FinalizeSubControl("Sub", _dyn, _ctrlCount);
                            }
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error saving category info: " + _ex.Message);
            }
            MessageBox.Show(@"New metric successfully saved");
        }

        #endregion MainCategory Methods

        #region SubCategory Methods

        private void AddSubControl()
        {
            if (_catName == "")
            {
                MessageBox.Show(@"Please enter the name of the Main Category first.");
                return;
            }
            int _ctrlId = 1;
            if (_subMetric.Count > 0)
            {
                _ctrlId += _subMetric.Count;
                foreach (DynamicSubMetric _d in _subMetric)
                {
                    if (_d.Visibility == Visibility.Collapsed)
                    {
                        _ctrlId--;
                    }
                }
            }
            DynamicSubMetric _subMetricCtrl = new DynamicSubMetric
            {
                TxbCtrlParent = { Text = _catName.Trim() },
                TxbTreeLevel = { Text = _ctrlId.ToString() },
                Name = _catName + "_" + _ctrlId
            };
            _subMetric.Add(_subMetricCtrl);
        }

        private void FinalizeSubControl(string ctrlType, DynamicSubMetric subMetric, int subTreelvl)
        {
            try
            {
                if (_catName == "")
                {
                    MessageBox.Show(@"Please enter a category title first.");
                    return;
                }

                XpresEntities _xps = new XpresEntities();

                List<DynamicInbMetric> _dynList = (from _a in _xps.DynamicInbMetrics
                                                   where _a.Parent.Trim() == _catName.Trim()
                                                   select _a).ToList();

                foreach (DynamicInbMetric _a in _dynList)
                {
                    if (_a.CategoryTreeLevel != subTreelvl || _a.Parent != subMetric.TxbCtrlParent.Text) continue;
                    _a.CategoryName = subMetric.TxtSubCat.Text.Trim();
                    _a.CategoryType = ctrlType.Trim();
                    _a.CategoryTreeLevel = subTreelvl;
                    _a.State = true;
                    _xps.SaveChanges();
                    MessageBox.Show(@"Metric Updated");
                    return;
                }

                DynamicInbMetric _d = new DynamicInbMetric
                {
                    Date = DateTime.Now.Date,
                    CategoryName = subMetric.TxtSubCat.Text.Trim(),
                    CategoryType = ctrlType.Trim(),
                    CategoryTreeLevel = subTreelvl,
                    State = true,
                    Parent = _catName
                };

                _xps.DynamicInbMetrics.Add(_d);
                _xps.SaveChanges();
                MessageBox.Show(@"New metric successfully saved");
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error saving category info: " + _ex.Message);
            }
        }

        private void AddSubSumControl()
        {
            _sumCats = new List<string>();
            if (_catName == "")
            {
                MessageBox.Show(@"Please enter the name of the Main Category first.");
                return;
            }
            int _ctrlId = 1;
            if (_subMetric.Count > 1)
            {
                _ctrlId += _subMetric.Count;
                foreach (DynamicSubMetric _d in _subMetric)
                {
                    if (_d.CbxSum.IsChecked == true)
                    {
                        if (_d.LblSubCat.Content == null || _d.LblSubCat.Content.ToString() == "Sub Category" || _d.LblSubCat.Content.ToString() == "")
                        {
                            MessageBox.Show(@"Categories must be named before being added to the sum list.");
                            return;
                        }
                        _sumCats.Add(_d.LblSubCat.Content.ToString().Trim());
                    }
                    else
                    {
                        _ctrlId--;
                    }
                }
            }
            else
            {
                MessageBox.Show(@"There must be at least two sub categories already available to create a sum box.");
                return;
            }
            if (_sumCats.Count < 2)
            {
                MessageBox.Show(@"There must be at least two sub categories selected to create a sum box.");
                return;
            }
            DynamicSubMetric _subMetricCtrl = new DynamicSubMetric
            {
                CboSum = { ItemsSource = _sumCats },
                TxbCtrlParent = { Text = _catName.Trim() },
                TxbTreeLevel = { Text = _ctrlId.ToString() },
                Name = _catName + "_" + _ctrlId,
                GrdSum = { Visibility = Visibility.Visible },
                GrdView = { Visibility = Visibility.Hidden },
                GrdAlter = { Visibility = Visibility.Hidden }
            };
            _subMetric.Add(_subMetricCtrl);
        }

        private void FinalizeSubSumControl(string ctrlType, DynamicSubMetric subMetric, int subTreelvl)
        {
            try
            {
                if (_sumCats.Count < 2)
                {
                    MessageBox.Show(@"There must be at least two items to be summed in this list.");
                    return;
                }

                XpresEntities _xps = new XpresEntities();

                List<DynamicInbMetric> _dynList = (from _a in _xps.DynamicInbMetrics
                                                   where _a.Parent.Trim() == _catName.Trim()
                                                   select _a).ToList();

                string _sumCatString = string.Empty;

                foreach (DynamicInbMetric _a in _dynList)
                {
                    if (_a.CategoryTreeLevel == subTreelvl && _a.Parent.Trim() == subMetric.TxbCtrlParent.Text.Trim())
                    {
                        foreach (string _s in subMetric.CboSum.Items)
                        {
                            _sumCatString += _s + "_";
                        }
                        _a.CategoryName = _sumCatString.Trim();
                        _a.CategoryType = ctrlType.Trim();
                        _a.CategoryTreeLevel = subTreelvl;
                        _a.State = true;
                        _xps.SaveChanges();
                        MessageBox.Show(@"Metric Updated");
                        return;
                    }
                }

                DynamicInbMetric _d = new DynamicInbMetric();
                foreach (string _s in subMetric.CboSum.Items)
                {
                    _sumCatString += _s + "_";
                }
                _d.CategoryName = _sumCatString.Trim();
                _d.CategoryType = ctrlType.Trim();
                _d.Date = DateTime.Now.Date;
                _d.CategoryTreeLevel = subTreelvl;
                _d.State = true;
                _d.Parent = _catName.Trim();
                _xps.DynamicInbMetrics.Add(_d);
                _xps.SaveChanges();
                MessageBox.Show(@"New metric successfully saved");
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error saving category info: " + _ex.Message);
            }
        }

        #endregion SubCategory Methods

        #region MicCategory Methods

        private void FinalizeMicList()
        {
            if (_micMetric.Count > 0)
            {
                int _ctrlCount = 0;
                foreach (DynamicMicro _dyn in _micMetric)
                {
                    if (_dyn.Visibility != Visibility.Visible) continue;
                    _ctrlCount++;
                    if (_dyn.CboSum.Items.Count > 0)
                        FinalizeMicSumControl("MicSum", _dyn, _ctrlCount);
                    else
                        FinalizeMicControl("Mic", _dyn, _ctrlCount);
                }
            }
        }

        private void AddMicControl()
        {
            if (_catName == "")
            {
                MessageBox.Show(@"Please enter the name of the Main Category first.");
                return;
            }
            int _ctrlId = 1;
            if (_micMetric.Count > 0)
            {
                _ctrlId += _micMetric.Count;
                foreach (DynamicMicro _d in _micMetric)
                {
                    if (_d.Visibility == Visibility.Collapsed)
                        _ctrlId--;
                }
            }
            DynamicMicro _micMetricCtrl = new DynamicMicro
            {
                TxbCtrlParent = { Text = _catName.Trim() },
                TxbTreeLevel = { Text = _ctrlId.ToString() },
                Name = _catName.Trim() + "_" + _ctrlId
            };
            _micMetric.Add(_micMetricCtrl);
        }

        private void FinalizeMicControl(string ctrlType, DynamicMicro micMetric, int subTreelvl)
        {
            try
            {
                if (_catName == "")
                {
                    MessageBox.Show(@"Please enter a category title first.");
                    return;
                }

                XpresEntities _xps = new XpresEntities();

                List<DynamicInbMetric> _dynList = (from _a in _xps.DynamicInbMetrics
                                                   where _a.Parent.Trim() == _catName.Trim()
                                                   select _a).ToList();

                foreach (DynamicInbMetric _a in _dynList)
                {
                    if (_a.CategoryTreeLevel != subTreelvl || _a.Parent.Trim() != micMetric.TxbCtrlParent.Text.Trim()) continue;
                    _a.CategoryName = micMetric.TxtMicroCat.Text.Trim();
                    _a.CategoryType = ctrlType.Trim();
                    _a.CategoryTreeLevel = subTreelvl;
                    _a.State = true;
                    _xps.SaveChanges();
                    MessageBox.Show(@"Metric Updated");
                    return;
                }

                DynamicInbMetric _d = new DynamicInbMetric
                {
                    Date = DateTime.Now.Date,
                    CategoryName = micMetric.TxtMicroCat.Text.Trim(),
                    CategoryType = ctrlType.Trim(),
                    CategoryTreeLevel = subTreelvl,
                    State = true,
                    Parent = _catName.Trim()
                };

                _xps.DynamicInbMetrics.Add(_d);
                _xps.SaveChanges();
                MessageBox.Show(@"New metric successfully saved");
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error saving category info: " + _ex.Message);
            }
        }

        private void AddMicSumControl()
        {
            _sumCats = new List<string>();
            if (_catName == "")
            {
                MessageBox.Show(@"Please enter the name of the Main Category first.");
                return;
            }
            int _ctrlId = 1;
            if (_micMetric.Count > 1)
            {
                _ctrlId += _micMetric.Count;
                foreach (DynamicMicro _d in _micMetric)
                {
                    if (_d.CbxSum.IsChecked == true)
                    {
                        if (_d.LblMicroCat.Content == null || _d.LblMicroCat.Content.ToString() == "Sub Category" || _d.LblMicroCat.Content.ToString() == "")
                        {
                            MessageBox.Show(@"Categories must be named before being added to the sum list.");
                            return;
                        }
                        _sumCats.Add(_d.LblMicroCat.Content.ToString());
                    }
                    else
                    {
                        _ctrlId--;
                    }
                }
            }
            else
            {
                MessageBox.Show(@"there must be at least two sub categories already available to create a sum box.");
                return;
            }
            if (_sumCats.Count < 2)
            {
                MessageBox.Show(@"there must be at least two sub categories selected to create a sum box.");
                return;
            }
            DynamicMicro _micMetricCtrl = new DynamicMicro
            {
                CboSum = { ItemsSource = _sumCats },
                TxbCtrlParent = { Text = _catName.Trim() },
                TxbTreeLevel = { Text = _ctrlId.ToString() },
                Name = _catName.Trim() + "_" + _ctrlId,
                GrdSum = { Visibility = Visibility.Visible },
                GrdAlter = { Visibility = Visibility.Hidden }
            };
            _micMetric.Add(_micMetricCtrl);
        }

        private void FinalizeMicSumControl(string ctrlType, DynamicMicro micMetric, int subTreelvl)
        {
            try
            {
                if (_sumCats.Count < 2)
                {
                    MessageBox.Show(@"There must be at least two items to be summed in this list.");
                    return;
                }

                XpresEntities _xps = new XpresEntities();

                List<DynamicInbMetric> _dynList = (from _a in _xps.DynamicInbMetrics
                                                   where _a.Parent.Trim() == _catName.Trim()
                                                   select _a).ToList();

                string _sumCatString = string.Empty;

                foreach (DynamicInbMetric _a in _dynList)
                {
                    if (_a.CategoryTreeLevel != subTreelvl || _a.Parent != micMetric.TxbCtrlParent.Text) continue;
                    foreach (string _s in micMetric.CboSum.Items)
                    {
                        _sumCatString += _s + "_";
                    }
                    _a.CategoryName = _sumCatString.Trim();
                    _a.CategoryType = ctrlType.Trim();
                    _xps.SaveChanges();
                    MessageBox.Show(@"Metric Updated");
                    return;
                }

                DynamicInbMetric _d = new DynamicInbMetric();
                foreach (string _s in micMetric.CboSum.Items)
                {
                    _sumCatString += _s + "_";
                }
                _d.CategoryName = _sumCatString.Trim();
                _d.CategoryType = ctrlType.Trim();
                _d.Date = DateTime.Now.Date;
                _d.CategoryTreeLevel = subTreelvl;
                _d.State = true;
                _d.Parent = _catName.Trim();
                _xps.DynamicInbMetrics.Add(_d);
                _xps.SaveChanges();
                MessageBox.Show(@"New metric successfully saved");
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error saving category info: " + _ex.Message);
            }
        }

        #endregion MicCategory Methods

        #endregion Edit Ctrl Methods

        #endregion Methods
    }

    public class DynCtrlInfo
    {
        public string CtrlParent { get; set; }
        public string CtrlTreeLvl { get; set; }
        public string CtrlCatName { get; set; }
        public string CtrlType { get; set; }
        public bool CtrlState { get; set; }
    }
}