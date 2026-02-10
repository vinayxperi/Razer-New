using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cDataRazer;
using System.Data;

namespace RazerWS
{
    class cUnitCalc
    {

        #region Enums
        public enum eOperator : int { Equal = 0, NotEqual = 1, In = 2, NotIn = 3 };
        public enum eConstruct : int { Add = 0, Subtract = 1 };
        public enum eQueryType : int { Contract = 0, Report = 1, Entity = 2, Location = 3 };
        #endregion


        #region Class Variables and Properties

        private DataTable dtTemp; //Temporary data table for holding intermediate results
        private DataSet dsWork = new DataSet(); //Work Dataset 

        private cDataRazer.cLASERDB SQLCA = new cDataRazer.cLASERDB();

        public int QueryCount { get; set; } //Tracks number of parameters passed
        private cDataRazer.cLASERBaseTable msTable;
        public cDataRazer.cLASERBaseTable sTable
        {
            get { return msTable; }
            set { msTable = value; }
        }


        private Int32 mFilterID;
        public Int32 FilterID
        {
            get { return mFilterID; }
            set { mFilterID = value; }
        }

        private Int32 mUnitPeriodType;
        public Int32 UnitPeriodType
        {
            get { return mUnitPeriodType; }
            set { mUnitPeriodType = value; }
        }

        private Int32 mEntityID;
        public Int32 EntityID
        {
            get { return mEntityID; }
            set { mEntityID = value; }
        }

        private Int32 mLocationID;
        public Int32 LocationID
        {
            get { return mLocationID; }
            set { mLocationID = value; }
        }

        private Int32 mContractID;
        public Int32 ContractID
        {
            get { return mContractID; }
            set { mContractID = value; }
        }

        private Int32 mReportID;
        public Int32 ReportID
        {
            get { return mReportID; }
            set { mReportID = value; }
        }

        private eQueryType mQueryType;
        public eQueryType QueryType
        {
            get { return mQueryType; }
            set { mQueryType = value; }
        }

        private Int32 mTotalEntityFlag;
        public Int32 TotalEntityFlag
        {
            get { return mTotalEntityFlag; }
            set { mTotalEntityFlag = value; }
        }

        private DateTime mServicePeriodStart;
        public DateTime ServicePeriodStart
        {
            get { return mServicePeriodStart; }
            set { mServicePeriodStart = value; }
        }

        private DateTime mServicePeriodEnd;
        public DateTime ServicePeriodEnd
        {
            get { return mServicePeriodEnd; }
            set { mServicePeriodEnd = value; }
        }

        //Debug Properties
        //private DataTable mTestData;
        //public DataTable TestData
        //{
        //    get { return mTestData; }
        //    set { mTestData = value; }
        //}

        //private DataTable mTestData2;
        //public DataTable TestData2
        //{
        //    get { return mTestData2; }
        //    set { mTestData2 = value; }
        //}

        private string mTestSQL;
        public string TestSQL
        {
            get { return mTestSQL; }
            set { mTestSQL = value; }
        }

        //public DataTable  TestData3 { get; set; }

        //private cLASERBaseTable sTable;

        #endregion

        public cUnitCalc(cLASERDB sqlCA)
        {
            //Establish the database connection variable
            SQLCA = sqlCA;

            //Create the base table used for queries
            sTable = new cLASERBaseTable(ref SQLCA, "Table");

            //Add the empty filter results table ToString the dsWork dataset
            dsWork.Tables.Add(CreateFilterResultTable());
            dsWork.Tables.Add(CreateFilterTotalTable());
            dsWork.Tables.Add(CreateErrorTable());
        }

