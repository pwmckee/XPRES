using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XPRES.DAL;

namespace XPRES.Departments.Inbound.Controls
{
    /// <summary>
    /// Interaction logic for DynCtrl.xaml
    /// </summary>
    public partial class DynCtrl : UserControl
    {
        public DynCtrl()
        {
            InitializeComponent();
            TxbCtrlName.Text = Name;
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
            GrdViewSum.Visibility = Visibility.Hidden;
        }
    }
}
