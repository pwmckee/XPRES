using System;
using System.Windows;
using System.Windows.Input;
using XPRES.Main.Views;

namespace XPRES.Commands
{
    public class HomeCommand : ICommand
    {
        public HomeCommand()
        {
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            bool _open = false;
            foreach (Window wnd in Application.Current.Windows)
            {
                if (wnd is MainWindow)
                {
                    _open = true;
                }
            }
            if (!_open)
            {
                MainWindow _home = new MainWindow();
                _home.Show();
            }
            else
            {
                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.Title == "XPRES")
                    {
                        wnd.Activate();
                    }
                }
            }
        }
    }
}