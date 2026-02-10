

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



namespace Entity
{
   
    public partial class EntityFolder : ScreenBase, IScreen
    {

        private int EntityID { get; set; }
        private string EntityName { get; set; }
        private static readonly string captionName = "Entity ";
        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }

        
        public EntityFolder()
            : base()
        {
           
            InitializeComponent();
         
        }

         
        public void Init(cBaseBusObject businessObject)
        {

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "Entity";
           
            this.DoNotSetDataContext = false;
            this.CanExecuteNewCommand = false;
            this.CanExecuteSaveCommand = false;


            // set the businessObject
            this.CurrentBusObj = businessObject;

            // add the Tabs
            TabCollection.Add(EntityGeneralTab);
            TabCollection.Add(EntityLocationTab);
            TabCollection.Add(EntityContractsTab);
            TabCollection.Add(EntityUnitsTab);

           
            // if there are parameters than we need to load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                
                EntityID = Convert.ToInt32(this.CurrentBusObj.Parms.ParmList.Rows[0][1]);
               
                txtEntityID.Text = EntityID.ToString();
                //loadParms
                this.loadParms(EntityID.ToString());

                // load the data
                this.Load();
                // Set the Header
                windowCaption =   "Entity - " +  this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[0].ToString();
                txtEntityName.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[1].ToString();
                SetHeaderName();

                 
            }
            else
                this.loadParms("");


        }

        private void loadParms(string EntityId)
        {
            try
            {
                if (EntityId != null && EntityId != "")
                {
                    //ksh 9/21/11 needed for calls from other folders
                    this.CurrentBusObj.Parms.ClearParms();
                    this.CurrentBusObj.Parms.AddParm("@mso_id", (object)EntityId);
                    
                }
                else
                {
                    //if EntityId NOT passed load   with global parm EntityId if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.ClearParms();
                        this.CurrentBusObj.Parms.AddParm("@mso_id", cGlobals.ReturnParms[0]);

                    }
                    else
                    {
                        this.CurrentBusObj.Parms.ClearParms();
                        this.CurrentBusObj.Parms.AddParm("@mso_id", -1);
                    }
                    //doing an insert setup dummy vals
                    
                }
                this.CurrentBusObj.Parms.AddParm("@service_period_start", "01/01/1900");
                this.CurrentBusObj.Parms.AddParm("@service_period_end", "01/01/1900"); 
                //////////////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }

       
        //Load Entity
        private void Entity_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on Entity ID field
           EntityLookup f = new EntityLookup();

            this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {

                //load current parms
                loadParms("");
                txtEntityID.Text = cGlobals.ReturnParms[0].ToString();
                clrUnits();
                // Call load 
                this.Load();
                
                windowCaption = "Entity -" + txtEntityID.Text;
                EntityID = Convert.ToInt32(txtEntityID.Text);
                SetHeaderName();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
                General.Focus();

            }
             
        }

        private void txtEntityID_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            EntityID = Convert.ToInt32(txtEntityID.Text);
        }

    
        private void txtEntityID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtEntityID.Text) != EntityID)
                EntityUnitsTab.txtServiceDateEnd.SelText = Convert.ToDateTime("01/01/1900");
                EntityUnitsTab.txtServiceDateStart.SelText = Convert.ToDateTime("01/01/1900");
                ReturnData(txtEntityID.Text, "@mso_id");
        }

       
        

        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                return true;
            }
            else
            {
                Messages.ShowWarning("Entity Not Found");
                return false;
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
            clrUnits();
            //load data
            //if coming from save do not do this...
            this.Load();
            //clear any unit totals on Units tab when entity changes
            EntityUnitsTab.txtUnitTotal.Text = "";


            //if customer number found then set header and pop otherwise send message
            if (chkForData())
            {
                SetHeaderName();
                this.General.Focus();
            }
        }

        private void clrUnits()
        {
            if (this.CurrentBusObj.ObjectData != null)
            {
                this.CurrentBusObj.ObjectData.Clear();
            }
            this.EntityUnitsTab.txtUnitTotal.Text = "0";
            this.EntityUnitsTab.txtServiceDateStart.SelText = Convert.ToDateTime("01/01/1900");
            this.EntityUnitsTab.txtServiceDateEnd.SelText = Convert.ToDateTime("01/01/1900");
        }

         
        private void SetHeaderName()
        {
            //Sets the header name when being called from another folder
            if (txtEntityID.Text == null)
            {
                //KSH 10/15/2011- added for click-throughs from other folders
                SetupWindowCaption();
            }
            //Sets the header name from within same folder
            else
            {
                //KSH 10/15/2011- added for click-throughs from other folders
                //if coming from another folder parent will be null need to run ignore hte content pane setup
                if (this.Parent != null)
                {
                    ContentPane p = (ContentPane)this.Parent;
                    p.Header = "Entity -" + txtEntityID.Text;
                    txtEntityName.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[1].ToString();
                }
                else
                {
                    SetupWindowCaption();
                }
            }
        }

        /// <summary>
        /// KSH 10/15/2011 -- setup window caption and screen state
        /// </summary>
        private void SetupWindowCaption()
        {
            windowCaption = "Entity -" + this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[1].ToString();
            txtEntityID.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[0].ToString();
            txtEntityName.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[1].ToString();
            EntityID = Convert.ToInt32(txtEntityID.Text);
        }

       
    }
}