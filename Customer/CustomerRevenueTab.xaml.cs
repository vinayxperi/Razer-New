

using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using RazerBase;
using RazerInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Customer
{
    /// <summary>
    /// This class represents a 'CustomerRevenueTab' object.
    /// </summary>
    public partial class CustomerRevenueTab : ScreenBase, IPreBindable
    {
        public ComboBoxItemsProvider cmbCMStatusGridCombo { get; set; }
        private List<string> dataKeys = new List<string> { "document_id", "document_type" }; //Used for double click
        private List<string> dataKeys2 = new List<string> { "document_id", "seq_code" }; //used for changing CM Status
        public string SelectedProductCode;
        public string documentID;
        public int seqID;


        /// <summary>
        /// Create a new instance of a 'CustomerRevenueTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerRevenueTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            Init();
        }

        /// <summary>
        /// Loads new revdetail records when aging rev record changes
        /// </summary>
        private void Grid_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            if (GridAgingRevenue.xGrid.Records.Count > 0)
            {
                DataRecord r = GridAgingRevenue.xGrid.ActiveRecord as DataRecord;
                if(r== null)
                {
                    r = GridAgingRevenue.xGrid.Records.FirstOrDefault() as DataRecord;
                }

                if (r.Cells["document_id"].Value.ToString().Substring(0, 3) == "ADJ")
                {
                    //refactorChangeParms(r);
                    this.CurrentBusObj.changeParm("@document_id", r.Cells["document_id"].Value.ToString());
                    this.CurrentBusObj.changeParm("@invoice_number", " ");
                }
                else
                {
                    this.CurrentBusObj.changeParm("@document_id", r.Cells["document_id"].Value.ToString());
                    this.CurrentBusObj.changeParm("@invoice_number", r.Cells["document_id"].Value.ToString());
                    //refactorChangeParms(r);
                }
            }
            this.CurrentBusObj.LoadTable("revdetail");
            
        }

        private void refactorChangeParms(DataRecord r)
        {
            this.CurrentBusObj.changeParm("@document_id", r.Cells["document_id"].Value.ToString());
            this.CurrentBusObj.changeParm("@invoice_number", r.Cells["document_id"].Value.ToString());
        }
        public void revdetailClearGrid()
        {
            this.CurrentBusObj.ObjectData.Tables["revdetail"].Rows.Clear();
        }

        public void revdetailLoadTable()
        {
            ////this.CurrentBusObj.changeParm("@document_id", "");
            ////this.CurrentBusObj.changeParm("@invoice_number", "");
            ////GridRevenueAcctg.xGrid.FieldLayouts.Clear();
            //this.CurrentBusObj.LoadTable("revdetail");
            //if (this.CurrentBusObj.ObjectData != null)
            //{
            //    if (GridAgingRevenue.xGrid.Records.Count > 0)
            //    {
            //        //GridAgingRevenue.xGrid.ActiveRecord = GridAgingRevenue.xGrid.Records[0];
            //        //GridAgingRevenue.CntrlFocus();
            //        ////set focus on first editable field

            //        GridAgingRevenue.xGrid.Records[0].IsSelected = true;
            //        GridAgingRevenue.xGrid.Records[0].IsActive = true;
            //        //this.CurrentBusObj.LoadTable("revenue");
                    
                     
            //    }
            //}
           
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
            
            //setup parent grid
            Style s = (System.Windows.Style)Application.Current.Resources["CreditMemoStatusColorConverter"];
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            f.DataRecordPresenterStyle = s;
            GridAgingRevenue.xGrid.FieldLayoutSettings = f;
            GridRevenueAcctg.xGrid.FieldLayoutSettings = f;

            //setup attributes for Parent
            MainTableName = "revenue";
            
            GridAgingRevenue.MainTableName = "revenue";
            GridAgingRevenue.ConfigFileName = "CustomerRevenue";
     
            GridAgingRevenue.ContextMenuAddIsVisible = false;
            GridAgingRevenue.ContextMenuRemoveIsVisible = false;
         
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
           GridAgingRevenue.WindowZoomDelegate = GridDoubleClickDelegate;

            GridAgingRevenue.SetGridSelectionBehavior(true, false);
            GridAgingRevenue.FieldLayoutResourceString = "CustomerRevenueGrid";
            //GridAgingRevenue.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "product_code" }, ChildGrids = { GridRevenueAcctg }, ParentFilterOnColumnNames = { "product_code" } });

            GridCollection.Add(GridAgingRevenue);

            //setup attributes for Child
            GridRevenueAcctg.MainTableName = "revdetail";
            GridRevenueAcctg.ConfigFileName = "CustomerRevenueAcctg";
            GridRevenueAcctg.ContextMenuAddIsVisible = false;
            GridRevenueAcctg.ContextMenuRemoveIsVisible = false;


            GridRevenueAcctg.xGrid.FieldSettings.AllowEdit = true;
            GridRevenueAcctg.WindowZoomDelegate = GridDoubleClickDelegate;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridRevenueAcctg.SetGridSelectionBehavior(true, false);

            GridRevenueAcctg.FieldLayoutResourceString = "CustomerRevenueDetailGrid";
            GridCollection.Add(GridRevenueAcctg);
        }

        public void GridDoubleClickDelegate()
        {
            //call contracts folder
            GridAgingRevenue.ReturnSelectedData(dataKeys);
            //Determine the document type and then set appropriate double click destination
            switch (cGlobals.ReturnParms[1].ToString().ToLower())
            {
                case "adjust":
                    cGlobals.ReturnParms[1] = ("AdjustmentZoom");
                    break;
                case "invoice":
                    cGlobals.ReturnParms[1] = ("InvoiceZoom");
                    break;
                //CLB Added Grid to the Cash Zoom - the View Cash needs to know it is being passed in from another window because it has other parms
                //It is expecting the Grid in the first part of the Parm
             
                case "minvoice":
                    cGlobals.ReturnParms[1] = ("CustomInvoiceZoom");
                    break;
                case "ninvoice":
                    cGlobals.ReturnParms[1] = ("idgNationalAdsSearch");
                    break;

                default:
                    cGlobals.ReturnParms[1] = ("CustomerViewZoom");
                    break;

            }
            //cGlobals.ReturnParms.Add("GridAgingDetail.CustomerView");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            //args.Source = GridAgingDetail.xGrid;
            args.Source = GridRevenueAcctg.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);
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

    }
   
}
