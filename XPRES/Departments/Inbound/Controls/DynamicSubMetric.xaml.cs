using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;

namespace XPRES.Departments.Inbound.Controls
{
    /// <summary>
    /// Interaction logic for DynamicSubMetric.xaml
    /// </summary>
    public partial class DynamicSubMetric : UserControl
    {
        public string CtrlParent { get; set; }
        public string CtrlTreeLvl { get; set; }
        public string CtrlCatName { get; set; }
        public string CtrlType { get; set; }
        public bool CtrlState { get; set; }
        private string _ctrlName;

        public DynamicSubMetric()
        {
            InitializeComponent();
            _ctrlName = this.Name.ToString();
            TxbCtrlName.Text = _ctrlName;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            DeleteCategory();
        }

        private void DeleteCategory()
        {
            this.Visibility = Visibility.Collapsed;
            XpresEntities _xps = new XpresEntities();
            try
            {
                int _treeLevel = Convert.ToInt32(TxbTreeLevel.Text);
                TxbTreeLevel.Text = "";
                var _q = (from _a in _xps.DynamicInbMetrics
                         where _a.Parent == TxbCtrlParent.Text && _a.CategoryTreeLevel == _treeLevel
                         select _a).SingleOrDefault();
                _q.CategoryTreeLevel = 0;
                _q.State = false;
                _xps.SaveChanges();
            }
            catch
            { 
                //ignore
            }
            Name = "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _ctrlName = Name;
            TxbCtrlName.Text = _ctrlName;
            GrdAlter.Visibility = Visibility.Hidden;
            GrdView.Visibility = Visibility.Visible;
        }

        private void btnAlter_Click(object sender, RoutedEventArgs e)
        {
            GrdAlter.Visibility = Visibility.Visible;
            GrdView.Visibility = Visibility.Hidden;
        }

        private void btnDelSum_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void btnAlterSum_Click(object sender, RoutedEventArgs e)
        {
            GrdAlterSum.Visibility = Visibility.Visible;
            GrdViewSum.Visibility = Visibility.Hidden;
        }
    }
}