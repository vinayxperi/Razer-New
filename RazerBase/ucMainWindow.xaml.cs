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
using System.IO;
using Infragistics.Windows.Ribbon;
using System.Xaml;
using System.Xml;
using Infragistics.Windows.DockManager;
using Razer.Common;
using System.Configuration;
using System.Windows.Controls.Primitives;
using System.Reflection;
using Infragistics.Windows.DataPresenter;


namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucMainWindow.xaml
    /// </summary>
    public partial class ucMainWindow : UserControl
    {
        public ucMainWindow()
        {
            InitializeComponent();
            LoadMenuItems();
        }


        private void LoadMenuItems()
        {
            try
            {
                //Load the menu from loose xaml
                XamRibbon menu;
                Assembly a = Assembly.GetEntryAssembly();

                using (Stream stream = a.GetManifestResourceStream("Razer.Resources.XamRibbon.xaml"))
                {
                    menu = System.Windows.Markup.XamlReader.Load(stream) as XamRibbon;
                    stream.Close();
                }

                foreach (ButtonTool button in menu.ApplicationMenu.FooterToolbar.FindChildren<ButtonTool>())
                {
                    if (button.Id == "ExitButton")
                    {
                        button.Command = cCommandLibrary.ExitApplication;
                    }
                }


                //if (exit != null)
                //{
                //    exit.Command = cCommandLibrary.ExitApplication;
                //}

                //Load the content area from loose xaml XamDockManager
                XamDockManager dockManager;
                using (Stream stream = a.GetManifestResourceStream("Razer.Resources.XamDockManager.xaml"))
                {
                    dockManager = System.Windows.Markup.XamlReader.Load(stream) as XamDockManager;
                    stream.Close();
                }


                //Must have a RibbonWindowContentHost since the main window derives from XamRibbonWindow
                RibbonWindowContentHost contentHost = new RibbonWindowContentHost();
                
                
                this.Content = contentHost;

                //add the menu to the window
                contentHost.Ribbon = menu;


                DockPanel.SetDock(menu, Dock.Top);

                //add the XamDockManager to the content area
                contentHost.Content = dockManager;

                //Build the status bar grid
                Grid g = new Grid();
                RowDefinition rd = new RowDefinition();
                ColumnDefinition cd ;
                for (int i = 0; i < 3; i++ )
                {
                    cd = new ColumnDefinition();
                    cd.Width = new GridLength(1, GridUnitType.Star);

                    g.ColumnDefinitions.Add(cd);
                }

                g.RowDefinitions.Add(rd);
                g.Width = 1000;
                g.Name = "StatusBarGrid";


                //add a status bar to the window
                StatusBar statusBar = new StatusBar();
                //statusBar.HorizontalContentAlignment = HorizontalAlignment.Right;



                //Get the application path
                string aPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

                //Create the bitmap brush from the my Razer logo
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(aPath + "\\Resources\\razerLogo.png", UriKind.RelativeOrAbsolute);
                bi.EndInit();

                ImageBrush ib = new ImageBrush(bi);
                ib.Stretch = Stretch.Uniform;

                //Create a rectangle for storing the logo
                Rectangle r = new Rectangle();
                r.Width = 120;
                r.Height = 15;
                r.Fill = ib;
                Grid.SetColumn(r, 1);
                g.Children.Add(r);

                //add the logo to the status bar
                //statusBar.Items.Add(r);

                //Create the HueSlider slider control and add to the status bar
                //HueSlider hs = new HueSlider();
                //hs.Maximum = 8;
                //hs.SmallChange = 1;
                //hs.TickFrequency = 1;
                //hs.TickPlacement = TickPlacement.Both;
                //hs.Width = 250;
                //hs.Height = 15;
                //hs.IsSnapToTickEnabled = true;
                //statusBar.Items.Add(hs);


                //create the textblock control for status messages and add to the status bar
                TextBlock ready = new TextBlock();
                //ready.Margin = new Thickness(0, 0, 50, 0);
                ready.Text = "Ready";
                Grid.SetColumn(ready, 0);
                //statusBar.Items.Add(ready);
                ready.Foreground = Brushes.White;
                ready.FontWeight = FontWeights.Bold;
                g.Children.Add(ready);

                //create the texblock control for the database status message and add to the status bar
                //TextBlock databaseName = new TextBlock();
                TextBlock databaseName = new TextBlock();
                //databaseName.Margin = new Thickness(0, 0, 500, 0);
                databaseName.Name = "databaseName";
                databaseName.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetColumn(databaseName, 2);
                databaseName.Foreground = Brushes.White;
                databaseName.FontWeight = FontWeights.Bold;
                //statusBar.Items.Add(databaseName);
                g.Children.Add(databaseName);

                statusBar.Items.Add(g);

                contentHost.StatusBar = statusBar;





                //Since we are using loose xaml need to add the routed event handlers here in code
                EventAggregator eventAggregator = new EventAggregator();
                foreach (ButtonTool button in UIHelper.FindChildren<ButtonTool>(menu))
                {
                    //if (button.Id == "ExitButton") { button.Command = cCommandLibrary.ExitApplication; }

                    if (button.Location == ToolLocation.Ribbon)
                    {
                        button.Click += new RoutedEventHandler(EventAggregator.RibbonButtonClickHandler);
                    }
                }

                //Set the configuration section that contains the window mappings
                WindowMappingsConfigSection section = (WindowMappingsConfigSection)ConfigurationManager.GetSection("WindowMappingsSection");
                if (section != null)
                {
                    EventAggregator.MenuMappings = section.MappingItems;
                }
                else
                {
                    throw new ApplicationException("Failed to load window mapping information from configuration.");
                }

                //Use code below to help determine why XAML doesn't convert to a valid object graph.
                //using (FileStream stream = new FileStream(@"Resources\XamRibbon.xaml", FileMode.Open))
                //{
                //    XamlXmlReader reader = new XamlXmlReader(stream);
                //    XamlObjectWriter writer = new XamlObjectWriter(reader.SchemaContext);

                //    while (reader.Read())
                //    {
                //        writer.WriteNode(reader);
                //    }
                //    stream.Close();  
                //    object menu = writer.Result;
                //    RibbonWindowContentHost contentHost = new RibbonWindowContentHost();
                //    this.Content = contentHost;
                //    contentHost.Ribbon = (XamRibbon)menu;
                //    DockPanel.SetDock((XamRibbon)menu, Dock.Top);
                //}  
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("The following error occurred loading the main window:  {0}", ex.Message));
            }
        }

 
    }
}
