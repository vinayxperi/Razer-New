

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
#endregion

namespace MiscFolders
{

    #region class CustomerDocumentDetailTab
    /// <summary>
    /// This class represents a 'CustomerDocumentDetailTab' object.
    /// </summary>
    public partial class CustomerDocumentDetailTab : ScreenBase
    {

        #region Private Variables
        string documentIDtoPass = "";
        string detailType = "";
        string adjustType = "ADJ";
        string cashType = "CSH";
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'CustomerDocumentDetailTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerDocumentDetailTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            Init();
        }
        #endregion

        #region Methods

        #region Init()
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
             // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "detail";
            //Set up Parent Child Relationship
            SetParentChildAttributes();

              
            
           
        }
        #endregion


        private void SetParentChildAttributes()
        {
            //Create the object
            CurrentBusObj = new cBaseBusObject("CustomerDocument");
            CurrentBusObj.Parms.ClearParms();
           

            //setup attributes for Parent
            //Establish the Parent Grid
            GridCustomerDocumentDetail.MainTableName = "detail";
            GridCustomerDocumentDetail.SetGridSelectionBehavior(false, true);
            GridCustomerDocumentDetail.ConfigFileName = "customerdocumentdetailconfig";
            GridCustomerDocumentDetail.FieldLayoutResourceString = "CustomerDocumentDetail";
            //add delegate to doubleclick and transfer control to customer document folder
            GridCustomerDocumentDetail.WindowZoomDelegate = GridDoubleClickDelegate; 
            GridCustomerDocumentDetail.WindowZoomDelegate = ReturnSelectedData;
            //GridCustomerDocumentDetail.IsFilterable = true;

            GridCustomerDocumentDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = {   "apply_to_seq" }, ChildGrids = { GridCustomerDocumentApplied }, ParentFilterOnColumnNames = {  "seq_code" } });

            //setup attributes for Child
            GridCustomerDocumentApplied.MainTableName = "detailadj";
            GridCustomerDocumentApplied.SetGridSelectionBehavior(true, false);
            GridCustomerDocumentApplied.ConfigFileName = "customerdocumentdetailappliedconfig";
            GridCustomerDocumentApplied.FieldLayoutResourceString = "CustomerDocumentDetailApplied";
            GridCustomerDocumentApplied.WindowZoomDelegate = GridDetailDoubleClickDelegate;
            GridCustomerDocumentApplied.SetGridSelectionBehavior(true, false);


            GridCollection.Add(GridCustomerDocumentDetail);
            GridCollection.Add(GridCustomerDocumentApplied);
        }

        public void GridDoubleClickDelegate()
        {
            //call customer document folder
            GridCustomerDocumentDetail.ReturnSelectedData("receivable_account");
            cGlobals.ReturnParms.Add("GridCustomerDocumentDetail.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = GridCustomerDocumentDetail.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        }

        public void GridDetailDoubleClickDelegate()
        {
            //determine what folder to call based on detail type
            cGlobals.ReturnParms.Clear();
            GridCustomerDocumentApplied.ReturnSelectedData("document_id");
            documentIDtoPass = cGlobals.ReturnParms[0].ToString();
            if (documentIDtoPass.Substring(0,3) == adjustType)
                cGlobals.ReturnParms.Add("GridCustomerDocumentApplied.xGrid");
            else
                if (documentIDtoPass.Substring(0, 3) == cashType)
                {

                    cGlobals.ReturnParms.Add("GridCustomerDocumentApplied2.xGrid");
                }
                else
                    cGlobals.ReturnParms.Add("GridCustomerDocumentApplied3.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            
            args.Source = GridCustomerDocumentDetail.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        }


        public void ReturnSelectedData()
        {
            //Zoom Functionality

        }

        #endregion

    }
    #endregion

}
