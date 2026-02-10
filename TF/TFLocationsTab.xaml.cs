using RazerBase;
using RazerInterface;
using RazerBase.Lookups;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;

namespace TF
{
    /// <summary>
    /// Interaction logic for TFLocationsTab.xaml
    /// </summary>
    public partial class TFLocationsTab : ScreenBase, IPreBindable
    {
        public string wfStatus;
        public int TFStatus;
        //public ComboBoxItemsProvider cmbServiceStatusPNI { get; set; }
        //public ComboBoxItemsProvider cmbHeadendTypePNI { get; set; }
        bool bLocationUpdate = false;
        bool bLocationPNIUpdate = false;
        bool bUncheckLine = false;

        public TFLocationsTab()
        {
            InitializeComponent();
            // Perform initializations for this object
            Init();
            //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            //{
            //    txtOldOwnerMSOID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_id"].ToString();
            //    txtOldOwnerMSOName.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_name"].ToString();
            //}
        }

        public void PreBind()
        {
            //Service Status dddw in pni grid
            //ComboBoxItemsProvider ip3 = new ComboBoxItemsProvider();
            //ip3 = new ComboBoxItemsProvider();
            //ip3.ItemsSource = CurrentBusObj.ObjectData.Tables["dddwService"].DefaultView;
            //ip3.ValuePath = "service_status";
            //ip3.DisplayMemberPath = ("status_desc");
            //cmbServiceStatusPNI = ip3;
            //Headend dddw in PNI grid
            //ComboBoxItemsProvider ip4 = new ComboBoxItemsProvider();
            //ip4 = new ComboBoxItemsProvider();
            //ip4.ItemsSource = CurrentBusObj.ObjectData.Tables["dddwHeadend"].DefaultView;
            //ip4.ValuePath = "head_end_type";
            //ip4.DisplayMemberPath = ("head_end_type_desc");
            //cmbHeadendTypePNI = ip4;
        }

