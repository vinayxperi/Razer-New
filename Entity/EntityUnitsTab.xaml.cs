

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace Entity
{

    #region class EntityUnitsTab
    /// <summary>
    /// This class represents a 'EntityCountTab' object.
    /// </summary>
    public partial class EntityUnitsTab : ScreenBase
    {
        String startDate;
        String endDate;
          

         
 
       
        /// Create a new instance of a 'EntityCountTab' object and call the ScreenBase's constructor.
        
        public EntityUnitsTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
       

        
        public void Init()
        {
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
           
            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "units";
            //Establish the WHT Tracking Grid
            GridEntityUnits.MainTableName = "units";
            GridEntityUnits.ConfigFileName = "GridEntityUnits";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridEntityUnits.SetGridSelectionBehavior(false, true);
            GridEntityUnits.FieldLayoutResourceString = "units";

            GridCollection.Add(GridEntityUnits);
        }

        //Retrieve Button pressed - retrieves data to populate grid based on search criteria
        private void bRetrieve_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            //if no records found return
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count < 1)
                return;

            //If no start input then select all previous
            if (txtServiceDateStart.SelText.ToString() == "")
                this.CurrentBusObj.changeParm("@service_period_start", "1/1/1900");
            else
                this.CurrentBusObj.changeParm("@service_period_start", txtServiceDateStart.SelText.ToString());

            //if no end date then select all future dates
            if (txtServiceDateEnd.SelText.ToString() == "")
                this.CurrentBusObj.changeParm("@service_period_end", "12/31/2100");
            else
                this.CurrentBusObj.changeParm("@service_period_end", txtServiceDateEnd.SelText.ToString());

           
            this.CurrentBusObj.LoadTable("units");
            if (CurrentBusObj.ObjectData.Tables["units"].Rows.Count == 0)
            {
                Messages.ShowWarning("No Units for Dates Specified");
            }
            else
            {
                //KSH - 8/16/12 total unit amts in grid
                sumUnitTotals();
            }
        }

        /// <summary>
        /// Sums unit amounts in grid and puts result in textbox
        /// KSH - 8/16/12 added
        /// </summary>
        private void sumUnitTotals()
        {
            decimal decSumUnitAmts = 0;
            //loop through grid and sum unit amts
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["units"].Rows)
            {
                decSumUnitAmts += Convert.ToDecimal(dr["amount"]);
            }
            txtUnitTotal.Text = decSumUnitAmts.ToString("n"); //the "n" separates thousands with comma
        }

        private string getEntityID()
        {
            var ParmInfo = from x in CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                        where x.Field<string>("parmName") == "@mso_id"
                                        select new
                                        {
                                            parmName = x.Field<string>("parmName"),
                                            parmValue = x.Field<string>("parmValue")
                                        };

               foreach (var info in ParmInfo)
                { 
                    if (info.parmName == "@mso_id")
                        return info.parmValue;
                
                 }
               return "";
        }


        private void txtServiceDateEnd_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }

       

        private void txtServiceDateEnd_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void txtServiceDateStart_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void txtServiceDateStart_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
                        

        }

       

    }
    #endregion

}
