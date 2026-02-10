using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
    /// Interaction logic for Discount.xaml
    /// </summary>
    public partial class DiscountType : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "DiscountType";
        private static readonly string mainTableName = "main";

        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public DiscountType()
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
            idgDiscount.xGrid.FieldLayoutSettings = layouts;
            idgDiscount.FieldLayoutResourceString = fieldLayoutResource;
            idgDiscount.MainTableName = mainTableName;
            idgDiscount.xGrid.FieldSettings.AllowEdit = true;
           

            this.Load(businessObject);

            idgDiscount.LoadGrid(businessObject, idgDiscount.MainTableName);
        }

        


        public override void Save()
        {
            int cumulative_flag = 0;
            int fixed_rate_flag = 0;
            foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["main"].Rows)
            {
                cumulative_flag = 0;
                fixed_rate_flag = 0;
                cumulative_flag = Convert.ToInt32(r["cumulative_flag"]);
                fixed_rate_flag = Convert.ToInt32(r["fixed_rate_flag"]);

               if ((cumulative_flag == 1) && (fixed_rate_flag == 1))
                {
                   Messages.ShowInformation("Both cumulative and fixed cannot be checked for a single discount");
                  return;
                }



            }
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
