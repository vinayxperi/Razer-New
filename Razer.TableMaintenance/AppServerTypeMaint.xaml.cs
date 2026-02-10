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
    /// Interaction logic for AppServerTypeMaint.xaml
    /// </summary>
    public partial class AppServerTypeMaint : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "AppServerType";
        private static readonly string mainTableName = "app_server_type";

        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public AppServerTypeMaint()
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
            idgAppServerTypeMaint.xGrid.FieldLayoutSettings = layouts;
            idgAppServerTypeMaint.FieldLayoutResourceString = fieldLayoutResource;
            idgAppServerTypeMaint.MainTableName = mainTableName;
            idgAppServerTypeMaint.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            idgAppServerTypeMaint.LoadGrid(businessObject, idgAppServerTypeMaint.MainTableName);
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
