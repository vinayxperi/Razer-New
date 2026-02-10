using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cDataRazer;

namespace cTransaction
{
    /// <summary>
    /// The cTransaction class is a base object for RAPTOR Web Services
    /// that will provide base functionality to higher level Business objects
    /// such as Adjustments
    /// </summary>
    /// <remarks>Brian Dyer, TV Guide --October 23, 2007</remarks>
    [Serializable()]
    public class cTransaction
    {

        # region Properties & Members

        /// <summary>
        /// iSQLCA is the database connection instance
        /// </summary>
        private cLASERDB iSQLCA;

        /// <summary>
        /// iMyData is the database interface instance
        /// </summary>
        private cLASERBaseTable iMyData;

        /// <summary>
        /// iTables is used in the passing of parameters to cTransaction
        /// </summary>
        private List<cTable> iTables;

        /// <summary>
        /// blnError shows that datatable "cTransactionErrors" exists in the dataset and is populated.
        /// </summary>
        public bool blnError { get; private set; }

        private DataTable dtErrorTable;

        private string UserName;

        /// <summary>
        /// Constructor for cTransaction class
        /// </summary>
        /// <param name="SQLCA">Base Connection Object</param>
        /// <remarks></remarks>
        public cTransaction(cLASERDB SQLCA)
        {
            if ((SQLCA != null))
            {
                iSQLCA = SQLCA;
                iMyData = new cLASERBaseTable(ref iSQLCA, "");
                var connStrProperties = new List<string>(iSQLCA.Conn.ConnectionString.Split(';'));
                this.UserName = (from x in connStrProperties
                                 where x.Contains("Workstation ID")
                                 select new
                                 {
                                     _UserName = x.Split('=')[1]
                                 }).Single()._UserName;
            }
        }

        /// <summary>
        /// TableCount returns the number tables in the business object
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public int TableCount()
        {
            return iTables.Count();
        }

        # endregion

        # region Error Logging

        /// <summary>
        /// Logs error information to the cTransactionErrors datatable
        /// </summary>
        /// <param name="ErrorType"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="ex"></param>
        /// <param name="Context"></param>
        private void LogError(string ErrorType, string ErrorMessage, Exception ex, string Context)
        {
            //Check if cTransactionErrors exists
            blnError = true;
            if (dtErrorTable == null)
            {
                dtErrorTable = BuildErrorTable();
            }

            DataRow row = dtErrorTable.NewRow();
            row["ErrorType"] = ErrorType;
            row["ErrorMessage"] = ErrorMessage;
            row["Exception"] = ex;
            row["Context"] = Context;

            dtErrorTable.Rows.Add(row);
        }

        /// <summary>
        /// Builds error datatable, cTransactionErrors
        /// </summary>
        /// <returns></returns>
        private DataTable BuildErrorTable()
        {
            DataTable dt = new DataTable("cTransactionErrors");
            dt.Columns.Add("ErrorType", typeof(string));
            dt.Columns.Add("ErrorMessage", typeof(string));
            dt.Columns.Add("Exception", typeof(object));
            dt.Columns.Add("Context", typeof(string));
            return dt;
        }

        # endregion

        #region " Retreive Function & resources "

        public DataSet NewObject(string BusinessObject, DataSet SecurityRole, DataSet Parms = null)
        {
            DataSet ds = null;
            ds = Retreive(BusinessObject, SecurityRole, Parms);

            foreach (DataTable dsTable in ds.Tables)
            {
                var table_name = dsTable.TableName;
                //retreive list of store procedures using usp_sel_robject_table_sp
                iMyData.Add_SP_Parm(BusinessObject, "@robject_name");
                iMyData.Add_SP_Parm(table_name, "@TableName");
                if (iMyData.SqlSpPopDt("usp_sel_robject_table_sp"))
                {
                    foreach (DataRow proc in iMyData.GetDataTable.Rows)
                    {
                        if (Convert.ToString(proc["trans_type"]).ToUpper().Trim() == "INS" && Convert.ToInt32(proc["update_order"]) == 0)
                        {
                            DataRow newRow = dsTable.NewRow();
                            foreach (DataColumn col in dsTable.Columns)
                            {
                                var xn = col.ColumnName;
                                var x = col.AllowDBNull;
                            }
                            dsTable.Rows.Add(newRow);
                            break;
                        }
                    }
                }
            }

            return ds;
        }