        public void SetParms(Int32 filterID, DateTime servicePeriodStart, DateTime servicePeriodEnd, Int32 unitPeriodType, Int32 entityID, Int32 totalEntityFlag, Int32 contractID,
            Int32 locationID, Int32 reportID, string queryType)
        {
            //Move Parms to class properties
            FilterID = filterID;
            ServicePeriodStart = servicePeriodStart;
            ServicePeriodEnd = servicePeriodEnd;
            UnitPeriodType = unitPeriodType;
            EntityID = entityID;
            TotalEntityFlag = totalEntityFlag;
            ContractID = contractID;
            LocationID = locationID;
            ReportID = reportID;
            //@@May want to modify this to use an integer value parameter to lessen data passed.
            //Determine the query type enum value based on the string passed
            switch (queryType.ToLower())
            {
                case "report":
                    {
                        QueryType = eQueryType.Report;
                        break;
                    }
                case "contract":
                    {
                        QueryType = eQueryType.Contract;
                        break;
                    }
                case "entity":
                    {
                        QueryType = eQueryType.Entity;
                        break;
                    }

                default: //Location
                    {
                        QueryType = eQueryType.Location;
                        break;
                    }
            } //End Switch
        } //End Set Parms

        public DataSet RunUnitCalc(DataTable dtParms)
        {
            //Call this method to generate a unit total result set

            //Establish the counter for the number of queries run
            QueryCount = 1;

            //Cycle through the parms and run the count filter code 1 query request at a time.
            foreach (DataRow r in dtParms.Rows)
            {
                SetParms(Convert.ToInt32(r["filter_id"]), Convert.ToDateTime(r["service_period_start"]), Convert.ToDateTime(r["service_period_end"]),
                        Convert.ToInt32(r["unit_period_type"]), Convert.ToInt32(r["entity_id"]), Convert.ToInt32(r["total_entity_flag"]),
                        Convert.ToInt32(r["contract_id"]), Convert.ToInt32(r["location_id"]), Convert.ToInt32(r["report_id"]), Convert.ToString(r["query_type"]));
                if (!CalcFilter())
                {
                    return dsWork;
                }
                QueryCount++;
            }

            return dsWork;

        }


