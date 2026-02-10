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
    /// Interaction logic for WorkflowRules.xaml
    /// </summary>
    public partial class WorkflowRules : RazerBase.ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "WorkFlowRules";
        private static readonly string mainTableName = "workflow_rules";


        //Needed for the combobox
        private static readonly string WfClassTableName = "workflow_class";
        private static readonly string WfClassDisplayPath = "description";
        private static readonly string WfClassValuePath = "wf_class";

        //Needed for the combobox
        private static readonly string WfTypeTableName = "workflow_type";
        private static readonly string WfTypeDisplayPath = "description";
        private static readonly string WfTypeValuePath = "wf_type";

        //Needed for a combobox
        public ComboBoxItemsProvider cmbWfClass { get; set; }
        public ComboBoxItemsProvider cmbWfType { get; set; }



        public string WindowCaption
        {
            get { return string.Empty; }
        }
        public WorkflowRules()
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
            idgWorkFlowRules.xGrid.FieldLayoutSettings = layouts;
            idgWorkFlowRules.FieldLayoutResourceString = fieldLayoutResource;
            idgWorkFlowRules.MainTableName = mainTableName;
            idgWorkFlowRules.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            //load Rule Header combobox
            if (businessObject.HasObjectData)
            {
                cmbWfClass = new ComboBoxItemsProvider();
                cmbWfClass.ItemsSource = businessObject.ObjectData.Tables[WfClassTableName].DefaultView;
                cmbWfClass.ValuePath = WfClassValuePath;
                cmbWfClass.DisplayMemberPath = WfClassDisplayPath;

                cmbWfType = new ComboBoxItemsProvider();
                cmbWfType.ItemsSource = businessObject.ObjectData.Tables[WfTypeTableName].DefaultView;
                cmbWfType.ValuePath = WfTypeValuePath;
                cmbWfType.DisplayMemberPath = WfTypeDisplayPath;
            }




            idgWorkFlowRules.LoadGrid(businessObject, idgWorkFlowRules.MainTableName);

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