        /// <summary>
        /// The Retreive function creates an object of that has the CRUD(Create, Read, Update and Delete)
        /// stored procedures for each table associated with a Business  object and returns a populated dataset
        /// with keys, constraints and relationships.
        /// </summary>
        /// <param name="BusinessObject">String: Business  Object name</param>
        /// <param name="Parms">Optional: DataSet</param>
        /// <returns>DataSet</returns>
        /// <remarks>This is not the select function</remarks>
        public DataSet Retreive(string BusinessObject, DataSet SecurityRole, DataSet Parms = null, String TableMember = null)
        {
            DataSet ds = null;
            List<cSpParm> ConvertedParms = ProcessParms(Parms);

            //Check to for valid name
            if (!string.IsNullOrEmpty(BusinessObject))
            {
                //Retreive Business  Object data from database
                iMyData.Add_SP_Parm(BusinessObject, "@robject_name");
                if (iMyData.SqlSpPopDt("usp_sel_robject_tables"))
                {
                    ds = BuildDS(iMyData.GetDataTable, BusinessObject, ConvertedParms, TableMember);
                    DataTable dt = new DataTable("BuisnessObjectName");
                    dt.Columns.Add(new DataColumn("BuisnessObjectName", typeof(string)));
                    DataRow row = dt.NewRow();
                    row["BuisnessObjectName"] = BusinessObject;
                    dt.Rows.Add(row);

                    ds.Tables.Add(dt);
                    if ((Parms != null) && Parms.Tables["ParmTable"].Rows.Count != 0)
                    {
                        ds.Tables.Add(Parms.Tables["ParmTable"].Clone());
                        foreach (DataRow ParmRow in Parms.Tables["ParmTable"].Rows)
                        {
                            ds.Tables["ParmTable"].Rows.Add(ParmRow.ItemArray);
                        }
                    }

                    //add user security to the dataset
                    ds.Tables.Add(GetUserSecurityDataTable(BusinessObject));
                    //DWR-Added 2/21 to get list of robject tables that can be modified
                    ds.Tables.Add(GetChangeableTables(BusinessObject));

                }
                else
                {
                    LogError("Database", iMyData.dberror, null, string.Format("Business Object, {0}, parameter did not return results. Retreive function in cTransaction", BusinessObject));
                }

            }
            else
            {
                LogError("Database", iMyData.dberror, null, string.Format("Business Object, {0}, parameter was null or empty. Retreive function in cTransaction", BusinessObject));
            }

            if (blnError) //check for any recorded Errors and add them to the dataset ds
            {
                if (ds.Tables.Contains("cTransactionErrors"))
                {
                    ds.Tables.Remove("cTransactionErrors");
                }
                ds.Tables.Add(dtErrorTable.Copy());
            }

            ds.AcceptChanges();

            return ds;
        }

        private static List<cSpParm> ProcessParms(DataSet Parms)
        {
            List<cSpParm> ConvertedParms = new List<cSpParm>();

            if ((Parms != null))
            {
                foreach (DataRow row in Parms.Tables["ParmTable"].Rows)
                {
                    ConvertedParms.Add(new cSpParm(Convert.ToString(row["parmName"]), Convert.ToString(row["parmValue"]), false));
                }
            }
            return ConvertedParms;
        }

        /// <summary>
        /// Builds Security Table based on the User and Business Object
        /// </summary>
        /// <param name="BusinessObject"></param>
        /// <returns></returns>
        private DataTable GetUserSecurityDataTable(string BusinessObject)
        {
            DataTable dt = null;
            iMyData.Add_SP_Parm(UserName, "@user_id");
            iMyData.Add_SP_Parm(BusinessObject, "@Robject_name");
            if (iMyData.SqlSpPopDt("usp_sel_security_perm_by_object_razer", false))
            {
                dt = iMyData.GetDataTable;
            }

            //List<string> TableNames = (from x in dt.AsEnumerable()
            //                           select x.Field<string>("Table_name")).Distinct().ToList();

            //foreach (string TableName in TableNames)
            //{

            //}
            dt.TableName = "UserPermissions";
            return dt;
        }

        /// <summary>
        /// Method returns the changeable tables in the selected robject
        /// To be changeable a table must have an insert, update and/or delete procedure attached to it through the object
        /// </summary>
        /// <param name="BusinessObject">The name of the Robject</param>
        /// <returns></returns>
        private DataTable GetChangeableTables(string BusinessObject)
        {
            DataTable dt = null;
            iMyData.Add_SP_Parm(BusinessObject, "@robject_name");
            if (iMyData.SqlSpPopDt("usp_sel_changeable_robjects", false))
            {
                dt = iMyData.GetDataTable;
            }
            dt.TableName = "changeable_objects";
            return dt;
        }


        /// <summary>
        /// The BuildDS function builds the base object DataSet.
        /// </summary>
        /// <param name="dt">DataTable: Member tables of the Business  Object</param>
        /// <param name="BusinessObject">String: Business  Object name</param>
        /// <param name="UserParms">Optional Collection of cSpParm</param>
        /// <returns>DataSet</returns>
        /// <remarks>Called by Retreive function</remarks>
        private DataSet BuildDS(DataTable dt, string BusinessObject, List<cSpParm> UserParms = null, string TableMember = null)
        {
            DataSet ds = new DataSet();

            BuildObjectTableRef(dt, BusinessObject, TableMember);
            //Build the iTables collection with the member tables of the Business  Object

            //Iterate throug each table
            foreach (cTable table in iTables)
            {
                //Check to make sure there are 
                if ((table.SelectSpParms != null))
                {
                    //find the parm with the matching order
                    foreach (cSpParm parm in table.SelectSpParms)
                    {
                        //find the corresponding user parm
                        foreach (cSpParm userParm in UserParms)
                        {
                            if (userParm.Parameter == parm.Parameter)
                            {
                                GetParmObject(parm, userParm);
                                //add parm to iMyData for query
                            }
                        }
                    }
                }

                //add datatable to datset
                if (!string.IsNullOrEmpty(table.TableSelect) && iMyData.SqlSpPopDt(table.TableSelect, false))
                {
                    ds.Tables.Add(iMyData.GetDataTable);
                    ds.Tables[ds.Tables.Count - 1].TableName = table.TableName;

                    foreach (DataColumn col in ds.Tables[table.TableName].Columns)
                    {
                        if (!col.AllowDBNull)
                        {
                            SetDefault(col);
                        }
                    }
                }
                else
                {
                    LogError("Database", iMyData.dberror, null, string.Format("Business Object, {0}, error running 'table.TableSelect' on table {1} using {2}. BuildDS function in cTransaction", BusinessObject, table.TableName, table.TableSelect));
                }
            }

            //ds = BuildTableConstraints(ds);
            //Add Primary Keys, Foreign Keys and Unique Constraints

            //ds = BuildTableRelationShips(ds);
            //Add Table relationships to dataset

            return ds;
        }

