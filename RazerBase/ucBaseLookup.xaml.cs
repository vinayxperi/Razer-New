using System;
using System.Collections;
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
using Microsoft.VisualBasic;
using System.Data;
using System.Diagnostics;
using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.DataPresenter.Events;

namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucBaseLookup.xaml
    /// </summary>
    public partial class ucBaseLookup
    {
        // Used to store the container name / db name pairs from the lookup screen
        DataTable dtLookup;
        //private ArrayList mReturnParmFields = new ArrayList();
        //public ArrayList ReturnParmFields
        //{
        //    get { return mReturnParmFields; }
        //    set { mReturnParmFields = value; }
        //}

        public List<string> ReturnParmFields = new List<string>();

        public ucBaseLookup()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.


        }


        public void ClearLookup()
        {
            //Sub to reinitialize the dtlookup table
            dtLookup = new DataTable("lookup");

            //Column to store the name of the text container to be used for filtering
            dtLookup.Columns.Add("text_box_name");
            //Column to store the database field name tied to the related container
            dtLookup.Columns.Add("field_name");

        }

        public void AddLookup(string TextBoxName, string FieldName)
        {
            dtLookup.Rows.Add(TextBoxName, FieldName);

        }

        public void FilterKeyPress(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            //Code to check each change in text lookup box and refilter data.
            dynamic tBox = (TextBox)sender;
            string FieldName = "";

            try
            {
                FieldName = dtLookup.Rows[FindDTRow(ref dtLookup, "text_box_name", tBox.Name.ToString())]["field_name"];
            }
            catch
            {
                Interaction.MsgBox(tBox.Name.ToString() + " textbox not properly configured for filtering.");
                return;
            }

            //Run the base object filter code
            uGrid.FilterGrid(FieldName, tBox.Text);

        }

        public int FindDTRow(ref DataTable DT, string ColName, string sValue)
        {
            //Function to find first row in datatable that matches the criteria of sValue in column ColName
            //Returns -1 if no match is found
            int i = 0;

            if (DT == null)
            {
                return -1;
            }
            for (i = 0; i <= DT.Rows.Count - 1; i++)
            {
                if (DT.Rows[i][ColName].ToString() == sValue)
                {
                    return i;
                }
            }

            return -1;

        }

        private void bCancel_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {

            CloseWindow();

        }

        private void bOK_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            ReturnSelectedData();
        }

        public void ReturnSelectedData()
        {
            //DataRecord r = default(DataRecord);
            ////Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            ////If error condition is received when retrieving selected row then the row of the currently active cell is used.
            //try
            //{
            //    r = (Infragistics.Windows.DataPresenter.DataRecord)(uGrid.xGrid.SelectedItems.Records[0]);
            //}
            //catch (Exception ex)
            //{
            //    // for debugging only
            //    string err = ex.ToString();

            //    // Set the record of the Active Cell
            //    r = uGrid.xGrid.ActiveCell.Record;
            //}

            //cGlobals.ReturnParms.Clear();
            //for (int i = 0; i < mReturnParmFields.Count; i++)
            //{
            //    cGlobals.ReturnParms.Add(r.Cells[mReturnParmFields[i].ToString()].Value);
            //}
            if (ReturnParmFields != null)
                this.uGrid.ReturnSelectedData(ReturnParmFields);
            

            CloseWindow();

        }

        private void CloseWindow()
        {
            Window w = Window.GetWindow(this);
            if (w != null)
            {
                w.Close();
            }
        }


    }
}
