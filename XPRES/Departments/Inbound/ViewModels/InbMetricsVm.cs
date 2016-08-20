using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.DAL;
using XPRES.Departments.Inbound.Controls;
using XPRES.Helpers;
using MessageBox = System.Windows.Forms.MessageBox;

namespace XPRES.Departments.Inbound.ViewModels
{
    public class InbMetricsVm : INotifyPropertyChanged
    {
        private string[] _sumCatArr;

        public InbMetricsVm()
        {
            _editDateWrap = new ObservableCollection<DynContainer>();
        }

        #region Properties

        private string _mainEntryWrap;
        private string _subEntryWrap;
        private string _micEntryWrap;

        private string _selDate;

        public string SelDate
        {
            get { return _selDate; }
            set
            {
                _selDate = value;
                OnPropertyChanged();
            }
        }

        private string _entryDate;

        public string EntryDate
        {
            get { return _entryDate; }
            set
            {
                _entryDate = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DynContainer> _editDateWrap;

        public ObservableCollection<DynContainer> EditDateWrap
        {
            get { return _editDateWrap; }
            set
            {
                _editDateWrap = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MetricViewCat1> _metricsCat1Wrap;

        public ObservableCollection<MetricViewCat1> MetricsCat1Wrap
        {
            get
            {
                if (_metricsCat1Wrap == null)
                {
                    _metricsCat1Wrap = new ObservableCollection<MetricViewCat1>();
                }
                return _metricsCat1Wrap;
            }
        }

        private ObservableCollection<MetricViewCat2> _metricsCat2Wrap;

        public ObservableCollection<MetricViewCat2> MetricsCat2Wrap
        {
            get
            {
                if (_metricsCat2Wrap == null)
                {
                    _metricsCat2Wrap = new ObservableCollection<MetricViewCat2>();
                }
                return _metricsCat2Wrap;
            }
        }

        private ObservableCollection<MetricViewCat3> _metricsCat3Wrap;

        public ObservableCollection<MetricViewCat3> MetricsCat3Wrap
        {
            get
            {
                if (_metricsCat3Wrap == null)
                {
                    _metricsCat3Wrap = new ObservableCollection<MetricViewCat3>();
                }
                return _metricsCat3Wrap;
            }
        }

        private ObservableCollection<MetricView> _metricsDateWrap;

        public ObservableCollection<MetricView> MetricsDateWrap
        {
            get
            {
                if (_metricsDateWrap == null)
                {
                    _metricsDateWrap = new ObservableCollection<MetricView>();
                }
                return _metricsDateWrap;
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

        #endregion Properties

        #region ICommand Members

        #region Enter Metrics Commands

        public ICommand AddDateCommand { get { return new RelayCommand(x => AddDateControl()); } }

        public ICommand EditDateCommand { get { return new RelayCommand(x => EditDateControl()); } }

        public ICommand SaveDateCommand { get { return new RelayCommand(x => SaveDateControlV2()); } }

        public ICommand GetSubSumCommand { get { return new RelayCommand(x => GetSubSum()); } }

        public ICommand GetMicSumCommand { get { return new RelayCommand(x => GetMicSum()); } }

        public ICommand ViewDateCommand { get { return new RelayCommand(x => ViewDateControl()); } }
        
        #endregion Enter Metrics Commands

        #region Edit Layout Commands

        public ICommand LoadSetCommand { get { return new RelayCommand(x => LoadSetV2()); } }

        #endregion Edit Layout Commands

        #endregion ICommand Members

        #region Methods

        #region Main Methods

        private void GetSubSum()
        {
            double _numValSum = 0;
            if (_metricsCat2Wrap == null) return;
            foreach (string _s in _sumCatArr)
            {
                foreach (MetricViewCat2 _m in _metricsCat2Wrap)
                {
                    if (_m.TxtCatName.Text == _s)
                    {
                        try
                        {
                            _numValSum += Convert.ToDouble(_m.TxtVal.Text);
                        }
                        catch
                        {
                            MessageBox.Show(@"Category: " + _m.TxtCatName + @" is designated to be a number to be summed.  Please enter a number or edit the control layout");
                            return;
                        }
                    }
                }
            }
            foreach (MetricViewCat2 _m in _metricsCat2Wrap)
            {
                if (_m.TxtNumVal.Visibility == Visibility.Visible)
                {
                    _m.TxtNumVal.Text = _numValSum.ToString();
                }
            }
        }

        private void GetMicSum()
        {
            double _numValSum = 0;
            if (_metricsCat3Wrap != null)
            {
                foreach (string _s in _sumCatArr)
                {
                    foreach (MetricViewCat3 _m in _metricsCat3Wrap)
                    {
                        if (_m.TxtCatName.Text == _s)
                        {
                            try
                            {
                                _numValSum += Convert.ToDouble(_m.TxtVal.Text);
                            }
                            catch
                            {
                                MessageBox.Show(@"Category: " + _m.TxtCatName + @" is designated to be a number to be summed.  Please enter a number or edit the control layout");
                                return;
                            }
                        }
                    }
                }
                foreach (MetricViewCat3 _m in _metricsCat3Wrap)
                {
                    if (_m.TxtNumVal.Visibility == Visibility.Visible)
                    {
                        _m.TxtNumVal.Text = _numValSum.ToString();
                    }
                }
            }
        }

        private void LoadSetV2()
        {
            DynCtrlVm _dynVm = new DynCtrlVm
            {
                CtrlCatName = "Date"
            };
            DynContainer _dynDate = new DynContainer
            {
                DataContext = _dynVm,
            };

            _dynVm.LoadCtrlSet();
            _editDateWrap.Add(_dynDate);
        }
        
        private void LoadSet()
        {
            try
            {
                XpsDates _xpsDates = new XpsDates();
                _xpsDates.CheckDateEntry(_selDate, DateTime.Today.Date.ToShortDateString());
                DynamicMetricVm _dynVm = new DynamicMetricVm();
                DynContainer _dynDate = new DynContainer
                {
                    DataContext = _dynVm,
                    LblDate = { Content = _selDate }
                };

                _dynVm.LoadSets(_xpsDates.ValidDate, "Main", "Main");
                _editDateWrap.Add(_dynDate);
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error loading Dynamic Metric Set: " + _ex.Message);
            }
        }

        #endregion Main Methods

        #region Enter Metrics Methods

        private void AddDateControl()
        {
            XpsDates _xDates = new XpsDates();
            _xDates.CheckDateEntry(_entryDate, DateTime.Today.Date.ToShortDateString());

            InbMetricsVm _vm = new InbMetricsVm
            {
                _entryDate = _xDates.ValidDate.ToShortDateString()
            };
            MetricView _mainCtrl = new MetricView { DataContext = _vm };

            _metricsDateWrap.Add(_mainCtrl);
            _vm.LoadViewV2(true, "Main", "Main");
        }

        private void ViewDateControl()
        {
            XpsDates _xDates = new XpsDates();
            _xDates.CheckDateEntry(_entryDate, DateTime.Today.Date.ToShortDateString());

            InbMetricsVm _vm = new InbMetricsVm
            {
                _entryDate = _xDates.ValidDate.ToShortDateString()
            };
            MetricView _mainCtrl = new MetricView { DataContext = _vm };

            _metricsDateWrap.Add(_mainCtrl);
            _vm.LoadViewV2(false, "Main", "Main");
        }

        private void EditDateControl()
        {
            XpsDates _xDates = new XpsDates();
            _xDates.CheckDateEntry(_entryDate, DateTime.Today.Date.ToShortDateString());

            try
            {
                var _dateEntries = (from _a in new XpresEntities().MetricsInbounds
                                    where _a.Date == _xDates.ValidDate && _a.CategoryType == "Main"
                                    select _a);
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (_dateEntries != null && _dateEntries.Any())
                {
                    // ReSharper disable once UnusedVariable
                    foreach (MetricsInbound _metricsInbound in _dateEntries)
                    {
                        InbMetricsVm _vm = new InbMetricsVm
                        {
                            _entryDate = _xDates.ValidDate.ToShortDateString()
                        };
                        MetricView _mainCtrl = new MetricView { DataContext = _vm };

                        _metricsDateWrap.Add(_mainCtrl);
                        _vm.LoadViewV2(false, "Main", "Main");
                    }
                }
                else
                {
                    MessageBox.Show(@"No entries found with this date.");
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error retrieving Metrics Controls using this date: " + _ex.Message);
            }
        }

        private void LoadViewV2(bool newEntry, string parent, string ctrlType)
        {
            XpsDates _xDates = new XpsDates();
            _xDates.CheckDateEntry(_entryDate, DateTime.Today.Date.ToShortDateString());

            try
            {
                var _metricList = (from _a in new XpresEntities().DynamicInbMetrics
                                   where _a.State == true && _a.Parent == parent && _a.CategoryType == ctrlType
                                   orderby _a.CategoryTreeLevel ascending
                                   select _a);

                MetricsInboundV2 _metricsEntry = null;
                if (!newEntry)
                {
                    _metricsEntry = (from _b in new XpresEntities().MetricsInboundV2
                                     where _b.Date == _xDates.ValidDate
                                     select _b).SingleOrDefault();
                }
               
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (_metricList == null) return;

                if (!_metricList.Any()) return;
                foreach (DynamicInbMetric _c in _metricList)
                {
                    switch (ctrlType)
                    {
                        case "Main":
                            {
                                InbMetricsVm _vm = new InbMetricsVm { _catName = _c.CategoryName.Trim() };
                                MetricViewCat1 _cat1 = new MetricViewCat1
                                {
                                    KeyId = _c.ID.ToString(),
                                    CatName = _vm._catName,
                                    TreeLvl = _c.CategoryTreeLevel.ToString(),
                                    CtrlParent = _c.Parent.Trim(),
                                    Type = _c.CategoryType.Trim(),
                                    DataContext = _vm
                                };

                                _metricsCat1Wrap.Add(_cat1);
                                _vm.LoadViewV2(newEntry, _c.CategoryName.Trim(), "Sub");
                                _vm.LoadViewV2(newEntry, _c.CategoryName.Trim(), "SubSum");
                            }
                            break;

                        case "Sub":
                            {
                                InbMetricsVm _vm = new InbMetricsVm { _catName = _c.CategoryName.Trim() };
                                MetricViewCat2 _cat2 = new MetricViewCat2();
                                if (_metricsEntry != null)
                                {
                                    MetricDecoder _subContainer = new MetricDecoder();
                                    string _subData = _metricsEntry.Sub;
                                    _subContainer.Unwrap(_subData);
                                    foreach (MetricDecoder.MetricBody _m in _subContainer.MetricContainer)
                                    {
                                        if (_m.Parent == _c.Parent && _m.TreeLvl == _c.CategoryTreeLevel.ToString())
                                        {
                                            _cat2.KeyId = _m.KeyId;
                                            _cat2.CatName = _m.CatName;
                                            _cat2.TreeLvl = _m.TreeLvl;
                                            _cat2.CtrlParent = _m.Parent;
                                            _cat2.Type = _m.Type;
                                            _cat2.Val = _m.Val;
                                            _cat2.TxtVal.Text = _m.Val;
                                            _cat2.DataContext = _vm;

                                        }
                                    }
                                    
                                }
                                else
                                {
                                    _cat2.KeyId = _c.ID.ToString();
                                    _cat2.CatName = _c.CategoryName.Trim();
                                    _cat2.TreeLvl = _c.CategoryTreeLevel.ToString();
                                    _cat2.CtrlParent = _c.Parent.Trim();
                                    _cat2.Type = _c.CategoryType.Trim();
                                    _cat2.DataContext = _vm;
                                }

                                if (_metricsCat2Wrap == null)
                                    _metricsCat2Wrap = new ObservableCollection<MetricViewCat2>();
                                _metricsCat2Wrap.Add(_cat2);
                                _vm.LoadViewV2(newEntry, _c.CategoryName.Trim(), "Mic");
                                _vm.LoadViewV2(newEntry, _c.CategoryName.Trim(), "MicSum");
                            }
                            break;

                        case "SubSum":
                            {
                                MetricViewCat2 _cat2 = new MetricViewCat2
                                {
                                    DataContext = this,
                                    TxtVal = { Visibility = Visibility.Hidden },
                                    SpCatName = { Visibility = Visibility.Hidden },
                                    TxtNumVal = { Visibility = Visibility.Visible },
                                    BtnCalc = { Visibility = Visibility.Visible },
                                    TxtSubSum = { Visibility = Visibility.Visible },
                                    TxtCatName = { Text = "Subtotal" },
                                    KeyId = _c.ID.ToString(),
                                    CatName = _c.CategoryName.Trim(),
                                    TreeLvl = _c.CategoryTreeLevel.ToString(),
                                    CtrlParent = _c.Parent.Trim(),
                                    Type = _c.CategoryType.Trim()
                                };
                                
                                string _sumConc = _c.CategoryName.Trim();
                                _sumCatArr = _sumConc.Split('_');

                                //if (_metricsEntry != null) GetSubSum();

                                _metricsCat2Wrap.Add(_cat2);
                            }
                            break;

                        case "Mic":
                            {
                                InbMetricsVm _vm = new InbMetricsVm { _catName = _c.CategoryName.Trim() };

                                MetricViewCat3 _cat3 = new MetricViewCat3();

                                if (_metricsEntry != null)
                                {
                                    MetricDecoder _subContainer = new MetricDecoder();
                                    string _subData = _metricsEntry.Sub;
                                    _subContainer.Unwrap(_subData);
                                    foreach (MetricDecoder.MetricBody _m in _subContainer.MetricContainer)
                                    {
                                        if (_m.Parent == _c.Parent && _m.TreeLvl == _c.CategoryTreeLevel.ToString())
                                        {
                                            _cat3.KeyId = _m.KeyId;
                                            _cat3.CatName = _m.CatName;
                                            _cat3.TreeLvl = _m.TreeLvl;
                                            _cat3.CtrlParent = _m.Parent;
                                            _cat3.Type = _m.Type;
                                            _cat3.Val = _m.Val;
                                            _cat3.TxtVal.Text = _m.Val;
                                            _cat3.DataContext = _vm;
                                        }
                                        else
                                        {
                                            _cat3.KeyId = _c.ID.ToString();
                                            _cat3.CatName = _c.CategoryName.Trim();
                                            _cat3.TreeLvl = _c.CategoryTreeLevel.ToString();
                                            _cat3.CtrlParent = _c.Parent.Trim();
                                            _cat3.Type = _c.CategoryType.Trim();
                                            _cat3.DataContext = _vm;
                                        }
                                    }
                                }
                                    
                                if (_metricsCat3Wrap == null)
                                    _metricsCat3Wrap = new ObservableCollection<MetricViewCat3>();
                                _metricsCat3Wrap.Add(_cat3);
                            }
                            break;

                        case "MicSum":
                            {
                                MetricViewCat3 _cat3 = new MetricViewCat3
                                {
                                    DataContext = this,
                                    TxtVal = { Visibility = Visibility.Hidden },
                                    SpCatName = { Visibility = Visibility.Hidden },
                                    TxtNumVal = { Visibility = Visibility.Visible },
                                    BtnCalc = { Visibility = Visibility.Visible },
                                    TxtMicSum = { Visibility = Visibility.Visible },
                                    TxtCatName = { Text = "Subtotal" },
                                    KeyId = _c.ID.ToString(),
                                    CatName = _c.CategoryName.Trim(),
                                    TreeLvl = _c.CategoryTreeLevel.ToString(),
                                    CtrlParent = _c.Parent.Trim(),
                                    Type = _c.CategoryType.Trim()
                                };
                                
                                string _sumConc = _c.CategoryName.Trim();
                                _sumCatArr = _sumConc.Split('_');

                                //if (_metricsEntry != null) GetMicSum();

                                _metricsCat3Wrap.Add(_cat3);
                            }
                            break;
                    }
                }
            
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error retrieving Inbound metrics: " + _ex.Message);
            }
        }

        private void LoadView(bool newEntry, string parent, string ctrlType)
        {
            try
            {
                XpsDates _xDates = new XpsDates();
                _xDates.CheckDateEntry(_entryDate, DateTime.Today.Date.ToShortDateString());

                var _metricList = (from _a in new XpresEntities().DynamicInbMetrics
                                   where _a.State == true && _a.Parent == parent && _a.CategoryType == ctrlType
                                   orderby _a.CategoryTreeLevel ascending
                                   select _a);

                IOrderedQueryable<MetricsInbound> _metricsEntry = null;
                if (!newEntry)
                {
                    _metricsEntry = (from _b in new XpresEntities().MetricsInbounds
                                     where _b.Parent == parent && _b.CategoryType == ctrlType && _b.Date == _xDates.ValidDate
                                     orderby _b.CategoryTreeLevel ascending
                                     select _b);
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (_metricList == null) return;

                if (!_metricList.Any()) return;
                foreach (DynamicInbMetric _c in _metricList)
                {
                    switch (ctrlType)
                    {
                        case "Main":
                            {
                                InbMetricsVm _vm = new InbMetricsVm { _catName = _c.CategoryName.Trim() };
                                MetricViewCat1 _cat1 = new MetricViewCat1
                                {
                                    KeyId = _c.ID.ToString(),
                                    CatName = _c.CategoryName.Trim(),
                                    TreeLvl = _c.CategoryTreeLevel.ToString(),
                                    CtrlParent = _c.Parent.Trim(),
                                    Type = _c.CategoryType.Trim(),
                                    DataContext = _vm
                                };

                                _metricsCat1Wrap.Add(_cat1);
                                _vm.LoadViewV2(newEntry, _c.CategoryName.Trim(), "Sub");
                                _vm.LoadViewV2(newEntry, _c.CategoryName.Trim(), "SubSum");
                            }
                            break;

                        case "Sub":
                            {
                                InbMetricsVm _vm = new InbMetricsVm { _catName = _c.CategoryName.Trim() };
                                MetricViewCat2 _cat2 = new MetricViewCat2
                                {
                                    KeyId = _c.ID.ToString(),
                                    CatName = _c.CategoryName.Trim(),
                                    TreeLvl = _c.CategoryTreeLevel.ToString(),
                                    CtrlParent = _c.Parent.Trim(),
                                    Type = _c.CategoryType.Trim(),
                                    DataContext = _vm
                                };
                                if (_metricsEntry != null)
                                {
                                   
                                    
                                    
                                    
                                    foreach (MetricsInbound _e in _metricsEntry.Where(a => a.CategoryName.Trim() == _c.CategoryName.Trim()))
                                    {
                                        //if (_e.CategoryName.Trim() == _c.CategoryType.Trim()) continue;
                                        _cat2.TxtVal.Text = _e.TextValue.Trim();
                                        _cat2.TxtNumVal.Text = _e.NumValue.ToString();
                                    }
                                }

                                if (_metricsCat2Wrap == null)
                                    _metricsCat2Wrap = new ObservableCollection<MetricViewCat2>();
                                _metricsCat2Wrap.Add(_cat2);
                                _vm.LoadViewV2(newEntry, _c.CategoryName.Trim(), "Mic");
                                _vm.LoadViewV2(newEntry, _c.CategoryName.Trim(), "MicSum");
                            }
                            break;

                        case "SubSum":
                            {
                                MetricViewCat2 _cat2 = new MetricViewCat2
                                {
                                    DataContext = this,
                                    TxtVal = { Visibility = Visibility.Hidden },
                                    SpCatName = { Visibility = Visibility.Hidden },
                                    TxtNumVal = { Visibility = Visibility.Visible },
                                    BtnCalc = { Visibility = Visibility.Visible },
                                    TxtSubSum = { Visibility = Visibility.Visible },
                                    TxtCatName = { Text = "Subtotal" },
                                    KeyId = _c.ID.ToString(),
                                    CatName = _c.CategoryName.Trim(),
                                    TreeLvl = _c.CategoryTreeLevel.ToString(),
                                    CtrlParent = _c.Parent.Trim(),
                                    Type = _c.CategoryType.Trim()
                                };

                                string _sumConc = _c.CategoryName.Trim();
                                _sumCatArr = _sumConc.Split('_');

                                if (_metricsEntry != null && _metricsEntry.Any())
                                    foreach (MetricsInbound _e in _metricsEntry.Where(a => a.CategoryName.Trim() == _c.CategoryName.Trim()))
                                    {
                                        //if (_e.CategoryName.Trim() == _c.CategoryName.Trim()) continue;
                                        _cat2.TxtVal.Text = _e.TextValue.Trim();
                                        _cat2.TxtNumVal.Text = _e.NumValue.ToString();
                                    }
                                _metricsCat2Wrap.Add(_cat2);
                            }
                            break;

                        case "Mic":
                            {
                                InbMetricsVm _vm = new InbMetricsVm { _catName = _c.CategoryName.Trim() };
                                MetricViewCat3 _cat3 = new MetricViewCat3
                                {
                                    KeyId = _c.ID.ToString(),
                                    CatName = _c.CategoryName.Trim(),
                                    TreeLvl = _c.CategoryTreeLevel.ToString(),
                                    CtrlParent = _c.Parent.Trim(),
                                    Type = _c.CategoryType.Trim(),
                                    DataContext = _vm
                                };
                                if (_metricsEntry != null && _metricsEntry.Any())
                                    foreach (MetricsInbound _e in _metricsEntry.Where(a => a.CategoryName.Trim() == _c.CategoryName.Trim()))
                                    {
                                        //if (_e.CategoryName.Trim() == _c.CategoryName.Trim()) continue;
                                        _cat3.TxtVal.Text = _e.TextValue.Trim();
                                        _cat3.TxtNumVal.Text = _e.NumValue.ToString();
                                    }
                                if (_metricsCat3Wrap == null)
                                    _metricsCat3Wrap = new ObservableCollection<MetricViewCat3>();
                                _metricsCat3Wrap.Add(_cat3);
                            }
                            break;

                        case "MicSum":
                            {
                                MetricViewCat3 _cat3 = new MetricViewCat3
                                {
                                    DataContext = this,
                                    TxtVal = { Visibility = Visibility.Hidden },
                                    SpCatName = { Visibility = Visibility.Hidden },
                                    TxtNumVal = { Visibility = Visibility.Visible },
                                    BtnCalc = { Visibility = Visibility.Visible },
                                    TxtMicSum = { Visibility = Visibility.Visible },
                                    TxtCatName = { Text = "Subtotal" },
                                    KeyId = _c.ID.ToString(),
                                    CatName = _c.CategoryName.Trim(),
                                    TreeLvl = _c.CategoryTreeLevel.ToString(),
                                    CtrlParent = _c.Parent.Trim(),
                                    Type = _c.CategoryType.Trim()
                                };
                                if (_metricsEntry != null && _metricsEntry.Any())
                                    foreach (MetricsInbound _e in _metricsEntry.Where(a => a.CategoryName.Trim() == _c.CategoryName.Trim()))
                                    {
                                        //if (_e.CategoryName.Trim() == _c.CategoryName.Trim()) continue;
                                        _cat3.TxtVal.Text = _e.TextValue.Trim();
                                        _cat3.TxtNumVal.Text = _e.NumValue.ToString();
                                    }
                                string _sumConc = _c.CategoryName.Trim();
                                _sumCatArr = _sumConc.Split('_');

                                _metricsCat3Wrap.Add(_cat3);
                            }
                            break;
                    }
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error retrieving metrics control: " + _ex.Message);
            }
        }

        private void SaveDateControlV2()
        {
            try
            {
                if (!_metricsDateWrap.Any()) return;
                XpresEntities _xps = new XpresEntities();
                // ReSharper disable once TooWideLocalVariableScope
                MetricsInboundV2 _updateCheck = null;

                XpsDates _xDates = new XpsDates();
                _xDates.CheckDateEntry(_entryDate, DateTime.Today.Date.ToShortDateString());
                int _keyId = 1;

                try
                {
                    _updateCheck = (from _a in _xps.MetricsInboundV2
                                    where _a.Date == _xDates.ValidDate
                                    select _a).SingleOrDefault();
                }
                catch{/* ignored */}


                foreach (MetricView _entry in _metricsDateWrap)
                {
                    InbMetricsVm _vmDate = _entry.DataContext as InbMetricsVm;
                    if (_vmDate != null)
                        foreach (MetricViewCat1 _ctrlMain in _vmDate.MetricsCat1Wrap)
                        {
                            string _mainCatName = _ctrlMain.TxtCat.Text.Trim();
                            int? _mainTreeLvl = 0;

                            if (!string.IsNullOrEmpty(_ctrlMain.TreeLvl))
                                _mainTreeLvl = Convert.ToInt32(_ctrlMain.TreeLvl);
                            if (!string.IsNullOrEmpty(_mainEntryWrap)) _mainEntryWrap += @"|";
                            _mainEntryWrap += @"[" + _keyId + "^" + _mainCatName + "^NULL^" + _mainTreeLvl + "^NULL^Main]";
                            _keyId++;

                            InbMetricsVm _vmMain = _ctrlMain.DataContext as InbMetricsVm;
                            if (_vmMain != null)
                                foreach (MetricViewCat2 _ctrlSub in _vmMain.MetricsCat2Wrap)
                                {
                                    string _subValue = _ctrlSub.TxtVal.Text.Trim();
                                    string _subType = _ctrlSub.Type.Trim();
                                    string _subParent = _ctrlSub.CtrlParent.Trim();
                                    double? _subNumVal = 0;
                                    int? _subTreeLvl = 0;
                                    if (!string.IsNullOrEmpty(_ctrlSub.TreeLvl))
                                        _subTreeLvl = Convert.ToInt32(_ctrlSub.TreeLvl.Trim());
                                    if (!string.IsNullOrEmpty(_ctrlSub.TxtNumVal.Text))
                                        _subNumVal = Convert.ToDouble(_ctrlSub.TxtNumVal.Text.Trim());

                                    string _subCatName = _subType == "Sub" ? _ctrlSub.TxtCatName.Text.Trim() : _ctrlSub.CatName.Trim();
                                    if (!string.IsNullOrEmpty(_subEntryWrap)) _subEntryWrap += @"|";
                                    _subEntryWrap += @"[" + _keyId + "^" + _subCatName + "^" + _subValue + "^" + _subTreeLvl + "^" + _subParent + "^" + _subType + "]";
                                    _keyId++;

                                    InbMetricsVm _vmSub = _ctrlSub.DataContext as InbMetricsVm;
                                    if (_vmSub != null)
                                    {
                                        foreach (MetricViewCat3 _ctrlMic in _vmSub._metricsCat3Wrap)
                                        {
                                            string _micValue = _ctrlMic.TxtVal.Text.Trim();
                                            string _micType = _ctrlMic.Type.Trim();
                                            string _micParent = _ctrlMic.CtrlParent.Trim();
                                            double? _micNumVal = 0;
                                            int? _micTreeLvl = 0;

                                            if (!string.IsNullOrEmpty(_ctrlMic.TreeLvl))
                                                _micTreeLvl = Convert.ToInt32(_ctrlMic.TreeLvl.Trim());
                                            if (!string.IsNullOrEmpty(_ctrlMic.TxtNumVal.Text))
                                                _micNumVal = Convert.ToDouble(_ctrlMic.TxtNumVal.Text.Trim());

                                            string _micCatName = _subType == "Mic" ? _ctrlMic.TxtCatName.Text.Trim() : _ctrlMic.CatName.Trim();
                                            if (!string.IsNullOrEmpty(_micEntryWrap)) _micEntryWrap += @"|";
                                            _micEntryWrap += @"[" + _keyId + "^" + _micCatName + "^" + _micValue + "^" + _micTreeLvl + "^" + _micParent + "^" + _micType + "]";
                                            _keyId++;
                                        }
                                    }
                                }
                        }
                }

                if (_updateCheck != null)
                {
                    _updateCheck.Main = _mainEntryWrap;
                    _updateCheck.Sub = _subEntryWrap;
                    _updateCheck.Sub = _micEntryWrap;
                }
                else
                {
                    MetricsInboundV2 _newEntry = new MetricsInboundV2
                    {
                        Date = _xDates.ValidDate,
                        Main = _mainEntryWrap,
                        Sub = _subEntryWrap,
                        Mic = _micEntryWrap
                    };
                    _xps.MetricsInboundV2.Add(_newEntry);
                }
                _xps.SaveChanges();
                _mainEntryWrap = "";
                _subEntryWrap = "";
                _micEntryWrap = "";
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error saving metrics: " + _ex.Message);
            }
        }

        private void SaveDateControl()
        {
            try
            {
                if (!_metricsDateWrap.Any()) return;
                XpresEntities _xps = new XpresEntities();
                // ReSharper disable once TooWideLocalVariableScope
                MetricsInbound _oldEntry;

                XpsDates _xDates = new XpsDates();
                _xDates.CheckDateEntry(_entryDate, DateTime.Today.Date.ToShortDateString());

                var _updateCheck = (from _a in _xps.MetricsInbounds
                                    where _a.Date == _xDates.ValidDate
                                    select _a);
                
                foreach (MetricView _entry in _metricsDateWrap)
                {
                    InbMetricsVm _vmDate = _entry.DataContext as InbMetricsVm;
                    if (_vmDate != null)
                        foreach (MetricViewCat1 _ctrlMain in _vmDate.MetricsCat1Wrap)
                        {
                            string _mainCatName = _ctrlMain.TxtCat.Text.Trim();
                            string _mainType = _ctrlMain.Type.Trim();
                            string _mainParent = _ctrlMain.CtrlParent.Trim();
                            int? _mainTreeLvl = 0;
                            
                            if (!string.IsNullOrEmpty(_ctrlMain.TreeLvl))
                                _mainTreeLvl = Convert.ToInt32(_ctrlMain.TreeLvl);

                            MetricsInbound _mainUpdate = new MetricsInbound
                            {
                                Date = _xDates.ValidDate,
                                CategoryName = _mainCatName,
                                CategoryTreeLevel = _mainTreeLvl,
                                CategoryType = _mainType,
                                Parent = _mainParent,
                            };
                            _xps.MetricsInbounds.Add(_mainUpdate);

                            InbMetricsVm _vmMain = _ctrlMain.DataContext as InbMetricsVm;
                            if (_vmMain != null)
                                foreach (MetricViewCat2 _ctrlSub in _vmMain.MetricsCat2Wrap)
                                {
                                    string _subValue = _ctrlSub.TxtVal.Text.Trim();
                                    string _subType = _ctrlSub.Type.Trim();
                                    string _subParent = _ctrlSub.CtrlParent.Trim();
                                    string _subCatName = "";

                                    _subCatName = _subType == "Sub"? _ctrlSub.TxtCatName.Text.Trim(): _ctrlSub.CatName.Trim();
                                    
                                    double? _subNumVal = 0;
                                    int? _subTreeLvl = 0;

                                    if (!string.IsNullOrEmpty(_ctrlSub.TreeLvl))
                                        _subTreeLvl = Convert.ToInt32(_ctrlSub.TreeLvl.Trim());
                                    if (!string.IsNullOrEmpty(_ctrlSub.TxtNumVal.Text))
                                        _subNumVal = Convert.ToDouble(_ctrlSub.TxtNumVal.Text.Trim());

                                    _oldEntry = null;
                                    try
                                    {
                                        _oldEntry = _updateCheck.SingleOrDefault(a => a.CategoryName.Trim() == _subCatName);
                                    }
                                    catch
                                    {
                                        continue;
                                    }

                                    if (_oldEntry != null)
                                    {
                                        _oldEntry.TextValue = _subValue;
                                        _oldEntry.NumValue = _subNumVal;
                                        _xps.SaveChanges();
                                    }
                                    else
                                    {
                                        MetricsInbound _subUpdate = new MetricsInbound
                                        {
                                            Date = _xDates.ValidDate,
                                            CategoryName = _subCatName,
                                            TextValue = _subValue,
                                            CategoryTreeLevel = _subTreeLvl,
                                            CategoryType = _subType,
                                            Parent = _subParent,
                                            NumValue = _subNumVal
                                        };
                                        _xps.MetricsInbounds.Add(_subUpdate);
                                    }

                                    InbMetricsVm _vmSub = _ctrlSub.DataContext as InbMetricsVm;
                                    if (_vmSub != null)
                                    {
                                        foreach (MetricViewCat3 _ctrlMic in _vmSub._metricsCat3Wrap)
                                        {
                                            string _micValue = _ctrlMic.TxtVal.Text.Trim();
                                            string _micType = _ctrlMic.Type.Trim();
                                            string _micParent = _ctrlMic.CtrlParent.Trim();
                                            string _micCatName = "";

                                            _micCatName = _micType == "Mic"? _ctrlMic.TxtCatName.Text.Trim(): _ctrlMic.CatName.Trim();
                                            
                                            double? _micNumVal = 0;
                                            int? _micTreeLvl = 0;

                                            if (!string.IsNullOrEmpty(_ctrlMic.TreeLvl))
                                                _micTreeLvl = Convert.ToInt32(_ctrlMic.TreeLvl.Trim());
                                            if (!string.IsNullOrEmpty(_ctrlMic.TxtNumVal.Text))
                                                _micNumVal = Convert.ToDouble(_ctrlMic.TxtNumVal.Text.Trim());

                                            _oldEntry = null;
                                            try
                                            {
                                                _oldEntry = _updateCheck.SingleOrDefault(a => a.CategoryName.Trim() == _micCatName);
                                            }
                                            catch
                                            {
                                                continue;
                                            }

                                            if (_oldEntry != null)
                                            {
                                                _oldEntry.TextValue = _micValue;
                                                _oldEntry.NumValue = _micNumVal;
                                                _xps.SaveChanges();
                                            }
                                            else
                                            {
                                                MetricsInbound _micUpdate = new MetricsInbound
                                                {
                                                    Date = _xDates.ValidDate,
                                                    CategoryName = _micCatName,
                                                    TextValue = _micValue,
                                                    CategoryTreeLevel = _micTreeLvl,
                                                    CategoryType = _micType,
                                                    Parent = _micParent,
                                                    NumValue = _micNumVal
                                                };
                                                _xps.MetricsInbounds.Add(_micUpdate);
                                            }
                                        }
                                    }
                                }
                        }
                }
                _xps.SaveChanges();
            }
            catch (Exception _ex)
            {
                MessageBox.Show(@"Error saving metrics: " + _ex.Message);
            }
        }

        #endregion Enter Metrics Methods

        #endregion Methods

        #region INotify Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotify Implementation
    }

    public class MetricDecoder
    {
        public MetricDecoder()
        {
            MetricContainer = new List<MetricBody>();
        }


        public class MetricBody
        {
            public string KeyId { get; set; }
            public string CatName { get; set; }
            public string Val { get; set; }
            public string TreeLvl { get; set; }
            public string Parent { get; set; }
            public string Type { get; set; }
        }

        public List<MetricBody> MetricContainer { get; set; }

        public void Unwrap(string metricData)
        {
            string[] _outerArr = metricData.Split('|');
            foreach (string _s in _outerArr)
            {
                string _outerString = _s.Replace(@"[", "");
                _outerString = _outerString.Replace(@"]", "");
                string[] _innerArr = _outerString.Split('^');
                MetricBody _container = new MetricBody
                {
                    KeyId = _innerArr[0],
                    CatName = _innerArr[1],
                    Val = _innerArr[2],
                    TreeLvl = _innerArr[3],
                    Parent = _innerArr[4],
                    Type = _innerArr[5]
                };
                MetricContainer.Add(_container);
            }
        }
    }
}