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
    /// Interaction logic for TableList2.xaml
    /// </summary>
    public partial class TableList2 : RazerBase.ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "TableList2";
        private static readonly string mainTableName = "maint_table";
        public string WindowCaption
        {
            get { return string.Empty; }
        }
        public TableList2()
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
            idgTableList2.xGrid.FieldLayoutSettings = layouts;
            idgTableList2.FieldLayoutResourceString = fieldLayoutResource;
            idgTableList2.MainTableName = mainTableName;
            idgTableList2.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            idgTableList2.LoadGrid(businessObject, idgTableList2.MainTableName);

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
