using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RazerBase
{
    public class DialogBase : System.Windows.Window
    {
        #region "Properties"

        private string mScreenName;
        internal string ScreenName
        {
            get { return mScreenName; }
            set { mScreenName = value; }
        }

        //Set to true if the window is used only for lookups
        private bool mIsLookupWindow;
        public bool LookupWindow
        {
            get { return mIsLookupWindow; }
            set { mIsLookupWindow = value; }
        }

        //Used to set the main table name.  This is the value of the table pulled from the base business object to bind to the screen
        private string mMainTableName;
        public string MainTableName
        {
            get { return mMainTableName; }
            set { mMainTableName = value; }
        }

        //Used to store the grids on an individual page
        //If a tab folder only use for the grids on the main page
        //Each tab user control should have its own grid collection
        private List<ucBaseGrid> mGridCollection = new List<ucBaseGrid>();
        public List<ucBaseGrid> GridCollection
        {
            get { return mGridCollection; }
            set { mGridCollection = value; }
        }

        //Used to store ref of bus obj being used
        private cBaseBusObject mCurrentBusObj;
        public cBaseBusObject CurrentBusObj
        {
            get { return mCurrentBusObj; }
            set { mCurrentBusObj = value; }
        }

        #endregion


        public DialogBase()
        {
            mIsLookupWindow = false;
        }

        protected void Load()
        {
            //check for nulls
            if (CurrentBusObj == null) return;

            this.CurrentBusObj.LoadData();
            doConstrOps(CurrentBusObj);
        }

        protected void LoadExistingData()
        {
            //check for nulls
            if (CurrentBusObj == null) return;

            doConstrOps(CurrentBusObj);
        }

        protected void Load(cBaseBusObject CurrentBusObj)
        {
            //check for nulls
            //if (CurrentBusObj == null) return;

            CurrentBusObj.LoadData();
            doConstrOps(CurrentBusObj);
        }

        private void doConstrOps(cBaseBusObject CurrentBusObj)
        {
            //Set the data context of the tab to be the same as the main window
            if (MainTableName != null)
            {
                this.DataContext = CurrentBusObj.ObjectData.Tables[MainTableName].DefaultView;
            }

            //If a grid is on the current folder or tab then loop through 
            foreach (ucBaseGrid G in GridCollection)
            {
                G.LoadGrid(CurrentBusObj, MainTableName);
            }
        }


    }
}
