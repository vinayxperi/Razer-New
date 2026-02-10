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
namespace RazerBase
{
    /// <summary>
    /// This class represents a 'ucComments' object.
    /// </summary>
    public partial class ucComments : ScreenBase, IPreBindable
    {
        private string bindPath;
        public ComboBoxItemsProvider cmbCommentCode { get; set; }
        //setting to true keeps the single click delegate from firing when context menu add record is selected
        public bool ContextAddClicked { get; set; }
        //used to reflect comment code which is really comment type from different folders, this defaults to RA if a value is not specified
        public string CommentCode { get; set; }
        public int CommentCodeDDDWValue { get; set; }
        private cBaseBusObject busObjHold = null;

        /// <summary>
        /// Create a new instance of a 'ucComments' object and call the ScreenBase's constructor.
        /// </summary>
        public ucComments()
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
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            //set to false as default
            ContextAddClicked = false;
            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "comments_char";
            GridComments.MainTableName = "comments_char";

            GridComments.xGrid.SelectedItemsChanged += new EventHandler<Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs>(GridComments_SelItemsChanged);
            GridComments.ContextMenuAddDelegate = ContextMenuAddDelegate;
            GridComments.ContextMenuAddDisplayName = "Add New Comment";
            GridComments.xGrid.FieldSettings.AllowEdit = true;
            GridComments.xGrid.FieldLayoutSettings.AllowDelete = false;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridComments.SetGridSelectionBehavior(true, false);
            GridComments.FieldLayoutResourceString = "CommentsGrid";
            
            GridCollection.Add(GridComments);

            //setup attachment grid properties
            uCommentAttachments.mainTableName = "comment_attachment";
            uCommentAttachments.fieldLayoutResource = "CommentAttachmentGrid";
            uCommentAttachments.LoadGrid("0");

        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ucComments),
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        /// <summary>
        /// This property gets or sets the value for 'BindPath'.
        /// </summary>
        public string BindPath
        {
            get { return bindPath; }
            set
            {
                // set the value
                bindPath = value;

                // if the bindPath is set
                if (bindPath != null)
                {
                    // Set the value
                    Binding binding = new Binding(bindPath);

                    // Set to two way mode
                    binding.Mode = BindingMode.TwoWay;

                    // Set the update source trigger
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    // set the binding
                    this.SetBinding(TextProperty, binding);
                }
            }
        }

        public void doSingleClick()
        {
            //populate comment attachment grid
            //don't run if context menu add is clicked
            if (ContextAddClicked == true) return;
            GridComments.ReturnSelectedData("comment_id");
            //GridComments.ReturnSelectedDataFromActiveRecord("comment_id");
            if (cGlobals.ReturnParms.Count > 0)
            {
                this.Text = "";
                //get comment text using comment_code
                string commentID = cGlobals.ReturnParms[0].ToString();
                if (commentID != "")
                {
                    //enable comment / attachments grid
                    uCommentAttachments.IsEnabled = true;
                    //Check that comment_attachment_id parm exists, if not create. It should always exist, but this saves from runtime if does not
                    if (CommentAttachParmExists())
                        this.CurrentBusObj.changeParm("@comment_attachments_id", commentID);
                    else
                        this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", commentID);
                    busObjHold = this.CurrentBusObj;
                    this.CurrentBusObj.LoadTable("comment_attachment");
                    uCommentAttachments.CurrentBusObj = null;
                    uCommentAttachments.CurrentBusObj = this.CurrentBusObj;
                    uCommentAttachments.LoadGrid();
                }
                else
                {
                    clearAttachmentGrid();
                    //disable comment / attachments grid
                    uCommentAttachments.IsEnabled = false;
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
            uCommentAttachments.CurrentBusObj = null;
            uCommentAttachments.CurrentBusObj = this.CurrentBusObj;
            uCommentAttachments.LoadGrid();
        }

        private void ContextMenuAddDelegate()
        {
            ContextAddClicked = true;
            //disable comment / attachments grid
            uCommentAttachments.IsEnabled = false;
            //set RA as default because customer folder does not pass comment code KSH
            if (CommentCode == "") CommentCode = "RA";
            //CLB Changing DDDWValue to 15 to default to Misc
            if (CommentCodeDDDWValue ==  0 ) CommentCodeDDDWValue =  15 ;
            GridComments.xGrid.FieldLayoutSettings.AllowAddNew = false;
            DataView dataSource = this.GridComments.xGrid.DataSource as DataView;

            if (dataSource != null)
            {
                GridComments.xGrid.FieldSettings.AllowEdit = true;
                //Add new grid row and set cursor in first cell of last row
                DataRowView row = dataSource.AddNew();
                GridComments.xGrid.ActiveDataItem = row;
                //GridComments.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridComments.xGrid.ActiveCell = (GridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
                //auto add date, comment type, username record to grid, user will add subject and text
                //add date
                (GridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = DateTime.Now.ToString("MM/dd/yyyy");
                //add comment code CLB - added comment CODE because it was calling comment type comment code
                (GridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = 15;
                //add user name
                //add comment type
                (GridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = CommentCode;
                //add user name
                (GridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = UserName.GetUserName;
                //set subj to nothing
                (GridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = "";
                //Set focus to Subject Cell and place in edit mode
                GridComments.xGrid.ActiveCell = (GridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells["subject"];
                GridComments.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                //(GridComments.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = "";
                txtCommentText.Text = "";
                GridComments.xGrid.ActiveCell.Field.Settings.AllowEdit = true;
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
            uCommentAttachments.SecurityContext = this.SecurityContext;
            uCommentAttachments.SetSecurity();
            //TabCollection.Add(uCommentAttachments);
        }

        public void PreBind()
        {
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                if (this.CurrentBusObj.HasObjectData)
                {
                    if (this.CurrentBusObj.ObjectData.Tables["dddwcommentcode"] == null)
                        return;
                    ComboBoxItemsProvider provider = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    provider.ItemsSource = this.CurrentBusObj.ObjectData.Tables["dddwcommentcode"].DefaultView;

                    //set the value and display path
                    provider.ValuePath = "comment_code";
                    provider.DisplayMemberPath = "description";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbCommentCode = provider;
                }
            }
        }

        public void GridUnHideCommentType()
        {
            GridComments.xGrid.FieldLayouts[0].Fields[1].Visibility = Visibility.Visible;
        }

        private void GridComments_SelItemsChanged(object sender, RoutedEventArgs e)
        {
            doSingleClick();
        }

     }
}
