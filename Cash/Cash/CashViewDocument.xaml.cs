using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RazerBase;
using RazerBase.Interfaces;
using System.Data;
using RazerBase.Lookups;
using Infragistics.Windows.DataPresenter;

namespace Cash
{
    /// <summary>
    /// Interaction logic for CashViewDocument.xaml
    /// </summary>
    public partial class CashViewDocument : ScreenBase, IScreen
    {

        ////Property is required for base objects that use IScreen
        public string WindowCaption { get; private set; }

        ////Screen Level Document ID
        public string DocumentID { get; set; }

   
        //////Setup keys for double click zooms from grids
        ////private List<string> gRemitDataKeys = new List<string> { "document_id"}; //Used for double click
        
        ////Miscellaneous variables and properties
        ////Variable for tracking a temporary remit id
        //private Int32 CurrentRemitID = 0;


        //Required constructor
        public CashViewDocument()
            : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialization event - this event should setup all tabs and grids as well as be able to handle being passed
        /// information from a zoom from another window.
        /// </summary>
        /// <param name="businessObject"></param>
        public void Init(cBaseBusObject businessObject)
        {
            WindowCaption = "View Cash";
        
            
            //Setting intial screenstate to empty
            CurrentState = ScreenState.Empty;

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder ;

             //Set the maintablename for the folder if it has one
            this.MainTableName = "general";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;

            //Add any Grid Configuration Information
            

            //add Grid
            gCashViewDocumentDetail.FieldLayoutResourceString = "cashViewDocumentDetail";
            gCashViewDocumentDetail.MainTableName = "detail";
            gCashViewDocumentDetail.ConfigFileName="CashViewDocumentConfig";

            //Add all grids to the grid collection - This allows grids to automatically load and participate with security
            GridCollection.Add(gCashViewDocumentDetail);

            //Add attachment grid as tab for security and proper functioning
            TabCollection.Add(gCashAttachments);

            //Debug code for hardwiring a test parameter set
            //CurrentBusObj.Parms.AddParm("@batch_id", "1" );

            // if there are parameters passed into the window on startup then we need to load the data
            if (CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                txtDocumentID.Text = CurrentBusObj.Parms.GetParm("@document_id");
                txtBatchID.Text = CurrentBusObj.Parms.GetParm("@batch_id");
                RetrieveData();
            }
            
        }

   
        /// <summary>
        /// Use the event to retrieve data into the base business object
        /// For Insert make sure to set CurrentState to Inserting
        /// </summary>
        /// <returns></returns>
        private bool RetrieveData()
        {
            this.CurrentBusObj.Parms.ClearParms();
            if (txtDocumentID.Text != null && txtDocumentID.Text != string.Empty)
            {
                this.CurrentBusObj.Parms.AddParm("@document_id", txtDocumentID.Text);
                this.CurrentBusObj.Parms.AddParm("@external_char_id", txtDocumentID.Text);
                //this.CurrentBusObj.Parms.AddParm("@batch_id", txtBatchID.Text );
                this.CurrentBusObj.Parms.AddParm("@remit_id", "-1");
 
            }
            else
            {
                this.CurrentBusObj.Parms.AddParm("@document_id", "-1");
                this.CurrentBusObj.Parms.AddParm("@batch_id", "-1");
                this.CurrentBusObj.Parms.AddParm("@remit_id", "-1");
                this.CurrentBusObj.Parms.AddParm("@external_char_id", "-1");
                DocumentID = string.Empty;
            }
            //constants
            this.CurrentBusObj.Parms.AddParm("@attachment_type", "SATTACH");
            this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
            this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");

            this.Load();

            //Verify that data was returned
            if (CurrentBusObj.ObjectData.Tables["general"] != null && CurrentBusObj.ObjectData.Tables["general"].Rows.Count>0)
            {
                DocumentID = txtDocumentID.Text;
                WindowCaption = "View Cash Document -" + txtDocumentID.Text ;
                this.HeaderName = WindowCaption;
                return true;
            }
            else
            {
                MessageBox.Show("No data retrieved for document id " + txtDocumentID.Text);
                //Reset the current remit ID
                DocumentID=string.Empty;
                this.HeaderName = "View Cash Document";
                txtDocumentID.Text = string.Empty;
                return false;
            }
        }

   

