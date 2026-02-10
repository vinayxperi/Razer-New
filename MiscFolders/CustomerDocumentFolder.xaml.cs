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
using Infragistics.Windows.DockManager;

namespace MiscFolders
{
    /// <summary>
    /// Interaction logic for CustomerDocumentFolder.xaml
    /// </summary>
    public partial class CustomerDocumentFolder : ScreenBase, IScreen
    {

        private string DocumentID { get; set; }
        private string DocumentType { get; set; }

        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }

        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBase constructor is also called.
        /// </summary>
        public CustomerDocumentFolder()
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
                this.MainTableName = "CustomerDocument";
                // Change this if your folder has item directly bound to it.
                // In many cases the Tabs have data binding, the Folders
                // do not.
                this.DoNotSetDataContext = false;

                

                // set the businessObject
                this.CurrentBusObj = businessObject;

                // add the Tabs
                TabCollection.Add(General);
                TabCollection.Add(Detail);
                TabCollection.Add(View);
                

                // if there are parameters than we need to load the data
                if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
                {
                    string DocumentID = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();
                    txtDocumentID.Text = DocumentID.ToString();
                    this.CurrentBusObj.Parms.AddParm("@comment_type", "RA");
                    this.CurrentBusObj.Parms.AddParm("@external_char_id", DocumentID);
                    // load the data
                    this.Load();
                    // Set the Header
                    if (this.CurrentBusObj.ObjectData.Tables["general"] != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count>0)
                    {
                        windowCaption = "Customer Document ID -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[0].ToString();
                        //txtDocumentType.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[3].ToString();
                        txtDescription.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[14].ToString();
                        ////SetHeaderName();
                    }
                    else
                    {
                        Messages.ShowWarning("Customer Document Not Found");
                        
                    }

                }

          

        }

        private void txtDocumentID_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            DocumentID = txtDocumentID.Text;
        }

        private void txtDocumentID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //If the Document ID changed then load a new document
            if (txtDocumentID.ToString() != DocumentID)
                ReturnData(txtDocumentID.Text, "@document_id");
        }


        private void ReturnData(string SearchValue, string DbParm)
        {
            
            //if no value do nothing
            if (SearchValue == "") return;
            //Clear the current parameters
            this.CurrentBusObj.Parms.ClearParms();
            //Add new parameters
            this.CurrentBusObj.Parms.AddParm(DbParm, SearchValue);

            //load data
            this.Load();
            if (chkForData()) SetHeaderName();
          
        }
        
                private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables[0].Rows.Count != 0 && this.CurrentBusObj.ObjectData.Tables["general"] != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count>0)
            {
                windowCaption = "Customer Document ID -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[0].ToString();
                //txtDocumentType.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[3].ToString();
                txtDescription.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[14].ToString();
                return true;
            }
            else
            {
                Messages.ShowWarning("Customer Document Not Found");
                return false;
            }
        }

 

                //if customer number found then set header and pop otherwise send message
           




        /// <summary>
        /// Sets HeaderName based on value entered into customerName textbox
        /// </summary>
        private void SetHeaderName()
        {
            //Sets the header name when being called from another folder
            if (txtDocumentID.Text == null)
            {
                windowCaption = "Customer Document ID -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[0].ToString();
                //txtDocumentType.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[3].ToString();
                txtDescription.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[14].ToString();
                
                
            }
            //Sets the header name from within same folder
            else
            {
                ContentPane p = (ContentPane)this.Parent;
                p.Header = "Customer Document ID -" + txtDocumentID.Text;
            }
        }

    }
}