using System.Windows.Controls;

namespace XPRES.Departments.Inbound.Controls
{
    /// <summary>
    /// Interaction logic for MetricVewCat1.xaml
    /// </summary>
    public partial class MetricViewCat1 : UserControl
    {
        public string KeyId { get; set; }
        public string CatName { get; set; }
        public string Val { get; set; }
        public string TreeLvl { get; set; }
        public string CtrlParent { get; set; }
        public string Type { get; set; }
        public MetricViewCat1()
        {
            InitializeComponent();
        }
    }
}