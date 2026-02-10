
//Dependency : user will need access to the appropriate network locations for the
//             file copy command to work.  See AttachmentHelper class
using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Collections.Specialized;
using System.Text;

namespace RazerBase
{

    /// <summary>
    /// This class represents a 'ucAttachmentsTab' object.
    /// </summary>
    public partial class ucAttachmentsTab : ScreenBase
    {
        private bool tabIsEnabled = true;
        public bool TabIsEnabled
        {
            get
            {
                return tabIsEnabled;
            }
            set
            {
                tabIsEnabled = value;
                this.IsEnabled = tabIsEnabled;            
                GridAttachments.IsEnabled = tabIsEnabled;
            }
        }

        
        public string mainTableName { get; set; }
        public string fieldLayoutResource { get; set; }
        //public cBaseBusObject CurrentBusObj { get; set; }
        public List<string> AttachmentCopyList = new List<string>();

        private static readonly string fieldLayoutResourceDefault = "AttachmentGrid";
        private static readonly string mainTableNameDefault = "Attachments";

        //Used to collapse the color key - defaults to true
        private bool mCollapseColorCode = true;
        public bool CollapseColorCode
        {
            get { return mCollapseColorCode; }
            set
            {
                mCollapseColorCode = value;
                if (mCollapseColorCode)
                    AttachColorKey.Visibility = Visibility.Collapsed;
                else
                    AttachColorKey.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Create a new instance of a 'ucAttachmentsTab' object and call the ScreenBase's constructor.
        /// </summary>
        public ucAttachmentsTab()
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
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
             //set 2X click delegate
            GridAttachments.WindowZoomDelegate = GridDoubleClickDelegate;
            //set add context menu delegate
            GridAttachments.ContextMenuAddDelegate = ContextMenuAddDelegate;
            GridAttachments.ContextMenuAddDisplayName = "Add New Attachment";
            //Don't think you are supposed to be able to delete an attachment
           // GridAttachments.ContextMenuRemoveDisplayName = "Remove Attachment";
            //GridAttachments.ContextMenuGenericDelegate1 = CopyAttachmentDelegate;
            //GridAttachments.ContextMenuGenericDisplayName1 = "Copy Attachment(s)";
            //GridAttachments.ContextMenuGenericIsVisible1 = true;
            //GridAttachments.ContextMenuGenericImageSwap1 = Icons.Move;
            GridAttachments.SetGridSelectionBehavior(false, true);
            //Add new attachment to grid
            GridAttachments.xGrid.FieldSettings.AllowEdit = true;
            //set fieldLayoutResource if not set by caller
            if (fieldLayoutResource == "" || fieldLayoutResource == null) fieldLayoutResource = fieldLayoutResourceDefault;
            //match grid to fieldlayouts using constant value
            GridAttachments.FieldLayoutResourceString = fieldLayoutResource;


            //Add this code to use a different style for the grid than the default.
            //This particular style is used for color coded grids.
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            GridAttachments.GridCellValuePresenterStyle = CellStyle;
            GridAttachments.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            

            LoadGrid(); 
        }

        public void LoadGrid(string Id = null)
        {
            //set main table name if not set by caller
            if (mainTableName == "" || mainTableName == null) mainTableName = mainTableNameDefault;
            //set screenbase type as Tab unless specified otherwise by caller
            if (mainTableName == "Attachments") this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //set mainTableName using constant
            MainTableName = mainTableName;
            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            GridAttachments.MainTableName = mainTableName;
 
            if (mainTableName == "Attachments")
            {
                //is a tab
                GridCollection.Add(GridAttachments);
            }
            else
            {
                if (this.CurrentBusObj != null)
                {
                    //is as singleton
                    GridAttachments.LoadGrid(this.CurrentBusObj, GridAttachments.MainTableName);
                }
            }
        }

        /// <summary>
        /// Delegate for grid 
        /// </summary>
        private void GridDoubleClickDelegate()
        {
            try
            {
                //RES 6/25/13 reload attachment path info in case move attachments has run since file was attached
                //this.CurrentBusObj.LoadTable("attachments");
                //RES 1/4/16 pass prod filename instead of original filename in case there are dups
                //GridAttachments.ReturnSelectedData("filename");
                GridAttachments.ReturnSelectedData("prod_filename");
                //pass attachment data table to attachment helper class
                AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                string PathFile = attachHelp.getAttachmentFileLocationId(this.CurrentBusObj.ObjectData.Tables[this.mainTableName], @cGlobals.ReturnParms[0].ToString());
                //string Path = @"\\tul1img1\image\\laser\attachments\rec_account\";
                //open up file
                System.Diagnostics.Process.Start(PathFile);
            }
            catch (Exception ex)
            {
                //if (ex.Message == "The system cannot find the file specified")
                //    Messages.ShowInformation(ex.Message + ".  Refresh the screen and try to open the attachment again. If it still won't open contact support." );
                //else
                    Messages.ShowInformation(ex.Message);
            }
            cGlobals.ReturnParms.Clear();
        }

        private void ContextMenuAddDelegate()
        {
            //GridAttachments.xGrid.FieldLayoutSettings.AllowAddNew = false;
            DataView dataSource = this.GridAttachments.xGrid.DataSource as DataView;

            if (dataSource != null)
            {
                //Open a dialog to select a file
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = ""; // Default file name
                dlg.DefaultExt = ".txt"; // Default file extension
                dlg.Filter = "All Files (*.*)|*.*"; // Filter files by extension
                dlg.Multiselect = true;

              
                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    List<string> FileNames = new List<string>();

                     //string filename;
                     //Int32 badchar = 0;
                     //Int32 slash = -1;
                     foreach(string f in dlg.FileNames)
                    {
                         //filename = f;
                         //slash = filename.IndexOf('\\');
                         //while (slash >= 0)
                         //{
                         //    filename = filename.Substring(slash + 1);
                         //    slash = filename.IndexOf('\\');
                         //}

                         //if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                         //{
                         //    badchar = f.IndexOfAny(Path.GetInvalidFileNameChars());
                         //    filename = f.Substring(f.IndexOfAny(Path.GetInvalidFileNameChars()));
                         //    Messages.ShowError("Filename " + filename + " contains invalid filename characters at position " + badchar.ToString());
                         //    //Messages.ShowError("Filename " + f + " contains invalid filename characters!  Check for special characters like foreign language.");
                         //}
                         //else
                            FileNames.Add(f);
                    }

                     AddAttachments(FileNames);
                }
            }
            else
            {
                return;
            }

        }

