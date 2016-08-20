using System.Windows.Controls;

namespace XPRES.Departments.Inbound.Controls
{
    /// <summary>
    /// Interaction logic for MetricViewCat2.xaml
    /// </summary>
    public partial class MetricViewCat2 : UserControl
    {
        public string KeyId { get; set; }
        public string CatName { get; set; }
        public string Val { get; set; }
        public string TreeLvl { get; set; }
        public string CtrlParent { get; set; }
        public string Type { get; set; }
        public MetricViewCat2()
        {
            InitializeComponent();
        }
    }
}