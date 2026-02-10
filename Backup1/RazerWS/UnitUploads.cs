using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Reflection;
using System.Text;

namespace RazerWS
{
    public class UnitUploads
    {
        private DataSet Uploads;
        private cDataRazer.cLASERBaseTable myData;
        public string UserSubmitted  = "";

        public UnitUploads(DataSet Uploads, string UserID, cDataRazer.cLASERBaseTable myData )
        {
            this.Uploads = Uploads;
            this.myData = myData;
            UserSubmitted = UserID;
            
        }

        /// <summary>
        /// This method handles the uploading of unit data
        /// Method was rewritten by DWR on 6/13/12 because of issues with performance.
        /// Old method is commented out below.
        /// </summary>
        /// <returns></returns>
        public string Load()
        {
            //Setup the return value of the string to be empty -- means no error
            string retVal = "";
            //List<UnitUploadReturn> ReturnList = new List<UnitUploadReturn>();
            
            //Open a continuous connection so that the temp table can be shared
            myData.BeginContinuosConnection();

            //Create the temporary table to load the units into
            string sSQL = "CREATE TABLE #work_unit " +
                "(unit_id INT NOT NULL,contract_id INT NOT NULL,report_id INT NOT NULL,mso_id INT NOT NULL,cs_id INT NOT NULL,unit_type_id INT NOT NULL," +
                " service_period_start DATE NOT NULL,service_period_end DATE NOT NULL,unit_period_type INT NOT NULL,product_code VARCHAR(8) NOT NULL," +
                " estimated_flag INT NOT NULL,amount NUMERIC(16,2) NOT NULL,ancillary_id INT NOT NULL,manu_country_id INT NOT NULL,dest_country_id INT NOT NULL," +
                " model_id INT NOT NULL,model VARCHAR(40) NOT NULL,tech_id INT NOT NULL,brand_id INT NOT NULL,brand VARCHAR(40) NOT NULL," +
                " data_service_type_id INT NOT NULL,subscriber_id INT NOT NULL,manufacturer_product_id INT NOT NULL,replicator_id INT NOT NULL," +
                //" title_id INT NOT NULL,oem_id INT NOT NULL,software_id INT NOT NULL,uploaded INT NOT NULL,inserted INT NOT NULL,updated INT NOT NULL," +
                //" temp_unit_key INT IDENTITY)";
                " title_id INT NOT NULL,oem_id INT NOT NULL,software_id INT NOT NULL,tivo_count_id INT NOT NULL,tivo_count_description VARCHAR(60) NOT NULL," +
                " uploaded INT NOT NULL,inserted INT NOT NULL,updated INT NOT NULL,temp_unit_key INT IDENTITY)";

            //If create fails then exit the service and send message back to the app
            if (!myData.NonQuerrySql(sSQL))
            {
                return "Upload Failed on temp table create - " + myData.dberror ;
            }
            else
            {
                //Loop through each data row to be uploaded and insert it into the temp table created above
                //This will allow for mass inserts and updates of the unit data on the server in a later step
                foreach (DataRow Upload in Uploads.Tables["Uploads"].Rows)
                {
                    //int Uploaded = 0, Inserted = 0, Updated = 0;
                    myData.Add_SP_Parm(Upload["contract_id"], "@contract_id");
                    myData.Add_SP_Parm(Upload["report_id"], "@report_id");
                    myData.Add_SP_Parm(Upload["mso_id"], "@mso_id");
                    myData.Add_SP_Parm(Upload["cs_id"], "@cs_id");
                    myData.Add_SP_Parm(Upload["unit_type_id"], "@unit_type_id");
                    myData.Add_SP_Parm(Upload["service_period_start"], "@service_period_start");
                    myData.Add_SP_Parm(Upload["service_period_end"], "@service_period_end");
                    myData.Add_SP_Parm(Upload["unit_period_type"], "@unit_period_type");
                    myData.Add_SP_Parm(Upload["product_code"], "@product_code");
                    myData.Add_SP_Parm(Upload["estimated_flag"], "@estimated_flag");
                    myData.Add_SP_Parm(Upload["amount"], "@amount");
                    myData.Add_SP_Parm(Upload["ancillary_id"], "@ancillary_id");
                    myData.Add_SP_Parm(Upload["manu_country_id"], "@manu_country_id");
                    myData.Add_SP_Parm(Upload["dest_country_id"], "@dest_country_id");
                    myData.Add_SP_Parm(Upload["model_id"], "@model_id");
                    myData.Add_SP_Parm(Upload["model"], "@model");
                    myData.Add_SP_Parm(Upload["tech_id"], "@tech_id");
                    myData.Add_SP_Parm(Upload["brand_id"], "@brand_id");
                    myData.Add_SP_Parm(Upload["brand"], "@brand");
                    myData.Add_SP_Parm(Upload["data_service_type_id"], "@data_service_type_id");
                    myData.Add_SP_Parm(Upload["subscriber_id"], "@subscriber_id");
                    myData.Add_SP_Parm(Upload["manufacturer_product_id"], "@manufacturer_product_id");
                    myData.Add_SP_Parm(Upload["replicator_id"], "@replicator_id");
                    myData.Add_SP_Parm(Upload["title_id"], "@title_id");
                    myData.Add_SP_Parm(Upload["oem_id"], "@oem_id");
                    myData.Add_SP_Parm(Upload["software_id"], "@software_id");
                    //RES 12/20/16 Tivo Billing
                    myData.Add_SP_Parm(Upload["tivo_count_id"], "@tivo_count_id");
                    myData.Add_SP_Parm(Upload["tivo_count_description"], "@tivo_count_description");
  
                    //If any inserts to temp table fail then return the information to the user
                    if (!myData.NonQuerrySqlSp("usp_ins_unit_upload_row"))
                    {
                        retVal = "Upload Failed on temp table insert - " + myData.dberror ;
 
                    } //END if (myData.NonQuerrySqlSp("usp_ins_unit_upload"))
                } //END foreach (DataRow Upload in Uploads.Tables["Uploads"].Rows)

  
                //Open a transaction in case the procedure dies in process
                myData.BeginTransaction();

                //Execute the stored procedure that:
                //1) Creates missing model and brand data
                //2) Inserts new units to the unit table
                //3) Updates existing unit data amounts and estimate flag
                //4) Inserts new unit data into the unit_md table

                if (!myData.NonQuerrySqlSp("usp_unit_upload_data_all"))
                {
                    retVal = "Upload Failed on Upload SQL - " + myData.dberror;
                    myData.Rollback();
                }
                else
                {
                    myData.Commit();//report_id
                   
                   //CLB need to find report ids entered to see if they have the autocreate flag on at the contract_product level
                    //If so, it will try to create a batch and run bill calc

                    var ReportIDs = (from rows in Uploads.Tables["Uploads"].AsEnumerable()
                                     where rows.Field<int>("report_id") > 0
                                     select rows.Field<int>("report_id")).Distinct();

                    foreach (int reportID in ReportIDs)
                    {
                        myData.Add_SP_Parm(reportID,  "@report_id");
                        myData.Add_SP_Parm(UserSubmitted.ToString(), "@user_id");

                        //If any inserts to temp table fail then return the information to the user
                        if (!myData.NonQuerrySqlSp("usp_autocreate_batch_by_reportid"))
                        {
                            retVal = "Autocreate Batch Failed " + myData.dberror;

                        } //END if (myData.NonQuerrySqlSp("usp_ins_unit_upload"))
                    }
                }


            }//End of Upload

            myData.EndContinuosConnection();
     
            return retVal ;

            //*******Former method below*******
            //var retVal = true;
            //List<UnitUploadReturn> ReturnList = new List<UnitUploadReturn>();

            //myData.BeginContinuosConnection();

            ////create temp table
            //StringBuilder sbSQL = new StringBuilder();
            //sbSQL.Append("CREATE TABLE #TempUnit");
            //sbSQL.AppendLine("(");
            //sbSQL.AppendLine("	[unit_id] [int] NOT NULL,");
            //sbSQL.AppendLine("	[contract_id] [int] NOT NULL,");
            //sbSQL.AppendLine("	[report_id] [int] NOT NULL,");
            //sbSQL.AppendLine("	[mso_id] [int] NOT NULL,");
            //sbSQL.AppendLine("	[cs_id] [int] NOT NULL,");
            //sbSQL.AppendLine("	[unit_type_id] [int] NOT NULL,");
            //sbSQL.AppendLine("	[service_period_start] [datetime] NOT NULL,");
            //sbSQL.AppendLine("	[service_period_end] [datetime] NOT NULL,");
            //sbSQL.AppendLine("	[unit_period_type] [int] NOT NULL,");
            //sbSQL.AppendLine("	[product_code] [char](8) NOT NULL");
            //sbSQL.AppendLine(")");

            //string unit_type_id = Uploads.Tables["Uploads"].Rows[0]["unit_type_id"].ToString();

            //if (myData.NonQuerrySql(sbSQL.ToString().Trim()))
            //{
            //    sbSQL = new StringBuilder();
            //    sbSQL.AppendLine("SELECT     unit_md_category.md_name, unit_md_category.maint_table_used, unit_md_category.id_field, unit_md_category.description_field, unit_md_xref.unit_md_id");
            //    sbSQL.AppendLine("FROM         unit_md_category INNER JOIN");
            //    sbSQL.AppendLine("unit_md_xref ON unit_md_category.unit_md_id = unit_md_xref.unit_md_id");
            //    sbSQL.AppendLine(string.Format("WHERE     (unit_md_xref.unit_type_id = {0}) AND (unit_md_category.filter_only_flag = 0)", unit_type_id));


            //    if (myData.SqlStringPopDt(sbSQL.ToString().Trim()))
            //    {
            //        foreach (DataRow row in myData.GetDataTable.Rows)
            //        {
            //            sbSQL = new StringBuilder(string.Format("ALTER TABLE #TempUnit ADD [{0}] INT NULL", row["md_name"].ToString()));
            //            if (!myData.NonQuerrySql(sbSQL.ToString().Trim())) { retVal = false; break; }
            //        }

            //        if (retVal)
            //        {
            //            foreach (DataRow Upload in Uploads.Tables["Uploads"].Rows)
            //            {
            //                int Uploaded = 0, Inserted = 0, Updated = 0;
            //                myData.Add_SP_Parm(Upload["contract_id"], "@contract_id");
            //                myData.Add_SP_Parm(Upload["report_id"], "@report_id");
            //                myData.Add_SP_Parm(Upload["mso_id"], "@mso_id");
            //                myData.Add_SP_Parm(Upload["cs_id"], "@cs_id");
            //                myData.Add_SP_Parm(Upload["unit_type_id"], "@unit_type_id");
            //                myData.Add_SP_Parm(Upload["service_period_start"], "@service_period_start");
            //                myData.Add_SP_Parm(Upload["service_period_end"], "@service_period_end");
            //                myData.Add_SP_Parm(Upload["unit_period_type"], "@unit_period_type");
            //                myData.Add_SP_Parm(Upload["product_code"], "@product_code");
            //                myData.Add_SP_Parm(Upload["estimated_flag"], "@estimated_flag");
            //                myData.Add_SP_Parm(Upload["amount"], "@amount");
            //                myData.Add_SP_Parm(Upload["ancillary_id"], "@ancillary_id");
            //                myData.Add_SP_Parm(Upload["manu_country_id"], "@manu_country_id");
            //                myData.Add_SP_Parm(Upload["dest_country_id"], "@dest_country_id");
            //                myData.Add_SP_Parm(Upload["model_id"], "@model_id");
            //                myData.Add_SP_Parm(Upload["model"], "@model");
            //                myData.Add_SP_Parm(Upload["tech_id"], "@tech_id");
            //                myData.Add_SP_Parm(Upload["brand_id"], "@brand_id");
            //                myData.Add_SP_Parm(Upload["brand"], "@brand");
            //                myData.Add_SP_Parm(Upload["data_service_type_id"], "@data_service_type_id");
            //                myData.Add_SP_Parm(Upload["subscriber_id"], "@subscriber_id");
            //                myData.Add_SP_Parm(Upload["manufacturer_product_id"], "@manufacturer_product_id");
            //                myData.Add_SP_Parm(Upload["replicator_id"], "@replicator_id");
            //                myData.Add_SP_Parm(Upload["title_id"], "@title_id");
            //                myData.Add_SP_Parm(Upload["oem_id"], "@oem_id");
            //                myData.Add_SP_Parm(Upload["software_id"], "@software_id");
            //                myData.Add_SP_Parm(Uploaded, "@Uploaded", true);
            //                myData.Add_SP_Parm(Inserted, "@Inserted", true);
            //                myData.Add_SP_Parm(Updated, "@Updated", true);
            //                if (myData.NonQuerrySqlSp("usp_unit_upload_data_check"))
            //                {
            //                    Uploaded = Convert.ToInt32(myData.OutPutValues["@Uploaded"]);
            //                    Inserted = Convert.ToInt32(myData.OutPutValues["@Inserted"]);
            //                    Updated = Convert.ToInt32(myData.OutPutValues["@Updated"]);

            //                    ReturnList.Add(
            //                        new UnitUploadReturn
            //                        {
            //                            Upload = Upload,
            //                            Inserted = true,
            //                            Error = ""
            //                        });
            //                }
            //                else
            //                {
            //                    retVal = false;
            //                    ReturnList.Add(
            //                        new UnitUploadReturn
            //                        {
            //                            Upload = Upload,
            //                            Inserted = false,
            //                            Error = myData.dberror
            //                        });
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        retVal = false;
            //    }
            //}
            //else
            //{
            //    retVal = false;
            //}

            //myData.EndContinuosConnection();
            //var dt = CustomObjectToDataTable(ReturnList);
            //var ds = new DataSet();
            //ds.Tables.Add(dt.Copy());
            //return retVal;

        }

