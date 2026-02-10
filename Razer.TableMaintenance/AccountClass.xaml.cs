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
    /// Interaction logic for AccountClass.xaml
    /// </summary>
    public partial class AccountClass : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "AccountClass";
        private static readonly string mainTableName = "acct_class";
        private static readonly string ratioTableName = "ratio_flag";
        private static readonly string ratioParameterName = "@code_name";
        private static readonly string ratioParameterValue = "RatioFlag";        
        private static readonly string ratioDisplayPath = "code_value";
        private static readonly string ratioValuePath = "fkey_int";        
        public ComboBoxItemsProvider cmbRatioFlag { get; set; }

        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public AccountClass()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            this.CurrentBusObj = businessObject;
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            layouts.AllowAddNew = true;
            layouts.AddNewRecordLocation = AddNewRecordLocation.OnTop;

            this.MainTableName = mainTableName;
            idgAccountClass.xGrid.FieldLayoutSettings = layouts;
            idgAccountClass.FieldLayoutResourceString = fieldLayoutResource;
            idgAccountClass.MainTableName = mainTableName;
            idgAccountClass.xGrid.FieldSettings.AllowEdit = true;            
            idgAccountClass.SetGridSelectionBehavior(false, false);           
            


            this.CurrentBusObj.Parms.AddParm(ratioParameterName, ratioParameterValue);
            this.Load(this.CurrentBusObj);

            if (this.CurrentBusObj.HasObjectData)
            {
                cmbRatioFlag = new ComboBoxItemsProvider();
                cmbRatioFlag.ItemsSource = this.CurrentBusObj.ObjectData.Tables[ratioTableName].DefaultView;
                cmbRatioFlag.ValuePath = ratioValuePath;
                cmbRatioFlag.DisplayMemberPath = ratioDisplayPath;
            }

            idgAccountClass.LoadGrid(businessObject, idgAccountClass.MainTableName);
            
            idgAccountClass.xGrid.ActiveCell = (idgAccountClass.xGrid.Records[0] as DataRecord).Cells[0];   
            
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
