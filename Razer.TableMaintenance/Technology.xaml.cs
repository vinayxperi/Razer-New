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
    /// Interaction logic for Technology.xaml
    /// </summary>
    public partial class Technology : RazerBase.ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "Technology";
        private static readonly string mainTableName = "Technology";
        public string WindowCaption
        {
            get { return string.Empty; }
        }
        public Technology()
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
            idgTechnology.xGrid.FieldLayoutSettings = layouts;
            idgTechnology.FieldLayoutResourceString = fieldLayoutResource;
            idgTechnology.MainTableName = mainTableName;
            idgTechnology.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            idgTechnology.LoadGrid(businessObject, idgTechnology.MainTableName);

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
