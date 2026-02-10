using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;



namespace RazerBase
{
    public class cParms
    {

        private DataTable mParmList;
        public DataTable ParmList
        {
            get { return mParmList; }
        }


        public void AddParm(string ParmName, string ParmValue)
        {
            //Adds stored procedure parameters to the parameter list
            //This list is shared among all procedures so it needs to be cleared and reloaded 
            //to be used if the parameters or parameters lists are different

            mParmList.Rows.Add(ParmName, ParmValue);
        }

        public void AddParm(string ParmName, object ParmValue)
        {
            //Adds stored procedure parameters to the parameter list
            //This list is shared among all procedures so it needs to be cleared and reloaded 
            //to be used if the parameters or parameters lists are different

            mParmList.Rows.Add(ParmName, ParmValue);
        }

        /// <summary>
        /// Allows existing parm to be changed by passing in the field description and the value to change to
        /// </summary>
        /// <param name="parmName"></param>
        /// <param name="parmValue"></param>
        public void UpdateParmValue(string parmName, string parmValue)
        {
            bool HasData = false;
            EnumerableRowCollection<DataRow> Parms = from ParmTable in this.ParmList.AsEnumerable()
                                                           where ParmTable.Field<string>("ParmName") == parmName 
                                                           select ParmTable ;
            
  
            foreach (DataRow r in Parms)
            {
                HasData = true;
                r["Value"] = parmValue;
            }

            if (!HasData)
            {
                AddParm(parmName, parmValue);
            }


        }

        /// <summary>
        /// Allows existing parm to be changed by passing in the field description and the value to change to
        /// </summary>
        /// <param name="parmName"></param>
        /// <param name="parmValue"></param>
        public void UpdateParmValue(string parmName, object parmValue)
        {
            bool HasData = false;
            EnumerableRowCollection<DataRow> Parms = from ParmTable in this.ParmList.AsEnumerable()
                                                     where ParmTable.Field<string>("ParmName") == parmName
                                                     select ParmTable;

            foreach (DataRow r in Parms)
            {
                HasData = true;
                r["Value"] = parmValue;
            }

            if (!HasData)
            {
                AddParm(parmName, parmValue);
            }

        }


        public string GetParm(string parmName)
        {
            string returnValue = "";
            EnumerableRowCollection<DataRow> Parms = from ParmTable in this.ParmList.AsEnumerable()
                                                     where ParmTable.Field<string>("ParmName") == parmName
                                                     select ParmTable;

            foreach (DataRow r in Parms)
            {
                returnValue  = r["Value"].ToString();
                break;
            }

            return returnValue;
        }




        public void ClearParms()
        {
            //Clears out the parameter list
            mParmList = new DataTable("Parms");
            mParmList.Columns.Add("ParmName");
            mParmList.Columns.Add("Value");
        }

        public cParms()
        {
            ClearParms();
        }
    }
}
