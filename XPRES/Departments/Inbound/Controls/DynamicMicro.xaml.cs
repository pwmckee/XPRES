using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;

namespace XPRES.Departments.Inbound.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DynamicMicro : UserControl
    {
        public string CtrlParent { get; set; }
        public string CtrlTreeLvl { get; set; }
        public string CtrlCatName { get; set; }
        public string CtrlType { get; set; }
        public bool CtrlState { get; set; }

        public DynamicMicro()
        {
            InitializeComponent();
            string _ctrlName = Name.ToString();
            TxbCtrlName.Text = _ctrlName;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            DeleteCategory();
        }

        private void DeleteCategory()
        {
            Visibility = Visibility.Collapsed;
            XpresEntities _xps = new XpresEntities();
            try
            {
                int _treeLevel = Convert.ToInt32(TxbTreeLevel.Text);
                TxbTreeLevel.Text = "";
                var _q = (from _a in _xps.DynamicInbMetrics
                          where _a.Parent == TxbCtrlParent.Text && _a.CategoryTreeLevel == _treeLevel
                          select _a).SingleOrDefault();
                if (_q != null)
                {
                    _q.CategoryTreeLevel = 0;
                    _q.State = false;
                }
                _xps.SaveChanges();
            }
            catch
            {
                // ignored
            }
            Name = "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            GrdAlter.Visibility = Visibility.Hidden;
            GrdView.Visibility = Visibility.Visible;
        }

        private void btnAlter_Click(object sender, RoutedEventArgs e)
        {
            GrdAlter.Visibility = Visibility.Visible;
            GrdView.Visibility = Visibility.Hidden;
        }
    }
}