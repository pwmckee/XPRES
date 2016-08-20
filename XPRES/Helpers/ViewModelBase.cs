using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XPRES.Helpers
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region INotify Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotify Implementation
    }
}
