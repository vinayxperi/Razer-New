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
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;


namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for CompanyMaint.xaml
    /// </summary>
    public partial class CompanyMaint : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "Company";
        private static readonly string mainTableName = "company";
        //Needed for the combobox
        private static readonly string remitTableName = "companyremit";
        private static readonly string remitDisplayPath = "remit_to_name";
        private static readonly string remitValuePath = "remit_to_id";
        private static readonly string stateTableName = "companystate";
        private static readonly string stateDisplayPath = "description";
        private static readonly string stateValuePath = "state";
        private static readonly string countryTableName = "companycountry";
        private static readonly string countryDisplayPath = "country";
        private static readonly string countryValuePath = "country_id";
        private static readonly string sobTableName = "companysob";
        private static readonly string sobDisplayPath = "name";
        private static readonly string sobValuePath = "sob_id";
        private static readonly string currencyTableName = "currency";
        private static readonly string currencyDisplayPath = "description";
        private static readonly string currencyValuePath = "currency_code";
                
        //Needed for a combobox
        public ComboBoxItemsProvider cmbRemitID { get; set; }
        public ComboBoxItemsProvider cmbState { get; set; }
        public ComboBoxItemsProvider cmbCountry { get; set; }
        public ComboBoxItemsProvider cmbSOB { get; set; }
        public ComboBoxItemsProvider cmbCurrency { get; set; }


        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public CompanyMaint()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            //Adds the insert row at the top
            layouts.AllowAddNew = true;
            layouts.AddNewRecordLocation = AddNewRecordLocation.OnTop;

            this.CurrentBusObj = businessObject;

            this.MainTableName = mainTableName;
            idgCompany.xGrid.FieldLayoutSettings = layouts;
            idgCompany.FieldLayoutResourceString = fieldLayoutResource;
            idgCompany.MainTableName = mainTableName;
            idgCompany.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load remit to combobox
            if (businessObject.HasObjectData)
            {
                cmbRemitID = new ComboBoxItemsProvider();
                cmbRemitID.ItemsSource = businessObject.ObjectData.Tables[remitTableName].DefaultView;
                cmbRemitID.ValuePath = remitValuePath;
                cmbRemitID.DisplayMemberPath = remitDisplayPath;
            }
            //load state combobox
            if (businessObject.HasObjectData)
            {
                cmbState = new ComboBoxItemsProvider();
                cmbState.ItemsSource = businessObject.ObjectData.Tables[stateTableName].DefaultView;
                cmbState.ValuePath = stateValuePath;
                cmbState.DisplayMemberPath = stateDisplayPath;
            }
            //load country combobox
            if (businessObject.HasObjectData)
            {
                cmbCountry = new ComboBoxItemsProvider();
                cmbCountry.ItemsSource = businessObject.ObjectData.Tables[countryTableName].DefaultView;
                cmbCountry.ValuePath = countryValuePath;
                cmbCountry.DisplayMemberPath = countryDisplayPath;
            }
            //load sob combobox
            if (businessObject.HasObjectData)
            {
                cmbSOB = new ComboBoxItemsProvider();
                cmbSOB.ItemsSource = businessObject.ObjectData.Tables[sobTableName].DefaultView;
                cmbSOB.ValuePath = sobValuePath;
                cmbSOB.DisplayMemberPath = sobDisplayPath;
            }
            //load currency combobox
            if (businessObject.HasObjectData)
            {
                cmbCurrency = new ComboBoxItemsProvider();
                cmbCurrency.ItemsSource = businessObject.ObjectData.Tables[currencyTableName].DefaultView;
                cmbCurrency.ValuePath = currencyValuePath;
                cmbCurrency.DisplayMemberPath = currencyDisplayPath;
            }

            if (businessObject.HasObjectData)
            {
                //cmbCurrency = new ComboBoxItemsProvider();
                //cmbCurrency.ItemsSource = businessObject.ObjectData.Tables[bankCurrencyTableName].DefaultView;
                //cmbCurrency.ValuePath = currencyValuePath;
                //cmbCurrency.DisplayMemberPath = currencyDisplayPath;
            }

            idgCompany.LoadGrid(businessObject, idgCompany.MainTableName);
        }

        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {
               
                Messages.ShowInformation("Save Successful");
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }


       
    }
}
