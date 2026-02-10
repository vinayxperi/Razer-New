using RazerInterface; //Required for IPreBindable
using RazerBase.Interfaces; //Required for IScreen
using RazerBase;
using RazerBase.Lookups;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DataPresenter;
using System;
using System.Data;
using System.Collections.Generic;
using Infragistics.Windows.Editors;
using Infragistics.Documents.Excel;
using System.Windows.Forms;
using System.IO;

namespace Contact
{
    /// <summary>
    /// Interaction logic for AncillaryUpload.xaml
    /// </summary>
    public partial class SalespersonUpload : ScreenBase, IScreen, IPreBindable 
    {
        //Property is required for base objects that use IScreen
        public string WindowCaption { get; private set; }
        public ComboBoxItemsProvider cmbNewSales { get; set; }
        
        private string User = UserName.GetUserName;

        public SalespersonUpload()
            : base()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            WindowCaption = "Salesperson";

            //Setting intial screenstate to empty
            CurrentState = ScreenState.Empty;

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            //Set the maintablename for the folder if it has one
            this.MainTableName = "SalespersonUpload";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;

            //Add any Grid Configuration Information
            gUpload.MainTableName = "main"; //Should match the ROBJECT table name
            gUpload.ConfigFileName = "SalesUploadGridConfig"; //This is the file name that will store any user customizations to the grid - must be unique in the app
            gUpload.SetGridSelectionBehavior(true, true); //Sets standard grid behavior for record select and multiselect
            gUpload.FieldLayoutResourceString = "SalespersonUploadGrid"; //The name of the FieldLayout in the Field Layouts xaml file - Must be unique
            gUpload.IsFilterable = false;
            gUpload.DoNotSelectFirstRecordOnLoad = true;
            gUpload.IsEnabled = true;
            gUpload.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;

            

            //Add all grids to the grid collection - This allows grids to automatically load and participate with security
            GridCollection.Add(gUpload);

            //Set dummy parm values for initial load
            
            RetrieveData();
            SetWindowStatus();


        }

        private void SetWindowStatus()
        {
            if (CurrentState == ScreenState.Empty)
            {
                
                btnPreview.IsEnabled = true;
                btnClear.IsEnabled = false;
                btnSave.IsEnabled = false;
            }
            return;
        }

        private void RetrieveData()
        {
            this.Load();
        }

        public void PreBind()
        {
            if (this.CurrentBusObj.HasObjectData)
            {
                this.cmbSalesperson.SetBindingExpression("contact_id", "name", this.CurrentBusObj.ObjectData.Tables["dddwsalesperson"]);
                ComboBoxItemsProvider provider = new ComboBoxItemsProvider();
                //Set the items source to be the databale of the DDDW
                provider.ItemsSource = this.CurrentBusObj.ObjectData.Tables["dddwsalesperson"].DefaultView;

                //set the value and display path
                provider.ValuePath = "contact_id";
                provider.DisplayMemberPath = "name";
                //Set the property that the grid combo will bind to
                //This value is in the binding in the layout resources file for the grid.
                cmbNewSales = provider;
            }
        }


        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            int ctr = 0;
            int newSalesID = 0;
            newSalesID = Convert.ToInt32(cmbSalesperson.SelectedValue);
            if (cmbSalesperson.SelectedText.ToString() != "")
                if (gUpload.xGrid.SelectedItems.Records.Count > 0)
                {
                    foreach (Record record in gUpload.xGrid.SelectedItems.Records)
                    {
                        ctr++;
                        DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                        r["new_salesperson_id"] = newSalesID;
                        r["title"] = "PENDING";
                    }
                    btnClear.IsEnabled = true;
                    btnSave.IsEnabled = true;
                }
                else
                    Messages.ShowError("No rows to change have been selected.  Please select rows to change before Previewing.");
            else
                Messages.ShowError("A Salesperson must be selected before Previewing.");

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
           
            //base.Save();
            bool NoChanges = true;
            gUpload.xGrid.ExecuteCommand(Infragistics.Windows.DataPresenter.DataPresenterCommands.EndEditModeAndAcceptChanges);

            Infragistics.Windows.DataPresenter.UpdateMode currentMode = gUpload.xGrid.UpdateMode;

            gUpload.xGrid.UpdateMode = Infragistics.Windows.DataPresenter.UpdateMode.OnUpdate;
            gUpload.xGrid.ExecuteCommand(Infragistics.Windows.DataPresenter.DataPresenterCommands.CommitChangesToAllRecords);

            gUpload.xGrid.UpdateMode = currentMode;