        /// <summary>
        /// The BuildObjectTableRef function gets the CRUD stored procedures associated with each table
        /// and their parameters
        /// </summary>
        /// <param name="dt">DataTable: Member tables of the Business  Object</param>
        /// <param name="BusinessObject">String: Business  Object name</param>
        /// <remarks>Called by BuildDS function</remarks>
        private void BuildObjectTableRef(DataTable dt, string BusinessObject, string TableMember = null)
        {
            iTables = new List<cTable>();

            //Get each member table of the Business  Object
            foreach (DataRow row in dt.Rows)
            {
                if (TableMember == null || Convert.ToString(row["table_name"]) == TableMember)
                {
                    List<cSpParm> parmsTemp = new List<cSpParm>();
                    //This will become part of the iTables collection
                    cTable tableTemp = new cTable(Convert.ToString(row["table_name"]));
                    //Name the table in iTables Collection

                    //retreive list of store procedures using usp_sel_robject_table_sp
                    iMyData.Add_SP_Parm(BusinessObject, "@robject_name");
                    iMyData.Add_SP_Parm(Convert.ToString(row["table_name"]), "@TableName");
                    if (iMyData.SqlSpPopDt("usp_sel_robject_table_sp"))
                    {

                        foreach (DataRow proc in iMyData.GetDataTable.Rows)
                        {
                            //Get information about the Stored Procedures using usp_sel_sp_parms
                            iMyData.Add_SP_Parm(Convert.ToString(proc["sprocedure"]), "@objname");
                            if (iMyData.SqlSpPopDt("usp_sel_sp_parms", false))
                            {

                                foreach (DataRow parmRow in iMyData.GetDataTable.Rows)
                                {
                                    //Add each parm to the list parmsTemp
                                    parmsTemp.Add((cSpParm)new cSpParm(Convert.ToString(parmRow["Parameter_name"]), Convert.ToString(parmRow["Type"]), Convert.ToInt32(parmRow["Length"]), Convert.ToInt32(parmRow["Param_order"]), Convert.ToBoolean(parmRow["IsOutParam"])).Clone());
                                }
                            }
                            else
                            {
                                LogError("Database", iMyData.dberror, null, string.Format("Business Object, {0}, error getting information about the Stored Procedure, {2}, using 'usp_sel_sp_parms' on table {1}. BuildObjectTableRef function in cTransaction", BusinessObject, tableTemp, Convert.ToString(proc["sprocedure"])));
                            }

                            //Add sp name to table object based on sp type
                            switch ((Convert.ToString(proc["trans_type"]).ToUpper().Trim()))
                            {
                                case "SEL":
                                    tableTemp.NewSelect(Convert.ToString(proc["sprocedure"]), parmsTemp);
                                    break;
                                case "INS":
                                    tableTemp.NewInsert(Convert.ToString(proc["sprocedure"]), parmsTemp);
                                    break;
                                case "UPD":
                                    tableTemp.NewUpdate(Convert.ToString(proc["sprocedure"]), parmsTemp);
                                    break;
                                case "DEL":
                                    tableTemp.NewDelete(Convert.ToString(proc["sprocedure"]), parmsTemp);
                                    break;
                                default:
                                    LogError("RObject Definition", string.Format("Transaction type:  '{0}' is not valid for {1} and the {2} object.  INS, SEL, DEL, UPD are the valid options", Convert.ToString(proc["trans_type"]), Convert.ToString(proc["sprocedure"]), Convert.ToString(row["table_name"])), null, "BuildObjectTableRef Method in cTransaction");
                                    break;
                            }

                            //Clear parmsTemp for next procedure
                            parmsTemp = new List<cSpParm>();

                        }
                    }
                    else
                    {
                        LogError("Database", iMyData.dberror, null, string.Format("Business Object, {0}, error retreiving list of store procedures using 'usp_sel_robject_table_sp' on table {1}. BuildObjectTableRef function in cTransaction", BusinessObject, tableTemp));
                    }

                    iTables.Add((cTable)tableTemp.Clone());
                }
            }
        }

