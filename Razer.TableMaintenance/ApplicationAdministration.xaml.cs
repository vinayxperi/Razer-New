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
using RazerInterface;

namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for ApplicationAdministration.xaml
    /// </summary>
    public partial class ApplicationAdministration : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ApplicationAdministration";
        private static readonly string mainTableName = "app_admin";
        private static readonly string appTableName = "app_type";
        private static readonly string appParameterName = "@code_name";
        private static readonly string appParameterValue = "AppType";
        private static readonly string appDisplayPath = "code_value";
        private static readonly string appValuePath = "fkey_int";     
   
        public ComboBoxItemsProvider cmbApplicationType { get; set; }   

        public string WindowCaption { get { return string.Empty; } }

        public ApplicationAdministration()
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
            idgApplicationAdministration.xGrid.FieldLayoutSettings = layouts;
            idgApplicationAdministration.FieldLayoutResourceString = fieldLayoutResource;
            idgApplicationAdministration.MainTableName = mainTableName;
            idgApplicationAdministration.xGrid.FieldSettings.AllowEdit = true;

            businessObject.Parms.AddParm(appParameterName, appParameterValue);

            this.Load(businessObject);

            if (businessObject.HasObjectData)
            {
                cmbApplicationType = new ComboBoxItemsProvider();
                cmbApplicationType.ItemsSource = businessObject.ObjectData.Tables[appTableName].DefaultView;
                cmbApplicationType.ValuePath = appValuePath;
                cmbApplicationType.DisplayMemberPath = appDisplayPath;
            }

            idgApplicationAdministration.LoadGrid(businessObject, idgApplicationAdministration.MainTableName);
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
