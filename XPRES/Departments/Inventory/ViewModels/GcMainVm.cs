using System.Collections.ObjectModel;
using System.Windows.Input;
using XPRES.Commands;
using XPRES.Departments.Inventory.Views.Controls;
using XPRES.Helpers;

namespace XPRES.Departments.Inventory.ViewModels
{
    public class GcMainVm : ViewModelBase
    {
        #region Properties

        private ObservableCollection<CycoTracker> _tracker;

        public ObservableCollection<CycoTracker> Tracker
            => _tracker ?? (_tracker = new ObservableCollection<CycoTracker>());

        #endregion Properties

        #region ICommand Members

        public ICommand ViewTrackerCommand { get { return new RelayCommand(x => ViewTracker()); } }

        #endregion ICommand Members

        #region Methods

        private void ViewTracker()
        {
            CycoTracker _cyco = new CycoTracker();
            _tracker.Add(_cyco);
        }

        #endregion Methods
    }
}
