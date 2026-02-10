using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using RazerBase.Lookups;
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Razer.BatchBilling
{
    /// <summary>
    /// Interaction logic for CreateBatchTab.xaml
    /// </summary>
    public partial class CreateBillingBatch : ScreenBase, IScreen
    {
        private static readonly string mainTableName = "batch";
        private static readonly string detailTableName = "batch_detail";
         
        private static readonly string searchTableName = "contracts";
        //RES 2/20/15 Company Consolidation
        //private static readonly string companyTable = "company";
        //private static readonly string companyValueField = "company_code";
        //private static readonly string companyDisplayField = "company_description";
        private static readonly string producttypeTable = "producttype";
        private static readonly string productTypeAllDisplayPath = "product_type_description";
        private static readonly string productTypeAllValuePath = "product_type";
        private static readonly string contractEntityTable = "contract_entity";
        private static readonly string contractEntityValueField = "mso_id";
        private static readonly string contractEntityDisplayField = "name";
        private static readonly string productsTable = "products";
        private static readonly string productsValueField = "product_code";
        private static readonly string productsDisplayField = "product_description";
        private static readonly string batchParameter = "@batch_id";
        //RES 2/20/15 Company Consolidation
        //private static readonly string companyParameter = "@company_code";
        private static readonly string producttypeParameter = "@product_type";
        private static readonly string entityParameter = "@bill_mso_id";
        private static readonly string productParameter = "@product_code";
        private static readonly string expiredParameter = "@include_expired_flag";
        private static readonly string searchObject = "BatchBillingContract";
        private static readonly string productsObject = "ProductsDropDown";
        //RES 2/20/15 Company Consolidation
        //private static readonly string companyObject = "CompanyDropDown";
        private static readonly string producttypeObject = "ProductTypeDropDown";
        private static readonly string msoObject = "MSODropDown";
        private static readonly string batchLayout = "BatchBillingList";
        private static readonly string contractsLayout = "BatchBillingContractList";
        private static readonly string contractId = "contract_id";
        private static readonly string msoId = "bill_mso_id";
        private static readonly string entityName = "name";
        private static readonly string contractEntityName = "bill_mso_name"; 
        private static readonly string productCode = "product_code";
        private static readonly string statusFlag = "status_flag";

        private cBaseBusObject contractsObject;
        private bool isUpdate = false;
        private string batchId;
        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }

            
        public CreateBillingBatch()
        {
            InitializeComponent();
            this.CancelCloseOnSaveConfirmation = true;
        }

        public void Init(cBaseBusObject businessObject)
        {
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            this.DoNotSetDataContext = false;

            this.CurrentBusObj = businessObject;
            this.MainTableName = mainTableName;
            this.CurrentBusObj.Parms.ClearParms();            
            this.CurrentBusObj.Parms.AddParm(batchParameter, 0);
            //this.Load();
            this.New();
            this.CanExecuteNewCommand = false;
            //needed because audit text box have different table
          
           
            //RES 2/20/15 Company Consolidation
            //cBaseBusObject company = new cBaseBusObject(companyObject);
            //company.LoadData();
            cBaseBusObject ProductType = new cBaseBusObject(producttypeObject);
            ProductType.LoadData();

            cBaseBusObject mso = new cBaseBusObject(msoObject);
            mso.LoadData();

            cBaseBusObject products = new cBaseBusObject(productsObject);
            products.LoadData();
         
            //RES 2/20/15 Company Consolidation
            //cmbRoviCompany.SetBindingExpression(companyValueField, companyDisplayField, company.ObjectData.Tables[companyTable]);
            cmbProductType.SetBindingExpression(productTypeAllValuePath, productTypeAllDisplayPath, ProductType.ObjectData.Tables[producttypeTable]);
            //cmbBillingOwner.SelectedValue = cGlobals.UserName;
            //cmbContractEntity.SetBindingExpression(contractEntityValueField, contractEntityDisplayField, mso.ObjectData.Tables[contractEntityTable]);
            cmbProduct.SetBindingExpression(productsValueField, productsDisplayField, products.ObjectData.Tables[productsTable]);

            FieldLayoutSettings layouts = new FieldLayoutSettings();            
            idgBatchList.xGrid.FieldLayoutSettings = layouts;
            idgBatchList.FieldLayoutResourceString = batchLayout;
            layouts.HighlightAlternateRecords = true;            
            idgBatchList.xGrid.FieldSettings.AllowEdit = false;
            idgBatchList.SetGridSelectionBehavior(true, false);
            idgBatchList.ContextMenuGenericDelegate1  = DeleteBatchDelegate;
            idgBatchList.ContextMenuGenericDisplayName1 = "Delete Unposted Batch";
            idgBatchList.ContextMenuGenericIsVisible1 = true;
            idgBatchList.ContextMenuGenericDelegate2 = ReSubmitBatchDelegate;
            idgBatchList.ContextMenuGenericDisplayName2 = "ReSubmit the Batch";
            idgBatchList.ContextMenuGenericIsVisible2 = true;
            idgBatchList.ContextMenuGenericDelegate3 = PermDeleteBatchDelegate;
            idgBatchList.ContextMenuGenericDisplayName3 = "Permanently Delete Unposted Batch";
            idgBatchList.ContextMenuGenericIsVisible3 = true;
            idgBatchList.ContextMenuGenericDelegate4 = DeleteContractfromBatchDelegate;
            idgBatchList.ContextMenuGenericDisplayName4 = "Delete Contract from Batch";
            idgBatchList.ContextMenuGenericIsVisible4 = true;
            idgBatchList.mWindowZoomDelegate = GridDoubleClicktoContract;
            idgBatchList.MainTableName = detailTableName;
            GridCollection.Add(idgBatchList);

            layouts = new FieldLayoutSettings();

            idgQueryList.xGrid.FieldLayoutSettings = layouts;
            //layouts.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Extended; 
            //use for color coded grid
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            idgQueryList.GridCellValuePresenterStyle = CellStyle;
            idgQueryList.FieldLayoutResourceString = contractsLayout;
            idgQueryList.mWindowZoomDelegate = GridDoubleClickDelegate;
            //layouts.HighlightAlternateRecords = true;
            idgQueryList.xGrid.FieldSettings.AllowEdit = false;
            idgQueryList.SetGridSelectionBehavior(true, true);
        }



        private void txtBatchId_LostFocus(object sender, RoutedEventArgs e)
        {            
            if (!string.IsNullOrEmpty(txtBatchId.Text) && txtBatchId.Text != batchId)
            {
                LoadData(txtBatchId.Text);
            }
        }

        private void txtBatchId_GotFocus(object sender, RoutedEventArgs e)
        {
            batchId = txtBatchId.Text;
        }

        private void txtBatchName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBatchName.Text))
            {
                this.CanExecuteCloseCommand = true;
            }
        }

        public void GridDoubleClickDelegate()
        {
            // performs same logic as pressing Include Selected button
            if (contractsObject != null && contractsObject.HasObjectData)
            {
                DataTable contracts = contractsObject.ObjectData.Tables[searchTableName];
                List<DataRow> rows = new List<DataRow>();

                if (CurrentBusObj.HasObjectData)
                {
                    foreach (DataRecord record in idgQueryList.xGrid.SelectedItems.Records)
                    {
                        DataRow row = ((DataRowView)record.DataItem).Row;

                        //Added this check since click events on grid occasionally included an 
                        //empty record in the SelectedItems.Records collection.
                        if ((int)row[contractId] > 0)
                        {
                            AddBatchListRecord(row);
                            rows.Add(row);
                        }
                    }

                    foreach (DataRow row in rows)
                    {
                        row.Delete();
                    }
                }

                idgBatchList.LoadGrid(CurrentBusObj, detailTableName);
                idgQueryList.LoadGrid(contractsObject, searchTableName);
            }




        }
        public void GridDoubleClicktoContract()
        {
            if (idgBatchList.xGrid.ActiveRecord != null)
            {
                //call customer document folder
                idgBatchList.ReturnSelectedData("contract_id");
                cGlobals.ReturnParms.Add("GridLocationContracts.xGrid");
                RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                args.Source = idgBatchList.xGrid;
                EventAggregator.GeneratedClickHandler(this, args);
            }
        
        }

        public void DeleteBatchDelegate()
        {

            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (txtBatchId.ToString() == "")
            {
                MessageBox.Show("Batch ID is required for the delete to occur");
                txtBatchId.Focus();
                return;
            }
               

            MessageBoxResult result = Messages.ShowYesNo("The Unposted Batch will be deleted to allow you to rerun the batch. Are you sure you want to delete Batch " + txtBatchId.Text.ToString() + "?",
                   System.Windows.MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                
               
               
                     
                int CurrentBatchID = Convert.ToInt32( txtBatchId.Text);
                    //Ask if they are sure they want to delete that batch

                   
                        if (cGlobals.BillService.DeleteBatch(CurrentBatchID) == true)
                        {
                            Messages.ShowWarning("Batch ID " + CurrentBatchID.ToString() + " Deleted");
                            btnScheduleJob.IsEnabled = false;
                            this.ClearafterDelete();
                        }
                        else
                            Messages.ShowWarning("Error Deleting Batch");
                    }
                    else
                 

                        Messages.ShowMessage("Batch not deleted", MessageBoxImage.Information);
                 
             
                 
         

   


        }

        public void PermDeleteBatchDelegate()
        {

            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (txtBatchId.ToString() == "")
            {
                MessageBox.Show("Batch ID is required for the delete to occur");
                txtBatchId.Focus();
                return;
            }


            MessageBoxResult result = Messages.ShowYesNo("The Unposted Batch will be permanently deleted. Are you sure you want to delete Batch " + txtBatchId.Text.ToString() + "?",
                   System.Windows.MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {




                int CurrentBatchID = Convert.ToInt32(txtBatchId.Text);
                //Ask if they are sure they want to delete that batch


                if (cGlobals.BillService.PermDeleteBatch(CurrentBatchID) == true)
                {
                    Messages.ShowWarning("Batch ID " + CurrentBatchID.ToString() + " Permanently Deleted");
                    btnScheduleJob.IsEnabled = false;
                    this.ClearafterDelete();
                     
                }
                else
                    Messages.ShowWarning("Error Deleting Batch");
            }
            else


                Messages.ShowMessage("Batch not deleted", MessageBoxImage.Information);








        }


        public void ReSubmitBatchDelegate()
        {



            string jobName = "BILBATCH";
            StringBuilder sbParameters = new StringBuilder(@"/A");
            sbParameters.Append(" ");
            sbParameters.Append(txtBatchId.Text);

            if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, sbParameters.ToString(), DateTime.Now, cGlobals.UserName))
            {
                Messages.ShowWarning("Job Scheduled to Run");
                btnScheduleJob.IsEnabled = false;
                this.ClearafterDelete();
                  
                //CLB Do not close screen per user request
                //this.CallScreenClose();
            }
            else
            {
                Messages.ShowWarning("Error Scheduling Job");
            }



        }
        public void DeleteContractfromBatchDelegate()
        {
            string sContractID;
            int batchID = 0;
            int contractID = 0;
            int statusBatch = 0;

            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (CurrentBusObj.HasObjectData)
            {
                if (this.CurrentBusObj.ObjectData.Tables["batch_detail"].Rows.Count > 0)
                {
                    if (idgBatchList.xGrid.SelectedItems.Count() != 0)
                    {
                        batchID = Convert.ToInt32(txtBatchId.Text);
                        foreach (Record record in idgBatchList.xGrid.SelectedItems.Records)
                        {

                            DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                            contractID = Convert.ToInt32(r["contract_id"]);
                            statusBatch = Convert.ToInt32(r["status_flag"]);
                        }
                        if (statusBatch == 0)
                        {
                            if (contractID > 0)
                            {
                                MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to delete contract ID " + contractID.ToString() + " from Batch ID " + batchID.ToString() + "?", System.Windows.MessageBoxImage.Question);


                                if (result == MessageBoxResult.Yes)
                                {



                                    if (cGlobals.BillService.DeleteContractfromCreateBatch(batchID, contractID) == true)
                                    {
                                        Messages.ShowWarning("Contract ID " + contractID.ToString() + " Deleted");
                                        this.CurrentBusObj.Parms.ClearParms();
                                        CurrentBusObj.Parms.AddParm(batchParameter, batchID);
                                        this.Load("batch_detail");
                                    }
                                }

                                else


                                    Messages.ShowMessage("Batch not deleted", MessageBoxImage.Information);

                            }
                            else
                            {
                                Messages.ShowWarning("A contract must be selected for a delete to occur");
                                return;
                            }
                        }
                            else
                        {

                                Messages.ShowWarning("Batch can only have a status of Ready to Process for contract delete to occur");
                                return;
                        }


                        
                    }
                    else
                    {
                        Messages.ShowWarning("A contract must be selected for a delete to occur");
                        return;
                    }

                }
           
            }

        }



        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {            
            contractsObject = new cBaseBusObject(searchObject);

            //RES 2/20/15 Company Consolidation
            //contractsObject.Parms.AddParm(companyParameter, cmbRoviCompany.SelectedValue ?? string.Empty);
            contractsObject.Parms.AddParm(producttypeParameter, cmbProductType.SelectedValue ?? "-1");
            string lsdebug;
            lsdebug = txtContractEntity.Text;
            if (lsdebug == "")
                contractsObject.Parms.AddParm(entityParameter,"0");
            else

            contractsObject.Parms.AddParm(entityParameter, txtContractEntity.Text ?? "0");
            contractsObject.Parms.AddParm(productParameter, cmbProduct.SelectedValue ?? string.Empty);
            contractsObject.Parms.AddParm(expiredParameter, chkIncludeExpired.IsChecked);

            contractsObject.LoadData();

            if (CurrentBusObj.HasObjectData && contractsObject.HasObjectData)
            {
                foreach (DataRow row in  contractsObject.ObjectData.Tables[searchTableName].Rows)
                {
                    var rows = (from r in CurrentBusObj.ObjectData.Tables[detailTableName].AsEnumerable()
                                where r.Field<int>(contractId) == row.Field<int>(contractId)
                                select r).ToList();
                    if (rows != null && (rows.Count > 0))
                    {
                        row.Delete();
                    }
                }

                contractsObject.ObjectData.Tables[searchTableName].AcceptChanges();
            }

            idgQueryList.LoadGrid(contractsObject, searchTableName);
        }

        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            if (contractsObject != null && contractsObject.HasObjectData)
            {
                DataTable contracts = contractsObject.ObjectData.Tables[searchTableName];

                if (CurrentBusObj.HasObjectData)
                {
                    foreach (DataRow row in contracts.Rows)
                    {
                        AddBatchListRecord(row);
                    }
                    contracts.Clear();
                }

                idgBatchList.LoadGrid(CurrentBusObj, detailTableName);
                idgQueryList.LoadGrid(contractsObject, searchTableName);
            }
        }

        private void AddBatchListRecord(DataRow row)
        {
            DataRow listRow = CurrentBusObj.ObjectData.Tables[detailTableName].NewRow();
            listRow[contractId] = row[contractId];
            listRow[entityName] = row[contractEntityName];
            listRow[productCode] = row[productCode];
            listRow[msoId] = row[msoId];
            listRow[statusFlag] = 0;

            CurrentBusObj.ObjectData.Tables[detailTableName].Rows.Add(listRow);
        }

        private void btnSelected_Click(object sender, RoutedEventArgs e)
        {
            if (contractsObject != null && contractsObject.HasObjectData)
            {
                DataTable contracts = contractsObject.ObjectData.Tables[searchTableName];
                List<DataRow> rows = new List<DataRow>();

                if (CurrentBusObj.HasObjectData)
                {
                    foreach (DataRecord record in idgQueryList.xGrid.SelectedItems.Records)
                    {
                        DataRow row = ((DataRowView)record.DataItem).Row;

                        //Added this check since click events on grid occasionally included an 
                        //empty record in the SelectedItems.Records collection.
                        if ((int)row[contractId] > 0)
                        {
                            AddBatchListRecord(row);
                            rows.Add(row);
                        }
                    }

                    foreach (DataRow row in rows)
                    {
                        row.Delete();
                    }
                }

                idgBatchList.LoadGrid(CurrentBusObj, detailTableName);
                idgQueryList.LoadGrid(contractsObject, searchTableName);
            }
        }


        private void ClearafterDelete()
        {
            btnAll.IsEnabled = true;
            btnAll.ToolTip = string.Empty;
            btnSelected.IsEnabled = true;
            btnSelected.ToolTip = string.Empty;

            if (CurrentBusObj != null && CurrentBusObj.HasObjectData)
            {
                CurrentBusObj.ObjectData.Tables[mainTableName].Clear();
                CurrentBusObj.ObjectData.Tables[detailTableName].Clear();
                
                this.CurrentBusObj.Parms.ClearParms();
                this.CurrentBusObj.Parms.AddParm(batchParameter, 0);
                this.New();
            }
            if (contractsObject != null && contractsObject.HasObjectData)
            {
                contractsObject.ObjectData.Tables[searchTableName].Reset();
            }

            cmbProduct.SelectedValue = null;
            //RES 2/20/15 Company Consolidation
            //cmbRoviCompany.SelectedValue = null;
            cmbProductType.SelectedValue = null;
            txtContractEntity.Text = null;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            btnAll.IsEnabled = true;
            btnAll.ToolTip = string.Empty;
            btnSelected.IsEnabled = true;
            btnSelected.ToolTip = string.Empty;

            if (CurrentBusObj != null && CurrentBusObj.HasObjectData)
            {
                CurrentBusObj.ObjectData.Tables[mainTableName].Clear();
                CurrentBusObj.ObjectData.Tables[detailTableName].Clear();
               
                this.CurrentBusObj.Parms.ClearParms();
                this.CurrentBusObj.Parms.AddParm(batchParameter, 0);
                this.New();
            }
            if (contractsObject != null && contractsObject.HasObjectData)
            {
                contractsObject.ObjectData.Tables[searchTableName].Reset();
            }

            cmbProduct.SelectedValue = null;
            //RES 2/20/15 Company Consolidation
            //cmbRoviCompany.SelectedValue = null;
            cmbProductType.SelectedValue = null;
            txtContractEntity.Text = null;
        }

        public override void Save()
        {
            this.PrepareFreeformForSave();
            if (string.IsNullOrEmpty(txtBatchName.Text))
            {                
                Messages.ShowWarning("You must enter a valid batch name.");
               
            }
            else
            {   if (txtCreatedBy.Text == "")
                 {
                     txtCreatedBy.Text = cGlobals.UserName;
                     txtCreated.Text = DateTime.Now.ToString();
                  }
                if (isUpdate)
                {
                    int id;
                    if (int.TryParse(txtBatchId.Text, out id))
                    {
                        CurrentBusObj.Parms.AddParm(batchParameter, id);
                    }
                }
                else
                {
                    CurrentBusObj.Parms.AddParm(batchParameter, -1);
                }

                base.Save();
                Messages.ShowInformation("Save successful.");
                btnScheduleJob.IsEnabled = true;              
              
                 
            }
        }

        private void LoadData(string id)
        {            
            this.CurrentBusObj.Parms.ClearParms();            
            this.CurrentBusObj.Parms.AddParm(batchParameter, id);

            if (this.CurrentBusObj != null)
            {
                base.Load();
                if (this.CurrentBusObj.ObjectData.Tables[mainTableName].Rows.Count > 0)
                {
                    if ((int)this.CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][statusFlag] > 0)
                    {
                        string tip = "The batch is not editable.  Choose clear to continue with a different item.";
                        btnAll.IsEnabled = false;
                        btnAll.ToolTip = tip;
                        btnSelected.IsEnabled = false;
                        btnSelected.ToolTip = tip;

                        btnScheduleJob.IsEnabled = false;
                    }
                    else
                    {
                        btnScheduleJob.IsEnabled = true;
                        if (contractsObject != null && contractsObject.HasObjectData)
                        {
                            contractsObject.ObjectData.Tables[searchTableName].Clear();
                        }
                       
                        isUpdate = true;
                    }
                }
            }
        }

        private void txtBatchId_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = EventAggregator.GeneratedClickEvent;
            args.Source = txtBatchId;
            EventAggregator.GeneratedClickHandler(this, args);

            if (cGlobals.ReturnParms.Count > 0)
            {
                string id = cGlobals.ReturnParms[0].ToString();
                LoadData(id);
                cGlobals.ReturnParms.Clear();
            }
        }

        private void btnScheduleJob_Click(object sender, RoutedEventArgs e)
        {
            string jobName = "BILBATCH";
            StringBuilder sbParameters = new StringBuilder(@"/A");
            sbParameters.Append(" ");
            sbParameters.Append(txtBatchId.Text);

            if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, sbParameters.ToString(), DateTime.Now, cGlobals.UserName))
            {
                Messages.ShowWarning("Job Scheduled to Run");
                btnScheduleJob.IsEnabled = false;
                this.ClearafterDelete();
                //CLB Do not close screen per user request
                //this.CallScreenClose();
            }
            else
            {
                Messages.ShowWarning("Error Scheduling Job");
            }
        }

        private void txtContractEntity_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on Entity ID field
            EntityLookup f = new EntityLookup();


            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {

                txtContractEntity.Text = cGlobals.ReturnParms[0].ToString();

                cGlobals.ReturnParms.Clear();
            }
        }

       
    }
}
