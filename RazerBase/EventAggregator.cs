using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.Windows.Ribbon;
using System.Windows;
using Infragistics.Windows.DockManager;
using Razer.Common;
using Microsoft.Practices.Unity;
using RazerBase.Interfaces;
using System.Windows.Controls;
using Infragistics.Windows.Editors;

namespace RazerBase
{    
    /// <summary>
    /// Class for defining and handling Routed events for UI.
    /// </summary>
    public class EventAggregator : UIElement
    {
        private static readonly string dockManager = "XamDockManager1";

        /// <summary>
        /// Store the mappings for menu/window relationships
        /// </summary>
        private static WindowMappingCollection menuMappings;

        public static WindowMappingCollection MenuMappings
        {
            get { return EventAggregator.menuMappings; }
            set { EventAggregator.menuMappings = value; }
        }
        
        //Use for xamDataGrid cell checkbox click events if necessary
        //EventManager.RegisterClassHandler(typeof(ValueEditor), ValueEditor.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(OnValueChanged));
        //public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("OnValueChanged", RoutingStrategy.Bubble,
        //    typeof(RoutedEventHandler), typeof(ValueEditor));

        //public event RoutedEventHandler OnValueChanged
        //{
        //    add { AddHandler(ValueChangedEvent, value); }
        //    remove { RemoveHandler(ValueChangedEvent, value); }
        //}

        /// <summary>
        /// Routed event to handle click events for the button tool on the ribbon menu.
        /// </summary>
        public static readonly RoutedEvent MenuRibbonButtonClickEvent = EventManager.RegisterRoutedEvent("RibbonButtonClick", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(ButtonTool));

        /// <summary>
        /// Define the handler for the routed event
        /// </summary>
        public event RoutedEventHandler RibbonButtonClick
        {
            add { AddHandler(MenuRibbonButtonClickEvent, value); }
            remove { RemoveHandler(MenuRibbonButtonClickEvent, value); }
        }

        /// <summary>
        /// Routed event to handle click events that are raised arbitrarily.
        /// </summary>
        public static readonly RoutedEvent GeneratedClickEvent = EventManager.RegisterRoutedEvent("GeneratedClick", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(UIElement));

        /// <summary>
        /// Define the handler for the routed event
        /// </summary>
        public event RoutedEventHandler GeneratedClick
        {
            add { AddHandler(GeneratedClickEvent, value); }
            remove { RemoveHandler(GeneratedClickEvent, value); }
        }

        /// <summary>
        /// Handler for ribbon button clicks.
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">Argument payload from the invoker.</param>
        public static void RibbonButtonClickHandler(object sender, RoutedEventArgs e)
        {
            if (typeof(ButtonTool) == e.Source.GetType())
            {          
                ButtonTool tool = (ButtonTool)e.Source;
                //WindowMappingElement mapping = menuMappings[tool.Id.ToString()];
                InitializeWindow(menuMappings[tool.Id.ToString()], (FrameworkElement)tool);               

                e.Handled = true;
            }
        }

        /// <summary>
        /// Handler for misc generated click events.
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">Argument payload from the invoker.</param>
        public static void GeneratedClickHandler(object sender, RoutedEventArgs e)
        {
            FrameworkElement tool = e.Source as FrameworkElement;

            if (tool != null)
            {        
                if (cGlobals.ReturnParms.Count > 1)
                {             
                    var mappings = (from m in menuMappings.OfType<WindowMappingElement>()
                                    where cGlobals.ReturnParms[1].ToString() == m.ButtonId
                                    select m).ToList();

                    if (mappings != null && mappings.Count > 0)
                    {
                        InitializeWindowById((WindowMappingElement)mappings[0]);                        
                    }
                }
                else if (cGlobals.ReturnParms.Count > 0)
                {
                    var idMappings = (from m in menuMappings.OfType<WindowMappingElement>()
                                    where cGlobals.ReturnParms[0].ToString() == m.ButtonId
                                    select m).ToList();

                    if (idMappings != null && idMappings.Count > 0)
                    {
                        InitializeWindow((WindowMappingElement)idMappings[0]);
                    }
                }                
                else
                {
                    InitializeWindow(menuMappings[tool.Name.ToString()], tool);
                }
            }            

            e.Handled = true;
            
        }


