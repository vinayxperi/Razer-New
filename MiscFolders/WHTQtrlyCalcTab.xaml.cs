



using RazerBase;
using RazerBase.Interfaces;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System;
using System.Data;

 

namespace MiscFolders
{

     
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class WHTQtrlyCalcTab : ScreenBase
    {

        
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        /// //set grid to default
            

        public WHTQtrlyCalcTab()
            : base()
        {
           
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
         

         
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
       
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "wht_qtrly_est";
            
            //Establish the WHT Qtrly Calc Grid - Robject Name
            gWHTQtrlyCalc.MainTableName = "wht_qtrly_est";
            //this is for the customized grid - needs to be unique
            gWHTQtrlyCalc.ConfigFileName = "WhtQtrlyGrid";
            gWHTQtrlyCalc.SetGridSelectionBehavior(false, true);
            //this should match FieldLayouts.xaml
            gWHTQtrlyCalc.FieldLayoutResourceString = "WhtQtrlyCalc";
            //Make the grid editable
            gWHTQtrlyCalc.xGrid.FieldSettings.AllowEdit = true;
          

           

            GridCollection.Add(gWHTQtrlyCalc);
           
            
           
            
        }

        private void bCreate_Entry_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            //Save the changes to gWHTQtrlyCalc
            //save table wht_qtrly_est before
            this.SaveTable("wht_qtrly_est");
            //Establish the WHT Qtrly Calc Grid - Robject Name
            gWHTGlEntry.MainTableName = "wht_gl_entry";
            
            //this is for the customized grid - needs to be unique
            gWHTGlEntry.ConfigFileName = "WHTGlEntry";
            gWHTGlEntry.SetGridSelectionBehavior(false, true);
            //this should match FieldLayouts.xaml
            gWHTGlEntry.FieldLayoutResourceString = "WHTGLentry";
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            gWHTGlEntry.xGrid.FieldLayoutSettings = layouts;
            //Make the grid editable
            gWHTGlEntry.xGrid.FieldSettings.AllowEdit = false;
            //GridCollection.Add(gWHTGlEntry);

            
            cBaseBusObject obj = new cBaseBusObject();
            obj.BusObjectName = "WHTEntry";

            Load(obj);
            gWHTGlEntry.LoadGrid(obj, gWHTGlEntry.MainTableName);

         
        }

         private void ScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
         {

         }

        
       

    }
  

}
