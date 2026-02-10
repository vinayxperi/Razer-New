using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;


namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for ManInvItemAlloc.xaml
    /// </summary>
    public partial class ManInvItemAlloc : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ManInvItemAlloc";
        private static readonly string mainTableName = "man_inv_item_alloc";

        //Needed for the combobox
        private static readonly string RuleHdrIDTableName = "rule_hdr";
        private static readonly string RuleHdrIDDisplayPath = "item_rule_desc";
        private static readonly string RuleHdrIDValuePath = "item_rule_id";

        //Needed for a combobox
        public ComboBoxItemsProvider cmbRuleHdrID { get; set; }
               
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public ManInvItemAlloc()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            //Adds the insert row at the top
            layouts.AllowAddNew = true;
            layouts.AddNewRecordLocation = AddNewRecordLocation.OnTop;

            this.CurrentBusObj = businessObject;

            this.MainTableName = mainTableName;
            idgManInvItemAlloc.xGrid.FieldLayoutSettings = layouts;
            idgManInvItemAlloc.FieldLayoutResourceString = fieldLayoutResource;
            idgManInvItemAlloc.MainTableName = mainTableName;
            idgManInvItemAlloc.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);


            //load Rule Header combobox
            if (businessObject.HasObjectData)
            {
                cmbRuleHdrID = new ComboBoxItemsProvider();
                cmbRuleHdrID.ItemsSource = businessObject.ObjectData.Tables[RuleHdrIDTableName].DefaultView;
                cmbRuleHdrID.ValuePath = RuleHdrIDValuePath;
                cmbRuleHdrID.DisplayMemberPath = RuleHdrIDDisplayPath;
            }

            idgManInvItemAlloc.LoadGrid(businessObject, idgManInvItemAlloc.MainTableName);
        }

        public override void Save()
        {
            Boolean bError = false;
            bool OverrideSaveTableListing = UIHelper.FindVisualParent<TableListing>(this).OverrideSave;

            UIHelper.FindVisualParent<TableListing>(this).OverrideSave = true;

            Int32 ictr = 1;
           
            decimal pctTotD = 0;
            decimal pctSumD = 0;
            int ruleID = 0;
           


            DataView view = new DataView(this.CurrentBusObj.ObjectData.Tables["man_inv_item_alloc"]); 
            DataTable distinctValues = view.ToTable(true, "item_rule_id");
            if (distinctValues.Rows.Count  > 0)
                ictr = distinctValues.Rows.Count;



            foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["man_inv_item_alloc"].Rows)
            {
                ruleID = Convert.ToInt32(r["item_rule_id"]);
               //validate each row is = 100, otherwise error!
                
                pctTotD = pctTotD + Convert.ToDecimal(r["alloc_pct"]);


            }
            pctSumD = pctTotD / ictr;

            if (pctSumD != 100)
            {
                bError = true;
                Messages.ShowError("Rules must total 100%");
            }

            if (bError == false)
            {
                base.Save();
                if (SaveSuccessful)
                {

                    Messages.ShowInformation("Save Successful");
                }
                else
                {
                    Messages.ShowInformation("Save Failed");
                }
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }



    }
}

