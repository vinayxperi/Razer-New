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
    /// Interaction logic for CPIMaint.xaml
    /// </summary>
    public partial class CPIMaint : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "CPIMaint";
        private static readonly string mainTableName = "cpi";

        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public CPIMaint()
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
            idgCPIMaint.xGrid.FieldLayoutSettings = layouts;
            idgCPIMaint.FieldLayoutResourceString = fieldLayoutResource;
            idgCPIMaint.MainTableName = mainTableName;
            idgCPIMaint.xGrid.FieldSettings.AllowEdit = true;
            idgCPIMaint.RecordActivatedDelegate = RowBeingClicked;

            this.Load(businessObject);

            idgCPIMaint.LoadGrid(businessObject, idgCPIMaint.MainTableName);
        }

        public void RowBeingClicked()
        {

            if (idgCPIMaint.xGrid.ActiveRecord.Index < 0)
            {

                idgCPIMaint.xGrid.FieldLayouts[0].Fields["index_name"].Settings.AllowEdit = true;
                idgCPIMaint.xGrid.FieldLayouts[0].Fields["index_date"].Settings.AllowEdit = true;
                //idgCPIMaint.xGrid.FieldLayouts[0].Fields["index_date"].Settings.EditorType = typeof(XamDateTimeEditor);

            }
            else
            {
                idgCPIMaint.xGrid.FieldLayouts[0].Fields["index_name"].Settings.AllowEdit = false;
                idgCPIMaint.xGrid.FieldLayouts[0].Fields["index_date"].Settings.AllowEdit = false;
            }
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
