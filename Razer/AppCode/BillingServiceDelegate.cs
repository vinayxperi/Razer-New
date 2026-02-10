using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RazerWS;
using RazerInterface;
using System.Data;

namespace Razer.AppCode
{
    class BillingServiceDelegate : IBillingService
    {
        private BillingServiceLocal.BillingServiceClient billingSrvc = new BillingServiceLocal.BillingServiceClient();

        public BillingServiceDelegate()
        {
            var env = System.Configuration.ConfigurationManager.AppSettings["env"].ToString();
            StringBuilder builder = new StringBuilder("svcURLBillingService");
            builder.Append(env);
            var svcURL = System.Configuration.ConfigurationManager.AppSettings[builder.ToString()].ToString();
            billingSrvc.Endpoint.Address = new System.ServiceModel.EndpointAddress(svcURL);
        }


        public DataSet GetCountValues(DataTable dtParms)
        {
            return billingSrvc.GetCountValues(dtParms);
        }

        public string UpdateBillingLocations(int batchID, string companyCode)
        {
            return billingSrvc.UpdateBillingLocations(batchID, companyCode);
        }


        public bool ScheduleJob(string UserID, string JobName, string JobParms, DateTime NextRunDate, string DistributionID)
        {
            return billingSrvc.ScheduleJob(UserID, JobName, JobParms, NextRunDate, DistributionID);
        }

        public bool GenericSQL(string SQL)
        {
            return billingSrvc.GenericSQL(SQL);
        }


        public bool ReprintNationalAdInvoice(string invoiceNumber, bool IsRevised, bool IsSaved, string printer)
        {
            return billingSrvc.ReprintNationalAdInvoice(invoiceNumber, IsRevised, IsSaved, printer);
        }


        public int InsertRemitEntry(int batchid, int RemitID, string DocumentID, DateTime RemitDate, string RemitNumber, decimal Amount, decimal AmountFunctional, string CurrencyCode, double ExchangeRate, decimal BankCharge)
        {
            return billingSrvc.InsertRemitEntry(batchid, RemitID, DocumentID, RemitDate, RemitNumber, Amount, AmountFunctional, CurrencyCode, ExchangeRate, BankCharge);

        }

        public void InsertAllocEntry(int BatchID, int RemitID, string ApplyToDoc, int Seq, decimal Amount, string CurrencyCode, string ReceivableAcct, string ProductCode, int Unapplied)
        {
            billingSrvc.InsertAllocEntry(BatchID, RemitID, ApplyToDoc, Seq, Amount, CurrencyCode, ReceivableAcct, ProductCode, Unapplied);

        }

        public int InsertCashEntry(int SourceID, DateTime BatchDate, int BankID, decimal BatchTotal, string CurrencyCode,DateTime AcctPeriod)
        {
            // 
            return billingSrvc.InsertCashEntry(SourceID, BatchDate, BankID, BatchTotal, CurrencyCode, AcctPeriod);
        }

        public int InsertNewRatefromRateCopy(int RateID, string EndDate, int NewStatus, string UserID)
        {
            return billingSrvc.InsertNewRatefromRateCopy(RateID, EndDate, NewStatus, UserID);
            //return billingSrvc.InsertNewRatefromRateCopy(
            //throw new NotImplementedException();
        }
//C:\Users\rsims\Documents\Visual Studio 2010\Projects\Razer\Razer\Razer\AppCode\BillingServiceDelegate.cs
        public bool DeleteRatefromRateCopy(int RateID)
        {
            return billingSrvc.DeleteRatefromRateCopy(RateID);
             
        }

        public string GetNextInvoiceNumber(string RemitType)
        {
            // return billingSrvc.ReprintNationalAdInvoice(invoiceNumber, IsRevised, IsSaved, printer);
            return billingSrvc.GetNextInvoiceNumber(RemitType);
        }

        public string NationalAdsAdjustment(DataSet NationalAdsAdjustmentDS)
        {
            return billingSrvc.NationalAdsAdjustment(NationalAdsAdjustmentDS);
        }

        public bool DeleteBatch(int BatchID)
        {
            return billingSrvc.DeleteBatch(BatchID);
        }

        public bool DeleteBCF(string BCFNumber)
        {
            return billingSrvc.DeleteBCF(BCFNumber);

        }

        public bool PermDeleteBatch(int BatchID)
        {
           return billingSrvc.PermDeleteBatch(BatchID);
           
        }

        public bool DeleteContractfromBatch(int BatchID, int ContractID)
        {
            return billingSrvc.DeleteContractfromBatch(BatchID, ContractID);
        }

