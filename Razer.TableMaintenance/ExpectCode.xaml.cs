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
    /// Interaction logic for ExpectCode.xaml
    /// </summary>
    public partial class ExpectCode : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ExpectCode";
        private static readonly string mainTableName = "exception_code";
        //Needed for the combobox
        private static readonly string attributeTableName = "exception_severity_level";
        private static readonly string attributeDisplayPath = "description";
        private static readonly string attributeValuePath = "severity_level";
                          
        //Needed for a combobox
        public ComboBoxItemsProvider cmbSevLev { get; set; }
      
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public ExpectCode()
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
            idgExpectCode.xGrid.FieldLayoutSettings = layouts;
            idgExpectCode.FieldLayoutResourceString = fieldLayoutResource;
            idgExpectCode.MainTableName = mainTableName;
            idgExpectCode.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load severity level combobox
            if (businessObject.HasObjectData)
            {
                cmbSevLev = new ComboBoxItemsProvider();
                cmbSevLev.ItemsSource = businessObject.ObjectData.Tables[attributeTableName].DefaultView;
                cmbSevLev.ValuePath = attributeValuePath;
                cmbSevLev.DisplayMemberPath = attributeDisplayPath;
            }
            if (businessObject.HasObjectData)
            {
                //cmbCurrency = new ComboBoxItemsProvider();
                //cmbCurrency.ItemsSource = businessObject.ObjectData.Tables[bankCurrencyTableName].DefaultView;
                //cmbCurrency.ValuePath = currencyValuePath;
                //cmbCurrency.DisplayMemberPath = currencyDisplayPath;
            }

            idgExpectCode.LoadGrid(businessObject, idgExpectCode.MainTableName);
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
