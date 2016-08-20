using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XPRES.DAL;

namespace XPRES.Departments.Inventory.Adjustments.Controls
{
    /// <summary>
    /// Interaction logic for AdjCreateControl.xaml
    /// </summary>
    public partial class AdjCreateControl : UserControl
    {
        private DataTable dt;
        private XpresEntities xps;
        private string pid;
        private string seq;
        private string desc;
        private double? adjQty;
        private double? unitCost;
        private double? extCost;

        public AdjCreateControl()
        {
            InitializeComponent();
            xps = new XpresEntities();
            SetupTable();
            pid = string.Empty;
            seq = string.Empty;
            desc = string.Empty;
            adjQty = 0;
            unitCost = 0;
            extCost = 0;
        }

        private void SetupTable()
        {
            dt = new DataTable();
            dt.Columns.Add("PartNumber", typeof(string));
            dt.Columns.Add("Sequence", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("AdjQTY", typeof(double));
            dt.Columns.Add("UnitCost", typeof(double));
            dt.Columns.Add("ExtCost", typeof(double));
            dt.Columns.Add("Comments", typeof(string));
            dgSubmit.ItemsSource = dt.DefaultView;
        }

        #region Button Click events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddAdjSequence();
        }

        private void btnCheckCost_Click(object sender, RoutedEventArgs e)
        {
            CheckExtCost();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            SubmitRequest();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSequence();
        }

        #endregion Button Click events

        #region Methods