        public bool DeleteContractfromCreateBatch(int BatchID, int ContractID)
        {
            return billingSrvc.DeleteContractfromCreateBatch(BatchID, ContractID);
        }

        public bool DeleteAdjustment(string DocumentID)
        {
            return billingSrvc.DeleteAdjustment(DocumentID);
            
            //throw new NotImplementedException();
        }
        public string UnitUpload(DataSet Uploads, string UserID)
        {
            return billingSrvc.UnitUpload(Uploads, UserID);
        }

        public string AncillaryUnitUpload(DataSet Uploads, string UserID)
        {
            return billingSrvc.AncillaryUnitUpload(Uploads, UserID);
        }

        public int InsertUnitEntry(int ContractID, int ReportID, int MsoID, int CSID, int UnitTypeID, DateTime PeriodStart, DateTime PeriodEnd, int UnitType, string ProductCode, int Estimated, decimal amount)
        {
            return billingSrvc.InsertUnitEntry(ContractID, ReportID, MsoID, CSID, UnitTypeID, PeriodStart, PeriodEnd, UnitType, ProductCode, Estimated, amount);
        }
        public void InsertUnitMetaData(int UnitID, int MetaDataID, int FkeyID)
        {
            billingSrvc.InsertUnitMetaData(UnitID, MetaDataID, FkeyID);
        }
        public bool AdjustmentStatusUpdate()
        {
            return billingSrvc.AdjustmentStatusUpdate();
        }

        public bool DeleteCustomInvoiceDetail(string invoiceNumber, int detailId)
        {
            return billingSrvc.DeleteCustomInvoiceDetail(invoiceNumber, detailId);
        }

        public bool UpdateCustomInvoiceDetail(DataSet detailData)
        {
            return billingSrvc.UpdateCustomInvoiceDetail(detailData);
        }

        public string InsertNewAdjustmentPreamble(string DocToAdjust, int AdjTypeID, string UserID, decimal Amount)
        {
            return billingSrvc.InsertNewAdjustmentPreamble(DocToAdjust, AdjTypeID, UserID, Amount);
        }

        public bool DeleteNewAdjusmentPreamble(string Document_ID)
        {
            return billingSrvc.DeleteNewAdjusmentPreamble(Document_ID);
        }

        public bool ClearExceptions(int typeID)
        {
            return billingSrvc.ClearExceptions(typeID);
        }


        public string WorkflowMultiApproval(DataTable dtToPass, string UserID)
        {
            return billingSrvc.WorkflowMultiApproval(dtToPass, UserID);
        }


        public bool EndJobonMonitor(int jobID)
        {
            return billingSrvc.EndJobonMonitor(jobID);
        }

        public string InsertNewAdjustmentUACash(string CustNum, string AdjAmt, string ProdCode, string UserID, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, ref string ErrMsg)
        {
            return billingSrvc.InsertNewAdjustmentUACash(CustNum, AdjAmt, ProdCode, UserID, CurrencyCode, CompanyCode, CostCtr, Acct, Region, GLProduct, OffsetCompany, OffsetCostCtr, OffsetAcct, OffsetProduct, OffsetRegion, OffsetInterCo, OffsetAmt, ref ErrMsg);
        }

        public bool GetMiscCreditDebitMaximums(ref string MaxNumAdjs, ref string MaxAdjAmt)
        {
            return billingSrvc.GetMiscCreditDebitMaximums(ref MaxNumAdjs, ref MaxAdjAmt);
        }

        public string InsertNewAdjustmentMiscCreditDebit(string CustNum, string AdjAmt, string ProdCode, string UserID, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, string ApplyToDoc, string SeqCode, ref string ErrMsg)
        {
            return billingSrvc.InsertNewAdjustmentMiscCreditDebit(CustNum, AdjAmt, ProdCode, UserID, CurrencyCode, CompanyCode, CostCtr, Acct, Region, GLProduct, OffsetCompany, OffsetCostCtr, OffsetAcct, OffsetProduct, OffsetRegion, OffsetInterCo, OffsetAmt, ApplyToDoc, SeqCode, ref ErrMsg);
        }

        public string InsertNewAdjustmentBillingDiff(string ApplyToDoc,string AdjustmentID,string AdjAmt,string Proforma,string ProformaAmt,string ProformaCurrency,string ConversionDate, string FXRate, string USDAmt, ref string ErrMsg)
        {
            return billingSrvc.InsertNewAdjustmentBillingDiff(ApplyToDoc, AdjustmentID, AdjAmt, Proforma, ProformaAmt, ProformaCurrency, ConversionDate, FXRate, USDAmt, ref ErrMsg);
        }

