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
using RazerInterface;
using System.ComponentModel;
using System.Data;

namespace Razer.CustomInvoice
{
    /// <summary>
    /// Interaction logic for CustomInvoiceGeneral.xaml
    /// </summary>
    public partial class CustomInvoiceGeneral : ScreenBase, IPreBindable
    {
        private static readonly string companyTableName = "company";
        private static readonly string currencyTableName = "currency";
        private static readonly string invoiceTypeTableName = "invoice_type";
        private static readonly string frequencyTableName = "recurring_frequency";
        private static readonly string companyValueField = "company_code";
        private static readonly string companyDisplayField = "company_description";
        private static readonly string currencyValueField = "currency_code";
        private static readonly string descriptionDisplayField = "description";
        private static readonly string invoiceTypeValueField = "invoice_type_id";
        private static readonly string frequencyDisplayField = "code_value";
        private static readonly string frequencyValueField = "fkey_int";        
        private static readonly string mainTableName = "general";
        private static readonly string selectedValueProperty = "SelectedValue";
        private static readonly string failedMessage = "The reprint job failed.";
        private static readonly string successMessage = "Reprint job successfully scheduled to run.";
        public string invoiceNumber = string.Empty;
        
        public int invoiceTypeId;

        public bool FirstTime = false;
        public bool FirstTime2 = false;
        public string sSQL;

        public CustomInvoiceGeneral()
        {
            InitializeComponent();
            this.MainTableName = mainTableName;
        }

        public void PreBind()
        {
            if (CurrentBusObj.HasObjectData)
            {
                cmbCompany.SetBindingExpression(companyValueField, companyDisplayField, CurrentBusObj.ObjectData.Tables[companyTableName]);
                cmbCurrencyCode.SetBindingExpression(currencyValueField, descriptionDisplayField, CurrentBusObj.ObjectData.Tables[currencyTableName]);
                cmbInvoiceType.SetBindingExpression(invoiceTypeValueField, descriptionDisplayField, CurrentBusObj.ObjectData.Tables[invoiceTypeTableName]);
                cmbRecurringFrequency.SetBindingExpression(frequencyValueField, frequencyDisplayField, CurrentBusObj.ObjectData.Tables[frequencyTableName]);
            }
        }

        private void chkRecurring_Checked(object sender, RoutedEventArgs e)
        {
            if (chkRecurring.IsChecked == 1)
            {
                ldpRecurringEndDate.IsEnabled = true;
                cmbRecurringFrequency.IsEnabled = true;
            }
        }

        private void chkRecurring_UnChecked(object sender, RoutedEventArgs e)
        {
            ldpRecurringEndDate.SelText = Convert.ToDateTime("1/1/1900 12:00:00 AM");
            cmbRecurringFrequency.SelectedValue = 0; 
            //cmbRecurringFrequency.SelectedText = "";            
            ldpRecurringEndDate.IsEnabled = false;
            cmbRecurringFrequency.IsEnabled = false;
        }
        private void chkProforma_Checked(object sender, RoutedEventArgs e)
        {
            if (chkProforma.IsChecked == 1)
            {
                
            }
        }

          
        private void chkProforma_Email_Checked(object sender, RoutedEventArgs e)
        {
            if (chkProformaEmail.IsChecked == 1)
            {
                
            }
        }

        private void chkProforma_UnChecked(object sender, RoutedEventArgs e)
        { 
           
        }

        private void chkProforma_Email_UnChecked(object sender, RoutedEventArgs e)
        {

        }


        
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

            if (invoiceNumber != null)
            {

                if (!cGlobals.BillService.ReprintCustomInvoice(invoiceNumber))
                {
                    Messages.ShowInformation(failedMessage);
                }
                else
                {
                     Messages.ShowInformation(successMessage);
                }
            }

            
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void chkConvert_Checked(object sender, RoutedEventArgs e)
        {
            //if (Convert.ToInt32(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["invoice_type"]) == 2)
            //{
            //    Messages.ShowWarning("Invoice has already been converted to previously proforma");
            //}
            if (FirstTime)
            {
                FirstTime = false;
                return;
            }
            if (chkConvert.IsChecked == 1)
            {
                if ((chkClose.IsChecked == 1))
                {
                    Messages.ShowWarning("Close is already requested, cannot request convert");
                    chkConvert.IsChecked = 0;
                    return;
                }
                //CurrentBusObj.LoadTable("prevcheck");
                //if (Convert.ToInt32(CurrentBusObj.ObjectData.Tables["prevcheck"].Rows[0]["count"]) > 0)
                //{
                //    Messages.ShowWarning("Adjustment has been put on hold once.  Cannot put it on hold again.");
                //    FirstTime = true;
                //    chkConvert.IsChecked = 0;
                //    return;
                //}
                MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to add Convert to Previously Proforma comment?", System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //string sSQL;
                    sSQL = "insert dbo.comment select 15,'" + cGlobals.UserName + "',getdate(),'Convert to Previously Proforma','MS',0,'" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["invoice_number"].ToString() +
                    "','Convert Invoice to Previously Proforma'";
                    if (cGlobals.BillService.GenericSQL(sSQL) == true)
                    {
                        Messages.ShowWarning("Convert to Previously Proforma comment added");
                        CurrentBusObj.LoadTable("comments_char");
                    }
                    else
                        Messages.ShowWarning("Error Inserting Comment");
                }
                else
                {
                    FirstTime = true;
                    chkConvert.IsChecked = 0;
                    return;
                }
            }
        }

