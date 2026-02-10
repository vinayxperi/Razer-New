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
    /// Interaction logic for BrandMaint.xaml
    /// </summary>
    public partial class TivoCountMaint : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "TivoCount";
        private static readonly string mainTableName = "tivo_count";

        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public TivoCountMaint()
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
            idgTivoCountMaint.xGrid.FieldLayoutSettings = layouts;
            idgTivoCountMaint.FieldLayoutResourceString = fieldLayoutResource;
            idgTivoCountMaint.MainTableName = mainTableName;
            idgTivoCountMaint.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            idgTivoCountMaint.LoadGrid(businessObject, idgTivoCountMaint.MainTableName);
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
