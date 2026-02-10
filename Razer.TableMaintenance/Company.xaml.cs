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
    /// Interaction logic for Bank.xaml
    /// </summary>
    public partial class Company : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "CompanyMaintenance";
        private static readonly string mainTableName = "company";
        //Needed for the combobox
        private static readonly string bankCurrencyTableName = "bank_currency";
        private static readonly string currencyDisplayPath = "description";
        private static readonly string currencyValuePath = "currency_code";
        
        //Needed for a combobox
        public ComboBoxItemsProvider cmbCurrency { get; set; }


        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public Company()
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

            //if (businessObject.HasObjectData)
            //{
            //    cmbCurrency = new ComboBoxItemsProvider();
            //    cmbCurrency.ItemsSource = businessObject.ObjectData.Tables[bankCurrencyTableName].DefaultView;
            //    cmbCurrency.ValuePath = currencyValuePath;
            //    cmbCurrency.DisplayMemberPath = currencyDisplayPath;
            //}

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
