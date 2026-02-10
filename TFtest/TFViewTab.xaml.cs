

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

namespace BCF
{

    #region class BCFViewTab
    /// <summary>
    /// This class represents a 'BCFViewTab' object.
    /// </summary>
    public partial class BCFViewTab : ScreenBase
    {
        string sServer_loc = " ";
        string sDirectory = " ";
        string sFilename = " ";
        string sLocation = " ";
        string sPathFile = " ";
        private static readonly string server_loc = "server_loc";
        private static readonly string file_name = "file_name";
        private static readonly string directory = "directory";
        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'BCFViewTab' object and call the ScreenBase's constructor.
        /// </summary>
        public BCFViewTab()
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
            CurrentBusObj = new cBaseBusObject("BCFFolder");
            CurrentBusObj.Parms.ClearParms();

            //Establish the Invoice View Grid
            gBCFView.MainTableName = "view";
            gBCFView.ConfigFileName = "BCFViewTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gBCFView.SetGridSelectionBehavior(false, true);
            gBCFView.FieldLayoutResourceString = "BCFView";
            gBCFView.WindowZoomDelegate = GridDoubleClickDelegate;
            //GridCustomerDocumentDetail.IsFilterable = true;
            //GridCustomerDocumentDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "apply_to_doc", "seq_code" }, ChildGrids = { GridCustomerDocumentApplied }, ParentFilterOnColumnNames = { "document_id", "seq_code" } });
            GridCollection.Add(gBCFView);

        #endregion
        }

        public void BCFviewClearGrid()
        {
            this.CurrentBusObj.ObjectData.Tables["view"].Rows.Clear();
        }

        public void GridDoubleClickDelegate()
        {

            DataRecord record = gBCFView.xGrid.ActiveRecord as DataRecord;

            if (record != null)
            {
                string serverLocation = record.Cells[server_loc].Value.ToString();
                string fileName = record.Cells[file_name].Value.ToString();
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
        }

        #endregion

    }
    #endregion

}
