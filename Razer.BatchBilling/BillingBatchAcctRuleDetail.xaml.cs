

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

namespace Razer.BatchBilling
{



    public partial class BillingBatchAcctRuleDetail : ScreenBase
    {




        public BillingBatchAcctRuleDetail(cBaseBusObject invAcctRuleTierObj)
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


            this.MainTableName = "BillingBatchAcctRule";
            //set up grids
            gBillingBatchAcctDetail.xGrid.FieldSettings.AllowEdit = false;
            gBillingBatchAcctDetail.MainTableName = "acctdetail";
            gBillingBatchAcctDetail.SetGridSelectionBehavior(false, true);
            gBillingBatchAcctDetail.FieldLayoutResourceString = "BillingBatchAcctDetail";
            gBillingBatchAcctDetail.ConfigFileName = "BillingBatchAcctDetail";



            gBillingBatchRuleGroup.MainTableName = "rulegroup";
            gBillingBatchRuleGroup.SetGridSelectionBehavior(true, false);
            gBillingBatchRuleGroup.FieldLayoutResourceString = "BillingBatchRuleGroup";
            gBillingBatchRuleGroup.ConfigFileName = "BillingBatchRuleGroup";



            GridCollection.Add(gBillingBatchAcctDetail);
            GridCollection.Add(gBillingBatchRuleGroup);


            this.Load();
            string sdebug = "";



        }


    }
}
