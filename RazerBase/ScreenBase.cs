using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Controls;
using RazerInterface;
using System.Windows;
using Infragistics.Windows.DockManager;
using System.Linq;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using System.Drawing.Printing;
using System.Windows.Media;
using System.Printing;



namespace RazerBase
{
    /// <summary>
    /// This class is the main base object that all screens inherit from.
    /// </summary>
    public class ScreenBase : UserControl
    {

        private bool IsSecuritySet = false;
        private string mScreenName;
        private bool mIsLookupWindow;
        private string mMainTableName;
        private List<ScreenBase> mTabCollections = new List<ScreenBase>();
        private List<ucBaseGrid> mGridCollection = new List<ucBaseGrid>();
        private cBaseBusObject mCurrentBusObj;
        private ScreenBaseTypeEnum screenBaseType;
        private bool doNotSetDataContext;
        private ControlSecurity ControlSec;
        private AccessLevel mSecurityContext;
        private string mScreenBaseParentObjName;
        public bool SaveSuccessful { get; protected set; }
        public List<BoundTable> BoundTables { get; private set; }
        private int? ParentID = 0;

        //this is for shared objects whose object names aren't unique; like attachments and comments
        public string RootParentObjectType { get; set; }

        public bool ScreenBaseIsClosing = false;
        public bool BypassBindafterSave = false;

        public bool CancelCloseOnSaveConfirmation { get; set; }
        public bool StopCloseIfCancelCloseOnSaveConfirmationTrue { get; protected set; }
        public ScreenState CurrentState { get; set; } //Enumeration used to track the current screen state
        public bool HasPrintReport { get; set; } //Set to true if the screen has a screen report tied to it
        public List<string> PrintReportParms { get; set; } //String list that holds the report parms for any print report
        public string PrintReportJobName { get; set; } //String that holds the report job name

        /// <summary>
        /// Use this property to override the IsScreenDirty calculation
        /// If this is set to true then the screen will force a save check on close.
        /// </summary>
        public bool ForceScreenDirty { get; set; }

        /// <summary>
        /// Checks to see if any datatables that have insert update or delete procedures tied to them have changed
        /// </summary>
        public bool IsScreenDirty
        {
            get
            {
                var TF = false;
                //DWR--2/22/12 - Modified to use Datachanged method in base business objects instead of HasChanges method of dataset
                if (this.mCurrentBusObj != null && this.mCurrentBusObj.DataChanged())
                {
                    TF = true;
                }
                return TF;
            }
        }

