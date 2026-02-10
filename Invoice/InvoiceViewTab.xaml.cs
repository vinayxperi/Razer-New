

#region using statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
using Infragistics.Windows.DataPresenter;
using System.Diagnostics;

#endregion

namespace Invoice
{

    #region class InvoiceViewTab
    /// <summary>
    /// This class represents a 'InvoiceViewTab' object.
    /// </summary>
    public partial class InvoiceViewTab : ScreenBase
    {
        string sServer_loc = " ";
        string  sDirectory = " ";
        string sFilename = " ";
        string sLocation = " ";
        string sPathFile = " ";
        private static readonly string server = "server_loc";
        private static readonly string file = "file_name";
        private static readonly string directory = "directory";
        #region Private Variables
        
     
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'InvoiceViewTab' object and call the ScreenBase's constructor.
        /// </summary>
        public InvoiceViewTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Methods

        #region Init()
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "view";
            //Set up Parent Child Relationship
            //Create the Customer Document object
            CurrentBusObj = new cBaseBusObject("Invoice");
            CurrentBusObj.Parms.ClearParms();
            
            //Establish the Invoice View Grid
            gInvoiceView.MainTableName = "view";
            gInvoiceView.ConfigFileName = "InvoiceViewTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gInvoiceView.SetGridSelectionBehavior(false, true);
            gInvoiceView.FieldLayoutResourceString = "InvoiceView";
            gInvoiceView.WindowZoomDelegate = GridDoubleClickDelegate;
            //GridCustomerDocumentDetail.IsFilterable = true;
            //GridCustomerDocumentDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "apply_to_doc", "seq_code" }, ChildGrids = { GridCustomerDocumentApplied }, ParentFilterOnColumnNames = { "document_id", "seq_code" } });
            GridCollection.Add(gInvoiceView);
        #endregion

    }

        public void GridDoubleClickDelegate()
        {

            DataRecord record = gInvoiceView.xGrid.ActiveRecord as DataRecord;

            if (record != null)
            {
                string serverLocation = record.Cells[server].Value.ToString();
                string fileName = record.Cells[file].Value.ToString();
                string folder = record.Cells[directory].Value.ToString();

                StringBuilder pathBuilder = new StringBuilder(serverLocation);
                pathBuilder.Append(folder);
                pathBuilder.Append(@"\");
                pathBuilder.Append(fileName);
                if (File.Exists(pathBuilder.ToString()))
                     Process.Start(pathBuilder.ToString());
                else
                    MessageBox.Show("File does not exist");
            }
            ////open up file
            //ReturnSelectedData();
             
         
            ////string PathFile = @"\\aurora\razer_source\prod\image\invoice\1010\INV0111713.pdf";
            //System.Diagnostics.Process.Start(sPathFile);
        }


       public void ReturnSelectedData()
        {
            //if (this.CurrentBusObj.ObjectData.Tables[0].Rows.Count != 0)
            //{
            //    sServer_loc = this.CurrentBusObj.ObjectData.Tables["view"].Rows[0].ItemArray[7].ToString();
            //    sDirectory = this.CurrentBusObj.ObjectData.Tables["view"].Rows[0].ItemArray[8].ToString();
            //    sFilename = this.CurrentBusObj.ObjectData.Tables["view"].Rows[0].ItemArray[9].ToString();
            //    sLocation = sServer_loc + sDirectory + @"\";
            //    sPathFile = sLocation + sFilename;
            //}
        }

        #endregion

        #endregion

    }
 

}
