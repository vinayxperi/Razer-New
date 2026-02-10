

 

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using Infragistics.Windows.Controls;



namespace MiscFolders
{


    /// <summary>
    /// This class represents a 'ucTab1' object.
    /// </summary>
    public partial class WHTTrackingTab : ScreenBase, IPreBindable
    {


        private static readonly string whtStatusTableName = "whtstatus";
        //private static readonly string whtStatusParameterName = "@code_name";
        //private static readonly string whtStatusParameterValue = "WHTStatus";
        private static readonly string whtStatusDisplayPath = "code_value";
        private static readonly string whtStatusValuePath = "fkey_int";
        public ComboBoxItemsProvider cmbWHTStatus { get; set; }
        string StartDate;
        string EndDate;

       
        public WHTTrackingTab()
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
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "wht_tracking";
            //assign delegate for displaying inactive users
            //EmployeeMaintenance.ContextMenuAddDelegate = EmployeeMaintenanceGridShowInactive;
            gWHTTracking.ContextMenuGenericDelegate1 = gWHTTrackingShowAll;
            gWHTTracking.ContextMenuGenericDisplayName1 = "Show All Documents";
            gWHTTracking.ContextMenuGenericIsVisible1 = true;

            //assign delegate for hiding inactive users
            //EmployeeMaintenance.ContextMenuAddDelegate = EmployeeMaintenanceGridHideInactive;
            gWHTTracking.ContextMenuGenericDelegate2 = gWHTTrackingShowWithWHTAdj;
            gWHTTracking.ContextMenuGenericDisplayName2 = "Only Show Documents With WHT Adjustments";
            gWHTTracking.ContextMenuGenericIsVisible2 = true;

            //assign delegate for hiding inactive users
            //EmployeeMaintenance.ContextMenuAddDelegate = EmployeeMaintenanceGridHideInactive;
            gWHTTracking.ContextMenuGenericDelegate3 = gWHTTrackingShowWithoutCert;
            gWHTTracking.ContextMenuGenericDisplayName3 = "Only Show Documents Without Certificates";
            gWHTTracking.ContextMenuGenericIsVisible3 = true;
            //Set up Parent Child Relationship
            SetParentChildAttributes();

