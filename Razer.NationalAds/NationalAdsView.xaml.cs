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
using System.Diagnostics;

namespace Razer.NationalAds
{
    /// <summary>
    /// Interaction logic for NationalAdsView.xaml
    /// </summary>
    public partial class NationalAdsView : ScreenBase
    {
        private static readonly string fieldLayoutResource = "NationalAdsViewAttachments";
        private static readonly string mainTableName = "view";
        private static readonly string server = "server_loc";
        private static readonly string file = "file_name";
        private static readonly string directory = "directory";

        public NationalAdsView()
            : base()
        {    
            InitializeComponent();
            Init();
        }

        public void Init()
        {            
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            layouts.AllowAddNew = false;            

            this.MainTableName = mainTableName;
            idgAdvertisementsView.xGrid.FieldLayoutSettings = layouts;
            idgAdvertisementsView.FieldLayoutResourceString = fieldLayoutResource;
            idgAdvertisementsView.MainTableName = mainTableName;
            idgAdvertisementsView.xGrid.FieldSettings.AllowEdit = false;
            idgAdvertisementsView.SetGridSelectionBehavior(true, false);

            GridCollection.Add(idgAdvertisementsView);

            idgAdvertisementsView.WindowZoomDelegate = GridDoubleClickDelegate;

        }

        public void GridDoubleClickDelegate()
        {
            DataRecord record = idgAdvertisementsView.xGrid.ActiveRecord as DataRecord;

            if (record != null)
            {
                string serverLocation = record.Cells[server].Value.ToString();
                string fileName = record.Cells[file].Value.ToString();
                string folder = record.Cells[directory].Value.ToString();

                StringBuilder pathBuilder = new StringBuilder(serverLocation);
                pathBuilder.Append(folder);
                pathBuilder.Append(@"\");
                pathBuilder.Append(fileName);

                //RES 12/11/18 Check to see if file exists before trying to open
                if (System.IO.File.Exists(pathBuilder.ToString()))
                {
                    Process.Start(pathBuilder.ToString()); 
                }
                else                
                    MessageBox.Show("File not found");
    
     

                //Process.Start(pathBuilder.ToString()); 
            }
        }
    }
}