        /// <summary>
        /// Adds new attachments either from right click / add or drag and drop from explorer or desktop
        /// </summary>
        /// <param name="fileNames"></param>
        private void AddAttachments(List<string> fileNames)
        {
            List<string> LengthErrors = new List<string>();
            List<string> DupErrors = new List<string>();

            foreach (string f in fileNames)
            {
                //Check the size of the file
                //If greater than allowed then write to error list and continue loop
                if (Path.GetFileName(f).Length > 255)
                {
                    LengthErrors.Add(f);
                    continue;
                }
                //Check for duplicate file
                //If exists then write to error list and continue loop
                //currently does not work for comments grid
                if (MainTableName.ToLower() == "attachments")
                {
                    if (DuplicateFilesExist(Path.GetFileName(f)))
                    {
                        DupErrors.Add(f);
                        continue;
                    }
                }

                //Add the row to the datatable
                DataRow dr = this.CurrentBusObj.ObjectData.Tables[GridAttachments.MainTableName].NewRow();
                //add attachment_id
                dr["attachment_id"] = 0;
                //type
                dr["attachment_type"] = "RATTACH";
                //subject
                dr["location_id"] = 0;
                //date
                dr["attachment_date"] = DateTime.Now.ToString("MM/dd/yyyy");
                dr["subject"] = "";
                //filename
                dr["filename"] = Path.GetFileName(f);
                //user name
                dr["user_id"] = UserName.GetUserName;
                //add ext int id
                dr["external_int_id"] = "0";
                //add cust #/ext char id
                dr["external_char_id"] = getCustNumFromParms();
                //add prod filename
                dr["prod_filename"] = "";
                //add comment id
                if (mainTableName.ToLower() == "attachments")
                    dr["comment_id"] = 0;
                else
                    dr["comment_id"] = getCommentIdFromParms();
                //this.CurrentBusObj.ObjectData.Tables[GridAttachments.MainTableName].Rows.Add(dr);
                this.CurrentBusObj.ObjectData.Tables[GridAttachments.MainTableName].Rows.InsertAt(dr, 0);

            
                AttachmentCopyList.Add(f);
            }

            string eMessage = "";
            //Loop through file size errors and send window message if any exist
            foreach (string f in LengthErrors)
            {
                eMessage = eMessage + f + "\n";
            }

            if (eMessage != "")
            {
                MessageBox.Show("File name must be 255 characters or less : " + "\n" + eMessage);
            }

            eMessage = "";
            //Loop through duplicate error messages and send to window if any exist
            foreach (string f in DupErrors)
            {
                eMessage = eMessage + f + "\n";
            }

            if (eMessage != "")
            {
                MessageBox.Show("File name is a duplicate name and will not be uploaded : " + "\n" + eMessage);
            }

            if (fileNames.Count > (DupErrors.Count + LengthErrors.Count))
            {
                if (mainTableName.ToLower() == "attachments")
                    SaveAttachments("attachments");
                else
                    SaveAttachments("comment_attachment");
                
            }

        }