        private void chkConvert_UnChecked(object sender, RoutedEventArgs e)
        {
            if (FirstTime)
            {
                FirstTime = false;
                return;
            }
            if ((chkConvert.IsChecked != 1) && (chkClose.IsChecked != 1))
            {
                MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to add Do Not Convert to Previously Proforma comment?", System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //string sSQL;
                    sSQL = "insert dbo.comment select 15,'" + cGlobals.UserName + "',getdate(),'Do Not Convert to Previously Proforma','MS',0,'" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["invoice_number"].ToString() +
                    "','Do Not Convert Invoice to Previously Proforma'";
                    if (cGlobals.BillService.GenericSQL(sSQL) == true)
                    {
                        Messages.ShowWarning("Do Not Convert to Previously Proforma comment added");
                        CurrentBusObj.LoadTable("comments_char");
                    }
                    else
                        Messages.ShowWarning("Error Inserting Comment");
                }
                else
                {
                    FirstTime = true;
                    chkConvert.IsChecked = 1;
                    return;
                }
            }
        }

        private void chkClose_Checked(object sender, RoutedEventArgs e)
        {
            if ((chkProforma.IsChecked == 1) || ((Convert.ToInt32(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["invoice_type"]) > 1)))
            {
                Messages.ShowWarning("Proforma Invoice is already closed");
                return;
            }
            if ((chkConvert.IsChecked == 1))
            {
                Messages.ShowWarning("Convert is already requested, cannot request close");
                chkClose.IsChecked = 0;
                return;
            }
            if (FirstTime2)
            {
                FirstTime2 = false;
                return;
            }
            if (chkClose.IsChecked == 1)
            {
                //CurrentBusObj.LoadTable("prevclose");
                //if (Convert.ToInt32(CurrentBusObj.ObjectData.Tables["prevclose"].Rows[0]["count"]) > 0)
                //{
                //    Messages.ShowWarning("Adjustment has been put on hold once.  Cannot put it on hold again.");
                //    FirstTime2 = true;
                //    chkClose.IsChecked = 0;
                //    return;
                //}
                MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to add Close Proforma comment?", System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //string sSQL;
                    sSQL = "insert dbo.comment select 15,'" + cGlobals.UserName + "',getdate(),'Close Proforma Invoice','MS',0,'" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["invoice_number"].ToString() +
                    "','Close Proforma Invoice'";
                    if (cGlobals.BillService.GenericSQL(sSQL) == true)
                    {
                        Messages.ShowWarning("Close Proforma Invocie comment added");
                        CurrentBusObj.LoadTable("comments_char");
                    }
                    else
                        Messages.ShowWarning("Error Inserting Comment");
                }
                else
                {
                    FirstTime2 = true;
                    chkClose.IsChecked = 0;
                    return;
                }
            }
        }

        private void chkClose_UnChecked(object sender, RoutedEventArgs e)
        {
            if (FirstTime2)
            {
                FirstTime2 = false;
                return;
            }
            if ((chkClose.IsChecked != 1) && (chkConvert.IsChecked != 1))
            {
                MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to add Do Not Close Proforma Invoice comment?", System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //string sSQL;
                    sSQL = "insert dbo.comment select 15,'" + cGlobals.UserName + "',getdate(),'Do Not Close Proforma Invoice','MS',0,'" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["invoice_number"].ToString() +
                    "','Do Not Close Proforma Invoice'";
                    if (cGlobals.BillService.GenericSQL(sSQL) == true)
                    {
                        Messages.ShowWarning("Do Not Close Proforma Invocie comment added");
                        CurrentBusObj.LoadTable("comments_char");
                    }
                    else
                        Messages.ShowWarning("Error Inserting Comment");
                }
                else
                {
                    FirstTime2 = true;
                    chkClose.IsChecked = 1;
                    return;
                }
            }
        }


        

        //public void cmbInvoiceType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == selectedValueProperty)
        //    {
        //        if (CurrentBusObj != null && CurrentBusObj.HasObjectData)
        //        {   
        //            switch (invoiceTypeId)
        //            {
        //                case 0:
        //                    var rows = (from r in CurrentBusObj.ObjectData.Tables[invoiceTypeTableName].AsEnumerable()
        //                                where r.Field<int>(invoiceTypeValueField) == 2
        //                                select r).ToList();

        //                    if (rows != null && rows.Count > 0)
        //                    {
        //                        rows[0].Delete();
        //                        rows[0].AcceptChanges();
        //                    }
        //                    break;
        //                case 1:
        //                    var invTypes = (from r in CurrentBusObj.ObjectData.Tables[invoiceTypeTableName].AsEnumerable()
        //                                where r.Field<int>(invoiceTypeValueField) == 1
        //                                select r).ToList();

        //                    if (invTypes != null && invTypes.Count > 0)
        //                    {
        //                        invTypes[0].Delete();
        //                        invTypes[0].AcceptChanges();
        //                    }
        //                    break;
        //                case 2:
        //                     var prevProTypes = (from r in CurrentBusObj.ObjectData.Tables[invoiceTypeTableName].AsEnumerable()
        //                                where r.Field<int>(invoiceTypeValueField) < 2
        //                                select r).ToList();

        //                    if (prevProTypes != null && prevProTypes.Count > 0)
        //                    {
        //                        prevProTypes.ForEach(r => r.Delete());
        //                        prevProTypes.ForEach(r => r.AcceptChanges());
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //}
    }
}