        public Boolean CalcFilter()
        {
            //Calculates the unit count for one set of query parameters.
            DataTable dtFilterRows;
            DataTable dtTempResult = CreateFilterResultTable();
            string sSQL = "";
            //Variable for storing total level error message
            string ErrorMessage = "";
            //Create variable for storing the total
            decimal UnitTotal = 0;
            //Variable for storing the last filter id that received an error so that multiple inactive status errors are not shown for the same filter.
            int LastFilterError = 0;

            DataRow rWork; //DataRow variable for various process tasks below

            ////Get the filter rows associated with the chosen unit filter and places them into a temp #work table
            sTable.Add_SP_Parm(Convert.ToString(FilterID), "@filter_id");
            if (!sTable.SqlSpPopDt("usp_get_filter_data"))
            //Error Code Here
            {
                if (sTable.RowCount < 1)
                {
                    ErrorMessage = "Filter not found";

                }
                else
                {
                    ErrorMessage = sTable.dberror;
                    //MessageBox.Show(sTable.dberror);
                }
                dsWork.Tables["FilterTotal"].Rows.Add(QueryCount, FilterID, UnitTotal);
                dsWork.Tables["Error"].Rows.Add(QueryCount, FilterID, FilterID, 0, ErrorMessage, "Unable to process");
                return true;
            }
            else //Query succcessful
            {
                dtFilterRows = sTable.GetDataTable;
                //dtFilterRows = "FilterRows";

                ////Add the filter rows to the work dataset
                //dsWork.Tables.Add(dtTemp);           
            }


            //Set the type of the query from the parameters passed
            string QueryLevel = "";
            switch (QueryType)
            {
                case eQueryType.Contract:
                    //Contract query does not require an Enitity as there is only one contarcting entity per contract
                    //If report ID is chosen, then it will only display the items under that report id for that contract
                    //If locatioon ID is chosen, it will only display the items under that contract and location -- and report if a value is given
                    QueryLevel = " AND a.contract_id = " + Convert.ToString(ContractID);
                    if (ReportID != 0)
                    { QueryLevel = QueryLevel + " AND a.report_id = " + Convert.ToString(ReportID); }
                    if (LocationID != 0)
                    { QueryLevel = QueryLevel + " AND a.cs_id = " + Convert.ToString(LocationID); }
                    break;
                case eQueryType.Report:
                    //Not sure this is needed.  It should basically do the same thing as selecting contract and then selecting a report
                    //@@If this is kept, a contract ID should be required
                    QueryLevel = " AND a.report_id = " + Convert.ToString(ReportID);
                    if (LocationID != 0)
                    { QueryLevel = QueryLevel + " AND a.cs_id = " + Convert.ToString(LocationID); }
                    break;
                case eQueryType.Entity:
                    //This will get all counts for the entire entity.  It can be filtered to a tighter level by providing contract, location and/or report
                    //@@Again not sure if contract and report options are needed as these can be derived from the contract query - If this remains it would be
                    //an error condition if a report id was provided without a contract id.
                    QueryLevel = " AND a.mso_id = " + Convert.ToString(EntityID);
                    if (ReportID != 0)
                    { QueryLevel = QueryLevel + " AND a.report_id = " + Convert.ToString(ReportID); }
                    if (LocationID != 0)
                    { QueryLevel = QueryLevel + " AND a.cs_id = " + Convert.ToString(LocationID); }
                    if (ContractID != 0)
                    { QueryLevel = QueryLevel + " AND a.contract_id = " + Convert.ToString(ContractID); }
                    break;
                case eQueryType.Location:
                    //Can return data for one location across all contracts, reports or entities
                    //Total entity flag gets the total for the entity. Not selecting the total Entity and providing an entity id will select only the counts for that location with that entity
                    //If the entity is not provided then the unit total represents all units for that location no matter what entity
                    //@@Again not sure if contract and report options are needed as these can be derived from the contract query - If this remains it would be
                    //an error condition if a report id was provided without a contract id.
                    if (TotalEntityFlag == 1 && EntityID != 0)
                    { QueryLevel = " AND a.mso_id = " + Convert.ToString(EntityID); }
                    else
                    { QueryLevel = " AND a.cs_id = " + Convert.ToString(LocationID); }
                    if (ReportID != 0)
                    { QueryLevel = QueryLevel + " AND a.report_id = " + Convert.ToString(ReportID); }
                    if (ContractID != 0)
                    { QueryLevel = QueryLevel + " AND a.contract_id = " + Convert.ToString(ContractID); }
                    if (EntityID != 0)
                    { QueryLevel = QueryLevel + " AND a.mso_id = " + Convert.ToString(EntityID); }
                    break;
                default:
                    break;
            }

            //Loop through filter rows and retrieve matching data
            foreach (DataRow r1 in dtFilterRows.Rows) // dsWork.Tables["FilterRows"].Rows)
            {
                //Reinitialize dtTemp
                dtTemp = new DataTable();
                //Setup SQL for the base operator type and the value to compare to the metadata type
                //Defaults to equal
                string Operator = "";
                //Value of -1 means that the code should select all units with any value assigned for that metadata type
                if ((string)r1["value"] != "-1")
                {
                    Operator = " AND FKey_id ";
                    switch ((int)r1["operator"])
                    {

                        case (int)eOperator.NotEqual: Operator = Operator + "<>" + Convert.ToString(r1["value"]);
                            break;
                        case (int)eOperator.In: Operator = Operator + "In (" + Convert.ToString(r1["value"]) + ")";
                            break;
                        case (int)eOperator.NotIn: Operator = Operator + "Not In (" + Convert.ToString(r1["value"]) + ")";
                            break;
                        default: Operator = Operator + "=" + Convert.ToString(r1["value"]);
                            break;
                    }
                }

                //The following SQL handles requesting unit totals for a service period range that differs from the service period range that the unit data is stored in
                //For example if a user requests one month of data from a unit count that is stored quarterly, this SQL will return 1/3 of the unit count.
                string RatioSQL = "CONVERT(DECIMAL(18,6),DATEDIFF(DAY,CASE WHEN '" + Convert.ToString(ServicePeriodStart) + "'>= a.service_period_start THEN '" +
                    Convert.ToString(ServicePeriodStart) + "' ELSE a.service_period_start END," +
                    "CASE  WHEN '" + Convert.ToString(ServicePeriodEnd) + "' <= a.service_period_end THEN '" + Convert.ToString(ServicePeriodEnd) + "' ELSE a.service_period_end END))" +
                    "/ CONVERT(DECIMAL(18,6),DATEDIFF(DAY,a.service_period_start,a.service_period_end)) ";

                //Select statement finds all units matching the filter criteria for the current filter row
                //Criteria includes
                //Service Periods: The service period start for the unit must be between the parameter service period start and service period end OR
                //                  The service period end must be between the two service period parameters OR
                //                  The service period start is less than the service period start parameter and the service period end is >= the service period end parameter
                //The fkey value of the unit metadata must match the Operator string built above
                sSQL = "SELECT Amount=CONVERT(DECIMAL(18,2),ROUND(SUM(amount * " + RatioSQL + "),2)) " +
                                          "FROM unit a, unit_md b " +
                                         "WHERE a.unit_id = b.unit_id " +
                                         " AND b.unit_md_id = " + Convert.ToString(r1["unit_md_id"]) +
                                         Operator + QueryLevel +
                                         " AND ((a.service_period_start BETWEEN '" + Convert.ToString(ServicePeriodStart) + "' AND '" + Convert.ToString(ServicePeriodEnd) + "' " +
                                         "OR a.service_period_end BETWEEN '" + Convert.ToString(ServicePeriodStart) + "' AND '" + Convert.ToString(ServicePeriodEnd) + "') " +
                                         "OR (a.service_period_start <= '" + Convert.ToString(ServicePeriodStart) +
                                         "' AND a.service_period_end >= '" + Convert.ToString(ServicePeriodEnd) + "'))";

                //Create the detailed work table row
                //rWork = dsWork.Tables["FilterResult"].NewRow();
                rWork = dtTempResult.NewRow();
                //rWork["filter_id"] = FilterID;
                rWork["query_id"] = QueryCount;
                rWork["filter_id"] = r1["filter_id"];
                rWork["seq_id"] = r1["seq_id"];
                rWork["amount"] = 0;
                rWork["construct"] = r1["construct"];
                rWork["sub_filter_id"] = r1["sub_filter_id"];
                rWork["sub_filter_construct"] = r1["sub_filter_construct"];
                rWork["sub_filter_sign"] = r1["sub_filter_sign"];
                rWork["filter_sign"] = r1["filter_sign"];


                //See if filter is inactive - Inactive filters will be calculated but an error condition will be recorded
                //Set last filter id variable so only one error per filter is received
                if (Convert.ToInt32(r1["inactive_flag"]) == 1 && LastFilterError != Convert.ToInt32(r1["filter_id"]))
                {
                    LastFilterError = Convert.ToInt32(r1["filter_id"]);
                    dsWork.Tables["Error"].Rows.Add(QueryCount, FilterID, r1["filter_id"], 0, "Inactive filter used in calculation", "Informational");

                }

                //Check for invalid parameter combinations
                //Check for equal with multi selection for values
                //If found then this row will not process
                if ((Convert.ToInt32(r1["operator"]) == 0 || Convert.ToInt32(r1["operator"]) == 1) && (r1["value"].ToString().IndexOf(",") > 0))
                {
                    dsWork.Tables["Error"].Rows.Add(QueryCount, FilterID, r1["filter_id"], r1["seq_id"], "Cannot have multiple values selected with an equal or not equal operator",
                                                                                            "Unable to process filter detail row");
                    //dsWork.Tables["FilterResult"].Rows.Add(rWork);
                    dtTempResult.Rows.Add(rWork);
                    //Skips to next row
                    continue;
                }

                if (!sTable.SqlStringPopDt(sSQL, false))
                { //Error Code Here
                    //Create Filter Results Error Record
                    dsWork.Tables["Error"].Rows.Add(QueryCount, FilterID, r1["filter_id"], r1["seq_id"], sTable.dberror, "Unable to process filter detail row");

                    //Debug Code
                    TestSQL = sSQL;
                }

                else
                {

                    //Place filter rows in dtTemp
                    dtTemp = sTable.GetDataTable;

                    //Make sure that rows were returned - If no rows then do nothing
                    if (dtTemp.Rows.Count > 0)
                    {
                        ////Check for null value and force to a zero
                        if (dtTemp.Rows[0]["amount"] == DBNull.Value)
                        {
                            rWork["amount"] = 0;
                            //If not a sub filter row then leave a message for no data
                            if (Convert.ToInt32(rWork["sub_filter_id"]) == 0)
                            {
                                dsWork.Tables["Error"].Rows.Add(QueryCount, FilterID, r1["filter_id"], r1["seq_id"], "No data for this filter row", "Informational");
                            }
                        }
                        else
                        { rWork["amount"] = Convert.ToDecimal(dtTemp.Rows[0]["amount"]) * Convert.ToDecimal(r1["filter_sign"]); }

                        //DEBUG Code
                        TestSQL = sSQL;
                    }
                    else
                    //Add no data found message if no rows returned
                    { dsWork.Tables["Error"].Rows.Add(QueryCount, FilterID, r1["filter_id"], r1["seq_id"], "No data for this filter row", "Informational"); }
                }

                //Add the record to the result table
                dtTempResult.Rows.Add(rWork);

            } // End foreach cycle through filter rows

            //Move the dtTempFilter rows to the work dataset
            CopyResultTable(dtTempResult, dsWork.Tables["FilterResult"]);

            //Set the unit total to zero
            UnitTotal = 0;

            //Cycle through temp filter results to come up with a single total
            foreach (DataRow r3 in dtTempResult.Rows)
            {
                UnitTotal = UnitTotal + Convert.ToDecimal(r3["amount"]);
            }

            //Add the total row to the results table
            dsWork.Tables["FilterTotal"].Rows.Add(QueryCount, FilterID, UnitTotal);

            return true;
        } //End of CalcFilter Method

