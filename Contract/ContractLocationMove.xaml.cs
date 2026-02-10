

using RazerBase;
using RazerInterface;
using System;
using System.Windows;
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Contract
{
    /// <summary>
    /// This class represents a 'ContractLocationMove' object.
    /// </summary>
    public partial class ContractLocationMove : ScreenBase
    {
        public cBaseBusObject ContractLocationMoveBusObject = new cBaseBusObject();
        public int ContractId = 0;
        public bool IsSingleClickOrigin { get; set; }
        private DataRow rDefault;
        //contract object from caller
        cBaseBusObject ContractObj;

        /// Create a new instance of a 'ContractLocationServiceLookup' object and call the ScreenBase's constructor.

        public ContractLocationMove(int _ContractId,  cBaseBusObject _ContractObj)
            : base()
        {
            //set obj
            this.CurrentBusObj = ContractLocationMoveBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "ContractLocationMove";
            ContractId = _ContractId;
            //get handle to contract obj
            ContractObj = _ContractObj;
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }

        
        /// This method performs initializations for this object.
       
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;

            this.MainTableName = "main";
            
            
            this.CurrentBusObj.Parms.AddParm("@contract_id", ContractId);

            this.CurrentBusObj.Parms.AddParm("@new_contract_id", 0);
            //setup locations grid
            GridLocationsAvailToMove.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridLocationsAvailToMove.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridLocationsAvailToMove.ContextMenuAddIsVisible = false;
            GridLocationsAvailToMove.ContextMenuRemoveIsVisible = false;
            GridLocationsAvailToMove.MainTableName = "availLocations";
            GridLocationsAvailToMove.ConfigFileName = "ContractLocationMoveLocations";
            GridLocationsAvailToMove.SetGridSelectionBehavior(true, true);
            GridLocationsAvailToMove.IsFilterable = true;
            GridLocationsAvailToMove.DoNotSelectFirstRecordOnLoad = true;
            GridLocationsAvailToMove.FieldLayoutResourceString = "GridAvailLocationsToMove";
            GridCollection.Add(GridLocationsAvailToMove);

          
            //load bus obj
            this.Load();
            //populate text boxes
            if (CurrentBusObj.ObjectData.Tables["main"] != null && CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                //txtFromContractID.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["from_contract_id"].ToString();
                //dtFromEndDate.SelText = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["from_end_date"]);
                //dtContractBillStartDate.SelText = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["to_start_date"]);
                txtSetFromComment.Text = "";
                txtToContractID.Text = "0";
            }

            
        }

       
        //Contract Lookup
        private void txtToContractID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on contract ID field
            RazerBase.Lookups.ContractLookup contractLookup = new RazerBase.Lookups.ContractLookup();
            contractLookup.Init(new cBaseBusObject("ContractLookup"));

            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();

            // gets the users response
            contractLookup.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                txtToContractID.Text = cGlobals.ReturnParms[0].ToString();
                 // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        private void txtToContractID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtToContractID.Text == "0")
            {
                MessageBox.Show("Invalid Contract ID to Move Location To.  Please choose a valid contract by double-clicking to invoke the lookup.");
                return;
            }
            CurrentBusObj.Parms.UpdateParmValue("@new_contract_id", txtToContractID.Text ?? string.Empty);
            CurrentBusObj.LoadTable("validate");
            if (CurrentBusObj.ObjectData.Tables["validate"] != null && CurrentBusObj.ObjectData.Tables["validate"].Rows.Count > 0)
            {
                //Check if contract is valid by checking valid_rows column 
                int validRows = 0;
                validRows = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["validate"].Rows[0]["valid_rows"]);
                if (validRows == 0)
                {



                    MessageBox.Show("Invalid Contract ID to Move Location To.  Please choose a valid contract by double-clicking to invoke the lookup.");
                }
            }
        }
       
       
        private void btnMove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Perform validations
            if (txtToContractID.Text == "")
            {
                MessageBox.Show("Contract ID to move locations to is required.");
                return;
            }

            if (txtToContractID.Text == "0")
            {
                MessageBox.Show("Invalid Contract ID to Move Location To.  Please choose a valid contract by double-clicking to invoke the lookup.");
                    return;
            }
            
            

            if (dtFromEndDate.SelText  ==  Convert.ToDateTime("1/1/1900 12:00:00 AM"))
            {
                MessageBox.Show("End Date to set on FROM Contract for locations moving is required.");
                return;
            }

            //Loop throught the grid and if it is highlighted, set the move indicator on and move columns that need moved
            // SP will first insert the new locations to the new contract and set appropriate columns and then update old
            // SP will have to update the existing locations for the old contract id, set end date and if a comment entered, set that

            int ctr = 0;
            
                    foreach (Record record in GridLocationsAvailToMove.xGrid.SelectedItems.Records)

                    {
                             ctr++;
                             DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                             r["to_contract_id"] = Convert.ToInt32(txtToContractID.Text);
                             r["new_contract_start_date"] = Convert.ToDateTime(dtContractBillStartDate.SelText);

                             r["old_end_bill_date"] = Convert.ToDateTime(dtFromEndDate.SelText);
                             r["old_comment"] = txtSetFromComment.Text.ToString();
                            r["move_indicator"] = 1;
                    }

                    if (ctr == 0)
                    {
                        MessageBox.Show("No locations selected to move.");
                        return;
                    }
            Save();
        }

        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Move Successful");
                //add contract_id parm for refresh of location grid on location tab
                ContractObj.Parms.UpdateParmValue ("@contract_id", ContractId);
                ContractObj.LoadTable("location");
                CloseScreen();
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
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

      

        private void dtFromEndDate_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //Need to calculate Contract Start date - date entered plus 1
            if (dtFromEndDate.SelText.ToString() == "1/1/1900")
            {
            }
            else
            {
                //date Plus 1 equal contract start date
              
                DateTime dt_work_end_date;
                dt_work_end_date = DateTime.Parse(dtFromEndDate.SelText.ToString());

                dtContractBillStartDate.SelText = DateTime.Parse(dt_work_end_date.ToString()).AddDays(1);
 
            }

                

        }

    }
}
