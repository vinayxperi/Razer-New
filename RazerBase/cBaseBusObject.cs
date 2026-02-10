

#region using statements

using System;
using System.ComponentModel;
using System.Data;
using System.Text;

#endregion

namespace RazerBase
{

    /// <summary>
    /// This is the business object base tied to robject 
    /// </summary>
    public class cBaseBusObject : INotifyPropertyChanged
    {

        private string mBusObjectName;
        private DataSet mObjectData;
        private cParms mParms = new cParms();

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public cBaseBusObject()
        {

        }

        /// <summary>
        /// Overloaded constructor accepting BusObjName
        /// </summary>
        /// <param name="strBusObjName"></param>
        public cBaseBusObject(string strBusObjName)
        {
            mBusObjectName = strBusObjName;
        }

        #region Methods

        /// <summary>
        /// This method returns a DataTable by its name;
        /// </summary>
        public object GetTable(string TableName)
        {
            // initial value
            object table = null;

            try
            {
                // if the ObjectData exists
                if ((this.HasObjectData) && (this.ObjectData.Tables[TableName] != null))
                {
                    // set the return value
                    table = this.ObjectData.Tables[TableName];
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }

            // return value
            return table;
        }

        public void UpdateParms()
        {
            if (mObjectData != null && mObjectData.Tables.IndexOf("ParmTable") != -1)
            {
                DataTable parmsTable = mObjectData.Tables["ParmTable"];
                if (parmsTable.GetChanges(DataRowState.Modified) != null && parmsTable.GetChanges(DataRowState.Modified).Rows.Count != 0)
                {
                    foreach (DataRow modifiedRow in parmsTable.GetChanges(DataRowState.Modified).Rows)
                    {
                        EnumerableRowCollection<DataRow> ParmListRows = from row in this.Parms.ParmList.AsEnumerable()
                                                                        where row.Field<string>("ParmName") == modifiedRow["parmName"].ToString()
                                                                        select row;

                        foreach (DataRow parmListRow in ParmListRows)
                        {
                            parmListRow["Value"] = modifiedRow["parmValue"];
                        }
                    }
                }
            }
        }

        public cDsParms SetupParms()
        {
            cDsParms SvcParms = new cDsParms();

            for (int i = 0; i < this.Parms.ParmList.Rows.Count; i++)
            {
                SvcParms.add(this.Parms.ParmList.Rows[i].ItemArray[1].ToString(), this.Parms.ParmList.Rows[i].ItemArray[0].ToString());
            }
            return SvcParms;
        }

        public void LoadTable(string TableName)
        {
            this.LoadData(TableName);
        }

        /// <summary>
        /// This method loads the data if we have Parms
        /// </summary>
        public void LoadData(string TableName = null)
        {
            if (Parms == null)
            {
                return;
            }

            UpdateParms();

            cDsParms SvcParms = SetupParms();

            DataSet dsSecurity = null;

            string ErrorMessage = null;
            DataSet returnSet = null;
            bool loaded = false;

            if (TableName == null)
            {
                returnSet = cGlobals.LCService.GetObject(this.BusObjectName, SvcParms.GetDS(), dsSecurity);
                loaded = CheckForTransactionErrors(returnSet, out ErrorMessage);
                if (loaded)
                {
                    this.ObjectData = returnSet;
                }
                else
                {
                    if (ErrorMessage == null)
                        Messages.ShowError("ReturnSet from load new business object was null");
                    else
                        Messages.ShowError(ErrorMessage);
                }
            }
            else
            {
                returnSet = cGlobals.LCService.GetTableObject(this.BusObjectName, SvcParms.GetDS(), TableName, dsSecurity);
                loaded = CheckForTransactionErrors(returnSet, out ErrorMessage);
                if (loaded)
                {
                    if (returnSet.Tables.IndexOf(TableName) != -1)
                    {
                        DataTable dt = returnSet.Tables[TableName];

                        //DWR-Added 1/23/13-To initialize object data if null
                        if (this.ObjectData == null)
                        {
                            this.ObjectData=new DataSet();
                        }

                        if (this.ObjectData.Tables.IndexOf(TableName) == -1)
                        {
                            DataTable newdt = dt.Copy();
                            this.ObjectData.Tables.Add(newdt);
                        }
                        else
                        {
                            this.ObjectData.Tables[TableName].Rows.Clear();
                            this.ObjectData.Tables[TableName].Merge(dt);
                        }
                    }
                }
                else
                {
                    if (ErrorMessage == null)
                        Messages.ShowError("ReturnSet from load new business object was null");
                    else
                        Messages.ShowError(ErrorMessage);
                }
            }

            if (!loaded)
            {
                this.ObjectData = returnSet;
            }
        }

