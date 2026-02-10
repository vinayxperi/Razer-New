



using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using Infragistics.Windows.DataPresenter;



namespace Customer
{


    /// <summary>
    /// This class represents a 'CustomerHistoryTab' object.
    /// </summary>
    public partial class CustomerHistoryTab : ScreenBase
    {
        private List<string> dataKeys = new List<string> { "document_id", "document_type" }; //Used for double click
        
        


        /// <summary>
        /// Create a new instance of a 'CustomerHistoryTab' object and call the ScreenBase's constructor.
        /// </summary>
       
        public CustomerHistoryTab()
            : base()
        {
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
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //The main table used on the tab item is the account_history table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "account_history";
            GridAcctHistory.MainTableName = "account_history";
            GridAcctHistory.ConfigFileName = "CustomerAcctHistory";
            GridAcctHistory.ContextMenuAddIsVisible = false;
            GridAcctHistory.ContextMenuRemoveIsVisible = false;
            GridAcctHistory.ContextMenuGenericDisplayName1 = "Email Invoice Attachments";
            GridAcctHistory.ContextMenuGenericIsVisible1 = true;
            GridAcctHistory.ContextMenuGenericDelegate1 = EmailInvoiceAttachments;
            GridAcctHistory.WindowZoomDelegate = GridDoubleClickDelegate;
            GridAcctHistory.IsFilterable = true;
            
            //Changes to highlight child rows
            GridAcctHistory.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            GridAcctHistory.GridCellValuePresenterStyle = CellStyle;
            //
            
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridAcctHistory.SetGridSelectionBehavior(true, true);
            GridAcctHistory.FieldLayoutResourceString = "CustomerAcctHistoryGrid";
            GridCollection.Add(GridAcctHistory);
            

            

            this.HasPrintReport = true;
            //CustomerAcctHistoryGridAll
            //GridAcctHistoryAll.Visibility = System.Windows.Visibility.Collapsed;
            //GridAcctHistoryAll.MainTableName = "account_history_all";
            //GridAcctHistoryAll.WindowZoomDelegate = GridDoubleClickDelegate;
            //GridAcctHistoryAll.IsFilterable = true;
            //if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            //GridAcctHistoryAll.SetGridSelectionBehavior(true, false);
            //GridAcctHistoryAll.FieldLayoutResourceString = "CustomerAcctHistoryGridAll";
            //GridCollection.Add(GridAcctHistoryAll);
        }

       
        /// <summary>
        /// Delegate for grid 
        /// </summary>
        public void GridDoubleClickDelegate()
        {
            ////call contracts folder
            //GridAcctHistory.ReturnSelectedData("document_id");
            //cGlobals.ReturnParms.Add("GridAcctHistory.xGrid");
            //RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            //args.Source = GridAcctHistory.xGrid ;
            //EventAggregator.GeneratedClickHandler(this, args);  
    
            
            GridAcctHistory.ReturnSelectedData(dataKeys);
            //Determine the document type and then set appropriate double click destination
            switch (cGlobals.ReturnParms[1].ToString().ToLower())
            {
                case "adjust":
                    cGlobals.ReturnParms[1] = ("AdjustmentZoom");
                    break;
                case "invoice":
                    cGlobals.ReturnParms[1] = ("InvoiceZoom");
                    break;
                case "cash":
                    cGlobals.ReturnParms[1] = ("GridCashZoom");
                    break;
                case "minvoice":
                    cGlobals.ReturnParms[1] = ("CustomInvoiceZoom");
                    break;

                default:
                    cGlobals.ReturnParms[1] = ("CustomerViewZoom");
                    break;

            }
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
           
            args.Source = GridAcctHistory.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);
        }


        public void GridContextMenuGenericDelegate1()
        {
          // List<string> AttachList = new List<string>();
          // string[] AttachmentsToSend;
          // int iCount = 0;
                        
          //  //Cycle through selected documents
          // //Check for invoice or custom invoice type
          // foreach (DataRecord r in GridAcctHistory.xGrid.SelectedItems)
          // {
          //     if (Convert.ToString(r.Cells["detail_type"].Value ).ToLower().Trim() == "invoice" || Convert.ToString(r.Cells["detail_type"].Value ).ToLower().Trim() == "minvoice")
          //      {

          //          //Add to attach list
          //         AttachList.Add(Convert.ToString(r.Cells["document_id"].Value).Trim());
          //     }

          // }

          ////Run the webservice to find the file locations if any rows were returned
          // //Call billing service SP to get the document location
          // //Find the PDF that goes with the document ID
          // if (AttachList.Count > 0)
          // {
          //     AttachmentsToSend = cGlobals.BillService.GetAttachmentsForEmail(AttachList.ToArray());
          //     if (AttachmentsToSend != null && AttachmentsToSend.Length > 0)
          //     {
          //         cMailMessage MailMessage = new cMailMessage();
          //         MailMessage.CreateEmail("", "", "Invoices", "", AttachmentsToSend);
          //     }
          //     else
          //         MessageBox.Show("Error - Attachment files not found.");
          // }
          // else
          //     MessageBox.Show("Error - No Valid attachments found. Must be of Invoice or Custom Invoice Types");


          //  //If any documents qualified then send mail
           

         }

        public override void PrintReport()
        {
            string validAcct;

            if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["general"] != null && CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {
                validAcct = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["receivable_account"].ToString();
                if (validAcct == "")
                {
                    MessageBox.Show("Customer Number required to run Report.");
                    return;
                }
                else
                {
                    PrintReportJobName = "Customer Acct History Tab Print";
                    PrintReportParms.Add(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["receivable_account"].ToString());
                    base.PrintReport();
                }
            }

        }

        private void EmailInvoiceAttachments()
        {
            string emailAcct;
            //opens ContractOpsUnpostData Window
            if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["general"] != null && CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {
                emailAcct = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["receivable_account"].ToString();
                if (emailAcct == "")
                {
                    MessageBox.Show("Customer Number required to email invoices.");
                    return;
                }
                CustomerEmailInvoice CustomerEmailInvoiceScreen = new CustomerEmailInvoice(emailAcct, this.CurrentBusObj);
                //////////////////////////////////////////////////////////////
                //create a new window , show it as a dialog
                System.Windows.Window CustomerEmailInvoiceWindow = new System.Windows.Window();
                //set new window properties///////////////////////////
                CustomerEmailInvoiceWindow.Title = "Customer Email Invoice attachments ";
                CustomerEmailInvoiceWindow.MaxHeight = 1880;
                CustomerEmailInvoiceWindow.MaxWidth = 2280;
                /////////////////////////////////////////////////////
                //set screen as content of new window
                CustomerEmailInvoiceWindow.Content = CustomerEmailInvoiceScreen;
                //open new window with embedded user control
                CustomerEmailInvoiceWindow.ShowDialog();
            }
        }

        //private void chkShowHistoy_UnChecked(object sender, RoutedEventArgs e)
        //{
        //    GridAcctHistory.Visibility = System.Windows.Visibility.Visible;
        //    GridAcctHistoryAll.Visibility = System.Windows.Visibility.Collapsed;
        //}

        //private void chkShowHistoy_Checked(object sender, RoutedEventArgs e)
        //{
        //    //show all history
        //    GridAcctHistory.Visibility = System.Windows.Visibility.Collapsed;
        //    GridAcctHistoryAll.Visibility = System.Windows.Visibility.Visible;
        //}



    }


}
