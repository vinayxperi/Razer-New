

using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using Infragistics.Windows.DockManager;
using System;

namespace Cash
{

    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class CashFolder : ScreenBase, IScreen
    {

        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }
        private string BatchID { get; set; }
        public BatchObject _currentobject = null;

        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBaase constructor is also called.
        /// </summary>
        public CashFolder()
            : base()
        {
            // Create Controls
            InitializeComponent();

            // performs initializations for this object.

        }

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            // Change this if your folder has item directly bound to it.
            // In many cases the Tabs have data binding, the Folders
            // do not.
            this.DoNotSetDataContext = true;

            //Set the maintablename for the folder if it has one
            this.MainTableName = "cash_batch";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;
            // add the Tab user controls that are of type screen base
            TabCollection.Add(General);
            TabCollection.Add(Detail);



            //Debug code for hardwiring a test parameter set


            // Set the Header
            // HeaderName = "Contract-" + txtContractID.Text;

        }

        private void txtBatchID_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtBatchID.Text != null)
            {
                BatchID = txtBatchID.Text.ToString();
            }
            else
            {
                BatchID = "";
            }
        }
        /// <summary>
        /// Load bus obj parms, used in multiple places
        /// </summary>
        private void loadParms(string customerId)
        {
            try
            {
                //Clear the current parameters
                //if custId passed load external_char_id and recv_acct with passed customerId
                this.CurrentBusObj.Parms.ClearParms();
                //Add new parameters
                this.CurrentBusObj.Parms.AddParm("@batch_id", txtBatchID.Text);
                CurrentBusObj.Parms.AddParm("@document_id", "0");
                if (customerId != "")
                {
                    this.CurrentBusObj.Parms.AddParm("@remit_id", "1");
                }
                else
                {
                    //if custId NOT passed load external_char_id and recv_acct with global parm CustId if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.AddParm("@remit_id", "1");
                    }
                    //doing an insert setup dummy vals
                    else
                    {
                        this.CurrentBusObj.Parms.AddParm("@remit_id", "1");
                    }
                }
                //constant parms


            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }
        }
        private void txtBatchID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //If the Document ID changed then load a new document
            int number;
            bool result = Int32.TryParse(txtBatchID.Text, out number);
            if (result)
            {


                if (!string.IsNullOrEmpty(txtBatchID.Text) && txtBatchID.Text != BatchID)
                {              //Clear the current parameters

                    this.CurrentBusObj.Parms.ClearParms();
                    //Add new parameters
                    this.CurrentBusObj.Parms.AddParm("@batch_id", txtBatchID.Text);
                    CurrentBusObj.Parms.AddParm("@document_id", "0");
                    Detail.gAllocation.xGrid.DataItems.Clear();
                    Detail.gRemit.xGrid.DataItems.Clear();
                    this.Load();
                    if (CurrentBusObj.HasObjectData)
                    {
                        try
                        {


                        if (CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_status"].ToString() == "1")
                        {
                            CanExecuteDeleteCommand = false;

                        }
                        else
                        {
                            CanExecuteDeleteCommand = true;
                        }
                        }
                        catch 
                        {

                        }
                      
                    }

                }
                if (chkForData()) SetHeaderName();
            }
            else
            {
                Messages.ShowError("Batch Id must equal a number");

            }

        }
        public void OnDataLoad()
        {
            if (cGlobals.ReturnParms.Count > 0)
            {
                _currentobject = new BatchObject();
                _currentobject = (BatchObject)cGlobals.ReturnParms[0];
            }
        }
        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables[0].Rows.Count != 0)
            {
                windowCaption = "Cash Batch ID -" + this.CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0].ItemArray[0].ToString();
                return true;
            }
            else
            {
                Messages.ShowWarning("Batch Number Not Found.");
                return false;
            }
        }
        private void SetHeaderName()
        {
            //Sets the header name when being called from another folder
            if (txtBatchID.Text == null)
            {
                windowCaption = "Cash Batch ID -" + this.CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0].ItemArray[0].ToString();
                txtBatchID.Text = this.CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0].ItemArray[0].ToString();


            }
            //Sets the header name from within same folder
            else
            {
                ContentPane p = (ContentPane)this.Parent;
                p.Header = "Cash Batch ID -" + txtBatchID.Text;
            }
        }
        public override void New()
        {
            Messages.ShowInformation("The Cash Folder if for updating only.  Use Cash Entry to add a cash batch");
        }
        public override void Save()
        {
            bool isvalid = true;
            string error_message = "";
            DataTable allocTable = this.CurrentBusObj.ObjectData.Tables["cash_alloc"].Copy();
            decimal total_amnt = 0M;
             foreach (DataRow item in allocTable.Rows)
            {
                if (item.RowState != DataRowState.Deleted)
                {

                if (item["unapplied_flag"].ToString() == "1")
                {
                    if (!string.IsNullOrEmpty(item["apply_to_doc"].ToString().Trim()))
                    {
                        isvalid = false;
                        error_message = "Cannot allocate an unapplied with an invoice number" + Environment.NewLine;
                    }
                    if (item["product_code"].ToString().Length < 1)
                    {
                        isvalid = false;
                        error_message += "You must choose a product for an unapplied allocation" + Environment.NewLine;
                    }
                    if (item["receivable_account"].ToString().Length < 1)
                    {

                        isvalid = false;
                        error_message += "You must select a customer id for an unapplied allocation" + Environment.NewLine;
                    }
                }
                bool isblank = false;
                if ((item["unapplied_flag"].ToString().Trim() == "0") && (string.IsNullOrEmpty(item["receivable_account"].ToString().Trim()) && (string.IsNullOrEmpty(item["apply_to_doc"].ToString().Trim()))))
                {
                    isblank = true;
                    isvalid = false;
                    error_message += "You cannot have a blank allocation" + Environment.NewLine;
                }
                total_amnt += decimal.Parse(item["amount"].ToString());


                }
             }
             decimal amnt_fnct = 0M;
            DataTable remitTable = this.CurrentBusObj.ObjectData.Tables["cash_remit"];
            foreach (DataRow item in remitTable.Rows)
            {
                if (item.RowState != DataRowState.Deleted)
                {
                    if (string.IsNullOrEmpty(item["remit_number"].ToString()) && item["amount_functional"].ToString().Trim() == "0.00")
                    {
                        isvalid = false;
                        error_message += "You cannot have a blank remits" + Environment.NewLine;

                    }
                    else
                    {
                        amnt_fnct += decimal.Parse(item["amount_functional"].ToString());
                    }
                }
            }
            DataTable curcash = CurrentBusObj.ObjectData.Tables["cash_batch"];
            decimal cur_amt = decimal.Parse(curcash.Rows[0]["batch_total"].ToString());
            if (cur_amt != amnt_fnct)
            {
                isvalid = false;
                error_message += "You must remit the total of the current batch: $" + cur_amt + Environment.NewLine;

            }
            if (cur_amt != total_amnt)
            {
                isvalid = false;
                error_message = "You must allocate the total of the current batch: $" + cur_amt + Environment.NewLine;

            }
            if (isvalid)

            {

                base.Save();
                if (SaveSuccessful)
                {
                    Messages.ShowInformation("Saved Successfully");
                    int number;
                    bool result = Int32.TryParse(txtBatchID.Text, out number);
                    if (result)
                    {


                        if (!string.IsNullOrEmpty(txtBatchID.Text) && txtBatchID.Text != BatchID)
                        {              //Clear the current parameters

                            this.CurrentBusObj.Parms.ClearParms();
                            //Add new parameters
                            this.CurrentBusObj.Parms.AddParm("@batch_id", txtBatchID.Text);
                            CurrentBusObj.Parms.AddParm("@document_id", "0");
                            Detail.gAllocation.xGrid.DataItems.Clear();
                            Detail.gRemit.xGrid.DataItems.Clear();
                            this.Load();

                        }
                        if (chkForData()) SetHeaderName();
                    }
                }
            }
            else
            {
                Messages.ShowError(error_message);
            }
        }
        private void txtBatchID_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.CurrentBusObj.Parms.ClearParms();

            CashLookup f = new CashLookup();


            // gets the users response
            f.ShowDialog();
            if (cGlobals.ReturnParms.Count > 0)
            {
                // now we can load the account
                this.CurrentBusObj.Parms.ClearParms();
                this.CurrentBusObj.Parms.AddParm("@batch_id", cGlobals.ReturnParms[0].ToString());
                CurrentBusObj.Parms.AddParm("@document_id", "0");
                txtBatchID.Text = cGlobals.ReturnParms[0].ToString();
                // Call load 
                this.Load();
                //HeaderName = "Contract-" + txtContractID.Text;
                windowCaption = "Cash-" + txtBatchID.Text;
                BatchID = txtBatchID.Text;

                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }

        }

        private void tcMainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
    }

}
    public class BatchObject
    {
        public int BatchID { get; set; }
        public int SourceID { get; set; }
        public DateTime BatchDate { get; set; }
        public int BankID { get; set; }
        public decimal BatchTotal { get; set; }
        public int BatchStatus { get; set; }
        public DateTime AcctPeriod { get; set; }
        public string CurrencyCode { get; set; }

    }