        /// <summary>
        /// This method is called when using LoadTable logic. Usually on a screenload some initial table values are set with an id of zero so no records are
        /// populated. However, when the id is available and you need to get the data you will have to change the parm of 0 to the new value.  This method does this.
        /// </summary>
        /// <param name="passedParmName"></param>
        /// <param name="passedParmValue"></param>
        public void changeParm(string passedParmName, string passedParmValue)
        {
            EnumerableRowCollection<DataRow> FilterParms = from parmDataTable in this.ObjectData.Tables["ParmTable"].AsEnumerable()
                                                           where parmDataTable.Field<string>("parmName") == passedParmName
                                                           select parmDataTable;

            foreach (DataRow row in FilterParms)
            {
                row["parmValue"] = passedParmValue;
            }
        }

        internal void LoadNewBusinessObjectInstance()
        {
            cDsParms SvcParms = SetupParms();

            DataSet dsSecurity = null;
            DataSet returnSet = cGlobals.LCService.NewObject(this.BusObjectName, SvcParms.GetDS(), dsSecurity);

            string ErrorMessage = null;
            bool loaded = CheckForTransactionErrors(returnSet, out ErrorMessage);
            if (loaded)
            {
                this.ObjectData = returnSet;
            }
            else
            {
                if (ErrorMessage == null)
                    Messages.ShowError("ReturnSet from load new business object was null");
                else
                    Messages.ShowError(ErrorMessage);
            }

            if (!loaded)
            {
                this.ObjectData = returnSet;
            }
            
        }

        /// <summary>
        /// This method saves the current ObjectData DataSet
        /// </summary>
        public bool Save()
        {
            // initial value
            bool saved = false;

            // mock security object for now
            DataSet dsSecurity = null;

            // execute the save
            DataSet returnSet = cGlobals.LCService.SaveObject(this.BusObjectName, this.ObjectData, dsSecurity);

            // as a test for now
            string ErrorMessage = null;
            saved = CheckForTransactionErrors(returnSet, out ErrorMessage);

            if (saved)
            {
                this.ObjectData = returnSet;
            }
            else
            {
                if (ErrorMessage==null)
                    Messages.ShowError("ReturnSet from save was null");
                else
                    Messages.ShowError(ErrorMessage);
            }

            // return value
            return saved;
        }

        private static bool CheckForTransactionErrors(DataSet returnSet, out string ErrorMessage)
        {
            bool successful = (returnSet != null && returnSet.Tables.IndexOf("cTransactionErrors") == -1);

            if (!successful && returnSet != null && returnSet.Tables.IndexOf("cTransactionErrors") != -1)
            {
                StringBuilder errorMessage = new StringBuilder();
                foreach (DataRow row in returnSet.Tables["cTransactionErrors"].Rows)
                {
                    errorMessage.AppendLine("--Begin Error------------------");
                    errorMessage.AppendLine(string.Format("Error Type: {0}", row["ErrorType"].ToString()));
                    errorMessage.AppendLine(string.Format("Error Message: {0}", row["ErrorMessage"].ToString()));
                    errorMessage.AppendLine(string.Format("Exception: {0}", row["Exception"].ToString()));
                    errorMessage.AppendLine(string.Format("Context: {0}", row["Context"].ToString()));
                    errorMessage.AppendLine("--End Error--------------------");
                    errorMessage.AppendLine("");
                }

                ErrorMessage = errorMessage.ToString();
            }
            else
            {
                ErrorMessage = null;
            }

            return successful;
        }