        /// <summary>
        /// The BuildTableConstraints adds PRIMARY KEY, FOREIGN KEY and UNIQUE column constraints
        /// to the tables in the dataset
        /// </summary>
        /// <param name="ds">DataSet: Built in BuildDS function</param>
        /// <returns>DataSet</returns>
        /// <remarks>Called by BuildDS function</remarks>
        private DataSet BuildTableConstraints(DataSet ds)
        {

            foreach (DataTable table in ds.Tables)
            {
                List<string> pkConstraints = null;
                List<string> fkConstraints = null;
                List<string> ukConstraints = null;

                //Get Table Constraints using usp_sel_constraints_name
                iMyData.Add_SP_Parm(table.TableName, "@TableName");
                if (iMyData.SqlSpPopDt("usp_sel_constraints_name"))
                {

                    foreach (DataRow constraint in iMyData.GetDataTable.Rows)
                    {
                        //Get constrained columns
                        iMyData.Add_SP_Parm(table.TableName, "@TableName");
                        iMyData.Add_SP_Parm(Convert.ToString(constraint["CONSTRAINT_NAME"]), "@CONSTRAINT_NAME");
                        if (iMyData.SqlSpPopDt("usp_sel_table_keys"))
                        {
                            foreach (DataRow constrainedCol in iMyData.GetDataTable.Rows)
                            {
                                switch ((Convert.ToString(constraint["CONSTRAINT_TYPE"]).ToUpper()))
                                {
                                    case "PRIMARY KEY":
                                        if (pkConstraints == null)
                                        {
                                            pkConstraints = new List<string>();
                                        }
                                        pkConstraints.Add(Convert.ToString(constrainedCol["COLUMN_NAME"]));
                                        break;
                                    case "FOREIGN KEY":
                                        if (fkConstraints == null)
                                        {
                                            fkConstraints = new List<string>();
                                        }
                                        fkConstraints.Add(Convert.ToString(constrainedCol["COLUMN_NAME"]));
                                        break;
                                    case "UNIQUE":
                                        if (ukConstraints == null)
                                        {
                                            ukConstraints = new List<string>();
                                        }
                                        ukConstraints.Add(Convert.ToString(constrainedCol["COLUMN_NAME"]));
                                        break;
                                    default:
                                        LogError("Gathering constraints for RObject", "Constraint type is not recognized:  '" + Convert.ToString(constraint["CONSTRAINT_TYPE"]) + "'", null, "BuildTableConstraints funcation in cTransaction");
                                        break;
                                }
                            }
                        }
                        else
                        {
                            LogError("Database", iMyData.dberror, null, string.Format("Error retreiving list of constrained columns using 'usp_sel_constraints_name' on table {0}. BuildTableConstraints function in cTransaction", table.TableName));
                        }

                        //determine constraint type & apply constraint
                        switch ((Convert.ToString(constraint["CONSTRAINT_TYPE"]).ToUpper()))
                        {
                            case "PRIMARY KEY":
                                List<DataColumn> pk = null;
                                //ArrayList of primary key columns
                                for (int i = 0; i <= pkConstraints.Count - 1; i++)
                                {
                                    if (pk == null)
                                    {
                                        pk = new List<DataColumn>();
                                    }
                                    pk.Add(ds.Tables[ds.Tables.IndexOf(table.TableName)].Columns[pkConstraints[i]]);
                                }

                                if ((pk != null))
                                {
                                    ds.Tables[ds.Tables.IndexOf(table.TableName)].PrimaryKey = pk.ToArray();
                                }
                                pkConstraints = null;
                                break;
                            case "FOREIGN KEY":
                                List<DataColumn> fk = null;
                                //ArrayList of foreign key columns
                                for (int i = 0; i <= fkConstraints.Count - 1; i++)
                                {
                                    if (fk == null)
                                    {
                                        fk = new List<DataColumn>();
                                    }
                                    fk.Add(ds.Tables[ds.Tables.IndexOf(table.TableName)].Columns[fkConstraints[i]]);
                                }

                                if ((fk != null))
                                {
                                    ds.Tables[ds.Tables.IndexOf(table.TableName)].Constraints.Add(Convert.ToString(constraint["CONSTRAINT_NAME"]), fk.ToArray(), false);
                                }
                                fkConstraints = null;
                                break;
                            case "UNIQUE":
                                List<DataColumn> uk = null;
                                //ArrayList of unique columns
                                for (int i = 0; i <= ukConstraints.Count - 1; i++)
                                {
                                    if (uk == null)
                                    {
                                        uk = new List<DataColumn>();
                                    }
                                    uk.Add(ds.Tables[ds.Tables.IndexOf(table.TableName)].Columns[ukConstraints[i]]);
                                }

                                if ((uk != null))
                                {
                                    ds.Tables[ds.Tables.IndexOf(table.TableName)].Constraints.Add(Convert.ToString(constraint["CONSTRAINT_NAME"]), uk.ToArray(), false);
                                }
                                ukConstraints = null;
                                break;
                            default:
                                LogError("Applying constraints for RObject", "Constraint type is not recognized:  '" + Convert.ToString(constraint["CONSTRAINT_TYPE"]) + "'", null, "BuildTableConstraints funcation in cTransaction");
                                break;
                        }

                    }
                }
                else
                {
                    LogError("Database", iMyData.dberror, null, string.Format("Error retreiving Table Constraints using 'usp_sel_constraints_name' on table {0}. BuildTableConstraints function in cTransaction", table.TableName));
                }
            }
            return ds;
        }