        private void AddAdjSequence()
        {
            #region Initial Validation

            if (txtPID.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter in a part number.");
                return;
            }
            if (txtAdj.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter in an adjustment qty.");
                return;
            }
            if (txtSeq.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter in a sequence number.");
                return;
            }
            try
            {
                Convert.ToSingle(txtAdj.Text.Trim().ToUpper());
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Invalid Adjustment QTY entered: " + ex.Message);
                return;
            }

            pid = txtPID.Text.Trim().ToUpper();
            seq = txtSeq.Text.Trim().ToUpper();
            try
            {
                var seqCheck = (from a in xps.Adjustments
                                where a.SEQ == seq
                                select a.SEQ).First();
                System.Windows.Forms.MessageBox.Show("Sequence: " + seq + " has already been entered on the tracker.  Please get a new sequence number or get with team leadership to have the tracker corrected.");
                return;
            }
            catch
            {
                adjQty = Convert.ToDouble(txtAdj.Text.Trim().ToUpper());

                foreach (DataRow _checkRow in dt.Rows)
                {
                    string _seq = _checkRow[1].ToString();
                    if (_checkRow[1].ToString() == seq)
                    {
                        System.Windows.Forms.MessageBox.Show("This sequence number has already been added to the sheet.  Please delete the sequence number from this request if you need to update it.");
                        return;
                    }
                }

                #endregion Initial Validation

                var _tuple = GetUnitInfo(pid);
                if (_tuple == null)
                {
                    return;
                }
                unitCost = _tuple.Item1;
                desc = _tuple.Item2;
                extCost = adjQty * unitCost;
                double? _sumReq = 0;
                double? _total = 0;

                if (dt.Rows.Count > 0)
                {
                    _sumReq = Convert.ToDouble(dt.Compute("Sum(ExtCost)", string.Empty));
                    _total = _sumReq + extCost;

                    if (_total >= 1500)
                    {
                        System.Windows.Forms.MessageBox.Show("This adjustment request will have a total of $1,500 or more " +
                            "if you add this sequence.  Please limit all single requests to under that amount.\n \n" +
                            "If you have a sequence that is more than $1,500 please add it to it's own request and " +
                            "send it seperately so it can be sent to finance for approval.");
                        return;
                    }
                }
                else
                {
                    _total = extCost;
                }

                DataRow dr = dt.NewRow();
                dr[0] = pid;
                dr[1] = seq;
                dr[2] = desc;
                dr[3] = adjQty;
                dr[4] = unitCost;
                dr[5] = extCost;

                #region Cost Validation

                bool currentChk = false;
                bool oldChk = false;
                bool adjChk = false;
                bool accChk = false;
                bool tdChk = false;
                bool bomChk = false;
                bool poChk = false;
                bool walkChk = false;
                if (chkCurrent.IsChecked.Value)
                    currentChk = true;
                if (chkOldLoc.IsChecked.Value)
                    oldChk = true;
                if (chkPriorAdj.IsChecked.Value)
                    adjChk = true;
                if (chkPriorAcc.IsChecked.Value)
                    accChk = true;
                if (chkTD.IsChecked.Value)
                    tdChk = true;
                if (chkBOM.IsChecked.Value)
                    bomChk = true;
                if (chkPO.IsChecked.Value)
                    poChk = true;
                if (chkWalk.IsChecked.Value)
                    walkChk = true;
                string commentTxt = "";

                if (extCost <= 0 && extCost > -1500)
                {
                    if (!currentChk || !oldChk || !adjChk || !accChk || !tdChk || !bomChk)
                    {
                        System.Windows.Forms.MessageBox.Show("You must indicate that you have checked all of the following:\n Current locations, Old Locations, Prior Adjustments/Account" +
                                            " Transactions, Teardowns, and BOM Issues.");
                        return;
                    }
                    commentTxt = txtComments.Text + " \\ Current locations counted \\ Old locations checked \\ Prior adjustments: " + txtPriorAdj.Text.Trim().ToUpper() +
                                                           " \\ Prior account transactions: " + txtPriorAcc.Text.Trim().ToUpper() + " \\ Teardowns: " + txtTD.Text.Trim().ToUpper() + " \\ BOM Issues checked.";
                }
                else
                {
                    if (extCost >= 0 && extCost < 1500)
                    {
                        if (!currentChk || !adjChk || !accChk || !bomChk)
                        {
                            System.Windows.Forms.MessageBox.Show("You must indicate that you have checked all of the following:\n Current locations, Prior Adjustments/Account Transactions," +
                                                " and BOM Issues.");
                            return;
                        }
                        commentTxt = txtComments.Text + " \\ Current locations counted \\ Prior adjustments: " + txtPriorAdj.Text.Trim().ToUpper() +
                                                               " \\ Prior account transactions: " + txtPriorAcc.Text.Trim().ToUpper() + " \\ BOM Issues checked.";
                    }
                }
                if (extCost <= -1500)
                {
                    if (!currentChk || !oldChk || !adjChk || !accChk || !tdChk || !bomChk || !poChk || !walkChk)
                    {
                        System.Windows.Forms.MessageBox.Show("For adjustments to write off $1500 or more you must indicate that you have checked Current locations, Old Locations," +
                                            " Prior Adjustments/Account Transactions, Teardowns, BOM Issues, if there are any PO Receipt errors and that you have" +
                                            " performed a Warehouse Walk.");
                        return;
                    }
                    commentTxt = txtComments.Text + " " + txtComments.Text.Trim().ToUpper() + " \\ Current locations counted \\ Old locations checked \\ Prior adjustments: " + txtPriorAdj.Text.Trim().ToUpper() +
                                                            " \\ Prior account transactions: " + txtPriorAcc.Text.Trim().ToUpper() + " \\ Teardowns: " + txtTD.Text.Trim().ToUpper() + " \\ BOM Issues checked " +
                                                            "\\ PO Receipts checked: " + txtPO.Text.Trim().ToUpper() + " \\ Warehouse Walk completed.";
                }
                if (extCost >= 1500)
                {
                    if (!currentChk || !adjChk || !accChk || !bomChk || !poChk)
                    {
                        System.Windows.Forms.MessageBox.Show("For adjustments to write on $1500 or more you must indicate that you have checked Current locations, Prior Adjustments/Account" +
                                        " Transactions, BOM Issues, and if there are any PO Receipt errors.");
                        return;
                    }
                    commentTxt = txtComments.Text + " \\ Current locations counted \\ Prior adjustments: " + txtPriorAdj.Text.Trim().ToUpper() +
                                                            " \\ Prior account transactions: " + txtPriorAcc.Text.Trim().ToUpper() + " \\ BOM Issues checked " +
                                                            "\\ PO Receipts checked: " + txtPO.Text.Trim().ToUpper();
                }

                #endregion Cost Validation

                dr[6] = commentTxt;
                dt.Rows.Add(dr);

                _total = Math.Round(Convert.ToDouble(_total), 2);
                lblTotalCost.Content = "$" + _total.ToString();

                dgSubmit.ItemsSource = dt.DefaultView;
                txtPID.Text = string.Empty;
                txtSeq.Text = string.Empty;
                txtAdj.Text = string.Empty;
                txtCheckCost.Text = string.Empty;
                txtComments.Text = string.Empty;
                chkCurrent.IsChecked = false;
                chkBOM.IsChecked = false;
                chkOldLoc.IsChecked = false;
                chkPO.IsChecked = false;
                chkPriorAcc.IsChecked = false;
                chkPriorAdj.IsChecked = false;
                chkTD.IsChecked = false;
                chkWalk.IsChecked = false;
                txtPriorAcc.Text = string.Empty;
                txtTD.Text = string.Empty;
                txtPriorAdj.Text = string.Empty;
                txtPO.Text = string.Empty;
            }
        }

