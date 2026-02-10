

#region using statements

using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using RazerInterface;
using Infragistics.Windows.DockManager;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows.Input;
#endregion

namespace Razer.BatchBilling
{


    /// <summary>
    /// This class represents a 'UpdateAccountingClass' object.
    /// </summary>
    public partial class UpdateAccountingClass : ScreenBase, IScreen, IPreBindable, IPostBindable
    {


        public string WindowCaption { get; set; }

        private int _acct_class_id = 0;
        private enum AcctClassRatioType { Percent, Ratio, Combo };

        public ComboBoxItemsProvider cmbDenominator { get; set; }
        public ComboBoxItemsProvider cmbNumerator { get; set; }

   
        //Stores the current ratio value in order to see if it needs to be reset when changing
        public int CurrentRatioValue { get; set; }

        //Firstload will turn off certain behaviors related to the use ratio flag when a new accounting class is loaded.
        private bool firstLoad = true;

        /// <summary>
        /// Create a new instance of a 'UpdateAccountingClass' object and call the ScreenBase's constructor.
        /// </summary>
        public UpdateAccountingClass()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            CurrentRatioValue = 0;
        }

        public override void New()
        {
            if (_acct_class_id != -1 && IsScreenDirty )
            {
                System.Windows.MessageBoxResult result = Messages.ShowYesNo("Would you like to save existing changes?",
                           System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    Save();
                }
            }
            this.CurrentBusObj.Parms.ClearParms();

            CurrentBusObj.ObjectData=null;
             _acct_class_id = -1;
            CurrentBusObj.Parms.AddParm("@acct_class_id", _acct_class_id);
            CurrentBusObj.Parms.AddParm("@code_name", "AcctClassType");
            CurrentBusObj.Parms.AddParm("@show_inactive", 0);
            EnableControls(true);
            ucAccTClassID.IsEnabled = false;
 

            base.New();

            gFilterGrid.LoadGrid(CurrentBusObj, "acct_class1");
            cbActive.IsChecked = 0;
            tbAcctClass.Text = "New Acct Class";
            tbDescription.Text = "New Acct Class Description";
            tbAcctClass.CntrlFocus();
            //CurrentBusObj.ObjectData.Tables["acct_class"].Rows.Add(0);
            //CurrentBusObj.ObjectData.Tables["acct_class"].Rows[0]["acct_class"] = "New Acct Class";
            //CurrentBusObj.ObjectData.Tables["acct_class"].Rows[0]["description"] = "New Acct Class Description";
            //CurrentBusObj.ObjectData.Tables["acct_class"].Rows[0]["status"] = 1;

        }

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            //CanExecuteSaveCommand = true;
            //CanExecuteNewCommand = false;

            // Set the ScreenBaseType
            CurrentBusObj = businessObject;
            
            MainTableName = "acct_class";
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.AllowAddNew = true;
            gFilterGrid.MainTableName = "acct_class1";
            gFilterGrid.ConfigFileName = "filtergrid1";
            gFilterGrid.xGrid.FieldLayoutSettings = f;
            gFilterGrid.SetGridSelectionBehavior(false, true);
            gFilterGrid.FieldLayoutResourceString = "AcctClassAlloc";
          
            gFilterGrid.ContextMenuAddIsVisible = false;
            gFilterGrid.ContextMenuRemoveDelegate = RemoveDelegate;
            gFilterGrid.ContextMenuRemoveIsVisible = true;
            
            
            //firstload = true;

            //businessObject.Parms.AddParm("@acct_class_id", -1);
            //businessObject.Parms.AddParm("@code_name", "AcctClassType");
            //businessObject.Parms.AddParm("@show_inactive", 0);
            //this.Load();
            //gFilterGrid.LoadGrid(CurrentBusObj, "acct_class1");
            //Set_Cells(); 

