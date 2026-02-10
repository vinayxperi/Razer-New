

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;

#endregion

namespace Razer.BatchBilling
{

    #region class BillingBatchDetailTab
    /// <summary>
    /// This class represents a 'BillingBatchDetailTab' object.
    /// </summary>
    public partial class BillingBatchDetailTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'BillingBatchDetailTab' object and call the ScreenBase's constructor.
        /// </summary>
        public BillingBatchDetailTab()
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
            {
                // Set the ScreenBaseType
                this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
                // Change this setting to the name of the DataTable that will be used for Binding.
                MainTableName = "BillingBatchFolder";

                //Establist the COmpany Contract Grid
                gBillingBatchDetail.xGrid.FieldSettings.AllowEdit = false;
                gBillingBatchDetail.MainTableName = "detail";
                gBillingBatchDetail.SetGridSelectionBehavior(false, true);
                gBillingBatchDetail.FieldLayoutResourceString = "BillingBatchDetail";
                gBillingBatchDetail.WindowZoomDelegate = GridDoubleClickDelegate;
                gBillingBatchDetail.ConfigFileName = "billingbatchdetail";
               
                gBillingBatchRuleDetail.MainTableName = "ruledetail";
                gBillingBatchRuleDetail.SetGridSelectionBehavior(true, false);
                gBillingBatchRuleDetail.FieldLayoutResourceString = "BillingBatchRuleDetail";
                gBillingBatchRuleDetail.ConfigFileName = "BillingBatchRuleDetail";
                gBillingBatchDetail.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "unposted_id" }, ChildGrids = { gBillingBatchRuleDetail }, ParentFilterOnColumnNames = { "unposted_id" } });
                //Moved to separate screen
                //gBillingBatchAcctDetail.MainTableName = "acctdetail";
                //gBillingBatchAcctDetail.SetGridSelectionBehavior(true, false);
                //gBillingBatchAcctDetail.FieldLayoutResourceString = "BillingBatchAcctDetail";
                //gBillingBatchDetail.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "unposted_id" }, ChildGrids = { gBillingBatchAcctDetail }, ParentFilterOnColumnNames = { "unposted_id" } });

                //gBillingBatchRuleGroup.MainTableName = "rulegroup";
                //gBillingBatchRuleGroup.SetGridSelectionBehavior(true, false);
                //gBillingBatchRuleGroup.FieldLayoutResourceString = "BillingBatchRuleGroup";
                //gBillingBatchRuleDetail.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "unposted_id", "rule_group_id" }, ChildGrids = { gBillingBatchRuleGroup }, ParentFilterOnColumnNames = { "unposted_id", "apply_to_group" } });

                GridCollection.Add(gBillingBatchDetail);
                GridCollection.Add(gBillingBatchRuleDetail);
                //Moved to separate screen
                //GridCollection.Add(gBillingBatchAcctDetail);
                //GridCollection.Add(gBillingBatchRuleGroup);
            }
        }
        #endregion

        #endregion
          public void GridDoubleClickDelegate()
        {
            //call invoiceactruletier detail screen
            //if double click on contract_id, go to contract folder, else call invoiceacctruletier detail screen

            
            Cell activecell = gBillingBatchDetail.xGrid.ActiveCell;
            if (activecell == null)
            {
            }
            else
               
            {
                Record activeRecord = gBillingBatchDetail.xGrid.Records[gBillingBatchDetail.ActiveRecord.Index];

                if (activecell.Field.Name == "contract_id")
                {
                    //call contract folder
                    gBillingBatchDetail.ReturnSelectedData("contract_id");
                    cGlobals.ReturnParms.Add("GridContracts.xGrid");
                    RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                    args.Source = gBillingBatchDetail.xGrid;
                    EventAggregator.GeneratedClickHandler(this, args);


                 
                }
                else
                {
                    cGlobals.ReturnParms.Clear();
                    this.GetUnpostedLineInfoToPass();
                }

            }
       
            
            


        } 
        public void GetUnpostedLineInfoToPass()
        {
            
            cBaseBusObject BillingBatchAcctRuleObj = new cBaseBusObject();
            //add invoice number and inv_line_id to bus obj parm table
            
             
            BillingBatchAcctRuleObj.BusObjectName = "BillingBatchAcctRule";
            gBillingBatchDetail.ReturnSelectedData("batch_id");
            if (cGlobals.ReturnParms.Count > 0)
            {
                //get unposted id
                string batch_id_to_pass = cGlobals.ReturnParms[0].ToString();
                BillingBatchAcctRuleObj.Parms.AddParm("@batch_id", batch_id_to_pass);
            }
            else
            {
                Messages.ShowInformation("No Detail Line Selected");
                 
            }
           
                 
         

            if (BillingBatchAcctRuleObj != null)
            {
                //show the billing acct rules screen
                callBillingBatchAcctRuleScreen(BillingBatchAcctRuleObj);
            }
            else
            {
                Messages.ShowInformation("Problem Opening Billing Batch Account Rule Detail Screen");
            }

        }

        #endregion
        private void callBillingBatchAcctRuleScreen(cBaseBusObject BillingBatchAcctRuleObj)
        {

            //tell the rules screen it is inserting if adding new record
            BillingBatchAcctRuleDetail unpostedDetailScreen = new BillingBatchAcctRuleDetail(BillingBatchAcctRuleObj);
            System.Windows.Window unpostedScreenWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            unpostedScreenWindow.Title = "Billing Batch Acct Rule Detail ";
            unpostedScreenWindow.MaxHeight = 1650;
            unpostedScreenWindow.MaxWidth = 1650;
            /////////////////////////////////////////////////////
            //set rules screen as content of new window
            unpostedScreenWindow.Content = unpostedDetailScreen;
            //open new window with embedded user control
            unpostedScreenWindow.ShowDialog();


            
        }
     

    


    }

    }
    

 