        private static void InitializeWindow(WindowMappingElement mapping, FrameworkElement tool)
        {
            if (mapping == null)
            {
                Messages.ShowWarning("The window you have chosen does not contain a mapping.");
                return;
            }

            cBaseBusObject razerObject = new cBaseBusObject(mapping.RazerObject);               

            ParameterOverrides overrides = new ParameterOverrides();
            IScreen screen = Global.Container.Resolve<IScreen>(mapping.WindowType, new ParameterOverride("", ""));
            screen.Init(razerObject);

            if (mapping.IsDialog)
            {
                Window dialog = Window.GetWindow((DependencyObject)screen);
                UIHelper.WireDialogToMainWindow(dialog);
                dialog.ShowDialog();
            }
            else
            {
                cMainWindowBase win = (cMainWindowBase)Window.GetWindow(tool);
                ContentPane contentPane = null;
                XamDockManager xamDockManager = null;
                xamDockManager = UIHelper.FindChild<XamDockManager>(win, dockManager);
                contentPane = xamDockManager.AddDocument(mapping.WindowCaption, screen);

                if (screen is ScreenBase)
                {
                    if ((screen as ScreenBase).CancelCloseOnSaveConfirmation)
                    {
                        contentPane.Closing += new EventHandler<Infragistics.Windows.DockManager.Events.PaneClosingEventArgs>(contentPaneCancelOnSave_Closing);
                    }
                    else
                    {
                        contentPane.Closing += new EventHandler<Infragistics.Windows.DockManager.Events.PaneClosingEventArgs>(contentPane_Closing);
                    }
                }
                
                contentPane.Activate();
                contentPane.AllowDocking = false;
                contentPane.AllowFloatingOnly = false;
            }                
        }

        static void contentPane_Closing(object sender, Infragistics.Windows.DockManager.Events.PaneClosingEventArgs e)
        {
            ContentPane x = sender as ContentPane;
            ScreenBase screen = x.Content as ScreenBase;
            screen.Prep_ucBaseGridsForSave();

            if ((screen != null) && (screen.CanExecuteCloseCommand))
            {
                screen.Close();
                if (!screen.SaveSuccessful)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        static void contentPaneCancelOnSave_Closing(object sender, Infragistics.Windows.DockManager.Events.PaneClosingEventArgs e)
        {
            ContentPane x = sender as ContentPane;
            ScreenBase screen = x.Content as ScreenBase;
            screen.Prep_ucBaseGridsForSave();

            if ((screen != null) && (screen.CanExecuteCloseCommand))
            {
                screen.Close();
                if (!screen.SaveSuccessful)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = screen.StopCloseIfCancelCloseOnSaveConfirmationTrue;
                }
            }
        }

        private static void InitializeWindow(WindowMappingElement mapping)
        {          
            cBaseBusObject razerObject = new cBaseBusObject(mapping.RazerObject);                

            ParameterOverrides overrides = new ParameterOverrides();
            IScreen screen = Global.Container.Resolve<IScreen>(mapping.WindowType, new ParameterOverride("", ""));
            screen.Init(razerObject);


            cMainWindowBase win = (cMainWindowBase)Window.GetWindow(Application.Current.MainWindow);
            
            var screens = (from s in UIHelper.FindChildren<ScreenBase>(win)
                            where s.Name == mapping.ContentId
                            select s).ToList();

            if (screens != null && screens.Count > 0)
            {
                ScreenBase content = (ScreenBase)screens[0];
                content.Content = screen;
                content.Visibility = Visibility.Visible;
            }         
        }

        private static void InitializeWindowById(WindowMappingElement mapping)
        {            
            cBaseBusObject razerObject = new cBaseBusObject(mapping.RazerObject);
            //razerObject.Parms.AddParm(mapping.ParameterId, cGlobals.ReturnParms[0].ToString());
            //The zero global paramter is always the button id
            if (mapping.ParameterList != null && mapping.ParameterList.Count > 0)
            {
                for (int i = 0; i < mapping.ParameterList.Count; i++)
                {
                    if (i > 0)
                    {
                        razerObject.Parms.AddParm(mapping.ParameterList[i].Name, cGlobals.ReturnParms[i+1]);
                    }
                    else
                    {
                        razerObject.Parms.AddParm(mapping.ParameterList[i].Name, cGlobals.ReturnParms[i]);
                    }
                }
            }
            ParameterOverrides overrides = new ParameterOverrides();
            IScreen screen = Global.Container.Resolve<IScreen>(mapping.WindowType, new ParameterOverride("", ""));
            screen.Init(razerObject);

            if (mapping.IsDialog)
            {
                Window dialog = Window.GetWindow((DependencyObject)screen);
                UIHelper.WireDialogToMainWindow(dialog);
                dialog.ShowDialog();
            }
            else
            {
                cMainWindowBase win = (cMainWindowBase)Window.GetWindow(Application.Current.MainWindow);
                ContentPane contentPane = null;
                XamDockManager xamDockManager = null;
                xamDockManager = UIHelper.FindChild<XamDockManager>(win, dockManager);
                contentPane = xamDockManager.AddDocument(screen.WindowCaption, screen);

                if (screen is ScreenBase)
                {
                    if ((screen as ScreenBase).CancelCloseOnSaveConfirmation)
                    {
                        contentPane.Closing += new EventHandler<Infragistics.Windows.DockManager.Events.PaneClosingEventArgs>(contentPaneCancelOnSave_Closing);
                    }
                    else
                    {
                        contentPane.Closing += new EventHandler<Infragistics.Windows.DockManager.Events.PaneClosingEventArgs>(contentPane_Closing);
                    }
                }

                contentPane.Activate();
                contentPane.AllowDocking = false;
                contentPane.AllowFloatingOnly = false;
            }
        }
    }

}
