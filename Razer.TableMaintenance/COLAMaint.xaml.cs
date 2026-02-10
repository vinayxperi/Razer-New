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
using System.Data;

namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for COLAMaint.xaml
    /// </summary>
    public partial class COLAMaint : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "COLAMaint";
        private static readonly string mainTableName = "cola";

        //Needed for the combobox
        private static readonly string cpiindexTableName = "cpi";
        private static readonly string cpiindexDisplayPath = "index_name";
        private static readonly string cpiindexValuePath = "cpi_amount";

        //Needed for a combobox
        public ComboBoxItemsProvider cmbCPIIndex { get; set; }

        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public COLAMaint()
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
            idgCOLAMaint.xGrid.FieldLayoutSettings = layouts;
            idgCOLAMaint.FieldLayoutResourceString = fieldLayoutResource;
            idgCOLAMaint.MainTableName = mainTableName;
            idgCOLAMaint.xGrid.FieldSettings.AllowEdit = true;
            this.Load(businessObject);

            //load cpi index to combobox
            if (businessObject.HasObjectData)
            {
                cmbCPIIndex = new ComboBoxItemsProvider();
                cmbCPIIndex.ItemsSource = businessObject.ObjectData.Tables[cpiindexTableName].DefaultView;
                //cmbCPIIndex.ValuePath = cpiindexValuePath;
                cmbCPIIndex.ValuePath = cpiindexDisplayPath;
                cmbCPIIndex.DisplayMemberPath = cpiindexDisplayPath;
            }

            this.Load(businessObject);

            idgCOLAMaint.LoadGrid(businessObject, idgCOLAMaint.MainTableName);
            idgCOLAMaint.CellUpdatedDelegate = GridDestination_CellUpdated;
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
 

        private void GridDestination_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        {
            //decimal cpi_numerator = 0;
            //decimal cpi_denominator = 0;

            //if (e.Cell.Field.Name == "cpi_index_name")
            //{
            //    DataRecord GridRecord = null;
            //    idgCOLAMaint.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //    GridRecord = idgCOLAMaint.ActiveRecord;
            //    if (GridRecord != null)
            //    {
            //        EnumerableRowCollection<DataRow> CPIValue = from CPITable in CurrentBusObj.ObjectData.Tables["cpi"].AsEnumerable()
            //                                               where CPITable.Field<string>("index_name") == GridRecord.Cells["cpi_index_name"].Value.ToString()
            //                                                 select CPITable;

            //        foreach (DataRow r in CPIValue)
            //        {
            //            if (r["index_name"].ToString() == GridRecord.Cells["cpi_index_name"].Value.ToString())
            //                GridRecord.Cells["cpi_numerator"].Value = Convert.ToDecimal(r["cpi_amount"]);
            //        }

            //        if ((Convert.ToDecimal(GridRecord.Cells["cpi_numerator"].Value) != 0) && (Convert.ToDecimal(GridRecord.Cells["cpi_denominator"].Value) != 0))
            //            //(Convert.ToDecimal(GridRecord.Cells["cpi_denominator"].Value) != null))
            //        {
            //            GridRecord.Cells["cola_amount"].Value = Convert.ToDecimal(GridRecord.Cells["cpi_numerator"].Value) / Convert.ToDecimal(GridRecord.Cells["cpi_denominator"].Value);
            //            idgCOLAMaint.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //        }
            //     }
            //}
            //if (e.Cell.Field.Name == "cpi_index_denominator")
            //{
            //    DataRecord GridRecord = null;
            //    idgCOLAMaint.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //    GridRecord = idgCOLAMaint.ActiveRecord;
            //    if (GridRecord != null)
            //    {
            //        EnumerableRowCollection<DataRow> CPIValue = from CPITable in CurrentBusObj.ObjectData.Tables["cpi"].AsEnumerable()
            //                                                where CPITable.Field<string>("index_name") == GridRecord.Cells["cpi_index_denominator"].Value.ToString()
            //                                                select CPITable;

            //        foreach (DataRow r in CPIValue)
            //        {
            //            if (r["index_name"].ToString() == GridRecord.Cells["cpi_index_denominator"].Value.ToString())
            //                GridRecord.Cells["cpi_denominator"].Value = Convert.ToDecimal(r["cpi_amount"]);
            //        }

            //        if ((Convert.ToDecimal(GridRecord.Cells["cpi_numerator"].Value) != 0) && (Convert.ToDecimal(GridRecord.Cells["cpi_denominator"].Value) != 0))
            //        {
            //            GridRecord.Cells["cola_amount"].Value = Convert.ToDecimal(GridRecord.Cells["cpi_numerator"].Value) / Convert.ToDecimal(GridRecord.Cells["cpi_denominator"].Value);
            //            idgCOLAMaint.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //        }
            //    }
            //}
        }
    }
}
