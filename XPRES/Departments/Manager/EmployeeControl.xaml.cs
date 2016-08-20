using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using XPRES.DAL;

namespace XPRES.Departments.Manager
{
    /// <summary>
    /// Interaction logic for EmployeeControl.xaml
    /// </summary>
    public partial class EmployeeControl : Window
    {
        private XpresEntities xps;
        private string dept;

        public EmployeeControl(string Dept)
        {
            InitializeComponent();
            xps = new XpresEntities();
            dept = Dept;
            lblDept.Content = dept;
            FillEmployees();
        }
        
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            AddOperator();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteOperator();
        }

        private void FillEmployees()
        {
            try
            {
                xps = new XpresEntities();
                List<string> _emp = new List<string>();
                var emp = (from a in xps.Employees
                           select a);

                switch (dept)
                {
                    case "Inventory":
                        emp = emp.Where(a => a.Inventory == true);
                        break;

                    case "OutBound":
                        emp = emp.Where(a => a.OutBound == true);
                        break;

                    case "InBound":
                        emp = emp.Where(a => a.InBound == true);
                        break;

                    case "Replen":
                        emp = emp.Where(a => a.Replen == true);
                        break;

                    default:
                        break;
                }

                foreach (var n in emp)
                {
                    _emp.Add(n.FullName);
                }
                cboEmpList.ItemsSource = _emp;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error trying to get employee list from the database: " + ex.Message);
                return;
            }
        }

        private void AddOperator()
        {
            string _emp = string.Empty;
            string _firstName = string.Empty;
            string _lastName = string.Empty;
            _firstName = txtFirst.Text;
            _lastName = txtLast.Text;

            if (_firstName == "" || _lastName == "")
            {
                System.Windows.Forms.MessageBox.Show("Both first and last name must be entered");
                return;
            }

            _emp = _firstName.Trim().ToUpper() + " " + _lastName.Trim().ToUpper();
            Employee emp = new Employee();
            try
            {
                emp = (from a in xps.Employees
                       where a.FullName == _emp
                       select a).SingleOrDefault();
                switch (dept)
                {
                    case "Inventory":
                        emp.Inventory = true;
                        break;

                    case "OutBound":
                        emp.OutBound = true;
                        break;

                    case "InBound":
                        emp.InBound = true;
                        break;

                    case "Replen":
                        emp.Replen = true;
                        break;

                    default:
                        break;
                }
            }
            catch
            {
                emp = new Employee();
                emp.FirstName = _firstName;
                emp.LastName = _lastName;
                emp.FullName = _emp;
                switch (dept)
                {
                    case "Inventory":
                        emp.Inventory = true;
                        break;

                    case "OutBound":
                        emp.OutBound = true;
                        break;

                    case "InBound":
                        emp.InBound = true;
                        break;

                    case "Replen":
                        emp.Replen = true;
                        break;

                    default:
                        break;
                }
                xps.Employees.Add(emp);
            }
            try
            {
                xps.SaveChanges();
                System.Windows.Forms.MessageBox.Show(_emp + " successfully added to the " + dept + " list.");
                txtFirst.Clear();
                txtLast.Clear();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to add " + _emp + " to the database: " + ex.Message);
                return;
            }
            FillEmployees();
        }

        private void DeleteOperator()
        {
            string _fullName = string.Empty;

            if (cboEmpList.SelectedIndex > -1)
                _fullName = cboEmpList.SelectedItem.ToString();

            try
            {
                var emp = (from a in xps.Employees
                           where a.FullName == _fullName
                           select a).SingleOrDefault();
                switch (dept)
                {
                    case "Inventory":
                        emp.Inventory = false;
                        break;

                    case "OutBound":
                        emp.OutBound = false;
                        break;

                    case "InBound":
                        emp.InBound = false;
                        break;

                    case "Replen":
                        emp.Replen = false;
                        break;

                    default:
                        break;
                }
                xps.SaveChanges();
                System.Windows.Forms.MessageBox.Show(_fullName + " removed from the " + dept + "list");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error while trying to delete " + _fullName + ": " + ex.Message);
                return;
            }
            FillEmployees();
        }
    }
}