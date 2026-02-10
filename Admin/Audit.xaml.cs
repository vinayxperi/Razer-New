using RazerInterface; //Required for IPreBindable
using RazerBase.Interfaces; //Required for IScreen
using RazerBase;
using RazerBase.Lookups;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infragistics.Windows.DataPresenter;
using System;
using System.Data;
using System.Collections.Generic;
using Infragistics.Windows.Editors;

namespace Admin 
{
    /// <summary>
    /// Interaction logic for TableListing.xaml
    /// </summary>
    public partial class Audit : ScreenBase, IScreen , IPreBindable
 

    {
       
         private string AudTableName { get; set; }
         private string AudUser { get; set; }
         //public ComboBoxItemsProvider cmbUser { get; set; }
         //public ComboBoxItemsProvider cmbAuditTable { get; set; }
        
        
        

        public string WindowCaption { get { return string.Empty; } }
        
        public Audit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Implement the Init method of IScreen
        /// </summary>
        /// <param name="businessObject">Then base busniess object</param>
        public void Init(cBaseBusObject businessObject)
        {
            
            
            this.CanExecuteSaveCommand = false;
            this.MainTableName = "AuditTables";
            this.CurrentBusObj = businessObject;

            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            gridAuditDetail.xGrid.FieldLayoutSettings = layouts;
            gridAuditDetail.IsFilterable = true;
           
               
                //load Custnum from bus obj
                AudTableName = "";
                AudUser = "";
                //load current parms
                this.loadParms(AudTableName);
                // load the data
                this.Load(businessObject);

               
                //gridAuditTables.LoadGrid(businessObject, gridAuditTables.MainTableName);

                System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
         
        

        }

        /// <summary>
        /// Handler for double click on grid or buttton click.
        /// </summary>

        private void loadParms(string TableName)
        {
            try
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                if (TableName == "")
                {
                    this.CurrentBusObj.Parms.AddParm("@tablename","");
                    this.CurrentBusObj.Parms.AddParm("@from_date", "01/01/1900");
                    this.CurrentBusObj.Parms.AddParm("@to_date", "01/01/1900");
                    this.CurrentBusObj.Parms.AddParm("@audit_user","");
                }

             
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }


        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {

                    
                    this.cmbUser.SetBindingExpression("USER_ID", "user_name", this.CurrentBusObj.ObjectData.Tables["dddwAuditUser"]);
                    this.cmbAuditTable.SetBindingExpression("robject_member_name", "audit_table_desc", this.CurrentBusObj.ObjectData.Tables["dddwAuditTables"]);

                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

 

        private void cmbAuditTable_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AudTableName = cmbAuditTable.SelectedValue.ToString();
            this.CurrentBusObj.changeParm("@tablename", AudTableName.ToString());


        }

        public void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cmbAuditTable.SelectedValue == null)
            {
                MessageBox.Show("Audit Table to view must be selected");
                cmbAuditTable.Focus();
                return;


            }
            //validate the audit table has been selected and if other columns populated, populate parms
            if (cmbAuditTable.SelectedValue == "")
            {
                MessageBox.Show("Audit Table to view must be selected");
                cmbAuditTable.Focus();
                return;


            }
            else
            {
                AudTableName = cmbAuditTable.SelectedValue.ToString();
                this.CurrentBusObj.changeParm("@tablename", AudTableName.ToString());
            }

            if (cmbUser.SelectedText.ToString() == "")
                this.CurrentBusObj.changeParm("@audit_user", "");
            else
                if (cmbUser.SelectedText.ToString() == " ALL")
                    this.CurrentBusObj.changeParm("@audit_user", "");
                else
                this.CurrentBusObj.changeParm("@audit_user", cmbUser.SelectedValue.ToString());
            
            if (ldtFromDate.SelText.ToString() == "")
                //DateTime.Today.ToString()
                this.CurrentBusObj.changeParm("@from_date", "01/01/1900");
            else
                this.CurrentBusObj.changeParm("@from_date", ldtFromDate.SelText.ToString());

          
            if (ldtToDate.SelText.ToString() == "")
                this.CurrentBusObj.changeParm("@to_date", "01/01/1900");
            else
                this.CurrentBusObj.changeParm("@to_date", ldtToDate.SelText.ToString());
           
          
            
                GridCollection.Add(gridAuditDetail);
                gridAuditDetail.MainTableName = AudTableName.ToString();
                this.Load(AudTableName.ToString());
                if (this.CurrentBusObj.HasObjectData)
                {
                }


            

        }

    

        private void cmbUser_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if  (cmbUser.SelectedValue.ToString() == null)
            {
                AudUser = "";
            }
            else
            {
            AudUser = cmbUser.SelectedValue.ToString();
            this.CurrentBusObj.changeParm("@audit_user", AudUser.ToString());
            }
        }

        private void ldtFromDate_Loaded(object sender, RoutedEventArgs e)
        {
        
        }



        
    }
}
