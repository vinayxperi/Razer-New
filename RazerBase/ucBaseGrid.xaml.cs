using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Infragistics.Documents.Excel;
using Infragistics.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.DataPresenter.ExcelExporter;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Collections.Generic;
using Infragistics.Windows.Editors;
using System.Windows.Input;
using Infragistics.Windows.DockManager;
using Infragistics.Windows;

namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucBaseGrid.xaml
    /// </summary>
    public partial class ucBaseGrid : INotifyPropertyChanged
    {
        #region "User Control variables and properties"

        private bool GridIsLoading = false;

        public DataRecord ActiveRecord
        {
            get { return (DataRecord)GetValue(ActiveRecordProperty); }
            set 
            { 
                SetValue(ActiveRecordProperty, value);
                SignalPropertyChanged("ActiveRecord");
            }
        }

        private Style gridCellValuePresenterStyle = new Style();
        /// <summary>
        /// The style to use for the cell presentation - Defaults to BaseGridDefaultStyle
        /// </summary>
        public Style GridCellValuePresenterStyle
        {
            get { return gridCellValuePresenterStyle; }
            set 
            { 
                  gridCellValuePresenterStyle = value;
                  xGrid.FieldSettings.CellValuePresenterStyle = GridCellValuePresenterStyle;
            }
        }
        //public DataRow ActiveRow
        //{
        //    get
        //    {
        //        return (DataRow)GetValue(ActiveRowProperty);
        //    }

        //    set
        //    {
        //        SetValue(ActiveRowProperty, value);
        //        SignalPropertyChanged("ActiveRow");
        //    }
        //}

        // Using a DependencyProperty as the backing store for ActiveRecord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveRecordProperty =
            DependencyProperty.Register("ActiveRecord", typeof(DataRecord), typeof(ucBaseGrid));

        //public static readonly DependencyProperty ActiveRowProperty =
        //    DependencyProperty.Register("ActiveRow", typeof(DataRow), typeof(ucBaseGrid));

        //Ok Delegate for Double Click
        public delegate void WindowZoom();
        public delegate void FormatGrid();
        public delegate void ContextMenuAdd();
        public delegate void ContextMenuRemove();
        public delegate void ContextMenuGeneric();
        public delegate void SingleClickZoom();
        public delegate void ToggleGridFilter();
        public delegate void RecordActivated();
        public delegate void EditModeStarted(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs e);
        public delegate void EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e);
        public delegate void GridGotFocus(object sender, RoutedEventArgs e);
        public delegate void GridPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e);
        public delegate void CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e);
        public delegate void GridLoaded(object sender, RoutedEventArgs e);
        public delegate void SelectedItemsChanging(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangingEventArgs e);

        //Delegate runs on single click of grid
        //Set by the parent window
        public WindowZoom mSingleClickZoomDelegate;
        public WindowZoom SingleClickZoomDelegate
        {
            get { return mSingleClickZoomDelegate; }
            set { mSingleClickZoomDelegate = value; }
        }

        //Zoom function delegate
        //Set by the parent window
        public WindowZoom mWindowZoomDelegate;
        public WindowZoom WindowZoomDelegate
        {
            get { return mWindowZoomDelegate; }
            set { mWindowZoomDelegate = value; }
        }

        //Context menu add delegate
        //Set by the parent window
        public ContextMenuAdd mContextMenuAddDelegate;
        public ContextMenuAdd ContextMenuAddDelegate
        {
            get { return mContextMenuAddDelegate; }
            set { mContextMenuAddDelegate = value; }
        }

        //Context menu remove delegate
        //Set by the parent window
        public ContextMenuRemove mContextMenuRemoveDelegate;
        public ContextMenuRemove ContextMenuRemoveDelegate
        {
            get { return mContextMenuRemoveDelegate; }
            set { mContextMenuRemoveDelegate = value; }
        }

        //Context menu generic delegate
        //Set by the parent window
        public ContextMenuGeneric mContextMenuGenericDelegate1;
        public ContextMenuGeneric ContextMenuGenericDelegate1
        {
            get { return mContextMenuGenericDelegate1; }
            set { mContextMenuGenericDelegate1 = value; }
        }

        //Context menu generic delegate
        //Set by the parent window
        public ContextMenuGeneric mContextMenuGenericDelegate2;
        public ContextMenuGeneric ContextMenuGenericDelegate2
        {
            get { return mContextMenuGenericDelegate2; }
            set { mContextMenuGenericDelegate2 = value; }
        }

        //Context menu generic delegate
        //Set by the parent window
        public ContextMenuGeneric mContextMenuGenericDelegate3;
        public ContextMenuGeneric ContextMenuGenericDelegate3
        {
            get { return mContextMenuGenericDelegate3; }
            set { mContextMenuGenericDelegate3 = value; }
        }

        //Context menu generic delegate
        //Set by the parent window
        public ContextMenuGeneric mContextMenuGenericDelegate4;
        public ContextMenuGeneric ContextMenuGenericDelegate4
        {
            get { return mContextMenuGenericDelegate4; }
            set { mContextMenuGenericDelegate4 = value; }
        }
        //Context menu toggle filter property
        //Set by the parent window
        public ToggleGridFilter mContextMenuToggleFilter;
        public ToggleGridFilter ContextMenuToggleFilter
        {
            get { return mContextMenuToggleFilter; }
            set { mContextMenuToggleFilter = value; }
        }

        private RecordActivated  mRecordActivatedDelegate;
        public RecordActivated RecordActivatedDelegate
        {
            get { return mRecordActivatedDelegate; }
            set { mRecordActivatedDelegate = value; }
        }

        private GridGotFocus mGridGotFocusDelegate;
        public GridGotFocus GridGotFocusDelegate
        {
            get { return mGridGotFocusDelegate; }
            set { mGridGotFocusDelegate = value; }
        }

        private EditModeStarted mEditModeStartedDelegate;
        public EditModeStarted  EditModeStartedDelegate
        {
            get { return mEditModeStartedDelegate; }
            set { mEditModeStartedDelegate = value; }
        }

        private EditModeEnded mEditModeEndedDelegate;
        public EditModeEnded EditModeEndedDelegate
        {
            get { return mEditModeEndedDelegate; }
            set { mEditModeEndedDelegate = value; }
        }

        private GridPreviewKeyDown mGridPreviewKeyDownDelegate;
        public GridPreviewKeyDown GridPreviewKeyDownDelegate
        {
            get { return mGridPreviewKeyDownDelegate; }
            set { mGridPreviewKeyDownDelegate = value; }
        }

        private CellUpdated mCellUpdatedDelegate;
        public CellUpdated CellUpdatedDelegate
        {
            get { return mCellUpdatedDelegate; }
            set { mCellUpdatedDelegate = value; }
        }

        private GridLoaded mGridLoadedDelegate;
        public GridLoaded GridLoadedDelegate
        {
            get { return mGridLoadedDelegate; }
            set { mGridLoadedDelegate = value; }
        }

        private SelectedItemsChanging mSelectedItemsChangingDelegate;
        public SelectedItemsChanging SelectedItemsChangingDelegate
        {
            get { return mSelectedItemsChangingDelegate; }
            set { mSelectedItemsChangingDelegate = value; }
        }

        //used for customizing context popup menu names
        public string ContextMenuSaveToExcelDisplayName { get; set; }
        public string ContextMenuSaveGridSettingsDisplayName { get; set; }
        public string ContextMenuResetGridSettingsDisplayName { get; set; }
        public string ContextMenuAddDisplayName { get; set; }
        public string ContextMenuRemoveDisplayName { get; set; }
        public string ContextMenuGenericDisplayName1 { get; set; }
        public string ContextMenuGenericDisplayName2 { get; set; }
        public string ContextMenuGenericDisplayName3 { get; set; }
        public string ContextMenuGenericDisplayName4 { get; set; }
        //public string ContextMenuToggleFilterDisplayName { get; set; }
        //used for hiding and displaying grid items, made nullable
        public bool? ContextMenuSaveToExcelIsVisible { get; set; }
        public bool? ContextMenuSaveGridSettingsIsVisible { get; set; }
        public bool? ContextMenuResetGridSettingsIsVisible { get; set; }
        public bool? ContextMenuAddIsVisible { get; set; }
        public bool? ContextMenuRemoveIsVisible { get; set; }
        public bool? ContextMenuGenericIsVisible1 { get; set; }
        public bool? ContextMenuGenericIsVisible2 { get; set; }
        public bool? ContextMenuGenericIsVisible3 { get; set; }
        public bool? ContextMenuGenericIsVisible4 { get; set; }
        public bool DisableBaseGridTab { get; set; }
        public bool AllowWindowDrag { get; set; } //Boolean to turn on ability to drag grid - defaults to false
        public Cell CurrentActiveEditCell { get; set; }
        public bool SkipReadOnlyCellsOnTab { get; set; }

        //Default true - Determines if the autovalidation string editor will be created for string value cells with no other editor set
        public bool UseGridAutoValidation { get; set; } 
        //used for swapping graphics
        public string ContextMenuGenericImageSwap1 { get; set; }
        public string ContextMenuGenericImageSwap2 { get; set; }
        public string ContextMenuGenericImageSwap3 { get; set; }
        public string ContextMenuGenericImageSwap4 { get; set; }
        public bool? mContextMenuToggleFilterIsVisible = true;
        public bool? ContextMenuToggleFilterIsVisible
        {
            get { return mContextMenuToggleFilterIsVisible; }
            set { mContextMenuToggleFilterIsVisible = value; }
        }

        //Security related properties
        //The screen level access
        public AccessLevel ParentSecurityLevel { get; set; }
        private enum MenuDisableType { Add, Delete };


        //Additional Grid Formatting that can't be handled in the layout file
        public FormatGrid mFormatGridDelegate;
        public FormatGrid FormatGridDelegate
        {
            get { return mFormatGridDelegate; }
            set { mFormatGridDelegate = value; }
        }

        //Determines if filter record show on grid
        public bool? IsFilterable
        {
            get { return (xGrid.FieldSettings.AllowRecordFiltering); }
            set { xGrid.FieldSettings.AllowRecordFiltering = value; }
        }

        //Grid field Layout loaded from xml file
        private string mFieldLayoutResourceString;
        public string FieldLayoutResourceString
        {
            get { return mFieldLayoutResourceString; }
            set
            {
                mFieldLayoutResourceString = value;
                xGrid.FieldLayoutSettings.AutoGenerateFields = false;
                xGrid.FieldLayouts.Clear();
                xGrid.FieldLayouts.Add((Infragistics.Windows.DataPresenter.FieldLayout)Application.Current.Resources[mFieldLayoutResourceString]);
            }
        }

        private string mMainTableName;
        public string MainTableName
        {
            get { return mMainTableName; }
            set { mMainTableName = value; }
        }

        //Configuration file name used to store customized grid settings. If none is given then no configuration will be saved
        private string mConfigFileName = "";
        public string ConfigFileName
        {
            get { return mConfigFileName; }
            set { mConfigFileName = value; }
        }

        //registers grid as parent
        //Determines if filter record show on grid
        public bool IsParent
        {
            get { return ChildSupport.Count > 0; }
        }

        //Supported ChildGrids as list of ChildSupport
        public List<ChildSupport> ChildSupport { get; set; }

        //Stores the name of the field double clicked
        public string DoubleClickFieldName { get; set; }

        #endregion

        public ucBaseGrid()
        {
            DoNotSelectFirstRecordOnLoad = false;
            DisableBaseGridTab = true;
            UseGridAutoValidation = true;
            AllowWindowDrag = false;
            SkipReadOnlyCellsOnTab = false;
            // This call is required by the designer.
            InitializeComponent();

            DoubleClickFieldName = "";

            ChildSupport = new List<ChildSupport>();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

            //Sets the base style of the grid
            GridCellValuePresenterStyle = (Style)TryFindResource("BaseGridDefaultStyle");
            //Turn on highlight alternate rows by default
            xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;

            // DO NOT ALLOW EDIT
            this.xGrid.FieldSettings.AllowEdit = false;
            this.xGrid.CellActivating += new EventHandler<Infragistics.Windows.DataPresenter.Events.CellActivatingEventArgs>(xGrid_CellActivating);
            this.xGrid.CellActivated += new EventHandler<Infragistics.Windows.DataPresenter.Events.CellActivatedEventArgs>(xGrid_CellActivated);
            this.xGrid.EditModeStarted += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs>(xGrid_EditModeStarted);
            this.xGrid.CellChanged += new EventHandler<Infragistics.Windows.DataPresenter.Events.CellChangedEventArgs>(xGrid_CellChanged);
            this.xGrid.RecordActivated += new EventHandler<Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs>(xGrid_RecordActivated);
            this.xGrid.PreviewKeyDown += xGrid_PreviewKeyDown;            
            //this.xGrid.InitializeRecord += new EventHandler<Infragistics.Windows.DataPresenter.Events.InitializeRecordEventArgs>(xGrid_InitializeRecord); 
            //ScreenBase Parent = UIHelper.FindVisualParent<ScreenBase>(this);
        }

        void xGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Fix to untrap <ctrl>+<s> since the xamDataGrid has a habit of stealing key strokes
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                try
                {
                    cMainWindowBase mainWindow = UIHelper.FindVisualParent<cMainWindowBase>(this);

                    if (mainWindow != null)
                    {
                        DocumentContentHost DocHost = UIHelper.FindChild<DocumentContentHost>(mainWindow, "DocHost");

                        ScreenBase currentScreen = DocHost.ActiveDocument.Content as ScreenBase;
                        ICommand save = ApplicationCommands.Save;
                        if (save.CanExecute(currentScreen))
                        {
                            save.Execute(currentScreen);
                        }
                    }
                }
                catch { }
            }
            else if (e.Key == Key.Tab && !DisableBaseGridTab)
            {
                //This method is not working properly
                TabToNextEditableColumn();
            }

            if (GridPreviewKeyDownDelegate != null)
                GridPreviewKeyDownDelegate(sender, e);
        }

        /// <summary>
        /// Call this when you need the value for a specified column name in datagrid. Returns in cGlobals.Parms
        /// </summary>
        /// <param name="dataKey">Column name to return value of</param>
        public void ReturnSelectedData(string dataKey)
        {
            //DataRecord record = default(DataRecord);

            //try
            //{
            //    record = (DataRecord)xGrid.SelectedItems.Records[0];
            //}
            //catch (Exception ex)
            //{
            //    // for debugging only
            //    string err = ex.ToString();

            //    // Set the current record
            //    record = xGrid.ActiveCell.Record;
            //}

            //if (record != null)
            //{
            //    cGlobals.ReturnParms.Clear();
            //    cGlobals.ReturnParms.Add(record.Cells[dataKey].Value);
            //}

            List<string> dataKeys = new List<string>();
            dataKeys.Add(dataKey);
            ReturnSelectedData(dataKeys);
        }

        public void ReturnSelectedData(List<string> dataKeys)
        {
            DataRecord record = default(DataRecord);
            
            if (xGrid.ActiveRecord != null)
            {
                record = (DataRecord)xGrid.ActiveRecord;
            }
            else if (xGrid.SelectedItems.Records.Count > 0)
            {
                record = (DataRecord)xGrid.SelectedItems.Records[0];
            }

            if (record != null)
            {
                cGlobals.ReturnParms.Clear();
                dataKeys.ForEach(s => cGlobals.ReturnParms.Add(record.Cells[s].Value.ToString().Trim())); 
            }
        }

        //Removed this since the RetrunSelectedData will now handle either occurence
        /// <summary>
        /// Call this when you need the value for a specified column name in datagrid.
        /// and you have to get the value from the active record instead of SelectedItems.
        /// Cases where this might happen are when you programatically move the grid cursor
        /// and set the active record.  See ucComments.GridComments_Loaded for an example of 
        /// when this is needed.
        /// Returns in cGlobals.Parms
        /// </summary>
        /// <param name="dataKey"></param>
        //public void ReturnSelectedDataFromActiveRecord(string dataKey)
        //{
        //    DataRecord record = default(DataRecord);

        //    if (xGrid.ActiveRecord != null)
        //    {
        //        record = (DataRecord)xGrid.ActiveRecord;
        //        cGlobals.ReturnParms.Clear();
        //        cGlobals.ReturnParms.Add(record.Cells[dataKey].Value);
        //    }
        //}

        public void LoadGrid(cBaseBusObject BusObj, string TableName)
        {
            GridIsLoading = true;
            //Local grid table name will override the passed in screen parent table name if one exists
            if (MainTableName != null)
            {
                TableName = MainTableName;
            }

            xGrid.DataSource = BusObj.ObjectData.Tables[TableName].DefaultView;
            
            //If rows returned the attach to grid and select first row
            //If no rows returned then clear the grid
            //*******If the field layouts dont match the SQL fields then immediately after this statement you
            //*******will see an additional field layout if you debug on xGrid.FieldLayouts
            //*******Normally you should only have one layout.  After this statement a 2nd one will be generated if
            //*******the field layout string does not match the SQL
            if (xGrid.Records.Count > 0 && xGrid.ViewableRecords.Count > 0  && !DoNotSelectFirstRecordOnLoad)
            {
                //Removed since it marks a record as selected w/o user input
                //xGrid.ViewableRecords[0].IsSelected = true;
                //xGrid.ViewableRecords[0].IsActive = true;

               // find fisrt viewable record
                foreach (Record record in xGrid.Records)
                {
                    if (record.Visibility == System.Windows.Visibility.Visible)
                    {
                        record.IsActive = true;
                        record.IsSelected = true;
                        break;
                    }
                }
                
            }
            else
                xGrid.ActiveRecord = null;


            FilterCall();
            //Else
            //MsgBox("Error retrieving data for grid " + Me.Name + " -- " + dt.Rows(0).Item("error_desc"))
            //xGrid.DataSource = Nothing
            //End If

            //Run the function passed from the parent window
            if (FormatGridDelegate != null)
            {
                mFormatGridDelegate();
            }

            LoadGridSettings();

            //Sets validations for maximum string length
            //Only runs if the UseGridAutoValidation bool is set to true - This is the default behavior
            if (UseGridAutoValidation)
                SetGridValidations(BusObj.ObjectData.Tables[TableName]);
            
            GridIsLoading = false;
        }

        /// <summary>
        /// Method to set the maximum string length allowed for text items in a grid cell
        /// The maximum length is determined by the database maximum length
        /// This method only runs against fields that do not have their own editor style set in the FieldLayoutSettings
        /// If the field has its own FieldLayoutSettings editor type then any required validations will need to be added there
        /// </summary>
        /// <param name="dt">Datatable that is bound to the current xGrid</param>
        public void SetGridValidations(DataTable dt)
        {
            //Verify that there is a field layout and datable associated with the grid
            if (xGrid.FieldLayouts != null && dt != null)
            {
                //Loop through each field in the grid
                foreach (Field f in xGrid.FieldLayouts[0].Fields)
                {
                    //Make sure that the column information is populated and that the datatype is string
                    if (dt.Columns[f.Name] != null && dt.Columns[f.Name].DataType == typeof(System.String))
                    {
                        //Verify that an editor style has not already been established for the grid.
                        //If one was configured in the field layout xaml for this field then no 
                        //validations will be set
                        if (f.Settings.EditorStyle == null)
                        {
                            //Create the infragistics value constraint and set the min value to 0 and the max value to the length of the database field
                            ValueConstraint vc = new ValueConstraint();
                            vc.MinLength = 0;
                            vc.MaxLength = dt.Columns[f.Name].MaxLength;
 
                            //Create the style for the editor
                            Style xteStyle = new Style(typeof(XamTextEditor));
                            //Establish the constraint
                            Setter vcSetter = new Setter(XamTextEditor.ValueConstraintProperty, vc);
                            xteStyle.Setters.Add(vcSetter);
                            //Set the field editor to the newly configured editor
                            f.Settings.EditorStyle = xteStyle;
                         }
                       
                    }
                }
            }
  
        }

        public void ClearFilter()
        {
            //clear current filter(s)
            xGrid.FieldLayouts[0].RecordFilters.Clear();
        }

        public void FilterGrid(string FieldName, string Value)
        {
            //Function to filer the xam grid by using contains value passed in the field name parameter
            //Fieldname - contains the datatable field name that the filter will apply to
            //Value - the value that will be used in a 'contains' query for the fieldname
            RecordFilter filter = new RecordFilter();

            filter.FieldName = FieldName;
            //filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.Contains, Value));
            //cbirney - changed to Start With instead of contains
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.StartsWith, Value));
            //Apply the filter to teh grid
            xGrid.FieldLayouts[0].RecordFilters.Add(filter);
            if (Value == "ALL") xGrid.FieldLayouts[0].RecordFilters.Clear();
        }

        public void FilterGrid(Dictionary<string, string> ColumnValueDictionary)
        {
            try
            {
                //clear current filter(s)
                xGrid.FieldLayouts[0].RecordFilters.Clear();

                //Function to filter the xam grid by using contains value passed in the field name parameter
                //Fieldname - contains the datatable field name that the filter will apply to
                //Value - the value that will be used in a 'contains' query for the fieldname
                foreach (var ValueKeyPair in ColumnValueDictionary)
                {
                    if (ValueKeyPair.Value != "ALL")
                    {
                        var filter = new RecordFilter();
                        filter.FieldName = ValueKeyPair.Key;
                        filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.Equals, ValueKeyPair.Value));

                        //Apply the filter to the grid
                        xGrid.FieldLayouts[0].RecordFilters.Add(filter);

                        //Filter any children
                        if (xGrid.ViewableRecords.Count > 0)
                        {
                            xGrid.ViewableRecords[0].IsSelected = true;
                            xGrid.ViewableRecords[0].IsActive = true;
                        }
                        else
                            xGrid.ActiveRecord = null;

                        FilterCall();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ExportToExcel()
        {
            //Procedure to allow save of all current data on the grid to an Excel sheet
            //Saves filtered data if a filter is currently in use

            SaveFileDialog save = new SaveFileDialog();
            //Can save as Excle 2007 or 2003
            save.Filter = " Office 2007 Excel File(*.xlsx)|*.xlsx|Office 2003 Excel File (*.xls)|*.xls";
            System.Nullable<bool> dialogResult = save.ShowDialog();
            if (dialogResult == true)
            {
                DataPresenterExcelExporter exporter = new DataPresenterExcelExporter();
 
                exporter.ExportStarted += new EventHandler<ExportStartedEventArgs>(exporter_ExportStarted);

                ////Establishes format for Excel file
                //FormatSettings labelFormatSettings = new FormatSettings();
                //labelFormatSettings.FillPattern = FillPatternStyle.Solid;
                //labelFormatSettings.FontWeight = FontWeights.Bold;
                //labelFormatSettings.FillPatternBackgroundColor = Colors.Gray;
                //labelFormatSettings.FontColor = Colors.Red;
                //labelFormatSettings.HorizontalAlignment = HorizontalCellAlignment.Right;
                //DataPresenterExcelExporter.SetExcelCellFormatSettings(xGrid, labelFormatSettings);

                //Saves file to destination
                if (save.FileName.Contains(".xlsx"))
                {
                    //**DWR Added 4/18/12 - Added Try Catch to stop app from crashing when excel doc was already opened.
                    try
                    {
                        exporter.Export(xGrid, save.FileName, WorkbookFormat.Excel2007);
                    }
                    catch 
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }

                }
                else
                {
                    try
                    {
                        exporter.Export(xGrid, save.FileName, WorkbookFormat.Excel97To2003);
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }


                }
            }
        }

        /// <summary>
        /// Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void exporter_ExportStarted(object sender, ExportStartedEventArgs e)
        {
            //Excel Exporter details
            //e.Workbook.DocumentProperties.Title = "Overview TEST******"
            e.CurrentWorksheet.DisplayOptions.PanesAreFrozen = true;
            e.CurrentWorksheet.DisplayOptions.FrozenPaneSettings.FrozenRows = 1;
            e.CurrentWorksheet.DisplayOptions.ShowGridlines = true;
            //e.CurrentWorksheet.DisplayOptions.GridlineColor = System.Drawing.Color.Red;
            //e.CurrentWorksheet.Name = "Customers TEST"
            //Throw New NotImplementedException
        }

        /// <summary>
        /// call function based on menu item selected
        /// "save to excel"
        /// "save grid settings"
        /// "reset grid settings"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem m = (MenuItem)sender;
            //Interaction.MsgBox(m.Header);

            switch (m.Name)
            {
                case "popupSaveToExcel":
                    ExportToExcel();
                    break;
                case "popupSaveGridSettings":
                    SaveGridSettings();
                    break;
                case "popupResetGridSettings":
                    ResetGridSettings();
                    break;
                case "popupToggleFilter":
                    ToggleFilters();
                    break;

            }

            //switch (m.Header.ToString().ToLower())
            //{
            //    case "save to excel":
            //        ExportToExcel();
            //        break;
            //    case "save grid settings":
            //        SaveGridSettings();
            //        break;
            //    case "reset grid settings":
            //        ResetGridSettings();
            //        break;
            //}
        }

        /// <summary>
        /// Sub to handle the selection style when the grid is clicked
        /// If IsRecordSelect is set to true then when the grid is clicked a record will be selected instead of a cell.
        /// this will not allow cell editing.
        /// If Select Multple rows is set to true then Shift and Ctrl will allow the user to highlight multiple rows
        /// </summary>
        /// <param name="IsRecordSelect"></param>
        /// <param name="SelectMultipleRows"></param>
        public void SetGridSelectionBehavior(bool IsRecordSelect, bool SelectMultipleRows)
        {
            //Set the click action to either select one record or select cell
            if (IsRecordSelect)
            {
                xGrid.FieldSettings.CellClickAction = CellClickAction.SelectRecord;
            }
            else
            {
                //xGrid.FieldSettings.CellClickAction = CellClickAction.SelectCell;
                xGrid.FieldSettings.CellClickAction = CellClickAction.EnterEditModeIfAllowed;
            }

            //Set the multi select on or off
            if (SelectMultipleRows)
            {
                xGrid.FieldLayoutSettings.SelectionTypeRecord = SelectionType.Extended;
            }
            else
            {
                xGrid.FieldLayoutSettings.SelectionTypeRecord = SelectionType.Single;
            }

            //xGrid.FieldLayoutSettings.SelectionTypeField = SelectionType.None
            //xGrid.FieldLayoutSettings.SelectionTypeCell = SelectionType.None

        }

        /// <summary>
        /// If individual cell is selected then add the row to the current selection list
        /// This tests to see if a filter row is clicked and if so ignores the sub
        /// If cell selected but the grid is setup for row select only then fire the zoom event
        /// Had to do this because IG grid was allowing cell selection on double click even though cell click action was set to record select
        /// Keeps cell from being made editable
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">e.Cell.Record.RecordType.ToString().Contains("Filter")</param>
        private void xGrid_CellActivating(object sender, Infragistics.Windows.DataPresenter.Events.CellActivatingEventArgs e)
        {
            //If individual cell is selected then add the row to the current selection list
            //This tests to see if a filter row is clicked and if so ignores the sub
            if (e.Cell.Record.RecordType.ToString().Contains("Filter"))
            {
                return;
            }
            xGrid.SelectedItems.Records.Clear();
            xGrid.SelectedItems.Records.Add(e.Cell.Record);
            /////////CANNOT DO THIS IN CELL ACTIVATING, MESSES UP SINGLE-CLICK EVENT AND ADDING NEW RECS TO GRID
            //If cell selected but the grid is setup for row select only then fire the zoom event
            //Had to do this because IG grid was allowing cell selection on double click even though cell click action was set to record select
            //if (xGrid.FieldSettings.CellClickAction == CellClickAction.SelectRecord)
            //{
            //    xGrid.SelectedItems.Cells.Clear();
            //    if (WindowZoomDelegate != null)
            //    {
            //        mWindowZoomDelegate();
            //    }
            //    //Keeps cell from being made editable
            //    e.Cancel = true;
            //}
            /////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        /// <summary>
        /// Activating event will allow for cells with edit mode set to false to be skipped on tab
        /// if SkipReadOnlyCellsOnTab is set to true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xGrid_CellActivated(object sender, Infragistics.Windows.DataPresenter.Events.CellActivatedEventArgs e)
        {
            if (SkipReadOnlyCellsOnTab)
            {
                //If grid in edit mode and tab to non editable field then skip to next field
                if (xGrid.FieldSettings.AllowEdit == true && e.Cell.Field.Settings.AllowEdit == false)
                    xGrid.ExecuteCommand(DataPresenterCommands.CellNextByTab);
            }

          }

        /// <summary>
        /// Need to set SelectionStart to move the cursor to the end of current text in the editor.
        /// Set SelectionLength so no text is actually selected.  This has the effect of leaving the cursor
        /// at the end of the last typed character.
        /// </summary>
        /// <param name="sender">The object that invokes the event.</param>
        /// <param name="e">The event arguments</param>
        void xGrid_EditModeStarted(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs e)
        {
            XamTextEditor editor = e.Editor as XamTextEditor;

            ////Check to see if we have an active cell
            //if (CurrentActiveEditCell == null && xGrid.ActiveCell != null)
            //    CurrentActiveEditCell = xGrid.ActiveCell;

            ////If grid has no active cell then exit
            //if (xGrid.ActiveCell == null)
            //    return;

            ////if grid cell is not in edit mode - skip ahead one cell
            //if (!xGrid.ActiveCell.IsInEditMode)
            //{
            //    xGrid.ExecuteCommand(DataPresenterCommands.CellNextByTab);
            //    if (xGrid.ActiveCell != null)
            //        CurrentActiveEditCell = xGrid.ActiveCell;
            //}
            //else
            //{
            //    CurrentActiveEditCell = xGrid.ActiveCell;
            //}


            if (editor != null)
            {
                //editor.SelectionStart = editor.Text.Length;
                //editor.SelectionLength = 0;
                editor.SelectAll();
            }
            else
            {
                XamNumericEditor numEditor = e.Editor as XamNumericEditor;
                if (editor != null)
                {
                    //editor.SelectionStart = editor.Text.Length;
                    //editor.SelectionLength = 0;
                    editor.SelectAll();
                }
            }
            //Run the local grid edit mode delegate if one exists
            if (EditModeStartedDelegate != null)
            {
                mEditModeStartedDelegate( sender,  e);
            }
        }

        private void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            //if text field and has a null value set to empty string instead
            //@@*******

            if (EditModeEndedDelegate != null)
            {
                mEditModeEndedDelegate(sender, e);
            }
            
        }


        /// <summary>
        /// Need to update the datasource each time a cell is changed.  This retains all the data
        /// that changes irrespective of UI navigation.  The grid commits data out of the box 
        /// when the row changes.
        /// </summary>
        /// <param name="sender">The object that invokes the event.</param>
        /// <param name="e">The event arguments</param>
        void xGrid_CellChanged(object sender, Infragistics.Windows.DataPresenter.Events.CellChangedEventArgs e)
        {
            //doesn't work either
            //xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);

            //if (!(e.Editor.GetType() == typeof(XamNumericEditor)))
            //{
            //    if (xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges))
            //    {
            //        if (e.Cell.Record.DataItemIndex >= 0)
            //        {
            //            xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //        }
            //        xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            //    }
            //}
        }

        void xGrid_RecordActivated(object sender, Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs e)
        {
            if (e.Record != null) // && xGrid.ActiveRecord != null && (xGrid.ActiveRecord as DataRecord).IsSelected)
            {
                ActiveRecord = xGrid.ActiveRecord as DataRecord;
                //Run the function passed from the parent window
                if (RecordActivatedDelegate  != null)
                {
                    mRecordActivatedDelegate();
                }
            }

            //if (e.Record != null && xGrid.DataContext != null)
            //{
            //    dynamic dataContext = xGrid.DataContext;

            //    if (dataContext != null && ActiveRecord != null && ActiveRecord.Index >= 0 && dataContext.GetType() == typeof(DataView))
            //    {
            //        if (ActiveRecord.Index <= ((DataView)dataContext).Table.Rows.Count - 1)
            //        {
            //            ActiveRow = dataContext.Table.Rows[ActiveRecord.Index];
            //        }                    
            //    }
            //    else if (dataContext != null && ActiveRecord != null && ActiveRecord.Index >= 0 && dataContext.GetType() == typeof(DataTable))
            //    {
            //        if (ActiveRecord.Index <= ((DataTable)dataContext).Rows.Count - 1)
            //        {
            //            ActiveRow = dataContext.Rows[ActiveRecord.Index];
            //        }
            //    }
            //    else
            //    {
            //        ActiveRow = null;
            //    }
            //}
        }

        void xGrid_InitializeRecord(object sender, Infragistics.Windows.DataPresenter.Events.InitializeRecordEventArgs e)
        {
            DataRecord record = (DataRecord)e.Record;

            foreach (Cell cell in record.Cells)
            {
                if (cell.Field.DataType == typeof(DateTime))
                {                    
                    DateTime minDate = DateTime.Parse("1/1/1901");                    
                    if ((DateTime)cell.Value <= minDate)
                    {
                        cell.Value = DateTime.Now;
                    }                    
                }
            }
             
        }
        
        /// <summary>
        /// saves grid settings to a config file
        /// </summary>
        public void SaveGridSettings()
        {
            if (!string.IsNullOrEmpty(ConfigFileName))
            {
                FileStream fs = new FileStream(ConfigFileName + ".xml", FileMode.Create, FileAccess.Write);
                xGrid.SaveCustomizations(fs);
            }
        }

        /// <summary>
        /// create config file for grid settings
        /// </summary>
        public void LoadGridSettings()
        {
            FileStream fs = null;
            if (!string.IsNullOrEmpty(ConfigFileName))
            {
                try
                {
                    fs = new FileStream(ConfigFileName + ".xml", FileMode.Open, FileAccess.Read);
                }
                catch
                {
                    return;
                }

                xGrid.LoadCustomizations(fs);
            }

        }

        /// <summary>
        /// Deletes config file for current grid
        /// </summary>
        public void ResetGridSettings()
        {
            try
            {
                System.IO.File.Delete(ConfigFileName + ".xml");
            }
            catch (Exception ex)
            {
                Interaction.MsgBox("Error deleteing cofiguration file - " + ex.Message);
            }

            ReloadGrid();

        }

        /// <summary>
        /// not in use
        /// </summary>
        public void ReloadGrid()
        {
        }

        /// <summary>
        ///clicked row on the grid become the selected row and runs parent delegate function
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">System.Windows.Input.MouseButtonEventArgs</param>
        private void xGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DependencyObject source = e.OriginalSource as DependencyObject;

            if (source == null)
                return;

            DataRecordPresenter presenter = Infragistics.Windows.Utilities.GetAncestorFromType(
                source, typeof(DataRecordPresenter), true) as DataRecordPresenter;

            if (presenter == null)
                return;

            CellValuePresenter cvp = Utilities.GetAncestorFromType(e.OriginalSource as DependencyObject, typeof(CellValuePresenter), false) as CellValuePresenter;
            //DWR 2/24/12 -- Added if logic as app would crash if doubleclick was made to header
            if (cvp != null && cvp.Field != null)
                DoubleClickFieldName = cvp.Field.Name;
            else
                return;


            //This was put in to keep double click on scroll bar from erroring
            //Index -1 prevents event from firing if filter row is double clicked.
            if (this.ActiveRecord != null && this.ActiveRecord.Index!=-1)
            {
                //Run the function passed from the parent window
                if (WindowZoomDelegate != null)
                {
                    mWindowZoomDelegate();
                }
            }
        }

        /// <summary>
        /// Determine if event was called on parent grid, if so call FilterChildren
        /// </summary>
        /// <param name="sender">XamDataGrid</param>
        /// <param name="e">Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs</param>
        private void xGrid_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            if (IsParent) FilterChildren(sender, e);
            //call single click delegate if applicable, also makes sure grid is not loading
            if (mSingleClickZoomDelegate != null && GridIsLoading == false)
            mSingleClickZoomDelegate();
        }

        /// <summary>
        /// iterate through child grid collection and run filter(s)
        /// </summary>
        /// <param name="sender">Cast to XamDatagrid</param>
        /// <param name="e">Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs</param>
        public void FilterChildren(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            FilterCall();
        }

        public void FilterCall()
        {
            DataRecord dataRecord = this.xGrid.ActiveRecord as DataRecord;

            var IsDataRecordGood = (dataRecord != null && dataRecord.Cells.Count > 0);

            foreach (var childSupportedGrid in this.ChildSupport)
            {
                var ColumnValueDictionary = new Dictionary<string, string>();

                //check that each Filtered column name is not null
                foreach (var ParentChildColumnNamesPair in childSupportedGrid.ParentChildFilterOnColumnNames)
                {
                    //ParentChildColumnNamesPair.Key is the Parent column name
                    //ParentChildColumnNamesPair.Value is the Child column name
                    if (IsDataRecordGood && dataRecord.Cells[ParentChildColumnNamesPair.Key].Value != null)
                        ColumnValueDictionary.Add(ParentChildColumnNamesPair.Value, dataRecord.Cells[ParentChildColumnNamesPair.Key].Value.ToString());
                    else
                        ColumnValueDictionary.Add(ParentChildColumnNamesPair.Value, "-11111");
                }

                //make sure value is not null
                foreach (ucBaseGrid childGrid in childSupportedGrid.ChildGrids)
                {
                    // filter the child grids
                    childGrid.FilterGrid(ColumnValueDictionary);
                }
            }
        }

        //Used for debugging
        //Private Sub xGrid_SelectedItemsChanging(ByVal sender As Object, ByVal e As Infragistics.Windows.DataPresenter.Events.SelectedItemsChangingEventArgs) Handles xGrid.SelectedItemsChanging
        //    MsgBox("Changing: " + CStr(xGrid.SelectedItems.Count))
        //End Sub

        //Private Sub xGrid_SelectedItemsChanged(ByVal sender As Object, ByVal e As Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs) Handles xGrid.SelectedItemsChanged
        //    MsgBox("Changed: " + CStr(xGrid.SelectedItems.Count))
        //End Sub

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SignalPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// insert grid item
        /// </summary>
        public void AddGridItem()
        {
            if (mContextMenuAddDelegate != null)
            {
                //call add delegate
                ContextMenuAddDelegate();
            }
        }

        /// <summary>
        /// delete grid item
        /// </summary>
        public void RemoveGridItem()
        {
            if (mContextMenuRemoveDelegate != null)
                //call remove delegate
                ContextMenuRemoveDelegate();
        }

        /// <summary>
        /// generic grid item
        /// </summary>
        public void GenericGridItem1()
        {
            if (mContextMenuGenericDelegate1 != null)
            {
                //call generic delegate
                ContextMenuGenericDelegate1();
            }
        }

        /// <summary>
        /// generic grid item
        /// </summary>
        public void GenericGridItem2()
        {
            if (mContextMenuGenericDelegate2 != null)
            {
                //call generic delegate
                ContextMenuGenericDelegate2();
            }
        }

        /// <summary>
        /// generic grid item
        /// </summary>
        public void GenericGridItem3()
        {
            if (mContextMenuGenericDelegate3 != null)
            {
                //call generic delegate
                ContextMenuGenericDelegate3();
            }
        }

        public void GenericGridItem4()
        {
            if (mContextMenuGenericDelegate4 != null)
            {
                //call generic delegate
                ContextMenuGenericDelegate4();
            }
        }

        /// <summary>
        /// Toggles grid filters on and off
        /// </summary>
        public void ToggleFilters()
        {
            if (this.IsFilterable != null)
            {
                if (this.IsFilterable.Value == true)
                    this.IsFilterable = false;
                else
                    this.IsFilterable = true;
            }
            else
                this.IsFilterable = true;
        }

        /// <summary>
        /// This is used to initialize context menu routed commands and for
        /// customizing grid 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //add custom names to menu items if applicable
            checkForCustomMenuNames();
            //check to see if menu item(s) need to be hidden
            checkForContextMenuHides();
            //check to see if custom icons are being used
            checkForContextMenuGraphicSwaps();
            cCommandLibrary.InitializePopUpCommands(this);
        }

        /// <summary>
        /// checks and establishes custom context menu names
        /// </summary>
        private void checkForCustomMenuNames()
        {
            //add custom names to menu items
            if (ContextMenuSaveToExcelDisplayName != null)
                popupSaveToExcel.Header = ContextMenuSaveToExcelDisplayName;
            if (ContextMenuSaveGridSettingsDisplayName != null)
                popupSaveGridSettings.Header = ContextMenuSaveGridSettingsDisplayName;
            if (ContextMenuResetGridSettingsDisplayName != null)
                popupResetGridSettings.Header = ContextMenuResetGridSettingsDisplayName;
            if (ContextMenuAddDisplayName  != null)
                popupAddRecord.Header = ContextMenuAddDisplayName;
            if (ContextMenuRemoveDisplayName != null)
                popupRemoveRecord.Header = ContextMenuRemoveDisplayName;
            //Generic 1
            if (ContextMenuGenericDisplayName1 != null)
                popupGeneric1.Header = ContextMenuGenericDisplayName1;
            //Generic 2
            if (ContextMenuGenericDisplayName2 != null)
                popupGeneric2.Header = ContextMenuGenericDisplayName2;
            //Generic 3
            if (ContextMenuGenericDisplayName3 != null)
                popupGeneric3.Header = ContextMenuGenericDisplayName3;
            //Generic 4
            if (ContextMenuGenericDisplayName4 != null)
                popupGeneric4.Header = ContextMenuGenericDisplayName4;
            //Toggle Grid Filter
            //if (ContextMenuToggleFilterDisplayName != null)
            //    popupToggleFilter.Header = ContextMenuToggleFilterDisplayName;
        }

        public void checkForContextMenuHides()
        {
            //check to see if a menu item needs to be hidden
            if (ContextMenuSaveToExcelIsVisible != null)
                if (ContextMenuSaveToExcelIsVisible == false)
                    popupSaveToExcel.Visibility = System.Windows.Visibility.Collapsed;
                else
                    popupSaveToExcel.Visibility = System.Windows.Visibility.Visible;
            if (ContextMenuSaveGridSettingsIsVisible != null)
                if (ContextMenuSaveGridSettingsIsVisible == false)
                    popupSaveGridSettings.Visibility = System.Windows.Visibility.Collapsed;
                else
                    popupSaveGridSettings.Visibility = System.Windows.Visibility.Visible;
            if (ContextMenuResetGridSettingsIsVisible != null)
                if (ContextMenuResetGridSettingsIsVisible == false)
                    popupResetGridSettings.Visibility = System.Windows.Visibility.Collapsed;
                else
                    popupResetGridSettings.Visibility = System.Windows.Visibility.Visible;
            if (ContextMenuAddIsVisible != null)
                if (ContextMenuAddIsVisible == false)
                    popupAddRecord.Visibility = System.Windows.Visibility.Collapsed;
                else
                    popupAddRecord.Visibility = System.Windows.Visibility.Visible;
            if (ContextMenuRemoveIsVisible != null)
                if (ContextMenuRemoveIsVisible == false)
                    popupRemoveRecord.Visibility = System.Windows.Visibility.Collapsed;
                else
                    popupRemoveRecord.Visibility = System.Windows.Visibility.Visible;
            //Generic 1
            if (ContextMenuGenericIsVisible1 != null)
                if (ContextMenuGenericIsVisible1 == false)
                    popupGeneric1.Visibility = System.Windows.Visibility.Collapsed;
                else
                    popupGeneric1.Visibility = System.Windows.Visibility.Visible;
            //Generic 2
            if (ContextMenuGenericIsVisible2 != null)
                if (ContextMenuGenericIsVisible2 == false)
                    popupGeneric2.Visibility = System.Windows.Visibility.Collapsed;
                else
                    popupGeneric2.Visibility = System.Windows.Visibility.Visible;
            //Generic 3
            if (ContextMenuGenericIsVisible3 != null)
                if (ContextMenuGenericIsVisible3 == false)
                    popupGeneric3.Visibility = System.Windows.Visibility.Collapsed;
                else
                    popupGeneric3.Visibility = System.Windows.Visibility.Visible;
            //Generic 4
            if (ContextMenuGenericIsVisible4 != null)
                if (ContextMenuGenericIsVisible4 == false)
                    popupGeneric4.Visibility = System.Windows.Visibility.Collapsed;
                else
                    popupGeneric4.Visibility = System.Windows.Visibility.Visible;
            //Toggle Filter
            if (ContextMenuToggleFilterIsVisible != null)
                if (ContextMenuToggleFilterIsVisible == false)
                    popupToggleFilter.Visibility = System.Windows.Visibility.Collapsed;
                else
                    popupToggleFilter.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// allows you to change to image for the context menu icon
        /// </summary>
        private void checkForContextMenuGraphicSwaps()
        {
            if (ContextMenuGenericImageSwap1 != null)
            {
                Uri imgURI = new Uri(ContextMenuGenericImageSwap1, UriKind.Relative);
                popupGenericImage1.Source = new System.Windows.Media.Imaging.BitmapImage(imgURI);
            }
            //if (ContextMenuGenericImageSwap1 != null)
            //{
            //    Uri imgURI = new Uri(ContextMenuGenericImageSwap2, UriKind.Relative);
            //    popupGenericImage2.Source = new System.Windows.Media.Imaging.BitmapImage(imgURI);
            //}
            //if (ContextMenuGenericImageSwap1 != null)
            //{
            //    Uri imgURI = new Uri(ContextMenuGenericImageSwap3, UriKind.Relative);
            //    popupGenericImage3.Source = new System.Windows.Media.Imaging.BitmapImage(imgURI);
            //}
        }
        private int _cbTabIndex;

        public int CntrlTabIndex
        {
            get { return _cbTabIndex; }
            set
            {
                _cbTabIndex = value;
                xGrid.TabIndex = value;
            }
        }
        public void CntrlFocus()
        {
            xGrid.Focus();
        }

        private void ucBaseGrid1_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CntrlFocus();
        }

        public void SetRowCellFocus(DataRecord record, string columnName)
        {
            //clear all selected records
            xGrid.ExecuteCommand(DataPresenterCommands.ClearAllSelected);

            
            //give gird focus
            this.CntrlFocus();

            //get index of the record
            int indexOfRecord = xGrid.Records.IndexOf(record);

            int indexOfColumn = xGrid.FieldLayouts[0].Fields.IndexOf(columnName);

            //set selected record
            if (indexOfRecord != -1)
            {
                xGrid.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
                Cell activeCell = record.Cells[indexOfColumn];
                
                xGrid.ActiveCell = activeCell;
                activeCell.IsActive = true;
                xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            }
        }

        /// <summary>
        /// ****This method is not working properly
        /// </summary>
        private void TabToNextEditableColumn()
        {
            int LastIndex = (xGrid.ActiveCell == null ? -1 : xGrid.ActiveCell.Field.Index);
            xGrid.ExecuteCommand(DataPresenterCommands.CellNextByTab);

            while (xGrid.ActiveCell.Field.Settings.AllowEdit == false && LastIndex != xGrid.ActiveCell.Field.Index && xGrid.ActiveCell.Field.Index < xGrid.ActiveRecord.FieldLayout.Fields.Count)
            {
                LastIndex = xGrid.ActiveCell.Field.Index;
                xGrid.ExecuteCommand(DataPresenterCommands.CellNextByTab);
            }

            //backup one because the grid will execute the tab after this runs
            xGrid.ExecuteCommand(DataPresenterCommands.CellPreviousByTab);

            xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }

        public bool DoNotSelectFirstRecordOnLoad { get; set; }


        //****Start of code to handle drag and drop of a base grid
        //To use the base grid must have AllowWindowDrag set to true to be able to drag it
        //The destination user control will need to have its AllowDrop property set to true
        //and a drop event created

        //This is set to true after the user clicks the left mouse button and holds while moving a minimu distance
        private bool IsDragging;
        //This represents the point the left mouse button was first pressed
        private Point startPoint;

        /// <summary>
        /// If the window is allowed to be dragged the start point of the down press is measured if it was not releated to a double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucBaseGrid1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //If a double click then exit this event and run the double click event.
            //if (e.ClickCount > 1)
            //{
            //    this.xGrid_MouseDoubleClick(sender, e);
            //}
                //If the grid is setup to be dragged then get the starting point of the drag
            if (AllowWindowDrag)
            {
                startPoint = e.GetPosition(null);
            }

        }

        /// <summary>
        /// If the grid is set to allow drag and drop and the mouse is pressed and dragged for a minimum distance then
        /// the StartDrag event is fired.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucBaseGrid1_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (AllowWindowDrag)
            {
                if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
                {
                    Point position = e.GetPosition(null);

                    if (Math.Abs(position.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(position.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        StartDrag(e);
                    }
                }
            }

        }

        /// <summary>
        /// Sets the isdragging variable to true and populates the drag object with the ucBaseGrid control
        /// </summary>
        /// <param name="e"></param>
        private void StartDrag(MouseEventArgs e)
        {
            IsDragging = true;
            DataObject data = new DataObject(this);
            DragDropEffects de = DragDrop.DoDragDrop( this, data, DragDropEffects.Move);
            IsDragging = false;
         }

        private void ucBaseGrid1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(IsDragging)
                IsDragging = false;
        }

        

        private void xGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (GridGotFocusDelegate != null)
                GridGotFocusDelegate(sender,e);
        }

        /// <summary>
        /// Sets the security for the context menus - disabling menu items that the user is not allowed to run
        /// This is launched from screenbase
        /// Currently disables by the menu item name
        /// Add/Insert for inserts
        /// Delete/Remove for delete
        /// </summary>
        /// <param name="SecurityAccess"></param>
        public void SetSecurity(AccessLevel SecurityAccess)
        {
            ParentSecurityLevel = SecurityAccess;
            if (SecurityAccess == AccessLevel.ViewOnly || SecurityAccess == AccessLevel.NoAccess )
            {
                DisableMenuItems(MenuDisableType.Add);
                DisableMenuItems(MenuDisableType.Delete);
            }
            else if (SecurityAccess == AccessLevel.ViewUpdate)
            {
                DisableMenuItems(MenuDisableType.Delete);
                DisableMenuItems(MenuDisableType.Add,true);
            }
            else if (SecurityAccess == AccessLevel.ViewUpdateDelete)
            {
                DisableMenuItems(MenuDisableType.Delete, true);
                DisableMenuItems(MenuDisableType.Add, true);
            }
            
        }

        /// <summary>
        /// Used to disable menu items based on type - either add or delete
        /// </summary>
        /// <param name="ToDisable"></param>
        /// <param name="Enable">Defaults to False - Disable - Set to true if the menu options should be enabled</param>
        private void DisableMenuItems(MenuDisableType ToDisable, bool Enable = false)
        {
            if (ToDisable == MenuDisableType.Add)
            {
                foreach (MenuItem m in mPopup.Items)
                {
                    if (m.Header.ToString().ToLower().Contains("add") || m.Header.ToString().ToLower().Contains("insert"))
                    {
                        m.IsEnabled = Enable;
                    }
                }
            }
            else if (ToDisable == MenuDisableType.Delete)
            {
                foreach (MenuItem m in mPopup.Items)
                {
                    if (m.Header.ToString().ToLower().Contains("delete") || m.Header.ToString().ToLower().Contains("remove"))
                    {
                        m.IsEnabled = Enable;
                    }
                }
            }
        }

        private void xGrid_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        {
            if (CellUpdatedDelegate != null)
                CellUpdatedDelegate(sender, e);
        }

        private void xGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (GridLoadedDelegate != null)
                GridLoadedDelegate(sender, e);
        }

        private void xGrid_SelectedItemsChanging(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangingEventArgs e)
        {
            if (SelectedItemsChangingDelegate != null)
                SelectedItemsChangingDelegate(sender, e);
        }

  

  
       
    }

    public class ChildSupport
    {
        public List<string> ParentFilterOnColumnNames { get; set; }
        public List<string> ChildFilterOnColumnNames { get; set; }
        public List<ucBaseGrid> ChildGrids { get; set; }

        private Dictionary<string, string> _ParentChildFilterOnColumnNames;
        public Dictionary<string, string> ParentChildFilterOnColumnNames
        {
            get
            {
                _ParentChildFilterOnColumnNames = new Dictionary<string, string>();

                if (ParentFilterOnColumnNames.Count != ChildFilterOnColumnNames.Count)
                    throw new Exception("ParentChild filters for ucBaseGrid must have the same number of columns.");

                for (int i = 0; i < ParentFilterOnColumnNames.Count; i++)
                {
                    _ParentChildFilterOnColumnNames.Add(ParentFilterOnColumnNames[i], ChildFilterOnColumnNames[i]);
                }

                return _ParentChildFilterOnColumnNames;
            }
            private set
            {
                _ParentChildFilterOnColumnNames = value;
            }
        }

        public ChildSupport()
        {
            ParentFilterOnColumnNames = new List<string>();
            ChildFilterOnColumnNames = new List<string>();
            ChildGrids = new List<ucBaseGrid>();
        }
     

    }
}
