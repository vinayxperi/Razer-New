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
    /// Interaction logic for RemitTo.xaml
    /// </summary>
    public partial class RemitTo : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "RemitTo";
        private static readonly string mainTableName = "remit_to";
        //Needed for the country combobox
        private static readonly string countryTableName = "country";
        private static readonly string countryDisplayPath = "country";
        private static readonly string countryValuePath = "country_id";
        //Needed for  state combobox
        private static readonly string stateTableName = "state";
        private static readonly string stateDisplayPath = "description";
        private static readonly string stateValuePath = "state";
        //Needed for province combobox
        private static readonly string provinceTableName = "province";
        private static readonly string provinceDisplayPath = "province";
        private static readonly string provinceValuePath = "province";
        //Needed for bank combobox
        private static readonly string bankTableName = "bank";
        private static readonly string bankDisplayPath = "bank_name";
        private static readonly string bankValuePath = "bank_id";
        //Needed for a combobox
        public ComboBoxItemsProvider cmbCountry { get; set; }
        public ComboBoxItemsProvider cmbState { get; set; }
        public ComboBoxItemsProvider cmbProvince { get; set; }
        public ComboBoxItemsProvider cmbBank { get; set; }



        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public RemitTo()
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
            idgRemitTo.xGrid.FieldLayoutSettings = layouts;
            idgRemitTo.FieldLayoutResourceString = fieldLayoutResource;
            idgRemitTo.MainTableName = mainTableName;
            idgRemitTo.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load country combobox
            if (businessObject.HasObjectData)
            {
                cmbCountry = new ComboBoxItemsProvider();
                cmbCountry.ItemsSource = businessObject.ObjectData.Tables[countryTableName].DefaultView;
                cmbCountry.ValuePath = countryValuePath;
                cmbCountry.DisplayMemberPath = countryDisplayPath;
            }
           
            //load state combobox
            if (businessObject.HasObjectData)
            {
                cmbState = new ComboBoxItemsProvider();
                cmbState.ItemsSource = businessObject.ObjectData.Tables[stateTableName].DefaultView;
                cmbState.ValuePath = stateValuePath;
                cmbState.DisplayMemberPath = stateDisplayPath;
            }

            //load province combobox
            if (businessObject.HasObjectData)
            {
                cmbProvince = new ComboBoxItemsProvider();
                cmbProvince.ItemsSource = businessObject.ObjectData.Tables[provinceTableName].DefaultView;
                cmbProvince.ValuePath = provinceValuePath;
                cmbProvince.DisplayMemberPath = provinceDisplayPath;
            }


            //load bank combobox
            if (businessObject.HasObjectData)
            {
                cmbBank = new ComboBoxItemsProvider();
                cmbBank.ItemsSource = businessObject.ObjectData.Tables[bankTableName].DefaultView;
                cmbBank.ValuePath = bankValuePath;
                cmbBank.DisplayMemberPath = bankDisplayPath;
            }
            idgRemitTo.LoadGrid(businessObject, idgRemitTo.MainTableName);
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
