using System;
using System.Collections;
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
using Infragistics.Windows.DataPresenter;
using System.Reflection;

namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucParentChildGrid.xaml
    /// </summary>
    public partial class ucParentChildGrid : ScreenBase
    {
        private ArrayList mReturnParmFields = new ArrayList();
        public ArrayList ReturnParmFields
        {
            get { return mReturnParmFields; }
            set { mReturnParmFields = value; }
        }

        #region "Parent Grid properties"

        private string mParentGridStyle;
        public string ParentGridStyle
        {
            get { return mParentGridStyle; }
            set { mParentGridStyle = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        //sets MainTableName in ScreenBase. Will tell screen which table in multi-table
        //dataset to use for screen
        private string mParentScreenBaseMainTable;
        public string ParentScreenBaseMainTable
        {
            get { return mParentScreenBaseMainTable; }
            set { mParentScreenBaseMainTable = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        //sets MainTableName in base grid. Will tell grid which table in multi-table
        //dataset to use for grid
        private string mParentGridMainTable;
        public string ParentGridMainTable
        {
            get { return mParentGridMainTable; }
            set { mParentGridMainTable = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        private bool mParentGridIsFilterable;
        public bool ParentGridIsFilterable
        {
            get { return mParentGridIsFilterable; }
            set { mParentGridIsFilterable = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        private bool mParentGridFullRowSelect;
        public bool ParentGridFullRowSelect
        {
            get { return mParentGridFullRowSelect; }
            set { mParentGridFullRowSelect = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        private bool mParentMultiRowSelect;
        public bool ParentMultiRowSelect
        {
            get { return mParentMultiRowSelect; }
            set { mParentMultiRowSelect = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        private string mParentFieldLayoutResourceString;
        public string ParentFieldLayoutResourceString
        {
            get { return mParentFieldLayoutResourceString; }
            set { mParentFieldLayoutResourceString = value; }
        }

        #endregion

        #region "Child Grid properties"

        private string mChildGridStyle;
        public string ChildGridStyle
        {
            get { return mChildGridStyle; }
            set { mChildGridStyle = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        private string mChildScreenBaseMainTable;
        public string ChildScreenBaseMainTable
        {
            get { return mChildScreenBaseMainTable; }
            set { mChildScreenBaseMainTable = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        //sets MainTableName in base grid. Will tell grid which table in multi-table
        //dataset to use for grid
        private string mChildGridMainTable;
        public string ChildGridMainTable
        {
            get { return mChildGridMainTable; }
            set { mChildGridMainTable = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        private bool mChildGridIsFilterable;
        public bool ChildGridIsFilterable
        {
            get { return mChildGridIsFilterable; }
            set { mChildGridIsFilterable = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        private bool mChildGridFullRowSelect;
        public bool ChildGridFullRowSelect
        {
            get { return mChildGridFullRowSelect; }
            set { mChildGridFullRowSelect = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        private bool mChildMultiRowSelect;
        public bool ChildMultiRowSelect
        {
            get { return mChildMultiRowSelect; }
            set { mChildMultiRowSelect = value; }
        }

        //don't really need but eaiser for those who don't understand obj model
        private string mChildFieldLayoutResourceString;
        public string ChildFieldLayoutResourceString
        {
            get { return mChildFieldLayoutResourceString; }
            set { mChildFieldLayoutResourceString = value; }
        }

        #endregion

        #region "Grid Filtering properties"

        public string mParentGridDefaultFilterColumnName;
        public string ParentGridDefaultFilterColumnName
        {
            get { return mParentGridDefaultFilterColumnName; }
            set { mParentGridDefaultFilterColumnName = value; }
        }

        public string mParentChildGridFilterONColumnName;
        public string ParentChildGridFilterONColumnName
        {
            get { return mParentChildGridFilterONColumnName; }
            set { mParentChildGridFilterONColumnName = value; }
        }

        #endregion

        /// <summary>
        /// ucParentChildGrid constructor
        /// </summary>
        public ucParentChildGrid()
        {
            InitializeComponent();
        }

        public void LoadGrids(cBaseBusObject MainObject)
        {
            //Style s = (System.Windows.Style)Application.Current.Resources["CreditMemoStatusColorConverter"];
            Style ParentStyle = (System.Windows.Style)Application.Current.Resources[mParentGridStyle];
            Style ChildStyle = (System.Windows.Style)Application.Current.Resources[mChildGridStyle];
            FieldLayoutSettings ParentLayout = new FieldLayoutSettings();
            FieldLayoutSettings ChildLayout = new FieldLayoutSettings();
            ParentLayout.DataRecordPresenterStyle = ParentStyle;
            ChildLayout.DataRecordPresenterStyle = ChildStyle;

            uGridParent.xGrid.FieldLayoutSettings = ParentLayout;
            uGridChild.xGrid.FieldLayoutSettings = ChildLayout;

            //MainTableName = "Aging";
            MainTableName = mParentScreenBaseMainTable;
            //uGridParent.MainTableName = "Aging";
            uGridParent.MainTableName = mParentGridMainTable;
            uGridParent.IsFilterable = mParentGridIsFilterable;
            //put in if allow nullable types in base grid
            //if (mParentGridFullRowSelect == null) mParentGridFullRowSelect = false;
            //if (mParentMultiRowSelect == null) mParentMultiRowSelect = false;
            //uGridParent.SetGridSelectionBehavior(true, false);
            uGridParent.SetGridSelectionBehavior(mParentGridFullRowSelect, mParentMultiRowSelect);

            //uGridParent.FieldLayoutResourceString = "RecvAcctAgingTotal";
            uGridParent.FieldLayoutResourceString = mParentFieldLayoutResourceString;

            uGridParent.WindowZoomDelegate = ReturnSelectedData;

            GridCollection.Add(uGridParent);

            //uGridChild.MainTableName = "AgingDetail";
            uGridChild.MainTableName = mChildScreenBaseMainTable;
            //uGridChild.IsFilterable = true;
            uGridChild.IsFilterable = mChildGridIsFilterable;
            //uGridChild.WindowZoomDelegate = ReturnSelectedData;
            //uGridChild.SetGridSelectionBehavior(true, false);
            uGridChild.SetGridSelectionBehavior(mChildGridFullRowSelect, mChildMultiRowSelect);
            //uGridChild.FieldLayoutResourceString = "RecvAcctAgingDetail";
            uGridChild.FieldLayoutResourceString = mChildFieldLayoutResourceString;

            GridCollection.Add(uGridChild);

            //Load BusObject with Data
            MainObject.LoadData();
             
            //Load ScreenBase context
            this.Load(MainObject);
        }


        private void Grid_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            if (uGridParent.xGrid.SelectedItems.Records.Count > 0)
            {
                DataRecord r = default(DataRecord);
                r = (Infragistics.Windows.DataPresenter.DataRecord)(uGridParent.xGrid.SelectedItems.Records[0]);
                uGridParent.FilterGrid(mParentGridDefaultFilterColumnName, r.Cells[mParentGridDefaultFilterColumnName].Value.ToString());
                if (r.Cells[mParentChildGridFilterONColumnName].Value.ToString().ToUpper() != "ALL")
                {
                    //uGridParent.FilterGrid("product_code", r.Cells["product_code"].Value.ToString());
                    uGridChild.FilterGrid(mParentChildGridFilterONColumnName, r.Cells[mParentChildGridFilterONColumnName].Value.ToString());
                }
                else
                {
                    uGridChild.FilterGrid(mParentChildGridFilterONColumnName, "");
                }

                //uGridChild.FillGrid()
            }

        }

        public void ReturnSelectedData()
        {
            //Zoom Functionality

        }
    }
}