        //Table creation methods
        private DataTable CreateFilterResultTable()
        {
            DataTable dt = new DataTable("FilterResult");
            dt.Columns.Add("query_id"); //represents the query row number from the parameter datatable.
            dt.Columns.Add("filter_id");
            dt.Columns.Add("seq_id");
            dt.Columns.Add("amount");
            dt.Columns.Add("construct");
            dt.Columns.Add("sub_filter_construct");
            dt.Columns.Add("sub_filter_id");
            dt.Columns.Add("filter_sign");
            dt.Columns.Add("sub_filter_sign");
            return dt;
        }

        private DataTable CreateFilterTotalTable()
        {
            DataTable dt = new DataTable("FilterTotal");
            dt.Columns.Add("query_id");//represents the query row number from the parameter datatable.
            dt.Columns.Add("filter_id");
            dt.Columns.Add("amount");
            return dt;
        }

        private DataTable CreateErrorTable()
        {
            DataTable dt = new DataTable("Error");
            dt.Columns.Add("query_id");//represents the query row number from the parameter datatable.
            dt.Columns.Add("parent_filter_id");
            dt.Columns.Add("filter_id");
            dt.Columns.Add("seq_id");
            dt.Columns.Add("error_message");
            dt.Columns.Add("error_severity");
            return dt;
        }

        private void CopyResultTable(DataTable FromTable, DataTable ToTable)
        {
            //Copies all data from the FromTable to the ToTable.
            //Column names must match exactly
            //It only copies the Column Names in the ToTable
            //so the To table can be a subset of the From table.
            foreach (DataRow rFrom in FromTable.Rows)
            {
                DataRow rTo = ToTable.NewRow();
                foreach (DataColumn c in ToTable.Columns)
                {
                    rTo[c.ColumnName] = rFrom[c.ColumnName];
                }
                ToTable.Rows.Add(rTo);
            }
        }


    }
}