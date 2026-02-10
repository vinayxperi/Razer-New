

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace RecvAcctFolder
{

    #region class ucRecvAcctGeneralTab
    /// <summary>
    /// Interaction logic for ucRecvAcctGeneralTab.xaml
    /// </summary>
    public partial class ucRecvAcctGeneralTab : ScreenBase, IPreBindable
    {
        
        #region Private Variables
        #endregion
        
        #region Constructor
        /// <summary>
        /// Create a new instance of a ucRecvAcctGeneralTab and call the ScreenBase's constructor
        /// </summary>
        public ucRecvAcctGeneralTab() : base()
        {
            // This call is required by the designer.
            InitializeComponent();
            //Add any initialization after the InitializeComponent() call.
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            
            //The main table used on the tab item is the General table.  This value will be used to 
            // determine what table to pull from the base business object dataset
            MainTableName = "General";
        }
        #endregion

        #region Methods

            #region PreBind()
            /// <summary>
            /// This method is used to perform any operations that must be set
            /// before the DataContext is set.
            /// </summary>
            public void PreBind()
            {
                try
                {
                    // if the object data was loaded
                    if (this.CurrentBusObj.HasObjectData) 
                    {
                        // set the dataTable for terms
                        DataTable termsDataTable = this.CurrentBusObj.GetTable("Terms") as DataTable;

                        // if the dataTable exists
                        if (termsDataTable != null)
                        {
                            // ************************************************
                            // **********  Setup the Binding For Terms  ************
                            // ************************************************

                            // Create the BindingObject for the CategoryLookup
                            BindingObject bindingObject = new BindingObject();

                            // You must set the SourceData. Here we are creating sample a sample DataTable
                            bindingObject.SourceData = termsDataTable;

                            // Set the DisplayField and the ValueField (this can be the same field)
                            bindingObject.DisplayField = "description";
                            bindingObject.ValueField = "terms_code";

                            // Set the title for the window
                            bindingObject.Title = "Select Terms";

                            // This bindingObject uses a string key
                            bindingObject.LookupAsString = true;

                            // The CallBackMethod does not need to be set here 
                            // because we are using the ucLookupTextBox.
                            // Look in the ucLookupTextBox.LookupValueSelected method
                            // to see an example of implementing your own CallBackMethod.
                            // this.CategoryTextBox.CallBackMethod = [CallBackMethodName];

                            // The name is only needed if the CallBackMethod is handling multiple
                            // Lookup selections, so you can set the returned values to the 
                            // correct controls.
                            bindingObject.Title = "Terms";

                            // Set the bindingObject
                            this.tTerms.BindingObject = bindingObject;
                        }
                    }
                }
                catch (Exception error)
                {
                    // for debugging only
                    string err = error.ToString();
                }
            }
            #endregion

        #endregion
        
    }
    #endregion

}