        /// <summary>
        /// Copies a range of attachments dropped from another window
        /// </summary>
        /// <param name="SenderGrid">ucBaseGrid</param>
        public void CopyAttachments(ucBaseGrid SenderGrid)
        {
            //If the passed grid has multiple items selected cycle through each item coprying it
            
            if (SenderGrid.xGrid.SelectedItems.Records.Count > 0)
            {
                foreach (DataRecord SenderRow in SenderGrid.xGrid.SelectedItems  )
                {
                    CopyRecord(SenderRow);
                }
            }
                //Otherwise grab the active record if one exists
            else
            {
                if(SenderGrid.xGrid.ActiveRecord!= null)
                {
                    CopyRecord(SenderGrid.xGrid.ActiveRecord as DataRecord);
                }
 

            }
            //Save the attachments grid
            //Need to make sure at least one row was added
                    if (mainTableName.ToLower() == "attachments")
                        SaveAttachments("attachments");
                    else
                        SaveAttachments("comment_attachment");
                
        }

        /// <summary>
        /// Copies one file to new document attachment
        /// This code currently does not prevent duplicate file names under one document
        /// </summary>
        /// <param name="SenderRow"></param>
        private void CopyRecord(DataRecord SenderRow)
        {
            //Add the row to the datatable
            DataRow dr = this.CurrentBusObj.ObjectData.Tables[GridAttachments.MainTableName].NewRow();
            //add attachment_id
            dr["attachment_id"] = 0;
            //type
            dr["attachment_type"] = "RATTACH";
            //subject
            dr["location_id"] = SenderRow.Cells["location_id"].Value.ToString();
            //date
            dr["attachment_date"] = SenderRow.Cells["attachment_date"].Value.ToString();
            dr["subject"] = SenderRow.Cells["subject"].Value.ToString();
            //filename
            dr["filename"] = SenderRow.Cells["filename"].Value.ToString();
            //user name
            dr["user_id"] = UserName.GetUserName;
            //add ext int id
            dr["external_int_id"] = "0";
            //add cust #/ext char id
            dr["external_char_id"] = getCustNumFromParms();
            //add prod filename
            dr["prod_filename"] = SenderRow.Cells["prod_filename"].Value.ToString();
            //add comment id
            if (mainTableName.ToLower() == "attachments")
                dr["comment_id"] = 0;
            else
                dr["comment_id"] = getCommentIdFromParms();
            //this.CurrentBusObj.ObjectData.Tables[GridAttachments.MainTableName].Rows.Add(dr);
            this.CurrentBusObj.ObjectData.Tables[GridAttachments.MainTableName].Rows.InsertAt(dr, 0);
        }
 
