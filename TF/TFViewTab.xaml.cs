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

namespace TF
{
    /// <summary>
    /// Interaction logic for TFViewTab.xaml
    /// </summary>
    public partial class TFViewTab : ScreenBase
    {
        string sServer_loc = " ";
        string sDirectory = " ";
        string sFilename = " ";
        string sLocation = " ";
        string sPathFile = " ";
        private static readonly string server_loc = "server_loc";
        private static readonly string file_name = "file_name";
        private static readonly string directory = "directory";
        public TFViewTab()
            : base()
        {
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }

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
            gTFView.MainTableName = "view";
            gTFView.ConfigFileName = "BCFViewTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gTFView.SetGridSelectionBehavior(false, true);
            gTFView.FieldLayoutResourceString = "BCFView";
            gTFView.WindowZoomDelegate = GridDoubleClickDelegate;
            //GridCustomerDocumentDetail.IsFilterable = true;
            //GridCustomerDocumentDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "apply_to_doc", "seq_code" }, ChildGrids = { GridCustomerDocumentApplied }, ParentFilterOnColumnNames = { "document_id", "seq_code" } });
            GridCollection.Add(gTFView);

        }


         public void BCFviewClearGrid()
         {
             this.CurrentBusObj.ObjectData.Tables["view"].Rows.Clear();
         }

         public void GridDoubleClickDelegate()
         {

             DataRecord record = gTFView.xGrid.ActiveRecord as DataRecord;

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
    }
}
