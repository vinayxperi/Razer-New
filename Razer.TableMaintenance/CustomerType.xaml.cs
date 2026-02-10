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
    /// Interaction logic for CustomerType.xaml
    /// </summary>
    public partial class CustomerType : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "CustomerType";
        private static readonly string mainTableName = "customer_type";
        public string WindowCaption { get { return string.Empty; } }

        public CustomerType()
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
            idgCustomerType.xGrid.FieldLayoutSettings = layouts;
            idgCustomerType.FieldLayoutResourceString = fieldLayoutResource;
            idgCustomerType.MainTableName = mainTableName;
            idgCustomerType.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            idgCustomerType.LoadGrid(businessObject, idgCustomerType.MainTableName);
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
