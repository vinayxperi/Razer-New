using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RazerBase
{
    public class cDsParms
    {
        private DataSet iDS;

        /// <summary>
        /// Constructor for cDsParms
        /// 
        /// </summary>
        /// <remarks>Creates "ParmTable" DataTable and adds it to the member dataset</remarks>
        public cDsParms()
        {
            iDS = new DataSet();
            DataTable dt = new DataTable("ParmTable");
            dt.Columns.Add("parmName", typeof(string));
            dt.Columns.Add("parmValue", typeof(string));
            iDS.Tables.Add(dt.Clone());
        }

        /// <summary>
        /// Adds a parameter and value to the collection
        /// </summary>
        /// <param name="parmValue">String: Parm value as a string</param>
        /// <param name="parmName">String: Parm name; i.e. "@ParmName"</param>
        /// <remarks></remarks>
        public void add(string parmValue, string parmName)
        {
            DataRow row = iDS.Tables["ParmTable"].NewRow();
            row["parmName"] = parmName;
            row["parmValue"] = parmValue;
            iDS.Tables["ParmTable"].Rows.Add(row);
        }

        /// <summary>
        /// GetDS function returns the dataset of parameters and values
        /// </summary>
        /// <returns>DataSet</returns>
        /// <remarks>Used to to pass parameters as a collection to the Transaction web service</remarks>
        public DataSet GetDS()
        {
            return iDS;
        }
    }
}
