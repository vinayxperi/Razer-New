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
    /// Interaction logic for WorkflowType.xaml
    /// </summary>
    
    public partial class WorkflowType : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "WorkFlowType";
        private static readonly string mainTableName = "workflow_type";
   

        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public WorkflowType()
        {
            InitializeComponent();
        }
        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            layouts.AllowAddNew = true;
            layouts.AddNewRecordLocation = AddNewRecordLocation.OnTop;

            this.CurrentBusObj = businessObject;
          
            this.MainTableName = mainTableName;

            idgWorkFlowType.xGrid.FieldLayoutSettings = layouts;
            idgWorkFlowType.FieldLayoutResourceString = fieldLayoutResource;
            idgWorkFlowType.MainTableName = mainTableName;
            idgWorkFlowType.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            idgWorkFlowType.LoadGrid(businessObject, idgWorkFlowType.MainTableName);

            
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
