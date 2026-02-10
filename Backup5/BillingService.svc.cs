using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cDataRazer;
using System.Data;
using System.Configuration;
using System.Transactions;

namespace RazerWS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BillingService" in code, svc and config file together.
    public class BillingService : IBillingService
    {
        private cLASERDB SQLCA = new cLASERDB();


        public DataSet GetCountValues(DataTable dtParms)
        {
            DataSet ds = new DataSet();
            if (InitDB())
            {
                cUnitCalc UnitCalc = new cUnitCalc(SQLCA);
                ds = UnitCalc.RunUnitCalc(dtParms);
            }
            return ds;
        }

        public string UpdateBillingLocations(int batchID, string companyCode)
        {
            if (InitDB())
            {
                //Verify the company code parm length is correct
                if (companyCode.Length > 2) { return "Company code is limited to 2 characters max."; }
                //Verify that the batch ID is not a negative number
                if (batchID < 0) { batchID = 0; }

                //Create the billing class and run the update method
                cBilling Billing = new cBilling(SQLCA);
                return Billing.UpdateBillingLocations(batchID, companyCode);
            }
            else { return "Error running update"; }
        }

        public bool ReprintNationalAdInvoice(string invoiceNumber, bool IsRevised, bool IsSaved, string printer)
        {
            if (InitDB())
            {
                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    cBilling billing = new cBilling(SQLCA);
                    return billing.ReprintNationAdInvoice(invoiceNumber, IsRevised, IsSaved, printer);
                }
            }
            return false;
        }

        public bool ScheduleJob(string UserID, string JobName, string JobParms, DateTime NextRunDate, string DistributionID)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(UserID, "@userid");
                myData.Add_SP_Parm(JobName, "@jobname");
                myData.Add_SP_Parm(JobParms, "@jobparms");
                myData.Add_SP_Parm(NextRunDate, "@next_run_dt");
                myData.Add_SP_Parm(DistributionID, "@distribution_id");

                if (myData.NonQuerrySqlSp("usp_insert_to_scheduler"))
                {
                    TF = true;
                }
            }
            return TF;
        }

        public int InsertRemitEntry(int batchid, int RemitID, string DocumentID, DateTime RemitDate, string RemitNumber, decimal Amount, decimal AmountFunctional, string CurrencyCode, Double ExchangeRate, decimal BankCharge)
        {
            int retVal = -1;
          
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(batchid, "@batch_id");
               
                myData.Add_SP_Parm(DocumentID, "@document_id");
                myData.Add_SP_Parm(RemitDate, "@remit_date");
                myData.Add_SP_Parm(RemitNumber, "@remit_number");
                myData.Add_SP_Parm(Amount, "@amount");
                myData.Add_SP_Parm(AmountFunctional, "@amount_functional");
                myData.Add_SP_Parm(CurrencyCode, "@currency_code");
                myData.Add_SP_Parm(ExchangeRate, "@exchange_rate");
                myData.Add_SP_Parm(BankCharge, "@bank_charge_amount");
                myData.Add_SP_Parm(retVal, "@remit_id", true);

                if (myData.NonQuerrySqlSp("usp_ins_remit"))
                {
                    foreach (var outputVal in myData.OutPutValues)
                    {
                        retVal = Convert.ToInt32(outputVal.Value);
                    }
                }
               
            }
            return retVal;
        }

        public void InsertAllocEntry(int BatchID, int RemitID, string ApplyToDoc, int Seq, decimal Amount, string CurrencyCode, string ReceivableAcct, string ProductCode, int Unapplied)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(BatchID, "@batch_id");
                myData.Add_SP_Parm(RemitID, "@remit_id");
                myData.Add_SP_Parm(ApplyToDoc, "@apply_to_doc");
                myData.Add_SP_Parm(Seq, "@apply_to_seq");
                myData.Add_SP_Parm(Amount, "@amount");
                myData.Add_SP_Parm(CurrencyCode, "@company_code");
                myData.Add_SP_Parm(ReceivableAcct, "@receivable_account");
                myData.Add_SP_Parm(ProductCode, "@product_code");
                myData.Add_SP_Parm(Unapplied, "@unapplied_flag");
                TF = myData.NonQuerrySqlSp("usp_ins_remit_alloc");
            }
        }

        public int InsertCashEntry(int SourceID, DateTime BatchDate, int BankID, decimal BatchTotal, string CurrencyCode, DateTime DatePeriod)
        {
            int retVal = -1;

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(SourceID, "@source_id");
                myData.Add_SP_Parm(BatchDate, "@batch_date");
                myData.Add_SP_Parm(BankID, "@bank_id");
                myData.Add_SP_Parm(BatchTotal, "@batch_total");
                myData.Add_SP_Parm(CurrencyCode, "@currency_code");
                myData.Add_SP_Parm(DatePeriod, "@acct_period");
                myData.Add_SP_Parm(retVal, "@batch_id", true);

                if (myData.NonQuerrySqlSp("usp_ins_cash_batch"))
                {
                    foreach (var outputVal in myData.OutPutValues)
                    {
                        retVal = Convert.ToInt32(outputVal.Value);
                    }
                }
            }

            return retVal;
        }

        public int InsertNewRatefromRateCopy(int RateID, string EndDate, int NewStatus, string UserID )
        {
            int retVal = -1;

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(RateID, "@rate_id");
                myData.Add_SP_Parm(EndDate, "@new_end_date");
                myData.Add_SP_Parm(NewStatus, "@new_status");
                myData.Add_SP_Parm(-1, "@newrateid", true);

                if (myData.NonQuerrySqlSp("usp_ins_rate_for_rate_copy"))
                {
                    foreach (var outputVal in myData.OutPutValues)
                    {
                        retVal = Convert.ToInt32(outputVal.Value);
                    }
                }
            }

            return retVal;
        }

        public bool DeleteRatefromRateCopy(int RateID)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(RateID, "@rate_id");


                if (myData.NonQuerrySqlSp("usp_del_rate_from_rate_copy"))
                {
                    TF = true;
                }
            }
            return TF;
        }

        public int InsertUnitEntry(int ContractID, int ReportID, int MsoID, int CSID, int UnitTypeID, DateTime PeriodStart, DateTime PeriodEnd, int UnitType, string ProductCode, int Estimated, decimal amount)
        {
            int retVal = -1;

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(ContractID, "@contract_id");
                myData.Add_SP_Parm(ReportID, "@report_id");
                myData.Add_SP_Parm(MsoID, "@mso_id");
                myData.Add_SP_Parm(CSID, "@cs_id");
                myData.Add_SP_Parm(UnitTypeID, "@unit_type_id");
                myData.Add_SP_Parm(PeriodStart, "@service_period_start");
                myData.Add_SP_Parm(PeriodEnd, "@service_period_end");
                myData.Add_SP_Parm(ProductCode, "@product_code");
                myData.Add_SP_Parm(UnitType, "@unit_period_type");
                myData.Add_SP_Parm(Estimated, "@estimated_flag");
                myData.Add_SP_Parm(amount, "@amount");
                myData.Add_SP_Parm(retVal, "@unit_id", true);
                if (myData.NonQuerrySqlSp("usp_ins_unit"))
                {
                    foreach (var outputVal in myData.OutPutValues)
                    {
                        retVal = Convert.ToInt32(outputVal.Value);
                    }
                }
            }

            return retVal;
        }
        public void InsertUnitMetaData(int UnitID, int MDID, int FKeyID)
        {
            bool TF = false;

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(UnitID, "@unit_id");
                myData.Add_SP_Parm(MDID, "@unit_md_id");
                myData.Add_SP_Parm(FKeyID, "@fkey_id");
                TF = myData.NonQuerrySqlSp("usp_ins_unit_md");
                
            }

          
        }
        public string GetNextInvoiceNumber(string RemitType)
        {
            string outVal = "";
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(RemitType, "@inv_type");

                if (myData.SqlSpPopDt("usp_next_invoice_number"))
                {
                    foreach (DataRow row in myData.GetDataTable.Rows)
                    {
                        outVal = row[0].ToString();
                    }
                }
            }
            return outVal;
        }

        public string NationalAdsAdjustment(DataSet NationalAdsAdjustmentDS)
        {            
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                NationalAdsAdjustment NationalAdsAdjustment = new NationalAdsAdjustment(NationalAdsAdjustmentDS, myData);
                return NationalAdsAdjustment.CreateAdjustment();
            }
            return string.Empty;
        }

        //public DataTable CreateCustomInvoiceMiscellaneousDetailRecords(DataTable parameters)
        //{
        //    return new DataTable();
        //}

        /// <summary>
        /// Executes Non query SQL (Inserts, Updates & Deletes)
        /// </summary>
        /// <param name="SQL">SQL Statement</param>
        /// <returns>Boolean</returns>
        public bool GenericSQL(string SQL)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                TF = myData.NonQuerrySql(SQL);
            }
            return TF;
        }

        public bool DeleteBatch(int BatchID)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(BatchID, "@batch_id");
               

                if (myData.NonQuerrySqlSp("usp_del_unposted_billing_batch"))
                {
                    TF = true;
                }
            }
            return TF;
        }

        public bool PermDeleteBatch(int BatchID)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(BatchID, "@batch_id");


                if (myData.NonQuerrySqlSp("usp_del_perm_unposted_billing_batch"))
                {
                    TF = true;
                }
            }
            return TF;
        }

        public bool DeleteContractfromBatch(int BatchID, int ContractID)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(BatchID, "@batch_id");
                myData.Add_SP_Parm(ContractID, "@contract_id");


                if (myData.NonQuerrySqlSp("usp_del_contract_from_billing_batch"))
                {
                    TF = true;
                }
            }
            return TF;
        }

        public bool DeleteContractfromCreateBatch(int BatchID, int ContractID)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(BatchID, "@batch_id");
                myData.Add_SP_Parm(ContractID, "@contract_id");


                if (myData.NonQuerrySqlSp("usp_del_contract_from_create_billing_batch"))
                {
                    TF = true;
                }
            }
            return TF;
        }


        public bool DeleteAdjustment(string DocumentID)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(DocumentID, "@document_id");


                if (myData.NonQuerrySqlSp("usp_del_adjustment"))
                {
                    TF = true;
                }
            }
            return TF;
        }

        public bool DeleteBCF(string BCFNumber)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(BCFNumber, "@BCF_number");


                if (myData.NonQuerrySqlSp("usp_del_bcf"))
                {
                    TF = true;
                }
            }
            return TF;
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


        public string UnitUpload(DataSet Uploads, string UserID)
        {
            string retVal = "";
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                var LoadUnits = new UnitUploads(Uploads, UserID, myData);
                retVal = LoadUnits.Load();
            }

            return retVal;
        }

        /// <summary>
        /// Added 10/29/12
        /// Method to load ancillary units that are driven by an MCA address
        /// This method uses the UnitUpload class to load and process the units in the ancillary_upload table
        /// </summary>
        /// <param name="Uploads">Dataset with the ancillary units to upload</param>
        /// <returns></returns>
        public string AncillaryUnitUpload(DataSet Uploads, string UserID)
        {
            string retVal = "";
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                var LoadUnits = new UnitUploads(Uploads, UserID, myData);
                retVal = LoadUnits.LoadAncillaryUnits();
            }

            return retVal;

        }

        public bool AdjustmentStatusUpdate()
        {
            bool retVal = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                if (myData.NonQuerrySqlSp("usp_upd_adjustment_status_to_schedule"))
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        public bool DeleteCustomInvoiceDetail(string invoiceNumber, int detailId)
        {
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                myData.Add_SP_Parm(invoiceNumber, "@invoice_number");
                myData.Add_SP_Parm(detailId, "@sequence_code");

                if (myData.NonQuerrySqlSp("usp_del_man_inv_dtl"))
                {
                    return true;
                }
            }

            return false;
        }

        public bool UpdateCustomInvoiceDetail(DataSet detailData)
        {
            int detailId, tempDetailId, acctDetailId;
            string idColumnName = "sequence_code";            

            if (InitDB())
            {                
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        foreach (DataRow row in detailData.Tables["detail"].Rows)
                        {   
                            if (!int.TryParse(row[idColumnName].ToString(), out tempDetailId))
                            {
                                scope.Dispose();
                                return false;
                            }

                            detailId = -1;

                            if (row.RowState == DataRowState.Added)
                            {
                                detailId = SaveCustomInvoiceDetailRecord(row, false);
                            }
                            else if (row.RowState == DataRowState.Modified)
                            {
                                detailId = SaveCustomInvoiceDetailRecord(row, true);
                            }

                            if (detailId > 0)
                            {
                                DeleteCustomInvoiceAccountDetailRecords(row["invoice_number"].ToString(), detailId);

                                foreach (DataRow childRow in detailData.Tables["acct_detail"].Rows)
                                {
                                    if (childRow.RowState == DataRowState.Deleted) { continue; }

                                    if (!int.TryParse(childRow[idColumnName].ToString(), out acctDetailId))
                                    {
                                        scope.Dispose();
                                        return false;
                                    }

                                    if (tempDetailId == acctDetailId)
                                    {
                                        if (!SaveCustomInvoiceAccountDetailRecord(childRow, detailId))
                                        {
                                            scope.Dispose();
                                            return false;
                                        }
                                    }
                                }
                            }
                            //else
                            //{
                            //    scope.Dispose();
                            //    return false;
                            //}
                        }
                    }
                    catch (TransactionAbortedException tex)
                    {
                        scope.Dispose();
                        return false;
                    }
                    scope.Complete();                        
                }              
            }

            return true;
        }

        
        private int SaveCustomInvoiceDetailRecord(DataRow row, bool isUpdate)
        {   
            cLASERBaseTable data = new cLASERBaseTable(ref SQLCA);            
            string sp = string.Empty;
            string idColumnName = "sequence_code";

            foreach (DataColumn column in row.Table.Columns)
            {
                string parameter = string.Format("@{0}", column.ColumnName);
                if (column.ColumnName == idColumnName)                
                {
                    if (!isUpdate)
                    {
                        sp = "usp_ins_man_inv_dtl";
                        data.Add_SP_Parm(-1, parameter, true);
                    }
                    else
                    {
                        sp = "usp_upd_man_inv_dtl";
                        data.Add_SP_Parm(row[column.ColumnName], parameter);
                    }   
                }
                else
                {
                    data.Add_SP_Parm(row[column.ColumnName], parameter);
                }
            }

            if (data.NonQuerrySqlSp(sp))
            {
                if (data.OutPutValues.ContainsKey(idColumnName))
                {
                    return (int)data.OutPutValues[idColumnName];
                }
                else
                {
                    return (int)row[idColumnName];
                }
            }

            return -1;
        }


        private bool DeleteCustomInvoiceAccountDetailRecords(string invoiceNumber, int detailId)
        {
            if (InitDB())
            {
                cLASERBaseTable data = new cLASERBaseTable(ref SQLCA);
                data.Add_SP_Parm(invoiceNumber, "@invoice_number");
                data.Add_SP_Parm(detailId, "@sequence_code");

                if (data.NonQuerrySqlSp("usp_del_man_inv_acct_dtl"))
                {
                    return true;
                }
            }
            return false;
        }


        private bool SaveCustomInvoiceAccountDetailRecord(DataRow row, int detailId)
        {           
            cLASERBaseTable data = new cLASERBaseTable(ref SQLCA);
            string sp = "usp_ins_misc_inv_acct_detail";
            string parentIdColumnName = "sequence_code";
            string idColumnName = "acct_detail_id";

            foreach (DataColumn column in row.Table.Columns)
            {
                string parameter = string.Format("@{0}", column.ColumnName);
                if (column.ColumnName == parentIdColumnName)
                {                    
                    data.Add_SP_Parm(detailId, parameter);                    
                }
                else if (!(column.ColumnName == idColumnName))
                {
                    data.Add_SP_Parm(row[column.ColumnName], parameter);
                }
            }

            if (data.NonQuerrySqlSp(sp))
            {
                return true;
            }

            return false;
        }

        public string WorkflowSubmit(string documentId, string userId)
        {
            string returnValue = string.Empty;

            if (InitDB())
            {
                cLASERBaseTable data = new cLASERBaseTable(ref SQLCA);

                string sp = string.Empty;

                if (documentId.ToUpper().StartsWith("M"))
                {
                    sp = "usp_sel_wf_man_inv_submit";
                }
                else if (documentId.ToUpper().StartsWith("ADJ"))
                {
                    sp = "usp_sel_wf_adjustment_inv_submit";
                }
                else if (documentId.ToUpper().StartsWith("BCF"))
                {
                    sp = "usp_sel_wf_bcf_submit";
                }
                else if (documentId.ToUpper().StartsWith("TF"))
                {
                    sp = "usp_sel_wf_tf_submit";
                }
                else if (documentId.ToUpper().StartsWith("INV"))
                {
                    sp = "usp_sel_wf_edit_inv_submit";
                }
                else
                {
                    return string.Format("There is no workflow associated with document Id {0}.", documentId);
                }
                
                string documentIdParameter = "@document_id";
                string userIdParameter = "@user_id";
                string errorMessageParameter = "@error_message";

                data.Add_SP_Parm(documentId, documentIdParameter);
                data.Add_SP_Parm(userId, userIdParameter);
                data.Add_SP_Parm(string.Empty, errorMessageParameter, true);

                if (data.NonQuerrySqlSp(sp))
                {
                    if (data.OutPutValues.Count() > 0)
                    {
                        returnValue = data.OutPutValues[errorMessageParameter].ToString();
                    }
                }
                else
                {
                    returnValue = string.Format("The stored procedure {0} failed to execute.", sp);
                }
            }

            return returnValue;
        }


        public string WorkflowApprove(string documentId, string userId)
        {
            string returnValue = string.Empty;

            if (InitDB())
            {
                cLASERBaseTable data = new cLASERBaseTable(ref SQLCA);

                string sp = string.Empty;

                if (documentId.ToUpper().StartsWith("M"))
                {
                    sp = "usp_sel_wf_man_inv_approve";
                }
                else if (documentId.ToUpper().StartsWith("ADJ"))
                {
                    sp = "usp_sel_wf_adjustment_inv_approve";
                }
                else if (documentId.ToUpper().StartsWith("BCF"))
                {
                    sp = "usp_sel_wf_bcf_approve";
                }
                else if (documentId.ToUpper().StartsWith("TF"))
                {
                    sp = "usp_sel_wf_tf_approve";
                }
                else if (documentId.ToUpper().StartsWith("INV"))
                {
                    sp = "usp_sel_wf_edit_inv_approve";
                }
                else
                {
                    return string.Format("There is no workflow associated with document Id {0}.", documentId);
                }
                
                string documentIdParameter = "@document_id";
                string userIdParameter = "@user_id";
                string errorMessageParameter = "@error_message";

                data.Add_SP_Parm(documentId, documentIdParameter);
                data.Add_SP_Parm(userId, userIdParameter);
                data.Add_SP_Parm(string.Empty, errorMessageParameter, true);

                if (data.NonQuerrySqlSp(sp))
                {
                    if (data.OutPutValues.Count() > 0)
                    {
                        returnValue = data.OutPutValues[errorMessageParameter].ToString();
                    }
                }
                else
                {
                    returnValue = string.Format("The stored procedure {0} failed to execute.", sp);
                }
            }

            return returnValue;
        }

        public DataTable LaunchWorkflowEmail(string documentId, string userId, int status)
        {
            //Dictionary<string, object> returnValues = new Dictionary<string, object>();
            DataTable table = new DataTable("returnValues");
            
            if (InitDB())
            {
                cLASERBaseTable data = new cLASERBaseTable(ref SQLCA);                
                
                string documentIdParameter = "@document_id";
                string userIdParameter = "@user_id";
                string statusParameter = "@wf_status";
                string typeParameter = "@wf_type";
                string classParameter = "@wf_class";
                string subjectParameter = "@subject";
                string errorMessageParameter = "@error_message";


                data.Add_SP_Parm(documentId, documentIdParameter);
                data.Add_SP_Parm(userId, userIdParameter);
                data.Add_SP_Parm(status, statusParameter);
                data.Add_SP_Parm(0, typeParameter, true);
                data.Add_SP_Parm(0, classParameter, true);
                data.Add_SP_Parm(string.Empty, subjectParameter, true);
                data.Add_SP_Parm(string.Empty, errorMessageParameter, true);

                table.Columns.Add(typeParameter);
                table.Columns.Add(classParameter);
                table.Columns.Add(subjectParameter);
                table.Columns.Add(errorMessageParameter);
                DataRow row = table.NewRow();
                table.Rows.Add(row);

                string sp = string.Empty;

                if (documentId.ToUpper().StartsWith("M"))
                {
                    sp = "usp_sel_wf_man_inv_popup";
                }
                else if (documentId.ToUpper().StartsWith("ADJ"))
                {
                    sp = "usp_sel_wf_adjustment_inv_popup";
                }
                else if (documentId.ToUpper().StartsWith("BCF"))
                {
                    sp = "usp_sel_wf_bcf_popup";
                }
                else if (documentId.ToUpper().StartsWith("TF"))
                {
                    sp = "usp_sel_wf_tf_popup";
                }
                else if (documentId.ToUpper().StartsWith("INV"))
                {
                    sp = "usp_sel_wf_edit_inv_popup";
                }
                else
                {
                    string error = string.Format("There is no workflow associated with document Id {0}.", documentId);

                    //returnValues.Add(errorMessageParameter, error);
                    row[errorMessageParameter] = error;
                    return table;
                }

                if (data.NonQuerrySqlSp(sp))
                {
                    if (data.OutPutValues.Count() > 0)
                    {
                        
                        foreach (KeyValuePair<string, object> pair in data.OutPutValues)
                        {   
                            row[pair.Key] = pair.Value;
                            //returnValues.Add(pair.Key, pair.Value);
                        }
                    }
                }
                else
                {
                    string error = string.Format("The stored procedure {0} failed to execute.", sp);
                    row[errorMessageParameter] = error;
                    return table;
                    //returnValues.Add(errorMessageParameter, error);
                }
            }

            return table;
        }

        public string SendWorkflowEmail(string documentId, string userId, int workflowId, int workflowStatus, string subject, string body, string emailTo)
        {

            string returnValue = string.Empty;

            if (InitDB())
            {
                cLASERBaseTable data = new cLASERBaseTable(ref SQLCA);

                string sp = string.Empty;

                if (documentId.ToUpper().StartsWith("M"))
                {
                    sp = "usp_sel_wf_man_inv_ok";
                }
                else if (documentId.ToUpper().StartsWith("ADJ"))
                {
                    sp = "usp_sel_wf_adjustment_inv_ok";
                }
                else if (documentId.ToUpper().StartsWith("BCF"))
                {
                    sp = "usp_sel_wf_bcf_ok";
                }
                else if (documentId.ToUpper().StartsWith("TF"))
                {
                    sp = "usp_sel_wf_tf_ok";
                }
                else if (documentId.ToUpper().StartsWith("INV"))
                {
                    sp = "usp_sel_wf_edit_inv_ok";
                }
                else
                {
                    return string.Format("There is no workflow associated with document Id {0}.", documentId);
                }
                
                string documentIdParameter = "@document_id";
                string userIdParameter = "@user_id";
                string workflowIdParameter = "@wf_id";
                string subjectParameter = "@email_subject";
                string bodyParameter = "@email_body";
                string emailToParameter = "@email_to";
                string errorMessageParameter = "@error_message";                
                string statusParameter = "@wf_status";

                data.Add_SP_Parm(documentId, documentIdParameter);
                data.Add_SP_Parm(userId, userIdParameter);
                data.Add_SP_Parm(workflowId, workflowIdParameter);
                data.Add_SP_Parm(subject, subjectParameter);
                data.Add_SP_Parm(body, bodyParameter);
                data.Add_SP_Parm(emailTo, emailToParameter);                
                data.Add_SP_Parm(workflowStatus, statusParameter);
                data.Add_SP_Parm(string.Empty, errorMessageParameter, true);

                if (data.NonQuerrySqlSp(sp))
                {
                    if (data.OutPutValues.Count() > 0)
                    {
                        returnValue = data.OutPutValues[errorMessageParameter].ToString();
                    }
                }
                else
                {
                    returnValue = string.Format("The stored procedure {0} failed to execute.", sp);
                }
            }

            return returnValue;
        }

        public bool AddWorkflowApprover(string documentId, string userId, int workflowSequnce, int positionIndicator, string userName)
        {
            if (InitDB())
            {
                cLASERBaseTable data = new cLASERBaseTable(ref SQLCA);

                string sp = "usp_add_approver_ok";
                string documentIdParameter = "@document_id";
                string userIdParameter = "@user_id";
                string workflowSeqParameter = "@wf_seq";
                string positionParameter = "@position_ind";
                string userNameParameter = "@username";


                data.Add_SP_Parm(documentId, documentIdParameter);
                data.Add_SP_Parm(userId, userIdParameter);
                data.Add_SP_Parm(workflowSequnce, workflowSeqParameter);
                data.Add_SP_Parm(positionIndicator, positionParameter);
                data.Add_SP_Parm(userName, userNameParameter);


                if (data.NonQuerrySqlSp(sp))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Used for AdjustmentAccountingConvInv and others
        /// </summary>
        /// <param name="DocToAdjust"></param>
        /// <param name="AdjTypeID"></param>
        /// <param name="UserID"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public string InsertNewAdjustmentPreamble(string DocToAdjust, int AdjTypeID, string UserID, decimal Amount)
        {
            string retVal = "";

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(DocToAdjust.Trim(), "@doc_to_adj_id");
                myData.Add_SP_Parm(AdjTypeID, "@adjustment_type_id");
                myData.Add_SP_Parm(UserID.Trim(), "@user_id");
                myData.Add_SP_Parm(Amount, "@amount");
                myData.Add_SP_Parm("", "@document_id", true);

                if (myData.NonQuerrySqlSp("usp_ins_adjustment"))
                {
                    foreach (var outputVal in myData.OutPutValues)
                    {
                        retVal = outputVal.Value.ToString();
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Used to delete Ajustment Preamble/Header on rollback
        /// </summary>
        /// <param name="Document_ID"></param>
        /// <returns></returns>
        public bool DeleteNewAdjusmentPreamble(string Document_ID)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(Document_ID.Trim(), "@document_id");


                if (myData.NonQuerrySqlSp("usp_del_adjustment_header"))
                {
                    TF = true;
                }
            }
            return TF;
        }

        public bool ClearExceptions(int typeID)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(typeID, "@type_id");


                if (myData.NonQuerrySqlSp("usp_del_exception"))
                {
                    TF = true;
                }
            }
            return TF;
        }

        public string WorkflowMultiApproval(DataTable dtToPass, string UserID)
        {
            //Need to look at document id - if Custom Invoice and begins with M, execute usp_sel_wf_man_inv_approve
            //if Adjustment, execute usp_sel_wf_adjustment_approve
           
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
            
                foreach (DataRow row in dtToPass.Rows)
                {
                    string documentID = "";
                    documentID = row["document_id"].ToString();
                    //bool isRowNew = false;
                    //bool isRowChanged = false;

                    myData.Add_SP_Parm(UserID, "@user_id");
                    myData.Add_SP_Parm(documentID, "@document_id");


                    string errmsg = "";
                    myData.Add_SP_Parm(errmsg, "@error_message", true);
                    string storedProc = "";

                    if (documentID.Substring(0, 1) == "M")
                      storedProc = "usp_sel_wf_man_inv_approve";
                    else if (documentID.Substring(0, 1) == "A")
                       storedProc = "usp_sel_wf_adjustment_approve";
                    else if (documentID.Substring(0, 1) == "B")
                        storedProc = "usp_sel_wf_bcf_approve";
                    else if (documentID.Substring(0, 1) == "I")
                        storedProc = "usp_sel_wf_edit_inv_approve";
  
  


                    if (myData.NonQuerrySqlSp(storedProc))
                    {
                       TF = true;
                    }
                    else
                       if (myData.OutPutValues.Count() != 0)
                           errmsg = myData.OutPutValues["@error_message"].ToString();
                }
            }
            return "";
            //return TF;
        }

        public bool EndJobonMonitor(int jobID)
        {
            bool TF = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(jobID, "@job_id");


                if (myData.NonQuerrySqlSp("usp_end_job_on_monitor"))
                {
                    TF = true;
                }
            }
            return TF;
        }

        /// <summary>
         /// used to insert UA Cash Adjustment
         /// </summary>
         /// <param name="CustNum"></param>
         /// <param name="AdjAmt"></param>
         /// <param name="ProdCode"></param>
         /// <param name="UserID"></param>
         /// <param name="CurrencyCode"></param>
         /// <param name="CompanyCode"></param>
         /// <param name="CostCtr"></param>
         /// <param name="Acct"></param>
         /// <param name="Region"></param>
         /// <param name="GLProduct"></param>
         /// <param name="OffsetCompany"></param>
         /// <param name="OffsetCostCtr"></param>
         /// <param name="OffsetAcct"></param>
         /// <param name="OffsetProduct"></param>
         /// <param name="OffsetRegion"></param>
         /// <param name="OffsetInterCo"></param>
         /// <param name="OffsetAmt"></param>
         /// <param name="ErrMsg"></param>
         /// <returns></returns>
        public string InsertNewAdjustmentUACash(string CustNum, string AdjAmt, string ProdCode, string UserID, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, ref string ErrMsg)
        {
            string retVal = "";

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                myData.Add_SP_Parm(CustNum.Trim(), "@receivable_account");
                myData.Add_SP_Parm(AdjAmt.Trim(), "@amt_to_adj");
                myData.Add_SP_Parm(ProdCode.Trim(), "@product_code");
	            myData.Add_SP_Parm(UserID, "@user_id");
                myData.Add_SP_Parm(CurrencyCode.Trim(), "@currency_code");
                myData.Add_SP_Parm(CompanyCode.Trim(), "@company_code");
                myData.Add_SP_Parm(CostCtr.Trim(), "@gl_center");
                myData.Add_SP_Parm(Acct.Trim(), "@gl_acct");
                myData.Add_SP_Parm(Region.Trim(), "@geography");
                myData.Add_SP_Parm(GLProduct.Trim(), "@gl_product");
                myData.Add_SP_Parm(OffsetCompany.Trim(), "@offsetting_co");
                myData.Add_SP_Parm(OffsetCostCtr.Trim(), "@offsetting_center");
                myData.Add_SP_Parm(OffsetAcct.Trim(), "@offsetting_account");
                myData.Add_SP_Parm(OffsetProduct.Trim(), "@offsetting_product");
                myData.Add_SP_Parm(OffsetRegion.Trim(), "@offsetting_region");
                myData.Add_SP_Parm(OffsetInterCo.Trim(), "@offsetting_interco");
                myData.Add_SP_Parm(OffsetAmt.Trim(), "@offsetting_amount");
                myData.Add_SP_Parm(retVal, "@adj_document_id", true);
                myData.Add_SP_Parm(ErrMsg, "@error_message", true);



                if (myData.NonQuerrySqlSp("usp_ins_accting_create_ua_cash_adjustment"))
                {
                    if (myData.OutPutValues.Count() != 0)
                    foreach (var outputVal in myData.OutPutValues)
                    {
                        ErrMsg = myData.OutPutValues["@error_message"].ToString();
                        return myData.OutPutValues["@adj_document_id"].ToString();
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// used to insert FX Credit Adjustment
        /// </summary>
        /// <param name="CustNum"></param>
        /// <param name="AdjAmt"></param>
        /// <param name="ProdCode"></param>
        /// <param name="UserID"></param>
        /// <param name="CurrencyCode"></param>
        /// <param name="CompanyCode"></param>
        /// <param name="CostCtr"></param>
        /// <param name="Acct"></param>
        /// <param name="Region"></param>
        /// <param name="GLProduct"></param>
        /// <param name="OffsetCompany"></param>
        /// <param name="OffsetCostCtr"></param>
        /// <param name="OffsetAcct"></param>
        /// <param name="OffsetProduct"></param>
        /// <param name="OffsetRegion"></param>
        /// <param name="OffsetInterCo"></param>
        /// <param name="OffsetAmt"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public string InsertNewAdjustmentFXCredit(string ApplyToDoc, string CustNum, string AdjAmt, string ProdCode, string UserID, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, ref string ErrMsg)
        {
            string retVal = "";

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                myData.Add_SP_Parm(ApplyToDoc.Trim(), "@apply_to_doc");
                myData.Add_SP_Parm(CustNum.Trim(), "@receivable_account");
                myData.Add_SP_Parm(AdjAmt.Trim(), "@amt_to_adj");
                myData.Add_SP_Parm(ProdCode.Trim(), "@product_code");
                myData.Add_SP_Parm(UserID, "@user_id");
                myData.Add_SP_Parm(CurrencyCode.Trim(), "@currency_code");
                myData.Add_SP_Parm(CompanyCode.Trim(), "@company_code");
                myData.Add_SP_Parm(CostCtr.Trim(), "@gl_center");
                myData.Add_SP_Parm(Acct.Trim(), "@gl_acct");
                myData.Add_SP_Parm(Region.Trim(), "@geography");
                myData.Add_SP_Parm(GLProduct.Trim(), "@gl_product");
                myData.Add_SP_Parm(OffsetCompany.Trim(), "@offsetting_co");
                myData.Add_SP_Parm(OffsetCostCtr.Trim(), "@offsetting_center");
                myData.Add_SP_Parm(OffsetAcct.Trim(), "@offsetting_account");
                myData.Add_SP_Parm(OffsetProduct.Trim(), "@offsetting_product");
                myData.Add_SP_Parm(OffsetRegion.Trim(), "@offsetting_region");
                myData.Add_SP_Parm(OffsetInterCo.Trim(), "@offsetting_interco");
                myData.Add_SP_Parm(OffsetAmt.Trim(), "@offsetting_amount");
                myData.Add_SP_Parm(retVal, "@adj_document_id", true);
                myData.Add_SP_Parm(ErrMsg, "@error_message", true);

                if (myData.NonQuerrySqlSp("usp_ins_accting_create_fx_credit_adjustment"))
                {
                    if (myData.OutPutValues.Count() != 0)
                        foreach (var outputVal in myData.OutPutValues)
                        {
                            ErrMsg = myData.OutPutValues["@error_message"].ToString();
                            return myData.OutPutValues["@adj_document_id"].ToString();
                        }
                }
            }

            return retVal;
        }

        /// <summary>
        /// used to insert FX Debit Adjustment
        /// </summary>
        /// <param name="CustNum"></param>
        /// <param name="AdjAmt"></param>
        /// <param name="ProdCode"></param>
        /// <param name="UserID"></param>
        /// <param name="CurrencyCode"></param>
        /// <param name="CompanyCode"></param>
        /// <param name="CostCtr"></param>
        /// <param name="Acct"></param>
        /// <param name="Region"></param>
        /// <param name="GLProduct"></param>
        /// <param name="OffsetCompany"></param>
        /// <param name="OffsetCostCtr"></param>
        /// <param name="OffsetAcct"></param>
        /// <param name="OffsetProduct"></param>
        /// <param name="OffsetRegion"></param>
        /// <param name="OffsetInterCo"></param>
        /// <param name="OffsetAmt"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public string InsertNewAdjustmentFXDebit(string ApplyToDoc, string SeqID, string NewDocID, string CustNum, string AdjAmt, string ProdCode, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLIC, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, ref string ErrMsg)
        {
            string retVal = "";

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                myData.Add_SP_Parm(ApplyToDoc.Trim(), "@apply_to_doc");
                myData.Add_SP_Parm(SeqID.Trim(), "@apply_to_seq");
                myData.Add_SP_Parm(NewDocID.Trim(), "@adj_document_id");
                myData.Add_SP_Parm(CustNum.Trim(), "@receivable_account");
                myData.Add_SP_Parm(AdjAmt.Trim(), "@amt_to_adjust");
                myData.Add_SP_Parm(ProdCode.Trim(), "@product_code");          
                myData.Add_SP_Parm(CurrencyCode.Trim(), "@currency_code");
                myData.Add_SP_Parm(CompanyCode.Trim(), "@company_code");
                myData.Add_SP_Parm(CostCtr.Trim(), "@gl_center");
                myData.Add_SP_Parm(Acct.Trim(), "@gl_acct");
                myData.Add_SP_Parm(Region.Trim(), "@geography");
                myData.Add_SP_Parm(GLIC.Trim(), "@interdivision");
                myData.Add_SP_Parm(GLProduct.Trim(), "@gl_product");
                myData.Add_SP_Parm(OffsetCompany.Trim(), "@offsetting_co");
                myData.Add_SP_Parm(OffsetCostCtr.Trim(), "@offsetting_center");
                myData.Add_SP_Parm(OffsetAcct.Trim(), "@offsetting_account");
                myData.Add_SP_Parm(OffsetProduct.Trim(), "@offsetting_product");
                myData.Add_SP_Parm(OffsetRegion.Trim(), "@offsetting_region");
                myData.Add_SP_Parm(OffsetInterCo.Trim(), "@offsetting_interco");
                myData.Add_SP_Parm(OffsetAmt.Trim(), "@offsetting_amount");

                myData.Add_SP_Parm(retVal, "@adj_number", true);
                myData.Add_SP_Parm(ErrMsg, "@error_message", true);

                
                if (myData.NonQuerrySqlSp("usp_ins_adjustment_detail_FX"))
                {
                    if (myData.OutPutValues.Count() != 0)
                        foreach (var outputVal in myData.OutPutValues)
                        {
                            ErrMsg = myData.OutPutValues["@error_message"].ToString();
                            return myData.OutPutValues["@adj_number"].ToString();
                        }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets Misc Credit Max # of Adjustments and Max Adjustment Amt
        /// </summary>
        /// <param name="MaxNumAdjs"></param>
        /// <param name="MaxAdjAmt"></param>
        /// <returns></returns>
        public bool GetMiscCreditDebitMaximums(ref string MaxNumAdjs, ref string MaxAdjAmt)
        {
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                myData.Add_SP_Parm(MaxNumAdjs, "@adj_crdb", true);
                myData.Add_SP_Parm(MaxAdjAmt, "@max_crdb", true);

                if (myData.NonQuerrySqlSp("usp_sel_razer_ref_miscdbcr"))
                {
                    if (myData.OutPutValues.Count() != 0)
                        foreach (var outputVal in myData.OutPutValues)
                        {
                            MaxNumAdjs = myData.OutPutValues["@adj_crdb"].ToString();
                            MaxAdjAmt = myData.OutPutValues["@max_crdb"].ToString();
                            return true;
                        }
                }
            }
            return false;
        }

        public string InsertNewAdjustmentMiscCreditDebit(string CustNum, string AdjAmt, string ProdCode, string UserID, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, string ApplyToDoc, string SeqCode, ref string ErrMsg)
        {
            string retVal = "";

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                myData.Add_SP_Parm(CustNum.Trim(), "@receivable_account");
                myData.Add_SP_Parm(AdjAmt.Trim(), "@amt_to_adj");
                myData.Add_SP_Parm(ProdCode.Trim(), "@product_code");
                myData.Add_SP_Parm(UserID, "@user_id");
                myData.Add_SP_Parm(CurrencyCode.Trim(), "@currency_code");
                myData.Add_SP_Parm(CompanyCode.Trim(), "@company_code");
                myData.Add_SP_Parm(CostCtr.Trim(), "@gl_center");
                myData.Add_SP_Parm(Acct.Trim(), "@gl_acct");
                myData.Add_SP_Parm(Region.Trim(), "@geography");
                myData.Add_SP_Parm(GLProduct.Trim(), "@gl_product");
                myData.Add_SP_Parm(OffsetCompany.Trim(), "@offsetting_co");
                myData.Add_SP_Parm(OffsetCostCtr.Trim(), "@offsetting_center");
                myData.Add_SP_Parm(OffsetAcct.Trim(), "@offsetting_account");
                myData.Add_SP_Parm(OffsetProduct.Trim(), "@offsetting_product");
                myData.Add_SP_Parm(OffsetRegion.Trim(), "@offsetting_region");
                myData.Add_SP_Parm(OffsetInterCo.Trim(), "@offsetting_interco");
                myData.Add_SP_Parm(OffsetAmt.Trim(), "@offsetting_amount");
                myData.Add_SP_Parm(ApplyToDoc.Trim(), "@apply_to_doc");
                myData.Add_SP_Parm(SeqCode.Trim(), "@apply_to_seq");

                myData.Add_SP_Parm(retVal, "@adj_document_id", true);
                myData.Add_SP_Parm(ErrMsg, "@error_message", true);

                if (myData.NonQuerrySqlSp("usp_ins_misc_dbcr_doc_to_adjust"))
                {
                    if (myData.OutPutValues.Count() != 0)
                        foreach (var outputVal in myData.OutPutValues)
                        {
                            ErrMsg = myData.OutPutValues["@error_message"].ToString();
                            return myData.OutPutValues["@adj_document_id"].ToString();
                        }
                }
            }

            return retVal;
        }

        public string InsertNewAdjustmentBillingDiff(string ApplyToDoc,string AdjustmentID,string AdjAmt,string Proforma,string ProformaAmt,string ProformaCurrency,string ConversionDate, string FXRate, string USDAmt, ref string ErrMsg)
        {
            string retVal = "";

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                myData.Add_SP_Parm(ApplyToDoc.Trim(), "@apply_to_doc");
                myData.Add_SP_Parm(AdjustmentID.Trim(), "@adj_document_id");
                myData.Add_SP_Parm(AdjAmt.Trim(), "@amount_adjusted");
                myData.Add_SP_Parm(Proforma.Trim(), "@proforma_id");
                myData.Add_SP_Parm(ProformaAmt.Trim(), "@proforma_amount");
                myData.Add_SP_Parm(ProformaCurrency.Trim(), "@proforma_currency");
                myData.Add_SP_Parm(ConversionDate.Trim(), "@conversion_date");
                myData.Add_SP_Parm(FXRate.Trim(), "@conversion_rate");
                myData.Add_SP_Parm(USDAmt.Trim(), "@usd_amount");

                myData.Add_SP_Parm(retVal, "@adj_number", true);
                myData.Add_SP_Parm(ErrMsg, "@error_message", true);

                if (myData.NonQuerrySqlSp("usp_ins_adj_billing_diff"))
                {
                    if (myData.OutPutValues.Count() != 0)
                        foreach (var outputVal in myData.OutPutValues)
                        {
                            ErrMsg = myData.OutPutValues["@error_message"].ToString();
                            return myData.OutPutValues["@adj_number"].ToString();
                        }
                }
            }

            return retVal;
        }

        public bool SetInvoiceEmailQueue(string primary_co)
        {
            bool TF = false;

            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                myData.Add_SP_Parm(primary_co, "@primary_co");

                if (myData.NonQuerrySqlSp("usp_upd_invoiceEmail_sentFlag_2_inProgress"))
                {
                    TF = true;
                }
            }
            return TF;
        }

        public bool InsertNewCashAdjustment(string[] arrSource, DataSet dsDest, int AdjType, ref string NewAdjId, ref string ErrMsg)
        {
            if (InitDB())
            {
                cLASERBaseTable myDataCashSource = new cLASERBaseTable(ref SQLCA);
                cLASERBaseTable myDataCashDestination = new cLASERBaseTable(ref SQLCA);
                cLASERBaseTable myDataDebitMain = new cLASERBaseTable(ref SQLCA);
                cLASERBaseTable myDataDebitDetail = new cLASERBaseTable(ref SQLCA);
                //start trans
                //myData.BeginTransaction();
                //Get a new Adj Id
                NewAdjId = InsertNewAdjustmentPreamble(arrSource[0].Trim(), AdjType, arrSource[10], Convert.ToDecimal(arrSource[2]) * -1);
                if (NewAdjId == "")
                {
                    //throw err return false
                    ErrMsg = "New Adjustment Cannot be Created : BillingService.InsertNewCashAdjustment";
                    return false;
                }
                else
                {
                    if (AdjType == 5) //Adjust Cash Adjustment
                    {
                        myDataCashSource.Add_SP_Parm(NewAdjId, "@adj_document_id");
                        myDataCashSource.Add_SP_Parm(arrSource[11].Trim(), "@apply_to_doc");
                        myDataCashSource.Add_SP_Parm(arrSource[1].Trim(), "@receivable_account");
                        myDataCashSource.Add_SP_Parm(arrSource[2].Trim(), "@amt_to_adjust");
                        myDataCashSource.Add_SP_Parm(arrSource[3].Trim(), "@product_code");
                        myDataCashSource.Add_SP_Parm(arrSource[4].Trim(), "@currency_code");
                        myDataCashSource.Add_SP_Parm(arrSource[5].Trim(), "@company_code");
                        myDataCashSource.Add_SP_Parm(arrSource[6].Trim(), "@gl_center");
                        myDataCashSource.Add_SP_Parm(arrSource[7].Trim(), "@gl_acct");
                        myDataCashSource.Add_SP_Parm(arrSource[8].Trim(), "@geography");
                        myDataCashSource.Add_SP_Parm(arrSource[9].Trim(), "@gl_product");
                        //myDataCashSource.Add_SP_Parm(arrSource[10], "@user_id");
                        myDataCashSource.Add_SP_Parm(arrSource[12], "@apply_to_seq");
                        //TODO: don't know what to do with division at this point. It has been eliminated most places
                        myDataCashSource.Add_SP_Parm("00", "@interdivision");
                        myDataCashSource.Add_SP_Parm(ErrMsg, "@error_message", true);
                        myDataCashSource.Add_SP_Parm(AdjType, "@adjustment_type_id");
                        if (myDataCashSource.NonQuerrySqlSp("usp_ins_adj_cash_adjustment_detail"))
                        {
                            if (myDataCashSource.OutPutValues.Count() != 0)
                            {
                                foreach (var outputVal in myDataCashSource.OutPutValues)
                                {
                                    ErrMsg = myDataCashSource.OutPutValues["@error_message"].ToString();
                                    if (ErrMsg != "")
                                    {
                                        //sp failed
                                        //myDataCashSource.Rollback();
                                        return false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //sp failed
                            ErrMsg = "Stored Procedure usp_ins_adj_cash_adjustment_detail failed to execute for document : " + arrSource[0].Trim();
                            //myDataCashSource.Rollback();
                            return false;
                        }
                    }
                    else
                    {
                        if (AdjType == 23) //RES 4/19/19 Debit Apply Cash Adjustment
                        {
                            try
                            {
                                foreach (DataRow row1 in dsDest.Tables["debitmain"].Rows)
                                {
                                    //if amount_to_adjust is 0 then no cash has been taken from the line so go to next line
                                    if (Convert.ToDouble(row1["amount_adjusted"].ToString().Trim()) == 0) continue;
                                    //Call adj_detail for row in GridSource and the AdjAmt (neg num)/////////////////////
                                    myDataDebitMain.Add_SP_Parm(row1["document_id"].ToString().Trim(), "@apply_to_doc");
                                    myDataDebitMain.Add_SP_Parm(row1["seq_code"].ToString().Trim(), "@apply_to_seq");
                                    myDataDebitMain.Add_SP_Parm(NewAdjId, "@adj_document_id");                                    
                                    myDataDebitMain.Add_SP_Parm(row1["receivable_account"].ToString().Trim(), "@receivable_account");
                                    myDataDebitMain.Add_SP_Parm(row1["amount_adjusted"].ToString().Trim(), "@amount_adjusted");
                                    myDataDebitMain.Add_SP_Parm(row1["product_code"].ToString().Trim(), "@product_code");
                                    myDataDebitMain.Add_SP_Parm(row1["currency_code"].ToString().Trim(), "@currency_code");
                                    myDataDebitMain.Add_SP_Parm(row1["company_code"].ToString().Trim(), "@company_code");
                                    myDataDebitMain.Add_SP_Parm(row1["rebill_flag"].ToString().Trim(), "@rebill_flag");
                                    myDataDebitMain.Add_SP_Parm(ErrMsg, "@error_message", true);
                                    //myDataDebitMain.Add_SP_Parm(AdjType, "@adjustment_type_id");
                                    if (myDataDebitMain.NonQuerrySqlSp("usp_ins_adj_credit_debit_dtl"))
                                    {
                                        if (myDataDebitMain.OutPutValues.Count() != 0)
                                        {
                                            foreach (var outputVal in myDataDebitMain.OutPutValues)
                                            {
                                                ErrMsg = myDataDebitMain.OutPutValues["@error_message"].ToString();
                                                if (ErrMsg != "")
                                                {
                                                    //sp failed
                                                    //myDataCashSource.Rollback();
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //sp failed
                                        ErrMsg = "Stored Procedure usp_ins_adj_credit_debit_dtl failed to execute for document : " + arrSource[0].Trim();
                                        //myDataCashSource.Rollback();
                                        return false;
                                    }
                                }
                                foreach (DataRow row1 in dsDest.Tables["detail"].Rows)
                                {
                                    //if amount_to_adjust is 0 then no cash has been taken from the line so go to next line
                                    if (Convert.ToDouble(row1["amount_adjusted"].ToString().Trim()) == 0) continue;
                                    //Call adj_detail for row in GridSource and the AdjAmt (neg num)/////////////////////
                                    myDataDebitDetail.Add_SP_Parm(row1["apply_to_doc"].ToString().Trim(), "@apply_to_doc");
                                    myDataDebitDetail.Add_SP_Parm(row1["apply_to_seq"].ToString().Trim(), "@apply_to_seq");
                                    myDataDebitDetail.Add_SP_Parm(NewAdjId, "@adj_document_id");
                                    myDataDebitDetail.Add_SP_Parm(row1["receivable_account"].ToString().Trim(), "@receivable_account");
                                    myDataDebitDetail.Add_SP_Parm(row1["amount_adjusted"].ToString().Trim(), "@amount_adjusted");
                                    myDataDebitDetail.Add_SP_Parm(row1["product_code"].ToString().Trim(), "@product_code");
                                    myDataDebitDetail.Add_SP_Parm(AdjType, "@adjustment_type");
                                    myDataDebitDetail.Add_SP_Parm(row1["currency_code"].ToString().Trim(), "@currency_code");
                                    myDataDebitDetail.Add_SP_Parm(row1["rebill_flag"].ToString().Trim(), "@rebill_flag");
                                    myDataDebitDetail.Add_SP_Parm(row1["inv_line_id"].ToString().Trim(), "@inv_line_id");
                                    myDataDebitDetail.Add_SP_Parm(row1["company_code"].ToString().Trim(), "@company_code");
                                    myDataDebitDetail.Add_SP_Parm(row1["gl_center"].ToString().Trim(), "@gl_center");
                                    myDataDebitDetail.Add_SP_Parm(row1["gl_acct"].ToString().Trim(), "@gl_acct");
                                    myDataDebitDetail.Add_SP_Parm(row1["geography"].ToString().Trim(), "@geography");
                                    myDataDebitDetail.Add_SP_Parm(row1["interdivision"].ToString().Trim(), "@interdivision");
                                    myDataDebitDetail.Add_SP_Parm(row1["gl_product"].ToString().Trim(), "@gl_product");
                                    myDataDebitDetail.Add_SP_Parm(row1["zero_tax_flag"].ToString().Trim(), "@zero_tax_flag");
                                    myDataDebitDetail.Add_SP_Parm(row1["use_current_tax_rates_flag"].ToString().Trim(), "@use_current_tax_rates_flag");
                                    myDataDebitDetail.Add_SP_Parm(row1["tax_amount"].ToString().Trim(), "@tax_amount");
                                    myDataDebitDetail.Add_SP_Parm(row1["extended"].ToString().Trim(), "@extended");
                                    myDataDebitDetail.Add_SP_Parm(row1["item_id"].ToString().Trim(), "@item_id");
                                    myDataDebitDetail.Add_SP_Parm(row1["adjusted_tax_amount"].ToString().Trim(), "@adjusted_tax_amount");
                                    myDataDebitDetail.Add_SP_Parm(ErrMsg, "@error_message", true);
                                    //myDataDebitDetail.Add_SP_Parm(AdjType, "@adjustment_type_id");
                                    if (myDataDebitDetail.NonQuerrySqlSp("usp_ins_adj_credit_debit_acct"))
                                    {
                                        if (myDataDebitDetail.OutPutValues.Count() != 0)
                                        {
                                            foreach (var outputVal in myDataDebitDetail.OutPutValues)
                                            {
                                                ErrMsg = myDataDebitDetail.OutPutValues["@error_message"].ToString();
                                                if (ErrMsg != "")
                                                {
                                                    //sp failed
                                                    //myDataCashSource.Rollback();
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //sp failed
                                        ErrMsg = "Stored Procedure usp_ins_adj_credit_debit_acct failed to execute for document : " + arrSource[0].Trim();
                                        //myDataCashSource.Rollback();
                                        return false;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ErrMsg = ex.Message;
                                //myDataCashDestination.Rollback();
                                return false;
                            }
                        }
                        //RES phase 3.0 8/5/13 add ability to apply cash from multiple product lines in cash doc
                        //For each rec in GridSource call usp_ins_adj_cash_adjustment_detail
                        try
                        {
                            foreach (DataRow row1 in dsDest.Tables["main"].Rows)
                            {
                                //if amount_to_adjust is 0 then no cash has been taken from the line so go to next line
                                if (Convert.ToDouble(row1["amount_to_adjust"].ToString().Trim()) == 0) continue;
                                //Call adj_detail for row in GridSource and the AdjAmt (neg num)/////////////////////
                                myDataCashSource.Add_SP_Parm(NewAdjId, "@adj_document_id");
                                myDataCashSource.Add_SP_Parm(row1["document_id"].ToString().Trim(), "@apply_to_doc");
                                myDataCashSource.Add_SP_Parm(row1["receivable_account"].ToString().Trim(), "@receivable_account");
                                myDataCashSource.Add_SP_Parm(row1["amount_to_adjust"].ToString().Trim(), "@amt_to_adjust");
                                myDataCashSource.Add_SP_Parm(row1["product_code"].ToString().Trim(), "@product_code");
                                myDataCashSource.Add_SP_Parm(row1["currency_code"].ToString().Trim(), "@currency_code");
                                myDataCashSource.Add_SP_Parm(row1["company_code"].ToString().Trim(), "@company_code");
                                myDataCashSource.Add_SP_Parm(row1["gl_center"].ToString().Trim(), "@gl_center");
                                myDataCashSource.Add_SP_Parm(row1["gl_acct"].ToString().Trim(), "@gl_acct");
                                myDataCashSource.Add_SP_Parm(row1["geography"].ToString().Trim(), "@geography");
                                myDataCashSource.Add_SP_Parm(row1["gl_product"].ToString().Trim(), "@gl_product");
                                //myDataCashSource.Add_SP_Parm(arrSource[10], "@user_id");
                                myDataCashSource.Add_SP_Parm(row1["seq_code"].ToString().Trim(), "@apply_to_seq");
                                //TODO: don't know what to do with division at this point. It has been eliminated most places
                                myDataCashSource.Add_SP_Parm("00", "@interdivision");
                                myDataCashSource.Add_SP_Parm(ErrMsg, "@error_message", true);
                                myDataCashSource.Add_SP_Parm(AdjType, "@adjustment_type_id");
                                if (myDataCashSource.NonQuerrySqlSp("usp_ins_adj_cash_adjustment_detail"))
                                {
                                    if (myDataCashSource.OutPutValues.Count() != 0)
                                    {
                                        foreach (var outputVal in myDataCashSource.OutPutValues)
                                        {
                                            ErrMsg = myDataCashSource.OutPutValues["@error_message"].ToString();
                                            if (ErrMsg != "")
                                            {
                                                //sp failed
                                                //myDataCashSource.Rollback();
                                                return false;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //sp failed
                                    ErrMsg = "Stored Procedure usp_ins_adj_cash_adjustment_detail failed to execute for document : " + arrSource[0].Trim();
                                    //myDataCashSource.Rollback();
                                    return false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrMsg = ex.Message;
                            //myDataCashDestination.Rollback();
                            return false;
                        }
                    }
                    //For each rec in GridDest call usp_ins_adj_cash_adjustment_detail
                    try
                    {
                        string ApplyToDocId = "";
                        foreach (DataRow row in dsDest.Tables["destination"].Rows)
                        {
                            //Call adj_detail for row in GridSource and the AdjAmt (neg num)/////////////////////
                            myDataCashDestination.Add_SP_Parm(NewAdjId, "@adj_document_id");
                            if (row["document_id"] == null) ApplyToDocId = "";
                            else ApplyToDocId = row["document_id"].ToString().Trim();
                            myDataCashDestination.Add_SP_Parm(row["document_id"].ToString().Trim(), "@apply_to_doc");
                            myDataCashDestination.Add_SP_Parm(row["receivable_account"].ToString().Trim(), "@receivable_account");
                            myDataCashDestination.Add_SP_Parm(row["amt_to_adjust"].ToString().Trim(), "@amt_to_adjust");
                            myDataCashDestination.Add_SP_Parm(row["product_code"].ToString().Trim(), "@product_code");
                            myDataCashDestination.Add_SP_Parm(row["currency_code"].ToString().Trim(), "@currency_code");
                            myDataCashDestination.Add_SP_Parm(row["company_code"].ToString().Trim(), "@company_code");
                            myDataCashDestination.Add_SP_Parm("", "@gl_center");
                            myDataCashDestination.Add_SP_Parm("", "@gl_acct");
                            myDataCashDestination.Add_SP_Parm("", "@geography");
                            myDataCashDestination.Add_SP_Parm("", "@gl_product");
                            //myDataCashDestination.Add_SP_Parm(row["exchange_rate"].ToString().Trim(), "@exchange_rate");
                            //don't know what to do with division at this point
                            myDataCashDestination.Add_SP_Parm("0000", "@interdivision");
                            myDataCashDestination.Add_SP_Parm(row["seq_code"].ToString().Trim(), "@apply_to_seq");
                            myDataCashDestination.Add_SP_Parm("", "@error_message", true);
                            myDataCashDestination.Add_SP_Parm(AdjType, "@adjustment_type_id");
                            //TODO: Add this in ASAP
                            //myDataCashDestination.Add_SP_Parm(arrSource[10], "@user_id");
                            if (myDataCashDestination.NonQuerrySqlSp("usp_ins_adj_cash_adjustment_detail"))
                            {
                                if (myDataCashDestination.OutPutValues.Count() != 0)
                                {
                                    foreach (var outputVal in myDataCashDestination.OutPutValues)
                                    {
                                        ErrMsg = myDataCashDestination.OutPutValues["@error_message"].ToString();
                                    }
                                    if (ErrMsg != "")
                                    {
                                        //sp failed
                                        //myDataCashDestination.Rollback();
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                //sp failed
                                ErrMsg = "Stored Procedure usp_ins_adj_cash_adjustment_detail failed to execute for document";
                                //myDataCashDestination.Rollback();
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrMsg = ex.Message;
                        //myDataCashDestination.Rollback();
                        return false;
                    }
                    //myDataCashDestination.Commit();
                    return true;
                }
            }
            else
            {
                ErrMsg = "DB Connection Failure has Occurred, Save Failed";
                return false;
            }

        }

        public string GetCustomerName(string CustId)
        {
            string outVal = "";
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(CustId, "@receivable_account");

                if (myData.SqlSpPopDt("usp_sel_cust_name"))
                {
                    foreach (DataRow row in myData.GetDataTable.Rows)
                    {
                        outVal = row[0].ToString();
                    }
                }
            }
            return outVal;
        }

        public bool DeleteUnit(int unit_id)
        {
            bool retVal = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(unit_id, "@unit_id");

                if (myData.NonQuerrySqlSp("usp_del_unit")) { retVal = true; }
            }
            return retVal;
        }

        public bool UpdateUnitAmount(int unit_id, decimal amount)
        {
            bool retVal = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(unit_id, "@unit_id");
                myData.Add_SP_Parm(amount, "@amount");

                if (myData.NonQuerrySqlSp("usp_upd_unit_amount")) { retVal = true; }
            }
            return retVal;
        }

        /// <summary>
        /// 2/24/15 RES Service for updating salesperson assignments.
        /// </summary>
        public bool UpdateSalesPerson(int contract_salesperson_id, int salesperson_id, string first_name, string last_name, string title, int contract_id, 
                                          string contract_description, string product_code, int new_salesperson_id)
        {
            bool retVal = false;
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);

                myData.Add_SP_Parm(contract_salesperson_id, "@contract_salesperson_id");
                myData.Add_SP_Parm(salesperson_id, "@salesperson_id");
                myData.Add_SP_Parm(first_name, "@first_name");
                myData.Add_SP_Parm(last_name, "@last_name");
                myData.Add_SP_Parm(title, "@title");
                myData.Add_SP_Parm(contract_id, "@contract_id");
                myData.Add_SP_Parm(contract_description, "@contract_description");
                myData.Add_SP_Parm(product_code, "@product_code");
                myData.Add_SP_Parm(new_salesperson_id, "@new_salesperson_id");

                if (myData.NonQuerrySqlSp("usp_upd_salesperson_assignment")) { retVal = true; }
            }
            return retVal;
        }

        /// <summary>
        /// Service for returning the file location of attachments for document ids passed in.
        /// </summary>
        /// <param name="DocumentIDs">String list of document ids</param>
        /// <returns></returns>
        public string[] GetAttachmentsForEmail(string[] DocumentIDs)
        {
            //Create new string list to save file locations in
            List<string> FileList = new List<string>();
            
            DataTable dtWork = new DataTable();

            if (InitDB())
            {
                cLASERBaseTable sTable = new cLASERBaseTable(ref SQLCA);
                //Loop through the document ids passed in and run the sp to receive the location.
                foreach (string s in DocumentIDs)
                {
                    //Add the document ID to the parameter list
                    sTable.Add_SP_Parm(s, "@document_id");
                    //Verify that the SQL ran successfully - Currently ignoring error row if one was found.
                    if (sTable.SqlSpPopDt("usp_sel_attachment"))
                    {
                        dtWork = sTable.GetDataTable;
                        //Verify that at least one row was returned
                        //This code will only grab the first document file location if more than one is returned from the query
                        //Can easily be modified to loop through and grab all or last or ... if required.
                        if (dtWork.Rows.Count > 0)
                        {
                            FileList.Add(dtWork.Rows[0]["server_loc"].ToString() + dtWork.Rows[0]["directory"].ToString() + "\\" + dtWork.Rows[0]["file_name"].ToString());
                        }
                    }
                }
            }

            return FileList.ToArray<string>();
        }

        /// <summary>
        /// Copies selected attachments to new location
        /// </summary>
        /// <param name="DocumentIDs">String list of document ids</param>
        /// <returns></returns>
        public bool CopyAttachments(string[] AttachmentIDs, string DocId, string UserName, ref string[] Errors)
        {
            bool retVal = true;
            List<string> FailList = new List<string>();
            if (InitDB())
            {
                cLASERBaseTable myData = new cLASERBaseTable(ref SQLCA);
                foreach (string Id in AttachmentIDs)
                {
                    string Error = "";
                    myData.Add_SP_Parm(Id, "@attachment_id");
                    myData.Add_SP_Parm(DocId, "@document_id");
                    myData.Add_SP_Parm(UserName, "@user_id");
                    myData.Add_SP_Parm(Error, "@error_message", true);

                    if (myData.NonQuerrySqlSp("usp_ins_copy_attachment")) 
                    {
                        if (Error != "")
                        {
                            //sp failed, write AttachmentId that failed to copy to Errors array and continue
                            FailList.Add(Id);
                        }
                    }
                }
                if (FailList.Count > 0)
                {
                    Errors = FailList.ToArray();
                }
            }
            return retVal;
          
        }

        public bool ReprintCustomInvoice(string invoiceNumber)
        {
            if (InitDB())
            {
                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    cBilling billing = new cBilling(SQLCA);
                    return billing.ReprintCustomInvoice(invoiceNumber);
                }
            }
            return false;
        }
    }

}
