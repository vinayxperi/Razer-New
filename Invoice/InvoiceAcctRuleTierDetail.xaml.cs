

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



    public partial class InvoiceAcctRuleTierDetail : ScreenBase 
    {
        



        public InvoiceAcctRuleTierDetail(cBaseBusObject invAcctRuleTierObj)
            : base()
        {
            // set the businessObject
            this.CurrentBusObj = invAcctRuleTierObj;
            // This call is required by the designer.
            InitializeComponent();
            Init();

        }





    

        public void Init()
        {
            
       
            this.MainTableName = "InvoiceAcctRuleTier";
          //set up grids
            gInvoiceRuleDetail.xGrid.FieldSettings.AllowEdit = false;
            gInvoiceRuleDetail.MainTableName = "ruledetail";
            gInvoiceRuleDetail.SetGridSelectionBehavior(false, true);
            gInvoiceRuleDetail.FieldLayoutResourceString = "InvoiceRuleDetail";

            gInvoiceAcctDetail.MainTableName = "acctdetail";
            gInvoiceAcctDetail.SetGridSelectionBehavior(true, false);
            gInvoiceAcctDetail.FieldLayoutResourceString = "InvoiceAcctDetail";
            gInvoiceRuleDetail.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "inv_rule_id" }, ChildGrids = { gInvoiceAcctDetail }, ParentFilterOnColumnNames = { "inv_rule_id" } });

            gInvoiceTierDetail.MainTableName = "tierdetail";
            gInvoiceTierDetail.SetGridSelectionBehavior(true, false);
            gInvoiceTierDetail.FieldLayoutResourceString = "InvoiceTierDetail";
            gInvoiceRuleDetail.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "inv_rule_id" }, ChildGrids = { gInvoiceTierDetail }, ParentFilterOnColumnNames = { "inv_rule_id" } });


           
            GridCollection.Add(gInvoiceRuleDetail);
            GridCollection.Add(gInvoiceAcctDetail);
            GridCollection.Add(gInvoiceTierDetail);

            this.Load();
            string sdebug = "";
           


        }

    


    }
}