        /// <summary>
        /// used to find customerId in objectData
        /// </summary>
        /// <returns></returns>
        private string getCustNumFromParms()
        {
            try
            {
                var newCustomerNumParm = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                         where x.Field<string>("parmName") == "@receivable_account"
                                         select new
                                         {
                                             parmName = x.Field<string>("parmName"),
                                             parmValue = x.Field<string>("parmValue")
                                         };

                foreach (var info in newCustomerNumParm)
                {
                    if (info.parmName == "@receivable_account")
                        return info.parmValue;
                }
                return "";
            }
            catch (Exception ex)
            {
                Messages.ShowError("Cannot find customer number. Error : " + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// used to find comment_id in objectData
        /// </summary>
        /// <returns></returns>
        private string getCommentIdFromParms()
        {
            try
            {
                var newCommentIDParm = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                       where x.Field<string>("parmName") == "@comment_attachments_id"
                                         select new
                                         {
                                             parmName = x.Field<string>("parmName"),
                                             parmValue = x.Field<string>("parmValue")
                                         };

                foreach (var info in newCommentIDParm)
                {
                    if (info.parmName == "@comment_attachments_id")
                        return info.parmValue;
                }
                return "";
            }
            catch (Exception ex)
            {
                Messages.ShowError("Cannot find comment_id. Error : " + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// check for duplicate attachment files
        /// </summary>
        /// <param name="possibleDupeFileName"></param>
        /// <returns></returns>
        private bool DuplicateFilesExist(string possibleDupeFileName)
        {
            //if selected file already exists return true
            //search grid for existing file
            for (int i = 0; i < GridAttachments.xGrid.Records.Count; i++)
            {
                DataRecord dr = GridAttachments.xGrid.Records[i] as DataRecord;
                string cellvalue = dr.Cells["filename"].Value.ToString();
                if (cellvalue.ToLower() == possibleDupeFileName.ToLower())
                {
                    //duplicate file found
                    return true;
                }
            }
            //if no dupes found return false
            return false;
        }

        /// <summary>
        /// This method allows files to be dropped into the attachment grid 
        /// from other attachment windows or from desktop or windows explorer files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenBase_Drop(object sender, DragEventArgs e)
        {
            //If no insert security then don't allow drop
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
                return;

           //Load the dragged object into the data object
            DataObject dataObject = e.Data as DataObject;

            //Check to see if it is a UC base grid
            if (dataObject.GetData(typeof(ucBaseGrid)) != null)
            {
                ucBaseGrid DropGrid = dataObject.GetData(typeof(ucBaseGrid)) as ucBaseGrid ;
                //If the grid is being dropped on iteself then do nothing
                if (DropGrid == GridAttachments)
                    return;
                //Copy selected rows
                else
                {
                    CopyAttachments(DropGrid);
 
                }
            }

            List<string> FileNames = new List<string>(); 

            if (dataObject.ContainsFileDropList())
            {
                StringCollection fileNames = dataObject.GetFileDropList();
                StringBuilder bd = new StringBuilder();
                foreach (var f in fileNames)
                {
                    FileNames.Add(f);
                }

                AddAttachments(FileNames);
            }
        }

        /// <summary>
        /// Save applicable attachments
        /// </summary>
        /// <param name="AttachTableName"></param>
        private void SaveAttachments(string AttachTableName)
        {
            DataSet dsAttach = new DataSet("dsAttach");
            dsAttach.Tables.Add(this.CurrentBusObj.ObjectData.Tables[AttachTableName].Copy());
            //dsAttach.Tables.Add("table1");
            //dsAttach = this.CurrentBusObj.ObjectData.Tables[AttachTableName].Copy();
            dsAttach.Tables[0].TableName = "copyattachments";
            //DataTable table = new DataTable("attachsave");
            //dsAttach.Tables.Add(table);
            //DataTable dtAttachSave = new DataTable();
            //DataTable dtAttachSave = this.CurrentBusObj.ObjectData.Tables[AttachTableName].Clone();
            //dtAttachSave = this.CurrentBusObj.ObjectData.Tables[AttachTableName];
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables[AttachTableName].Rows)
            {
                string sOut = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(dr["filename"].ToString()));
                if (dr["filename"].ToString() != sOut)
                {       
                    //MessageBox.Show("Error saving attachments - filename " + dr["filename"].ToString() + " contains invalid characters.  Attachment save Failed!");
                    MessageBox.Show("Error saving attachments - filename '" + sOut.Trim() + "' contains invalid characters identified by question marks.  Attachment save Failed!");
                    this.CurrentBusObj.LoadTable(AttachTableName);
                    return;
                }
                //dsAttach.Tables["attachsave"].ImportRow(dr);
                //dsAttach.Tables["attachsave"].AcceptChanges();
                //dtAttachSave.ImportRow(dr);
            }
            //dsAttach.Tables["attachsave"] = this.CurrentBusObj.ObjectData.Tables[AttachTableName];
            //DataTable dtAttach = this.CurrentBusObj.ObjectData.Tables[AttachTableName];
            if (!CurrentBusObj.SaveTable(AttachTableName))
            {
                MessageBox.Show("Error saving attachments - ");
                return;
            }
            else //Save documents
            {
                //check if attachment tab files need to be copied
                if (AttachmentCopyList.Count > 0)
                {
                    //if so pass attachment data table to attachment helper class
                    AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                    //attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables[AttachTableName], AttachmentCopyList);
                    //dsAttach.Tables[0].TableName = "copyattachments";
                    //for (int k = 0; k < dsAttach.Tables[0].Rows.Count; k++)
                    //{
                    ////    if (Convert.ToInt32(dsAttach.Tables[0].Rows[k]["attachment_id"].ToString()) == 0)
                    //this.CurrentBusObj.ObjectData.Tables[AttachTableName].Rows[k]["filename"] = dsAttach.Tables[0].Rows[k]["filename"];
                    ////        this.CurrentBusObj.ObjectData.Tables[AttachTableName].Rows[dsAttach.Tables[0].Rows.Count - 1 - k]["filename"] = dsAttach.Tables[0].Rows[k]["filename"];
                    //}
                    attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables[AttachTableName], AttachmentCopyList);
                    //attachHelp.doAttachments(dsAttach.Tables[0], AttachmentCopyList);
                    AttachmentCopyList = new List<string>();
                    //if (!CurrentBusObj.SaveTable(AttachTableName))
                    //{
                    //    MessageBox.Show("Error deleting failed attachment - ");
                    //    return;
                    //}
                    //GridAttachments.LoadGrid(this.CurrentBusObj, "attachments");
                }
            }
            //MessageBox.Show("Attachments Saved ");

        }
    }
}
