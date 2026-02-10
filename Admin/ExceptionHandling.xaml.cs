using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using RazerInterface;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.ComponentModel;

namespace Admin
{
   
    /// Interaction logic for ExceptionHandling

    public partial class ExceptionHandling : ScreenBase, IScreen, IPreBindable
    {
        private static readonly string fieldLayoutResource = "exceptionHandling";
        private static readonly string mainTableName = "ExceptionHandling";
        
            string exceptionValueField = "type_id";
            string exceptionDisplayField = "description";
            int  ExceptionType = 0;
            Boolean LoadfirstTime = true;
            string exceptionDesc = " ";

            public cBaseBusObject ExceptionHandlingBusObject = new cBaseBusObject();
        public string WindowCaption { get { return string.Empty; } }
        

        public ExceptionHandling()
            : base()
        {

            //set obj
            this.CurrentBusObj = ExceptionHandlingBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "ExceptionHandling";
            // This call is required by the designer.
            InitializeComponent();
            cmbExceptionType.PropertyChanged += new PropertyChangedEventHandler(cmbExceptionType_PropertyChanged);
                      
        }

        
        public void Init(cBaseBusObject businessObject)
        {
           
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;
            this.MainTableName = "ExceptionHandling";
            this.DoNotSetDataContext = true;
            ////do this to pop product dropdown
            this.CurrentBusObj.Parms.ClearParms();
            this.CurrentBusObj.Parms.AddParm("@type_id", 0);
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            this.CanExecuteSaveCommand = false;

            //Set up generic context menu selections
            gExceptions.ContextMenuRemoveIsVisible = false;
            gExceptions.ContextMenuGenericDelegate1 = ContextMenuClearExceptions;
            gExceptions.ContextMenuGenericDisplayName1 = "Clear Exceptions";
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                gExceptions.ContextMenuGenericIsVisible1 = false;
                gExceptions.ContextMenuAddIsVisible = false;
               
           
            }
            else
            {
                gExceptions.ContextMenuGenericIsVisible1 = true;
                gExceptions.ContextMenuAddIsVisible = true;
               
            }
         
            gExceptions.ConfigFileName = "ExceptionsConfigFile";

            //add delegates to be enabled
            gExceptions.ContextMenuAddDelegate = ContextMenuClearExceptions;

            gExceptions.ContextMenuAddIsVisible = false;

            //gAdjustmentQueue.xGrid.FieldLayoutSettings = layouts;
            gExceptions.FieldLayoutResourceString = fieldLayoutResource;
            gExceptions.MainTableName = "exceptions";
            this.MainTableName = mainTableName;
            this.Load();
            GridCollection.Add(gExceptions);
 
           //////////////////////

        }
        private void populateFields()
        {
            this.CurrentBusObj.Parms.ClearParms();
            this.CurrentBusObj.Parms.AddParm("@type_id", cmbExceptionType.SelectedValue.ToString().Trim());
             
            //load the object
            this.Load();
             gExceptions.SetGridSelectionBehavior(true, true);
            if (ExceptionType > 0)  
                gExceptions.LoadGrid(CurrentBusObj, gExceptions.MainTableName);
              

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);      
            
        }









        public void PreBind()
        {
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                if (LoadfirstTime == true)
                {
                    cmbExceptionType.SetBindingExpression(exceptionValueField, exceptionDisplayField, this.CurrentBusObj.ObjectData.Tables["dddwExceptionTypes"]);
                    LoadfirstTime = false;
                }
            }
        }
           

        private void ContextMenuClearExceptions()
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            //Find the current batch id
            if (this.CurrentBusObj.ObjectData.Tables["exceptions"].Rows.Count != 0)
            {
                {
                   //execute a service to delete exceptions for the exception type selected
                    if (ExceptionType.ToString() == "0")
                        Messages.ShowWarning("No Processs selected to Delete!");
                    else
                          exceptionDesc = cmbExceptionType.SelectedText.ToString();
                        MessageBoxResult result = Messages.ShowYesNo("Clear Exceptions for  " + exceptionDesc.ToString() , 
                            System.Windows.MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            if (cGlobals.BillService.ClearExceptions(ExceptionType) == true)
                            {
                                Messages.ShowWarning("Exceptions Cleared Successfully!");
                                populateFields();
                            }


                            else
                                Messages.ShowWarning("Error clearing exceptions");
                        }
                        else
                   

                            Messages.ShowMessage("Exceptions Not Cleared", MessageBoxImage.Information);
                }



                }

            }   
      
        
       
     

        private void cmbExceptionType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           
                    if (cmbExceptionType.SelectedValue.ToString().Trim() != "")
                    {
                        ExceptionType = int.Parse(cmbExceptionType.SelectedValue.ToString());
                        this.CurrentBusObj.Parms.AddParm("@type_id", ExceptionType);
                        populateFields();
                    }
               
        }

      
        

    }
}
