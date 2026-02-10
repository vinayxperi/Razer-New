using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Infragistics.Windows.DockManager;
using RazerWS;
using Infragistics.Windows.Ribbon;
using Infragistics.Windows.Controls ;

namespace RazerBase
{
    public class cCommandLibrary
    {
        /*Library for custom and standard commands 
        To add a custom command: ******************************************
        1) Setup a new private shared RoutedUICommand Variable
               Private Shared mTestCommand1 As RoutedUICommand
    
        2) Establish a new shared readonly property
               Public Shared ReadOnly Property TestCommand1 As RoutedUICommand
                   Get
                       Return mTestCommand1
                   End Get
               End Property
    
        3) Instantiate the routed command in the New Sub using the appropriate key combinations
                       Dim inputs As New InputGestureCollection  '--***** The first two lines are only used if a key combination is needed
                       inputs.Add(New KeyGesture(Key.A, ModifierKeys.Control, "Ctrl-A"))
                       mTestCommand1 = New RoutedUICommand("TestCommand 1", "TestCommand1", GetType(cCommandLibrary), inputs)
    
        4) Create two events one for executed and on for can execute - always make the can execute event as simple as possible as it will run all of the time automatically
               Public Shared Sub TestCommand1_Executed(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
                   MsgBox("Hello " + sender.ToString)
               End Sub
               Public Shared Sub TestCommand1_CanExecute(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
                   e.CanExecute = True
               End Sub
    
        5) Add the command binding to the initialize command event
               Dim Binding As New CommandBinding(cCommandLibrary.mTestCommand1)
               AddHandler Binding.Executed, AddressOf cCommandLibrary.TestCommand1_Executed
               AddHandler Binding.CanExecute, AddressOf cCommandLibrary.TestCommand1_CanExecute
               Win.CommandBindings.Add(Binding)
         End of steps for adding a custom command ****************************************************************
        Steps to add a prebuilt command
        1) Create two events one for executed and on for can execute - always make the can execute event as simple as possible as it will run all of the time automatically
              Public Shared Sub NewCommand_Executed(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
                   MsgBox("Hello NEW " + sender.ToString)
               End Sub

               Public Shared Sub NewCommand_CanExecute(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
                   e.CanExecute = True
               End Sub
    
        2) Add binding code to the initialization Event
               Binding = New CommandBinding(ApplicationCommands.[New])
               AddHandler Binding.Executed, AddressOf NewCommand_Executed
               AddHandler Binding.CanExecute, AddressOf NewCommand_CanExecute
               Win.CommandBindings.Add(Binding)
        ****************************End of steps to add prebuilt command*/

        private static readonly string DocumentHost = "DocHost";
        public IRazerService LCService { get; set; }

        #region Routed Commands
        //private static RoutedUICommand mTestCommand1 = new RoutedUICommand("_TC1", "TC1", typeof(cCommandLibrary));
        //public static RoutedUICommand TestCommand1
        //{
        //    get
        //    {
        //        return mTestCommand1;
        //    }̜
        //}́


        //public static RoutedUICommand mNewWindow = new RoutedUICommand("Ne_w Window", "NewWindow", typeof(cCommandLibrary));
        //public static RoutedUICommand NewWindow
        //{
        //    get
        //    {
        //        return mNewWindow;
        //    }
        //}

        private static RoutedUICommand mSaveGridSettings = new RoutedUICommand("_SGS", "SGS", typeof(cCommandLibrary));
        public static RoutedUICommand SaveGridSettings
        {
            get
            {
                return mSaveGridSettings;
            }
        }

        private static RoutedUICommand mResetGridSettings = new RoutedUICommand("_RGS", "RGS", typeof(cCommandLibrary));
        public static RoutedUICommand ResetGridSettings
        {
            get
            {
                return mResetGridSettings;
            }
        }

        public static RoutedUICommand mSave = new RoutedUICommand("SaveCmd", "Save", typeof(cCommandLibrary));
        public static RoutedUICommand Save
        {
            get
            {
                return mSave;
            }
        }

        private static RoutedUICommand mAddGridItem = new RoutedUICommand("_Add", "Add", typeof(cCommandLibrary));
        public static RoutedUICommand AddGridItem
        {
            get
            {
                return mAddGridItem;
            }
        }

        private static RoutedUICommand mRemoveGridItem = new RoutedUICommand("_Remove", "Remove", typeof(cCommandLibrary));
        public static RoutedUICommand RemoveGridItem
        {
            get
            {
                return mRemoveGridItem;
            }
        }

        private static RoutedUICommand mGenericGridItem1 = new RoutedUICommand("_Generic1", "Generic1", typeof(cCommandLibrary));
        public static RoutedUICommand GenericGridItem1
        {
            get
            {
                return mGenericGridItem1;
            }
        }

        private static RoutedUICommand mGenericGridItem2 = new RoutedUICommand("_Generic2", "Generic2", typeof(cCommandLibrary));
        public static RoutedUICommand GenericGridItem2
        {
            get
            {
                return mGenericGridItem2;
            }
        }

