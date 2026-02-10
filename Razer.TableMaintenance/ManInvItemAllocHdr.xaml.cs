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
    /// Interaction logic for ManInvItemAllocHdr.xaml
    /// </summary>
    public partial class ManInvItemAllocHdr : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ManInvItemAllocHdr";
        private static readonly string mainTableName = "man_inv_item_alloc_hdr";
               
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public ManInvItemAllocHdr()
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
            idgManInvItemAllocHdr.xGrid.FieldLayoutSettings = layouts;
            idgManInvItemAllocHdr.FieldLayoutResourceString = fieldLayoutResource;
            idgManInvItemAllocHdr.MainTableName = mainTableName;
            idgManInvItemAllocHdr.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            idgManInvItemAllocHdr.LoadGrid(businessObject, idgManInvItemAllocHdr.MainTableName);
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
