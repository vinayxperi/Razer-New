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
    /// Interaction logic for TableListing.xaml
    /// </summary>
    public partial class TableListing : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "TableMaintenanceList";
        private static readonly string mainTableName = "maint_table";
        private static readonly string dataKey = "maint_table_id";
        public bool MenuSave { get; set; } //Added to handle saving from menu or CTRL-S Defaults to true

        public string WindowCaption { get { return string.Empty; } }
        public bool OverrideSave = false;
        public TableListing()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Implement the Init method of IScreen
        /// </summary>
        /// <param name="businessObject">Then base busniess object</param>
        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;

            idgTableListing.WindowZoomDelegate = ReturnSelectedData;
            idgTableListing.xGrid.FieldLayoutSettings = layouts;
            idgTableListing.FieldLayoutResourceString = fieldLayoutResource;
            idgTableListing.MainTableName = mainTableName;
            this.MainTableName = mainTableName;

            this.Load(businessObject);

            idgTableListing.LoadGrid(businessObject, idgTableListing.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            //Default value.  If not called from menu then this will be set to false
            MenuSave = true;

        }

        /// <summary>
        /// Handler for double click on grid or buttton click.
        /// </summary>
        public void ReturnSelectedData()
        {
            //Since save will be called without menu, set to false
            MenuSave = false;
            Save();
            grdTableMaintenance.RowDefinitions[0].Height = new GridLength(325);
            grdTableMaintenance.RowDefinitions[2].Height = new GridLength(150, GridUnitType.Star);
            
            idgTableListing.ReturnSelectedData(dataKey);            
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = EventAggregator.GeneratedClickEvent;
            args.Source = idgTableListing;
            EventAggregator.GeneratedClickHandler(this, args);
            
            //Everytime new grid is opened populate the base business object - This was done so that the menu commands and security would function properly
            ScreenBase content = this.gridContent.Content as ScreenBase;
            if (content != null)
            {
                this.CurrentBusObj = content.CurrentBusObj;
            }

        }

        /// <summary>
        /// Override for the screen base in thte content area of the window.
        /// The top grid doesn't requires save so only the data in the placeholder is saved.
        /// </summary>
        public override void Save()
        {
            ScreenBase content = this.gridContent.Content as ScreenBase;

            //If coming from a menu, then any current record needs to be committed before we can check to see if the object has any changes
            if (MenuSave)
            {
                var grids = (from s in UIHelper.FindChildren<ucBaseGrid>(content)
                             select s).ToList();

                if (grids != null && grids.Count > 0)
                {
                    ucBaseGrid grid = grids[0] as ucBaseGrid;
                    grid.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndCommitRecord);
                }
            }

            if (content != null)
            {
                this.CurrentBusObj = content.CurrentBusObj;

                if (CurrentBusObj.ObjectData.HasChanges())
                {
                    //Default to Yes for save so that menu saves will automaticallly run the save.
                    MessageBoxResult result = MessageBoxResult.Yes;
                    if (!MenuSave)
                    {
                        result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                    }

                    if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                    {
                        //base.Save(); //commented Brian Dyer 3-13-2012
                        content.Save();
                        //if (OverrideSave == true)
                        //{
                        //    //skip the following logic
                        //}
                        //else
                         

                            //if (base.SaveSuccessful) //commented Brian Dyer 3-13-2012
                            if (content.SaveSuccessful)
                            {
                                //Each child has only one grid
                                var grids = (from s in UIHelper.FindChildren<ucBaseGrid>(content)
                                             select s).ToList();

                                if (grids != null && grids.Count > 0)
                                {
                                    ucBaseGrid grid = grids[0] as ucBaseGrid;

                                    if (grid != null)
                                    {
                                        grid.LoadGrid(this.CurrentBusObj, content.MainTableName);
                                    }
                                }
                                if (OverrideSave == true)
                                {
                                    //donothing
                                }
                                //else
                                //Messages.ShowInformation("Save Successful!");
                            }
                         
                    }
                }
            }
            //Reset menu save back to the default
            MenuSave = true;
        }


        public override void Close()
        {
            //Set to false since not called from menu
            MenuSave = false;
            this.Save();               
        }
    }
}
