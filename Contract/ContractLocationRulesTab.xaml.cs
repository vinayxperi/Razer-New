
using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;

namespace Contract
{
    /// <summary>
    /// This class represents a 'ContractLocationRulesTab' object.
    /// </summary>
    public partial class ContractLocationRulesTab : ScreenBase
    {
        /// <summary>
        /// Create a new instance of a 'ContractLocationRulesTab' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractLocationRulesTab()
            : base()
        {
            //Creates a routed event for the xamCheckEditor, scope is project wide so all xamcheck boxes will be affected by this code when it runs
            EventManager.RegisterClassHandler(typeof(ValueEditor), ValueEditor.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(OnValueChanged));
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }

        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            MainTableName = "location_rules";
            //Contract Location Rules Grid
            gLocationRules.MainTableName = "location_rules";
            gLocationRules.xGrid.FieldSettings.AllowEdit = true;
            gLocationRules.SetGridSelectionBehavior(false, false);
            gLocationRules.ContextMenuAddIsVisible = false;
            gLocationRules.ContextMenuRemoveIsVisible = false;
            gLocationRules.ConfigFileName = "ContractLocationRules";
            gLocationRules.FieldLayoutResourceString = "GridContractLocationRules";
            gLocationRules.EditModeEndedDelegate = LocGridEditModeEndedDelegate;
            //not in use
            //gLocationRules.CellUpdatedDelegate = gLocationRules_CellUpdated;
            GridCollection.Add(gLocationRules);
        }

        /// <summary>
        /// must be handled in order to force grid cell out of edit mode in order for xamCheckEditor et al to update values on check on/off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        private void OnValueChanged(object sender, RoutedEventArgs e)
        {
            //DO NOT USE THE LINE BELOW : it messes up data entry into the comboboxes, can only be used with check box
            //if (sender is XamComboEditor || sender is XamCheckEditor || sender is XamDateTimeEditor)
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (sender is XamCheckEditor)
            {
                (sender as ValueEditor).EndEditMode(true, true);
            }
        }

        /// <summary>
        /// enable batch processing fields and set grid for multi-row updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBatch_Checked(object sender, RoutedEventArgs e)
        {
            brdBatch.IsEnabled = true;
            gLocationRules.xGrid.FieldSettings.AllowEdit = false;
            gLocationRules.SetGridSelectionBehavior(true, true);
        }

        /// <summary>
        /// disable fields and set grid mode for single row updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBatch_UnChecked(object sender, RoutedEventArgs e)
        {
            brdBatch.IsEnabled = false;
            gLocationRules.xGrid.FieldSettings.AllowEdit = true;
            gLocationRules.SetGridSelectionBehavior(false, false);
        }

        /// <summary>
        /// Used to add default values to fields when override check box is clicked in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LocGridEditModeEndedDelegate(object sender, RoutedEventArgs e)
        {
            if (gLocationRules.xGrid.Records.Count > 0)
            {
                if (Convert.ToInt32(gLocationRules.ActiveRecord.Cells["override_flag"].Value) == 1) 
                {
                    if (Convert.ToDateTime(gLocationRules.ActiveRecord.Cells["effective_start_date"].Value).ToString("MM/dd/yyyy") == "01/01/1900")
                    {
                        gLocationRules.ActiveRecord.Cells["effective_start_date"].Value = gLocationRules.ActiveRecord.Cells["rule_start_date"].Value;
                        gLocationRules.ActiveRecord.Cells["effective_end_date"].Value = gLocationRules.ActiveRecord.Cells["rule_end_date"].Value;
                        gLocationRules.ActiveRecord.Cells["roll_forward_flag"].Value = 1;
                    }
                }
                else
                {
                    gLocationRules.ActiveRecord.Cells["effective_start_date"].Value = "01/01/1900";
                    gLocationRules.ActiveRecord.Cells["effective_end_date"].Value = "01/01/1900";
                    gLocationRules.ActiveRecord.Cells["roll_forward_flag"].Value = 0;
                }
            }
        }

        /// <summary>
        /// criteria from batch processing fields applied on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < gLocationRules.xGrid.SelectedItems.Records.Count; i++)
            { 
                DataRecord dr = gLocationRules.xGrid.SelectedItems.Records[i] as DataRecord;
                dr.Cells["override_flag"].Value = chkOverride.IsChecked;
                if (dtpEffStartDate.SelText.ToString() == "")
                    dr.Cells["effective_start_date"].Value = Convert.ToDateTime("01/01/1900");
                else
                    dr.Cells["effective_start_date"].Value = dtpEffStartDate.SelText.ToString();
                if (dtpEffEndDate.SelText.ToString() == "")
                    dr.Cells["effective_end_date"].Value = Convert.ToDateTime("01/01/1900");
                else
                    dr.Cells["effective_end_date"].Value = dtpEffEndDate.SelText.ToString();
                dr.Cells["roll_forward_flag"].Value = chkRoll.IsChecked;
            }
            clearBatchSelections();
        }

        /// <summary>
        /// clears batch processing selection criteria
        /// </summary>
        public void clearBatchSelections()
        {
            chkOverride.IsChecked = 0;
            dtpEffStartDate.SelText = Convert.ToDateTime("01/01/1900");
            dtpEffEndDate.SelText = Convert.ToDateTime("01/01/1900");
            chkRoll.IsChecked = 0;
            chkBatch.IsChecked = 0;
        }
         
    }
}
