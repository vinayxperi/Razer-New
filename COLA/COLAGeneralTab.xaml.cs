#region using statements
using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using RazerBase.Interfaces;
using RazerBase.Lookups;

#endregion

namespace COLA
{
    public partial class COLAGeneralTab : ScreenBase, IPreBindable 
    {
        private static readonly string TableNameConstant = "general";
        private static readonly string GridNameConstant = "GridCOLA";
        private static readonly string busObjNameConstant = "ColaCandidates";

        #region Constructor Stuff
        public COLAGeneralTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();
            //runs when screen loads
            Init();
        }

        /// <summary>
        /// constructuor
        /// </summary>
        public void Init()
        {
            setScreenConstants();
            setGridBehavior();
            loadBusObjAndGrid();
            cmbProductCode.Focus();
            //GridCola.mWindowZoomDelegate = GridDoubleClickDelegate;
            //GridCollection.Add(GridCola);
        }

        /// <summary>
        /// This method jumps to the contract folder on doubleclick
        /// </summary>
        public void GridDoubleClickDelegate()
        {
            if (GridCola.xGrid.ActiveRecord != null)
            {
                //call customer document folder
                GridCola.ReturnSelectedData("contract_id");
                cGlobals.ReturnParms.Add("GridLocationContracts.xGrid");
                RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                args.Source = GridCola.xGrid;
                EventAggregator.GeneratedClickHandler(this, args);
            }
        }
        /// <summary>
        /// set up screen constants
        /// </summary>
        private void setScreenConstants()
        {
            CurrentState = ScreenState.Empty;
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            this.MainTableName = TableNameConstant;
        }

