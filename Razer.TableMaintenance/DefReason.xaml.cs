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
    /// Interaction logic for Language.xaml
    /// </summary>
    public partial class DefReason : RazerBase.ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "DefReason";
        private static readonly string mainTableName = "DefReason";
        //Needed for a combobox
        public ComboBoxItemsProvider cmbDefAcctType { get; set; }
        public string WindowCaption
        {
            get { return string.Empty; }
        }
        public DefReason()
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
            idgDefReason.xGrid.FieldLayoutSettings = layouts;
            idgDefReason.FieldLayoutResourceString = fieldLayoutResource;
            idgDefReason.MainTableName = mainTableName;
            idgDefReason.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load Rule Header combobox
            if (businessObject.HasObjectData)
            {
                cmbDefAcctType = new ComboBoxItemsProvider();
                cmbDefAcctType.ItemsSource = businessObject.ObjectData.Tables["dddwDefAcct"].DefaultView;
                cmbDefAcctType.ValuePath = "def_acct_type";
                cmbDefAcctType.DisplayMemberPath = "def_acct_desc";
            }
            idgDefReason.LoadGrid(businessObject, idgDefReason.MainTableName);

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
