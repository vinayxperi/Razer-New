

#region using statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RazerBase;
using RazerBase.Interfaces;
using RazerBase.Lookups;
using Infragistics.Windows.DockManager;

#endregion

namespace Location
{

    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class LocationFolder : ScreenBase, IScreen
    {

        #region Private Variables
        private string LocationID { get; set; }
        private string LocationName { get; set; }

        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }
        //private static readonly string attachmentParameter = "@external_char_id";
        //private static readonly string attachmentLocID = "@location_id";
        //private static readonly string attachmentType = "@attachment_type";
        //private static readonly string attachmentExtIntParameter = "@external_int_id";
        //private static readonly string commentAttachmentsIdParameter = "@comment_attachments_id";
        //private static readonly string commentTypeParameter = "@comment_type";
        #endregion

        #region Constructor Stuff

        public LocationFolder()
            : base()
        {
            // Create Controls
            InitializeComponent();
 
        }
       
        public void Init(cBaseBusObject businessObject)
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "Location";
            // Change this if your folder has item directly bound to it.
            // In many cases the Tabs have data binding, the Folders
            // do not.
            this.DoNotSetDataContext = false;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            // add the Tabs
            TabCollection.Add(LocationGeneralTab);
            TabCollection.Add(LocationHeadendTab);
            TabCollection.Add(LocationUnitsTab);
            TabCollection.Add(LocationContractsTab);
            TabCollection.Add(LocationBillingTotalTab);
            TabCollection.Add(LocationCommentsTab);
            TabCollection.Add(LocationAttachmentsTab);

            LocationAttachmentsTab.TabIsEnabled = true;

