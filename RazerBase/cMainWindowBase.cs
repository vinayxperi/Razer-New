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
using Infragistics.Windows.Ribbon;

namespace RazerBase
{
    public class cMainWindowBase : XamRibbonWindow
    {
        public RazerWS.IRazerService LCService { get; set; }
        public RazerWS.IBillingService BillService { get; set; }
        //public RazerWS.IWHTCalcEntryService WHTCalcEntryService { get; set; }
        
        public cMainWindowBase()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(CatchUnhandledExceptionHandler);
        }

        private static void CatchUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            StringBuilder builder = new StringBuilder("The following application error occurred:  ");
            builder.Append(e.Message);
            MessageBox.Show(builder.ToString());
        }
    }
}
