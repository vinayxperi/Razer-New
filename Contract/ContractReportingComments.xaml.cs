using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using System.Windows.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows.Shapes;

using Infragistics.Windows.Editors;
using System.Collections.Generic;

namespace Contract
{
    /// <summary>
    /// Interaction logic for ContractReportingComments.xaml
    /// </summary>
    public partial class ContractReportingComments : ScreenBase 
    {

        public cBaseBusObject ContractReportingCommentsBusObject = new cBaseBusObject();
        private string bindPath;
        public int ContractId = 0;
        public int ReportID = 0;
        public bool IsSingleClickOrigin { get; set; }
        private DataRow rDefault;
        //contract object from caller
        cBaseBusObject ContractObj;
        public ComboBoxItemsProvider cmbCommentCode { get; set; }
        //setting to true keeps the single click delegate from firing when context menu add record is selected
        public bool ContextAddClicked { get; set; }
        //used to reflect comment code which is really comment type from different folders, this defaults to RA if a value is not specified
        public string CommentCode { get; set; }
        public int CommentCodeDDDWValue { get; set; }
        private cBaseBusObject busObjHold = null;

        public ContractReportingComments(int _ContractId, int _ReportID, cBaseBusObject _ContractObj)
            : base()
        {
            //set obj
            this.CurrentBusObj = ContractReportingCommentsBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "ContractReporting";
            ContractId = _ContractId;
            //get handle to contract obj
            ReportID = _ReportID;
            ContractObj = _ContractObj;
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
        
            InitializeComponent();
            Init();
        }
      

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            //set parameters
            this.CurrentBusObj.Parms.AddParm("@comment_type", "CT");
            this.CurrentBusObj.Parms.AddParm("@comment_code", 22);  //reporting
            this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-1");
            //attachment parms
            this.CurrentBusObj.Parms.AddParm("@attachment_type", "CATTACH");
            //Modified DWR 1/27
            this.CurrentBusObj.Parms.AddParm("@external_char_id", ReportID.ToString());
            this.CurrentBusObj.Parms.AddParm("@external_int_id", ContractId.ToString());
            this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
            //set to false as default
            ContextAddClicked = false;
            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "comments_char";
            RGridComments.MainTableName = "comments_char";

            RGridComments.xGrid.SelectedItemsChanged += new EventHandler<Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs>(GridComments_SelItemsChanged);
            RGridComments.ContextMenuAddDelegate = ContextMenuAddDelegate;
            RGridComments.ContextMenuAddDisplayName = "Add New Comment";
            RGridComments.xGrid.FieldSettings.AllowEdit = true;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            RGridComments.SetGridSelectionBehavior(true, false);
            RGridComments.FieldLayoutResourceString = "RCommentsGrid";
            
            GridCollection.Add(RGridComments);
            //load bus obj
            this.Load();
           
