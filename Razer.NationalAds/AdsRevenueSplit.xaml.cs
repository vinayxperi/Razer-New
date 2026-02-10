

using RazerBase.Interfaces; //Required for IScreen
using RazerBase;
using RazerBase.Lookups;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infragistics.Windows.DataPresenter;
using System;
using System.Data;
using System.Collections.Generic;
using Infragistics.Windows.Editors;

namespace Razer.NationalAds
{

   
    /// <summary>
    /// This class represents a 'AdsRevenueSplit' object.
    /// </summary>
    public partial class AdsRevenueSplit: ScreenBase, IScreen
    {
        String broadcastMonth = "01/01/1900";
        decimal totalAllocated = 0;
        public string WindowCaption { get { return string.Empty; } }

       
 
        /// Create a new instance of a 'LocationUnits' object and call the ScreenBase's constructor.
        /// </summary>
        public AdsRevenueSplit()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

        }

        public void Init(cBaseBusObject businessObject)
        {
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            this.CanExecuteNewCommand = false;
            this.CanExecuteSaveCommand = true;
            // Change this setting to the name of the DataTable that will be used for Binding.
            this.MainTableName = "AdsAllocation";
            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;

            //Establish the Ads Revenue Split Grid
            GridAdsProducts.MainTableName = "adsalloc";
            GridAdsProducts.ConfigFileName = "GridAdsRevenue";
            GridAdsProducts.SetGridSelectionBehavior(false, false);
            GridAdsProducts.SkipReadOnlyCellsOnTab = true;
            GridAdsProducts.FieldLayoutResourceString = "AdsRevenueSplit";
            GridAdsProducts.EditModeEndedDelegate = GridAdsProducts_EditModeEnded; //This allows for data checks after each cell is exited
            ////Turn off Grid config and filter abilities as these will mess up the hardwiring of tabbing thorugh grid
            GridAdsProducts.ContextMenuToggleFilterIsVisible = false;
            GridAdsProducts.ContextMenuResetGridSettingsIsVisible = false;
            GridAdsProducts.ContextMenuSaveGridSettingsIsVisible = false;
            GridAdsProducts.ContextMenuRemoveIsVisible = false;
            GridAdsProducts.ContextMenuAddIsVisible = false;
            GridAdsProducts.xGrid.FieldSettings.AllowEdit = true; 
            GridCollection.Add(GridAdsProducts);
             
             
            this.loadParms(broadcastMonth);
        }


        public void TotalAllocated()
        {
            decimal workingTot = 0;
            totalAllocated = 0;
            txtTotalAllocated.Text = "0";
            //Loop through the grid records looking for empty records
            //KSH 8/17/12 - check for nulls
            if (this.CurrentBusObj.ObjectData != null)
            {
                //KSH 8/17/12 - changed loop to look at datatable instead of grid cells to make this work
                foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["adsalloc"].Rows)
                {
                    workingTot = Convert.ToDecimal(dr["allocation"]);
                    totalAllocated = totalAllocated + Convert.ToDecimal(dr["allocation"]);
                }
                if (totalAllocated == 100)
                    txtTotalAllocated.Text = "100";
                else
                    txtTotalAllocated.Text = totalAllocated.ToString();
            }
        }
        
        private void loadParms(string broadcastMonth)
        {
            try
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                //if custId passed load external_char_id and recv_acct with passed customerId
                if ((broadcastMonth != "") & (broadcastMonth != "01/01/1900"))
                {
                    this.CurrentBusObj.Parms.AddParm("@broadcast_month", broadcastMonth);
                   
                }
                else
                {
                        this.CurrentBusObj.Parms.AddParm("@broadcast_month", "01/01/1900");
                      
                }
               
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }

        private void txtBroadcastDateStart_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void txtBroadcastDateStart_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((broadcastMonth != null) && (broadcastMonth == txtBroadcastMonth.SelText.ToString()))
                return;

            if (txtBroadcastMonth.SelText.ToString() == "")
            {
               
                return;
            }
            else
            {
                if (IsScreenDirty)
                {
                    System.Windows.MessageBoxResult result = Messages.ShowYesNo("Would you like to save existing changes?",
                               System.Windows.MessageBoxImage.Question);
                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        Save();
                        if (!SaveSuccessful)
                        {
                            Messages.ShowWarning("Save failed");
                            return;
                        }
                    }
                }

                this.CurrentBusObj.Parms.ClearParms();
                this.CurrentBusObj.Parms.UpdateParmValue ("@broadcast_month", txtBroadcastMonth.SelText.ToString());

                broadcastMonth = txtBroadcastMonth.SelText.ToString();
                
                this.Load();
                if (CurrentBusObj.ObjectData.Tables["adsalloc"].Rows.Count == 0)

                    Messages.ShowWarning("No Ads Products Returned");
                else
                {
                  
                    //do the following to get the cursor set to the allocation column in the grid
                    (GridAdsProducts.xGrid.Records[GridAdsProducts.ActiveRecord.Index] as DataRecord).Cells["allocation"].IsActive = true;


                    //Moves the cursor into the active cell - This code may be required to get the cell in edit mode without clicking.
                    GridAdsProducts.xGrid.Records.DataPresenter.BringCellIntoView(GridAdsProducts.xGrid.ActiveCell);
                    //gUnitEntry.xGrid.ActiveDataItem = 1;

                    //Puts the cell into edit mode
                    GridAdsProducts.SetRowCellFocus((GridAdsProducts.xGrid.Records[GridAdsProducts.ActiveRecord.Index] as DataRecord), "allocation");
                    GridAdsProducts.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                    //set the total allocated initially on load
                    this.TotalAllocated();
                }
            }   
        }

        public void GridAdsProducts_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            DataRecord row = GridAdsProducts.ActiveRecord;
            //get new total for allocation
            if (e.Cell.Field.Name == "allocation")
                this.TotalAllocated();
        }

        public override void Save()
        {
                //KSH 8/17/12 - must have for TotalAllocated nethod to work
                GridAdsProducts.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndCommitRecord);
                //Verify that the total = 100
                this.TotalAllocated();
                if (txtTotalAllocated.Text  != "100" )
                {
                    MessageBox.Show("Allocation Percent must total 100%");
                    SaveSuccessful = false;
                    return;
                }
                //  save the data
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

    }
}
 


      


 

 
