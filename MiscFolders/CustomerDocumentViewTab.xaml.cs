

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

namespace MiscFolders
{

    #region class CustomerDocumentViewTab
    /// <summary>
    /// This class represents a 'CustomerDocumentViewTab' object.
    /// </summary>
    public partial class CustomerDocumentViewTab : ScreenBase
    {
        string sServer_loc = " ";
        string sDirectory = " ";
        string sFilename = " ";
        string sLocation = " ";
        string sPathFile = " ";
        
        #region Private Variables
        private List<string> dataKeys = new List<string> { "file_name"}; //Used for double click

        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'CustomerDocumentViewTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerDocumentViewTab()
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

            //Establish the CustomerDocument View Grid - Robject Name
            //The GridView name ties to the grid in the CustomerDocumentViewTab xaml
            GridView.MainTableName = "view";
            //this is for the customized grid - needs to be unique
            GridView.ConfigFileName = "GridCustomerDocumentView";
            GridView.SetGridSelectionBehavior(false, true);
            GridView.WindowZoomDelegate = GridDoubleClickDelegate;
            //this should match FieldLayouts.xaml
            GridView.FieldLayoutResourceString = "CustomerDocumentView";
            GridCollection.Add(GridView);
        }
        #endregion

        public void GridDoubleClickDelegate()
        {

            //open up file


            ReturnSelectedData();

            
            if (File.Exists(sPathFile.ToString()))
                System.Diagnostics.Process.Start(sPathFile);
            else
                MessageBox.Show("File does not exist");
            //string PathFile = @"\\aurora\razer_source\prod\image\invoice\1010\INV0111713.pdf";
            //System.Diagnostics.Process.Start(sPathFile);
        }


        public void ReturnSelectedData()
        {
            
            
            if (this.CurrentBusObj.ObjectData.Tables[0].Rows.Count != 0)
            {



                GridView.ReturnSelectedData(dataKeys);

                
                sServer_loc = this.CurrentBusObj.ObjectData.Tables["view"].Rows[0].ItemArray[7].ToString();
                sDirectory = this.CurrentBusObj.ObjectData.Tables["view"].Rows[0].ItemArray[8].ToString();
                //sFilename = this.CurrentBusObj.ObjectData.Tables["view"].Rows[0].ItemArray[9].ToString();
                sFilename = cGlobals.ReturnParms[0].ToString();
                sLocation = sServer_loc + sDirectory + @"\";
                sPathFile = sLocation + sFilename;
            }
        }

        #endregion

    }
    #endregion

}
