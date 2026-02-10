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
    /// Interaction logic for State.xaml
    /// </summary>
    public partial class State : RazerBase.ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "State";
        private static readonly string mainTableName = "state";
        public string WindowCaption
        {
            get { return string.Empty; }
        }
        public State()
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
            idgState.xGrid.FieldLayoutSettings = layouts;
            idgState.FieldLayoutResourceString = fieldLayoutResource;
            idgState.MainTableName = mainTableName;
            idgState.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            idgState.LoadGrid(businessObject, idgState.MainTableName);

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
