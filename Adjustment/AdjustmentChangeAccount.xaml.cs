

using RazerBase;
using RazerInterface;
using System;
using System.Windows;
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Adjustment
{
    /// <summary>
  
    /// </summary>
    public partial class AdjustmentChangeAccount : ScreenBase
    {
        public cBaseBusObject AdjustmentChangeAccountBusObject = new cBaseBusObject();
        public string DocumentId = "";
        public bool InvalidAcct = false;
         
        //contract object from caller
        cBaseBusObject AdjustmentObj;

        /// Create a new instance of a 'ContractLocationServiceLookup' object and call the ScreenBase's constructor.

        public AdjustmentChangeAccount(string _DocumentId, cBaseBusObject _AdjustmentObj)
            : base()
        {
            //set obj
            this.CurrentBusObj = AdjustmentChangeAccountBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentChangeAccount";
            DocumentId = _DocumentId;
            //get handle to Adjustment obj
            AdjustmentObj = _AdjustmentObj;
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
            
            
            this.CurrentBusObj.Parms.AddParm("@document_id", DocumentId);
            //setup locations grid
            GridAdjustmentChangeAcct.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridAdjustmentChangeAcct.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            GridAdjustmentChangeAcct.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridAdjustmentChangeAcct.ContextMenuAddIsVisible = false;
            GridAdjustmentChangeAcct.ContextMenuRemoveIsVisible = false;
            GridAdjustmentChangeAcct.MainTableName = "main";
            GridAdjustmentChangeAcct.ConfigFileName = "AdjustmentChangeAcctCode";
            GridAdjustmentChangeAcct.xGrid.FieldSettings.AllowEdit = true;
            GridAdjustmentChangeAcct.SetGridSelectionBehavior(false, false);
            GridAdjustmentChangeAcct.DoNotSelectFirstRecordOnLoad = true;
            GridAdjustmentChangeAcct.FieldLayoutResourceString = "AdjustmentChangeAcctCode";
            GridCollection.Add(GridAdjustmentChangeAcct);
            this.CurrentBusObj.Parms.AddParm("@error_message", "");
            this.CurrentBusObj.Parms.AddParm("@account_code", "");
            this.Load();
           
           
           

            
        }

        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            InvalidAcct = false;
            e.Handled = true;
            int edit_index = GridAdjustmentChangeAcct.ActiveRecord.Cells.IndexOf(e.Cell);
            DataRecord GR = GridAdjustmentChangeAcct.ActiveRecord;
            if (GR.Cells["account_code"].Value.ToString() == "")
            {
                Messages.ShowInformation("The Account Code must be entered.");
            }
           
                //commit user entered value to datatable
                GridAdjustmentChangeAcct.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                DataRecord GridRecord = null;
                GridRecord = GridAdjustmentChangeAcct.ActiveRecord;
                DataRowView dr = GridRecord.DataItem as DataRowView;
                DataView dv = dr.DataView;
               
                //validate GL info
                DataRecord row = (DataRecord)GridAdjustmentChangeAcct.xGrid.ActiveRecord;
                //change parms for GL Validation
                this.CurrentBusObj.changeParm("@account_code", row.Cells["account_code"].Value.ToString());
                this.CurrentBusObj.LoadTable("glValidate");
                if (this.CurrentBusObj.ObjectData.Tables["glValidate"].Rows.Count < 1)
                {
                    Messages.ShowInformation("The Account Code is Invalid.");
                    InvalidAcct = true;
                }
            }
        
       
       
        private void btnUpdate_Click(object sender, System.Windows.RoutedEventArgs e)
        {



            GridAdjustmentChangeAcct.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);


                       
            Save();
        }

        public override void Save()
        {
            if (InvalidAcct)
            {
                MessageBox.Show("Cannot Save with an Invalid Account");
                return;
            }

            base.Save();
            if (SaveSuccessful)
            {
                  
                    //error messages from Stored Proc
                    string strSPErrMg = getInfoFromStoredProc();
                    if (strSPErrMg != null && strSPErrMg != "")
                    {
                        Messages.ShowError(strSPErrMg);
                        return;
                    }
                    else
                    {
                        Messages.ShowInformation("Account Code Changes Successful");
                        //add contract_id parm for refresh of location grid on location tab
                          AdjustmentObj.Parms.UpdateParmValue("@document_id", DocumentId);
                        AdjustmentObj.LoadTable("detailacctgrid");
                        AdjustmentObj.LoadTable("acctdetail");
                        CloseScreen();
                    }
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }

        private string getInfoFromStoredProc()
        {
            var SPErrorMsg = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                             where x.Field<string>("parmName") == "@error_message"
                             select new
                             {
                                 parmName = x.Field<string>("parmName"),
                                 parmValue = x.Field<string>("parmValue")
                             };
            foreach (var info in SPErrorMsg)
            {
                if (info.parmName == "@error_message")
                    return info.parmValue;
            }
            return "";
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

        private void ScreenBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentBusObj.ObjectData.Tables["main"] != null && CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                btnUpdate.IsEnabled = true;
            }
            else
            {
                btnUpdate.IsEnabled = false;
                MessageBox.Show("No account codes available to update.");
                //AdjustmentObj.Parms.UpdateParmValue("@document_id", DocumentId);
                ////AdjustmentObj.LoadTable("acctdetail");
                System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);


                if (!ScreenBaseIsClosing)
                {
                    AdjParent.Close();
                }

            }
        }

      

        

    }
}