        /// <summary>
        /// The BuildTableRelationShips adds table relationships to the dataset for databinding purposes
        /// </summary>
        /// <param name="ds">DataSet: Built in BuildDS function</param>
        /// <returns>DataSet</returns>
        /// <remarks>Called by BuildDS function</remarks>
        private DataSet BuildTableRelationShips(DataSet ds)
        {


            if (ds.Tables.Count > 1)
            {
                //Build List of Relationships
                List<string> alRelationships = new List<string>();
                if (iMyData.SqlSpPopDt("usp_sel_relationship_names"))
                {
                    foreach (DataRow relationship in iMyData.GetDataTable.Rows)
                    {
                        alRelationships.Add(Convert.ToString(relationship["CONSTRAINT_NAME"]));
                    }
                }
                else
                {
                    LogError("Database", iMyData.dberror, null, string.Format("Error retreiving List of Relationships using 'usp_sel_relationship_names'. BuildTableRelationShips function in cTransaction"));
                }

                //Build List of Table names
                List<string> alTables = new List<string>();
                foreach (DataTable table in ds.Tables)
                {
                    alTables.Add(table.TableName);
                }

                //Iterate through the relationships
                foreach (string relationshipName in alRelationships)
                {
                    iMyData.Add_SP_Parm(relationshipName, "@CONSTRAINT_NAME");
                    //Get the relationship data
                    if (iMyData.SqlSpPopDt("usp_sel_relationships"))
                    {
                        List<DataColumn> parentColumns = new List<DataColumn>();
                        List<DataColumn> childColumns = new List<DataColumn>();

                        foreach (DataRow relationship in iMyData.GetDataTable.Rows)
                        {
                            //Check to see if the relationship belongs to these tables
                            if (alTables.Contains(Convert.ToString(relationship["ParentTable"])) && alTables.Contains(Convert.ToString(relationship["ChildTable"])))
                            {
                                parentColumns.Add(ds.Tables[Convert.ToString(relationship["ParentTable"])].Columns[Convert.ToString(relationship["ParentColumn"])]);
                                childColumns.Add(ds.Tables[Convert.ToString(relationship["ChildTable"])].Columns[Convert.ToString(relationship["ChildColumn"])]);
                            }
                        }

                        //Only add the relationship if it is applicable to the dataset
                        if (!(parentColumns.Count == 0) && !(childColumns.Count == 0))
                        {
                            ds.Relations.Add(relationshipName, parentColumns.ToArray(), childColumns.ToArray());
                        }
                    }
                    else
                    {
                        LogError("Database", iMyData.dberror, null, string.Format("Error retreiving relationship data using 'usp_sel_relationships'.  Relationship Name: {0}. BuildTableRelationShips function in cTransaction", relationshipName));
                    }
                }

            }

            return ds;
        }

        /// <summary>
        /// Sets the default value for a column that doesn't allow nulls
        /// </summary>
        /// <param name="col">DataColumn</param>
        /// <remarks></remarks>
        private void SetDefault(DataColumn col)
        {
            switch ((col.DataType.ToString()))
            {
                case "System.String":
                    col.DefaultValue = "";
                    break;
                case "System.Byte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Decimal":
                case "System.Single":
                case "System.Double":
                    if (!col.AutoIncrement)
                    {
                        col.DefaultValue = 0;
                    }
                    break;
                case "System.Boolean":
                    col.DefaultValue = false;
                    break;
                case "System.DateTime":
                    col.DefaultValue = "1/1/1900";
                    break;
                default:
                    LogError("Applying default type for RObject", string.Format("Column: {0} with Type: {1} was unhandled", col.ColumnName, col.DataType.ToString()), null, "SetDefault funcation in cTransaction");
                    break;
            }
        }

        #endregion

        #region " Update Function & resources "