        /// <summary>
        /// This method handles the uploading of ancillary unit data
        /// </summary>
        /// <returns></returns>
        public string LoadAncillaryUnits()
        {
            //Setup the return value of the string to be empty -- means no error
            string retVal = "";
            bool BadDay = false;
            //List<UnitUploadReturn> ReturnList = new List<UnitUploadReturn>();

            //Open a continuous connection for performance
            myData.BeginContinuosConnection();

                //Loop through each data row to be uploaded and insert it into the temp table created above
                //This will allow for mass inserts and updates of the unit data on the server in a later step
            foreach (DataRow Upload in Uploads.Tables["ancillary_upload"].Rows)
            {
                //int Uploaded = 0, Inserted = 0, Updated = 0;
                myData.Add_SP_Parm(Upload["temp_status"], "@temp_status");
                myData.Add_SP_Parm(Upload["mca_address"], "@mca_address");
                myData.Add_SP_Parm(Upload["anc_code"], "@anc_code");
                myData.Add_SP_Parm(Upload["service_period"], "@service_period");
                myData.Add_SP_Parm(Upload["start_date"], "@start_date");
                myData.Add_SP_Parm(Upload["end_date"], "@end_date");
                myData.Add_SP_Parm(Upload["comment"], "@comment");
                myData.Add_SP_Parm(Upload["user_id"], "@user_id");

                //If any inserts to temp table fail then return the information to the user
                if (!myData.NonQuerrySqlSp("usp_ins_ancillary_upload"))
                {
                    retVal = "Upload Failed on insert to ancillary_upload table - " + myData.dberror;

                } //END if (myData.NonQuerrySqlSp("usp_ins_unit_upload"))
                //If any inserts to temp table fail then return the information to the user
                //if (!myData.NonQuerrySql("usp_ins_ancillary_upload"))
                //{
                //    retVal = "Upload Failed on insert to ancillary_upload table - " + myData.dberror;

                //} //END if (myDa

            }//END foreach (DataRow Upload in Uploads.Tables["Uploads"].Rows)

            if (retVal == "")
            {
                DateTime ServicePeriod;
                myData.Add_SP_Parm(Uploads.Tables["ancillary_upload"].Rows[0]["temp_status"], "@temp_status");
                if (!myData.SqlSpPopDt("usp_sel_ancillary_upload_service_period"))
                    retVal = "Ancillary upload service period check failed - " + myData.dberror;
                else
                {
                    retVal = "Service period(s) in upload spreadsheet: ";
                    foreach (DataRow row in myData.GetDataTable.Rows)
                    {
                        ServicePeriod = Convert.ToDateTime(row["service_period"]);
                        //RES 4/4/16 check for service period that is not first day of month
                        if (ServicePeriod.Day != 1) BadDay = true;
                        if (retVal == "Service period(s) in upload spreadsheet: ")
                            retVal = retVal + " " + ServicePeriod.ToShortDateString();
                        else
                            retVal = retVal + ", " + ServicePeriod.ToShortDateString();
                    }
                    if (BadDay) retVal = "Error Invalid " + retVal;
                }
            }

    
            myData.EndContinuosConnection();

            return retVal;
            
        }

        public DataTable CustomObjectToDataTable<T>(List<T> items)
        {
            DataTable dt = new DataTable();
            Type tType = typeof(T);
            PropertyInfo[] props = tType.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                DataColumn col = new DataColumn(prop.Name, prop.PropertyType);
                dt.Columns.Add(col);
            }

            foreach (T item in items)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo prop in props)
                {
                    dr[prop.Name] = prop.GetValue(item, null);
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

    }

    public class UnitUploadReturn
    {
        public dynamic Upload { get; set; }
        public bool Inserted { get; set; }
        public string Error { get; set; }
    }
}