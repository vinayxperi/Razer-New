

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace Customer
{
    /// <summary>
    /// This class represents a 'CustomerAgingTab' object.
    /// </summary>
    public partial class CustomerAgingTab : ScreenBase, IPreBindable
    {
        public ComboBoxItemsProvider cmbCMStatusGridCombo { get; set; }
        private List<string> dataKeys = new List<string> {"document_id","document_type"}; //Used for double click
        private List<string> dataKeys2 = new List<string> { "document_id", "seq_code" }; //used for changing CM Status
        public string SelectedProductCode;
        public string documentID;
        public int seqID;


        /// <summary>
        /// Create a new instance of a 'CustomerAgingTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerAgingTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            Init();
        }

        /// <summary>
        /// This event [enter description here].
        /// </summary>
        private void Grid_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            if (GridAgingTotal.xGrid.SelectedItems.Records.Count > 0)
            {
                DataRecord r = default(DataRecord);
                r = (Infragistics.Windows.DataPresenter.DataRecord)(GridAgingTotal.xGrid.SelectedItems.Records[0]);
                GridAgingDetail.FilterGrid("company_code", r.Cells["company_code"].Value.ToString());

                if (r.Cells["product_code"].Value.ToString().ToUpper() != "ALL")
                {
                    GridAgingDetail.FilterGrid("product_code", r.Cells["product_code"].Value.ToString());
                }
                else
                {
                    //GridAgingDetail.FilterGrid("product_code", "");
                    //GridAgingDetail.FilterGrid("company_code", r.Cells["company_code"].Value.ToString());
                    GridAgingDetail.ClearFilter();
                }
                SelectedProductCode = r.Cells["product_code"].Value.ToString().ToUpper();
            }

        }

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "Aging";
            SetParentChildAttributes();
            this.HasPrintReport = true;
        }

        private void SetParentChildAttributes()
        {
            //Create the receivable account object
            CurrentBusObj = new cBaseBusObject("Customer");
            CurrentBusObj.Parms.ClearParms();
            //CurrentBusObj.Parms.AddParm("@receivable_account", "8041022");
            //CurrentBusObj.Parms.AddParm("@product_code", "a");
            //setup parent grid
            Style s = (System.Windows.Style)Application.Current.Resources["CreditMemoStatusColorConverter"];
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            f.DataRecordPresenterStyle = s;
            GridAgingTotal.xGrid.FieldLayoutSettings = f;
            GridAgingDetail.xGrid.FieldLayoutSettings = f;

            //setup attributes for Parent
            MainTableName = "Aging";
            //GridAgingTotal.WindowZoomDelegate = ReturnSelectedData;
            GridAgingTotal.MainTableName = "Aging";
            GridAgingTotal.IsFilterable = true;
            GridAgingTotal.ConfigFileName = "CustomerAgingTotal";
            GridAgingTotal.ContextMenuAddIsVisible = false;
            GridAgingTotal.ContextMenuRemoveIsVisible = false;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridAgingTotal.SetGridSelectionBehavior(true, false);
            GridAgingTotal.FieldLayoutResourceString = "CustomerAgingTotalGrid";
            GridAgingTotal.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "product_code" }, ChildGrids = { GridAgingDetail }, ParentFilterOnColumnNames = { "product_code" } });

            GridCollection.Add(GridAgingTotal);

            //setup attributes for Child
            GridAgingDetail.MainTableName = "aging_detail";
            GridAgingDetail.ConfigFileName="CustomerAgingDetail";
            GridAgingDetail.ContextMenuAddIsVisible = false;
            GridAgingDetail.ContextMenuRemoveIsVisible = false;

            GridAgingDetail.ContextMenuAddDelegate = GridAgingDetailCMStatusDelegate;
            GridAgingDetail.ContextMenuAddDisplayName = "Change CM Status";
            GridAgingDetail.ContextMenuAddIsVisible = true;
            GridAgingDetail.ContextMenuGenericDelegate1 = GridAgingDetailCMCommentDelegate;
            GridAgingDetail.ContextMenuGenericDisplayName1 = "Enter Credit Comment";
            GridAgingDetail.ContextMenuGenericIsVisible1 = true;
            GridAgingDetail.ContextMenuRemoveDelegate = GridAgingDetailDisputeDelegate;
            GridAgingDetail.ContextMenuRemoveDisplayName = "Enter Dispute Amount";
            GridAgingDetail.ContextMenuRemoveIsVisible = false;
            GridAgingDetail.xGrid.FieldSettings.AllowEdit = true;
            GridAgingDetail.IsFilterable = true;
            GridAgingDetail.WindowZoomDelegate = GridDoubleClickDelegate;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridAgingDetail.SetGridSelectionBehavior(true, false);
            GridAgingDetail.IsFilterable = true;
            GridAgingDetail.FieldLayoutResourceString = "CustomerAgingDetailGrid";
        }

        /// <summary>
        /// Delegate for grid 
        /// </summary>
        public void GridDoubleClickDelegate()
        {
            //call contracts folder
            GridAgingDetail.ReturnSelectedData(dataKeys) ;
            //Determine the document type and then set appropriate double click destination
            switch (cGlobals.ReturnParms[1].ToString().ToLower())
            {
                case "adjust":
                    cGlobals.ReturnParms[1]=("AdjustmentZoom");
                    break;
                case "invoice":
                    cGlobals.ReturnParms[1] = ("InvoiceZoom");
                    break;
                    //CLB Added Grid to the Cash Zoom - the View Cash needs to know it is being passed in from another window because it has other parms
                    //It is expecting the Grid in the first part of the Parm
                case "cash":
                    cGlobals.ReturnParms[1]=("GridCashZoom");
                    break;
                case "minvoice":
                    cGlobals.ReturnParms[1]=("CustomInvoiceZoom");
                    break;
                case "ninvoice":
                    cGlobals.ReturnParms[1] = ("idgNationalAdsSearch");
                    break;

                default:
                    cGlobals.ReturnParms[1]=("CustomerViewZoom");
                    break;

            }
            //cGlobals.ReturnParms.Add("GridAgingDetail.CustomerView");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            //args.Source = GridAgingDetail.xGrid;
            args.Source = GridAgingDetail.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);
        }
        private void GridAgingDetailCMStatusDelegate()
        {
            if (GridAgingDetail.xGrid.ActiveRecord != null)
            {
                if (GridAgingDetail.xGrid.ActiveRecord.Index == -1)
                {
                    MessageBox.Show("Please select a record in the grid.");
                    return;
                }
                GridAgingDetail.ReturnSelectedData(dataKeys2);
                documentID = cGlobals.ReturnParms[0].ToString();

                seqID = Convert.ToInt32(cGlobals.ReturnParms[1]);
                //instance location service screen
                CustomerChangeCreditMemo ChangeCreditMemoScreen = new CustomerChangeCreditMemo(documentID, seqID, this.CurrentBusObj);
                //////////////////////////////////////////////////////////////
                //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
                System.Windows.Window ChangeCreditMemoWindow = new System.Windows.Window();
                //set new window properties///////////////////////////


                ChangeCreditMemoWindow.Title = "Change CM Status";
                ChangeCreditMemoWindow.MaxHeight = 1280;
                ChangeCreditMemoWindow.MaxWidth = 1280;
                /////////////////////////////////////////////////////
                //set screen as content of new window
                ChangeCreditMemoWindow.Content = ChangeCreditMemoScreen;
                //open new window with embedded user control
                ChangeCreditMemoWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a record in the grid.");
                return;
            }
        }
        private void GridAgingDetailCMCommentDelegate()
        {
            if (GridAgingDetail.xGrid.ActiveRecord != null)
            {
                if ( GridAgingDetail.xGrid.ActiveRecord.Index  == -1)
                {
                    MessageBox.Show("Please select a record in the grid.");
                    return;
                }

                GridAgingDetail.ReturnSelectedData(dataKeys2);
                documentID = cGlobals.ReturnParms[0].ToString();

                seqID = Convert.ToInt32(cGlobals.ReturnParms[1]);
                //instance comment screen
                CustomerCreditCMComment CustomerCreditCMCommentScreen = new CustomerCreditCMComment(documentID, seqID, this.CurrentBusObj);
                //////////////////////////////////////////////////////////////
                //create a new window and embed  Screen usercontrol, show it as a dialog
                System.Windows.Window CreditCMWindow = new System.Windows.Window();
                //set new window properties///////////////////////////


                CreditCMWindow.Title = "Credit Comment";
                CreditCMWindow.MaxHeight = 1280;
                CreditCMWindow.MaxWidth = 1280;
                /////////////////////////////////////////////////////
                //set screen as content of new window
                CreditCMWindow.Content = CustomerCreditCMCommentScreen;
                //open new window with embedded user control
                CreditCMWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a record in the grid.");
                return;
            }
        }

        private void GridAgingDetailDisputeDelegate()
        {
            if (GridAgingDetail.xGrid.ActiveRecord != null)
            {
                if (GridAgingDetail.xGrid.ActiveRecord.Index == -1)
                {
                    MessageBox.Show("Please select a record in the grid.");
                    return;
                }
                GridAgingDetail.ReturnSelectedData(dataKeys2);
                documentID = cGlobals.ReturnParms[0].ToString();

                seqID = Convert.ToInt32(cGlobals.ReturnParms[1]);
                //instance location service screen
                CustomerDispute CustomerDisputeScreen = new CustomerDispute(documentID, seqID, this.CurrentBusObj);
                //////////////////////////////////////////////////////////////
                //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
                System.Windows.Window CustomerDisputeWindow = new System.Windows.Window();
                //set new window properties///////////////////////////


                CustomerDisputeWindow.Title = "Enter Dispute Amount";
                CustomerDisputeWindow.MaxHeight = 1280;
                CustomerDisputeWindow.MaxWidth = 1280;
                /////////////////////////////////////////////////////
                //set screen as content of new window
                CustomerDisputeWindow.Content = CustomerDisputeScreen;
                //open new window with embedded user control
                CustomerDisputeWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a record in the grid.");
                return;
            }
        }

        public void PreBind()
        {
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                ComboBoxItemsProvider provider = new ComboBoxItemsProvider();
                //Set the items source to be the databale of the DDDW
                provider.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cm_status_lookup"].DefaultView;

                //set the value and display path
                provider.ValuePath = "cm_status";
                provider.DisplayMemberPath = "cm_desc";
                //Set the property that the grid combo will bind to
                //This value is in the binding in the layout resources file for the grid.
                cmbCMStatusGridCombo = provider;
            }
        }

        public override void PrintReport()
        {
            string validAcct;
            if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["general"] != null && CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0 && GridAgingTotal.ActiveRecord!=null)
            {
                validAcct = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["receivable_account"].ToString();
                if (validAcct == "")
                {
                    MessageBox.Show("Customer Number required to run Report.");
                    return;
                }
                else
                {

                    this.PrintReportJobName = "Customer Aging Tab Print";
                    PrintReportParms.Add(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["receivable_account"].ToString());
                    PrintReportParms.Add(GridAgingTotal.ActiveRecord.Cells["product_code"].Value.ToString());

                    base.PrintReport();
                }
            }
            else
            {
                MessageBox.Show("Customer Number required to run Report.");
                return;
            }

            //jobparms = jobparms + " /A " + SelectedProductCode;
            //jobparms = jobparms + " /A " + settings.PrinterName;

            //if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobname, jobparms, dt, cGlobals.UserName.ToString()) == true)
            //{
            //    Messages.ShowWarning("Screen Report Sent to Printer");
            //}
            //else
            //    Messages.ShowWarning("Screen Print Error");


        }
    }
   
}