        public void DeleteAllTableRows(string dtName)
        {
            if (HasObjectData && ObjectData.Tables[dtName] != null && ObjectData.Tables[dtName].Rows.Count > 0)
            {
                for (int i=0; i < ObjectData.Tables[dtName].Rows.Count ;i++)
                    ObjectData.Tables[dtName].Rows[i].Delete();
            }
        }

        public bool SaveTable(string TableName)
        {

            // initial value
            bool saved = false;

            // mock security object for now
            DataSet dsSecurity = null;

            // execute the save
            DataSet returnSet = cGlobals.LCService.SaveTableObject(this.BusObjectName, this.ObjectData, TableName, dsSecurity);

            // as a test for now
            string ErrorMessage = null;
            saved = CheckForTransactionErrors(returnSet, out ErrorMessage);

            if (saved)
            {
                //this.ObjectData = returnSet;
                if (returnSet.Tables.IndexOf(TableName) != -1)
                {
                    DataTable dt = returnSet.Tables[TableName];
                    if (this.ObjectData.Tables.IndexOf(TableName) == -1)
                    {
                        DataTable newdt = dt.Copy();
                        this.ObjectData.Tables.Add(newdt);
                    }
                    else
                    {
                        this.ObjectData.Tables[TableName].Rows.Clear();
                        this.ObjectData.Tables[TableName].Merge(dt);
                    }
                }
 
            }
            else
            {
                if (ErrorMessage == null)
                    Messages.ShowError("ReturnSet from save was null");
                else
                    Messages.ShowError(ErrorMessage);
            }

            // return value
            return saved;
        }

        /// <summary>
        /// This method is used to return the name of this object so when debugging you get some info
        /// </summary>
        public override string ToString()
        {
            // initial value
            string name = "cBaseBusObject";

            // if the name exists
            if (!String.IsNullOrEmpty(this.BusObjectName))
            {
                // set the return value
                name = this.BusObjectName;
            }

            // return the name of this object;
            return name;
        }

        protected void SignalPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Method to determine if any of the changeable tables have changed since loaded
        /// Use this instead of the dataset haschanges method as that method returns true if non changeable tables are changed
        /// like the parms table
        /// </summary>
        /// <returns></returns>
        public bool DataChanged()
        {
            //If the business object data has changeable tables then loop through each table and see if any changes have been made
            if (ObjectData != null && HasObjectData && ObjectData.Tables["changeable_objects"] != null && ObjectData.Tables["changeable_objects"].Rows.Count > 0)
            {
                foreach (DataRow r in ObjectData.Tables["changeable_objects"].Rows)
                {
                    if (ObjectData.Tables[r["table_name"].ToString()].GetChanges() != null)
                        return true;
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// This property gets or sets the name of this object.
        /// </summary>
        public string BusObjectName
        {
            get { return mBusObjectName; }
            set { mBusObjectName = value; }
        }

        /// <summary>
        /// This property returns true if this object has an 'ObjectData'.
        /// </summary>
        public bool HasObjectData
        {
            get
            {
                // initial value
                bool hasObjectData = (this.ObjectData != null);

                // return value
                return hasObjectData;
            }
        }

        /// <summary>
        /// This property gets or sets the Data for this object.
        /// </summary>
        public DataSet ObjectData
        {
            get { return mObjectData; }
            set
            {
                mObjectData = value;
                SignalPropertyChanged("ObjectData");
            }
        }

        /// <summary>
        /// This property gets or sets the Parms for this object.
        /// </summary>
        public cParms Parms
        {
            get { return mParms; }
            set { mParms = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
