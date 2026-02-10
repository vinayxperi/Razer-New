

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using System.Collections.Generic;

#endregion

namespace Customer
{

    #region class CustomerProformaTab
    /// <summary>
    /// This class represents a 'CustomerProformaTab' object.
    /// </summary>
    public partial class CustomerProformaTab : ScreenBase
    {
        private List<string> dataKeys2 = new List<string> { "invoice_number"}; //used for changing CM Status
        public string documentID;
        public int seqID;
        #region Constructor
        /// <summary>
        /// Create a new instance of a 'CustomerHistoryTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerProformaTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
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
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //The main table used on the tab item is the account_history table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "proforma";
            GridProforma.MainTableName = "proforma";
            GridProforma.ContextMenuAddIsVisible = false;
            GridProforma.ContextMenuRemoveIsVisible = false;

            /* 10/18/18 Add comments to proforma invoices */
            GridProforma.ContextMenuGenericDelegate1 = GridProformaCMCommentDelegate;
            GridProforma.ContextMenuGenericDisplayName1 = "Enter Credit Comment";
            GridProforma.ContextMenuGenericIsVisible1 = true;

            GridProforma.WindowZoomDelegate = GridDoubleClickDelegate;
            GridProforma.IsFilterable = false;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridProforma.SetGridSelectionBehavior(true, false);
            GridProforma.FieldLayoutResourceString = "CustomerProformaInvoices";

            //This particular style is used for color coded grids.
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            GridProforma.GridCellValuePresenterStyle = CellStyle;
            GridProforma.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            GridProforma.DoNotSelectFirstRecordOnLoad = true;

            GridProforma.SetGridSelectionBehavior(true, false);
            //GridProforma.LoadGrid(businessObject, GridProforma.MainTableName);


            GridCollection.Add(GridProforma);

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
        #endregion

        /// <summary>
        /// Delegate for grid 
        /// </summary>
        public void GridDoubleClickDelegate()
        {
            //call contracts folder
            GridProforma.ReturnSelectedData("invoice_number");
            cGlobals.ReturnParms.Add("GridProforma.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = GridProforma.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);            
        }
        /* 10/18/18 Add comments to proforma invoices */
        private void GridProformaCMCommentDelegate()
        {
            if (GridProforma.xGrid.ActiveRecord != null)
            {
                if (GridProforma.xGrid.ActiveRecord.Index == -1)
                {
                    MessageBox.Show("Please select a record in the grid.");
                    return;
                }

                GridProforma.ReturnSelectedData(dataKeys2);
                documentID = cGlobals.ReturnParms[0].ToString();

                //seqID = Convert.ToInt32(cGlobals.ReturnParms[1]);
                seqID = 1;
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
            
        #endregion

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
    #endregion

}
