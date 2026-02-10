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
using System.Windows.Shapes;
using RazerBase;
using RazerWS;
using Razer.Common;
using System.Media;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using System.Deployment.Application;
using System.Windows.Controls.Primitives;


namespace Razer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly string sectionName = "unity";
        private static readonly string containerName = "mainContainer";

        public MainWindow()
        {
            try
            {
                this.Title = "Razer";
                InitializeComponent();
                this.LCService = new AppCode.RazerServiceDelegate();
                this.BillService = new AppCode.BillingServiceDelegate();

                string version = "Not Installed as ClickOnce.";
                //string documentID;
                string Password;
                string ExpiredPassword;
                //int DaysToExpire;
                //int seqID;


                if (ApplicationDeployment.IsNetworkDeployed) //determines if the app is installed as ClickOnce
                {
                    version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }

                cCommandLibrary.InitializeCommands(this);

                IUnityContainer container = new UnityContainer();
                UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(sectionName);
                section.Configure(container, containerName);
                Global.Container = container;

                //find status bar
                var StatusBar = (this.RazerMainWindow.Content as Infragistics.Windows.Ribbon.RibbonWindowContentHost).StatusBar;

                //find databaseName text block
                var TextBlocks = (from x in UIHelper.FindChildren<TextBlock>(StatusBar)
                                  where x.Name == "databaseName"
                                  select x);

                //set text of databaseName
                foreach (TextBlock textblock in TextBlocks)
                {
                    textblock.Text = cGlobals.DatabaseName;
                }
            
                //SoundPlayer sp = new SoundPlayer();
                //sp.SoundLocation = "Resources/Memo.wav";
                //sp.Play();

                this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);

                //DWR - 1/29/13 Added the below to handle color coding the ribbon and status bar based on the environment
                //**Hardcoded colors based on server and database names
                //This should be converted to a database table at some point.
                ResourceDictionary d = new ResourceDictionary();

                //RES 10/21/24 added MFA 
                if (cGlobals.SecurityDT.Rows[0]["role_id"].ToString() == "1")
                {                    
                    Password = cGlobals.SecurityDT.Rows[0]["Password"].ToString();
                    ExpiredPassword = cGlobals.SecurityDT.Rows[0]["ChangePassword"].ToString();
                    if (ExpiredPassword == "Y")
                    {
                        MessageBox.Show(string.Format("Expired Password, contact Admin to reset"));
                        //throw new Exception("Expired Password");
                        //this.Shutdown();
                        System.Environment.Exit(0);
                    }
                    //DaysToExpire = cGlobals.SecurityDT.Rows[0]["DaysToExpire"].ToString();
                    if (cGlobals.SecurityDT.Rows[0]["DaysToExpire"].ToString() != "-1")
                    {
                        if (cGlobals.SecurityDT.Rows[0]["DaysToExpire"].ToString() != "-1")
                            MessageBox.Show(string.Format("Password will expire in ") + cGlobals.SecurityDT.Rows[0]["DaysToExpire"].ToString() + " days");
                        else
                            MessageBox.Show(string.Format("Password will expire today!"));
                    }

                    //instance password screen
                    AdminMFA AdminMFAScreen = new AdminMFA(Password);
                    //////////////////////////////////////////////////////////////
                    //create a new window and embed  Screen usercontrol, show it as a dialog
                    System.Windows.Window AdminMFAWindow = new System.Windows.Window();
                    //set new window properties///////////////////////////
                    AdminMFAWindow.Title = "Security Question";
                    AdminMFAWindow.MaxHeight = 250;
                    AdminMFAWindow.MaxWidth = 250;
                    /////////////////////////////////////////////////////
                    //set screen as content of new window
                    AdminMFAWindow.Content = AdminMFAScreen;
                    //open new window with embedded user control
                    AdminMFAWindow.ShowDialog();
                    if (cGlobals.SecurityDT.Rows[0]["passwordOK"].ToString() == "N")
                    {
                        //throw new Exception("Invalid Password");
                        MessageBox.Show(string.Format("Invalid Password"));
                        System.Environment.Exit(0);
                    }
                }

                if (cGlobals.DatabaseName.ToLower()=="tul1razddb1 razer")
                {

                    string SourceString = "Resources\\Styles\\StylePurple.xaml";
                    d.Source = new Uri(SourceString, UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(d);
                }
                else if (cGlobals.DatabaseName.ToLower() == ("tul1razddb1new razer"))
                {
                    string SourceString = "Resources\\Styles\\StylePurple.xaml";
                    d.Source = new Uri(SourceString, UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(d);
                }
                else if (cGlobals.DatabaseName.ToLower()==("tul1razddb1 razer_dev"))
                {
                    string SourceString = "Resources\\Styles\\StyleTeal.xaml";

                    d.Source = new Uri(SourceString, UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(d);
                }
                else if (cGlobals.DatabaseName.ToLower() == ("tul1razddb1new razer_dev"))
                {
                    string SourceString = "Resources\\Styles\\StyleTeal.xaml";

                    d.Source = new Uri(SourceString, UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(d);
                }
                //else if (cGlobals.DatabaseName.ToLower().StartsWith("tul1razpdb1"))
                //{

                //    string SourceString = "Resources\\Styles\\StyleBlue.xaml";

                //    d.Source = new Uri(SourceString, UriKind.Relative);
                //    Application.Current.Resources.MergedDictionaries.Add(d);
                //}
                else if (cGlobals.DatabaseName.ToLower() == ("tul1razpdb1 razer"))
                {

                    string SourceString = "Resources\\Styles\\StyleBlue.xaml";

                    d.Source = new Uri(SourceString, UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(d);
                }
                else if (cGlobals.DatabaseName.ToLower() == ("tul1razpdb1new razer"))
                {
                    string SourceString = "Resources\\Styles\\StylePurple.xaml";

                    d.Source = new Uri(SourceString, UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(d);
                }
                else if (cGlobals.DatabaseName.ToLower() == "tul1raztdb1 razer_test")
                {

                    string SourceString = "Resources\\Styles\\StyleGreen.xaml";
                    d.Source = new Uri(SourceString, UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(d);

                }
                else if (cGlobals.DatabaseName.ToLower() == "tul1raztdb1new razer_test")
                {

                    string SourceString = "Resources\\Styles\\StyleGreen.xaml";
                    d.Source = new Uri(SourceString, UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(d);

                }
                else if (cGlobals.DatabaseName.ToLower() == "tul1raztdb1 razer_uat")
                {
                    string SourceString = "Resources\\Styles\\StyleRed.xaml";
                    d.Source = new Uri(SourceString, UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(d);
                }
                else if (cGlobals.DatabaseName.ToLower() == "tul1raztdb1new razer_uat")
                {
                    string SourceString = "Resources\\Styles\\StyleRed.xaml";
                    d.Source = new Uri(SourceString, UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(d);
                }
                //If not found then black will be used.
                //else
                //{

                //}
                StatusBar.Background = (LinearGradientBrush)TryFindResource("MenuBackgroundGradient");
                

            }
            catch (Exception ex)
            {                               
                MessageBox.Show(string.Format("The following error occurred starting the application:  {0}", ex.Message));
                throw ex;
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IEnumerable<ScreenBase> OpenScreens = UIHelper.FindChildren<ScreenBase>(this);
            bool HasDirtyScreens = false;

            foreach (ScreenBase sb in OpenScreens)
            {
                sb.Prep_ucBaseGridsForSave();

                if (sb.IsScreenDirty)
                {
                    HasDirtyScreens = true;
                    break;
                }
            }

            if (HasDirtyScreens)
            {
                var result = Messages.ShowYesNo("You have unsaved changes.  Are you sure you want to close the application before saving?", MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// DWR Added 1/30/13 - Resizes status bar anytime the window size is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RazerMainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Find the status bar
            var StatusBar = (this.RazerMainWindow.Content as Infragistics.Windows.Ribbon.RibbonWindowContentHost).StatusBar;
            if (StatusBar != null)
            {
                //Get the display grid from the StatusBar
                var Grids = (from x in UIHelper.FindChildren<Grid>(StatusBar)
                                  where x.Name == "StatusBarGrid"
                                  select x);

                //set text of databaseName
                foreach (Grid g in Grids)
                {
                    //Had to use desired size as width was not updating on maximize
                    g.Width=this.DesiredSize.Width-25 ;
                    
                }
            }
        }
    }
}
