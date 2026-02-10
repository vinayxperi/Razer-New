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
    /// Interaction logic for DataMaint.xaml
    /// </summary>
    public partial class DataMaint : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "DataServiceType";
        private static readonly string mainTableName = "data";

        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public DataMaint()
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
            idgData.xGrid.FieldLayoutSettings = layouts;
            idgData.FieldLayoutResourceString = fieldLayoutResource;
            idgData.MainTableName = mainTableName;
            idgData.xGrid.FieldSettings.AllowEdit = true;
           

            this.Load(businessObject);

            idgData.LoadGrid(businessObject, idgData.MainTableName);
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
