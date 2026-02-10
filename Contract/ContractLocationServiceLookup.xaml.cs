

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Contract
{
    /// <summary>
    /// This class represents a 'ContractLocationServiceLookup' object.
    /// </summary>
    public partial class ContractLocationServiceLookup : ScreenBase
    {
        public cBaseBusObject ContractLocationServiceBusObject = new cBaseBusObject();
        public int ContractId = 0;
        public bool IsSingleClickOrigin { get; set; }
        private DataRow rDefault;
        //contract object from caller
        cBaseBusObject ContractObj;
        /// <summary>
        /// Create a new instance of a 'ContractLocationServiceLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractLocationServiceLookup(int _ContractId, cBaseBusObject _ContractObj)
            : base()
        {
            //set obj
            this.CurrentBusObj = ContractLocationServiceBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "ContractLocationService";
            ContractId = _ContractId;
            //get handle to contract obj
            ContractObj = _ContractObj;
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;

            this.MainTableName = "contract_location_service";
            //dtpStartDate.SelectedDateChangedDelegate = SelectedStartDateChanged;
            //dtpEndDate.SelectedDateChangedDelegate = SelectedEndDateChanged;
            this.CurrentBusObj.Parms.AddParm("@contract_id", ContractId);


            //setup locations grid
            GridLocationsAll.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridLocationsAll.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridLocationsAll.ContextMenuAddIsVisible = false;
            GridLocationsAll.ContextMenuRemoveIsVisible = false;
            GridLocationsAll.MainTableName = "contract_location_service";
            GridLocationsAll.ConfigFileName = "ContractLocationServiceAllLocations";
            GridLocationsAll.SingleClickZoomDelegate = SingleClickZoomDelegateOrigin;
            GridLocationsAll.SetGridSelectionBehavior(true, false);
            GridLocationsAll.IsFilterable = true;
            GridLocationsAll.FieldLayoutResourceString = "GridLocationAllLocations";
            GridCollection.Add(GridLocationsAll);

            GridLocationsFinal.MainTableName = "contract_location_final";
            GridLocationsFinal.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridLocationsFinal.ContextMenuAddIsVisible = false;
            GridLocationsFinal.ContextMenuRemoveIsVisible = false;
            GridLocationsFinal.ConfigFileName = "ContractLocationServiceFinal";
            GridLocationsFinal.SingleClickZoomDelegate = SingleClickZoomDelegateSourceFinal;
            GridLocationsFinal.SetGridSelectionBehavior(true, false);
            GridLocationsFinal.FieldLayoutResourceString = "GridLocationsFinal";
            GridLocationsFinal.xGrid.FieldLayoutSettings.SelectionTypeRecord = Infragistics.Windows.Controls.SelectionType.Extended;
            GridCollection.Add(GridLocationsFinal);

            //load bus obj
            this.Load();

            //Establish default values
            if (CurrentBusObj.ObjectData.Tables["contract_location_defaults"].Rows  != null)
            {
                rDefault = CurrentBusObj.ObjectData.Tables["contract_location_defaults"].Rows[0];
                txtCustomerID.Text = Convert.ToString(rDefault["receivable_account"]);
                dtpBillActiveDate.SelText = Convert.ToDateTime(rDefault["bill_activation_date"]);
                dtpBeginBillDate.SelText = Convert.ToDateTime(rDefault["begin_bill_date"]);
                dtpModifyLaunchDate.SelText = Convert.ToDateTime(rDefault["modified_launch_date"]);
                chkNeverBill.IsChecked = 0;
                chkGroupBill.IsChecked = 1;
                chkTransfer.IsChecked = 0;
                dtpContractBillStartDate.SelText = Convert.ToDateTime(rDefault["bill_activation_date"]);
            }
        }

        /// <summary>
        /// single click in this grid loads template data and grid data to workspace grid
        /// </summary>
        public void SingleClickZoomDelegateOrigin()
        {
            //This first if statement checks to see if a filter row has been selected - (index -1).  if it has it exits the sub
            if (GridLocationsAll.xGrid.ActiveRecord.Index != -1)
            {

                //set this to prevent gridLocationfinal single click delegate from firing when GridLocationsFinal.xGrid.ActiveDataItem = row; runs
                IsSingleClickOrigin = true;
                System.Collections.Generic.List<string> LocationFieldList = new System.Collections.Generic.List<string>();
                LocationFieldList.Add("product_code");
                LocationFieldList.Add("cs_id");
                LocationFieldList.Add("cs_name");
                LocationFieldList.Add("psa_city");
                LocationFieldList.Add("psa_state");
                LocationFieldList.Add("mso_id");
                LocationFieldList.Add("mso_name");
                //LocationFieldList.Add("receivable_account");
                cGlobals.ReturnParms.Clear();
                GridLocationsAll.ReturnSelectedData(LocationFieldList);
                if (cGlobals.ReturnParms.Count > 0)
                {
                    DataView dataSource = this.GridLocationsFinal.xGrid.DataSource as DataView;

                    //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
                    //Add new grid row and set cursor in first cell of last row
                    DataRowView row = dataSource.AddNew();
                    GridLocationsFinal.xGrid.ActiveDataItem = row;
                    GridLocationsFinal.xGrid.ActiveCell = (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
                    //product
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = cGlobals.ReturnParms[0];
                    //cs_id
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = cGlobals.ReturnParms[1];
                    //cs_name
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[2];
                    //city
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = cGlobals.ReturnParms[3];
                    //state
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[4];
                    //mso_id
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = cGlobals.ReturnParms[5];
                    //mso_name
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = cGlobals.ReturnParms[6];
                    //recv acct
                    if (txtCustomerID.Text != "" && txtCustomerID.Text != null)
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = txtCustomerID.Text.Trim();
                    else
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = "0";
                    //bill active date
                    if (dtpBillActiveDate.SelText.ToString() != "" && dtpBillActiveDate.SelText != null)
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = dtpBillActiveDate.SelText.ToString().Trim();
                    else
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = "01/01/1900";
                    //bill begin date
                    if (dtpBeginBillDate.SelText.ToString() != "" && dtpBeginBillDate.SelText != null)
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = dtpBeginBillDate.SelText.ToString().Trim();
                    else
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = "01/01/1900";
                    //modify date
                    if (dtpModifyLaunchDate.SelText.ToString() != "" && dtpModifyLaunchDate.SelText != null)
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[10].Value = dtpModifyLaunchDate.SelText.ToString().Trim();
                    else
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[10].Value = "01/01/1900";
                    //never bill
                    if (chkNeverBill.IsChecked == 1 && chkNeverBill != null)
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[11].Value = 1;
                    else
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[11].Value = 0;
                    //group bill 
                    if (chkGroupBill.IsChecked == 1 && chkGroupBill != null)
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[12].Value = 1;
                    else
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[12].Value = 0;
                    //transfer flag
                    if (chkTransfer.IsChecked == 1 && chkTransfer != null)
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[13].Value = 1;
                    else
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[13].Value = 0;
                    //constant values///////////////////////////////////////////////////////////////////////////////////////
                    //hold_flag int
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[14].Value = 0;
                    //hold_date date
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[15].Value = "01/01/1900";
                    //hold_reason varchar(80)
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[16].Value = "";
                    //termination_date date
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[17].Value = "01/01/1900";
                    //termination_reason varchar(80)
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[18].Value = "";
                    //termination_acct_date date
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[19].Value = "01/01/1900";
                    //last_bill_date date
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[20].Value = "01/01/1900";
                    //end_date date
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[21].Value = "01/01/1900";
                    //nto_date date
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[22].Value = "01/01/1900";
                    //nto_acct_date date
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[23].Value = "01/01/1900";
                    //reference varchar(40)
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[24].Value = "";
                    //contract_bill_start_date datetime
                    if (dtpContractBillStartDate.SelText.ToString() != "" && dtpContractBillStartDate.SelText != null)
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[25].Value = dtpContractBillStartDate.SelText.ToString().Trim();
                    else
                        (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[25].Value = "01/01/1900";

                    //comment varchar(255)
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[26].Value = "";
                    //is_dirty_flag int
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[27].Value = 0;
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //contract id
                    (GridLocationsFinal.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[28].Value = ContractId;

                    GridLocationsFinal.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    IsSingleClickOrigin = false;
                }
                else
                {
                    IsSingleClickOrigin = false;
                    return;
                }
            }
        }

        /// <summary>
        /// single click in this grid removes the active record
        /// </summary>
        public void SingleClickZoomDelegateSourceFinal()
        {
            //make sure this single click delegate is not fired by the origin single click delegate
            if (IsSingleClickOrigin == false)
            {
                DataPresenterCommands.DeleteSelectedDataRecords.Execute("nothing", GridLocationsFinal);
                //if (GridLocationsFinal.xGrid.ActiveRecord.Index != null && GridLocationsFinal.xGrid.ActiveRecord.Index >= 0)
                //    GridLocationsFinal.xGrid.DataItems.RemoveAt(GridLocationsFinal.xGrid.ActiveRecord.Index);
            }
        }

        /// <summary>
        /// pulls up customer lookup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCustomerID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on contract ID field
            RazerBase.Lookups.CustomerLookup custLookup = new RazerBase.Lookups.CustomerLookup();
            custLookup.Init(new cBaseBusObject("CustomerLookup"));

            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();

            // gets the users response
            custLookup.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                txtCustomerID.Text = cGlobals.ReturnParms[0].ToString();
                //// Call load 
                //this.Load();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        /// <summary>
        /// clear template controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void chkEnableTemplate_UnChecked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    txtCustomerID.Text = "";
        //    dtpBillActiveDate.SelText = Convert.ToDateTime("01/01/1900");
        //    dtpBeginBillDate.SelText = Convert.ToDateTime("01/01/1900");
        //    dtpModifyLaunchDate.SelText = Convert.ToDateTime("01/01/1900");
        //    chkNeverBill.IsChecked = 0;
        //    chkGroupBill.IsChecked = 0;
        //    chkTransfer.IsChecked = 0;

        //}

        /// <summary>
        /// check that a valid customer num is entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCustomerID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            int valChk = -1;
            //make sure cust id is a number
            bool result = Int32.TryParse(txtCustomerID.Text, out valChk);
            if (!result)
            {
                Messages.ShowInformation("Invalid Customer Num Entered");
                txtCustomerID.Focus();
            }
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Save();
        }

        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Save Successful");
                //Check that comment_attachment_id parm exists, if not create.
                if (CommentAttachParmExists())
                    this.CurrentBusObj.changeParm("@contract_id", ContractId.ToString());
                else
                    this.CurrentBusObj.Parms.AddParm("@contract_id", ContractId.ToString());
                ContractObj.LoadTable("location");
                CloseScreen();
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }

        private bool CommentAttachParmExists()
        {
            var row = from x in this.CurrentBusObj.Parms.ParmList.AsEnumerable()
                      where x.Field<string>("parmName") == "@contract_id"
                      select x.Field<string>("Value");

            foreach (var val in row)
            {
                //return true if comment_attachment_id found in parm list
                return true;
            }
            return false;
        }

        private void CloseScreen()
        {
            System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                AdjParent.Close();
            }
        }

    }
}