        private static RoutedUICommand mGenericGridItem3 = new RoutedUICommand("_Generic3", "Generic3", typeof(cCommandLibrary));
        public static RoutedUICommand GenericGridItem3
        {
            get
            {
                return mGenericGridItem3;
            }
        }

        private static RoutedUICommand mGenericGridItem4 = new RoutedUICommand("_Generic4", "Generic4", typeof(cCommandLibrary));
        public static RoutedUICommand GenericGridItem4
        {
            get
            {
                return mGenericGridItem4;
            }
        }

        private static RoutedUICommand mToggleGridFilter = new RoutedUICommand("_ToggleGridFilter", "ToggleGridFilter", typeof(cCommandLibrary));
        public static RoutedUICommand ToggleGridFilter
        {
            get
            {
                return mToggleGridFilter;
            }
        }

        public static RoutedUICommand mDelete = new RoutedUICommand("DeleteCmd", "Delete", typeof(cCommandLibrary));
        public static RoutedUICommand Delete
        {
            get
            {
                return mDelete;
            }
        }

        public static RoutedUICommand mInsert = new RoutedUICommand("InsertCmd", "I_nsert", typeof(cCommandLibrary));
        public static RoutedUICommand Insert
        {
            get
            {
                return mInsert;
            }
        }

        public static RoutedUICommand mRefresh = new RoutedUICommand("RefreshCmd", "Refresh", typeof(cCommandLibrary));
        public static RoutedUICommand Refresh
        {
            get
            {
                return mRefresh;
            }
        }

        //public static RoutedUICommand mPrint = new RoutedUICommand("PrintScreenCmd", "PrintScreen", typeof(cCommandLibrary));
        //public static RoutedUICommand PrintScreen
        //{
        //    get
        //    {
        //        return mPrint;
        //    }
        //}



        //Temporary commands for initial testing
        //public static RoutedUICommand OpenReceivableAccountWindow = new RoutedUICommand("OpenReceivableAccount", "OpenReceivableAccount", typeof(cCommandLibrary),
        //    new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.A, ModifierKeys.Control, "Open Receivable Account") }));

        //public static RoutedUICommand OpenCashWindow = new RoutedUICommand("OpenCash", "OpenCash", typeof(cCommandLibrary),
        //    new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.Q, ModifierKeys.Control, "Open Cash") }));

        //public static RoutedUICommand OpenCashBatchEntryWindow = new RoutedUICommand("OpenCashBatchEntry", "OpenCashBatchEntry", typeof(cCommandLibrary),
        //    new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Control, "Open Cash Batch Entry") }));

        //public static RoutedUICommand OpenCustomerWindow = new RoutedUICommand("OpenCustomerFolder", "OpenCustomerFolder", typeof(cCommandLibrary),
        //    new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.K, ModifierKeys.Control, "Open Customer Folder") }));

        //public static RoutedUICommand OpenNationalAdsSearch = new RoutedUICommand("OpenNationalAdsSearch", "OpenNationalAdsSearch", typeof(cCommandLibrary),
        //    new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.N, ModifierKeys.Control, "Open National Ads Search") }));

