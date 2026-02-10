



using RazerBase;
using RazerBase.Interfaces;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows.Controls;
using Infragistics.Windows.Controls;


namespace MiscFolders
{


    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class WHTFolder : ScreenBase, IScreen
    {
        public string WindowCaption { get { return string.Empty; } }
        private static readonly string whtStatusParameterName = "@code_name";
        private static readonly string whtStatusParameterValue = "WHTStatus";

        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBase constructor is also called.
        /// </summary>
        public WHTFolder()
            : base()
        {
            // Create Controls
            InitializeComponent();

            // performs initializations for this object.
            //Init();
        }

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            // Change this if your folder has item directly bound to it.
            // In many cases the Tabs have data binding, the Folders
            // do not.
            this.DoNotSetDataContext = false;

            //Set the maintablename for the folder if it has one
            this.MainTableName = "wht_tracking";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;

            // add the Tab user controls that are of type screen base
            TabCollection.Add(Tracking);
            TabCollection.Add(QtrlyCalc);
            TabCollection.Add(Attachments);
            TabCollection.Add(WHTAttachments);
                
            //Load Parms on business object to be used in Prebind in the WHTTracking Tab to load the combobox
            this.CurrentBusObj.Parms.AddParm(whtStatusParameterName, whtStatusParameterValue);

            //load parms
            loadParms();
            // load the data
            this.Load();
                   
            // Set the Header
            HeaderName = "Withholding Tax Folder";
            btnRetrieve.Visibility = System.Windows.Visibility.Visible;
            txtFilename.Visibility = System.Windows.Visibility.Visible;

        }

        private void loadParms()
        {
            //attachment tab parms
            this.CurrentBusObj.Parms.AddParm("@document_id", -1);
            this.CurrentBusObj.Parms.AddParm("@external_char_id", "WHTFolder");
            this.CurrentBusObj.Parms.AddParm("@attachment_type", "HATTACH");
            this.CurrentBusObj.Parms.AddParm("@external_int_id", 0);
            this.CurrentBusObj.Parms.AddParm("@location_id", -1);
            // 2/6/20 RES add parm to do a search on attachments filename
            this.CurrentBusObj.Parms.AddParm("@filenamesearch", "%xxxxxxxxxxx%");
            //this.CurrentBusObj.Parms.AddParm("@firsttime", "Y");
            this.CurrentBusObj.Parms.AddParm("@invoice", "");
            //Wht tracking parms
            this.CurrentBusObj.Parms.AddParm("@date_start", "01/01/1900");
            this.CurrentBusObj.Parms.AddParm("@date_end", "01/01/1900");
            if (this.CurrentBusObj.ObjectData != null)
            {
                this.CurrentBusObj.changeParm("@date_start", "1/1/1900");
                this.CurrentBusObj.changeParm("@date_end", "1/1/1900");
                Tracking.ClearDates();
            }
        }

        private void Create_Entry(object sender, System.Windows.RoutedEventArgs e)
        {
            // this.Load();
        }

         /// Load bus obj parms, used in multiple places
        /// </summary>
       
          

        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {
                ////check if attachment tab files need to be copied
                //if (cGlobals.GlobalAttachmentsStorageList.Count > 0)
                //{
                //    //if so pass attachment data table to attachment helper class
                //    AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                //    attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables["Attachments"]);

                //}
                //check if attachment files need to be copied
                //if (cGlobals.GlobalTempStorageList.Count > 0)
                //{
                //    //if so pass attachment data table to attachment helper class
                //    AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                //    attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables["attachments"]);
                //}                             
               
                Messages.ShowInformation("Save Successful");
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }

        private void btnRetrieve_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.CurrentBusObj == null)
                return;
            
            //If no start input then select all previous
            if (txtFilename.Text == "" || txtFilename.Text == null || txtFilename.Text == " ")
            {
                this.CurrentBusObj.changeParm("@filenamesearch", "%%");
                txtFilename.Text = "";
            }
            else
            {
                this.CurrentBusObj.changeParm("@filenamesearch", "%" + txtFilename.Text + "%");
            }

            this.CurrentBusObj.LoadData("search");
            if (CurrentBusObj.ObjectData.Tables["search"].Rows.Count == 0)
            {
                Messages.ShowWarning("No attachments found");
            }                
 
        }        

        //private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    txtFilename.Text = string.Empty;           
        //}

        private void tcMainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object source = e.Source;
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0].GetType() == typeof(TabItemEx) && ((TabItemEx)e.AddedItems[0]).Header.Equals("Search Attachments"))
                {
                    btnRetrieve.Visibility = System.Windows.Visibility.Visible;
                    txtFilename.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    btnRetrieve.Visibility = System.Windows.Visibility.Collapsed;
                    txtFilename.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
        
    }
}