        /// <summary>
        /// configures grid
        /// </summary>
        private void setGridBehavior()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;            
            GridCola.xGrid.FieldSettings.AllowEdit = true;
            GridCola.MainTableName = TableNameConstant;
            GridCola.ConfigFileName = GridNameConstant;
            GridCola.FieldLayoutResourceString = GridNameConstant;
            GridCola.SetGridSelectionBehavior(false, false);
            GridCola.ContextMenuAddIsVisible = false;
            GridCola.ContextMenuRemoveIsVisible = false;
            GridCola.ContextMenuGenericDelegate1 =  SubmitCandidateListDelegate;
            GridCola.ContextMenuGenericDisplayName1 = "Create COLA Candidate List";
            GridCola.ContextMenuGenericIsVisible1 = true;
            GridCola.ContextMenuGenericDelegate2 =  SubmitApplyCOLADelegate;
            GridCola.ContextMenuGenericDisplayName2 = "Apply COLA";
            GridCola.ContextMenuGenericIsVisible2 = true;
            GridCola.ContextMenuGenericDelegate3 = RefreshDelegate;
            GridCola.ContextMenuGenericDisplayName3 = "Refresh";
            GridCola.ContextMenuGenericIsVisible3 = true;
            GridCola.ContextMenuGenericDelegate4 = CheckBillingFlag;
            GridCola.ContextMenuGenericDisplayName4 = "Check/Uncheck Billing Hold Flag All";
            GridCola.ContextMenuGenericIsVisible4 = true;
            GridCola.SkipReadOnlyCellsOnTab = true;
            GridCola.CellUpdatedDelegate = GridCola_CellUpdated;
            //GridCola.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(GridCola_EditModeEnded);
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                GridCola.ContextMenuGenericIsVisible1 = false;
                //GridCola.ContextMenuAddIsVisible = false;
                GridCola.ContextMenuGenericIsVisible2 = false;
            }
            else
            {
                GridCola.ContextMenuGenericIsVisible1 = true;
                //GridCola.ContextMenuAddIsVisible = true;
                GridCola.ContextMenuGenericIsVisible2 = true;
            }

            //Add this code to use a different style for the grid than the default.
            //This particular style is used for color coded grids.
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            GridCola.GridCellValuePresenterStyle = CellStyle;
            GridCola.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;

            GridCola.mWindowZoomDelegate = GridDoubleClickDelegate;
        }

        /// <summary>
        /// loads bus obj and data grid
        /// </summary>
        private void loadBusObjAndGrid()
        {
            //GridCola.LoadGrid(ColaBusObj, TableNameConstant);
            GridCollection.Add(GridCola);
        }
        #endregion

        #region Remove Hold Button Logic


        private void SubmitCandidateListDelegate()
        {
            //GridCola.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
            //foreach (DataRecord r in GridCola.xGrid.Records)
            //{
            //    //if (r.Cells["hold_flag"].Value.ToString() == "0")
            //        GridCola.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //}
            //foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["general"].Rows)
            //{
            //    dr.AcceptChanges();
            //}
            //this.Save();
            string nameofJob = "";
            nameofJob = "List";
            ScheduleCOLAjobs ScheduleCOLAjobsScreen = new ScheduleCOLAjobs(this.CurrentBusObj, nameofJob);
            //////////////////////////////////////////////////////////////
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window ScheduleCOLAjobsWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            ScheduleCOLAjobsWindow.Title = "Schedule COLA jobs";
            ScheduleCOLAjobsWindow.MaxHeight = 1280;
            ScheduleCOLAjobsWindow.MaxWidth = 1280;
            /////////////////////////////////////////////////////
            //set screen as content of new window
            ScheduleCOLAjobsWindow.Content = ScheduleCOLAjobsScreen;
            //open new window with embedded user control
            ScheduleCOLAjobsWindow.ShowDialog();
            this.findRootScreenBase(this).Refresh();
        }

       
        private void SubmitApplyCOLADelegate()
        {
            //instance location service screen
            //GridCola.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
            //foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["general"].Rows)
            //{
            //    dr.AcceptChanges();
            //}
            //foreach (DataRecord r in GridCola.xGrid.Records)
            //{
            //    GridCola.xGrid.ExecuteCommand(DataPresenterCommands.comm    .CommitChangesToActiveRecord);
            //}
            string nameofJob = "";
            nameofJob = "Apply";
            ScheduleCOLAjobs ScheduleCOLAjobsScreen = new ScheduleCOLAjobs(this.CurrentBusObj, nameofJob);
            //////////////////////////////////////////////////////////////
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window ScheduleCOLAjobsWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            ScheduleCOLAjobsWindow.Title = "Schedule COLA jobs";
            ScheduleCOLAjobsWindow.MaxHeight = 1280;
            ScheduleCOLAjobsWindow.MaxWidth = 1280;
            /////////////////////////////////////////////////////
            //set screen as content of new window
            ScheduleCOLAjobsWindow.Content = ScheduleCOLAjobsScreen;
            //open new window with embedded user control
            ScheduleCOLAjobsWindow.ShowDialog();
            this.findRootScreenBase(this).Refresh();
        }

        private void RefreshDelegate()
        {
            
            this.findRootScreenBase(this).Refresh();
        }
        /// <summary>
        /// RES Phase 3.1 Check/Uncheck billing hold flag on all cola candidates that bill in advance
        private void CheckBillingFlag()
        {
            if (this.CurrentBusObj == null)
                return;
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count < 1)
                return;
            int intFlagCnt = 0;
            int checkflag = -1;
            //loop through grid and remove hold flag where values are equal to product and mso Id 

            foreach (DataRecord r in GridCola.xGrid.Records)
            {
                if (r.Cells["bill_in_advance_flag"].Value.ToString() == "1")
                {
                    if (checkflag == -1)
                    {
                        if (r.Cells["billing_hold_flag"].Value.ToString() == "0")
                            checkflag = 1;
                        else
                            checkflag = 0;
                    }
                    if (checkflag == 1) r.Cells["billing_hold_flag"].Value = 1;
                    if (checkflag == 0) r.Cells["billing_hold_flag"].Value = 0;
                    GridCola.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    intFlagCnt++;
                }
            }
            
            if (checkflag == 1)
                Messages.ShowWarning(intFlagCnt + " Contracts placed on Billing Hold");
            if (checkflag == 0)
                Messages.ShowWarning(intFlagCnt + " Contracts taken off of Billing Hold");
        }
        /// <summary>
        /// Allows users to remove holds on multiple records
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //validate fields////////////////////////////////////////////////////////////////////////////////////////
            if (this.CurrentBusObj == null)
                return;
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count < 1)
                return;
            //if (cmbProductCode.SelectedValue == null || txtEntityId.Text == null || cmbProductCode.SelectedValue.ToString().Trim() == "" || txtEntityId.Text == "")
            //{
            //    showMsg();
            //    return;
            //}
            if (cmbProductCode.SelectedValue == null || cmbProductCode.SelectedValue.ToString().Trim() == "")
            {
                showMsg();
                return;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            int intFlagCnt = 0;
            foreach (DataRecord r in GridCola.xGrid.Records)
            {
                //if (r.Cells["product_code"].Value.ToString().ToLower().Trim() == cmbProductCode.SelectedValue.ToString().ToLower().Trim() && r.Cells["bill_mso_id"].Value.ToString().Trim() == txtEntityId.Text.Trim())
                if (r.Cells["product_code"].Value.ToString().ToLower().Trim() == cmbProductCode.SelectedValue.ToString().ToLower().Trim() )
                {
                    r.Cells["hold_flag"].Value = 0;
                    GridCola.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    intFlagCnt++;
                }
            }
            //Messages.ShowWarning(intFlagCnt + " Holds Removed From Product: " + cmbProductCode.SelectedValue.ToString().Trim() + " Entity ID: " + txtEntityId.Text.Trim());
            Messages.ShowWarning(intFlagCnt + " Holds Removed From Product: " + cmbProductCode.SelectedValue.ToString().Trim() + ". You will need to do a Save for the changes to take effect.");
        }

        /// <summary>
        /// shows message when incomplete data exists
        /// </summary>
        private void showMsg()
        {
            //Messages.ShowWarning("Both Product and Entity ID Must be Entered to Remove Hold");
            Messages.ShowWarning("Product Must be Entered to Remove Hold");
        }
        #endregion

        /// <summary>
        /// Call the Entity Lookup Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEntityId_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on Entity ID field
            EntityLookup f = new EntityLookup();

            this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                txtEntityId.Text = cGlobals.ReturnParms[0].ToString();
                txtEntityName.Text = cGlobals.ReturnParms[1].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        private void txtEntityId_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtEntityId.Text == null)
            {
                txtEntityName.Text = "";
                return;
            }
            if (txtEntityId.Text == "")
            {
                txtEntityName.Text = "";
                return;
            }
            //get entity name
            cBaseBusObject ContractVerification = new cBaseBusObject("ContractVerification");

            ContractVerification.Parms.ClearParms();
            ContractVerification.Parms.AddParm("@mso_id", txtEntityId.Text);
            ContractVerification.LoadTable("entity_name");
            if (ContractVerification.ObjectData.Tables["entity_name"] == null || ContractVerification.ObjectData.Tables["entity_name"].Rows.Count < 1)
            {
                Messages.ShowInformation("Invalid entity ID.  Please select a valid entity ID.");
                txtEntityId.Text = "0";
                txtEntityName.Text = "";
            }
            else
            {
                txtEntityName.Text = ContractVerification.ObjectData.Tables["entity_name"].Rows[0]["name"].ToString();
            }
        }

        /// <summary>
        /// get pre-bound junk
        /// </summary>
        public void PreBind()
        {
            //check this to keep recursion to this event from occurruing when load is called
            //if (IsLoading == true) return;
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                this.cmbProductCode.SetBindingExpression("product_code", "product_description", this.CurrentBusObj.ObjectData.Tables["products"]);
            }
        }

        private void GridCola_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        {
            if (e.Cell.Field.Name == "billing_hold_flag")
            {
                DataRecord GridRecord = null;
                GridCola.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridRecord = GridCola.ActiveRecord;
                if (GridRecord != null)
                {
                    foreach (DataRecord r in GridCola.xGrid.Records)
                    {
                        //RES 3/15/22 take out code that unchecks billing hold and put in code to prevent billing hold gettin checked 
                        // on contracts that are not billed in advance
                        //    if (r.Cells["bill_in_advance_flag"].Value.ToString() != "1" && r.Cells["billing_hold_flag"].Value.ToString() == "1")
                        //    {
                        //        r.Cells["billing_hold_flag"].Value = 0;
                        //        GridCola.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                        //    }

                        if (r.Cells["bill_in_advance_flag"].Value.ToString() != "1" && r.Cells["billing_hold_flag"].Value.ToString() == "1")
                        {
                            Messages.ShowInformation("Cannot put contract on billing hold that is not billed in the future.  Billing hold removed.");
                            r.Cells["billing_hold_flag"].Value = 0;
                            GridCola.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                        }
                    }
                }
            }
        }
    }
}
