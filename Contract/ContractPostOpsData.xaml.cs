

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
    public partial class ContractPostOpsData : ScreenBase
    {
        public cBaseBusObject ContractPostOpsDataBusObject = new cBaseBusObject();
        public int ContractId = 0;
        public bool IsSingleClickOrigin { get; set; }
        private DataRow rDefault;
        //contract object from caller
        cBaseBusObject ContractObj;

        /// Create a new instance of a 'ContractLocationServiceLookup' object and call the ScreenBase's constructor.

        public ContractPostOpsData(int _ContractId,  cBaseBusObject _ContractObj)
            : base()
        {
            //set obj
            this.CurrentBusObj = ContractPostOpsDataBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "ContractPostOps";
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


            GridPostOps.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            GridPostOps.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridPostOps.ContextMenuAddIsVisible = false;
            GridPostOps.ContextMenuRemoveIsVisible = false;
            GridPostOps.MainTableName = "main";
            GridPostOps.ConfigFileName = "ContractPostOps";
            GridPostOps.SetGridSelectionBehavior(true, true);
            GridPostOps.IsFilterable = true;
            GridPostOps.DoNotSelectFirstRecordOnLoad = true;
            GridPostOps.FieldLayoutResourceString = "GridPostOps";
            GridCollection.Add(GridPostOps);

          
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
                Messages.ShowInformation("Ops Data Posted Successful");
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


        private void btnPost_Click(object sender, RoutedEventArgs e)
        {
            //Loop throught the grid and if it is highlighted, set the post indicator on and move columns that need posted
            // SP will first insert rows to billing_location_posted if they do not exist and it is not a rebill
            // SP then will delete the rows from billing_location

            int ctr = 0;

            foreach (Record record in GridPostOps.xGrid.SelectedItems.Records)
            {
                ctr++;
                DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;

                r["post_indicator"] = 1;
            }

            if (ctr == 0)
            {
                MessageBox.Show("No Ops Data selected to post.");
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