            //RES 8/28/14 filter out documents without WHT adjustments when window is opened
            var filter = new RecordFilter();
            filter.FieldName = "wht_adjustments";
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 0));
            gWHTTracking.xGrid.FieldLayouts[0].RecordFilters.Add(filter);

            //RES 2/13/20 filter out documents that have received certificates when window is opened
            var filter2 = new RecordFilter();
            filter2.FieldName = "certificate_recv_flag";
            filter2.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 1));
            gWHTTracking.xGrid.FieldLayouts[0].RecordFilters.Add(filter2);

            var filter3 = new RecordFilter();
            filter3.FieldName = "no_cert_flag";
            filter3.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 1));
            gWHTTracking.xGrid.FieldLayouts[0].RecordFilters.Add(filter3);
           
        }

      

        private void SetParentChildAttributes()
        {
            //Create the WHT Tracking object
            CurrentBusObj = new cBaseBusObject("WHTFolder");
            CurrentBusObj.Parms.ClearParms();
            //setup parent grid
            
            //Establish the WHT Tracking Grid
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            gWHTTracking.xGrid.FieldLayoutSettings = f;
            gWHTTrackingAdjbyDoc.xGrid.FieldLayoutSettings = f;
            gWHTTracking.MainTableName = "wht_tracking";
            gWHTTracking.ConfigFileName = "WhtTrackingGrid";
            gWHTTracking.FieldLayoutResourceString = "WhtTracking";
            //Set the grid to allow edits, for readonly columns set the allowedit to false in the field layouts file
            gWHTTracking.xGrid.FieldSettings.AllowEdit = true;
            //add delegate to doubleclick and transfer control to customer document folder
            gWHTTracking.WindowZoomDelegate = GridDoubleClickDelegate; 
           // gWHTTracking.IsFilterable = true;
            gWHTTracking.SetGridSelectionBehavior(false, false);
            gWHTTracking.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "apply_to_doc" }, ChildGrids = { gWHTTrackingAdjbyDoc }, ParentFilterOnColumnNames = { "document_id" } });
            //setup attributes for Child
            gWHTTrackingAdjbyDoc.MainTableName = "wht_tracking_adj";
            gWHTTrackingAdjbyDoc.ConfigFileName = "WhtTrackingAdjbyDocGrid";
            gWHTTrackingAdjbyDoc.FieldLayoutResourceString = "WhtTrackingAdjbyDoc";
            //gWHTTracking.xGrid.FieldSettings.AllowEdit = false;
           // gWHTTrackingAdjbyDoc.IsFilterable = true;
            gWHTTrackingAdjbyDoc.WindowZoomDelegate = AdjGridDoubleClickDelegate;
            gWHTTrackingAdjbyDoc.SetGridSelectionBehavior(false, false);


            GridCollection.Add(gWHTTracking);
            GridCollection.Add(gWHTTrackingAdjbyDoc);
        }

        //logic to transfer control to the Customer Document Folder
        public void GridDoubleClickDelegate()
        {
            //call customer document folder
            gWHTTracking.ReturnSelectedData("document_id");
            
            cGlobals.ReturnParms.Add("gWHTTracking.xGrid");
            Cell  activecell = gWHTTracking.xGrid.ActiveCell;
            if (activecell == null)
            {
            }
            else
            {
                Record  activeRecord = gWHTTracking.xGrid.Records[gWHTTracking.ActiveRecord.Index];
            
                if (activecell.Field.Name == "document_id")
                {

                    RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                    args.Source = gWHTTracking.xGrid;
                    EventAggregator.GeneratedClickHandler(this, args);
                }
                else
                    cGlobals.ReturnParms.Clear();

            }



        }

        public void AdjGridDoubleClickDelegate()
        {
            //call customer document folder
            gWHTTrackingAdjbyDoc.ReturnSelectedData("document_id");
            
                cGlobals.ReturnParms.Add("gWHTTrackingAdjbyDoc.xGrid");
                RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                args.Source = gWHTTrackingAdjbyDoc.xGrid;
                EventAggregator.GeneratedClickHandler(this, args);
          

        }

        private void gWHTTrackingShowAll()
        {
            //EmployeeMaintenance.ClearFilter();
            var filter = new RecordFilter();
            filter.FieldName = "wht_adjustments";
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 9));

            //Apply the filter to the grid
            gWHTTracking.xGrid.FieldLayouts[0].RecordFilters.Add(filter);

            //RES 8/28/14 do not filter out documents without WHT adjustments 
            var filter2 = new RecordFilter();
            filter2.FieldName = "certificate_recv_flag";
            filter2.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 2));
            gWHTTracking.xGrid.FieldLayouts[0].RecordFilters.Add(filter2);
            var filter3 = new RecordFilter();
            filter3.FieldName = "no_cert_flag";
            filter3.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 2));
            gWHTTracking.xGrid.FieldLayouts[0].RecordFilters.Add(filter3);

        }
        private void gWHTTrackingShowWithWHTAdj()
        {
            var filter = new RecordFilter();
            filter.FieldName = "wht_adjustments";
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 0));

            //Apply the filter to the grid
            gWHTTracking.xGrid.FieldLayouts[0].RecordFilters.Add(filter);
        }
        private void gWHTTrackingShowWithoutCert()
        {
            var filter = new RecordFilter();
            filter.FieldName = "certificate_recv_flag";
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 1));    
            gWHTTracking.xGrid.FieldLayouts[0].RecordFilters.Add(filter);

            var filter2 = new RecordFilter();
            filter2.FieldName = "no_cert_flag";
            filter2.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 1));
            gWHTTracking.xGrid.FieldLayouts[0].RecordFilters.Add(filter2);
        }

        public void PreBind()
        {
           
            if (this.CurrentBusObj.HasObjectData)
            {

                cmbWHTStatus = new ComboBoxItemsProvider();
                cmbWHTStatus.ItemsSource = this.CurrentBusObj.ObjectData.Tables[whtStatusTableName].DefaultView;
                cmbWHTStatus.ValuePath = whtStatusValuePath;
                cmbWHTStatus.DisplayMemberPath = whtStatusDisplayPath;
            }
        }

        public void ReturnSelectedData()
        {
            //Zoom Functionality

        }
        /// <summary>
        /// Retrieve Button pressed - retrieves data to populate grid based on search criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRetrieve_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //KSH - 8/29/12 keep button click from blowing up app when no rec selected
            if (this.CurrentBusObj == null)
                return;         

            //If no start input then select all previous
            if (txtInvoiceDateStart.SelText.ToString() == "" || txtInvoiceDateStart.SelText == null || txtInvoiceDateStart.SelText.ToString() == "1/1/1900 12:00:00 AM")
            {
                this.CurrentBusObj.changeParm("@date_start", "1/1/1900");
                StartDate = "1/1/1900";
            }
            else
            {
                this.CurrentBusObj.changeParm("@date_start", txtInvoiceDateStart.SelText.ToString());
                int BlankStart = txtInvoiceDateStart.SelText.ToString().IndexOf(" ");
                StartDate = txtInvoiceDateStart.SelText.ToString().Substring(0, BlankStart);
            }

            //if no end date then select all future dates
            if (txtInvoiceDateEnd.SelText.ToString() == "1/1/1900 12:00:00 AM" || txtInvoiceDateEnd.SelText == null)
            {
                this.CurrentBusObj.changeParm("@date_end", "12/31/2100");
                EndDate = "12/31/2100";
            }
            else
            {
                this.CurrentBusObj.changeParm("@date_end", txtInvoiceDateEnd.SelText.ToString());
                int BlankEnd = txtInvoiceDateEnd.SelText.ToString().IndexOf(" ");
                EndDate = txtInvoiceDateEnd.SelText.ToString().Substring(0, BlankEnd);
            }

            //this.CurrentBusObj.LoadTable("ops");
            this.CurrentBusObj.LoadData("wht_tracking");
            //this.findRootScreenBase(this).Refresh();
            //approvalBusinessObject.LoadData();
            //idgAdjustments.LoadGrid(approvalBusinessObject, mainTableName);
            if (CurrentBusObj.ObjectData.Tables["wht_tracking"].Rows.Count == 0)
            {
                Messages.ShowWarning("No WHT Tracking for dates Specified");
            }
            else
            {
                //gOps.LoadGrid(this.CurrentBusObj, MainTableName);
            }
        }

        public void ClearDates()
        {
            txtInvoiceDateStart.SelText = null;
            txtInvoiceDateEnd.SelText = null;
        }
    }
}