            //setup attachment grid properties
            uRCommentAttachments.mainTableName = "comment_attachment";
            uRCommentAttachments.fieldLayoutResource = "RCommentAttachmentGrid";
            uRCommentAttachments.LoadGrid("0");

        }

      
        public void doSingleClick()
        {
            //populate comment attachment grid
            //don't run if context menu add is clicked
            if (ContextAddClicked == true) return;
            RGridComments.ReturnSelectedData("comment_id");
            //GridComments.ReturnSelectedDataFromActiveRecord("comment_id");
            if (cGlobals.ReturnParms.Count > 0)
            {
                //this.Text = "";
                //get comment text using comment_code
                string commentID = cGlobals.ReturnParms[0].ToString();
             
                if (commentID != "")
                {
                    //set text
                    //   Int32 commentInt = Convert.ToInt32(commentID);
               
                    //foreach (Record record in RGridComments.xGrid.SelectedItems.Records)
                    //{

                    //    DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;

                    //    if (commentInt == Convert.ToInt32(r["comment_id"]))
                    //        txtRCommentText.Text = (r["comment_text"].ToString());
                    //}
                    
                    //enable comment / attachments grid
                    uRCommentAttachments.IsEnabled = true;
                    //Check that comment_attachment_id parm exists, if not create. It should always exist, but this saves from runtime if does not
                    if (CommentAttachParmExists())
                        this.CurrentBusObj.changeParm("@comment_attachments_id", commentID);
                    else
                        this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", commentID);
                    busObjHold = this.CurrentBusObj;
                    this.CurrentBusObj.LoadTable("comment_attachment");
                    uRCommentAttachments.CurrentBusObj = null;
                    uRCommentAttachments.CurrentBusObj = this.CurrentBusObj;
                    uRCommentAttachments.LoadGrid();
                }
                else
                {
                    clearAttachmentGrid();
                    //disable comment / attachments grid
                    uRCommentAttachments.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// checks for comment_attachment_parm
        /// </summary>
        /// <returns></returns>
        private bool CommentAttachParmExists()
        {
            var row = from x in this.CurrentBusObj.Parms.ParmList.AsEnumerable()
                                  where x.Field<string>("parmName") == "@comment_attachments_id"
                                  select x.Field<string>("Value");

            foreach (var val in row)
            {
                //return true if comment_attachment_id found in parm list
                return true;
            }
            return false;
        }

        private void clearAttachmentGrid()
        {
            //clear attachments grid if no comment id is passed
            //Check that comment_attachment_id parm exists, if not create.
            if (CommentAttachParmExists())
                this.CurrentBusObj.changeParm("@comment_attachments_id", "-250");
            else
                this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-250");
            //this.CurrentBusObj.changeParm("@comment_attachments_id", "-250");
            this.CurrentBusObj.LoadTable("comment_attachment");
            uRCommentAttachments.CurrentBusObj = null;
            uRCommentAttachments.CurrentBusObj = this.CurrentBusObj;
            uRCommentAttachments.LoadGrid();
        }

        private void ContextMenuAddDelegate()
        {
            ContextAddClicked = true;
            //disable comment / attachments grid
            uRCommentAttachments.IsEnabled = false;
            //set RA as default because customer folder does not pass comment code KSH
            if (CommentCode == "") CommentCode = "CT";
            //CLB Changing DDDWValue to 15 to default to Misc
            if (CommentCodeDDDWValue ==  0 ) CommentCodeDDDWValue =  22 ;
            RGridComments.xGrid.FieldLayoutSettings.AllowAddNew = false;
            DataView dataSource = this.RGridComments.xGrid.DataSource as DataView;

            if (dataSource != null)
            {
                RGridComments.xGrid.FieldSettings.AllowEdit = true;
                //Add new grid row and set cursor in first cell of last row
                DataRowView row = dataSource.AddNew();
                RGridComments.xGrid.ActiveDataItem = row;
                //GridComments.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                RGridComments.xGrid.ActiveCell = (RGridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
                //auto add date, comment type, username record to grid, user will add subject and text
                //add date
                (RGridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = DateTime.Now.ToString("MM/dd/yyyy");
                //add comment code CLB - added comment CODE because it was calling comment type comment code
                (RGridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = 22;
                //add user name
                //add comment type
                (RGridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = CommentCode;
                //add user name
                (RGridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = UserName.GetUserName;
                //set subj to nothing
                (RGridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = "Report ID: " + ReportID.ToString();
                //set external int id
                (RGridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells["external_int_id"].Value = ContractId.ToString();
                //set external int id
                (RGridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells["external_char_id"].Value = ReportID.ToString();
                //Set focus to Subject Cell and place in edit mode
                RGridComments.xGrid.ActiveCell = (RGridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells["subject"];
                RGridComments.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                //(GridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = "";
                txtRCommentText.Text = "";
                RGridComments.xGrid.ActiveCell.Field.Settings.AllowEdit = true;
                ContextAddClicked = false;
                clearAttachmentGrid();


              
            }
            else
            {
                ContextAddClicked = false;
                return;
            }

        }

        private void GridComments_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.CurrentBusObj != null)
            {
                //doSingleClick();
                //if (GridComments.
                //bring the first record into view and set selector on it
                //GridComments.xGrid.ExecuteCommand(DataPresenterCommands.RecordFirstOverall);
                ////make the first record the active record
                //GridComments.xGrid.ActiveRecord.IsActive = true;
                ////call the delegate that populates the attachments grid
               // SingleClickZoomDelegate();
            }
            //DWR-Added 4/20/12 to make the attachment screen base on the comment screen get the updated security for the comment screen
            uRCommentAttachments.SecurityContext = this.SecurityContext;
            uRCommentAttachments.SetSecurity();
            //TabCollection.Add(uCommentAttachments);
        }

      
        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Save Successful");
                //add contract_id parm for refresh of location grid on location tab
                //ContractObj.Parms.UpdateParmValue("@contract_id", ContractId);
                //ContractObj.LoadTable("contract_report");
                //CloseScreen();
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }

        private void CloseScreen()
        {
            System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                AdjParent.Close();
            }
        }

        public void GridUnHideCommentType()
        {
            RGridComments.xGrid.FieldLayouts[0].Fields[1].Visibility = Visibility.Visible;
        }

        private void GridComments_SelItemsChanged(object sender, RoutedEventArgs e)
        {
            doSingleClick();
        }

        private void txtCommentText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            foreach (Record record in RGridComments.xGrid.SelectedItems.Records)
            {

                DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;

              r["comment_text"] = txtRCommentText.Text;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

     }
        }
 
