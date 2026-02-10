using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using RazerBase;

namespace RecvAcctFolder
{
    /// <summary>
    /// Interaction logic for ucRecvAcctLookup.xaml
    /// </summary>
    public partial class ucRecvAcctLookup : DialogBase
    {
        public ucRecvAcctLookup()
        {
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);

            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            uBaseLookup.ClearLookup();

            //Define all lookup fields
            uBaseLookup.AddLookup("tReceivableAcct", "receivable_account");
            uBaseLookup.AddLookup("tAcctName", "account_name");
            uBaseLookup.AddLookup("tAddress1", "address1");
            uBaseLookup.AddLookup("tAcctDescription", "description");
            uBaseLookup.AddLookup("tCity", "city");
            uBaseLookup.AddLookup("tState", "state");

            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add("receivable_account");

            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = ReturnSelectedData;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            //uBaseLookup.uGrid.SelectProcedureIsQuery = true;
            uBaseLookup.uGrid.FieldLayoutResourceString = "RecvAcctLookup";
            uBaseLookup.uGrid.MainTableName = "main";

            //Set the rows to change color based on account_status field
            Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];

            FieldLayoutSettings f = new FieldLayoutSettings();
            f.DataRecordPresenterStyle = s;
            uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")
            cBaseBusObject obj = new cBaseBusObject(this.CurrentBusObj.BusObjectName);
            obj.BusObjectName = "recv_acct_lookup_grid_wpf";
            Load(obj);

            uBaseLookup.uGrid.LoadGrid(obj, uBaseLookup.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);

        }

        private void FilterKeyPress(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            //Filter returned data by individual key strokes
            uBaseLookup.FilterKeyPress(sender, e);
        }

        public void ReturnSelectedData()
        {
            DataRecord r = default(DataRecord);
            //Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            //If error condition is received when retrieving selected row then the row of the currently active cell is used.
            try
            {
                r = (Infragistics.Windows.DataPresenter.DataRecord)uBaseLookup.uGrid.xGrid.SelectedItems.Records[0];
            }
            catch (Exception ex)
            {
                // for debugging only
                string err = ex.ToString();

                // Set the current record
                r = uBaseLookup.uGrid.xGrid.ActiveCell.Record;
            }
            cGlobals.ReturnParms.Clear();
            cGlobals.ReturnParms.Add(r.Cells["receivable_account"].Value);
            this.Close();

        }

        private void chkInactive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Took this out so would compile
            //try
            //{
            //    // Show Inactives
            //    if (chkInactive.IsChecked == true)
            //    {
            //        uBaseLookup.uGrid.SelectProcedure = "SELECT dbo.recv_account_u.receivable_account," + "dbo.recv_account_u.account_name, dbo.recv_account_u.address_1,dbo.recv_account_u.city,dbo.recv_account_u.state," + "dbo.recv_account_u.description,dbo.recv_account_u.account_status" + " FROM dbo.recv_account_u WHERE dbo.recv_account_u.account_name LIKE '%'  ORDER BY dbo.recv_account_u.account_name";
            //    }
            //    else
            //    {
            //        uBaseLookup.uGrid.SelectProcedure = "SELECT dbo.recv_account_u.receivable_account," + "dbo.recv_account_u.account_name, dbo.recv_account_u.address_1,dbo.recv_account_u.city,dbo.recv_account_u.state," + "dbo.recv_account_u.description,dbo.recv_account_u.account_status" + " FROM dbo.recv_account_u WHERE dbo.recv_account_u.account_name LIKE '%' and account_status = 0 ORDER BY dbo.recv_account_u.account_name";
            //    }


            //    //Load the grid
            //    uBaseLookup.uGrid.FillGrid();
            //    System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
            //}
            //catch (Exception ex) { throw ex; }
            //finally { System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow); }
        }

        private void DialogBase_Loaded(object sender, RoutedEventArgs e)
        {
            //Check return parameters and add to filter cells

            if (cGlobals.ReturnParms.Count > 0)
            {
                tReceivableAcct.Text = cGlobals.ReturnParms[0].ToString();
                FilterKeyPress(tReceivableAcct, null);
                tAcctName.Text = cGlobals.ReturnParms[1].ToString();
                FilterKeyPress(tAcctName, null);
            }

            cGlobals.ReturnParms.Clear();
        }

    }
}
