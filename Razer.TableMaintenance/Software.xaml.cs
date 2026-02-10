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
    /// Interaction logic for Software.xaml
    /// </summary>
    public partial class Software : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "Software";
        private static readonly string mainTableName = "software";

        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public Software()
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
            idgSoftware.xGrid.FieldLayoutSettings = layouts;
            idgSoftware.FieldLayoutResourceString = fieldLayoutResource;
            idgSoftware.MainTableName = mainTableName;
            idgSoftware.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            idgSoftware.LoadGrid(businessObject, idgSoftware.MainTableName);
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