            DataTable salespersonChanges = this.CurrentBusObj.ObjectData.Tables["main"].GetChanges(DataRowState.Modified);
            if (salespersonChanges != null)
            {
                foreach (DataRow row in salespersonChanges.Rows)
                {
                    if (Convert.ToString(row["title"]) == "PENDING") 
                    {
                        NoChanges = false;
                        int contract_salesperson_id = Convert.ToInt32(row["contract_salesperson_id"]);
                        int salesperson_id = Convert.ToInt32(row["salesperson_id"]);
                        string first_name = Convert.ToString(row["first_name"]);
                        string last_name = Convert.ToString(row["last_name"]);
                        string title = Convert.ToString(row["title"]);
                        int contract_id = Convert.ToInt32(row["contract_id"]);
                        string contract_description = Convert.ToString(row["contract_description"]);
                        string product_code = Convert.ToString(row["product_code"]);
                        int new_salesperson_id = Convert.ToInt32(row["new_salesperson_id"]);
                        cGlobals.BillService.UpdateSalesPerson(contract_salesperson_id, salesperson_id, first_name, last_name, title, contract_id,
                                                              contract_description, product_code, new_salesperson_id);
                    }
                }
            }
            if (NoChanges)
                Messages.ShowInformation("No pending changes to save. Preview changes before saving.");
            else 
                Messages.ShowInformation("Save Completed.");
            int ctr = 0;
            int comma;
            int newSalesID = 0;
            string new_first_name;
            string new_last_name;
            newSalesID = Convert.ToInt32(cmbSalesperson.SelectedValue);
            if (cmbSalesperson.SelectedText.ToString() != "")
                if (gUpload.xGrid.SelectedItems.Records.Count > 0)
                {
                    comma = cmbSalesperson.SelectedText.ToString().IndexOf(",");
                    new_last_name = cmbSalesperson.SelectedText.ToString().Substring(0, comma);
                    new_first_name = cmbSalesperson.SelectedText.ToString().Substring(comma + 2, cmbSalesperson.SelectedText.ToString().Length - (comma + 2));
                    foreach (Record record in gUpload.xGrid.SelectedItems.Records)
                    {
                        ctr++;
                        DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                        if (Convert.ToString(r["title"]) == "PENDING")
                        {
                            r["first_name"] = new_first_name;
                            r["last_name"] = new_last_name;
                            r["title"] = "UPDATED";
                        }
                    }
                    //btnClear.IsEnabled = false;
                    btnSave.IsEnabled = false;
                }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.Load();
            btnClear.IsEnabled = false;
            btnSave.IsEnabled = false;
        }

        public override void Save()
        {
            bool NoChanges = true;
            gUpload.xGrid.ExecuteCommand(Infragistics.Windows.DataPresenter.DataPresenterCommands.EndEditModeAndAcceptChanges);

            Infragistics.Windows.DataPresenter.UpdateMode currentMode = gUpload.xGrid.UpdateMode;

            gUpload.xGrid.UpdateMode = Infragistics.Windows.DataPresenter.UpdateMode.OnUpdate;
            gUpload.xGrid.ExecuteCommand(Infragistics.Windows.DataPresenter.DataPresenterCommands.CommitChangesToAllRecords);

            gUpload.xGrid.UpdateMode = currentMode;

            DataTable salespersonChanges = this.CurrentBusObj.ObjectData.Tables["main"].GetChanges(DataRowState.Modified);
            if (salespersonChanges != null)
            {
                foreach (DataRow row in salespersonChanges.Rows)
                {
                    if (Convert.ToString(row["title"]) == "PENDING")
                    {
                        NoChanges = false;
                        int contract_salesperson_id = Convert.ToInt32(row["contract_salesperson_id"]);
                        int salesperson_id = Convert.ToInt32(row["salesperson_id"]);
                        string first_name = Convert.ToString(row["first_name"]);
                        string last_name = Convert.ToString(row["last_name"]);
                        string title = Convert.ToString(row["title"]);
                        int contract_id = Convert.ToInt32(row["contract_id"]);
                        string contract_description = Convert.ToString(row["contract_description"]);
                        string product_code = Convert.ToString(row["product_code"]);
                        int new_salesperson_id = Convert.ToInt32(row["new_salesperson_id"]);
                        cGlobals.BillService.UpdateSalesPerson(contract_salesperson_id, salesperson_id, first_name, last_name, title, contract_id,
                                                              contract_description, product_code, new_salesperson_id);
                    }
                }
            }
            if (NoChanges)
                Messages.ShowInformation("No pending changes to save. Preview changes before saving.");
            else
                Messages.ShowInformation("Save Completed.");
            int ctr = 0;
            int comma;
            int newSalesID = 0;
            string new_first_name;
            string new_last_name;
            newSalesID = Convert.ToInt32(cmbSalesperson.SelectedValue);
            if (cmbSalesperson.SelectedText.ToString() != "")
                if (gUpload.xGrid.SelectedItems.Records.Count > 0)
                {
                    comma = cmbSalesperson.SelectedText.ToString().IndexOf(",");
                    new_last_name = cmbSalesperson.SelectedText.ToString().Substring(0, comma);
                    new_first_name = cmbSalesperson.SelectedText.ToString().Substring(comma + 2, cmbSalesperson.SelectedText.ToString().Length - (comma + 2));
                    foreach (Record record in gUpload.xGrid.SelectedItems.Records)
                    {
                        ctr++;
                        DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                        if (Convert.ToString(r["title"]) == "PENDING")
                        {
                            r["first_name"] = new_first_name;
                            r["last_name"] = new_last_name;
                            r["title"] = "UPDATED";
                        }
                    }
                    //btnClear.IsEnabled = false;
                    btnSave.IsEnabled = false;
                }
        }

    }

       
}
