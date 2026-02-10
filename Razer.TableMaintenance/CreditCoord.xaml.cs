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
    /// Interaction logic for CreditCoord.xaml
    /// </summary>
    public partial class CreditCoord : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "CreditCoord";
        private static readonly string mainTableName = "credit_coordinator";
        //Needed for the combobox
        private static readonly string creditcoordTableName = "SecurityUsers";
        private static readonly string creditcoordDisplayPath = "user_name";
        private static readonly string creditcoordValuePath = "user_id";
                    
        //Needed for a combobox
        public ComboBoxItemsProvider cmbCreditCoord { get; set; }
       
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public CreditCoord()
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
            idgCreditCoord.xGrid.FieldLayoutSettings = layouts;
            idgCreditCoord.FieldLayoutResourceString = fieldLayoutResource;
            idgCreditCoord.MainTableName = mainTableName;
            idgCreditCoord.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load security_user combobox
            if (businessObject.HasObjectData)
            {
                cmbCreditCoord = new ComboBoxItemsProvider();
                cmbCreditCoord.ItemsSource = businessObject.ObjectData.Tables[creditcoordTableName].DefaultView;
                cmbCreditCoord.ValuePath = creditcoordValuePath;
                cmbCreditCoord.DisplayMemberPath = creditcoordDisplayPath;
            }
           
            this.Load(businessObject);

            idgCreditCoord.LoadGrid(businessObject, idgCreditCoord.MainTableName);
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
