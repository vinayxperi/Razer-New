using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace RazerWS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBillingService" in both code and config file together.
    [ServiceContract]
    public interface IBillingService
    {        
        /// <summary>
        /// Service to receive a datatable containing a row for each request for count data and return the associated count information
        /// 
        /// </summary>
        /// <param name="dtParms">
        ///The dtParms table expects to receive the below columns of information 
        ///dtParms.Columns.Add("filter_id");  -- The filter id to caluclate
        ///dtParms.Columns.Add("service_period_start"); -- The beginning service period
        ///dtParms.Columns.Add("service_period_end"); -- The end date of the service period
        ///dtParms.Columns.Add("unit_period_type"); -- 0 = actual / 1=forecast
        ///dtParms.Columns.Add("entity_id"); -- The entity id being calculated for
        ///dtParms.Columns.Add("total_entity_flag"); -- Calculate for the total entity instead of the location
        ///dtParms.Columns.Add("contract_id"); -- The contract ID to caluclate for
        ///dtParms.Columns.Add("location_id"); -- The location ID to calculate for / 0=All Locations
        ///dtParms.Columns.Add("report_id"); -- The specific report to calculate for 0 = All Reports
        ///dtParms.Columns.Add("query_type"); -- The type of query to run determined by the string passed in. Valid values are:
        ///Report, Entity, Contract, Location
        /// </param>
        /// <returns>
        /// Currently the service returns a dataset containing three tables: Error, FIlterResult and FilterTotal .The Error table that shows 
        /// all error and warning messages received. The FilterResult table contains all of the individual rows of the count filter and 
        /// how they each calculated, (this table is for debug purposes and will probably be removed when this is made live.  
        /// The FilterTotal table returns the total amount for each filter requested.
        /// </returns>
        [OperationContract]
        DataSet GetCountValues(DataTable dtParms);
        
        /// <summary>
        /// Service to update billing location table to include all billing data for the selected group
        /// If no specific batch or company is provided, then the billing location is updated for all contracts
        /// If both company and batch is provided, then update runs for the batch only.
        /// </summary>
        /// <param name="batchID">The ID of the batch to update the locations for / 0=Not for a batch</param>
        /// <param name="companyCode">The 2 character company code of the company to run for - Empty string means it is not for a specific company</param>
        /// <returns>empty string means the job ran successfully - A value in the string represents any error message received from running the service</returns>
        [OperationContract]
        string UpdateBillingLocations(int batchID, string companyCode);

        [OperationContract]
        bool ScheduleJob(string UserID, string JobName, string JobParms, DateTime NextRunDate, string DistributionID);

        [OperationContract]
        bool GenericSQL(string SQL);

        [OperationContract]
        bool ReprintNationalAdInvoice(string invoiceNumber, bool IsRevised, bool IsSaved, string printer);

        [OperationContract]
        int InsertRemitEntry(int batchid, int RemitID, string DocumentID, DateTime RemitDate, string RemitNumber, decimal Amount, decimal AmountFunctional, string CurrencyCode, Double ExchangeRate, decimal BankCharge);

        [OperationContract]
        void InsertAllocEntry(int BatchID, int RemitID, string ApplyToDoc, int Seq, decimal Amount, string CurrencyCode, string ReceivableAcct, string ProductCode, int Unapplied);

        [OperationContract]
        int InsertCashEntry(int SourceID, DateTime BatchDate, int BankID, decimal BatchTotal, string CurrencyCode,DateTime AcctPeriod);
       
        [OperationContract]
        int InsertUnitEntry(int ContractID, int ReportID, int MsoID, int CSID, int UnitTypeID, DateTime PeriodStart, DateTime PeriodEnd, int UnitType, string ProductCode, int Estimated, decimal amount);

        [OperationContract]
        void InsertUnitMetaData(int UnitID, int MetaDataID, int FkeyID);

        [OperationContract]
        int InsertNewRatefromRateCopy(int RateID, string EndDate, int NewStatus, string UserID);

        [OperationContract]
        bool DeleteRatefromRateCopy(int BatchID);

        [OperationContract]
        string GetNextInvoiceNumber(string RemitType);

        [OperationContract]
        string NationalAdsAdjustment(DataSet NationalAdsAdjustmentDS);

        [OperationContract]
        bool DeleteBatch(int BatchID);

        [OperationContract]
        bool DeleteBCF(string BCFNumber);

        [OperationContract]
        bool PermDeleteBatch(int BatchID);

        [OperationContract]
        bool DeleteAdjustment(string DocumentID);

        [OperationContract]
        bool DeleteContractfromBatch(int BatchID, int ContractID);

        [OperationContract]
        bool DeleteContractfromCreateBatch(int BatchID, int ContractID);

        [OperationContract]
        string UnitUpload(DataSet Uploads, String UserID);

        [OperationContract]
        string AncillaryUnitUpload(DataSet Uploads, String UserID);

        [OperationContract]
        bool AdjustmentStatusUpdate();

        [OperationContract]
        bool DeleteCustomInvoiceDetail(string invoiceNumber, int detailId);

        [OperationContract]
        bool UpdateCustomInvoiceDetail(DataSet detailData);

        [OperationContract]
        string InsertNewAdjustmentPreamble(string DocToAdjust, int AdjTypeID, string UserID, decimal Amount);

        [OperationContract]
        bool DeleteNewAdjusmentPreamble(string Document_ID);

        [OperationContract]
        bool ClearExceptions(int typeID);

        [OperationContract]
        bool EndJobonMonitor(int jobID);

        [OperationContract]
        string WorkflowMultiApproval(DataTable dtToPass, string UserID);

        [OperationContract]
        string InsertNewAdjustmentUACash(string CustNum, string AdjAmt, string ProdCode, string UserID, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, ref string ErrMsg);

        [OperationContract]
        bool GetMiscCreditDebitMaximums(ref string MaxNumAdjs, ref string MaxAdjAmt);

        [OperationContract]
        string InsertNewAdjustmentMiscCreditDebit(string CustNum, string AdjAmt, string ProdCode, string UserID, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, string ApplyToDoc, string SeqCode, ref string ErrMsg);

        [OperationContract]
        string InsertNewAdjustmentBillingDiff(string ApplyToDoc, string AdjustmentID, string AdjAmt, string Proforma, string ProformaAmt, string ProformaCurrency, string ConversionDate, string FXRate, string USDAmt, ref string ErrMsg);

        [OperationContract]
        string InsertNewAdjustmentFXCredit(string ApplyToDoc, string CustNum, string AdjAmt, string ProdCode, string UserID, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, ref string ErrMsg);
  
        [OperationContract]
        string InsertNewAdjustmentFXDebit(string ApplyToDoc, string SeqID, string NewDocID, string CustNum, string AdjAmt, string ProdCode, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLIC, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, ref string ErrMsg);
 
        [OperationContract]
        string WorkflowSubmit(string documentId, string userId);

        [OperationContract]
        string WorkflowApprove(string documentId, string userId);

        [OperationContract]
        DataTable LaunchWorkflowEmail(string documentId, string userId, int status);

        [OperationContract]
        string SendWorkflowEmail(string documentId, string userId, int workflowId, int workflowStatus, string subject, string body, string emailTo);

        [OperationContract]
        bool AddWorkflowApprover(string documentId, string userId, int workflowSequnce, int positionIndicator, string userName);

        [OperationContract]
        bool SetInvoiceEmailQueue(string primary_co);

        [OperationContract]
        bool InsertNewCashAdjustment(string[] arrSource, DataSet dsDest, int AdjType, ref string NewAdjId, ref string ErrMsg);

        [OperationContract]
        string GetCustomerName(string CustId);

        [OperationContract]
        bool DeleteUnit(int unit_id);

        [OperationContract]
        bool UpdateUnitAmount(int unit_id, decimal amount);

        [OperationContract]
        bool UpdateSalesPerson(int contract_salesperson_id, int salesperson_id, string first_name, string last_name, string title, int contract_id,
                               string contract_description, string product_code, int new_salesperson_id);

        [OperationContract]
        string[] GetAttachmentsForEmail(string[] DocumentIDs);

        [OperationContract]
        bool CopyAttachments(string[] AttachmentIDs, string DocId, string UserName, ref string[] Errors);

        [OperationContract]
        bool ReprintCustomInvoice(string invoiceNumber);

    }
}