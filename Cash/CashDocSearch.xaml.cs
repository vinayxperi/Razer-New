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
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Cash
{
    /// <summary>
    /// Interaction logic for NationalAdsSearch.xaml
    /// </summary>
    public partial class CashDocSearch : ScreenBase, IScreen
    {
        private static readonly string CashDocSearchTable = "CashDocumentLookup";    
        private static readonly string customerId = "@customer_id";
        private static readonly string accountName = "@account_name";
        private static readonly string RemitFromDate = "@remit_date_from";
        private static readonly string RemitToDate = "@remit_date_to";        
        private static readonly string RemitFromAmount = "@remit_amount_from";
        private static readonly string RemitToAmount = "@remit_amount_to";
        private static readonly string RemitNumber = "@remit_number";
        public string WindowCaption { get { return string.Empty; } }        

        public CashDocSearch()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            this.CurrentBusObj = businessObject;          
            idgCashDocSearch.WindowZoomDelegate = GridDoubleClickDelegate;           
        }        

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            CurrentBusObj = new cBaseBusObject(this.CurrentBusObj.BusObjectName);
            CurrentBusObj.Parms.AddParm(customerId, txtReceivableAccount.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(accountName, txtAccountName.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(RemitNumber, txtRemitNumber.Text ?? string.Empty);
           
            if (ldpRemitFrom.SelText != null)
                CurrentBusObj.Parms.AddParm(RemitFromDate, ldpRemitFrom.SelText);            
            else            
                CurrentBusObj.Parms.AddParm(RemitFromDate, new DateTime(1900, 1, 1));

            if ((ldpRemitTo.SelText != null) && (ldpRemitTo.SelText.ToString() != "1/1/1900 12:00:00 AM"))
                CurrentBusObj.Parms.AddParm(RemitToDate, ldpRemitTo.SelText);
            else
                CurrentBusObj.Parms.AddParm(RemitToDate, new DateTime(9999, 1, 1));

            string sRemitAmtFrom = UnformatTextField(txtRemitAmtFrom.Text.Trim());

            if ((sRemitAmtFrom != null) && (sRemitAmtFrom != ""))
                CurrentBusObj.Parms.AddParm(RemitFromAmount, sRemitAmtFrom);
            else
                CurrentBusObj.Parms.AddParm(RemitFromAmount, "0.00");

            string sRemitAmtTo = UnformatTextField(txtRemitAmtTo.Text.Trim());

            if ((sRemitAmtTo != null) && (sRemitAmtTo != "") && (sRemitAmtTo != "0"))
                CurrentBusObj.Parms.AddParm(RemitToAmount, sRemitAmtTo);
            else
                CurrentBusObj.Parms.AddParm(RemitToAmount, "9999999999.00");                       

            
            this.MainTableName = "CashDocumentLookup";
            idgCashDocSearch.FieldLayoutResourceString = "viewCashLookup";

            FieldLayoutSettings layouts = new FieldLayoutSettings();
            idgCashDocSearch.xGrid.FieldLayoutSettings = layouts;
            layouts.HighlightAlternateRecords = true;
            idgCashDocSearch.xGrid.FieldSettings.AllowEdit = false;
            idgCashDocSearch.SetGridSelectionBehavior(true, false);
            
            this.Load(CurrentBusObj);

            if (CurrentBusObj.HasObjectData)
            {                
                idgCashDocSearch.LoadGrid(CurrentBusObj, this.MainTableName);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentBusObj.HasObjectData)
                this.CurrentBusObj.changeParm(customerId,"ZZZZZZZZZ");
            //else
            //    CurrentBusObj.Parms.AddParm(customerId, txtReceivableAccount.Text ?? string.Empty);
            this.CurrentBusObj.LoadData(this.MainTableName);
            idgCashDocSearch.LoadGrid(this.CurrentBusObj, this.MainTableName);
            txtReceivableAccount.Text = string.Empty;
            txtAccountName.Text = string.Empty;
            ldpRemitFrom.SelText = null;
            ldpRemitTo.SelText = null;
            txtRemitAmtFrom.Text = "";
            txtRemitAmtTo.Text = "";
            txtRemitNumber.Text = string.Empty;
        }

        public void GridDoubleClickDelegate()
        {
            idgCashDocSearch.ReturnSelectedData("document_id");
            cGlobals.ReturnParms.Add(idgCashDocSearch.Name);
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = idgCashDocSearch.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);
        }

        private void txtRemitAmtFrom_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtRemitAmtFrom.Text == "") return;
            txtSelectAll(sender);            
            txtRemitAmtFrom.Text = txtRemitAmtFrom.Text.Replace("$", "");
            txtRemitAmtFrom.Text = txtRemitAmtFrom.Text.Replace(",", "");
            txtRemitAmtFrom.Text = txtRemitAmtFrom.Text.Replace("(", "-");
            txtRemitAmtFrom.Text = txtRemitAmtFrom.Text.Replace(")", "");
        }        

        private void txtRemitAmtFrom_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtRemitAmtFrom.Text == "") return;
            //Double result = 0;
            //if (Double.TryParse(txtRemitAmtFrom.Text, out result) == false)
            //{
            //    txtRemitAmtFrom.Text = "0.00";
            //}
            //set a default value if user skips
            if (txtRemitAmtFrom.Text == "") txtRemitAmtFrom.Text = "0.00";
            //convert the value to a double and format to add trailing zeros if missing
            double formatAmt = Convert.ToDouble(txtRemitAmtFrom.Text);
            txtRemitAmtFrom.Text = formatAmt.ToString("0.00");
            txtRemitAmtFrom.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRemitAmtFrom.Text));            
        }

        private void txtRemitAmtTo_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtRemitAmtTo.Text == "") return;
            txtSelectAll(sender);
            txtRemitAmtTo.Text = txtRemitAmtTo.Text.Replace("$", "");
            txtRemitAmtTo.Text = txtRemitAmtTo.Text.Replace(",", "");
            txtRemitAmtTo.Text = txtRemitAmtTo.Text.Replace("(", "-");
            txtRemitAmtTo.Text = txtRemitAmtTo.Text.Replace(")", "");
        }

        private void txtRemitAmtTo_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {

            if (txtRemitAmtTo.Text == "") return;
            //Double result = 0;
            //if (Double.TryParse(txtRemitAmtTo.Text, out result) == false)
            //{
            //    txtRemitAmtTo.Text = "9999999999.00";
            //}
            //set a default value if user skips
            if (txtRemitAmtTo.Text == "") txtRemitAmtTo.Text = "9999999999.00";
            //convert the value to a double and format to add trailing zeros if missing
            double formatAmt = Convert.ToDouble(txtRemitAmtTo.Text);
            txtRemitAmtTo.Text = formatAmt.ToString("0.00");
            txtRemitAmtTo.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRemitAmtTo.Text));
        }

        private void txtSelectAll(object sender)
        {
            if (sender is RazerBase.ucLabelTextBox)
                (sender as RazerBase.ucLabelTextBox).SelectAll();
        }

        private string UnformatTextField(string FormattedTextField)
        {
            if (FormattedTextField == null || FormattedTextField == "") return "0";

            string sUnformattedTextField = FormattedTextField.Replace("$", "");
            sUnformattedTextField = sUnformattedTextField.Replace(",", "");
            sUnformattedTextField = sUnformattedTextField.Replace("(", "-");
            sUnformattedTextField = sUnformattedTextField.Replace(")", "");

            return sUnformattedTextField;
        }
    }
}
