using RazerBase.Interfaces;
using RazerBase;
using RazerInterface;
using RazerBase.Lookups;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Linq;
using Microsoft.VisualBasic;
using Infragistics.Windows.DataPresenter.Events;



namespace Customer
{

    /// Interaction logic for CustomerDispute.xaml

    public partial class CustomerDispute : ScreenBase 
     
    {

        private static readonly string mainTableName = "main";
        public cBaseBusObject CustomerDisputeUpdate = new cBaseBusObject();
         
        string DocumentID = "";
        int SeqID;
        Boolean NewCMFlag = false;
        string detailFieldLayouts = "DisputeDetailGrid";
        

       

        public string WindowCaption { get { return string.Empty; } }
            
    

        public CustomerDispute(string documentID, int seqID, cBaseBusObject _CustomerBusObj)
            : base()
        {

            DocumentID = documentID;
            SeqID = seqID;

 

       

       
             
            InitializeComponent();

            // Perform initializations for this object
            Init();
           
        
    }

     
        public void Init()
        {
           
           
            this.CanExecuteSaveCommand = true;
            
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;

            this.MainTableName = "main";
            this.CurrentBusObj = CustomerDisputeUpdate;
            this.CurrentBusObj.Parms.AddParm("@document_id", DocumentID);

            this.CurrentBusObj.Parms.AddParm("@recv_doc_line", SeqID);


            //FieldLayoutSettings layouts = new FieldLayoutSettings();
            //layouts.HighlightAlternateRecords = true;
            ////layouts.AddNewRecordLocation = AddNewRecordLocation.OnTop;
            //idgDetails.xGrid.FieldLayoutSettings = layouts;
            idgDetails.FieldLayoutResourceString = detailFieldLayouts;
            idgDetails.SetGridSelectionBehavior(false, false);
            idgDetails.xGrid.FieldSettings.AllowEdit = true;

            idgDetails.MainTableName = "main";

            //idgDetails.xGrid.CellUpdated += new EventHandler<CellUpdatedEventArgs>(xGrid_CellUpdated);
            //idgDetails.xGrid.SelectedItemsChanged += new EventHandler<SelectedItemsChangedEventArgs>(xGrid_SelectedItemsChanged);
            idgDetails.xGrid.UpdateMode = UpdateMode.OnCellChange;//set obj
            

            GridCollection.Add(idgDetails);

            //name obj
            this.CurrentBusObj.BusObjectName = "CustomerDisputeUpdate";
            //Save customer business object for reload of aging tab

           


            this.Load();
                      
            //System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["main"] != null && CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {

                idgDetails.LoadGrid(CurrentBusObj, idgDetails.MainTableName);
                  
            }
         
         
             



        }

        // Save the changes
        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Check to see if there is data in the grid. If not return...
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count == 0)
            {
                return;
            }


                      
            //Need to loop through and set all records to new so they will get inserted.
            //The insert stored procedure will only insert records into the def_pool_dispute_amount table is the dispute amount <> 0

            //if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["document_id"].ToString() == "")
            //{//Need to set status_code to either 0 (None) or 3 Deferred
            //    this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["document_id"] = DocumentID.ToString();
            //    this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["recv_doc_line"] = SeqID.ToString();
            //    this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["posted_flag"] = 0;
               

            //}



          
            idgDetails.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);


            DataTable dt = this.CurrentBusObj.ObjectData.Tables["main"];
            if (dt != null)
            {

                foreach (DataRow row in dt.Rows)
                {   
                    if (row["dispute_amount"].ToString() == "")
                    {
                        row["dispute_amount"] = "0.00";
                    }

                    if (decimal.Parse(row["dispute_amount"].ToString())  > decimal.Parse(row["amount"].ToString()))
                    {

                        if (decimal.Parse(row["amount"].ToString()) < 0)
                        {

                            if  (decimal.Parse(row["dispute_amount"].ToString())  < decimal.Parse(row["amount"].ToString()))
                            {

                                Messages.ShowInformation("Dispute amount cannot be larger than the amount.");
                                return;
                            }


                        }
                        else
                        {

                            Messages.ShowInformation("Dispute amount cannot be larger than the amount.");
                            return;
                        }
                    }
                }
            }
                
            




            //DataTable dt = this.CurrentBusObj.ObjectData.Tables["main"];
            //if (dt != null)
            //{
            //    dt.AcceptChanges();
            //    foreach (DataRow row in dt.Rows)
            //    {
                   
            //            row.SetAdded();
                    
            //    }
            //}

            //idgDetails.LoadGrid(this.CurrentBusObj, idgDetails.MainTableName);

            this.Save();





        }


        // Save the changes
        private void btnAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {


            //Logic to set all the amounts to disputed
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count == 0)
            {
                return;
            }

            DataTable dt = this.CurrentBusObj.ObjectData.Tables["main"];
            if (dt != null)
            {
                 
                foreach (DataRow row in dt.Rows)
                {
                    row["dispute_amount"] = row["amount"];
                }
            }

            idgDetails.LoadGrid(this.CurrentBusObj, idgDetails.MainTableName);
            
            return;
            
        }

      



        public override void Save()
        {
             
            base.Save();
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Save Successful");
                //No need to refresh  
                //add customer_id parm for refresh of aging detail grid on aging tab                
               
               CloseScreen();
            }
            else
            {
                Messages.ShowInformation("No Changes to Save.");
            }
        }
        private void CloseScreen()
        {

            System.Windows.Window CustomerParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                CustomerParent.Close();
            }
        }

      

      

        

    }
}