        public void Init()
        {
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty locations ");
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "TFFolder";

            //Set up Parent Child Relationship
            //Create the Customer Document object
            CurrentBusObj = new cBaseBusObject("TFFolder");
            CurrentBusObj.Parms.ClearParms();
            //Contract Location Grid
            
            //gTFLocations.ConfigFileName = "TFLocationsGrid";
            //gTFLocationsPNI.ConfigFileName = "TFLocationsPNIGrid";

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

  
            gTFLocations.MainTableName = "locations";
            gTFLocations.SetGridSelectionBehavior(false, false);
            gTFLocations.FieldLayoutResourceString = "TFLocations";
            //gTFLocations.GridGotFocusDelegate = gAllocGrid_GotFocus; //This ties the got focus event of the remit base grid to this method.

            gTFLocations.MainTableName = "locations";
            gTFLocations.FieldLayoutResourceString = "TFLocations";
            gTFLocations.xGrid.FieldSettings.AllowEdit = false;
            gTFLocations.SetGridSelectionBehavior(true, false);
            gTFLocations.SkipReadOnlyCellsOnTab = true;
            //gTFLocations.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            gTFLocations.CellUpdatedDelegate = gTFLocations_CellUpdated;
            //gTFLocations.RecordActivatedDelegate = gTFLocations_RecordActivated;
            gTFLocations.ContextMenuGenericDelegate1 = TFShowInactiveDelegate;
            gTFLocations.ContextMenuGenericDisplayName1 = "Show Inactive";
            gTFLocations.ContextMenuGenericIsVisible1 = true;
            gTFLocations.ContextMenuGenericDelegate2 = TFShowActiveDelegate;
            gTFLocations.ContextMenuGenericDisplayName2 = "Show Active";
            gTFLocations.ContextMenuGenericIsVisible2 = true;
            //gTFLocations.RecordActivatedDelegate = RowBeingClicked;
            //gTFLocations.EditModeEndedDelegate = gTFLocations_EditModeEnded;
            gTFLocations.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(gTFLocations_EditModeEnded);
            gTFLocations.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "cs_id" }, ChildGrids = { gTFLocationsPNI }, ParentFilterOnColumnNames = { "cs_id" } });
            gTFLocationsPNI.FieldLayoutResourceString = "TFLocationsPNI";
            gTFLocationsPNI.MainTableName = "locations_pni";
            gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = false;
            gTFLocationsPNI.SetGridSelectionBehavior(true, false);
            gTFLocationsPNI.SkipReadOnlyCellsOnTab = true;
            gTFLocationsPNI.CellUpdatedDelegate = gTFLocationsPNI_CellUpdated;
            //gTFLocationsPNI.RecordActivatedDelegate = PNIRowBeingClicked;

            //var filter = new RecordFilter();
            //filter.FieldName = "system_status";
            //filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.StartsWith, 'A'));
            //gTFLocations.xGrid.FieldLayouts[0].RecordFilters.Add(filter);
            //var filter2 = new RecordFilter();
            //filter2.FieldName = "status";
            //filter2.Conditions.Add(new ComparisonCondition(ComparisonOperator.StartsWith, 'A'));
            //gTFLocationsPNI.xGrid.FieldLayouts[0].RecordFilters.Add(filter2);

            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                //gTFLocations.ContextMenuAddIsVisible = false;
                //gTFLocations.ContextMenuRemoveIsVisible = false;
                gTFLocations.xGrid.FieldSettings.AllowEdit = false;
                //gTFLocationsPNI.ContextMenuAddIsVisible = false;
                //gTFLocationsPNI.ContextMenuRemoveIsVisible = false;
                gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = false;
            }
            GridCollection.Add(gTFLocations);
            GridCollection.Add(gTFLocationsPNI);
            //TFShowActiveDelegate();
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty locations ");
        }

        public void TFlocationsClearGrid()
        {
            this.CurrentBusObj.ObjectData.Tables["locations"].Rows.Clear();
        }

        //public void gAllocGrid_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    //This if tests to see if we are already in the got focus event as infragistics fires the got focus when the active record changes
        //    if (!gTFLocations.xGrid.IsFocused)
        //    {
        //        e.Handled = true;
        //        return;
        //    }

        //    return;
        //}
 
 

        private void gTFLocations_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        {
            //DataRecord GridRecord = null;
            //GridRecord = e.Cell.Record;
            //GridRecord.IsSelected = true;
            if (bLocationPNIUpdate)
                return;
            else
                bLocationUpdate = true;

            if (e.Cell.Field.Name == "move")
            {
                DataRecord GridRecord = null;
                //gTFLocations.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                GridRecord = e.Cell.Record;
                GridRecord.IsSelected = true;
                if (GridRecord != null)
                {
                    //if (GridRecord.IsSelected == false)
                    //    GridRecord.IsSelected = true;
                    if (GridRecord.Cells["move"].Value.ToString() == "1")
                    {
                        //DataRecord row = gTFLocations.ActiveRecord;
                        //row.Cells["move"].Value = e.Cell.Value;
                        foreach (DataRecord r in gTFLocationsPNI.xGrid.Records)
                        {
                            if ((r.Cells["move"].Value.ToString() == "0") && (r.Cells["cs_id"].Value.ToString() == GridRecord.Cells["cs_id"].Value.ToString()))
                                r.Cells["move"].Value = 1;
                        }
                    }
                    else
                    {
                        foreach (DataRecord r in gTFLocationsPNI.xGrid.Records)
                        {
                            if ((r.Cells["move"].Value.ToString() == "1") && (r.Cells["cs_id"].Value.ToString() == GridRecord.Cells["cs_id"].Value.ToString()))
                                r.Cells["move"].Value = 0;
                        }
                        bUncheckLine = true;
                        chkSelectAll.IsChecked = 0;
                    }
                    gTFLocationsPNI.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                    gTFLocations.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                    //btnSave.IsEnabled = true;
                }
            }
            //this.Load();
            //base.Save();
            //gTFLocations.LoadGrid(this.CurrentBusObj, "locations");
            //base.Save();
            //this.CurrentBusObj.LoadData("locations");
            //this.CurrentBusObj.ObjectData.Tables["locations"].AcceptChanges();
            //DataRecord GridRecord2 = null;
            //DataRecord row = gTFLocationsPNI.ActiveRecord;
            //this.CurrentBusObj.SaveTable("locations_pni");
            //(gTFLocationsPNI.xGrid.Records[0] as DataRecord).Cells["head_id"].IsActive = true;
            //(gTFLocationsPNI.xGrid.Records[0] as DataRecord).Cells["head_id"].IsSelected = true;
            //gTFLocationsPNI.xGrid.Records[0].IsSelected = true;
            //CurrentState = ScreenState.Normal;
            //this.CurrentBusObj.ObjectData.Tables["locations_pni"].AcceptChanges();
            //row = gTFLocationsPNI.ActiveRecord;
            //row.IsSelected = true;
            //row.IsActive = true;
            //this.CurrentBusObj.SaveTable("locations");
            bLocationUpdate = false;
        }

        private void gTFLocationsPNI_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        {
            if (bLocationUpdate)
                return;
            else
                bLocationPNIUpdate = true;

            bool bChecked = false;
            if (e.Cell.Field.Name == "move")
            {
                DataRecord GridRecord = null;
                //gTFLocationsPNI.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridRecord = gTFLocationsPNI.ActiveRecord;
                //gTFLocationsPNI.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                //GridRecord = e.Cell.Record;
                if (GridRecord != null)
                {
                    if (GridRecord.IsSelected == false)
                        GridRecord.IsSelected = true;
                    if (GridRecord.Cells["move"].Value.ToString() == "1")
                    {
                        bChecked = true;
                        foreach (DataRecord r in gTFLocations.xGrid.Records)
                        {
                            if ((r.Cells["move"].Value.ToString() == "0") && (r.Cells["cs_id"].Value.ToString() == GridRecord.Cells["cs_id"].Value.ToString()))
                            {
                                r.Cells["move"].Value = 1;
                                //bChecked = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRecord r in gTFLocationsPNI.xGrid.Records)
                        {
                            if ((r.Cells["move"].Value.ToString() == "1") && (r.Cells["cs_id"].Value.ToString() == GridRecord.Cells["cs_id"].Value.ToString()))
                                bChecked = true;
                        }
                        bUncheckLine = true;
                        chkSelectAll.IsChecked = 0;
                    }
                    if (!bChecked)
                    //    foreach (DataRecord r in gTFLocations.xGrid.Records)
                    //    {
                    //        if ((r.Cells["move"].Value.ToString() == "0") && (r.Cells["cs_id"].Value.ToString() == GridRecord.Cells["cs_id"].Value.ToString()))
                    //            r.Cells["move"].Value = 1;
                    //    }
                    //else
                        foreach (DataRecord r in gTFLocations.xGrid.Records)
                        {
                            if ((r.Cells["move"].Value.ToString() == "1") && (r.Cells["cs_id"].Value.ToString() == GridRecord.Cells["cs_id"].Value.ToString()))
                                r.Cells["move"].Value = 0;
                        }
                    //gTFLocationsPNI.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                    //gTFLocations.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                    //btnSave.IsEnabled = true;
                }
            }
            //this.CurrentBusObj.SaveTable("locations_pni");
            bLocationPNIUpdate = false;
        }

        public void GridDoubleClickDelegate()
        {
            //call invoiceactruletier detail screen
        }

        public void gTFLocations_RecordActivated()
        {
            //MessageBox.Show("Activated");
            //CalculateAmountAllocated();
        }

        public void gTFLocations_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            DataRecord row = gTFLocations.ActiveRecord;
        }

        private void chkSelectAll_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //Loop through and select all rows in the grid
            foreach (DataRecord r in gTFLocations.xGrid.Records)
            {

                if (r.Cells["move"].Value.ToString() == "0")
                    r.Cells["move"].Value = 1;

                gTFLocations.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
            }
            foreach (DataRecord r in gTFLocationsPNI.xGrid.Records)
            {

                if (r.Cells["move"].Value.ToString() == "0")
                    r.Cells["move"].Value = 1;

                gTFLocationsPNI.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
            }
            bUncheckLine = false;

        }

        private void chkSelectAll_UnChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!bUncheckLine)
                foreach (DataRecord r in gTFLocations.xGrid.Records)
                {

                    if (r.Cells["move"].Value.ToString() == "1")
                        r.Cells["move"].Value = 0;

                    gTFLocations.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                }
            bUncheckLine = false;
        }

        public void TFShowInactiveDelegate()
        {
            ////EmployeeMaintenance.ClearFilter();
            //var filter = new RecordFilter();
            //filter.FieldName = "system_status";
            //filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.DoesNotStartWith, 'Z'));
            ////Apply the filter to the grid
            //gTFLocations.xGrid.FieldLayouts[0].RecordFilters.Add(filter);
            //var filter2 = new RecordFilter();
            //filter2.FieldName = "status";
            //filter2.Conditions.Add(new ComparisonCondition(ComparisonOperator.DoesNotStartWith, 'Z'));
            //gTFLocationsPNI.xGrid.FieldLayouts[0].RecordFilters.Add(filter2);
            //int ShowComplete = 0;
            //if (Convert.ToBoolean(chkComplete.IsChecked))
            //{
            //    ShowComplete = 1;
            //}

            this.CurrentBusObj.Parms.UpdateParmValue("@status", "I");
            this.CurrentBusObj.LoadTable("locations");
            gTFLocations.LoadGrid(CurrentBusObj, "locations");
            this.CurrentBusObj.LoadTable("locations_pni");
            gTFLocations.LoadGrid(CurrentBusObj, "locations_pni");
        }
        public void TFShowActiveDelegate()
        {
            //var filter = new RecordFilter();
            //filter.FieldName = "system_status";
            //filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.StartsWith, 'A'));
            ////Apply the filter to the grid
            //gTFLocations.xGrid.FieldLayouts[0].RecordFilters.Add(filter);
            //var filter2 = new RecordFilter();
            //filter2.FieldName = "status";
            //filter2.Conditions.Add(new ComparisonCondition(ComparisonOperator.Equals, 'A'));
            //gTFLocationsPNI.xGrid.FieldLayouts[0].RecordFilters.Add(filter2);
            this.CurrentBusObj.Parms.UpdateParmValue("@status", "A");
            this.CurrentBusObj.LoadTable("locations");
            gTFLocations.LoadGrid(CurrentBusObj, "locations");
            this.CurrentBusObj.LoadTable("locations_pni");
            gTFLocations.LoadGrid(CurrentBusObj, "locations_pni");
        }

        public void RowBeingClicked()
        {
            wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
            if (wfStatus[0] == 'I')
                gTFLocations.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
            else
                gTFLocations.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = true;
        }

        public void PNIRowBeingClicked()
        {
            wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
            if (wfStatus[0] == 'I')
                gTFLocationsPNI.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
            else
                gTFLocationsPNI.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = true;
        }

    }
}
