using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cDataRazer;
using System.Data;
using System.Configuration;

namespace RazerWS
{
    public class RazerSvc : IRazerService
    {
        private cLASERDB SQLCA = new cLASERDB();
        private cTransaction.cTransaction tran;

        public DataSet GetObject(string BusinessObjectName, DataSet Parms, DataSet SecurityRole)
        {
            DataSet ds = null;
            if (!string.IsNullOrEmpty(BusinessObjectName))
            {
                if (InitDB())
                {
                    var mydata = new cLASERBaseTable(ref SQLCA, "");
                    tran = new cTransaction.cTransaction(SQLCA);
                    if (Parms.Tables.Count == 0)
                    {
                        Parms = null;
                    }
                    ds = tran.Retreive(BusinessObjectName, SecurityRole, Parms);
                }
            }

            return ds;
        }

        public DataSet NewObject(string BusinessObjectName, DataSet Parms, DataSet SecurityRole)
        {
            DataSet ds = null;
            if (!string.IsNullOrEmpty(BusinessObjectName))
            {
                if (InitDB())
                {
                    var mydata = new cLASERBaseTable(ref SQLCA);
                    tran = new cTransaction.cTransaction(SQLCA);
                    if (Parms.Tables.Count == 0)
                    {
                        Parms = null;
                    }
                    ds = tran.NewObject(BusinessObjectName, SecurityRole, Parms);
                }
            }
            return ds;
        }

        public DataSet GetTableObject(string BusinessObjectName, DataSet Parms, string TableName, DataSet SecurityRole)
        {
            DataSet ds = null;
            if (!string.IsNullOrEmpty(BusinessObjectName))
            {
                if (InitDB())
                {
                    var mydata = new cLASERBaseTable(ref SQLCA, "");
                    tran = new cTransaction.cTransaction(SQLCA);
                    if (Parms.Tables.Count == 0)
                    {
                        Parms = null;
                    }
                    ds = tran.Retreive(BusinessObjectName, SecurityRole, Parms, TableName);
                }
            }

            return ds;
        }

        public DataSet SaveObject(string BusinessObjectName, DataSet ObjectData, DataSet SecurityRole)
        {
            if (InitDB())
            {
                tran = new cTransaction.cTransaction(SQLCA);
                ObjectData = tran.Update(ObjectData);
            }

            return ObjectData;
        }

        public DataSet SaveTableObject(string BusinessObjectName, DataSet ObjectData, string TableName, DataSet SecurityRole)
        {
            if (InitDB())
            {
                tran = new cTransaction.cTransaction(SQLCA);
                ObjectData = tran.Update(ObjectData, TableName);
            }

            return ObjectData;
        }

        private bool InitDB()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            //Get principal username
            var p = System.Threading.Thread.CurrentPrincipal as System.Security.Principal.WindowsPrincipal;
            string UserName = "";
            try { UserName = p.Identity.Name.Split('\\')[1]; }
            catch { UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1]; }

            return SQLCA.InitDB(connStr, UserName);
        }


        public string GetDatabaseServerName()
        {
            string serverName = "";

            if (InitDB())
            {
                var mydata = new cLASERBaseTable(ref SQLCA);
                if (mydata.SqlStringPopDt("select @@SERVERNAME as ServerName"))
                {
                    foreach (DataRow row in mydata.GetDataTable.Rows)
                    {
                        serverName = row["ServerName"].ToString();
                    }
                }

                serverName += " " + SQLCA.Database;
            }

            return serverName.Trim();
        }
    }
}
