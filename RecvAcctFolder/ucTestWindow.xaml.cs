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
using System.Data;
using Infragistics.Windows.DataPresenter;
using RazerBase;

namespace RecvAcctFolder
{
    /// <summary>
    /// Interaction logic for ucTestWindow.xaml
    /// </summary>
    public partial class ucTestWindow : ScreenBase
    {
        public ucTestWindow()
        {
            InitializeComponent();
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
            SetParentGridAttributes();
            SetBaseLookupAttributes();
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
        }

        private void SetParentGridAttributes()
        {
            //Create the receivable account object
            CurrentBusObj = new cBaseBusObject("RecvAcct");
            CurrentBusObj.Parms.ClearParms();
            CurrentBusObj.Parms.AddParm("@receivable_account", "8041022");
            CurrentBusObj.Parms.AddParm("@product_code", "a");
            //setup parent grid
            Style s = (System.Windows.Style)Application.Current.Resources["CreditMemoStatusColorConverter"];
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.DataRecordPresenterStyle = s;
            uBG1.xGrid.FieldLayoutSettings = f;
            uBG2.xGrid.FieldLayoutSettings = f;

            //setup attributes for Parent
            MainTableName = "Aging";
            uBG1.WindowZoomDelegate = ReturnSelectedData;
            uBG1.MainTableName = "Aging";
            uBG1.IsFilterable = true;
            uBG1.SetGridSelectionBehavior(true, false);
            uBG1.FieldLayoutResourceString = "RecvAcctAgingTotal";
            //uBG1.IsParent = true;
            //uBG1.mFilterOnColumnName = "product_code";

            GridCollection.Add(uBG1);

            //setup attributes for Child
            uBG2.MainTableName = "AgingDetail";
            uBG2.IsFilterable = true;
            uBG2.WindowZoomDelegate = ReturnSelectedData;
            uBG2.SetGridSelectionBehavior(true, false);
            uBG2.IsFilterable = true;
            uBG2.FieldLayoutResourceString = "RecvAcctAgingDetail";
            //uBG2.mFilterOnColumnName = "product_code";
            //uBG1.ChildGridCollection.Add(uBG2);

            uBG3.MainTableName = "AgingDetail";
            uBG3.IsFilterable = true;
            uBG3.WindowZoomDelegate = ReturnSelectedData;
            uBG3.SetGridSelectionBehavior(true, false);
            uBG3.IsFilterable = true;
            uBG3.FieldLayoutResourceString = "RecvAcctAgingDetail";
            //uBG3.mFilterOnColumnName = "product_code";
            //uBG1.ChildGridCollection.Add(uBG3);

            MainTableName = "Aging";
            uBG4.WindowZoomDelegate = ReturnSelectedData;
            uBG4.MainTableName = "Aging";
            uBG4.IsFilterable = true;
            uBG4.SetGridSelectionBehavior(true, false);
            uBG4.FieldLayoutResourceString = "RecvAcctAgingTotal";
            //uBG4.IsParent = true;
            //uBG4.mFilterOnColumnName = "product_code";
            GridCollection.Add(uBG4);

            uBG5.MainTableName = "AgingDetail";
            uBG5.IsFilterable = true;
            uBG5.WindowZoomDelegate = ReturnSelectedData;
            uBG5.SetGridSelectionBehavior(true, false);
            uBG5.IsFilterable = true;
            uBG5.FieldLayoutResourceString = "RecvAcctAgingDetail";
            //uBG5.mFilterOnColumnName = "product_code";
            //uBG4.ChildGridCollection.Add(uBG5);

            Load(CurrentBusObj);

            //OLD ParentChildGrid
            //uPC.ParentGridStyle = "CreditMemoStatusColorConverter";
            //uPC.ParentScreenBaseMainTable = "Aging";
            //uPC.ParentGridMainTable = "Aging";
            //uPC.ParentGridIsFilterable = true;
            //uPC.ParentGridFullRowSelect = true;
            //uPC.ParentMultiRowSelect = false;
            //uPC.ParentFieldLayoutResourceString = "RecvAcctAgingTotal";

            ////setup child grid
            //uPC.ChildGridStyle = "CreditMemoStatusColorConverter";
            //uPC.ChildScreenBaseMainTable = "AgingDetail";
            //uPC.ChildGridMainTable = "Aging";
            //uPC.ChildGridIsFilterable = true;
            //uPC.ChildGridFullRowSelect = true;
            //uPC.ChildMultiRowSelect = false;
            //uPC.ChildFieldLayoutResourceString = "RecvAcctAgingDetail";

            //uPC.ParentGridDefaultFilterColumnName = "company_code";
            //uPC.ParentChildGridFilterONColumnName = "product_code";

        }

        private void SetBaseLookupAttributes()
        {

        }
        
        public void ReturnSelectedData()
        {
            //Zoom Functionality

        }

    }
}