            //****If this screen will be zoomed to at some point then this code will need to be changed 
            //to use the zoom accounting class id if one is available
            LookupVals(-1);
            // Change this setting to the name of the DataTable that will be used for Binding.
            //      gFilterGrid.SelectedItemsChanged += new EventHandler<Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs>(xGrid_SelectedItemsChanged);
        }

        /// <summary>
        /// Handles zoom to lookup window and return of data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucAccTClassID_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            cGlobals.ReturnParms.Clear();
            AccountingClassLookup acl = new AccountingClassLookup();
            acl.ShowDialog();
            if (cGlobals.ReturnParms.Count > 0)
            {
                firstLoad = true;
                ucAccTClassID.Text = cGlobals.ReturnParms[0].ToString();

                LookupVals(Int32.Parse(ucAccTClassID.Text));
                EnableControls(true);
                tbAcctClass.CntrlFocus();
                CurrentBusObj.ObjectData.AcceptChanges();
            }

        }

        private void ucEntityClick_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            int tempAcctClassId;
            //if (!string.IsNullOrEmpty(ucAccTClassID.Text) && ucAccTClassID.Text != _acct_class_id.ToString())
            //{              //Clear the current parameters

            //Changed to tryparse to keep the query from crashing when a non numeric was entered.
            if (ucAccTClassID.IsEnabled)
            {
                if (Int32.TryParse(ucAccTClassID.Text, out tempAcctClassId))
                {
                    //If ID hasn't changed then do not requery the database
                    if (tempAcctClassId == _acct_class_id)
                        return;

                    LookupVals(tempAcctClassId);

                    //If data then update the window
                    if (chkForData())
                    {
                        EnableControls(true);
                        _acct_class_id = tempAcctClassId;
                        SetHeaderName();
                        //ucAccTClassID.Text = _acct_class_id.ToString();
                        CurrentBusObj.ObjectData.AcceptChanges();

                    }
                    //No data - reset to nothing
                    else
                    {
                        EnableControls(false);
                        ucAccTClassID.Text = "";
                        ucAccTClassID.CntrlFocus();
                    }

                }
            }

        }
   
  
        /// <summary>
        /// Returns the accounting class data from a typed in value or a lookup
        /// </summary>
        /// <param name="acct_class_id"></param>
        private void LookupVals(int acct_class_id)
        {
            //if (acct_class_id != -1)
            //{
                _acct_class_id = acct_class_id;
                CurrentBusObj.Parms.ClearParms();
                CurrentBusObj.Parms.AddParm("@acct_class_id", _acct_class_id);
                CurrentBusObj.Parms.AddParm("@code_name", "AcctClassType");
                CurrentBusObj.Parms.AddParm("@show_inactive", 0);
                firstLoad = true;
                this.Load(CurrentBusObj);

                gFilterGrid.LoadGrid(CurrentBusObj, "acct_class1");
                Set_Cells();
                firstLoad = false;
                CurrentBusObj.ObjectData.AcceptChanges();
            //}

        }

        /// <summary>
        /// Verifies that data was returned from a change in account class id
        /// </summary>
        /// <returns></returns>
        private bool chkForData()
        {
            bool hasdata = true;
            if (this.CurrentBusObj.ObjectData != null)
            {
                if (this.CurrentBusObj.ObjectData.Tables[0].Rows.Count != 0)
                {
                    hasdata = true;
                }
                else
                {
                    Messages.ShowWarning("Accounting Class Not Found");
                    hasdata = false;
                }
            }
            return hasdata;
        }


        /// <summary>
        /// Establishes which grid cells can be edited based on which use ratio type is picked.
        /// </summary>
        private void Set_Cells()
        {
            if (this.CurrentBusObj.HasObjectData)
            {
                DataTable dt = CurrentBusObj.GetTable("acct_class") as DataTable;
                if (dt.Rows.Count > 0)
                {
                    CurrentRatioValue = Convert.ToInt32(dt.Rows[0]["use_ratio_flag"]);
                    if (Convert.ToInt32(dt.Rows[0]["use_ratio_flag"]) == Convert.ToInt32(AcctClassRatioType.Percent))
                    {
                        gFilterGrid.xGrid.FieldLayouts[0].Fields["percentage"].Settings.AllowEdit = true;
                        gFilterGrid.xGrid.FieldLayouts[0].Fields["ratio_denominator"].Settings.AllowEdit = false;
                        gFilterGrid.xGrid.FieldLayouts[0].Fields["ratio_numerator"].Settings.AllowEdit = false;

                    }
                    else if (Convert.ToInt32(dt.Rows[0]["use_ratio_flag"]) == Convert.ToInt32(AcctClassRatioType.Ratio))
                    {
                        gFilterGrid.xGrid.FieldLayouts["acct_class1"].Fields["ratio_denominator"].Settings.AllowEdit = true;
                        gFilterGrid.xGrid.FieldLayouts[0].Fields["ratio_numerator"].Settings.AllowEdit = true;
                        gFilterGrid.xGrid.FieldLayouts[0].Fields["percentage"].Settings.AllowEdit = false;
                    }
                    else //Combo
                    {
                        gFilterGrid.xGrid.FieldLayouts[0].Fields["percentage"].Settings.AllowEdit = true;
                        gFilterGrid.xGrid.FieldLayouts[0].Fields["ratio_denominator"].Settings.AllowEdit = true;
                        gFilterGrid.xGrid.FieldLayouts[0].Fields["ratio_numerator"].Settings.AllowEdit = true;
                    }
                }
            }
        }

        private void SetHeaderName()
        {
            //Sets the header name when being called from another folder
            if (this.CurrentBusObj.ObjectData != null)
            {
                if (this.CurrentBusObj.ObjectData.Tables["acct_class"].Rows.Count > 0)
                {
                    if (ucAccTClassID.Text == null ||  ucAccTClassID.Text  == "")
                    {
                        WindowCaption = "Accounting Class - " + this.CurrentBusObj.ObjectData.Tables["acct_class"].Rows[0]["acct_class"].ToString();
                        ucAccTClassID.Text = this.CurrentBusObj.ObjectData.Tables["acct_class"].Rows[0]["acct_class_id"].ToString();
                    }
                    //Sets the header name from within same folder
                    else
                    {
                        ContentPane p = (ContentPane)this.Parent;
                        p.Header = "Accounting Class - " + this.CurrentBusObj.ObjectData.Tables["acct_class"].Rows[0]["acct_class"].ToString();
                    }
                }
            }
        }
 

        /// <summary>
        /// Handles change of accounting class use ratio type.
        /// If converted from ratio to percentage then all ratio records will be deleted
        /// If converted from percentage to ratio then all percentage records will be deleted
        /// Ratio, percentage and combo change also fires off event to disable and enable the proper grid columns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRatio_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!firstLoad && cbRatio.SelectedValue != null)
            {
                //Messages.ShowInformation("Selection Changed + CB Value: " + cbRatio.SelectedValue.ToString() + " CurrentValue:" + CurrentRatioValue.ToString());

                if ((CurrentBusObj.GetTable("acct_class") as DataTable) != null && (CurrentBusObj.GetTable("acct_class1") as DataTable).Rows.Count > 0)
                {
                    //Check to make sure there are detail rows
                    if (CurrentRatioValue != Convert.ToInt32(cbRatio.SelectedValue))
                    {
                        //Changing to a percentage type
                        if (Convert.ToInt32(cbRatio.SelectedValue) == Convert.ToInt32(AcctClassRatioType.Percent))
                        {
                            System.Windows.MessageBoxResult result = Messages.ShowYesNo("Changing to percentage will cause the existing ratio allocation records to be deleted.  Do you wish to continue?",
                                System.Windows.MessageBoxImage.Question);
                            if (result == System.Windows.MessageBoxResult.Yes)
                            {
                                DeleteAllocations(AcctClassRatioType.Percent);
                                Save();
                                Set_Cells();
                            }
                            else //User decided not to change
                            {
                                //(CurrentBusObj.GetTable("acct_class") as DataTable).Rows[0]["use_ratio_flag"] = CurrentRatioValue;
                                //firstLoad = true;
                                //cbRatio.SelectedValue = CurrentRatioValue;

                                ResetUseRatio();
                            }
                        }
                        //Changing to a ratio type
                        else if (Convert.ToInt32(cbRatio.SelectedValue) == Convert.ToInt32(AcctClassRatioType.Ratio))
                        {
                            System.Windows.MessageBoxResult result = Messages.ShowYesNo("Changing to ratio will cause the existing percentage allocation records to be deleted.  Do you wish to continue?",
                                System.Windows.MessageBoxImage.Question);
                            if (result == System.Windows.MessageBoxResult.Yes)
                            {
                                DeleteAllocations(AcctClassRatioType.Ratio);
                                Save();
                                Set_Cells();
                            }
                            else //User decided not to change
                            {
                                ResetUseRatio();
                            }
                        }
                        //Changing to a combo type
                        else if (Convert.ToInt32(cbRatio.SelectedValue) == Convert.ToInt32(AcctClassRatioType.Combo))
                        {
                            Set_Cells();
                        }

                    }
                }
                else
                {
                    Set_Cells();
                }

            }
            else if (firstLoad && cbRatio.SelectedValue != null)
            {
                firstLoad = false;
            }


  
        }

        /// <summary>
        /// Deletes the rows that have a percentage if ratio is selected or a ratio if percentage is selecetd and immediately perofrms a save.
        /// </summary>
        /// <param name="Type"></param>
        private void DeleteAllocations(AcctClassRatioType Type)
        {
            if (CurrentBusObj.ObjectData != null)
            {
                foreach (DataRow r in CurrentBusObj.ObjectData.Tables["acct_class1"].Rows)
                {
                    if (Type == AcctClassRatioType.Percent && Convert.ToInt32(r["ratio_numerator"]) != 0)
                        r.Delete();
                    else if (Type == AcctClassRatioType.Ratio && Convert.ToInt32(r["percentage"]) > 0)
                        r.Delete();
                }

            }

        }

        /// <summary>
        /// Sets the ratio used back to the current value if a user decided against changing it.
        /// </summary>
        private void ResetUseRatio()
        {
            firstLoad = true;
            (CurrentBusObj.GetTable("acct_class") as DataTable).Rows[0]["use_ratio_flag"] = CurrentRatioValue;
            //cbRatio.SelectedValue = CurrentRatioValue;
            //firstLoad = false;

        }

        public override void Save()
        {
            //RES 11/11/15 validate percentages total 100 when use ratio is percentage
            decimal TotalPct = 0;
            DataTable dt = CurrentBusObj.GetTable("acct_class") as DataTable;
            if (dt.Rows.Count > 0)
            {
                tbDescription.CntrlFocus();
                if (Convert.ToInt32(dt.Rows[0]["use_ratio_flag"]) == Convert.ToInt32(AcctClassRatioType.Percent))
                {
                    foreach (DataRecord x in gFilterGrid.xGrid.Records)
                    {
                        TotalPct = TotalPct +  Convert.ToDecimal(x.Cells["percentage"].Value);
                    }
                    if (TotalPct != 100)
                    {
                        Messages.ShowWarning("Percentage(s) must total 100 for Use Ratio type of Pctg");
                        return;
                    }
                }
            }
            firstLoad = true;
            base.Save();
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Accounting Class Saved!");
                Int32.TryParse(ucAccTClassID.Text,out _acct_class_id);
                gFilterGrid.LoadGrid(CurrentBusObj, "acct_class1");
                CurrentBusObj.ObjectData.AcceptChanges();
            }

            firstLoad = false;
            ucAccTClassID.IsEnabled = true;

        }

        public void PreBind()
        {
            if (this.CurrentBusObj.HasObjectData)
            {
                DataRow dr = CurrentBusObj.ObjectData.Tables["filter_acct"].NewRow();
                dr["filter_id"] = 0;
                dr["filter_name"] = " ";
                this.CurrentBusObj.ObjectData.Tables["filter_acct"].Rows.InsertAt(dr, 0);
                cmbDenominator = new ComboBoxItemsProvider();
                cmbDenominator.ItemsSource = this.CurrentBusObj.ObjectData.Tables["filter_acct"].DefaultView;
                cmbDenominator.ValuePath = "filter_id";
                cmbDenominator.DisplayMemberPath = "filter_name";
                cmbNumerator = new ComboBoxItemsProvider();
                cmbNumerator.ItemsSource = CurrentBusObj.ObjectData.Tables["filter_acct"].DefaultView;
                cmbNumerator.ValuePath = "filter_id";
                cmbNumerator.DisplayMemberPath = "filter_name";
                this.cbRatio.SetBindingExpression("fkey_int", "code_value", this.CurrentBusObj.GetTable("ratio_flag") as DataTable, "");
            }

        }
        public void PostBind()
        {

        }

 

        private void RemoveDelegate()
        {
            DataRecord r = gFilterGrid.ActiveRecord;
            if (r != null)
            {
                DataRow row = (r.DataItem as DataRowView).Row;
                if (row != null)
                {
                    row.Delete();
                }
            }
        }

        /// <summary>
        /// Turns header controls on and off
        /// </summary>
        /// <param name="Enable"></param>
        public void EnableControls(bool Enable)
        {
            ucAccTClassID.IsEnabled = true;
            tbAcctClass.IsEnabled = Enable;
            cbActive.IsEnabled = Enable;
            cbRatio.IsEnabled = Enable;
            tbDescription.IsEnabled = Enable;
        }

  
    }
}
    


                           
 