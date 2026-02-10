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
    /// Interaction logic for Oem.xaml
    /// </summary>
    public partial class Oem : RazerBase.ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "Oem";
        private static readonly string mainTableName = "oem";
        public string WindowCaption
        {
            get { return string.Empty; }
        }
        public Oem()
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
            idgOem.xGrid.FieldLayoutSettings = layouts;
            idgOem.FieldLayoutResourceString = fieldLayoutResource;
            idgOem.MainTableName = mainTableName;
            idgOem.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            idgOem.LoadGrid(businessObject, idgOem.MainTableName);

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