        private void CheckExtCost()
        {
            if (txtPID.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter in a part number.");
                return;
            }
            if (txtAdj.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter in an adjustment qty.");
                return;
            }
            try
            {
                Convert.ToSingle(txtAdj.Text.Trim().ToUpper());
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Invalid Adjustment QTY entered: " + ex.Message);
                return;
            }

            unitCost = 0;
            string _pid = txtPID.Text.Trim().ToUpper();
            var uc = (from a in xps.UnitCosts
                      where a.Item == _pid
                      select a.Unit_Cost).First();
            unitCost = uc;

            adjQty = Convert.ToDouble(txtAdj.Text.Trim().ToUpper());

            try
            {
                extCost = adjQty * unitCost;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error calculating adjustment cost: " + ex.Message);
                return;
            }
            extCost = Math.Round(Convert.ToDouble(extCost), 2);
            txtCheckCost.Text = extCost.ToString();
        }

        private void SubmitRequest()
        {
            if (txtRequestor.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter your name in the Requestor box before submitting");
                return;
            }

            if (dt.Rows.Count < 1)
            {
                System.Windows.Forms.MessageBox.Show("Please add a request item to the sheet before submitting");
                return;
            }
            if (cboType.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Please select an adjustment type");
                return;
            }

            DateTime _subDate = DateTime.Now.Date;

            string _adjType = cboType.Text;
            string _requester = txtRequestor.Text.Trim().ToUpper();
            int? _fileID = 0;
            try
            {
                var q = (from a in xps.Adjustments
                         where a.ReqDate == _subDate && a.Type == _adjType
                         select a.FileID).Max();
                _fileID = q;
                if (_fileID != null)
                {
                    _fileID += 1;
                }
                else
                {
                    _fileID = 1;
                }
            }
            catch
            {
                _fileID += 1;
            }

            string _reqID = _subDate.ToString("MM/dd/yyyy") + "_" + _adjType + "_" + _fileID;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                Adjustment adj = new Adjustment();
                adj.PID = dr[0].ToString();
                adj.SEQ = dr[1].ToString();
                adj.Desc = dr[2].ToString();
                adj.Qty = Convert.ToDouble(dr[3]);
                adj.ABSQty = Math.Abs(Convert.ToDouble(dr[3]));
                adj.UnitCost = Convert.ToDouble(dr[4]);
                adj.NetValue = Convert.ToDouble(dr[5]);
                adj.ABSValue = Math.Abs(Convert.ToDouble(dr[5]));
                adj.Comments = dr[6].ToString();
                adj.ReqDate = DateTime.Now.Date;
                adj.Type = _adjType;
                adj.Status = "Needs Review";
                adj.FileID = _fileID;
                adj.ReqID = _reqID;
                adj.Requester = _requester;
                xps.Adjustments.Add(adj);
            }
            cboType.SelectedIndex = -1;
            xps.SaveChanges();
            System.Windows.Forms.MessageBox.Show("Count Sheet successfuly saved");
            dt.Clear();
            dgSubmit.ItemsSource = null;
        }

        private void DeleteSequence()
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                string _seqDT = dr[1].ToString();
                string _seqTxt = txtSeq.Text.Trim().ToUpper();
                if (_seqDT == _seqTxt)
                {
                    dt.Rows.Remove(dr);
                }
            }
        }

        private Tuple<double?, string> GetUnitInfo(string pid)
        {
            try
            {
                var uc = (from a in xps.UnitCosts
                          where a.Item == pid
                          select a).First();
                unitCost = uc.Unit_Cost;
                desc = uc.Description;
            }
            catch
            {
                System.Windows.Forms.DialogResult _diag = System.Windows.Forms.MessageBox.Show(
                    "No item cost or description found for " + pid + ". Would you like to add " +
                    "it to the database with a cost of $0.00?",
                    "Item not found", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (_diag == System.Windows.Forms.DialogResult.Yes)
                {
                    UnitCost _uc = new UnitCost();
                    _uc.Item = pid;
                    _uc.Description = "No Description";
                    _uc.Unit_Cost = 0;
                    xps.UnitCosts.Add(_uc);
                    xps.SaveChanges();
                    var uc = (from a in xps.UnitCosts
                              where a.Item == pid
                              select a).First();
                    unitCost = uc.Unit_Cost;
                    desc = uc.Description;
                }
                else if (_diag == System.Windows.Forms.DialogResult.No)
                {
                    System.Windows.Forms.MessageBox.Show("If you need to add this item to the database, please ask a lead or supervisor.");
                    return null;
                }
            }

            return new Tuple<double?, string>(unitCost, desc);
        }

        #endregion Methods
    }
}