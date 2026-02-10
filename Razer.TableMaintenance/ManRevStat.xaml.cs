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
    /// Interaction logic for ManRevStat.xaml
    /// </summary>
    public partial class ManRevStat : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ManRevStat";
        private static readonly string mainTableName = "man_inv_revenue_status";

        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public ManRevStat()
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
            idgManRevStat.xGrid.FieldLayoutSettings = layouts;
            idgManRevStat.FieldLayoutResourceString = fieldLayoutResource;
            idgManRevStat.MainTableName = mainTableName;
            idgManRevStat.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            idgManRevStat.LoadGrid(businessObject, idgManRevStat.MainTableName);
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