        public string InsertNewAdjustmentFXCredit(string ApplyToDoc, string CustNum, string AdjAmt, string ProdCode, string UserID, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, ref string ErrMsg)
        {
            return billingSrvc.InsertNewAdjustmentFXCredit(ApplyToDoc, CustNum, AdjAmt, ProdCode, UserID, CurrencyCode, CompanyCode, CostCtr, Acct, Region, GLProduct, OffsetCompany, OffsetCostCtr, OffsetAcct, OffsetProduct, OffsetRegion, OffsetInterCo, OffsetAmt, ref ErrMsg);
        }

        public string InsertNewAdjustmentFXDebit(string ApplyToDoc, string SeqID, string NewDocID, string CustNum, string AdjAmt, string ProdCode, string CurrencyCode, string CompanyCode, string CostCtr, string Acct, string Region, string GLIC, string GLProduct, string OffsetCompany, string OffsetCostCtr, string OffsetAcct, string OffsetProduct, string OffsetRegion, string OffsetInterCo, string OffsetAmt, ref string ErrMsg)
        {
            return billingSrvc.InsertNewAdjustmentFXDebit(ApplyToDoc, SeqID, NewDocID, CustNum, AdjAmt, ProdCode, CurrencyCode, CompanyCode, CostCtr, Acct, Region, GLIC, GLProduct, OffsetCompany, OffsetCostCtr, OffsetAcct, OffsetProduct, OffsetRegion, OffsetInterCo, OffsetAmt, ref ErrMsg);
        }

        public string WorkflowSubmit(string documentId, string userId)
        {
            return billingSrvc.WorkflowSubmit(documentId, userId);
        }

        public string WorkflowApprove(string documentId, string userId)
        {
            return billingSrvc.WorkflowApprove(documentId, userId);
        }

        public DataTable LaunchWorkflowEmail(string documentId, string userId, int status)
        {
            return billingSrvc.LaunchWorkflowEmail(documentId, userId, status);
        }

        public string SendWorkflowEmail(string documentId, string userId, int workflowId, int workflowStatus, string subject, string body, string emailTo)
        {
            return billingSrvc.SendWorkflowEmail(documentId, userId, workflowId, workflowStatus, subject, body, emailTo);
        }

        public bool AddWorkflowApprover(string documentId, string userId, int workflowSequnce, int positionIndicator, string userName)
        {
            return billingSrvc.AddWorkflowApprover(documentId, userId, workflowSequnce, positionIndicator, userName);
        }

        public bool SetInvoiceEmailQueue(string primary_co)
        {
            return billingSrvc.SetInvoiceEmailQueue(primary_co);
        }

        public bool InsertNewCashAdjustment(string[] arrSource, DataSet dsDest, int AdjType, ref string NewAdjId, ref string ErrMsg)
        {
            return billingSrvc.InsertNewCashAdjustment(arrSource, dsDest, AdjType, ref NewAdjId, ref ErrMsg);
        }

        public string GetCustomerName(string CustId)
        {
            return billingSrvc.GetCustomerName(CustId);
        }
        
        public bool DeleteUnit(int unit_id)
        {
            return billingSrvc.DeleteUnit(unit_id);
        }

        public bool UpdateUnitAmount(int unit_id, decimal amount)
        {
            return billingSrvc.UpdateUnitAmount(unit_id, amount);
        }

        public bool UpdateSalesPerson(int contract_salesperson_id, int salesperson_id, string first_name, string last_name, string title, int contract_id,
                                          string contract_description, string product_code, int new_salesperson_id)
        {
            return billingSrvc.UpdateSalesPerson(contract_salesperson_id, salesperson_id, first_name, last_name, title, contract_id,
                                                          contract_description, product_code, new_salesperson_id);
        }

        public string[] GetAttachmentsForEmail(string[] DocumentIDs)
        {
            return billingSrvc.GetAttachmentsForEmail(DocumentIDs); 
        }

        public bool CopyAttachments(string[] AttachmentIDs, string DocId, string UserName, ref string[] Errors)
        {
            return billingSrvc.CopyAttachments(AttachmentIDs, DocId, UserName, ref Errors);
        }

        public bool ReprintCustomInvoice(string invoiceNumber)
        {
            return billingSrvc.ReprintCustomInvoice(invoiceNumber);
            //throw new NotImplementedException();
        }



       
    }
}


