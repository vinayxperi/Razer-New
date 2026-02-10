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

namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for BillFreqMaint.xaml
    /// </summary>
    public partial class BillFreqMaint : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "BillFreqMaint";
        private static readonly string mainTableName = "man_inv_bill_frequency";

        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public BillFreqMaint()
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
            idgBillFreqMaint.xGrid.FieldLayoutSettings = layouts;
            idgBillFreqMaint.FieldLayoutResourceString = fieldLayoutResource;
            idgBillFreqMaint.MainTableName = mainTableName;
            idgBillFreqMaint.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            idgBillFreqMaint.LoadGrid(businessObject, idgBillFreqMaint.MainTableName);
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