        /// <summary>
        /// The function update inserts new records, deletes delete-marked records
        /// and updates update-marked records in the dataset and applies the changes
        /// to the database.  Then returns a refreshed copy of the data from the database.
        /// </summary>
        /// <param name="ds">DataSet: users working copy</param>
        /// <returns>DataSet: refreshed/updated copy</returns>
        /// <remarks></remarks>
        public DataSet Update(DataSet ds, string TableMember = null)
        {

            //Get original set of Parms from dataset ds if they exist
            DataSet Parms = null;
            if (!(ds.Tables.IndexOf("ParmTable") == -1))
            {
                Parms = new DataSet();
                Parms.Tables.Add(ds.Tables["ParmTable"].Copy());
            }

            DataRow rowBO = ds.Tables["BuisnessObjectName"].Rows[0];
            //Get the buisness object name from dataset ds

            //this will populate iTables which contains the CRUD defined for each table in the buisness object
            Retreive(Convert.ToString(rowBO["BuisnessObjectName"]), new DataSet(), Parms, TableMember);

            iMyData.BeginTransaction();
            bool Continue = true;

            //Insert new rows
            if ((ds.GetChanges(DataRowState.Added) != null))
            {
                var InsertableTableList = new Dictionary<string, int>();

                //Get the insert order from the database
                var sSQL = string.Format("SELECT b.table_name, b.update_order FROM robject AS a INNER JOIN robject_detail AS b ON a.robject_id = b.robject_id WHERE (a.robject_name = '{0}') and (b.trans_type = 'INS') order by update_order", rowBO["BuisnessObjectName"].ToString());
                if (iMyData.SqlStringPopDt(sSQL))
                {
                    foreach (DataRow row in iMyData.GetDataTable.Rows)
                    {
                        var TableName = row["table_name"].ToString();
                        var update_order = Convert.ToInt32(row["update_order"]);
                        InsertableTableList.Add(TableName, update_order);
                    }
                }

                var InsertedTableList = new List<string>();

                //Go through each table in the insert list
                foreach (var TableOrderPair in InsertableTableList)
                {
                    if (!Continue) { break; }

                    if ((TableMember == null || TableMember == TableOrderPair.Key) && ds.Tables.IndexOf(TableOrderPair.Key) != -1)
                    {
                        DataTable table = ds.Tables[TableOrderPair.Key];

                        if (table.GetChanges(DataRowState.Added) != null)
                        {
                            string localTableName = table.TableName;
                            InsertedTableList.Add(localTableName);

                            //Did this because using the iteration variable in query expressions can have unpredictable results
                            var iTable = (from x in iTables where x.TableName == localTableName select x).Single();
                            foreach (DataRow row in table.GetChanges(DataRowState.Added).Rows)
                            {
                                if (!Continue) { break; }
                                var ParameterAddedList = new List<string>();

                                foreach (DataColumn col in table.Columns)
                                {
                                    var localColumnName = "@" + col.ColumnName.ToLower();

                                    if (Parms != null && Parms.Tables["ParmTable"].Rows.Count > 0)
                                    {
                                        EnumerableRowCollection<DataRow> updateParmVal = from x in Parms.Tables["ParmTable"].AsEnumerable()
                                                                                         where x.Field<string>("parmName") == localColumnName
                                                                                         select x;

                                        foreach (DataRow updateValRow in updateParmVal)
                                        {
                                            row[col.ColumnName] = updateValRow["parmValue"];
                                        }
                                    }

                                    try
                                    {
                                        //Did this because using the iteration variable in query expressions can have unpredictable results
                                        var parm = (from x in iTable.InsertSpParms where x.Parameter.ToLower() == localColumnName select x).Single();
                                        GetParmObject(parm, new cSpParm(parm.Parameter, row[col.ColumnName], parm.IsOutPutParm));
                                        ParameterAddedList.Add(parm.Parameter);
                                        //add parm to iMyData for query
                                    }
                                    catch
                                    {
                                    }
                                    finally
                                    {
                                    }
                                }

                                //find any output parms in parmtable that aren't in the datarow
                                foreach (cSpParm parmInfo in iTable.InsertSpParms)
                                {
                                    if (parmInfo.IsOutPutParm && !ParameterAddedList.Contains(parmInfo.Parameter))
                                    {
                                        string localColumnName = parmInfo.Parameter.Replace("@", "");
                                        EnumerableRowCollection<DataRow> updateParmVal = null;
                                        object localParmValue = null;

                                        if (Parms != null)
                                        {
                                            updateParmVal = from x in Parms.Tables["ParmTable"].AsEnumerable()
                                                            where x.Field<string>("parmName") == parmInfo.Parameter
                                                            select x;

                                            foreach (DataRow updateValRow in updateParmVal)
                                            {
                                                localParmValue = updateValRow["parmValue"];
                                            }
                                        }

                                        if (localColumnName == "sp_error_halt") { localParmValue = 0; }
                                        try
                                        {
                                            //Did this because using the iteration variable in query expressions can have unpredictable results
                                            var parm = (from x in iTable.InsertSpParms where x.Parameter.ToLower() == parmInfo.Parameter select x).Single();
                                            GetParmObject(parm, new cSpParm(parm.Parameter, localParmValue, parm.IsOutPutParm));
                                            ParameterAddedList.Add(parm.Parameter);
                                            //add parm to iMyData for query
                                        }
                                        catch
                                        {
                                        }
                                        finally
                                        {
                                        }
                                    }
                                }
                                if (iMyData.NonQuerrySqlSp(iTable.TableInsert))
                                {
                                    row.AcceptChanges();

                                    if (Parms != null && Parms.Tables["ParmTable"].Rows.Count > 0)
                                    {
                                        foreach (var OutPutValue in iMyData.OutPutValues)
                                        {
                                            DataTable dtParmTable = Parms.Tables["ParmTable"];
                                            DataTable dtCurrentTable = ds.Tables[localTableName];

                                            var parmName = OutPutValue.Key;
                                            var parmValue = OutPutValue.Value;

                                            if (!(parmValue is System.DBNull))
                                            {
                                                //Check for any parmtable parameters that need to be updated
                                                EnumerableRowCollection<DataRow> ParmTableRows = from pt in dtParmTable.AsEnumerable()
                                                                                                 where pt.Field<string>("parmName") == parmName
                                                                                                 select pt;

                                                foreach (DataRow ParmTableRow in ParmTableRows)
                                                {
                                                    ParmTableRow["parmValue"] = OutPutValue.Value;
                                                }
                                            }

                                            if (parmName == "@sp_error_halt" && Convert.ToString(parmValue) == "1")
                                            {
                                                Continue = false;
                                            }

                                            //Get output parameters return values and dump them back into to tables
                                            foreach (var TableFromList in InsertableTableList)
                                            {
                                                if (TableMember == null || TableMember == TableFromList.Key)
                                                {
                                                    if (!InsertedTableList.Contains(TableFromList.Key)) //exclude tables that have already been udpated
                                                    {
                                                        DataTable CurrentDataTableToUpdate = ds.Tables[TableFromList.Key];

                                                        string fieldName = OutPutValue.Key.Substring(1);  //get column name
                                                        if (CurrentDataTableToUpdate != null && CurrentDataTableToUpdate.Rows.Count != 0 && CurrentDataTableToUpdate.Columns.IndexOf(fieldName) != -1) //check if column exists in table and make sure there are rows
                                                        {
                                                            EnumerableRowCollection<DataRow> DataSetRows = from dsrs in CurrentDataTableToUpdate.GetChanges(DataRowState.Added).AsEnumerable()
                                                                                                           select dsrs;

                                                            foreach (DataRow dsrsRow in DataSetRows)
                                                            {
                                                                dsrsRow[fieldName] = OutPutValue.Value;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else //Error
                                {
                                    if (string.IsNullOrEmpty(iTable.TableInsert))
                                    {
                                        LogError("ROBJECT", "", null, string.Format("Cannot insert on table '{0}' as no insert method is defined", table.TableName));
                                    }
                                    else
                                    {
                                        LogError("Database", iMyData.dberror, null, string.Format("Error inserting new record using '{0}'. Update function in cTransaction", iTable.TableInsert));
                                    }
                                }
                            }
                        } 
                    }
                }
            }

            //Delete deleted rows
            if ((ds.GetChanges(DataRowState.Deleted) != null) && Continue)
            {
                //Get Each table for the buisness object
                foreach (DataTable table in ds.Tables)
                {
                    if (TableMember == null || TableMember == table.TableName)
                    {
                        //The two table listed are not part of any buisness object Information or not null   
                        if (table.GetChanges(DataRowState.Deleted) != null && !(table.TableName == "cTransactionErrors" || table.TableName == "ParmTable" || table.TableName == "BuisnessObjectName"))
                        {
                            string localTableName = table.TableName;
                            //Did this because using the iteration variable in query expressions can have unpredictable results
                            var iTable = (from x in iTables where x.TableName == localTableName select x).Single();
                            foreach (DataRow row in table.GetChanges(DataRowState.Deleted).Rows)
                            {
                                foreach (DataColumn col in table.Columns)
                                {
                                    var localColumnName = "@" + col.ColumnName.ToLower();
                                    //Did this because using the iteration variable in query expressions can have unpredictable results
                                    try
                                    {
                                        var parm = (from x in iTable.DeleteSpParms where x.Parameter.ToLower() == localColumnName select x).Single();
                                        GetParmObject(parm, new cSpParm(parm.Parameter, row[col.ColumnName, DataRowVersion.Original], parm.IsOutPutParm));
                                        //add parm to iMyData for query
                                    }
                                    catch
                                    {
                                    }
                                    finally
                                    {
                                    }
                                }
                                if (!iMyData.NonQuerrySqlSp(iTable.TableDelete))
                                {
                                    if (string.IsNullOrEmpty(iTable.TableDelete))
                                    {
                                        LogError("ROBJECT", "", null, string.Format("Cannot delete from table '{0}' as no delete method is defined", table.TableName));
                                    }
                                    else
                                    {
                                        LogError("Database", iMyData.dberror, null, string.Format("Error deleting record using '{0}'. Update function in cTransaction", iTable.TableDelete));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Update changed rows
            if ((ds.GetChanges(DataRowState.Modified) != null) && Continue)
            {
                //Get Each table for the buisness object
                foreach (DataTable table in ds.Tables)
                {
                    if (TableMember == null || TableMember == table.TableName)
                    {
                        //The two table listed are not part of any buisness object Information or not null
                        if (table.GetChanges(DataRowState.Modified) != null && !(table.TableName == "cTransactionErrors" || table.TableName == "ParmTable" || table.TableName == "BuisnessObjectName"))
                        {
                            string localTableName = table.TableName;
                            //Did this because using the iteration variable in query expressions can have unpredictable results
                            var iTable = (from x in iTables where x.TableName == localTableName select x).Single();
                            foreach (DataRow row in table.GetChanges(DataRowState.Modified).Rows)
                            {
                                foreach (DataColumn col in table.Columns)
                                {
                                    var localColumnName = "@" + col.ColumnName.ToLower();
                                    //Did this because using the iteration variable in query expressions can have unpredictable results
                                    try
                                    {
                                        var parm = (from x in iTable.UpdateSpParms where x.Parameter.ToLower() == localColumnName select x).Single();
                                        GetParmObject(parm, new cSpParm(parm.Parameter, row[col.ColumnName], parm.IsOutPutParm));
                                        //add parm to iMyData for query
                                    }
                                    catch (Exception ex)
                                    {
                                        var errormsg = ex.Message;
                                    }
                                    finally
                                    {
                                    }
                                }
                                if (!iMyData.NonQuerrySqlSp(iTable.TableUpdate))
                                {
                                    if (string.IsNullOrEmpty(iTable.TableUpdate))
                                    {
                                        LogError("ROBJECT", "", null, string.Format("Cannot update table '{0}' as no update method is defined", table.TableName));
                                    }
                                    else
                                    {
                                        LogError("Database", iMyData.dberror, null, string.Format("Error updating record using '{0}'. Update function in cTransaction", iTable.TableUpdate));
                                    }
                                }
                            }
                        } 
                    }
                }
            }

            if (blnError)
            {
                iMyData.Rollback();
            }
            else
            {
                iMyData.Commit();
            }

            //repopulate ds
            ds = Retreive(Convert.ToString(rowBO["BuisnessObjectName"]), new DataSet(), Parms, TableMember);

            if (blnError) //check for any recorded Errors and add them to the dataset ds
            {
                
                if (ds.Tables.Contains("cTransactionErrors"))
                {
                    ds.Tables.Remove("cTransactionErrors");
                }
                ds.Tables.Add(dtErrorTable.Copy());
            }

            return ds;
        }

        #endregion

        /// <summary>
        /// GetParmObject converts the text value of a parm to the correct database type for CRUD procedures
        /// </summary>
        /// <param name="parm">cSpParm: Database parm definition</param>
        /// <param name="userParm">cSpParm: User provided parm/value</param>
        /// <remarks></remarks>
        private void GetParmObject(cSpParm parm, cSpParm userParm)
        {
            //var x = (Type)userParm.Parm.GetType();
            //var y = typeof(x).ToString();
            if (typeof(System.DBNull).ToString() == userParm.Parm.GetType().ToString())
            {
                iMyData.Add_SP_Parm((System.DBNull)userParm.Parm, userParm.Parameter);
                //add the user parm to the SP Call
            }
            else
            {
                switch ((parm.Type.ToLower()))
                {
                    case "varchar":
                    case "nvarchar":
                    case "text":
                    case "ntext":
                    case "char":
                    case "nchar":
                    case "nvarchar(max)":
                        iMyData.Add_SP_Parm(Convert.ToString(userParm.Parm), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "bit":
                        iMyData.Add_SP_Parm(Convert.ToBoolean(userParm.Parm), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "varbinary":
                    case "binary":
                    case "rowversion":
                        //byte()
                        iMyData.Add_SP_Parm(Convert.ToByte(userParm.Parm), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "uniqueidentifier":
                        //Guid
                        //iMyData.Add_SP_Parm((Guid)userParm.Parm, userParm.Parameter);
                        iMyData.Add_SP_Parm(new Guid(Convert.ToString(userParm.Parm)), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "tinyint":
                        //byte
                        iMyData.Add_SP_Parm(Convert.ToByte(userParm.Parm), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "smallint":
                        //int16
                        //iMyData.Add_SP_Parm((Int16)userParm.Parm, userParm.Parameter);
                        iMyData.Add_SP_Parm(Convert.ToInt16(userParm.Parm), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "int":
                    case "integer":
                        //int32
                        //iMyData.Add_SP_Parm((Int32)userParm.Parm, userParm.Parameter);
                        iMyData.Add_SP_Parm(Convert.ToInt32(userParm.Parm), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "bigint":
                        //int64
                        //iMyData.Add_SP_Parm((Int64)userParm.Parm, userParm.Parameter);
                        iMyData.Add_SP_Parm(Convert.ToInt64(userParm.Parm), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "smallmoney":
                    case "money":
                    case "numeric":
                    case "decimal":
                        //decimal
                        iMyData.Add_SP_Parm(Convert.ToDecimal(userParm.Parm), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "real":
                        //single
                        iMyData.Add_SP_Parm(Convert.ToSingle(userParm.Parm), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "float":
                        //double
                        iMyData.Add_SP_Parm(Convert.ToDouble(userParm.Parm), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "smalldatetime":
                    case "datetime":
                    case "date":
                        //datetime
                        //iMyData.Add_SP_Parm((DateTime)userParm.Parm, userParm.Parameter);    
                        Type t = userParm.Parm.GetType();
                        iMyData.Add_SP_Parm(Convert.ChangeType(userParm.Parm, t), userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    case "sql_variant":
                        //object
                        iMyData.Add_SP_Parm((object)userParm.Parm, userParm.Parameter, userParm.IsOutPutParm);
                        //add the user parm to the SP Call
                        break;
                    default:
                        LogError("Converting parm to database type for RObject", string.Format("Parameter: {0} with Type: {1} was unhandled", parm.Parameter, parm.Type), null, "GetParmObject funcation in cTransaction");
                        break;
                }
            }

        }
    }
}