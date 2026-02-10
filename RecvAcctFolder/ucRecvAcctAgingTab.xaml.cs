

#region using statements

using System.Windows;
using Infragistics.Windows.DataPresenter;
using RazerBase;

#endregion

namespace RecvAcctFolder
{

    #region class ucRecvAcctAgingTab
    /// <summary>
    /// Interaction logic for ucRecvAcctAgingTab.xaml
    /// </summary>
    public partial class ucRecvAcctAgingTab : ScreenBase
    {
        
        #region Private Variables
        #endregion
        
        #region Constructor
        /// <summary>
        /// This constructor [enter description here].
        /// </summary>
        public ucRecvAcctAgingTab() : base()
        {
            // This call is required by the designer.
            InitializeComponent();
            
            // Add any initialization after the InitializeComponent() call.
            //Setup base grid information
            //Set the rows to change color based on account_status field
            
            Style s = (System.Windows.Style)Application.Current.Resources["CreditMemoStatusColorConverter"];
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.DataRecordPresenterStyle = s;
            gAgingDetail.xGrid.FieldLayoutSettings = f;
            
            MainTableName = "Aging";
            gAgingTotal.WindowZoomDelegate = ReturnSelectedData;
            gAgingTotal.MainTableName = "Aging";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gAgingTotal.SetGridSelectionBehavior(true, false);
            gAgingTotal.FieldLayoutResourceString = "RecvAcctAgingTotal";
            
            GridCollection.Add(gAgingTotal);
            
            gAgingDetail.MainTableName = "AgingDetail";
            gAgingDetail.IsFilterable = true;
            gAgingDetail.WindowZoomDelegate = ReturnSelectedData;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gAgingDetail.SetGridSelectionBehavior(true, false);
            gAgingDetail.FieldLayoutResourceString = "RecvAcctAgingDetail";
            
            GridCollection.Add(gAgingDetail);
            
        }
        #endregion
        
        #region Events
            
            #region Grid_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
            /// <summary>
            /// This event [enter description here].
            /// </summary>
            private void Grid_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
            {
                if (gAgingTotal.xGrid.SelectedItems.Records.Count > 0)
                {
                    DataRecord r = default(DataRecord);
                    r = (Infragistics.Windows.DataPresenter.DataRecord)(gAgingTotal.xGrid.SelectedItems.Records[0]);
                    gAgingDetail.FilterGrid("company_code", r.Cells["company_code"].Value.ToString());
                    
                    if (r.Cells["product_code"].Value.ToString().ToUpper() != "ALL")
                    {
                        gAgingDetail.FilterGrid("product_code", r.Cells["product_code"].Value.ToString());
                    }
                    else
                    {
                        gAgingDetail.FilterGrid("product_code", "");
                    }
                }
                
            }
            #endregion
            
        #endregion
        
        #region Methods
            
            #region ReturnSelectedData()
            /// <summary>
            /// This method [enter description here].
            /// </summary>
            public void ReturnSelectedData()
            {
                //Zoom Functionality
                
            }
            #endregion
            
        #endregion
        
    }
    #endregion

}
