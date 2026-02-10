

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace Location
{

    #region class LocationHeadendTab
   
    public partial class LocationHeadendTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public LocationHeadendTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Methods

        
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            this.CanExecuteNewCommand = false;
            this.CanExecuteSaveCommand = false;
            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "locations";

            SetParentChildAttributes();
           

                    
        

           


        }

      

        private void SetParentChildAttributes()
        {
            //Create the WHT Tracking object
            CurrentBusObj = new cBaseBusObject("Location");
            CurrentBusObj.Parms.ClearParms();
            //setup parent grid
            
            //Establish the WHT Tracking Grid
            
             //Establish the HeadEnd Grid
            GridLocationHeadEnd.MainTableName = "headends";
            GridLocationHeadEnd.ConfigFileName = "GridLocationHeadEnd";
            GridLocationHeadEnd.SetGridSelectionBehavior(false, true);
            GridLocationHeadEnd.FieldLayoutResourceString = "headends";
            //Add Tab to collection
            GridCollection.Add(GridLocationHeadEnd);

            GridLocationHeadEnd.SetGridSelectionBehavior(false, false);
            GridLocationHeadEnd.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "head_id" }, ChildGrids = { GridLocationServices }, ParentFilterOnColumnNames = { "head_id" } });




            //setup attributes for Child
            GridLocationServices.MainTableName = "service";
            GridLocationServices.ConfigFileName = "GridLocationService";
            GridLocationServices.FieldLayoutResourceString = "services";

            GridLocationServices.SetGridSelectionBehavior(false, false);

            //Add Tab to collection
            GridCollection.Add(GridLocationServices);
        }
        }
        #endregion

   

    }
    #endregion


