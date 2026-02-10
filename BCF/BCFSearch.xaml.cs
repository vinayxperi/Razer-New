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

namespace BCF
{
    /// <summary>
    /// Interaction logic for BCFSearch.xaml
    /// </summary>
    public partial class BCFSearch : ScreenBase, IScreen
    {
        private static readonly string bcfSearchTable = "bcf_search";
        private static readonly string bcfSearchLayout = "BCFLookupGrid";
        private static readonly string contractId = "@contract_id";
        private static readonly string customerNumber = "@customer_number";
        private static readonly string locationId = "@location_id";
        private static readonly string contractDescription = "@contract_description";
        private static readonly string customerName = "@customer_name";
        private static readonly string bcfDescription = "@bcf_description";
        private static readonly string productCode = "@product_code";

        public BCFSearch(cBaseBusObject searchObj)
        {
            InitializeComponent();

            Init(searchObj);
        }

        System.Windows.Window searchScreenWindow;

        public void Init(cBaseBusObject businessObject)
        {
            this.CurrentBusObj = businessObject;
            idgBCFSearchGrid.WindowZoomDelegate = ReturnSelectedData;

            searchScreenWindow = new System.Windows.Window();
            searchScreenWindow.Title = "BCF Search";
            //set rules screen as content of new window
            searchScreenWindow.Content = this;

            //open new window with embedded user control
            searchScreenWindow.ShowDialog();


        }

        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            CurrentBusObj = new cBaseBusObject(this.CurrentBusObj.BusObjectName);

            CurrentBusObj.Parms.AddParm(contractId, txtContractId.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(customerNumber, txtCustomerNumber.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(locationId, txtLocationId.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(contractDescription, txtContractDescription.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(customerName, txtCustomerName.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(bcfDescription, txtBCFDescription.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(productCode, txtProductCode.Text ?? string.Empty);


            this.MainTableName = bcfSearchTable;
            idgBCFSearchGrid.FieldLayoutResourceString = bcfSearchLayout;

            FieldLayoutSettings layouts = new FieldLayoutSettings();

            idgBCFSearchGrid.xGrid.FieldLayoutSettings = layouts;
            layouts.HighlightAlternateRecords = true;
            idgBCFSearchGrid.xGrid.FieldSettings.AllowEdit = false;
            idgBCFSearchGrid.SetGridSelectionBehavior(true, false);

            this.Load(CurrentBusObj);

            if (CurrentBusObj.HasObjectData)
            {
                idgBCFSearchGrid.LoadGrid(CurrentBusObj, this.MainTableName);

                if (CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows.Count > 0)
                    btnOk.IsEnabled = true;
                else
                    btnOk.IsEnabled = false;
            }
            else
            {
                btnOk.IsEnabled = false;
            }
        }

        public void ReturnSelectedData()
        {
            idgBCFSearchGrid.ReturnSelectedData("bcf_number");

            CloseWindow();
            //searchScreenWindow.Close();

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtContractId.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            txtContractDescription.Text = string.Empty;
            txtCustomerNumber.Text = string.Empty;
            txtLocationId.Text = string.Empty;
            txtBCFDescription.Text = string.Empty;
            txtProductCode.Text = string.Empty;

            idgBCFSearchGrid.xGrid.DataSource = null;

            btnOk.IsEnabled = false;


        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            ReturnSelectedData();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            Window w = Window.GetWindow(this);
            if (w != null)
            {
                w.Close();
            }
        }

        //private void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    //if (e.Key == Key.Enter)
        //    //{
        //    //    var sourceTextBox = sender as ucLabelTextBox;
        //    //    if (sourceTextBox == null) return;

        //    //    string textToPush = sourceTextBox.Text;

        //    //    // Determine target textbox based on source
        //    //    ucLabelTextBox targetTextBox = null;

        //    //    if (sourceTextBox == txtContractId)
        //    //        targetTextBox = txtContractId;
        //    //    else if (sourceTextBox == txtContractDescription)
        //    //        targetTextBox = txtContractDescription;
        //    //    else if (sourceTextBox == txtCustomerNumber)
        //    //        targetTextBox = txtCustomerNumber;
        //    //    else if (sourceTextBox == txtCustomerName)
        //    //        targetTextBox = txtCustomerName;
        //    //    else if (sourceTextBox == txtLocationId)
        //    //        targetTextBox = txtLocationId;

        //    //    if (targetTextBox != null)
        //    //    {
        //    //        targetTextBox.Text = textToPush;
        //    //        targetTextBox.Focus(); // Optional: move focus
        //    //    }

        //    //    e.Handled = true; // Prevent default Enter behavior
        //    //}
        //}

        //private void Txt_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {

        //        //ucLabelTextBox sourceTextBox = sender as ucLabelTextBox;
        //        //if (sourceTextBox == null) return;

        //        //string textToPush = sourceTextBox.Text;

        //        //// Determine target textbox based on source
        //        //ucLabelTextBox targetTextBox = null;

        //        //if (sourceTextBox == txtContractId)
        //        //    targetTextBox = txtContractId;
        //        //else if (sourceTextBox == txtContractDescription)
        //        //    targetTextBox = txtContractDescription;
        //        //else if (sourceTextBox == txtCustomerNumber)
        //        //    targetTextBox = txtCustomerNumber;
        //        //else if (sourceTextBox == txtCustomerName)
        //        //    targetTextBox = txtCustomerName;
        //        //else if (sourceTextBox == txtLocationId)
        //        //    targetTextBox = txtLocationId;

        //        //if (targetTextBox != null)
        //        //{
        //        //    targetTextBox.Text = textToPush;
        //        //    targetTextBox.Focus(); // Optional: move focus
        //        //}


        //        btnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        //        e.Handled = true;
        //    }

        //}
    }
}

