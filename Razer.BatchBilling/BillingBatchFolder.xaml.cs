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

namespace Razer.BatchBilling
{
    /// <summary>
    /// Interaction logic for BillingBatchFolder.xaml
    /// </summary>
    public partial class BillingBatchFolder : ScreenBase, IScreen
    {        
        private int BatchID { get; set; }
        private string BatchName { get; set; }

        private string windowCaption;
        public int exceptionFlag = 0;
        public string WindowCaption
        {
            get { return windowCaption; }
        }
        public BillingBatchFolder()
            : base()
        {
            InitializeComponent();
        }

       public void Init(cBaseBusObject businessObject)
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "BillingBatchFolder";
            // Change this if your folder has item directly bound to it.
            // In many cases the Tabs have data binding, the Folders
            // do not.
            this.DoNotSetDataContext = false;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            // add the Tabs
            TabCollection.Add(BillingBatchSummaryTab);
            TabCollection.Add(BillingBatchDetailTab);
            TabCollection.Add(BillingBatchExceptionTab);
            TabCollection.Add(BillingBatchRptingTab);
           
            // if there are parameters than we need to load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                BatchID = Convert.ToInt32(this.CurrentBusObj.Parms.ParmList.Rows[0][1]);

                txtBatchId.Text = BatchID.ToString();
                //loadParms
                this.loadParms(BatchID.ToString());
                // load the data
                this.Load();
                // Set the Header
                windowCaption = "Batch ID -" + this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0].ItemArray[0].ToString();
                //txtDocumentType.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[3].ToString();
                //txtBatchName.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0].ItemArray[1].ToString();
                txtBatchName.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0]["batch_name"].ToString();
                ////SetHeaderName();["created_by"]
            }
            else
                this.loadParms("");

        }

        private void loadParms(string BatchID)
        {
            try
            {
                if (BatchID != "")
                {
                    this.CurrentBusObj.Parms.ClearParms();
                    this.CurrentBusObj.Parms.AddParm("@batch_id", BatchID);

                }
                else
                {
                    //if BatchId NOT passed load   with global parm BatchId if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.ClearParms();
                        this.CurrentBusObj.Parms.AddParm("@batch_id", cGlobals.ReturnParms[0]);
                      

                    }
                    else
                    {
                        //doing an insert setup dummy vals
                        this.CurrentBusObj.Parms.AddParm("@batch_id", -1);
                    }
                }
                
                //////////////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }

   

     

        //Load Batch
        private void Batch_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on Location ID field
            UnpostedBillingBatchLookup f = new UnpostedBillingBatchLookup();

            this.CurrentBusObj.Parms.ClearParms();

            //// gets the users response
            f.ShowDialog();

            //RoutedEventArgs args = new RoutedEventArgs();
            //args.RoutedEvent = EventAggregator.GeneratedClickEvent;
            //args.Source = txtBatchId;
            //EventAggregator.GeneratedClickHandler(this, args);

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {

                //load current parms
                 loadParms(cGlobals.ReturnParms[0].ToString());
                txtBatchId.Text = cGlobals.ReturnParms[0].ToString();
                txtBatchName.Text = cGlobals.ReturnParms[1].ToString();
                // Call load 
                this.Load();
                if (this.CurrentBusObj.ObjectData.Tables["summary"].Rows.Count > 0)
                {
                    //set audit fields
                    txtCreatedBy.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0]["created_by"].ToString();
                    txtCreatedOn.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0]["created_on"].ToString();
                    txtBatchName.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0]["batch_name"].ToString();

                }
                //ADDED THIS CBIRNEY 12.20.11
                this.chkForData();
                
                ContentPane p = (ContentPane)this.Parent;
                p.Header = "Batch -" + txtBatchId.Text;

                //BatchID = Convert.ToInt32(txtBatchId.Text);
                //SetHeaderName();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
                Summary.Focus();

            }

        }

        private void txtBatchID_GotFocus(object sender, RoutedEventArgs e)
        {
            string sBatchID = txtBatchId.Text;

            if (!string.IsNullOrEmpty(sBatchID))
            {
                int iBatchID = 0;
                if (int.TryParse(sBatchID, out iBatchID))
                {
                    BatchID = iBatchID;
                }
            }
        }

        private void txtBatchName_LostFocus(object sender, RoutedEventArgs e)
        {
            LoadBatchByBatchID();
        }

        private void txtBatchName_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtBatchID_LostFocus(object sender, RoutedEventArgs e)
        {
            LoadBatchByBatchID();
        }

        private void LoadBatchByBatchID()
        {
            string sBatchID = txtBatchId.Text;

            if (!string.IsNullOrEmpty(sBatchID))
            {
                int iBatchID = 0;
                if (int.TryParse(sBatchID, out iBatchID))
                {
                    if (iBatchID != BatchID)
                    {
                        ReturnData(sBatchID, "@batch_id");
                    }
                }
                else
                {
                    Messages.ShowInformation("Batch ID must be a number.");
                }
            }
        }

        private void ReturnData(string SearchValue, string DbParm)
        {
            //if no value do nothing
            if (SearchValue == "") return;
            //Add new parameters
            this.CurrentBusObj.Parms.ClearParms();
            //Add new parameters
            //this.CurrentBusObj.Parms.AddParm(DbParm, SearchValue);
            this.loadParms(SearchValue);
            //load data
            //if coming from save do not do this...
            this.Load();
           



            //if customer number found then set header and pop otherwise send message
            if (chkForData()) SetHeaderName();
        }

        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["summary"].Rows.Count != 0)
            {
                exceptionFlag = 0;
                txtCreatedBy.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0]["created_by"].ToString();
                txtCreatedOn.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0]["created_on"].ToString();
                return true;
            }
            else
            {
                if (this.CurrentBusObj.ObjectData.Tables["exceptions"].Rows.Count != 0)
                {
                    //need to check so when you click on the exception tab, it does not show the warning again on that tab
                    if (exceptionFlag == 1)
                        return true;
                    else
                    {
                        Messages.ShowWarning("Batch has exceptions. Click exception tab to view.");
                        exceptionFlag = 1;
                        return true;
                    }
                }
                else
                {
                   
                        Messages.ShowWarning("Batch Not Found");
                        return false;
                    
                }
            }
        }

        private void SetHeaderName()
        {
            //I ended up moving the double click from lookup logic to the event handler that called it

            //Sets the header name when being called from another folder
            if (txtBatchId.Text == null)
            {

                windowCaption = "Batch -" + this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0].ItemArray[12].ToString();
                txtBatchId.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0].ItemArray[0].ToString();
                //txtBatchName.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0].ItemArray[1].ToString();
                txtBatchName.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0]["batch_name"].ToString();
                txtCreatedBy.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0]["created_by"].ToString();
                txtCreatedOn.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0]["created_on"].ToString();
                //BatchID = Convert.ToInt32(txtBatchId.Text);

            }
            else  //Sets the header name from within same folder
            {
                if (exceptionFlag == 1)
                {
                    ContentPane p = (ContentPane)this.Parent;
                    p.Header = "Batch -" + txtBatchId.Text;
                    txtBatchName.Text = "Batch in Error. See Exceptions.";

                }
                else
                {
                    ContentPane p = (ContentPane)this.Parent;
                    p.Header = "Batch -" + txtBatchId.Text;
                    //txtBatchName.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0].ItemArray[1].ToString();
                    txtBatchName.Text = this.CurrentBusObj.ObjectData.Tables["summary"].Rows[0]["batch_name"].ToString();
                }
            }
        }

        /// <summary>
        /// DWR Added 5/6/13 - Phase 3.0 - Allow users to save billing exception resolved flag and comments
        /// </summary>
        public override void Save()
        {

            //If verified or deleting the save the data
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
    } 

}