        public override void Save()
        {
            
            //If verified or deleting the save the data
            base.Save();
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Save Successful");
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
           
           
        }

        private void DocumentID_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.IsScreenDirty)
            {
                var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                {
                    this.Save();
                }

            }
            //Event handles opening of the lookup window upon double click on Location ID field
            CashDocumentLookup f = new CashDocumentLookup();

            this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {

                //load current parms
                //loadParms("");
                txtDocumentID.Text = cGlobals.ReturnParms[0].ToString();
                txtBatchID.Text = cGlobals.ReturnParms[1].ToString();
                cGlobals.ReturnParms.Clear();
                RetrieveData();

 
            }
        }

        private void txtDocumentID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDocumentID.Text != DocumentID)
            {
                RetrieveData();
            }
        }
  

        //public cBaseBusObject CashViewBusObject = new cBaseBusObject();
        //public string DocumentID;
        //public string BatchID;
        //private string gridPassedIn = "G";
        //public CashViewDocument()
        //{
        //    //set obj
        //    this.CurrentBusObj = CashViewBusObject;
        //    //name obj
        //    this.CurrentBusObj.BusObjectName = "CashViewDocument";
        //    // This call is required by the designer.
        //    InitializeComponent();
        //}

        //public void Init(cBaseBusObject businessObject)
        //{
        //    this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;
        //    this.MainTableName = "general";
        //    this.DoNotSetDataContext = false;
        //    //need this to activate attachments grid, otherwise it is dead
        //    TabCollection.Add(gCashAttachments);

        //    //add Grid
        //    gCashViewDocumentDetail.FieldLayoutResourceString = "cashViewDocumentDetail";
        //    gCashViewDocumentDetail.MainTableName = "detail";
        //    GridCollection.Add(gCashViewDocumentDetail);

        //    loadParms("");

        //    // if there are parameters load the data
        //    if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
        //    {
        //        this.Load();

        //        if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
        //        {
        //            windowCaption = "View Cash Document -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
        //            //txtDocumentID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
        //            //txtRemitType.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_type"].ToString();
        //            //txtRemitNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_number"].ToString();
        //            //txtRemitDate.Text = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_date"]).ToShortDateString();
        //            //txtBatchID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_id"].ToString();
        //            //txtTotal.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["amount"].ToString();
        //            //txtBatchDate.Text = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_date"].ToString()).ToShortDateString();
        //            //need to reload from general in case came from other window and parms were set to default values

        //            this.CurrentBusObj.Parms.ClearParms();
        //            this.CurrentBusObj.Parms.AddParm("@document_id", DocumentID);
        //            this.CurrentBusObj.Parms.AddParm("@external_char_id", DocumentID);
        //            this.CurrentBusObj.Parms.AddParm("@batch_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_id"].ToString());
        //            this.CurrentBusObj.Parms.AddParm("@remit_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_id"].ToString());
        //            this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
        //            this.Load(CurrentBusObj);
        //            //gCashViewDocumentDetail.LoadGrid(this.CurrentBusObj, "detail");

        //        }
        //    }
        //    if (cGlobals.ReturnParms.Count > 0)
        //    {
        //        txtDocumentID.Text = cGlobals.ReturnParms[0].ToString();
        //        txtBatchID.Text = cGlobals.ReturnParms[1].ToString();
        //        loadParms(txtDocumentID.Text);
        //        this.Load();
        //        if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
        //        {
        //            windowCaption = "View Cash Document -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
        //            //txtDocumentID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
        //            //txtRemitType.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_type"].ToString();
        //            //txtRemitNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_number"].ToString();
        //            //txtRemitDate.Text = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_date"]).ToShortDateString();
        //            //txtBatchID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_id"].ToString();
        //            //txtTotal.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["amount"].ToString();
        //            //txtBatchDate.Text = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_date"].ToString()).ToShortDateString();
        //            this.CurrentBusObj.Parms.ClearParms();
        //            this.CurrentBusObj.Parms.AddParm("@document_id", DocumentID);
        //            this.CurrentBusObj.Parms.AddParm("@external_char_id", DocumentID);
        //            this.CurrentBusObj.Parms.AddParm("@batch_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_id"].ToString());
        //            this.CurrentBusObj.Parms.AddParm("@remit_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_id"].ToString());
        //            this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
        //            this.Load(CurrentBusObj);
        //            //gCashViewDocumentDetail.LoadGrid(this.CurrentBusObj, "detail");

        //        }
        //        windowCaption = "View Cash Document - " + txtDocumentID.Text;
        //        DocumentID = txtDocumentID.Text;
        //        BatchID = txtBatchID.Text;
        //    }
        //    string sdebug = "";
        //}

        //private void DocumentID_DoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    //Event handles opening of the lookup window upon double click on Location ID field
        //    CashDocumentLookup f = new CashDocumentLookup();

        //    this.CurrentBusObj.Parms.ClearParms();

        //    // gets the users response
        //    f.ShowDialog();

        //    // Check if a value is returned
        //    if (cGlobals.ReturnParms.Count > 0)
        //    {

        //        //load current parms
        //        //loadParms("");
        //        txtDocumentID.Text = cGlobals.ReturnParms[0].ToString();
        //        txtBatchID.Text = cGlobals.ReturnParms[1].ToString();
        //        loadParms(txtDocumentID.Text);
        //        // Call load 
        //        this.Load();
        //        if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
        //        {
        //            windowCaption = "View Cash Document -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
        //            //txtDocumentID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
        //            //txtRemitType.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_type"].ToString();
        //            //txtRemitNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_number"].ToString();
        //            //txtRemitDate.Text = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_date"]).ToShortDateString();
        //            //txtBatchID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_id"].ToString();
        //            //txtTotal.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["amount"].ToString();
        //            //txtBatchDate.Text = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_date"].ToString()).ToShortDateString();  
        //            this.CurrentBusObj.Parms.ClearParms();
        //            this.CurrentBusObj.Parms.AddParm("@document_id", DocumentID);
        //            this.CurrentBusObj.Parms.AddParm("@external_char_id", DocumentID);
        //            this.CurrentBusObj.Parms.AddParm("@batch_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_id"].ToString());
        //            this.CurrentBusObj.Parms.AddParm("@remit_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_id"].ToString());
        //            this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
        //            this.Load(CurrentBusObj);
        //            //gCashViewDocumentDetail.LoadGrid(this.CurrentBusObj, "detail");

        //        }
        //        windowCaption = "View Cash Document - " + txtDocumentID.Text;
        //        DocumentID = txtDocumentID.Text;
        //        BatchID = txtBatchID.Text;

        //        // Clear the parms
        //        cGlobals.ReturnParms.Clear();
        //    }
        //}



        //private void btnView_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void txtDocumentID_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (txtDocumentID.Text != DocumentID)
        //    {
        //        ReturnData(txtDocumentID.Text, "@document_id");
        //        DocumentID = txtDocumentID.ToString();
        //    }


        //}

        //private bool chkForData()
        //{
        //    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
        //    {
        //        if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count != 0)

        //            return true;
        //        else
        //        {
        //            //Need to use the parameters from general to reload the detail
        //            this.CurrentBusObj.Parms.ClearParms();
        //            if (DocumentID != "")
        //            {
        //                windowCaption = "View Cash Document -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
        //                //txtDocumentID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
        //                //txtRemitType.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_type"].ToString();
        //                //txtRemitNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_number"].ToString();
        //                //txtRemitDate.Text =  Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_date"]).ToShortDateString();
        //                //txtBatchID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_id"].ToString();
        //                //txtTotal.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["amount"].ToString();
        //                //txtBatchDate.Text = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_date"].ToString()).ToShortDateString();  
        //                this.CurrentBusObj.Parms.AddParm("@document_id", DocumentID);
        //                this.CurrentBusObj.Parms.AddParm("@external_char_id", DocumentID);
        //                this.CurrentBusObj.Parms.AddParm("@batch_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["batch_id"].ToString());
        //                this.CurrentBusObj.Parms.AddParm("@remit_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["remit_id"].ToString());
        //                this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
        //                this.Load(CurrentBusObj, false, "detail");
        //                gCashViewDocumentDetail.LoadGrid(this.CurrentBusObj, "detail");
        //            }

        //        }
        //        return true;
        //    }
        //    else
        //    {
        //        Messages.ShowWarning("Cash Document Not Found");
        //        return false;
        //    }
        //}


        ///// <summary>
        ///// pop screen must be public because used in modal screens
        ///// </summary>
        ///// <param name="SearchValue"></param>
        ///// <param name="DbParm"></param>
        //public void ReturnData(string SearchValue, string DbParm)
        //{
        //    //if no value do nothing
        //    if (SearchValue == "") return;
        //    //Add new parameters
        //    loadParms(SearchValue);

        //    //load data
        //    //if coming from save do not do this...
        //    this.Load();
        //    //if invoiceNumber found then set header and pop otherwise send message
        //    if (chkForData())
        //    {
        //        //SetHeaderName();

        //    }
        //}

        //private void loadParms(string DocumentID)
        //{
        //    try
        //    {
        //        //Clear the current parameters
        //        this.CurrentBusObj.Parms.ClearParms();
        //        //if adjustment number passed load document id
        //        if (DocumentID != "")
        //        {
        //            this.CurrentBusObj.Parms.AddParm("@document_id", DocumentID);
        //            //this.CurrentBusObj.Parms.AddParm("@batch_id", BatchID);
        //            this.CurrentBusObj.Parms.AddParm("@external_char_id", DocumentID);
        //            this.CurrentBusObj.Parms.AddParm("@batch_id", "-1");
        //            this.CurrentBusObj.Parms.AddParm("@remit_id", "-1");

        //        }
        //        else
        //        {
        //            //if   NOT passed load   with global parm document_id if exists
        //            if (cGlobals.ReturnParms.Count > 0)
        //            {
        //                this.CurrentBusObj.Parms.AddParm("@document_id", cGlobals.ReturnParms[0].ToString());
        //                if (cGlobals.ReturnParms[1].ToString().Substring(0, 1) == gridPassedIn)
        //                {
        //                    DocumentID = cGlobals.ReturnParms[0].ToString();
        //                    //set dummy vals if coming from another window
        //                    this.CurrentBusObj.Parms.AddParm("@batch_id", "-1");
        //                    this.CurrentBusObj.Parms.AddParm("@remit_id", "-1");
        //                    this.CurrentBusObj.Parms.AddParm("@external_char_id", cGlobals.ReturnParms[0].ToString());


        //                }
        //                else
        //                {
        //                    this.CurrentBusObj.Parms.AddParm("@batch_id", cGlobals.ReturnParms[1].ToString());
        //                    this.CurrentBusObj.Parms.AddParm("@remit_id", cGlobals.ReturnParms[2].ToString());
        //                    //for attachments
        //                    this.CurrentBusObj.Parms.AddParm("@external_char_id", cGlobals.ReturnParms[0].ToString());
        //                }
        //            }
        //            //set dummy vals
        //            else
        //            {

        //                this.CurrentBusObj.Parms.AddParm("@document_id", "-1");
        //                //for attachments
        //                this.CurrentBusObj.Parms.AddParm("@batch_id", "-1");
        //                this.CurrentBusObj.Parms.AddParm("@remit_id", "-1");
        //                this.CurrentBusObj.Parms.AddParm("@external_char_id", "-1");
        //            }
        //        }
        //        //constants
        //        this.CurrentBusObj.Parms.AddParm("@attachment_type", "SATTACH");
        //        this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
        //        this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");

        //    }
        //    catch (Exception ex)
        //    {
        //        Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
        //    }

        //}
        //private void txtDocumentID_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{

        //}

        //public override void Save()
        //{
        //    //put this in because unidentified code is making changes to the general table causing the save to fail.
        //    this.CurrentBusObj.ObjectData.Tables["general"].RejectChanges();
        //    this.CurrentBusObj.ObjectData.Tables["general"].AcceptChanges();

        //    base.Save();
        //    if (SaveSuccessful)
        //    {
        //        ////the following is required to get the detail grid to reload
        //        //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
        //        //{
        //        //    this.Load(CurrentBusObj, false, "detail");
        //        //    gCashViewDocumentDetail.LoadGrid(this.CurrentBusObj, "detail");
        //        //}
        //        ////check if attachment tab files need to be copied
        //        //if (cGlobals.GlobalAttachmentsStorageList.Count > 0)
        //        //{
        //        //    //if so pass attachment data table to attachment helper class
        //        //    AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
        //        //    attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables["Attachments"]);

        //        //}

        //        Messages.ShowInformation("Save Successful");


        //    }
        //    else
        //    {
        //        Messages.ShowInformation("Save Failed");
        //    }
        //}
    }
}
