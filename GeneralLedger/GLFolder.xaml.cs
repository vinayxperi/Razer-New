

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

namespace GeneralLedger
{

    #region class GLFolder
    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class GLFolder : ScreenBase, IScreen
    {

        private string GLJournalID { get; set; }


        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
        }
     
        public GLFolder()
            : base()
        {
            // Create Controls
            InitializeComponent();



        }


        public void Init(cBaseBusObject businessObject)
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "GLFolder";
            this.DoNotSetDataContext = false;
            // set the businessObject
            this.CurrentBusObj = businessObject;

            // add the Tabs
            TabCollection.Add(GeneralTab);
            TabCollection.Add(DetailTab);
         


            // if there are parameters than we need to load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                string GLJournalID = this.CurrentBusObj.Parms.ParmList.Rows[0]["journal_ctrl_num"].ToString();

                

                //set document_id for View tab
                this.loadParms(GLJournalID);
                // load the data
                this.Load();
                // Set the Header
                //Need to chack 
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
                {
                    windowCaption = "GL Journal -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["journal_ctrl_num"].ToString();
                    txtJournal.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["journal_ctrl_num"].ToString();
                    txtDescription.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["description"].ToString();
                    
                }
                else
                    Messages.ShowWarning("GL Journal not found!");
            }
        }


        private void loadParms(string GLJournalID)
        {
            try
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                //if adjustment number passed load document id
                if (GLJournalID != "")
                {
                    this.CurrentBusObj.Parms.AddParm("@journal_ctrl_num", GLJournalID);
                    //this.CurrentBusObj.Parms.AddParm("@document_id", invoiceNumber);

                }
                else
                {
                    //if adjustmentid NOT passed load   with global parm adjustmentid if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.AddParm("@journal_ctrl_num", cGlobals.ReturnParms[0].ToString());
                        //this.CurrentBusObj.Parms.AddParm("@document_id", cGlobals.ReturnParms[0].ToString());
                    }
                    //set dummy vals
                    else
                    {

                        this.CurrentBusObj.Parms.AddParm("@journal_ctrl_num", "-1");
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
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
               
                return true;
            }
            else
            {
                Messages.ShowWarning("GL Journal Not Found");
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
            if (txtJournal.Text == null)
            {
                windowCaption = "GL Journal -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["journal_ctrl_num"].ToString();
                txtJournal.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["journal_ctrl_num"].ToString();
                txtDescription.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["description"].ToString();


            }
            //Sets the header name from within same folder
            else
            {
                ContentPane p = (ContentPane)this.Parent;
                p.Header = "GL Journal -" + txtJournal.Text;
                txtJournal.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["journal_ctrl_num"].ToString();
                txtDescription.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["description"].ToString();

            }
        }

        private void GLJV_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on Location ID field
            GLJVLookup f = new GLJVLookup();

            this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {

                //load current parms
                loadParms("");
                txtJournal.Text = cGlobals.ReturnParms[0].ToString();

                // Call load 
                this.Load();

                windowCaption = "GL Journal -" + txtJournal.Text;
                GLJournalID =  txtJournal.Text;
                SetHeaderName();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
                GeneralTab.Focus();

            }

        }

        

        private void txtJournal_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtJournal.Text != GLJournalID)

                ReturnData(txtJournal.Text, "@journal_ctrl_num");
        }

        private void txtJournal_GotFocus(object sender, RoutedEventArgs e)
        {
            GLJournalID = txtJournal.Text;
        }

    }
}


    #endregion;
