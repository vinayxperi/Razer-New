using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using RazerInterface;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Linq;
using Microsoft.VisualBasic;

namespace GeneralLedger
{

    /// Interaction logic for DefRevenue.xaml

    public partial class DefRevenue : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "GLVchrFieldLayouts";
        public string DefAcct = "";
        public string PoolID = "";



        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
        }


        public DefRevenue()
            : base()
        {
            InitializeComponent();
        }


        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            //set isscreendirty = false to prevent save message
            //this.IsScreenDirty = false;
            this.MainTableName = "defTotal";
            this.CanExecuteSaveCommand = false;
            GridDefPool.WindowZoomDelegate = ReturnSelectedData;
            GridDefPool.xGrid.FieldLayoutSettings = layouts;
            GridDefPool.FieldLayoutResourceString = "DefPoolSmry";
            GridDefPool.SetGridSelectionBehavior(false, true);
            GridDefPool.MainTableName = "defPool";
            GridDefPool.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "pool_detail_id" }, ChildGrids = { GridDefPoolDetail }, ParentFilterOnColumnNames = { "pool_detail_id" } });
            GridDefPoolDetail.FieldLayoutResourceString = "DefPoolDetail";
            GridDefPoolDetail.SetGridSelectionBehavior(false, true);
            GridDefPoolDetail.MainTableName = "defDetail";

            GridCollection.Add(GridDefPool);
            GridCollection.Add(GridDefPoolDetail);

            //this.Load(businessObject);
            this.CurrentBusObj = businessObject;
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                string DefAcct = this.CurrentBusObj.Parms.ParmList.Rows[0]["def_account_code"].ToString();
                //GridDefPool.LoadGrid(businessObject, GridDefPool.MainTableName);

                //GridDefPoolDetail.LoadGrid(businessObject, GridDefPoolDetail.MainTableName);
                this.Load();

                System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            }

        }




        /// Handler for double click on grid to return the app selected to run
        public void ReturnSelectedData()
        {

            //gCashBatchesToRun.ReturnSelectedData();
            //if (cGlobals.ReturnParms.Count > 0)
            //{
            //    jobName = cGlobals.ReturnParms[0].ToString();

            //}



        }


      
       

        public override void Close()
        {

          

        }



        private void txtDefAcct_GotFocus(object sender, RoutedEventArgs e)
        {
            DefAcct = txtDefAcct.Text;
        }

        private void txtDefAcct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDefAcct.Text != DefAcct)

                ReturnData(txtDefAcct.Text, "@def_account_code");
        }

        private void txtDefAcct_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on Location ID field
            DefPoolLookup f = new DefPoolLookup();

            this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {

                //load current parms
                DefAcct = null;
                loadParms("");
                txtDefAcct.Text = cGlobals.ReturnParms[0].ToString();
                //PoolID = cGlobals.ReturnParms[1].ToString();
                // Call load 
                this.Load();

                //if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
                //{
                //    GridDefPool.LoadGrid(CurrentBusObj, GridDefPool.MainTableName);
                //    GridDefPoolDetail.LoadGrid(CurrentBusObj, GridDefPoolDetail.MainTableName);
                
                //}
           
                windowCaption = "Deferred Pool -" + txtDefAcct.Text;
                DefAcct = txtDefAcct.Text;
                SetHeaderName();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
               

            }
        }


        private void loadParms(string GLJournalID)
        {
            try
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                //if adjustment number passed load document id
               if (!string.IsNullOrEmpty(DefAcct))
                {
                    this.CurrentBusObj.Parms.AddParm("@def_account_code", DefAcct);
                    //this.CurrentBusObj.Parms.AddParm("@pool_id", PoolID);
               

                }
                else
                {
                    //if adjustmentid NOT passed load   with global parm adjustmentid if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.AddParm("@def_account_code", cGlobals.ReturnParms[0].ToString());
                        //this.CurrentBusObj.Parms.AddParm("@pool_id", cGlobals.ReturnParms[1].ToString());
                        
                    }
                    //set dummy vals
                    else
                    {

                        this.CurrentBusObj.Parms.AddParm("@def_account_code", "-1");
                        //this.CurrentBusObj.Parms.AddParm("@related_to_char_id", "-1");
                    }
                }

            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }




        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["defPool"].Rows.Count != 0)
            {

                return true;
            }
            else
            {
                Messages.ShowWarning("Deferred Revenue Account Not Found");
                return false;
            }
        }


        private void ReturnData(string SearchValue, string DbParm)
        {
            //if no value do nothing
            if (SearchValue == "") return;
            //Add new parameters
            loadParms(SearchValue);
            //load data
            //if coming from save do not do this...
            this.Load();
            //if invoiceNumber found then set header and pop otherwise send message
            if (chkForData())
            {
                SetHeaderName();

            }
        }

        private void SetHeaderName()
        {
            //Sets the header name when being called from another folder
            if (txtDefAcct.Text == null)
            {
                txtDefAcct.Text = this.CurrentBusObj.ObjectData.Tables["defDetail"].Rows[0]["def_account_code"].ToString();
                windowCaption = "Deferred Account -" + txtDefAcct.Text ;
  
            }
            //Sets the header name from within same folder
            else
            {
                windowCaption = "Deferred Account -" + txtDefAcct.Text;
                //txtDefAcct.Text = this.CurrentBusObj.ObjectData.Tables["defDetail"].Rows[0]["def_account_code"].ToString();
             
            }
        }

        




    }
}
