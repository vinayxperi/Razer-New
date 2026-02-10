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
    /// Interaction logic for CustAttr.xaml
    /// </summary>
    public partial class CustAttr : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "CustAttr";
        private static readonly string mainTableName = "customer_attribute";
        //Needed for the combobox
        private static readonly string attributeTableName = "attribute";
        private static readonly string attributeDisplayPath = "attribute";
        private static readonly string attributeValuePath = "attribute";
        private static readonly string customerTableName = "recv_account";
        private static readonly string customerDisplayPath = "account_name";
        private static readonly string customerValuePath = "receivable_account";
                        
        //Needed for a combobox
        public ComboBoxItemsProvider cmbAttribute { get; set; }
        public ComboBoxItemsProvider cmbCustomer { get; set; }
      
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public CustAttr()
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
            idgCustAttr.xGrid.FieldLayoutSettings = layouts;
            idgCustAttr.FieldLayoutResourceString = fieldLayoutResource;
            idgCustAttr.MainTableName = mainTableName;
            idgCustAttr.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load attribute combobox
            if (businessObject.HasObjectData)
            {
                cmbAttribute = new ComboBoxItemsProvider();
                cmbAttribute.ItemsSource = businessObject.ObjectData.Tables[attributeTableName].DefaultView;
                cmbAttribute.ValuePath = attributeValuePath;
                cmbAttribute.DisplayMemberPath = attributeDisplayPath;
            }
            //load customer combobox
            if (businessObject.HasObjectData)
            {
                cmbCustomer = new ComboBoxItemsProvider();
                cmbCustomer.ItemsSource = businessObject.ObjectData.Tables[customerTableName].DefaultView;
                cmbCustomer.ValuePath = customerValuePath;
                cmbCustomer.DisplayMemberPath = customerDisplayPath;
            }
            

            if (businessObject.HasObjectData)
            {
                //cmbCurrency = new ComboBoxItemsProvider();
                //cmbCurrency.ItemsSource = businessObject.ObjectData.Tables[bankCurrencyTableName].DefaultView;
                //cmbCurrency.ValuePath = currencyValuePath;
                //cmbCurrency.DisplayMemberPath = currencyDisplayPath;
            }

            idgCustAttr.LoadGrid(businessObject, idgCustAttr.MainTableName);
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
