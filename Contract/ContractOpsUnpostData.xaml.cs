

using RazerBase;
using RazerInterface;
using System;
using System.Windows;
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Contract
{
    /// <summary>
    /// This class represents a 'ContractOpsUnpostData' object.
    /// </summary>
    public partial class ContractOpsUnpostData : ScreenBase
    {
        public cBaseBusObject ContractOpsUnpostDataBusObject = new cBaseBusObject();
        public int ContractId = 0;
        public bool IsSingleClickOrigin { get; set; }
        private DataRow rDefault;
        //contract object from caller
        cBaseBusObject ContractObj;

        /// Create a new instance of a 'ContractLocationServiceLookup' object and call the ScreenBase's constructor.

        public ContractOpsUnpostData(int _ContractId,  cBaseBusObject _ContractObj)
            : base()
        {
            //set obj
            this.CurrentBusObj = ContractOpsUnpostDataBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "ContractOpsUnpost";
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
            GridOpstoUnpost.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            GridOpstoUnpost.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridOpstoUnpost.ContextMenuAddIsVisible = false;
            GridOpstoUnpost.ContextMenuRemoveIsVisible = false;
            GridOpstoUnpost.MainTableName = "main";
            GridOpstoUnpost.ConfigFileName = "ContractOpsUnpost";
            GridOpstoUnpost.SetGridSelectionBehavior(true, true);
            GridOpstoUnpost.IsFilterable = true;
            GridOpstoUnpost.DoNotSelectFirstRecordOnLoad = true;
            GridOpstoUnpost.FieldLayoutResourceString = "GridUnpostOps";
            GridCollection.Add(GridOpstoUnpost);

          
            //load bus obj
            this.Load();
            //populate text boxes
            if (CurrentBusObj.ObjectData.Tables["main"] != null && CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                //txtFromContractID.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["from_contract_id"].ToString();
                //dtFromEndDate.SelText = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["from_end_date"]);
                //dtContractBillStartDate.SelText = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["to_start_date"]);
                 
            }

            
        }

       
      


        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Ops Data deleted from Posted Successful");
                //add contract_id parm for refresh of location grid on location tab
                ContractObj.Parms.UpdateParmValue ("@contract_id", ContractId);
                ContractObj.LoadTable("ops");
                CloseScreen();
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }


        private void btnUnpost_Click(object sender, RoutedEventArgs e)
        {
            //Loop throught the grid and if it is highlighted, set the move indicator on and move columns that need moved
            // SP will first insert the new locations to the new contract and set appropriate columns and then update old
            // SP will have to update the existing locations for the old contract id, set end date and if a comment entered, set that

            int ctr = 0;

            foreach (Record record in GridOpstoUnpost.xGrid.SelectedItems.Records)
            {
                ctr++;
                DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;

                r["unpost_indicator"] = 1;
            }

            if (ctr == 0)
            {
                MessageBox.Show("No Ops Data selected to unpost.");
                return;
            }
            Save();
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