            // if there are parameters than we need to load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                //LocationID = Convert.ToInt32(this.CurrentBusObj.Parms.ParmList.Rows[0][1]);
                LocationID = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();
                txtLocationID.Text = LocationID.ToString();
                //loadParms
                this.loadParms(LocationID.ToString());
                // load the data
                this.Load();
                // Set the Header
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count >= 0)
                {
                    windowCaption = "Location ID -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[0].ToString();
                    //txtDocumentType.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[3].ToString();
                    txtLocationName.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[3].ToString();
                    //addAttachmentParms(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    Messages.ShowInformation("Invalid Location ID Entered");
                }
                ////SetHeaderName();
            }
            else
                this.loadParms("");

        }

        #endregion

        #region Load Data Related
        /// <summary>
        /// Load Location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Location_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on Location ID field
            LocationLookup f = new LocationLookup();

            this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {

                //load current parms
                loadParms("");
                txtLocationID.Text = cGlobals.ReturnParms[0].ToString();
                //clear comments/attachments grid if applicable bus obj
                clrCommentsAttachmentsObj();
                clrUnits();
                // Call load 
                this.Load();
                windowCaption = "Location -" + txtLocationID.Text;
                LocationID =  txtLocationID.Text;
                SetHeaderName();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
                General.Focus();
            }
        }

        private void loadParms(string LocationID)
        {
            try
            {
                if (LocationID != null && LocationID != "")
                {
                    this.CurrentBusObj.Parms.ClearParms();
                    this.CurrentBusObj.Parms.AddParm("@cs_id", LocationID);
                    this.CurrentBusObj.Parms.AddParm("@external_char_id", LocationID.ToString());

                }
                else
                {
                    //if EntityId NOT passed load   with global parm EntityId if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.ClearParms();
                        this.CurrentBusObj.Parms.AddParm("@cs_id", cGlobals.ReturnParms[0]);
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", cGlobals.ReturnParms[0].ToString());
                    }
                    else
                    {
                        //doing an insert setup dummy vals
                        this.CurrentBusObj.Parms.AddParm("@cs_id", -1);
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", "-1");

                    }
                }
                this.CurrentBusObj.Parms.AddParm("@service_period_start", "01/01/1900");
                this.CurrentBusObj.Parms.AddParm("@service_period_end", "01/01/1900");
                this.CurrentBusObj.Parms.AddParm("@comment_type", "LA");
                this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-1");
                //attachment tab parms
                this.CurrentBusObj.Parms.AddParm("@attachment_type", "LATTACH");
                this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");
                this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
                //////////////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }

        private void ReturnData(string SearchValue, string DbParm)
        {
            //if no value do nothing
            if (string.IsNullOrEmpty(SearchValue))
                return;
            //Add new parameters
            int iDummy = 0;
            if (!(Int32.TryParse(txtLocationID.Text, out iDummy)))
            {
                Messages.ShowInformation("Invalid Location ID Entered");
                return;
            }

            this.CurrentBusObj.Parms.ClearParms();
            //Add new parameters
            //this.CurrentBusObj.Parms.AddParm(DbParm, SearchValue);
            this.loadParms(SearchValue);
            //clear comments/attachments grid if applicable bus obj
            clrCommentsAttachmentsObj();
            clrUnits();
            //load data
            //if coming from save do not do this...
            this.Load();
            //if customer number found then set header and pop otherwise send message
            if (chkForData())
            {
                SetHeaderName();

                //addAttachmentParms(LocationID);
            }
            General.Focus();
        }

        private void clrUnits()
        {
            this.LocationUnitsTab.txtUnitTotal.Text = "0";
            this.LocationUnitsTab.txtServiceDateStart.SelText = Convert.ToDateTime("01/01/1900");
            this.LocationUnitsTab.txtServiceDateEnd.SelText = Convert.ToDateTime("01/01/1900");
        }

        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                return true;
            }
            else
            {
                Messages.ShowWarning("Location Not Found");
                return false;
            }
        }

        private void SetHeaderName()
        {
            //Sets the header name when being called from another folder
            if (txtLocationID.Text == null)
            {
                windowCaption = "Location -" + this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[3].ToString();
                txtLocationID.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[0].ToString();
                txtLocationName.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[3].ToString();
                //LocationID = Convert.ToInt32(txtLocationID.Text);
            }
            //Sets the header name from within same folder
            else
            {
                ContentPane p = (ContentPane)this.Parent;
                p.Header = "Location -" + txtLocationID.Text;
                txtLocationName.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[3].ToString();
            }
        }

        private void txtLocationID_GotFocus(object sender, RoutedEventArgs e)
        {

            LocationID = txtLocationID.Text;

        }

        private void txtEntityName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLocationID.Text != LocationID)
                ReturnData(txtLocationID.Text, "@cs_id");
        }

        private void txtLocationID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLocationID.Text != LocationID)
            {
                LocationUnitsTab.txtServiceDateEnd.SelText = Convert.ToDateTime("01/01/1900");
                LocationUnitsTab.txtServiceDateStart.SelText = Convert.ToDateTime("01/01/1900");
                ReturnData(txtLocationID.Text, "@cs_id");
            }
        }

        private void clrCommentsAttachmentsObj()
        {
            if (this.CurrentBusObj != null)
                if (this.CurrentBusObj.ObjectData != null)
                    this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Clear();
        }

        #endregion

        #region Save Related

        public override void Save()
        {
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

        #endregion

        #region Hold For Now, May Delete if Still Commented in Prod
        //private void addAttachmentParms(string LocationID)
        //{
        //    //comment tab parms
        //    this.CurrentBusObj.Parms.AddParm(attachmentParameter, LocationID);
        //    this.CurrentBusObj.Parms.AddParm(commentTypeParameter, "LS");
        //    this.CurrentBusObj.Parms.AddParm(commentAttachmentsIdParameter, "-1");
        //    //attachment tab parms
        //    this.CurrentBusObj.Parms.AddParm(attachmentType, "LATTACH");
        //    this.CurrentBusObj.Parms.AddParm(attachmentExtIntParameter, "0");
        //    this.CurrentBusObj.Parms.AddParm(attachmentLocID, "-1");
        //}
        #endregion


    }
}
