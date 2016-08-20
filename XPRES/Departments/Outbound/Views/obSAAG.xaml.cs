using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using XPRES.Departments.Outbound.ViewModels;

namespace XPRES.Departments.Outbound.Views
{
    /// <summary>
    /// Interaction logic for PickBoard.xaml
    /// </summary>
    public partial class obSAAG : Window
    {
        #region Constructor

        public obSAAG()
        {
            InitializeComponent();
            txtCommand.Focus();
            _multiPickNum = 0;
        }

        #endregion Constructor

        #region Properties

        private int _multiPickNum;

        #endregion Properties

        #region KeyDown Events

        #region Command

        private void txtCommand_KeyDown(object sender, KeyEventArgs e)
        {
            var _vm = DataContext as ObSaagVm;
            _vm.PickStackTimer.Stop();
            if (e.Key == Key.Enter)
            {
                if (txtCommand.Text.Trim() == "")
                {
                    txtCommand.Background = Brushes.LightSalmon;
                    return;
                }
                else
                {
                    txtCommand.Background = null;
                }
                string _command = txtCommand.Text.Trim();
                switch (_command)
                {
                    case ("StartSinglePick"):
                        StartSinglePick();
                        break;
                    case ("StartMultiPick"):
                        StartMultiPick();
                        break;
                    case ("CompletePick"):
                        CompletePick();
                        break;
                }
                txtCommand.Clear();
            }
        }

        #endregion Command

        #region Single Pick

