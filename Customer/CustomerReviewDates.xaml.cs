

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;

#endregion

namespace Customer
{

    #region class CustomerReviewDates
    /// <summary>
    /// This class represents a 'CustomerReviewDates' object.
    /// </summary>
  //  public partial class CustomerReviewDates : ScreenBase 
         public partial class CustomerReviewDates : ScreenBase, IPreBindable
    {

        //for reviewer
        public ComboBoxItemsProvider cmbReviewer { get; set; }
        bool newDate = false;
        bool errorsExist = false;

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'CustomerReviewDates' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerReviewDates()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Methods

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            //Set field layout information
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.AllowDelete = false;
            f.AddNewRecordLocation = AddNewRecordLocation.OnTop;
            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "customer_review_dates";
            GridReviewDates.MainTableName = "customer_review_dates";
            GridReviewDates.ConfigFileName = "CustomerReviewDates";
            GridReviewDates.xGrid.FieldSettings.AllowEdit = true;
            //GridReviewDates.WindowZoomDelegate = ReturnSelectedData;
            GridReviewDates.ContextMenuAddDelegate = CustomerReviewAddDelegate;
            GridReviewDates.ContextMenuAddDisplayName = "Add Review Date";
            GridReviewDates.ContextMenuAddIsVisible = true;
            GridReviewDates.ContextMenuRemoveIsVisible = false;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridReviewDates.SetGridSelectionBehavior(false, false);
            GridReviewDates.FieldLayoutResourceString = "CustomerReviewDatesGrid";
            GridCollection.Add(GridReviewDates);
        }

        private void CustomerReviewAddDelegate()
        {
            newDate = true;
            GridReviewDates.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = GridReviewDates.xGrid.RecordManager.CurrentAddRecord;
            //Set the default values for the columns
            row.Cells["receivable_account"].Value = getCustomerId();
            row.Cells["review_date"].Value = "01/01/1900";
            row.Cells["review_reason"].Value = "";
            row.Cells["date_reviewed"].Value = "01/01/1900";
            row.Cells["reviewed_by"].Value = "";
            row.Cells["review_result"].Value = "";
            row.Cells["targeted_reviewer"].Value = "";
            row.Cells["review_frequency"].Value = 0;
            row.Cells["reviewed_flag"].Value = 0;
            row.Cells["auto_close_flag"].Value = 0;
            row.Cells["review_notice_date"].Value = "1/1/1900";
            //Commit the add new record - required to make this record active
            GridReviewDates.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            GridReviewDates.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            GridReviewDates.xGrid.ActiveRecord = GridReviewDates.xGrid.Records[0];
            //Set the field as active
            (GridReviewDates.xGrid.Records[GridReviewDates.ActiveRecord.Index] as DataRecord).Cells["review_date"].IsActive = true;
            //Moves the cursor into the active cell 
            GridReviewDates.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }

        private string getCustomerId()
        {
            var localCustomerId = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                  where x.Field<string>("parmName") == "@receivable_account"
                                  select x.Field<string>("parmValue");

            foreach (var id in localCustomerId)
            {
                string CustomerId =  id.ToString();
                //return contract id
                return CustomerId;
            }
            return " ";
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
        public void ReturnSelectedData()
        {
            //Zoom Functionality

        }

        #endregion

    }
    #endregion

}
