

#region using statements

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
using System.Data;
using Infragistics.Windows.DataPresenter;
 

#endregion

namespace Invoice
{

    public partial class InvoiceRevRecDetail : ScreenBase 
    {        
        public InvoiceRevRecDetail(cBaseBusObject invRevRecObj)
            : base()
        {
            // set the businessObject
            this.CurrentBusObj = invRevRecObj;
            // This call is required by the designer.
            InitializeComponent();
            Init();
        }

        public void Init()
        {           
            this.MainTableName = "InvoiceRevRec";
          //set up grids
            gInvoiceRevRec.xGrid.FieldSettings.AllowEdit = false;
            gInvoiceRevRec.MainTableName = "revrec";
            gInvoiceRevRec.SetGridSelectionBehavior(false, true);
            gInvoiceRevRec.FieldLayoutResourceString = "InvoiceRevRec";

            GridCollection.Add(gInvoiceRevRec);
            this.Load();
            //string sdebug = ""; 
        }
    }
}
