
using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using Infragistics.Windows.Controls;



namespace MiscFolders
{
    /// <summary>
    /// This class represents a 'ucTab1' object.
    /// </summary>
    public partial class WHTAttachmentTab : ScreenBase
    {
        /// <summary>
        /// Create a new instance of a 'ucTab1' object and call the ScreenBase's constructor.
        /// </summary>
        public WHTAttachmentTab()
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
            //Create the WHT Tracking object
            //CurrentBusObj = new cBaseBusObject("WHTFolder");
            //CurrentBusObj.Parms.ClearParms();
            MainTableName = "search";
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            GridAttachments.xGrid.FieldLayoutSettings = f;
            GridAttachments.MainTableName = "search";
            GridAttachments.ConfigFileName = "AdjustmentSearch";
            GridAttachments.FieldLayoutResourceString = "AdjustmentSearch";
            GridAttachments.xGrid.FieldSettings.AllowEdit = false;
            //add delegate to doubleclick and transfer control to customer document folder
            GridAttachments.WindowZoomDelegate = GridDoubleClickDelegate;
            // gWHTTracking.IsFilterable = true;
            GridAttachments.SetGridSelectionBehavior(false, true);
            //GridAttachments.SetGridSelectionBehavior(false, false);          

            GridCollection.Add(GridAttachments);
        
        }

        /// <summary>
        /// Delegate for grid 
        /// </summary>
        private void GridDoubleClickDelegate()
        {
            try
            {
                GridAttachments.ReturnSelectedData("prod_filename");
                //pass attachment data table to attachment helper class
                AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                string PathFile = attachHelp.getAttachmentFileLocationId(this.CurrentBusObj.ObjectData.Tables["search"], @cGlobals.ReturnParms[0].ToString());
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
        
    }

}