        public static RoutedUICommand ExitApplication = new RoutedUICommand("ExitApplication", "ExitApplication", typeof(cCommandLibrary),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.X, ModifierKeys.Alt, "Exit Application") }));
        //public static RoutedUICommand PrintScreen = new RoutedUICommand("PrintPreview", "PrintPreview", typeof(cCommandLibrary),
        //   new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.P, ModifierKeys.Alt, "Print Screen") })); 
        #endregion
        
        public cCommandLibrary()
        {
            try
            {
                //Add key gestures for custom commands in this event
                InputGestureCollection inputs = new InputGestureCollection();

                //New Window
                //inputs.Add(new KeyGesture(Key.A, ModifierKeys.Control, "Ctrl-A"));
                //mNewWindow = new RoutedUICommand("Ne_w Window", "NewWindow", typeof(cCommandLibrary));

                //Save Grid Settings
                inputs = new InputGestureCollection();
                //--***** The first two lines are only used if a key combination is needed
                inputs.Add(new KeyGesture(Key.G, ModifierKeys.Control, "Ctrl-G"));
                mSaveGridSettings = new RoutedUICommand("Save Grid Settings", "SaveGridSettings", typeof(cCommandLibrary), inputs);

                //Reset Grid Settings
                inputs = new InputGestureCollection();
                inputs.Add(new KeyGesture(Key.E, ModifierKeys.Control, "Ctrl-E"));
                mResetGridSettings = new RoutedUICommand("Reset Grid Settings", "ResetGridSettings", typeof(cCommandLibrary), inputs);

                //Insert Grid Item
                inputs = new InputGestureCollection();
                inputs.Add(new KeyGesture(Key.A, ModifierKeys.Control, "Ctrl-A"));
                mAddGridItem = new RoutedUICommand("Add Record", "AddRecord", typeof(cCommandLibrary), inputs);

                //Delete Grid Item
                inputs = new InputGestureCollection();
                inputs.Add(new KeyGesture(Key.R, ModifierKeys.Control, "Ctrl-R"));
                mRemoveGridItem = new RoutedUICommand("Remove Record", "RemoveRecord", typeof(cCommandLibrary), inputs);

                //Toggle Grid Filter
                inputs = new InputGestureCollection();
                inputs.Add(new KeyGesture(Key.T, ModifierKeys.Control, "Ctrl-T"));
                mToggleGridFilter = new RoutedUICommand("Toggle Filter", "ToggleFilter", typeof(cCommandLibrary), inputs);

                //Delete
                inputs = new InputGestureCollection();
                inputs.Add(new KeyGesture(Key.D, ModifierKeys.Control, "Ctrl-D"));
                mDelete = new RoutedUICommand("Delete", "Delete", typeof(cCommandLibrary), inputs);

                //Insert
                inputs = new InputGestureCollection();
                inputs.Add(new KeyGesture(Key.I, ModifierKeys.Control, "Ctrl-I"));
                mInsert = new RoutedUICommand("Insert Command", "InsertCommand", typeof(cCommandLibrary), inputs);

                //Print Screen
                //inputs = new InputGestureCollection();
                //inputs.Add(new KeyGesture(Key.P, ModifierKeys.Control, "Ctrl-P"));
                //mPrint = new RoutedUICommand("Print Screen", "PrintScreen", typeof(cCommandLibrary), inputs);

            }
            catch (Exception ex) { throw ex; }
        }

        #region Command Initialization
        ///<summary>
        ///Initializes command bindings and attaches to the window
        ///</summary>
        ///<param name="Win">The window to attach command bindings to</param>
        ///<remarks></remarks>
        public static void InitializeCommands(cMainWindowBase Win)
        {
            var globals = new cGlobals(Win.LCService, Win.BillService);

            //Get principal username
            cGlobals.UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];

            //Get Database & Server Name
            cGlobals.DatabaseName = cGlobals.LCService.GetDatabaseServerName();

            //Set security DataTable in cGlobals
            var parms = new cDsParms();
            parms.add(cGlobals.UserName, "@user_id");
            cGlobals.SecurityDT = cGlobals.LCService.GetObject("SecurityObject", parms.GetDS(), new System.Data.DataSet()).Tables["SecurityTable"];

            cGlobals.Environment = System.Configuration.ConfigurationManager.AppSettings["env"].ToString();

            try
            {
                cGlobals.DisableSecurity = System.Configuration.ConfigurationManager.AppSettings["DisableSecurity"].ToString();
            }
            catch
            {
                cGlobals.DisableSecurity = "unknown";
            }

            SetMenuSecurity(Win);

            SetCommandInitialization(Win);

        }

        private static void SetMenuSecurity(cMainWindowBase Win)
        {
            IEnumerable<XamRibbon> Ribbons = UIHelper.FindChildren<XamRibbon>(Win);

            if (Ribbons != null)
            {
                foreach (XamRibbon ribbon in Ribbons)
                {
                    IEnumerable<ButtonTool> Buttons = UIHelper.FindChildren<ButtonTool>(ribbon);

                    if (Buttons != null)
                    {
                        foreach (ButtonTool button in Buttons)
                        {
                            if (button.Parent.GetType().FullName == "Infragistics.Windows.Ribbon.ToolHorizontalWrapPanel")
                            {
                                ControlSecurity controlSecurity = ApplicationSecurityManager.GetMenuVisibility(button.Id);
                                button.Visibility = controlSecurity.ControlVisibility;
                            }
                        }
                    }
                }
            }
        }

        public static void InitializeCommands(Window Win)
        {
            SetCommandInitialization(Win);
        }

        private static void SetCommandInitialization(Window Win)
        {
            try
            {
                CommandBinding Binding = new CommandBinding();

                //Command NEW
                Binding = new CommandBinding(ApplicationCommands.New, NewCommand_Executed, NewCommand_CanExecute);
                Win.CommandBindings.Add(Binding);

                //Command Save
                Binding = new CommandBinding(ApplicationCommands.Save, SaveCommand_Executed, SaveCommand_CanExecute);
                Win.CommandBindings.Add(Binding);

                //Command Open
                Binding = new CommandBinding(ApplicationCommands.Open, OpenCommand_Executed, OpenCommand_CanExecute);
                Win.CommandBindings.Add(Binding);

                //Command NewWindow
                //Binding = new CommandBinding(cCommandLibrary.NewWindow, NewWindowCommand_Executed, NewWindowCommand_CanExecute);
                //Win.CommandBindings.Add(Binding);

                //Command Delete
                Binding = new CommandBinding(ApplicationCommands.Delete, DeleteCommand_Executed, DeleteCommand_CanExecute);
                Win.CommandBindings.Add(Binding);

                //Command Print Report
                Binding = new CommandBinding(ApplicationCommands.Print, PrintReportCommand_Executed, PrintReportCommand_CanExecute);
                Win.CommandBindings.Add(Binding);

                //Command Print Screen
                Binding = new CommandBinding(ApplicationCommands.PrintPreview, PrintScreenCommand_Executed, PrintScreenCommand_CanExecute);
                Win.CommandBindings.Add(Binding);

                //Command Refresh
                Binding = new CommandBinding(NavigationCommands.Refresh, RefreshCommand_Executed, RefreshCommand_CanExecute);
                Win.CommandBindings.Add(Binding);

                ////Temporary commands for initial testing
                //Binding = new CommandBinding(cCommandLibrary.OpenReceivableAccountWindow, OpenReceivableAccountCommand_Executed);
                //Win.CommandBindings.Add(Binding);

                //Binding = new CommandBinding(cCommandLibrary.OpenCashWindow, OpenCashCommand_Executed);
                //Win.CommandBindings.Add(Binding);

                //Binding = new CommandBinding(cCommandLibrary.OpenCashBatchEntryWindow, OpenCashBatchEntryCommand_Executed);
                //Win.CommandBindings.Add(Binding);

                //Binding = new CommandBinding(cCommandLibrary.OpenCashBatchEntryWindow, OpenCashBatchEntryCommand_Executed);
                //Win.CommandBindings.Add(Binding);

                //Binding = new CommandBinding(cCommandLibrary.OpenCustomerWindow, OpenCustomerWindowCommand_Executed);
                //Win.CommandBindings.Add(Binding);

                Binding = new CommandBinding(cCommandLibrary.ExitApplication, ExitApplication_Executed);
                Win.CommandBindings.Add(Binding);

                Binding = new CommandBinding(ApplicationCommands.Close, CloseApplication_Executed, CloseCommand_CanExecute);
                Win.CommandBindings.Add(Binding);

            }
            catch (Exception ex) { throw ex; }
        }

        public static void InitializePopUpCommands(UserControl Source)
        {
            try
            {
                //This event takes the place of the Initialize command event if the commands are for an object popup menu
                CommandBinding Binding = default(CommandBinding);

                ////Determine the type of object sending the initialize to determine the commands being used
                //if (Source.GetType() == typeof(ucBaseGrid))
                //{
                //ucBaseGrid objSource;
                //objSource = (ucBaseGrid)Source;

                //Command SaveGridSettings
                Binding = new CommandBinding(cCommandLibrary.SaveGridSettings, SaveGridSettings_Executed, SaveGridSettings_CanExecute);
                Source.CommandBindings.Add(Binding);

                //Command ResetGridSettings
                Binding = new CommandBinding(cCommandLibrary.ResetGridSettings, ResetGridSettings_Executed, SaveGridSettings_CanExecute);  //Uses the same information as SaveGridSettings command so the function is shared
                Source.CommandBindings.Add(Binding);

                //Command InsertGridItem
                Binding = new CommandBinding(cCommandLibrary.AddGridItem, AddGridItem_Executed, AddGridItem_CanExecute);
                Source.CommandBindings.Add(Binding);

                //Command DeleteGridItem
                Binding = new CommandBinding(cCommandLibrary.RemoveGridItem, RemoveGridItem_Executed, RemoveGridItem_CanExecute);
                Source.CommandBindings.Add(Binding);

                //Command GenericGridItem1
                Binding = new CommandBinding(cCommandLibrary.GenericGridItem1, GenericGridItem1_Executed, GenericGridItem1_CanExecute);
                Source.CommandBindings.Add(Binding);

                //Command GenericGridItem2
                Binding = new CommandBinding(cCommandLibrary.GenericGridItem2, GenericGridItem2_Executed, GenericGridItem2_CanExecute);
                Source.CommandBindings.Add(Binding);

                //Command GenericGridItem3
                Binding = new CommandBinding(cCommandLibrary.GenericGridItem3, GenericGridItem3_Executed, GenericGridItem3_CanExecute);
                Source.CommandBindings.Add(Binding);

                //Command GenericGridItem4
                Binding = new CommandBinding(cCommandLibrary.GenericGridItem4, GenericGridItem4_Executed, GenericGridItem4_CanExecute);
                Source.CommandBindings.Add(Binding);


                Binding = new CommandBinding(cCommandLibrary.ToggleGridFilter, ToggleGridFilter_Executed, ToggleGridFilter_CanExecute);
                Source.CommandBindings.Add(Binding);

                //Binding = new CommandBinding(cCommandLibrary.OpenNationalAdsSearch, OpenNationalAdsSearchCommand_Executed, AlwaysOn_CanExecute);
                //Source.CommandBindings.Add(Binding);


                //}
            }
            catch (Exception ex) { throw ex; }
        }
        #endregion

        private static ScreenBase GetScreenInstance(object window)
        {
            try
            {
                cMainWindowBase mainWindow = window as cMainWindowBase;

                if (mainWindow != null)
                {
                    DocumentContentHost DocHost = UIHelper.FindChild<DocumentContentHost>(mainWindow, DocumentHost);
                    ScreenBase currentScreen = DocHost.ActiveDocument.Content as ScreenBase;
                    return currentScreen;
                }
                else
                    return null;
            }
            catch (Exception)
            {                
                return null;
            }
        }

        #region Commands Executed
        public static void CloseApplication_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                // cast the sender as a MainWindow object
                cMainWindowBase mainWindow = sender as cMainWindowBase;

                // if the mainWindow was found
                if (mainWindow != null)
                {
                    // if the currentScreen exists
                    DocumentContentHost DocHost = UIHelper.FindChild<DocumentContentHost>(mainWindow, DocumentHost);
                    ScreenBase currentScreen = DocHost.ActiveDocument.Content as ScreenBase;

                    DocHost.ActiveDocument.ExecuteCommand(ContentPaneCommands.Close);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static void ExitApplication_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Controls screen insert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void NewCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            //TODO:  For Brian Dyer
            //Get current type of window and create instance of screen with blank data
            //may need some varialbe in screen base to denote new
            //if new override save to create primary key etc...

            ScreenBase currentScreen = GetScreenInstance(sender);
            if (currentScreen != null)
            {
                currentScreen.New();
            }
        }

        public static void SaveCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            ScreenBase currentScreen = GetScreenInstance(sender);
            if (currentScreen != null)
            {
                // save the current screen
                currentScreen.Save();
            }
        }

        public static void RefreshCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            ScreenBase currentScreen = GetScreenInstance(sender);
            if (currentScreen != null)
            {
                // save the current screen
                currentScreen.Refresh();
            }
        }

        public static void DeleteCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            ScreenBase currentScreen = GetScreenInstance(sender);
            if (currentScreen != null)
            {
                //do delete actions
                currentScreen.Delete();
            }
        }

        public static void PrintReportCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            ScreenBase currentScreen = GetScreenInstance(sender);

            //Make sure it is a screen base and has print reports
            if (currentScreen == null || !currentScreen.HasPrintReport)
            {
                return;
            }
            else
            {

                //Find any tab controls if a folder
                if (currentScreen.ScreenBaseType == ScreenBaseTypeEnum.Folder)
                {
                    foreach (XamTabControl xt in currentScreen.FindChildren<XamTabControl>())
                    {
                        foreach (TabItem ti in xt.Items)
                        {
                            //Find the currently Active Tab
                            if (ti.IsSelected)
                            {

                                //Looks for all types of screen base
                                foreach (ScreenBase sb in ti.FindChildren<ScreenBase>())
                                {
                                    if (sb.HasPrintReport)
                                    {
                                        sb.PrintReport();
                                    }
                                }
                            }
                        }
                    }
                }
                else //Not a folder
                    currentScreen.PrintReport();
            }
            
        }

        public static void PrintScreenCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            ScreenBase currentScreen = GetScreenInstance(sender);
            if (currentScreen != null)
            {
                //Run Print Report
                //Visual v = this;
                currentScreen.PrintScreen();
            }
        }

        public static void OpenCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                //TODO: Commented out in orig code
                //'Dim w As wMain = CType(sender, wMain)
                //'Dim s As New Invoice

                //'Dim Pane As ContentPane = w.XamDockManager1.AddDocument("Invoice", s)
                //'Pane.AllowDocking = False
                //''contentPane.AllowFloatingOnly = False
            }
            catch (Exception ex) { throw ex; }

        }

        //public static void OpenCustomerWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    cMainWindowBase win = (cMainWindowBase)sender;
        //    ScreenBase screen = null;
        //    ContentPane contentPane = null;
        //    XamDockManager XamDockManager1 = null;
        //    cBaseBusObject RecvAcct = new cBaseBusObject("RecvAcct");

        //    //Get type of ucRecvAcctScreen using reflection
        //    Type ucCustomerScreenType = Type.GetType("Customer.CustomerMainScreen, RecvAcctFolder");

        //    //Create an instance of ucRecvAcctScreen using reflection
        //    screen = Activator.CreateInstance(ucCustomerScreenType, new object[] { RecvAcct }) as ScreenBase;

        //    XamDockManager1 = UIHelper.FindChild<XamDockManager>(win, "XamDockManager1");
        //    contentPane = XamDockManager1.AddDocument("Receivable Account", screen);
        //    contentPane.Activate();
        //    contentPane.AllowDocking = false;
        //    contentPane.AllowFloatingOnly = false;
        //}

        //public static void OpenCashCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    cMainWindowBase win = (cMainWindowBase)sender;
        //    ScreenBase screen = null;
        //    ContentPane contentPane = null;
        //    XamDockManager XamDockManager1 = null;
        //    cBaseBusObject cashScreen = new cBaseBusObject("CashScreen");

        //    //Get type of ucRecvAcctScreen using reflection
        //    Type cashScreenType = Type.GetType("Cash.CashScreen, Cash");

        //    // the Type must be found for this to work
        //    if (cashScreenType != null)
        //    {
        //        //Create an instance of ucRecvAcctScreen using reflection
        //        screen = Activator.CreateInstance(cashScreenType, new object[] { cashScreen }) as ScreenBase;

        //        XamDockManager1 = UIHelper.FindChild<XamDockManager>(win, "XamDockManager1");
        //        contentPane = XamDockManager1.AddDocument("Cash", screen);
        //        contentPane.Activate();
        //        contentPane.AllowDocking = false;
        //        contentPane.AllowFloatingOnly = false;
        //    }
        //}

        //public static void OpenReceivableAccountCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    cMainWindowBase win = (cMainWindowBase)sender;
        //    ScreenBase screen = null;
        //    ContentPane contentPane = null;
        //    XamDockManager XamDockManager1 = null;
        //    cBaseBusObject RecvAcct = new cBaseBusObject("RecvAcct");

        //    //Get type of ucRecvAcctScreen using reflection
        //    Type ucRecvAcctScreenType = Type.GetType("RecvAcctFolder.ucRecvAcctScreen, RecvAcctFolder");

        //    //Create an instance of ucRecvAcctScreen using reflection
        //    screen = Activator.CreateInstance(ucRecvAcctScreenType, new object[] { RecvAcct }) as ScreenBase;

        //    XamDockManager1 = UIHelper.FindChild<XamDockManager>(win, "XamDockManager1");
        //    contentPane = XamDockManager1.AddDocument("Receivable Account", screen);
        //    contentPane.Activate();
        //    contentPane.AllowDocking = false;
        //    contentPane.AllowFloatingOnly = false;
        //}

        //public static void OpenCashBatchEntryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    cMainWindowBase win = (cMainWindowBase)sender;
        //    ScreenBase screen = null;
        //    ContentPane contentPane = null;
        //    XamDockManager XamDockManager1 = null;
        //    cBaseBusObject cashScreen = new cBaseBusObject("CashBatchEntry");

        //    //Get type of ucRecvAcctScreen using reflection
        //    Type cashScreenType = Type.GetType("Cash.CashBatchEntryScreen, Cash");

        //    // the Type must be found for this to work
        //    if (cashScreenType != null)
        //    {
        //        //Create an instance of ucRecvAcctScreen using reflection
        //        screen = Activator.CreateInstance(cashScreenType, new object[] { cashScreen }) as ScreenBase;

        //        XamDockManager1 = UIHelper.FindChild<XamDockManager>(win, "XamDockManager1");
        //        contentPane = XamDockManager1.AddDocument("Cash", screen);
        //        contentPane.Activate();
        //        contentPane.AllowDocking = false;
        //        contentPane.AllowFloatingOnly = false;
        //    }
        //}

        public static void NewWindowCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            //try
            //{
            //    //object win = (object)sender;
            //    cMainWindowBase win = (cMainWindowBase)sender;
            //    ScreenBase screen = null;
            //    ContentPane contentPane = null;
            //    XamDockManager XamDockManager1 = null;


            //    if (e.Parameter != null)
            //    {
            //        switch (e.Parameter.ToString())
            //        {
            //            case "bReceivableAccount":

            //                //folder level security check
            //                //OL 1
            //                //if (!RazerBase.ApplicationSecurityManager.FolderAccess("3000", "FOLDER"))
            //                //OL 2
            //                //if (!RazerBase.ApplicationSecurityManager.FolderAccess("RecvAcct", "FOLDER"))
            //                //OL 3
            //                //if (!RazerBase.ApplicationSecurityManager.FolderAccess("RecvAcct", "FOLDER"))
            //                //{
            //                //    MessageBox.Show("Access Denied", "Razer Security", MessageBoxButton.OK, MessageBoxImage.Warning);
            //                //    return;
            //                //}
            //                //setup base bus obj 
            //                cBaseBusObject RecvAcct = new cBaseBusObject("RecvAcct");

            //                //Get type of ucRecvAcctScreen using reflection
            //                Type ucRecvAcctScreenType = Type.GetType("RecvAcctFolder.ucRecvAcctScreen, RecvAcctFolder");

            //                //Create an instance of ucRecvAcctScreen using reflection
            //                screen = Activator.CreateInstance(ucRecvAcctScreenType, new object[] { RecvAcct }) as ScreenBase;

            //                XamDockManager1 = UIHelper.FindChild<XamDockManager>(win, "XamDockManager1");
            //                contentPane = XamDockManager1.AddDocument("Receivable Account", screen);
            //                contentPane.Activate();
            //                contentPane.AllowDocking = false;
            //                contentPane.AllowFloatingOnly = false;

            //                break;

            //            case "bSystem":

            //                //s = new ucSystem();
            //                //ContentPane P = win.XamDockManager1.AddDocument("Object WS Test", s);
            //                //P.Activate();
            //                //P.AllowDocking = false;
            //                //P.AllowFloatingOnly = false;

            //                break;
            //            case "bSelectStyle":

            //                //ResourceDictionary d = new ResourceDictionary();

            //                //switch (CurrentSkin.ToLower)
            //                //{
            //                //    case "raptorred":
            //                //        d.Source = new Uri("Resources/RoviBlack.xaml", UriKind.Relative);
            //                //        CurrentSkin = "roviblack";

            //                //        break;
            //                //    case "roviblack":
            //                //        d.Source = new Uri("Resources/GroovyGreen.xaml", UriKind.Relative);
            //                //        CurrentSkin = "groovygreen";

            //                //        break;
            //                //    case "groovygreen":
            //                //        d.Source = new Uri("Resources/BirneyBlue.xaml", UriKind.Relative);
            //                //        CurrentSkin = "birneyblue";
            //                //        break;
            //                //    case "birneyblue":
            //                //        d.Source = new Uri("Resources/RaptorRed.xaml", UriKind.Relative);
            //                //        CurrentSkin = "raptorred";
            //                //        break;
            //                //}

            //                //Application.Current.Resources.MergedDictionaries.Add(d);
            //                //My.Settings.DefaultSkin = CurrentSkin;
            //                break;

            //            case "bTest":

            //                //setup base bus obj 
            //                cBaseBusObject folderMainScreen = new cBaseBusObject("FolderMainScreen");

            //                //Get type of ucRecvAcctScreen using reflection
            //                Type folderMainScreenType = Type.GetType("RazerTestProject.FolderMainScreen, RazerTestProject");

            //                // the Type must be found for this to work
            //                if (folderMainScreenType != null)
            //                {
            //                    //Create an instance of ucRecvAcctScreen using reflection
            //                    screen = Activator.CreateInstance(folderMainScreenType, new object[] { folderMainScreen }) as ScreenBase;

            //                    XamDockManager1 = UIHelper.FindChild<XamDockManager>(win, "XamDockManager1");
            //                    contentPane = XamDockManager1.AddDocument("Customer Info", screen);
            //                    contentPane.Activate();
            //                    contentPane.AllowDocking = false;
            //                    contentPane.AllowFloatingOnly = false;
            //                }

            //                // required
            //                break;

            //            case "btnCash":

            //                //setup base bus obj 
            //                cBaseBusObject cashScreen = new cBaseBusObject("CashScreen");

            //                //Get type of ucRecvAcctScreen using reflection
            //                Type cashScreenType = Type.GetType("Cash.CashScreen, Cash");

            //                // the Type must be found for this to work
            //                if (cashScreenType != null)
            //                {
            //                    //Create an instance of ucRecvAcctScreen using reflection
            //                    screen = Activator.CreateInstance(cashScreenType, new object[] { cashScreen }) as ScreenBase;

            //                    XamDockManager1 = UIHelper.FindChild<XamDockManager>(win, "XamDockManager1");
            //                    contentPane = XamDockManager1.AddDocument("Cash", screen);
            //                    contentPane.Activate();
            //                    contentPane.AllowDocking = false;
            //                    contentPane.AllowFloatingOnly = false;
            //                }

            //                // required
            //                break;

            //            default:

            //                // raise exception command not handled
            //                string message = "Command '" + e.Parameter.ToString() + "' was not handled in the CommandLibrary.";

            //                // raise the error
            //                throw new Exception(message);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        public static void AddGridItem_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ((ucBaseGrid)sender).AddGridItem();
            }
            catch (Exception ex) { throw ex; }
        }

        public static void RemoveGridItem_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ((ucBaseGrid)sender).RemoveGridItem();
            }
            catch (Exception ex) { throw ex; }
        }

        public static void SaveGridSettings_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ((ucBaseGrid)sender).SaveGridSettings();
            }
            catch (Exception ex) { throw ex; }
        }

        public static void ResetGridSettings_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ((ucBaseGrid)sender).ResetGridSettings();
            }
            catch (Exception ex) { throw ex; }
        }

        public static void GenericGridItem1_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ((ucBaseGrid)sender).GenericGridItem1();
            }
            catch (Exception ex) { throw ex; }
        }

        public static void GenericGridItem2_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ((ucBaseGrid)sender).GenericGridItem2();
            }
            catch (Exception ex) { throw ex; }
        }

        public static void GenericGridItem3_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ((ucBaseGrid)sender).GenericGridItem3();
            }
            catch (Exception ex) { throw ex; }
        }

        public static void GenericGridItem4_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ((ucBaseGrid)sender).GenericGridItem4();
            }
            catch (Exception ex) { throw ex; }
        }

        public static void ToggleGridFilter_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ((ucBaseGrid)sender).ToggleFilters();
            }
            catch (Exception ex) { throw ex; }
        }

  
        
        #endregion

        #region Commands Can Execute
        public static void NewCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            ScreenBase currentScreen = GetScreenInstance(sender);
            if (currentScreen != null && currentScreen.CanExecuteNewCommand && currentScreen.SecurityContext!= AccessLevel.NoAccess && currentScreen.SecurityContext!= AccessLevel.ViewOnly )
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public static void CloseCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            ScreenBase currentScreen = GetScreenInstance(sender);
            if (currentScreen != null && currentScreen.CanExecuteCloseCommand)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public static void SaveCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                ScreenBase currentScreen = GetScreenInstance(sender);
                if (currentScreen != null && (currentScreen.CanExecuteSaveCommand || (currentScreen.CurrentBusObj!=null && currentScreen.CurrentBusObj.HasObjectData))
                    && (currentScreen.SecurityContext == AccessLevel.ViewUpdate || currentScreen.SecurityContext == AccessLevel.ViewUpdateDelete))
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static void RefreshCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                ScreenBase currentScreen = GetScreenInstance(sender);
                if (currentScreen != null && currentScreen.CanExecuteRefreshCommand)
                {
                    if (currentScreen.CurrentBusObj!= null && currentScreen.CurrentBusObj.HasObjectData
                        && currentScreen.SecurityContext != AccessLevel.NoAccess )
                    {
                        e.CanExecute = true;
                    }
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception ex) { throw ex; }


        }


        public static void DeleteCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                ScreenBase currentScreen = GetScreenInstance(sender);
                if (currentScreen != null && currentScreen.CanExecuteDeleteCommand && currentScreen.SecurityContext == AccessLevel.ViewUpdateDelete)
                {
                    if (currentScreen.CurrentBusObj != null && currentScreen.CurrentBusObj.HasObjectData)
                    {
                        e.CanExecute = true;
                    }
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static void PrintReportCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            ScreenBase currentScreen = GetScreenInstance(sender);

            //Make sure it is a screen base and has print reports
            if (currentScreen == null || currentScreen.CurrentBusObj==null || !currentScreen.CurrentBusObj.HasObjectData || !currentScreen.HasPrintReport)
            {
                e.CanExecute = false;
            }
            else
            {
                //Find any tab controls if a folder
                if (currentScreen.ScreenBaseType == ScreenBaseTypeEnum.Folder)
                {
                    foreach (XamTabControl xt in currentScreen.FindChildren<XamTabControl>())
                    {
                        foreach (TabItem ti in xt.Items)
                        {
                            //Find the currently Active Tab
                            if (ti.IsSelected)
                            {

                                //Looks for all types of screen base
                                foreach (ScreenBase sb in ti.FindChildren<ScreenBase>())
                                {
                                    if (sb.HasPrintReport)
                                    {
                                        e.CanExecute = true;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    e.CanExecute = false;
                }
                else //Not a folder
                    e.CanExecute = true;
            }

        }

        public static void PrintScreenCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            //RES 3/19/15 enable print screen
            //e.CanExecute = false;
            e.CanExecute = true;
        }

        public static void OpenCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = true;
            }
            catch (Exception ex) { throw ex; }
        }

        public static void AlwaysOn_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = true;
            }
            catch (Exception ex) { throw ex; }
        }

        public static void NewWindowCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = true;
            }
            catch (Exception ex) { throw ex; }
        }

        public static void AddGridItem_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                //if context menu delegate is not handled disable
                ucBaseGrid baseGrid = sender as ucBaseGrid;
                if (baseGrid != null && baseGrid.ContextMenuAddDelegate == null)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;
            }
            catch (Exception ex) { throw ex; }
        }

        public static void RemoveGridItem_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                //if context menu delegate is not handled disable
                ucBaseGrid baseGrid = sender as ucBaseGrid;
                if (baseGrid != null && baseGrid.ContextMenuRemoveDelegate == null)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;               
            }
            catch (Exception ex) { throw ex; }
        }

        public static void SaveGridSettings_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                //Function is used for both the save grid settings and the reset grid settings can execute handler
                if (!string.IsNullOrEmpty(((ucBaseGrid)sender).ConfigFileName))
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }

            }
            catch (Exception ex) { throw ex; }

        }

        public static void GenericGridItem1_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                //if context menu delegate is not handled disable
                ucBaseGrid baseGrid = sender as ucBaseGrid;
                if (baseGrid != null && baseGrid.ContextMenuGenericDelegate1 == null)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;
            }
            catch (Exception ex) { throw ex; }
        }

        public static void GenericGridItem2_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                //if context menu delegate is not handled disable
                ucBaseGrid baseGrid = sender as ucBaseGrid;
                if (baseGrid.ContextMenuGenericDelegate2 == null)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;
            }
            catch (Exception ex) { throw ex; }
        }

        public static void GenericGridItem3_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                //if context menu delegate is not handled disable
                ucBaseGrid baseGrid = sender as ucBaseGrid;
                if (baseGrid != null && baseGrid.ContextMenuGenericDelegate3 == null)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;
            }
            catch (Exception ex) { throw ex; }
        }

        public static void GenericGridItem4_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                //if context menu delegate is not handled disable
                ucBaseGrid baseGrid = sender as ucBaseGrid;
                if (baseGrid != null && baseGrid.ContextMenuGenericDelegate4 == null)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;
            }
            catch (Exception ex) { throw ex; }
        }

        public static void ToggleGridFilter_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                //if context menu delegate is not handled disable
                ucBaseGrid baseGrid = sender as ucBaseGrid;
                if (baseGrid != null && baseGrid.ContextMenuToggleFilter == null)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            catch (Exception ex) { throw ex; }
        }
        #endregion

    }
}
