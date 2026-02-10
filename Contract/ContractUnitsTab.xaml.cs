

using RazerBase;
using RazerInterface;
using System;
using System.Data;

namespace Contract
{

    /// <summary>
    /// This class represents a 'ContractUnitsTab' object.
    /// </summary>
    public partial class ContractUnitsTab : ScreenBase
    {
        /// <summary>
        /// Create a new instance of a 'ContractUnitsTab' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractUnitsTab()
            : base()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            this.CanExecuteNewCommand = false;
            this.CanExecuteSaveCommand = false;
            MainTableName = "contract_unit_counts";
            GridContractUnits.MainTableName = "contract_unit_counts";
            GridContractUnits.ConfigFileName = "GridContractUnits";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridContractUnits.SetGridSelectionBehavior(false, true);
            GridContractUnits.FieldLayoutResourceString = "ContractUnitCounts";

            GridCollection.Add(GridContractUnits);
        }

        /// <summary>
        /// Sums unit amounts in grid and puts result in textbox
        /// KSH - 8/16/12 added
        /// </summary>
        public void sumUnitTotals()
        {
            decimal decSumUnitAmts = 0;
            //loop through grid and sum unit amts
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["contract_unit_counts"].Rows)
            {
                decSumUnitAmts += Convert.ToDecimal(dr["amount"]);
            }
            txtUnitTotal.Text = decSumUnitAmts.ToString("n"); //the "n" separates thousands with comma
        }

        /// <summary>
        /// Retrieve Button pressed - retrieves data to populate grid based on search criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bRetrieve_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //KSH - 8/29/12 keep button click from blowing up app when no rec selected
            if (this.CurrentBusObj == null)
                return;
            //If no start input then select all previous
            if (txtServiceDateStart.SelText.ToString() == "")
                this.CurrentBusObj.changeParm("@service_period_start", "1/1/1900");
            else
                this.CurrentBusObj.changeParm("@service_period_start", txtServiceDateStart.SelText.ToString());

            //if no end date then select all future dates
            if (txtServiceDateEnd.SelText.ToString() == "1/1/1900 12:00:00 AM")
                this.CurrentBusObj.changeParm("@service_period_end", "12/31/2100");
            else
                this.CurrentBusObj.changeParm("@service_period_end", txtServiceDateEnd.SelText.ToString());

            this.CurrentBusObj.LoadTable("contract_unit_counts");
            if (CurrentBusObj.ObjectData.Tables["contract_unit_counts"].Rows.Count == 0)
            {
                Messages.ShowWarning("No Units for Dates Specified");
            }
            else
            {
                //KSH - 8/16/12 total unit amts in grid
                sumUnitTotals();
            }
        }
    }
}
