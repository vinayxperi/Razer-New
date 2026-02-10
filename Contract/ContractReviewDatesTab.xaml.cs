



using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;



namespace Contract
{

  
    /// <summary>
    /// This class represents a 'ContractLocationsTab' object.
    /// </summary>
    public partial class ContractReviewDatesTab : ScreenBase, IPreBindable
    {
        /// <summary>
        bool newDate = false;
        bool errorsExist = false;
        
        //for reviewer
        public ComboBoxItemsProvider cmbReviewer { get; set; }
        /// Create a new instance of a 'ContractReviewDatesTab' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractReviewDatesTab()
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
            MainTableName = "contract_review";
            //Set field layout information
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.AllowDelete = false;
            f.AddNewRecordLocation = AddNewRecordLocation.OnTop;
            
            //Contract Location Grid
            gReviewDates.MainTableName = "contract_review";
            gReviewDates.xGrid.FieldSettings.AllowEdit = true;
            gReviewDates.SetGridSelectionBehavior(false, false);
            gReviewDates.ContextMenuAddDelegate = ContractReviewAddDelegate;
            gReviewDates.ContextMenuAddDisplayName = "Add Review Date";
            gReviewDates.ContextMenuAddIsVisible = true;
            gReviewDates.ContextMenuRemoveIsVisible = false;
            gReviewDates.ConfigFileName = "ContractReviewDates";
            gReviewDates.FieldLayoutResourceString = "ContractReviewDates";
            
            GridCollection.Add(gReviewDates);
           
        }

        


        /// <summary>
        /// Locations grid add delegate, calls Location Service Lookup to add new locations to contract
        /// </summary>
        private void ContractReviewAddDelegate()
        {
            newDate = true;
            gReviewDates.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = gReviewDates.xGrid.RecordManager.CurrentAddRecord;
            //Set the default values for the columns
            row.Cells["contract_id"].Value = getContractId();
            row.Cells["review_date"].Value = "01/01/1900";
            row.Cells["review_reason"].Value = "";
            row.Cells["date_reviewed"].Value = "01/01/1900";
            row.Cells["reviewed_by"].Value = "";
            row.Cells["review_result"].Value = "";
            row.Cells["targeted_reviewer"].Value = "";
            row.Cells["review_frequency"].Value = 0;
            row.Cells["reviewed_flag"].Value = 0;
            row.Cells["auto_close_flag"].Value = 0;
            //Commit the add new record - required to make this record active
            gReviewDates.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            gReviewDates.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            gReviewDates.xGrid.ActiveRecord = gReviewDates.xGrid.Records[0];
            //Set the field as active
            (gReviewDates.xGrid.Records[gReviewDates.ActiveRecord.Index] as DataRecord).Cells["review_date"].IsActive = true;
            //Moves the cursor into the active cell 
            gReviewDates.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }

        /// <summary>
       
        private int getContractId()
        {
            var localContractId = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                  where x.Field<string>("parmName") == "@contract_id"
                                  select x.Field<string>("parmValue");

            foreach (var id in localContractId)
            {
                int ContractId = Convert.ToInt32(id);
                //return contract id
                return ContractId;
            }
            return 0;
        }

        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {

                    //Setup Grid Combo Boxes
                    //Product drop down box 
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["reviewer"].DefaultView;
                    ip.ValuePath = "user_id";
                    ip.DisplayMemberPath = ("user_name");
                    cmbReviewer = ip;
                  
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

       

        //public bool ValidatebeforeSave()
        //{
           
        //    ////Need to validate targeted_reviewer is populated
        //    foreach (DataRow drE in this.CurrentBusObj.ObjectData.Tables["contract_review"].Rows)
        //    {
        //        string sdebug;
        //        sdebug = drE["targeted_reviewer"].ToString();
        //        //if datarowEnty row state is deleted do not do this
        //        if (drE.RowState != DataRowState.Deleted && Convert.ToString(drE["targeted_reviewer"]) == " ")
        //        {
        //            MessageBox.Show("Targeted Reviewer must be populated on the Contract Review Tab");
        //            return true;
        //        }
                 
        //        if (drE["targeted_reviewer"] == null)
        //        {
        //            MessageBox.Show("Targeted Reviewer must be populated on the Contract Review Tab");
        //            return true;
        //        }
                

        //    }
        //    return false;
            

        //}
       



    }


}