        private void txtSinglePicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtSingleDeliveryID.Clear();
                txtSingleDeliveryID.Focus();
            }
        }

        private void txtSingleDeliveryID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtSinglePicker.Text.Trim() == "")
                {
                    txtSingleDeliveryID.Clear();
                    txtSinglePicker.Background = Brushes.LightSalmon;
                    txtSinglePicker.Focus();
                }
                else
                {
                    txtSinglePicker.Background = null;
                    txtSingleLineCount.Clear();
                    txtSingleLineCount.Focus();
                }
            }
        }

        private void txtSingleLineCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtSinglePicker.Text.Trim() == "")
                {
                    txtSingleLineCount.Clear();
                    txtSinglePicker.Background = Brushes.LightSalmon;
                    txtSinglePicker.Focus();
                }
                else if (txtSingleDeliveryID.Text.Trim() == "")
                {
                    txtSingleLineCount.Clear();
                    txtSingleDeliveryID.Background = Brushes.LightSalmon;
                    txtSingleDeliveryID.Focus();
                }
                else
                {
                    txtSinglePicker.Background = null;
                    txtSingleDeliveryID.Background = null;
                    foreach (StackPanel _sp in grdMainMenu.Children)
                    {
                        if (_sp.Name == nameof(spCommand))
                        {
                            _sp.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            _sp.Visibility = Visibility.Hidden;
                        }
                    }
                    txtCommand.Clear();
                    txtCommand.Focus();
                    var _vm = DataContext as ObSaagVm;
                    _vm.StartSinglePick();
                }
            }
        }
        #endregion Single Pick

        #region Multi Pick

        private void txtMultiPicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtMultiPickNum.Clear();
                txtMultiPickNum.Focus();
            }
        }

        private void txtMultiPickNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtMultiPicker.Text.Trim() == "")
                {
                    txtMultiPickNum.Clear();
                    txtMultiPicker.Background = Brushes.LightSalmon;
                    txtMultiPicker.Focus();
                }
                else
                {
                    txtMultiPicker.Background = null;
                    try
                    {
                        _multiPickNum = Convert.ToInt32(txtMultiPickNum.Text.Trim());

                        if (_multiPickNum > 100)
                        {
                            System.Windows.Forms.MessageBox.Show(@"Invalid amount of picks entered");
                            txtMultiPickNum.Background = Brushes.LightSalmon;
                            txtMultiPickNum.Clear();
                            return;
                        }
                        txtMultiDeliveryID.Clear();
                        txtMultiDeliveryID.Focus();
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show(@"Invalid amount of picks entered");
                        txtMultiPickNum.Background = Brushes.LightSalmon;
                        txtMultiPickNum.Clear();
                    }
                }
            }
        }

        private void txtMultiDeliveryID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtMultiPicker.Text.Trim() == "")
                {
                    txtMultiPicker.Background = Brushes.LightSalmon;
                    txtMultiDeliveryID.Clear();
                    txtMultiPicker.Focus();
                }
                else if (txtMultiPickNum.Text.Trim() == "")
                {
                    txtMultiPickNum.Background = Brushes.LightSalmon;
                    txtMultiDeliveryID.Clear();
                    txtMultiPickNum.Focus();
                }
                else
                {
                    txtMultiPicker.Background = null;
                    txtMultiPickNum.Background = null;
                    txtMultiLineCount.Clear();
                    txtMultiLineCount.Focus();
                }
            }
        }

        private void txtMultiLineCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtMultiPicker.Text.Trim() == "")
                {
                    txtMultiPicker.Background = Brushes.LightSalmon;
                    txtMultiPicker.Focus();
                }
                else if (txtMultiPickNum.Text.Trim() == "")
                {
                    txtMultiPickNum.Background = Brushes.LightSalmon;
                    txtMultiPickNum.Focus();
                }
                else if (txtMultiDeliveryID.Text.Trim() == "")
                {
                    txtMultiLineCount.Clear();
                    txtMultiDeliveryID.Background = Brushes.LightSalmon;
                }
                else
                {
                    txtMultiPicker.Background = null;
                    txtMultiPickNum.Background = null;
                    txtMultiDeliveryID.Background = null;
                    CheckMultiPick();
                }
            }
        }

        #endregion Multi Pick

        #region Complete Pick

        private void txtCompletePicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var _vm = DataContext as ObSaagVm;
                _vm.CompletePick();
                txtCompletePicker.Clear();
                foreach (StackPanel _sp in grdMainMenu.Children)
                {
                    if (_sp.Name == nameof(spCommand))
                    {
                        _sp.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _sp.Visibility = Visibility.Hidden;
                    }
                }
                txtCommand.Clear();
                txtCommand.Focus();
            }
        }

        #endregion Complete Pick

        #endregion KeyDown Events

        #region Methods

        private void StartSinglePick()
        {
            txtCommand.Clear();
            foreach (StackPanel _sp in grdMainMenu.Children)
            {
                if (_sp.Name == nameof(spSinglePickEntry))
                {
                    _sp.Visibility = Visibility.Visible;
                }
                else
                {
                    _sp.Visibility = Visibility.Hidden;
                }
            }
            txtSinglePicker.Clear();
            txtSinglePicker.Focus();
        }

        private void StartMultiPick()
        {
            txtCommand.Clear();
            foreach (StackPanel _sp in grdMainMenu.Children)
            {
                if (_sp.Name == nameof(spMultiPickEntry))
                {
                    _sp.Visibility = Visibility.Visible;
                }
                else
                {
                    _sp.Visibility = Visibility.Hidden;
                }
            }
            txtMultiPicker.Clear();
            txtMultiPicker.Focus();
        }

        private void CheckMultiPick()
        {
            var _vm = DataContext as ObSaagVm;
            grdMultiPick.Visibility = Visibility.Visible;
            grdFinishedPicks.Visibility = Visibility.Hidden;
            _vm.AddMultiPick();
            txtMultiDeliveryID.Clear();
            txtMultiDeliveryID.Focus();
            if (_vm.MultiPickCtrls.Count == _multiPickNum)
            {
                foreach (StackPanel _sp in grdMainMenu.Children)
                {
                    if (_sp.Name == nameof(spCommand))
                    {
                        _sp.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _sp.Visibility = Visibility.Hidden;
                    }
                }
                grdMultiPick.Visibility = Visibility.Hidden;
                grdFinishedPicks.Visibility = Visibility.Visible;
                txtCommand.Clear();
                txtCommand.Focus();
                _vm.StartMultiPick();
            }
        }

        private void CompletePick()
        {
            txtCommand.Clear();
            foreach (StackPanel _sp in grdMainMenu.Children)
            {
                if (_sp.Name == nameof(spComplete))
                {
                    _sp.Visibility = Visibility.Visible;
                }
                else
                {
                    _sp.Visibility = Visibility.Hidden;
                }
            }
            txtCompletePicker.Clear();
            txtCompletePicker.Focus();
        }

        #endregion Methods

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var _vm = DataContext as ObSaagVm;
            _vm.PickStackTimer.Stop();
        }
    }
}