        private bool IsInDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(this);
            }
        }

        private bool canExecuteNewCommand;
        private bool canExecuteSaveCommand;
        private bool canExecuteCloseCommand;
        private bool canExecuteDeleteCommand;
        private bool canExecuteRefreshCommand;

        public bool CanExecuteNewCommand
        {
            get { return canExecuteNewCommand; }
            set { canExecuteNewCommand = value; }
        }

        public bool CanExecuteSaveCommand
        {
            get { return canExecuteSaveCommand; }
            set { canExecuteSaveCommand = value; }
        }

        public bool CanExecuteCloseCommand
        {
            get { return canExecuteCloseCommand; }
            set { canExecuteCloseCommand = value; }
        }

        public bool CanExecuteDeleteCommand
        {
            get { return canExecuteDeleteCommand; }
            set { canExecuteDeleteCommand = value; }
        }

        public bool CanExecuteRefreshCommand
        {
            get { return canExecuteRefreshCommand; }
            set { canExecuteRefreshCommand = value; }
        }

        private bool objectSecurityFound = false;

        /// <summary>
        /// Default constructor no parms
        /// </summary>
        public ScreenBase()
        {
            SaveSuccessful = true;
            this.Loaded += new RoutedEventHandler(ScreenBase_Loaded);
            BoundTables = new List<BoundTable>();

            //set default to null for RootParentObjectType
            RootParentObjectType = null;

            //Default the UC to not be a tab item or a lookup window
            mIsLookupWindow = false;
            //set SBType as unknown
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;
            //Get/Set Security Context
            //if (this.ToString() != null)
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

            ControlSec = RazerBase.ApplicationSecurityManager.ObjAccess(this.ToString(), out objectSecurityFound);
            mSecurityContext = ControlSec.AccessLevel;
            this.ParentID = ControlSec.ParentID;

            //initialize commands 
            canExecuteNewCommand = true;
            canExecuteSaveCommand = false;
            canExecuteCloseCommand = true;
            canExecuteDeleteCommand = false;
            canExecuteRefreshCommand = true;
            CurrentState = ScreenState.Normal;
            ForceScreenDirty = false;
            HasPrintReport = false;
            PrintReportParms = new List<string>();
            PrintReportJobName = "";
            
        }

        //**DWR Modified 8/28/12 - to change from private to protected so that the method could be used by decendent tabs to find their parent folder.
        protected ScreenBase findRootScreenBase(ScreenBase current)
        {
            ScreenBase sb = UIHelper.FindVisualParent<ScreenBase>(current);

            if (sb == null)
                sb = current;
            else
                if (!sb.objectSecurityFound)
                    sb = findRootScreenBase(sb);

            return sb;
        }

        public void CallScreenClose()
        {
            cMainWindowBase mainWindow = UIHelper.FindVisualParent<cMainWindowBase>(this);

            // if the mainWindow was found
            if (mainWindow != null)
            {
                // if the currentScreen exists
                DocumentContentHost DocHost = UIHelper.FindChild<DocumentContentHost>(mainWindow, "DocHost");
                ScreenBase currentScreen = DocHost.ActiveDocument.Content as ScreenBase;

                DocHost.ActiveDocument.ExecuteCommand(ContentPaneCommands.Close);
            }

        }

        private void ScreenBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (!objectSecurityFound) //look for root permissions in object was not found explicitly
            {
                ScreenBase sb = findRootScreenBase(this);
                if (sb != null)
                {
                    this.ControlSec = sb.ControlSec;
                    this.mSecurityContext = sb.mSecurityContext;
                }
            }
            else if (RootParentObjectType != null)
            {
                var ParentControlSec = RazerBase.ApplicationSecurityManager.ObjAccess(RootParentObjectType, out objectSecurityFound);

                this.ControlSec = RazerBase.ApplicationSecurityManager.ObjAccess(this.ToString(), out objectSecurityFound, ParentControlSec.ObjectID);
                mSecurityContext = ControlSec.AccessLevel;
                this.ParentID = ControlSec.ParentID;

                //reset security for the tab
                foreach (Grid grid in this.FindChildren<Grid>())
                {
                    foreach (NoAccess noAccess in grid.FindChildren<NoAccess>())
                    {
                        grid.Children.Remove(noAccess);
                    }
                }

                if (SecurityContext == AccessLevel.NoAccess)
                    this.IsEnabled = false;
                else
                    this.IsEnabled = true;

                IsSecuritySet = false;
                SetSecurity();
                
 

            }

            if (this.ParentID == 0 && !this.IsInDesignMode)
            {
                SetSecurity();
            }
            cGlobals.ReturnParms.Clear();
        }

        public void SetSecurity()
        {
            //took this out of the if test can't remember why it was needed -- seems to not be...
            //this.CurrentBusObj != null && this.CurrentBusObj.HasObjectData && 
            if (!IsSecuritySet)
            {
                IsSecuritySet = true;

                if (this.ScreenBaseType != ScreenBaseTypeEnum.Tab && SecurityContext == AccessLevel.NoAccess)
                {
                    Messages.ShowInformation("Your permissions are not sufficient to view this screen.");
                    this.CallScreenClose();
                    return;
                }

                switch (SecurityContext)
                {
                    case AccessLevel.NoAccess:
                        this.IsEnabled = false;
                        foreach (Grid grid in this.FindChildren<Grid>())
                        {
                            int gridRows = grid.RowDefinitions.Count();
                            int gridColumns = grid.ColumnDefinitions.Count();

                            NoAccess noAccess = new NoAccess();
                            Grid.SetColumn(noAccess, 0);
                            Grid.SetRow(noAccess, 0);
                            if (gridColumns > 0)
                                Grid.SetColumnSpan(noAccess, gridColumns);

                            if (gridRows > 0)
                                Grid.SetRowSpan(noAccess, gridRows);

                            Grid.SetZIndex(noAccess, int.MaxValue);
                            grid.Children.Add(noAccess);
                            break;
                        }
                        break;
                    case AccessLevel.ViewOnly:
                        break;
                    case AccessLevel.ViewUpdate:
                        break;
                    case AccessLevel.ViewUpdateDelete:
                        break;
                    default:
                        break;
                }

                foreach (ScreenBase tab in TabCollection)
                {
                    if (tab.ParentID == null && !tab.objectSecurityFound)
                        tab.SecurityContext = this.SecurityContext;
                    tab.SetSecurity();
                }

                //Added 2/6/12 to run security checks against grids in the grid collection
                foreach (ucBaseGrid uGrid in GridCollection)
                {
                    uGrid.SetSecurity(SecurityContext);
                }
            }
        }

        /// <summary>
        /// This method [enter description here].
        /// </summary>
        public virtual void Delete()
        {
            //    Interaction.MsgBox("Delete Ancestor");
        }

        private void LoadTable(string tableName)
        {
            this.Load(tableName);
        }

        /// <summary>
        /// This method loads the businessObject
        /// </summary>
        protected virtual void Load(cBaseBusObject businessObject, bool isNewBusinessObjectInstance = false, string tableName = null)
        {
            // verfiy the object exists
            if ((businessObject != null) && (this.HasMainTableName))
            {
                //for security so we know which tables are bound to this screenbase
                addBoundTable(this.MainTableName, this.ToString());

                // load the data for this object
                if (isNewBusinessObjectInstance)
                    businessObject.LoadNewBusinessObjectInstance();
                else
                    businessObject.LoadData(tableName);
                // if the object data exists
                if (businessObject.HasObjectData)
                {
                    BindToBusinessObjectData(businessObject);
                }
                else
                {
                    // raise an exception data not loaded
                    string message = "Data not loaded in the business object '" + businessObject.BusObjectName + "'.";
                    //throw new Exception(message);
                }

            }
        }

        private void addBoundTable(string TableName, string ScreenBaseName)
        {
            int iCount = (from x in BoundTables
                          where x.ScreenBaseName == ScreenBaseName && x.TableName == TableName
                          select x).Count();
            if (iCount == 0)
            {
                BoundTables.Add(new BoundTable { TableName = TableName, ScreenBaseName = ScreenBaseName });
            }
        }

        private void BindToBusinessObjectData(cBaseBusObject businessObject)
        {
            // if the tab is IPreBindable
            IPreBindable mainPreBindableObject = this as IPreBindable;
            IPostBindable mainPostBindableObject = this as IPostBindable;

            // if the preBindableObject exists
            if (mainPreBindableObject != null)
            {
                // call prebind
                mainPreBindableObject.PreBind();
            }
            //By default the DataContext is set, unless you turn it off.
            //if ((!this.DoNotSetDataContext) && (this.HasMainTableName))
            if (!this.DoNotSetDataContext)
            {
                // Set the DataContext
                this.DataContext = businessObject.ObjectData.Tables[MainTableName];
            }

            // If this object has any tabs then loop through all of the tabs
            if (this.HasOneOrMoreTabs)
            {
                // iterate the tabs
                foreach (ScreenBase tab in TabCollection)
                {
                    // if the MainTableName exists
                    if (!String.IsNullOrEmpty(tab.MainTableName))
                    {
                        //for security so we know which tables are bound to this screenbase
                        addBoundTable(tab.MainTableName, tab.ToString());

                        // Set the CurrentBusinessObject
                        tab.CurrentBusObj = businessObject;

                        // if the tab is IPreBindable
                        IPreBindable preBindableObject = tab as IPreBindable;
                        IPostBindable postBindableObject = tab as IPostBindable;
                        // if the preBindableObject exists
                        if (preBindableObject != null)
                        {
                            // call prebind
                            preBindableObject.PreBind();
                        }

                        // Set the DataContext
                        if (!tab.DoNotSetDataContext)
                        {
                            tab.DataContext = businessObject.ObjectData.Tables[tab.MainTableName];
                        }

                        // if the tab has grids
                        if (tab.HasOneOrMoreGrids)
                        {
                            // load the grids for this tab
                            LoadGrids(tab);
                        }
                        if (postBindableObject != null)
                        {
                            postBindableObject.PostBind();
                        }
                    }
                }
            }

            // Load the Grids if this object has any 
            LoadGrids(this);
            if (mainPostBindableObject != null)
            {
                mainPostBindableObject.PostBind();
            }
        }

        /// <summary>
        /// This method loads this object
        /// </summary>
        protected virtual void Load(string tableName = null)
        {
            //check for nulls
            if (CurrentBusObj == null)
            {
                // exit for now, should an error be raised ?
                return;
            }

            // call override for this method
            this.Load(this.CurrentBusObj, false, tableName);
        }

        /// <summary>
        /// This method loads the grids for an object that has a GridCollection
        /// </summary>
        private void LoadGrids(ScreenBase screen)
        {
            // if the Screen object exists has one or more grids and has a current business object
            if ((screen != null) && (screen.HasOneOrMoreGrids) && (screen.HasCurrentBusObj))
            {
                //If a grid is on the current folder or tab then loop through 
                foreach (ucBaseGrid grid in screen.GridCollection)
                {
                    //for security so we know which tables are bound to this screenbase
                    addBoundTable(grid.MainTableName, screen.ToString());

                    // load the grid
                    grid.LoadGrid(this.CurrentBusObj, grid.MainTableName);

                    //add child grids to collection
                    foreach (var ChildGridCollection in grid.ChildSupport)
                    {
                        foreach (ucBaseGrid childGrid in ChildGridCollection.ChildGrids)
                        {
                            //call child support
                            childGrid.LoadGrid(this.CurrentBusObj, childGrid.MainTableName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reloads the CurrentBusinessObject
        /// Can be overwritten at the descendent
        /// </summary>
        public virtual void Refresh()
        {
            if (CurrentBusObj == null)
                return;
            else
            {
                if (this.IsScreenDirty)
                {
                    System.Windows.MessageBoxResult result = Messages.ShowYesNo("Any Changes to current screen / folder will be lost. Continue?",
                                   System.Windows.MessageBoxImage.Question);
                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        Load(CurrentBusObj);
                    }
                    else
                        return;
                }
                else
                    Load(CurrentBusObj);
            }

        }

        /// <summary>
        /// This method is used to Save this object.
        /// </summary>
        public virtual void Save()
        {
            Prep_ucBaseGridsForSave();
            PrepareFreeformForSave();

            // if the Current Business Object exists
            if (this.HasCurrentBusObj && ShouldExecuteSave())
            {
                // Save the current data set
                if (this.CurrentBusObj.Save())
                {
                    SaveSuccessful = true;
                    if (BypassBindafterSave == false)
                        //Bind the new data
                        this.BindToBusinessObjectData(this.CurrentBusObj);
                }
                else
                {
                    SaveSuccessful = false;
                }
            }
            //DWR Added - 3/29/13 - To fix issue where RAZER says save was successful when nothing was saved 
            //due to invalid security or validation errors.
            else
            {
                SaveSuccessful = false;
            }
        }

        public virtual void PrintReport()
        {
            //Reprort Job anme and any parms should be set in the child screenbase before base.PrintReport() is called
            PrinterSettings settings = new PrinterSettings();
            //If no printer settings then send error and return
            if (settings == null || settings.PrinterName == null || settings.PrinterName == "")
            {
                Messages.ShowError("Cannot find printer.  Please verify that you have a default printer configured.");
            }
            else //Configure and schedule job
            {
                string jobParms = "";

                foreach (string s in PrintReportParms)
                {
                    jobParms = jobParms + "/A " + s + " ";
                }

                jobParms = jobParms + "/A " + settings.PrinterName;

                if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, PrintReportJobName, jobParms, DateTime.Now, cGlobals.UserName.ToString()) == true)
                {
                    Messages.ShowInformation("Screen Report Sent to Printer  " );
                }
                else
                    Messages.ShowWarning("Screen Print Error");
            }
            //Cleanup print report variables
            PrintReportParms.Clear();
            PrintReportJobName = "";
        }

        //public virtual void PrintScreen()
        //{
        //    //RES 3/19/15 add print screen cabability
        //    //MessageBox.Show("Print Screen");
        //    //PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
        //    //if (printDlg.ShowDialog() == true)
        //    //{
        //    //    printDlg.PrintVisual(this, "WPF Print");
        //    //}
        //    PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
        //    if (printDlg.ShowDialog() == true)
        //    {
        //        //get selected printer capabilities
        //        System.Printing.PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

        //        //get scale of the print wrt to screen of WPF visual
        //        double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
        //                       this.ActualHeight);

        //        //Transform the Visual to scale
        //        this.LayoutTransform = new ScaleTransform(scale, scale);

        //        //get the size of the printer page
        //        Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

        //        //update the layout of the visual to the printer page size.
        //        this.Measure(sz);
        //        this.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

        //        //now print the visual to printer to fit on the one page.
        //        printDlg.PrintVisual(this, "First Fit to Page WPF Print");

        //    }

        //}

        public virtual void PrintScreen()
        {
            PrinterSettings settings = new PrinterSettings();
            //If no printer settings then send error and return
            if (settings == null || settings.PrinterName == null || settings.PrinterName == "")
            {
                Messages.ShowError("Cannot find printer.  Please verify that you have a default printer configured.");
                return;
            }
           
            //Visual v = System.Windows.FrameworkElement;
            //System.Windows.FrameworkElement e = v as System.Windows.FrameworkElement;
            System.Windows.FrameworkElement e = this as System.Windows.FrameworkElement;
            if (e == null)
                return;

            PrintDialog pd = new PrintDialog();
            //if (pd.ShowDialog() == true)
            //{
                //set page orientation to landscape
                pd.PrintTicket.PageOrientation = PageOrientation.Landscape;
                //store original scale
                Transform originalScale = e.LayoutTransform;
                //get selected printer capabilities
                System.Printing.PrintCapabilities capabilities = pd.PrintQueue.GetPrintCapabilities(pd.PrintTicket);

                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / e.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                               e.ActualHeight);

                //Transform the Visual to scale
                e.LayoutTransform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                System.Windows.Size sz = new System.Windows.Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                e.Measure(sz);
                e.Arrange(new System.Windows.Rect(new System.Windows.Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                //now print the visual to printer to fit on the one page.
                pd.PrintVisual(this, "My Print");

                //apply the original transform.
                e.LayoutTransform = originalScale;
            //}
        }

        public void PrepareFreeformForSave()
        {
            object ob = Keyboard.FocusedElement;
            if (ob is TextBox)
            {
                TextBox tb = ob as TextBox;
                BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }

        }

        public void Prep_ucBaseGridsForSave()
        {
            /*
             * this goes throug each ucBaseGrid and:
             *  1.  Ends the edit mode for any open cell and commits the cell change to the datasource
             *  2.  Saves the current UpdateMode of the xamDataGrid
             *  3.  Changes the current UpdateMode of the xamDataGrid to OnUpdate so CommitChangesToAllRecords command will work
             *  4.  Restorse the UpdateMode of the xamDataGrid to it's previous state
             */
            foreach (ucBaseGrid bg in UIHelper.FindChildren<ucBaseGrid>(this))
            {
                bg.xGrid.ExecuteCommand(Infragistics.Windows.DataPresenter.DataPresenterCommands.EndEditModeAndAcceptChanges);

                Infragistics.Windows.DataPresenter.UpdateMode currentMode = bg.xGrid.UpdateMode;

                bg.xGrid.UpdateMode = Infragistics.Windows.DataPresenter.UpdateMode.OnUpdate;
                bg.xGrid.ExecuteCommand(Infragistics.Windows.DataPresenter.DataPresenterCommands.CommitChangesToAllRecords);

                bg.xGrid.UpdateMode = currentMode;
            }
        }
        public virtual void SaveTable(string TableName)
        {
            // if the Current Business Object exists
            if (this.HasCurrentBusObj && ShouldExecuteSave(TableName))
            {
                // Save the current data set
                if (this.CurrentBusObj.SaveTable(TableName))
                {
                    SaveSuccessful = true;
                    //Bind the new data
                    this.BindToBusinessObjectData(this.CurrentBusObj);
                }
                else
                {
                    SaveSuccessful = false;
                }
            }
        }

        public virtual bool ShouldExecuteSave(string TableName = null)
        {
            bool retVal = false;

            if (this.CurrentBusObj.ObjectData.HasChanges())
            {
                List<string> SecuredTables = new List<string>();
                List<ScreenBase> sbList = new List<ScreenBase>(UIHelper.FindChildren<ScreenBase>(this));
                sbList.Add(this);

                foreach (ScreenBase sb in sbList)
                {
                    List<string> BoundTablesInSB = (from x in this.BoundTables
                                                    where x.ScreenBaseName == sb.ToString()
                                                    select x.TableName).ToList();

                    foreach (string boundTableName in BoundTablesInSB)
                    {
                        if (!SecuredTables.Contains(boundTableName))
                        {
                            SecuredTables.Add(boundTableName);

                            if (TableName == null || TableName == boundTableName)
                            {
                                DataTable table = this.CurrentBusObj.ObjectData.Tables[boundTableName];

                                if (table != null && !(table.TableName == "cTransactionErrors" || table.TableName == "ParmTable" || table.TableName == "BuisnessObjectName"))
                                {
                                    switch (sb.SecurityContext)
                                    {
                                        case AccessLevel.ViewUpdate:
                                            retVal = true;

                                            if (table.GetChanges(DataRowState.Deleted) != null)
                                            {
                                                foreach (DataRow row in table.GetChanges(DataRowState.Deleted).Rows)
                                                {
                                                    row.RejectChanges();
                                                }
                                            }
                                            break;

                                        case AccessLevel.ViewUpdateDelete:
                                            retVal = true;
                                            break;

                                        case AccessLevel.NoAccess:
                                        case AccessLevel.ViewOnly:
                                        default:
                                            table.RejectChanges();
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!retVal) { SaveSuccessful = true; }

            return retVal;
        }

        public virtual void New()
        {
            //check for nulls
            if (CurrentBusObj == null)
            {
                // exit for now, should an error be raised ?
                return;
            }
            this.Load(this.CurrentBusObj, true);
        }

        #region Properties

        /// <summary>
        /// Method to create a binding object that can be used to pupulate the list selector
        /// </summary>
        /// <param name="dt">Datatable to bind to</param> 
        /// <param name="StringLookup">True - Code field is a string value / False - Code field is a numeric value</param> 
        /// <param name="DisplayFieldName">The name of the field from the lookup table that holds the display value</param>
        /// <param name="ValueFieldName">The name of the field from the lookup datatable that holds to id value</param>
        /// <param name="WindowTitle">The title to display on the list selector control popup</param>
        /// <returns></returns>
        public BindingObject EstablishListObjectBinding(DataTable dt, bool StringLookup, string DisplayFieldName, string ValueFieldName, string WindowTitle)
        {
            BindingObject bindingObject = new BindingObject();
            if (dt != null)
            {
                // You must set the SourceData. Here we are creating sample a sample DataTable
                bindingObject.SourceData = dt;

                // Set the DisplayField and the ValueField (this can be the same field)
                bindingObject.DisplayField = DisplayFieldName;
                bindingObject.ValueField = ValueFieldName;

                // Set the title for the window
                bindingObject.Title = WindowTitle;

                // This bindingObject uses a string key
                bindingObject.LookupAsString = StringLookup;

                // The CallBackMethod does not need to be set here 
                // because we are using the ucLookupTextBox.
                // Look in the ucLookupTextBox.LookupValueSelected method
                // to see an example of implementing your own CallBackMethod.
                // this.CategoryTextBox.CallBackMethod = [CallBackMethodName];
            }
            return bindingObject;
        }

        /// <summary>
        /// This property [enter description here].
        /// </summary>
        public cBaseBusObject CurrentBusObj
        {
            get { return mCurrentBusObj; }
            set { mCurrentBusObj = value; }
        }

        /// <summary>
        /// This property gets or sets the value for 'DoNotSetDataContext'.
        /// This is generally true for Folders that do not contain any edited 
        /// data. If a Folder contains any tabs, the tabs will still have the 
        /// DataContext set, unless the tab has DoNotSetDataContext
        /// set to true.
        /// </summary>
        public bool DoNotSetDataContext
        {
            get { return doNotSetDataContext; }
            set { doNotSetDataContext = value; }
        }

        /// <summary>
        /// This property [enter description here].
        /// </summary>
        public List<ucBaseGrid> GridCollection
        {
            get { return mGridCollection; }
            set { mGridCollection = value; }
        }

        /// <summary>
        /// This property returns true if this object has a 'CurrentBusObj'.
        /// </summary>
        public bool HasCurrentBusObj
        {
            get
            {
                // initial value
                bool hasCurrentBusObj = (this.CurrentBusObj != null);

                // return value
                return hasCurrentBusObj;
            }
        }

        /// <summary>
        /// This property returns true if this object has a 'GridCollection'.
        /// </summary>
        public bool HasGridCollection
        {
            get
            {
                // initial value
                bool hasGridCollection = (this.GridCollection != null);

                // return value
                return hasGridCollection;
            }
        }

        /// <summary>
        /// This property returns true if the 'MainTableName' exists.
        /// </summary>
        public bool HasMainTableName
        {
            get
            {
                // initial value
                bool hasMainTableName = (!String.IsNullOrEmpty(this.MainTableName));

                // return value
                return hasMainTableName;
            }
        }

        /// <summary>
        /// This property returns true if this object has one or more grids
        /// </summary>
        public bool HasOneOrMoreGrids
        {
            get
            {
                // initial value
                bool hasOneOrMoreGrids = false;

                // if this object has a GridCollection
                if (this.HasGridCollection)
                {
                    // set the return value
                    hasOneOrMoreGrids = (this.GridCollection.Count > 0);
                }

                // return value
                return hasOneOrMoreGrids;
            }
        }

        /// <summary>
        /// This property returns true if this object has one or more tabs
        /// </summary>
        public bool HasOneOrMoreTabs
        {
            get
            {
                // initial value
                bool hasOneOrMoreTabs = false;

                // if this object has a TabCollection
                if (this.HasTabCollection)
                {
                    // set the return value
                    hasOneOrMoreTabs = (this.TabCollection.Count > 0);
                }

                // return value
                return hasOneOrMoreTabs;
            }
        }

        /// <summary>
        /// This property returns true if this object has a 'TabCollection'.
        /// </summary>
        public bool HasTabCollection
        {
            get
            {
                // initial value
                bool hasTabCollection = (this.TabCollection != null);

                // return value
                return hasTabCollection;
            }
        }

        /// <summary>
        /// This property [enter description here].
        /// </summary>
        public bool LookupWindow
        {
            get { return mIsLookupWindow; }
            set { mIsLookupWindow = value; }
        }

        /// <summary>
        /// This property [enter description here].
        /// </summary>
        public string MainTableName
        {
            get { return mMainTableName; }
            set { mMainTableName = value; }
        }

        /// <summary>
        /// This property gets or sets the value for 'ScreenBaseType'.
        /// </summary>
        public ScreenBaseTypeEnum ScreenBaseType
        {
            get { return screenBaseType; }
            set { screenBaseType = value; }
        }

        /// <summary>
        /// This property [enter description here].
        /// </summary>
        internal string ScreenName
        {
            get { return mScreenName; }
            set { mScreenName = value; }
        }

        /// <summary>
        /// This property gets or sets the value for TabCollection.
        /// </summary>
        public List<ScreenBase> TabCollection
        {
            get { return mTabCollections; }
            set { mTabCollections = value; }
        }

        /// <summary>
        /// This property contains security information for Screenbase object. Set during load method or during focus change on tabs, etc
        /// </summary>
        public AccessLevel SecurityContext
        {
            get { return mSecurityContext; }
            set { mSecurityContext = value; }
        }

        /// <summary>
        /// This property contains security information for Screenbase object. Set during load method or during focus change on tabs, etc
        /// </summary>
        public string ScreenBaseParentObjName
        {
            get { return mScreenBaseParentObjName; }
            set { mScreenBaseParentObjName = value; }
        }

        /// <summary>
        /// The header name to use on the opened window
        /// Setting this value will change the window header name of the window tab
        /// </summary>
        private string headerName = "";
        public string HeaderName
        {
            get { return headerName; }
            set
            {
                headerName = value;
                ContentPane p = (ContentPane)this.Parent;
                if (p == null)
                {
                    return;
                }
                p.Header = headerName;
            }
        }

        #endregion

        public virtual void Close()
        {
            SaveSuccessful = true;
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (this.CurrentBusObj != null && (IsScreenDirty || ForceScreenDirty)
                && (this.SecurityContext == AccessLevel.ViewUpdate || this.SecurityContext == AccessLevel.ViewUpdateDelete))
            {
                var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                {
                    this.Save();
                    if (SaveSuccessful == false)
                        return;
                    //@@Need to add code here to stop the window from closing if save fails
                    StopCloseIfCancelCloseOnSaveConfirmationTrue = true;
                }
            }
            //Set the business object to null so that we do not receive any false trues on future app close checks
            if(this.CurrentBusObj!=null && this.CurrentBusObj.ObjectData!=null)
                this.CurrentBusObj.ObjectData = null;
        }
    }

    public enum ScreenState
    {
        Empty,
        Normal,
        Inserting,
        Editable,
        Locked,
        Deleting
    }

    public class BoundTable
    {
        public string ScreenBaseName { get; set; }
        public string TableName { get; set; }
    }

}
