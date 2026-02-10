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
    /// Interaction logic for ExpectType.xaml
    /// </summary>
    public partial class ExpectType : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ExpectType";
        private static readonly string mainTableName = "exception_type";
                                 
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public ExpectType()
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
            idgExpectType.xGrid.FieldLayoutSettings = layouts;
            idgExpectType.FieldLayoutResourceString = fieldLayoutResource;
            idgExpectType.MainTableName = mainTableName;
            idgExpectType.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
           
            if (businessObject.HasObjectData)
            {
                //cmbCurrency = new ComboBoxItemsProvider();
                //cmbCurrency.ItemsSource = businessObject.ObjectData.Tables[bankCurrencyTableName].DefaultView;
                //cmbCurrency.ValuePath = currencyValuePath;
                //cmbCurrency.DisplayMemberPath = currencyDisplayPath;
            }

            idgExpectType.LoadGrid(businessObject, idgExpectType.MainTableName);
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
